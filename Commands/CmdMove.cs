/*
Copyright (C) 2010-2013 David Mitchell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
namespace MCForge.Commands
{
    public sealed class CmdMove : Command
    {
        public override string name { get { return "move"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdMove() { }

        public override void Use(Player p, string message)
        {
            // /move name map
            // /move x y z
            // /move name x y z

            string[] param = message.Split(' ');

            if (param.Length < 1 || param.Length > 4) { Help(p); return; }

            // /move name
            if (param.Length == 1)
            {
                // Use main world by default
                // Add the world name to the 2nd param so that the IF block below is used
                param = new string[] { param[0], Server.mainLevel.name };
            }

            if (param.Length == 2)     // /move name map
            {
                Player who = Player.Find(param[0]);
                Level where = Level.Find(param[1]);
                if (who == null) { Player.SendMessage(p, "Could not find player specified"); return; }
                if (where == null) { Player.SendMessage(p, "Could not find level specified"); return; }
                if (p != null && who.group.Permission > p.group.Permission) { Player.SendMessage(p, "Cannot move someone of greater rank"); return; }

                Command.all.Find("goto").Use(who, where.name);
                if (who.level == where)
                    Player.SendMessage(p, "Sent " + who.color + who.name + Server.DefaultColor + " to " + where.name);
                else
                    Player.SendMessage(p, where.name + " is not loaded");
            }
            else
            {
                // /move name x y z
                // /move x y z

                Player who;

                if (param.Length == 4)
                {
                    who = Player.Find(param[0]);
                    if (who == null) { Player.SendMessage(p, "Could not find player specified"); return; }
                    if (p != null && who.group.Permission > p.group.Permission) { Player.SendMessage(p, "Cannot move someone of greater rank"); return; }
                    message = message.Substring(message.IndexOf(' ') + 1);
                }
                else
                {
                    who = p;
                }

                try
                {
                    ushort x = System.Convert.ToUInt16(message.Split(' ')[0]);
                    ushort y = System.Convert.ToUInt16(message.Split(' ')[1]);
                    ushort z = System.Convert.ToUInt16(message.Split(' ')[2]);
                    x *= 32; x += 16;
                    y *= 32; y += 32;
                    z *= 32; z += 16;
                    unchecked { who.SendPos((byte)-1, x, y, z, p.rot[0], p.rot[1]); }
                    if (p != who) Player.SendMessage(p, "Moved " + who.color + who.name);
                }
                catch { Player.SendMessage(p, "Invalid co-ordinates"); }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/move <player> <map> <x> <y> <z> - Move <player> to <map> or given coordinates");
            Player.SendMessage(p, "<map> must be blank if x, y or z is used and vice versa");
            Player.SendMessage(p, "If <map> is empty, the main level be assumed");
        }
    }
}