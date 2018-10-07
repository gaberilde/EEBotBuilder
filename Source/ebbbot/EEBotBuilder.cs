using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebbbot
{
    public static class EEBotBuilder
    {
        public static List<List<string>> getInstructionText(string code)
        {
            List<List<string>> lls = new List<List<string>>();
            List<string> poss = new List<string>() { "log ", "#", "bot ",
            "botemail ", "bottoken ", "botpass ",
            "botlogin ", "botworld ", "botjoin ",
            "botevent ", "botfunction ",
            "botfunction ", "botdisconnect ", "botlogout ", 
            "botdisconnected ", "userinput ",
            "bottalk "};

            bool addingfunc = false;
            string func = "";

            code = code.Replace("\r", "");

            for (int i = 0; i < code.Split('\n').Length; i++)
            {
                try
                {
                    string l = code.Split('\n')[i];
                    bool canrun = true;
                    if (l.StartsWith("var:"))
                    {
                        string[] t = l.Split(':');
                        string varn = t[2];
                        string vl = t[1];
                        if (vl == "*owner" || vl == "*worldname" || vl == "*favorites" ||
                            vl == "*plays" || vl == "*worldwidth" || vl == "*worldheight" ||
                            vl == "*likes" || vl == "*myid" || vl == "*smiley" || vl == "*aurashape" ||
                            vl == "*auracolor" || vl == "*goldborder" || vl == "*spawnx" || vl == "*spawny" ||
                            vl == "*chatcolor" || vl == "*myname" || vl == "*canedit" || vl == "*isowner" ||
                            vl == "*favorited" || vl == "*liked" || vl == "*background" || vl == "*gravity"
                            || vl == "*accessible" || vl == "*hidden" || vl == "*spectatingallowed" || vl == "*description" ||
                            vl == "*curselimit" || vl == "*zombielimit" || vl == "*iscampaign" || vl == "*crewid" ||
                            vl == "*crew" || vl == "*canchangeworldoptions" || vl == "*crewstatus" || vl == "*badge" ||
                            vl == "*iscrewmember" || vl == "*minimapenabled" || vl == "*lobbypreviewenabled")
                            lls.Add(new List<string>() { "", "Tried to create a variable with a neutral object only EEBotBuilder should have access to. " });
                        else if (!addingfunc)
                            lls.Add(new List<string>() { l, "" });

                        if (addingfunc)
                            func += (l + '\n');

                        continue;
                    }

                    if (l.Contains(":") && !l.StartsWith("var:"))
                    {
                        bool doit = true;
                        for (int xe = 0; xe < poss.Count; xe++)
                            if (l.Split(':')[0].StartsWith(poss[xe]))
                            {
                                doit = false;
                                break;
                            }

                        if (doit)
                        {
                            canrun = false;
                            string testl = l.Substring(l.Split(':')[0].Length + 2 + l.Split(':')[1].Length);

                            if (addingfunc)
                                func += (l + '\n');
                            else
                                lls.Add(new List<string>() { l, "" });
                        }
                    }

                    if (l.StartsWith("log "))
                    {
                        string lxe = "log " + l.Substring(4);
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { "log " + l.Substring(4), "" });
                        else if (canrun)
                            func += (l + '\n');
                    }
                    else if (l.StartsWith("#") && canrun && !addingfunc)
                        lls.Add(new List<string>() { "", "" });
                    else if (l.StartsWith("bot "))
                    {
                        string lxe = "bot " + l.Substring(4);
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { "bot " + l.Substring(4), "" });
                        else if (canrun)
                            func += "bot " + l.Substring(4) + '\n';
                    }
                    else if (l.StartsWith("botemail ") || l.StartsWith("bottoken "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        string em = sp[2];
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("botpass"))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        string em = sp[2];
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("botlogin "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("botdisconnect "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("botdisconnected "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        string fun = sp[2];
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("userinput "))
                    {
                        string fun = l.Substring(10);
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("bottalk "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        string latter = (l.Substring(9 + bot.Length));
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("botevent "))
                    {
                        string[] ft = l.Split(' ');
                        string bot = ft[1];
                        string even = ft[2];
                        int latterInd = (bot.Length + even.Length + 11);
                        string lator = l.Substring(latterInd);
                        bool can = false;
                        string final = "";
                        switch (even.ToLower())
                        {
                            case "botjoined": // Bot Joined
                            case "botjoin":
                            case "botinit":
                            case "init":
                            case "inited":
                            case "join":
                            case "botconnected":
                            case "botconnect":
                            case "bot_joined":
                            case "bot_init":
                            case "bot_connected":
                            case "bot_connect":
                                final = ("botevent " + bot + " botjoined " + lator);
                                can = true;
                                break;
                            case "plrjoined": // Player Joined
                            case "add":
                            case "init2":
                            case "playerjoined":
                            case "plrjoin":
                            case "playerjoin":
                            case "player_joined":
                            case "player_join":
                            case "newplayer":
                            case "new_player":
                                final = ("botevent " + bot + " plrjoined " + lator);
                                can = true;
                                break;
                            case "plrleft": // Player Left
                            case "left":
                            case "leave":
                            case "playerleft":
                            case "plrleave":
                            case "playerleave":
                            case "player_left":
                            case "player_leave":
                                final = ("botevent " + bot + " plrleft " + lator);
                                can = true;
                                break;
                        }

                        if (!can)
                            lls.Add(new List<string>() { "", "Used invalid event name." });
                        else if (canrun && !addingfunc)
                            lls.Add(new List<string>() { final, "" });
                        else if (canrun)
                            func += (final + '\n');
                    }
                    else if (l.StartsWith("bottalk "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        string latter = l.Substring(9 + bot.Length);
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += (l + '\n');
                    }
                    else if (l.StartsWith("botlogout "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("botworld "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        string world = sp[2];
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("botjoin "))
                    {
                        string[] sp = l.Split(' ');
                        string bot = sp[1];
                        if (canrun && !addingfunc)
                            lls.Add(new List<string>() { l, "" });
                        else if (canrun)
                            func += l + '\n';
                    }
                    else if (l.StartsWith("function:"))
                        lls.Add(new List<string>() { "", "Tried to create a private object for EEBotBuilder's language." });
                    else if (l.StartsWith("botfunction ") && l.EndsWith("{"))
                    {
                        if (addingfunc)
                            lls.Add(new List<string>() { "", "Adding a function inside a function." });
                        else
                        {
                            addingfunc = true;
                            func = "function:" + (l.Split(' ')[1].Substring(0, l.Split(' ')[1].Length - 1)) + '\n';
                        }
                    }
                    else if (l.StartsWith("botfunction ") && l.EndsWith("!"))
                    {
                        string[] t = l.Split(' ');
                        string f = t[1];
                        f = f.Substring(0, f.Length - 1);
                        if (addingfunc)
                            func += l + '\n';
                        else if (canrun)
                            lls.Add(new List<string>() { l, "" });
                    }
                    else if (l == "}" && addingfunc)
                    {
                        addingfunc = false;
                        List<List<string>> newone = new List<List<string>>();
                        newone.Add(new List<string>() { func, "" });
                        for (int ix = 0; ix < lls.Count; ix++)
                            newone.Add(lls[ix]);

                        lls = newone;
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                    lls.Add(new List<string>() { "", "Error at line " + i.ToString() + "\n(" + code.Split('\n')[i] + ")" });
                }
            }

            return lls;
        }
        public static string findErrors(string code)
        {
            List<List<string>> t = getInstructionText(code);
            for (int i = 0; i < t.Count; i++)
                if (t[i][1] != "")
                    return t[i][1];

            return "";
        }
        //public static string obfuscate(string txt)
        //{
        //    Random r = new Random();
        //    string obf = "";
        //    for (int i = 0; i < txt.Split('\n').Length; i++)
        //    {
        //        string l = txt.Split('\n')[i];
        //        if (l.StartsWith("#"))
        //        {
        //            l = l.Replace("a", "◘");
        //            l = l.Replace("o", "┤");
        //            for (int x = (l.Length - 1); x >= 0; x--)
        //                obf += (l[x]);

        //            obf += "/%ß";
        //        }
        //        else if (l.StartsWith("var:"))
        //        {
        //            l = l.Replace("var:", "╣");
        //            l = l.Replace("e", "☼");
        //            l = l.Replace(":", "Ð");
        //            for (int x = (l.Length - 1); x >= 0; x--)
        //                obf += (l[x]);

        //            obf += "$À";
        //        }
        //        else
        //        {
        //            int r2 = r.Next(0, 4);

        //            l = l.Replace("a", "▓");
        //            l = l.Replace("e", "▒");

        //            if (r2 == 1)
        //                l = l.Replace("o", "‗");
        //            else
        //                l = l.Replace("o", "Æ");

        //            r2 = r.Next(0, 4);

        //            if (r2 == 1)
        //                l = l.Replace("b", "♣");
        //            else
        //                l = l.Replace("b", "♠");

        //            r2 = r.Next(0, 4);

        //            if (r2 == 1)
        //                l = l.Replace("t", "○");
        //            else
        //                l = l.Replace("t", "☺");

        //            l = l.Replace(":", "◘");
        //            l = l.Replace(" ", "⌂");
        //            l = l.Replace("A", "█");
        //            l = l.Replace("E", "▬");
        //            l = l.Replace("O", "♂");
        //            l = l.Replace("B", "•");
        //            l = l.Replace("T", "♦");

        //            obf += l;
        //        }

        //        obf += ('\n');
        //    }

        //    obf = obf.Replace("\r", "\n");
        //    obf = obf.Replace("\n\n", "\n");

        //    return obf;
        //}
        //public static string deobfuscate(string txt)
        //{
        //    string obf = "";
        //    for (int i = 0; i < txt.Split('\n').Length; i++)
        //    {
        //        string l = txt.Split('\n')[i];
        //        if (l.EndsWith("#/%ß"))
        //        {
        //            l = l.Replace("◘", "a");
        //            l = l.Replace("┤", "o");
        //            l = l.Replace("/%ß", "");

        //            for (int x = (l.Length - 1); x >= 0; x--)
        //                obf += (l[x]);
        //        }
        //        else if (l.StartsWith("$À"))
        //        {
        //            l = l.Replace("╣", "var:");
        //            l = l.Replace("☼", "e");
        //            l = l.Replace("Ð", ":");
        //            l = l.Replace("$À", "");

        //            for (int x = (l.Length - 1); x >= 0; x--)
        //                obf += (l[x]);
        //        }
        //        else
        //        {
        //            l = l.Replace("▓", "a");
        //            l = l.Replace("▒", "e");
        //            l = l.Replace("‗", "o");
        //            l = l.Replace("Æ", "o");
        //            l = l.Replace("♣", "b");
        //            l = l.Replace("♠", "b");
        //            l = l.Replace("○", "t");
        //            l = l.Replace("☺", "t");
        //            l = l.Replace("◘", ":");
        //            l = l.Replace("⌂", " ");
        //            l = l.Replace("█", "A");
        //            l = l.Replace("▬", "E");
        //            l = l.Replace("♂", "O");
        //            l = l.Replace("•", "B");
        //            l = l.Replace("♦", "T");

        //            obf += l;
        //        }

        //        obf += ('\n');
        //    }

        //    return obf;
        //}
    }
}
