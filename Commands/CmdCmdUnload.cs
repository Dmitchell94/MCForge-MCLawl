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
    public sealed class CmdCmdUnload : Command
    {
        public override string name { get { return "cmdunload"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public CmdCmdUnload() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (Command.core.Contains(message.Split(' ')[0]))
            {
                Player.SendMessage(p, "/" + message.Split(' ')[0] + " is a core command, you cannot unload it!");
                return;
            }
            Command foundCmd = Command.all.Find(message.Split(' ')[0]);
            if(foundCmd == null)
            {
                Player.SendMessage(p, message.Split(' ')[0] + " is not a valid or loaded command.");
                return;
            }
            Command.all.Remove(foundCmd);
            GrpCommands.fillRanks();
            Player.SendMessage(p, "Command was successfully unloaded.");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/cmdunload <command> - Unloads a command from the server.");
        }
    }
}
