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
    public sealed class CmdNewLvl : Command
    {
        public override string name { get { return "newlvl"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdNewLvl() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            string[] parameters = message.Split(' '); // Grab the parameters from the player's message
            if (parameters.Length >= 5 && parameters.Length <= 6) // make sure there are 5 or 6 params
            {
                switch (parameters[4])
                {
                    case "flat":
                    case "pixel":
                    case "island":
                    case "mountains":
                    case "ocean":
                    case "forest":
                    case "desert":
                    case "space":
                    case "rainbow":
                    case "hell":
                        break;

                    default:
                        Player.SendMessage(p, "Valid types: island, mountains, forest, ocean, flat, pixel, desert, space, rainbow, and hell"); return;
                }

                string name = parameters[0].ToLower();
                ushort x = 1, y = 1, z = 1;
                int seed = 0;
                bool useSeed = false;
                try
                {
                    x = Convert.ToUInt16(parameters[1]);
                    y = Convert.ToUInt16(parameters[2]);
                    z = Convert.ToUInt16(parameters[3]);
                }
                catch { Player.SendMessage(p, "Invalid dimensions."); return; }
                if (parameters.Length == 6)
                {
                    try { seed = Convert.ToInt32(parameters[5]); }
                    catch { seed = parameters[5].GetHashCode(); }
                    useSeed = true;
                }
                if (!isGood(x)) { Player.SendMessage(p, x + " is not a good dimension! Use a power of 2 next time."); return; }
                if (!isGood(y)) { Player.SendMessage(p, y + " is not a good dimension! Use a power of 2 next time."); return; }
                if (!isGood(z)) { Player.SendMessage(p, z + " is not a good dimension! Use a power of 2 next time."); return; }

                if (!Player.ValidName(name)) { Player.SendMessage(p, "Invalid name!"); return; }
                if (System.IO.File.Exists("levels/" + name + ".lvl")) { Player.SendMessage(p, "Level \"" + name + "\" already exists!"); return; }

                /*try
                {
                    if (p != null)
                    if (p.group.Permission < LevelPermission.Admin)
                    {
                        if (x * y * z > 30000000) { Player.SendMessage(p, "Cannot create a map with over 30million blocks"); return; }
                    }
                    else
                    {
                        if (x * y * z > 225000000) { Player.SendMessage(p, "You cannot make a map with over 225million blocks"); return; }
                    }
                }
                catch 
                { 
                    Player.SendMessage(p, "An error occured"); 
                }*/

                // create a new level...
                try
                {
                    using (Level lvl = new Level(name, x, y, z, parameters[4], seed, useSeed))
                    {
                        lvl.Save(true); //... and save it.
                        lvl.Dispose(); // Then take out the garbage.
                    }
                }
                finally
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                Player.GlobalMessage("Level \"" + name + "\" created" + (useSeed ? " with seed \"" + parameters[5] + "\"" : "")); // The player needs some form of confirmation.

            }
            else
                Help(p);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/newlvl - creates a new level.");
            Player.SendMessage(p, "/newlvl mapname 128 64 128 type seed");
            Player.SendMessage(p, "Valid sizes: 16, 32, 64, 128, 256, 512, 1024"); //Update this to add more?
            Player.SendMessage(p, "Valid types: island, mountains, forest, ocean, flat, pixel, desert, space, rainbow, and hell");
            Player.SendMessage(p, "The seed is optional, and controls how the level is generated.");
            Player.SendMessage(p, "If the seed is the same, the level will be the same.");
            Player.SendMessage(p, "The seed does not do anything on flat and pixel type maps.");
        }

        public bool isGood(ushort value)
        {
            switch (value)
            {
                //case 2:
                //case 4:
                //case 8:
                case 16: // below this is currently invalid.
                case 32:
                case 64:
                case 128:
                case 256:
                case 512:
                case 1024:
                case 2048:
                case 4096:
                case 8192:
                    return true;
            }

            return false;
        }
    }
}
