using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using PlayerIOClient;

// This code has been licensed by realmaster under the MIT license.
// dont forget gabrielo!

namespace ebbbot
{
    class Program
    {
        public static string bbb = "1.1",
            onInputFunction = "";
        public static bool console = true;
        public static Dictionary<string, string> vars = new Dictionary<string, string>();
        public static Dictionary<string, string> nvars = new Dictionary<string, string>();
        public static Dictionary<string, Client> clients = new Dictionary<string, Client>();
        public static Dictionary<string, Connection> connections = new Dictionary<string, Connection>();
        public static int d = -999;
        public static List<string[]> sayQueue = new List<string[]>();
        public static System.Threading.Thread queuer;
        public static List<Exception> errors = new List<Exception>();

        public static void SayQueuer()
        {
            if (sayQueue.Count > 0)
            {
                string[] sq = sayQueue[0];
                sayQueue.RemoveAt(0);
                try
                {
                    connections[sq[0]].Send("say", sq[1]);
                }
                catch (Exception x)
                {
                    errors.Add(x);
                }
            }

            System.Threading.Thread.Sleep(655);
        }

        public static bool isParam(string t)
        {
            try
            {
                int abc = int.Parse(t.Replace("*", ""));
                return true;
            }
            catch (Exception x)
            {
                errors.Add(x);
                return false;
            }
        }
        public static string m(string msg, List<string> paramss)
        {
            string upd = msg;
            for (int i = 0; i < paramss.Count; i++)
                upd = upd.Replace("*" + (i + 1).ToString(), paramss[i]);

            foreach (string nv in nvars.Keys)
                if (upd.Contains(nv))
                    upd = upd.Replace(nv, nvars[nv]);

            while (upd.IndexOf("=") > -1)
            {
                bool first = false;
                int fin = 0;
                bool f = false;

                for (int i = 0; i < upd.Length; i++)
                    if (upd[i] == '=' && first)
                    {
                        f = true;
                        string updS = "";

                        if (fin != 0)
                        {
                            for (int x = 0; x < fin; x++)
                                updS += (upd[x]);
                        }

                        string varp = "";
                        for (int e = (fin + 1); e < i; e++)
                            varp += (upd[e]);

                        updS += vars[varp];

                        if (i != upd.Length)
                            for (int x = (i + 1); x < upd.Length; x++)
                                updS += upd[x];

                        upd = updS;

                        first = false;
                        f = false;
                        fin = 0;
                    }
                    else if (upd[i] == '=')
                    {
                        fin = i;
                        first = true;
                    }

                if (!f)
                    break;
            }

            return upd;
        }
        public static void compile_read(string code, string function = "", List<string> paramss = null)
        {
            #region compile
            List<List<string>> orders = EEBotBuilder.getInstructionText(code);
            if (paramss == null)
                paramss = new List<string>() { };

            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i][0].StartsWith("function:"))
                {
                    string[] t = orders[i][0].Split(':');
                    string t2 = t[1];
                    if (t2.Contains("\n"))
                        t2 = t2.Substring(0, t2.IndexOf("\n"));

                    string newt = "";
                    if (orders[i][0].Split('\n').Length > 1)
                        for (int ie = 1; ie < orders[i][0].Split('\n').Length; ie++)
                            newt += (orders[i][0].Split('\n')[ie] + '\n');

                    vars[t2] = newt;
                }
                else if (orders[i][0].StartsWith("var:"))
                {
                    string[] test = orders[i][0].Split(':');
                    string n = m(test[1], paramss);

                    string v = "";

                    for (int xx = 2; xx < test.Length; xx++)
                        v += test[xx] + ":";

                    v = v.Substring(0, v.Length - 1);
                    v = m(v, paramss);

                    if (v == "?ask" && console)
                        v = Console.ReadLine();
                    else if (v == "?askyes" && console)
                    {
                        string r = Console.ReadLine().ToLower();
                        if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                            v = "yes";
                        else
                            v = "no";
                    }

                    vars[n] = v;
                }

                if (orders[i][0].StartsWith("log "))
                    Console.WriteLine(m(orders[i][0].Substring(4), paramss) + '\n');
                else if (orders[i][0].StartsWith("bot "))
                {
                    vars[m(orders[i][0].Substring(4), paramss)] = "Client";
                    vars[m(orders[i][0].Substring(4), paramss) + "_email"] = "";
                    vars[m(orders[i][0].Substring(4), paramss) + "_pass"] = "";
                }
                else if (orders[i][0].StartsWith("userinput "))
                {
                    string f = orders[i][0].Substring(10);
                    onInputFunction = m(f, paramss);
                }
                else if (orders[i][0].StartsWith("botemail ") || orders[i][0].StartsWith("bottoken "))
                {
                    if (orders[i][0].Split(' ')[2] == "?ask" && console)
                        orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " " + Console.ReadLine());
                    else if (orders[i][0].Split(' ')[2] == "?askyes" && console)
                    {
                        string r = Console.ReadLine().ToLower();
                        if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                            orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                        else
                            orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                    }

