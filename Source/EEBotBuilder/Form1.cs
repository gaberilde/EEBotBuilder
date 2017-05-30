using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;
using PlayerIOClient;

// This code has been licensed by realmaster under the MIT license.

namespace EEBotBuilder
{
    public partial class Form1 : Form
    {
        Socket s = null;
        bool unsavedChanges = false,
            console = false,
            disconnecting = false,
            outputGet = false;
        string bbb = "1",
            hp = "",
            onInputFunction = "";
        int d = -999;
        List<string[]> sayQueue = new List<string[]>();
        List<List<string>> orders;
        List<string> poss = new List<string>() { "log [message]", "# [comment]", "bot [name]",
            "botemail [name] [email]", "bottoken [name] [token]", "botpass [name] [password]",
            "botlogin [name]", "botworld [name] [worldid]", "botjoin [name]",
            "botevent [name] [event] [botfunction]", "botfunction [name] {[code]}",
            "botfunction [name]!", "botdisconnect [name]", "botlogout [name]", 
            "botdisconnected [name] [botfunction]", "userinput [botfunction]",
            "bottalk [name] [message]"};
        Dictionary<string, string> vars = new Dictionary<string, string>();
        Dictionary<string, string> nvars = new Dictionary<string, string>();
        Dictionary<string, Client> clients = new Dictionary<string, Client>();
        Dictionary<string, Connection> connections = new Dictionary<string, Connection>();
        List<string> lhex = new List<string>();
        List<Exception> errors = new List<Exception>();
        System.Threading.Thread t2 = null;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            hp = new WebClient().DownloadString("http://pastebin.com/raw/hrPBajCc");
            if (hp != bbb)
                MessageBox.Show("Your EE Bot Builder tool is outdated. If you'd wish to have the latest one, please visit realmaster42-projects.weebly.com\nIt is required to use the latest version as some people tend to bypass bans!\n\nYou are using V" + bbb + " while latest one is V" + hp + "!", "EE Bot Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public void SaveText()
        {
            try
            {
                if (File.Exists(@Environment.CurrentDirectory + @"\my_code_" + Directory.GetFiles(@Environment.CurrentDirectory).Length.ToString() + ".txt"))
                    File.Delete(@Environment.CurrentDirectory + @"\my_code_" + Directory.GetFiles(@Environment.CurrentDirectory).Length.ToString() + ".txt");

                File.WriteAllText(@Environment.CurrentDirectory + @"\my_code_" + Directory.GetFiles(@Environment.CurrentDirectory).Length.ToString() + ".txt", richTextBox1.Text);
                unsavedChanges = false;
                button1.Enabled = false;
            }
            catch (Exception x)
            {
                errors.Add(x);
                MessageBox.Show("Failed to save! Please proceed to saving manually. Copy the code and paste it into a text file you may use anytime.", "Save error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (unsavedChanges)
            {
                if (MessageBox.Show("Hold on! You have unsaved changes. Would you like to save before closing?", "HOLD ON!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveText();
                    s.Dispose();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveText();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            unsavedChanges = true;
            button1.Enabled = true;
            string lastword = "";
            bool spacecheck = false;

            if (richTextBox1.Text.Split('\n').Length == 0)
            {
                if (richTextBox1.Text.StartsWith("#"))
                    spacecheck = true;
                else
                    lastword = richTextBox1.Text;
            }
            else if (richTextBox1.Text.Split('\n')[richTextBox1.Text.Split('\n').Length - 1].StartsWith("#"))
                spacecheck = true;

            if (spacecheck)
            {
                if (richTextBox1.Text.Split(' ').Length == 1)
                    lastword = richTextBox1.Text;
                else if (richTextBox1.Text.Split(' ').Length > 1)
                    lastword = (richTextBox1.Text.Split(' ')[richTextBox1.Text.Split(' ').Length - 1]);

                if (richTextBox1.Text.EndsWith(" "))
                    lastword = (richTextBox1.Text.Split(' ')[richTextBox1.Text.Split(' ').Length - 2]);
            }
            else if (lastword == "")
                lastword = (richTextBox1.Text.Split('\n')[richTextBox1.Text.Split('\n').Length - 1]);

            if (lastword.Contains("\n"))
                lastword = lastword.Split('\n')[1];
            if (lastword.Contains("\r"))
                lastword = lastword.Split('\r')[1];

            richTextBox2.Text = "";
            for (int i = 0; i < poss.Count; i++)
                if (poss[i].StartsWith(lastword))
                    richTextBox2.Text += (poss[i] + '\n');
        }
        /*public void userinput(object sender, KeyEventArgs e)
        {
            if (textBox1.Enabled)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (onInputFunction != "")
                        if (onInputFunction != "")
                            compile_read(vars[onInputFunction], onInputFunction, new List<string>() { textBox1.Text, m(textBox1.Text, null), "", "", "", "" });

                    if (!outputGet)
                        textBox1.Text = "";
                }
            }

            e.Handled = true;
        }
        public string enableWaitOutput()
        {
            textBox1.Enabled = true;
            label1.Text = "Press enter once finished typing.";
            textBox1.BackColor = Color.LightGreen;

            int w = -1;
            string answer = "";
            outputGet = true;

            KeyEventHandler c = new KeyEventHandler(delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    answer = textBox1.Text;
                    textBox1.Text = "";
                }

                e.Handled = true;
            });
            textBox1.KeyDown += c;

            while (answer == "")
                w++;

            textBox1.KeyDown -= c;

            outputGet = false;
            label1.Text = "";

            return answer;
        }*/
        private void button3_Click(object sender, EventArgs e)
        {
            if (hp != bbb)
            {
                MessageBox.Show("Your EE Bot Builder tool is outdated. If you'd wish to have the latest one, please visit realmaster42-projects.weebly.com\nIt is required to use the latest version as some people tend to bypass bans!\n\nYou are using V" + bbb + " while latest one is V" + hp + "!", "EE Bot Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }

            if (MessageBox.Show("Are you sure you want to convert your code into an executable file?\nYour code will get obfuscated into a file in the same directory as this application, aswell as the .exe!", "You sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                string ee = EEBotBuilder.findErrors(richTextBox1.Text);
                if (ee != "")
                {
                    MessageBox.Show("The compiler found errors that need fixing. Please fix them before uploading as an .exe!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    richTextBox2.Text = ee;
                }
                else
                {
                    if (File.Exists(@Environment.CurrentDirectory + @"\ebbbot.exe"))
                    {
                        string n = (@Environment.CurrentDirectory + @"\compiled_ebbbot.exe");

                        if (File.Exists(@Environment.CurrentDirectory + @"\compiled_ebbbot.exe"))
                        {
                            n = @Environment.CurrentDirectory + @"\compiled_ebbbot" + Directory.GetFiles(@Environment.CurrentDirectory).Length.ToString().ToString() + ".exe";
                            MessageBox.Show("A required file for compiling 'compiled_ebbbot.exe' already exists in the current directory!\nA new one will now automatically get generated now.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            File.Copy(@Environment.CurrentDirectory + @"\ebbbot.exe", n);
                        }
                        else
                            File.Copy(@Environment.CurrentDirectory + @"\ebbbot.exe", @Environment.CurrentDirectory + @"\compiled_ebbbot.exe");

                        File.AppendAllText(n, "\n*code•\n" + EEBotBuilder.obfuscate(richTextBox1.Text));
                    }
                    else
                        MessageBox.Show("The required .exe ebbbot is missing from the directory this application is. Therefore, it will not save as an executable!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public bool isParam(string t)
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
        public string m(string msg, List<string> paramss)
        {
            string upd = msg;
            if (paramss == null)
                paramss = new List<string>() { };

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
        public void compile_read(string code, string function = "", List<string> paramss = null)
        {
            #region compile
            if (!disconnecting)
            {
                try
                {
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
                            //else if (v == "?ask" && !console)
                            //    v = enableWaitOutput();
                            else if (v == "?askyes" && console)
                            {
                                string r = Console.ReadLine().ToLower();
                                if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                                    v = "yes";
                                else
                                    v = "no";
                            }
                            /*else if (v == "?askyes" && !console)
                            {
                                string r = enableWaitOutput().ToLower();
                                if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                                    v = "yes";
                                else
                                    v = "no";
                            }*/

                            vars[n] = v;
                        }

                        if (orders[i][0].StartsWith("log "))
                            richTextBox2.Text += (m(orders[i][0].Substring(4), paramss) + '\n');
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
                            //else if (orders[i][0].Split(' ')[2] == "?ask" && !console)
                            //    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " " + enableWaitOutput());
                            else if (orders[i][0].Split(' ')[2] == "?askyes" && console)
                            {
                                string r = Console.ReadLine().ToLower();
                                if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                                else
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                            }
                            /*else if (orders[i][0].Split(' ')[2] == "?askyes" && !console)
                            {
                                string r = enableWaitOutput().ToLower();
                                if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                                else
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                            }*/

                            vars[m(orders[i][0].Split(' ')[1], paramss) + "_email"] = m(orders[i][0].Split(' ')[2], paramss);
                        }
                        else if (orders[i][0].StartsWith("botpass "))
                        {
                            if (orders[i][0].Split(' ')[2] == "?ask" && console)
                                orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " " + Console.ReadLine());
                            //else if (orders[i][0].Split(' ')[2] == "?ask" && !console)
                            //    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " " + enableWaitOutput());
                            else if (orders[i][0].Split(' ')[2] == "?askyes" && console)
                            {
                                string r = Console.ReadLine().ToLower();
                                if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                                else
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                            }
                            /*else if (orders[i][0].Split(' ')[2] == "?askyes" && !console)
                            {
                                string r = enableWaitOutput().ToLower();
                                if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                                else
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                            }*/

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
                            //else if (orders[i][0].Split(' ')[2] == "?ask" && !console)
                            //    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " " + enableWaitOutput());
                            else if (orders[i][0].Split(' ')[2] == "?askyes" && console)
                            {
                                string r = Console.ReadLine().ToLower();
                                if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                                else
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                            }
                            /*else if (orders[i][0].Split(' ')[2] == "?askyes" && !console)
                            {
                                string r = enableWaitOutput().ToLower();
                                if (r.StartsWith("yes") || r.StartsWith("ya") || r.StartsWith("yep") || r.StartsWith("yup") || r.StartsWith("ok") || r.StartsWith("sure") || r.StartsWith("alright"))
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " yes");
                                else
                                    orders[i][0] = (orders[i][0].Split(' ')[0] + " " + orders[i][0].Split(' ')[1] + " no");
                            }*/

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
                                                nvars["*gravity"] = msg.GetDouble(20).ToString();
                                                nvars["*background"] = msg.GetUInt(21).ToString();
                                                nvars["*accessible"] = msg.GetBoolean(22).ToString();
                                                nvars["*hidden"] = msg.GetBoolean(23).ToString();
                                                nvars["*spectatingallowed"] = msg.GetBoolean(24).ToString();
                                                nvars["*description"] = msg.GetString(25);
                                                nvars["*curselimit"] = msg.GetInt(26).ToString();
                                                nvars["*zombielimit"] = msg.GetInt(27).ToString();
                                                nvars["*iscampaign"] = msg.GetBoolean(28).ToString();
                                                nvars["*crewid"] = msg.GetString(29);
                                                nvars["*crew"] = msg.GetString(30);
                                                nvars["*canchangeworldoptions"] = (msg.GetBoolean(31) == true ? "yes" : "no");
                                                nvars["*crewstatus"] = msg.GetInt(32).ToString();
                                                nvars["*badge"] = msg.GetString(33);
                                                nvars["*iscrewmember"] = (msg.GetBoolean(34) == true ? "yes" : "no");
                                                nvars["*minimapenabled"] = (msg.GetBoolean(35) == true ? "yes" : "no");
                                                nvars["*lobbypreviewenabled"] = (msg.GetBoolean(36) == true ? "yes" : "no");

                                                foreach (string k in nvars.Keys)
                                                {
                                                    if (nvars[k] == "true")
                                                        nvars[k] = "yes";
                                                    else if (nvars[k] == "false")
                                                        nvars[k] = "no";
                                                }

                                                List<string> par = nvars.Values.ToList<string>();

                                                if (vars.ContainsKey(derp + "_event_botjoined"))
                                                    compile_read(vars[derp + "_event_botjoined"], derp + "_event_botjoined", par);
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
                                        if (!disconnecting)
                                            MessageBox.Show("An error has occurred on the message handler.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            bool run = true;

                            if (function == f && !lhex.Contains(f))
                            {
                                if (MessageBox.Show("The debugger has found a possible loophole! Do you want to keep it running? (Function " + f + ")", "Loophole detected!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                                    run = false;
                                else
                                    lhex.Add(f);
                            }

                            if (run)
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
                                    if (isParam(abc))
                                        yes = true;
                                }

                                if (vars.ContainsKey(m(abc, paramss)))
                                    yes = true;

                                if (v == (yes == true ? "yes" : "no"))
                                {
                                    string newx = orders[i][0].Substring(orders[i][0].Split(':')[0].Length + 2 + orders[i][0].Split(':')[1].Length);

                                    compile_read(m(newx, paramss), function, paramss);
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

                                string newx = orders[i][0].Substring(orders[i][0].Split(':')[0].Length + 2 + orders[i][0].Split(':')[1].Length);

                                compile_read(m(newx, paramss), function, paramss);
                            }
                        }
                    }
                }
                catch (Exception x)
                {
                    errors.Add(x);
                    if (!disconnecting)
                    {
                        stopdebug();

                        if (function != "")
                            MessageBox.Show("An error of unknown nature has occurred, could be user error. Please do not report this one unless you know how to reproduce the issue.\nThis error occurred under the function '" + function + "'.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Cannot keep debugging! An user error (incorrect email is an example of these) has been detected and caused a crash.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            #endregion
        }
        public void stopdebug()
        {
            disconnecting = true;
            button2.Text = "Debug";
            //label1.Text = "";
            //textBox1.Enabled = false;
            //textBox1.BackColor = Color.LimeGreen;
            d = -999;
            queuer.Enabled = false;
            queuer.Stop();
            sayQueue = new List<string[]>();
            lhex = new List<string>();
            vars = new Dictionary<string, string>();
            nvars = new Dictionary<string, string>();
            if (t2 != null)
            {
                t2.Abort();
                t2 = null;
            }

            foreach (Connection con in connections.Values)
                con.Disconnect();
            foreach (Client cl in clients.Values)
                cl.Logout();
            clients = new Dictionary<string, Client>();
            connections = new Dictionary<string, Connection>();
            richTextBox1.ReadOnly = false;
            orders = null;
            disconnecting = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (hp != bbb)
            {
                MessageBox.Show("Your EE Bot Builder tool is outdated. If you'd wish to have the latest one, please visit realmaster42-projects.weebly.com\nIt is required to use the latest version as some people tend to bypass bans!\n\nYou are using V" + bbb + " while latest one is V" + hp + "!", "EE Bot Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }

            if (MessageBox.Show("Are you sure you want to " + (button2.Text == "Debug" ? "start" : "stop") + " debugging your current code?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                if (button2.Text == "Debug")
                {
                    string error = EEBotBuilder.findErrors(richTextBox1.Text);
                    if (error != "")
                    {
                        MessageBox.Show("An error has been found, not allowing the debug to keep on. Please fix it before keeping on to debugging!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        richTextBox2.Text = error;
                    }
                    else
                    {
                        orders = EEBotBuilder.getInstructionText(richTextBox1.Text);
                        //textBox1.Enabled = true;
                        //textBox1.BackColor = Color.LightGreen;

                        richTextBox2.Clear();

                        #region compile
                        try
                        {
                            compile_read(richTextBox1.Text);

                            queuer.Enabled = true;
                            queuer.Start();
                            richTextBox1.ReadOnly = true;
                            button2.Text = "Stop Debug";
                        }
                        catch (Exception x)
                        {
                            errors.Add(x);
                            stopdebug();
                        }
                        #endregion
                    }
                }
                else
                    stopdebug();
            }
        }

        private void debugger_Tick(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string errors = EEBotBuilder.findErrors(richTextBox1.Text);
            if (errors == "")
                MessageBox.Show("Nice! You have no compilation errors.", "No errors detected.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                MessageBox.Show("The compilation has found an error that needs fixing.\n'" + errors + "'", "Error detected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void queuer_Tick(object sender, EventArgs e)
        {
            if (sayQueue.Count > 0)
            {
                try
                {
                    string[] talk = sayQueue[0];
                    sayQueue.RemoveAt(0);
                    connections[talk[0]].Send("say", talk[1]);
                }
                catch (Exception x)
                {
                    errors.Add(x);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            while (hp == "")
                d++;
            if (hp != bbb)
                Application.Exit();
            else
                d = -999;
        }
    }
}
