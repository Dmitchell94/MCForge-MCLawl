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

namespace MCForge.Commands
{
    public sealed class CmdMoveAll : Command
    {
        public override string name { get { return "moveall"; } }
        public override string shortcut { get { return "ma"; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override void Use(Player p, string message)
        {
            Level level = Level.Find(message.Split(' ')[0]);
            if (level == null) { Player.SendMessage(p, "There is no level named '" + message.Split(' ')[0] + "'."); return; }
            if (p == null)
                foreach (Player pl in Player.players) { Command.all.Find("move").Use(null, pl.name + " " + level.name); }
            else
                foreach (Player pl in Player.players) { if (pl.group.Permission < p.group.Permission) Command.all.Find("move").Use(p, pl.name + " " + level.name); else Player.SendMessage(p, "You cannot move " + pl.color + pl.name + Server.DefaultColor + " because they are of equal or higher rank"); }
        }
        public override void Help(Player p) { Player.SendMessage(p, "/moveall <level> - Moves all players to the level specified."); }
    }
}