                    vars[m(orders[i][0].Split(' ')[1], paramss) + "_email"] = m(orders[i][0].Split(' ')[2], paramss);
                }
                else if (orders[i][0].StartsWith("botpass "))
                {
                    if (orders[i][0].Split(' ')[2] == "?ask" && console)
                        orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " " + Console.ReadLine());
                    else if (orders[i][0].Split(' ')[2] == "?askyes" && console)
                    {
                        string r = Console.ReadLine().ToLower();
                        if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                            orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                        else
                            orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                    }

                    vars[m(orders[i][0].Split(' ')[1], paramss) + "_pass"] = m(orders[i][0].Split(' ')[2], paramss);
                }
                else if (orders[i][0].StartsWith("botlogin "))
                {
                    clients[m(orders[i][0].Split(' ')[1], paramss)] = PlayerIO.QuickConnect.SimpleConnect(
                        "everybody-edits-su9rn58o40itdbnw69plyw",
                        vars[m(orders[i][0].Split(' ')[1], paramss) + "_email"],
                        vars[m(orders[i][0].Split(' ')[1], paramss) + "_pass"],
                        null);
                }
                else if (orders[i][0].StartsWith("botdisconnect "))
                {
                    connections[m(orders[i][0].Split(' ')[1], paramss)].Disconnect();
                    connections.Remove(m(orders[i][0].Split(' ')[1], paramss));
                }
                else if (orders[i][0].StartsWith("botdisconnected "))
                {
                    string[] splitted = orders[i][0].Split(' ');

                    connections[m(splitted[1], paramss)].OnDisconnect += new DisconnectEventHandler(delegate(object sender, string reasson)
                    {
                        try
                        {
                            compile_read(vars[m(splitted[2], paramss)], m(splitted[2], paramss),
                                new List<string>() { reasson, "", "", "", "", "" });
                        }
                        catch (Exception x)
                        {
                            errors.Add(x);
                        }
                    });
                }
                else if (orders[i][0].StartsWith("botlogout "))
                {
                    if (connections.ContainsKey(m(orders[i][0].Split(' ')[1], paramss)))
                    {
                        connections[m(orders[i][0].Split(' ')[1], paramss)].Disconnect();
                        connections.Remove(m(orders[i][0].Split(' ')[1], paramss));
                    }
                    clients[m(orders[i][0].Split(' ')[1], paramss)].Logout();
                    clients.Remove(m(orders[i][0].Split(' ')[1], paramss));
                }
                else if (orders[i][0].StartsWith("botworld "))
                {
                    if (orders[i][0].Split(' ')[2] == "?ask" && console)
                        orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " " + Console.ReadLine());
                    else if (orders[i][0].Split(' ')[2] == "?askyes" && console)
                    {
                        string r = Console.ReadLine().ToLower();
                        if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                            orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                        else
                            orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                    }

                    vars[m(orders[i][0].Split(' ')[1], paramss) + "_world"] = m(orders[i][0].Split(' ')[2], paramss);
                }
                else if (orders[i][0].StartsWith("botjoin "))
                {
                    bool ready = false;
                    clients[m(orders[i][0].Split(' ')[1], paramss)].BigDB.Load("config", "config", delegate(DatabaseObject o)
                    {
                        connections[m(orders[i][0].Split(' ')[1], paramss)] = clients[m(orders[i][0].Split(' ')[1], paramss)].Multiplayer.CreateJoinRoom(
                            vars[m(orders[i][0].Split(' ')[1], paramss) + "_world"],
                            "Everybodyedits" + o.GetInt("version").ToString(),
                            true,
                            null,
                            null);
                        string derp = m(orders[i][0].Split(' ')[1], paramss);
                        connections[derp].OnMessage += new PlayerIOClient.MessageReceivedEventHandler(delegate(object sender, PlayerIOClient.Message msg)
                        {
                            try
                            {
                                switch (msg.Type)
                                {
                                    case "init":
                                        nvars["*owner"] = msg.GetString(0);
                                        nvars["*worldname"] = msg.GetString(1);
                                        nvars["*plays"] = msg.GetInt(2).ToString();
                                        nvars["*favorites"] = msg.GetInt(3).ToString();
                                        nvars["*likes"] = msg.GetInt(4).ToString();
                                        nvars["*myid"] = msg.GetInt(5).ToString();
                                        nvars["*smiley"] = msg.GetInt(6).ToString();
                                        nvars["*aurashape"] = msg.GetInt(7).ToString();
                                        nvars["*auracolor"] = msg.GetInt(8).ToString();
                                        nvars["*goldborder"] = msg.GetBoolean(9).ToString();
                                        nvars["*spawnx"] = msg.GetDouble(10).ToString();
                                        nvars["*spawny"] = msg.GetDouble(11).ToString();
                                        nvars["*chatcolor"] = msg.GetUInt(12).ToString();
                                        nvars["*myname"] = msg.GetString(13).ToString();
                                        nvars["*canedit"] = msg.GetBoolean(14).ToString();
                                        nvars["*isowner"] = msg.GetBoolean(15).ToString();
                                        nvars["*favorited"] = msg.GetBoolean(16).ToString();
                                        nvars["*liked"] = msg.GetBoolean(17).ToString();
                                        nvars["*worldwidth"] = msg.GetInt(18).ToString();
                                        nvars["*worldheight"] = msg.GetInt(19).ToString();
                                        nvars["*background"] = msg.GetUInt(20).ToString();

                                        if (nvars["*goldborder"] == "true")
                                            nvars["*goldborder"] = "yes";
                                        else
                                            nvars["*goldborder"] = "no";

                                        if (nvars["*canedit"] == "true")
                                            nvars["*canedit"] = "yes";
                                        else
                                            nvars["*canedit"] = "no";

                                        if (nvars["*isowner"] == "true")
                                            nvars["*isowner"] = "yes";
                                        else
                                            nvars["*isowner"] = "no";

                                        if (nvars["*favorited"] == "true")
                                            nvars["*favorited"] = "yes";
                                        else
                                            nvars["*favorited"] = "no";

                                        if (nvars["*liked"] == "true")
                                            nvars["*liked"] = "yes";
                                        else
                                            nvars["*liked"] = "no";

                                        if (vars.ContainsKey(derp + "_event_botjoined"))
                                            compile_read(vars[derp + "_event_botjoined"], derp + "_event_botjoined", new List<string>() { 
                                                nvars["*owner"],
                                                nvars["*worldname"],
                                                nvars["*plays"],
                                                nvars["*favorites"],
                                                nvars["*likes"],
                                                nvars["*myid"],
                                                nvars["*smiley"],
                                                nvars["*aurashape"],
                                                nvars["*auracolor"],
                                                nvars["*goldborder"],
                                                nvars["*spawnx"],
                                                nvars["*spawny"],
                                                nvars["*chatcolor"],
                                                nvars["*myname"],
                                                nvars["*canedit"],
                                                nvars["*isowner"],
                                                nvars["*favorited"],
                                                nvars["*liked"],
                                                nvars["*worldwidth"],
                                                nvars["*worldheight"],
                                                nvars["*background"]});
                                        connections[derp].Send("init2");
                                        break;
                                    case "add":
                                        if (vars.ContainsKey(derp + "_event_plrjoined"))
                                            compile_read(vars[derp + "_event_plrjoined"], derp + "_event_plrjoined", new List<string>() { msg.GetInt(0).ToString(), msg.GetString(1), msg.GetString(2), msg.GetInt(3).ToString(), 
                                                    msg.GetDouble(4).ToString(), msg.GetDouble(5).ToString(), 
                                                    (msg.GetBoolean(6) == true ? "yes" : "no"),
                                                    (msg.GetBoolean(7) == true ? "yes" : "no"),
                                                    (msg.GetBoolean(8) == true ? "yes" : "no"),
                                                    msg.GetInt(9).ToString(),
                                                    msg.GetInt(10).ToString(),
                                                    msg.GetInt(11).ToString(),
                                                    (msg.GetBoolean(12) == true ? "yes" : "no"),
                                                    (msg.GetBoolean(13) == true ? "yes" : "no"),
                                                    (msg.GetBoolean(14) == true ? "yes" : "no"),
                                                    (msg.GetBoolean(15) == true ? "yes" : "no"),
                                                    msg.GetInt(16).ToString(),
                                                    msg.GetInt(17).ToString(),
                                                    msg.GetInt(18).ToString(),
                                                    msg.GetInt(19).ToString(),
                                                    msg.GetString(20),
                                                    (msg.GetBoolean(21) == true ? "yes" : "no")});
                                        break;
                                    case "left":
                                        if (vars.ContainsKey(derp + "_event_plrleft"))
                                            compile_read(vars[derp + "_event_plrleft"], derp + "_event_plrleft", new List<string>() { msg.GetInt(0).ToString() });
                                        break;
                                }
                            }
                            catch (Exception x)
                            {
                                errors.Add(x);
                            }
                        });
                        connections[m(orders[i][0].Split(' ')[1], paramss)].Send("init");

                        ready = true;
                    });

                    while (!ready)
                        d++;
                }
                else if (orders[i][0].StartsWith("bottalk "))
                {
                    string[] sp = orders[i][0].Split(' ');
                    string bot = m(sp[1], paramss);
                    string latter = m(orders[i][0].Substring(9 + bot.Length), paramss);

                    sayQueue.Add(new string[2] { bot, latter });
                }
                else if (orders[i][0].StartsWith("botevent "))
                {
                    string[] sp = orders[i][0].Split(' ');
                    string bot = m(sp[1], paramss);
                    string even = sp[2];
                    int latterInd = (bot.Length + even.Length + 11);
                    string latter = m(orders[i][0].Substring(latterInd), paramss);
                    vars[bot + "_event_" + even] = vars[latter];
                }
                else if (orders[i][0].StartsWith("botfunction ") && orders[i][0].EndsWith("!"))
                {
                    string[] abc = orders[i][0].Split(' ');
                    string f = abc[1];
                    f = f.Substring(0, f.Length - 1);

                    compile_read(vars[f], f);
                }
                else if (orders[i][0].Contains(":") && !orders[i][0].StartsWith("var:"))
                {
                    if (orders[i][0].Split(':')[0].StartsWith("??"))
                    {
                        string abc = orders[i][0].Split(':')[0].Replace("??", "");
                        string v = orders[i][0].Split(':')[1];
                        bool yes = false;

                        if (v != "no" && v != "yes" && v != "*no*" && v != "*yes*")
                            v = "no";

                        if (v.StartsWith("*") && v.EndsWith("*"))
                        {
                            if (v == "*no*")
                                v = "yes";
                            else if (v == "*yes*")
                                v = "no";
                            else
                                v = m(v, paramss);
                        }

                        if (abc.StartsWith("*"))
                        {
                            if (isParam(orders[i][0].Split(':')[0]))
                                yes = true;
                        }
                        if (vars.ContainsKey(abc))
                            yes = true;

                        if (yes)
                        {
                            string newx = "";
                            for (int xx = 2; xx < orders[i][0].Split(':').Length; xx++)
                                newx += (orders[i][0].Split(':')[xx] + ":");

                            newx = newx.Substring(0, newx.Length - 1);
                            compile_read(newx);
                        }
                    }
                    else if (vars.ContainsKey(orders[i][0].Split(':')[0]) || isParam(orders[i][0].Split(':')[0]))
                    {
                        string[] test = orders[i][0].Split(':');
                        string n = m(test[0], paramss);
                        bool inverse = false;

                        if (test[1].StartsWith("*") && test[1].EndsWith("*"))
                            inverse = true;

                        if (test[1].Length == 2 && inverse)
                            test[1] = "";
                        else if (inverse)
                        {
                            test[1] = test[1].Substring(0, test[1].Length - 1);
                            test[1] = test[1].Substring(1);
                        }

                        string v = m(test[1], paramss);

                        if (isParam(orders[i][0].Split(':')[0]))
                        {
                            string tst = orders[i][0].Split(':')[0];
                            int abc = int.Parse(tst.Replace("*", ""));
                            if ((paramss.Count - 1) >= abc)
                            {
                                if (paramss[abc - 1] != v && !inverse)
                                    continue;
                                else if (paramss[abc - 1] == v && inverse)
                                    continue;
                            }
                        }

                        if (!isParam(orders[i][0].Split(':')[0]))
                            if ((vars[n] != v && !inverse) || (vars[n] == v && inverse))
                                continue;

                        string newx = "";
                        for (int xx = 2; xx < test.Length; xx++)
                            newx += (test[xx] + ":");

                        newx = newx.Substring(0, newx.Length - 1);
                        compile_read(newx);
                    }
                }
            }
            #endregion
        }

        static void Main(string[] args)
        {
            if (File.ReadAllText(System.AppDomain.CurrentDomain.FriendlyName).IndexOf("\n*code•") == -1)
            {
                Console.WriteLine("Error! The code source was not found. Cannot compile because code is missing.");
                Console.WriteLine("Please contact the bot's creator concerning this issue.");
                while (true)
                    Console.ReadKey();
            }
            else
            {
                string test = File.ReadAllText(System.AppDomain.CurrentDomain.FriendlyName).Substring(File.ReadAllText(System.AppDomain.CurrentDomain.FriendlyName).IndexOf("\n*code•") + 8);
                string code = test;
                queuer = new System.Threading.Thread(new System.Threading.ThreadStart(SayQueuer));
                queuer.Start();

                compile_read(code);

                while (true)
                {
                    string line = Console.ReadLine();
                    if (onInputFunction != "")
                        compile_read(vars[onInputFunction], onInputFunction, new List<string>() { line, m(line, null), "", "", "", "" });
                }
            }
        }
    }
}
