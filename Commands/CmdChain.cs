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
using System.Threading;
namespace MCForge.Commands
{
    public sealed class CmdChain : Command
    {
        public override string name { get { return "chain"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdChain() { }
        // Fields
        public int x;
        public int y;
        public int z;

        // Methods
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/chain - Shoots a chain of brown mushrooms and grabs a block and brings it back to the start.");
        }

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, "Sorry, you can't run this from console!");
                return;
            }
            if (p.level.permissionbuild > p.group.Permission)
            {
                Player.SendMessage(p, "You cannot build on this map!");
            }
            else
            {
                int num = p.rot[0];
                int num2 = p.pos[0] / 0x20;
                int num3 = p.pos[1] / 0x20;
                int num4 = p.pos[2] / 0x20;
                ushort x = Convert.ToUInt16(num2);
                ushort y = Convert.ToUInt16(num3);
                ushort z = Convert.ToUInt16(num4);
                int width = p.level.width;
                int num9 = width - num2;
                int length = p.level.height;
                int num11 = length - num4;
                if ((num > 0x21) && (num < 0x60))
                {
                    for (int i = 0; i <= num9; i++)
                    {
                        ushort num13 = Convert.ToUInt16((int)(x + i));
                        if (num13 == (width - 1))
                        {
                            return;
                        }
                        ushort num14 = Convert.ToUInt16((int)(num13 + 1));
                        Thread.Sleep(250);
                        p.level.Blockchange(p, num13, y, z, 0x27);
                        if (p.level.GetTile(num14, y, z) != 0)
                        {
                            byte type = p.level.GetTile(num14, y, z);
                            for (int j = 0; j <= num9; j++)
                            {
                                if (p.level.GetTile(x, y, z) == type)
                                {
                                    return;
                                }
                                ushort num17 = Convert.ToUInt16((int)((x - j) + i));
                                if (num17 == 0)
                                {
                                    return;
                                }
                                ushort num18 = Convert.ToUInt16((int)(num17 + 1));
                                Convert.ToUInt16((int)(num17 - 1));
                                Thread.Sleep(250);
                                p.level.Blockchange(p, num17, y, z, type);
                                p.level.Blockchange(p, num18, y, z, 0);
                            }
                            return;
                        }
                    }
                }
                if ((num > 0xa1) && (num < 0xf4))
                {
                    for (int k = 0; k <= num9; k++)
                    {
                        ushort num20 = Convert.ToUInt16((int)(x - k));
                        if (num20 == 0)
                        {
                            return;
                        }
                        ushort num21 = Convert.ToUInt16((int)(num20 - 1));
                        Thread.Sleep(250);
                        p.level.Blockchange(p, num20, y, z, 0x27);
                        if (p.level.GetTile(num21, y, z) != 0)
                        {
                            byte num22 = p.level.GetTile(num21, y, z);
                            for (int m = 0; m <= num9; m++)
                            {
                                if (p.level.GetTile(x, y, z) != num22)
                                {
                                    ushort num24 = Convert.ToUInt16((int)((x + m) - k));
                                    ushort num25 = Convert.ToUInt16((int)(num24 - 1));
                                    Convert.ToUInt16((int)(num24 + 1));
                                    Thread.Sleep(250);
                                    p.level.Blockchange(p, num24, y, z, num22);
                                    p.level.Blockchange(p, num25, y, z, 0);
                                }
                                else
                                {
                                    return;
                                }
                            }
                            return;
                        }
                    }
                }
                if ((num > 0x61) && (num < 160))
                {
                    for (int n = 0; n <= num11; n++)
                    {
                        ushort num27 = Convert.ToUInt16((int)(z + n));
                        if (num27 == (length - 1))
                        {
                            return;
                        }
                        ushort num28 = Convert.ToUInt16((int)(num27 + 1));
                        Thread.Sleep(250);
                        p.level.Blockchange(p, x, y, num27, 0x27);
                        if (p.level.GetTile(x, y, num28) != 0)
                        {
                            byte num29 = p.level.GetTile(x, y, num28);
                            for (int num30 = 0; num30 <= num9; num30++)
                            {
                                if (p.level.GetTile(x, y, z) == num29)
                                {
                                    return;
                                }
                                ushort num31 = Convert.ToUInt16((int)((z - num30) + n));
                                if (num31 == 0)
                                {
                                    return;
                                }
                                ushort num32 = Convert.ToUInt16((int)(num31 + 1));
                                Convert.ToUInt16((int)(num31 - 1));
                                Thread.Sleep(250);
                                p.level.Blockchange(p, x, y, num31, num29);
                                p.level.Blockchange(p, x, y, num32, 0);
                            }
                            return;
                        }
                    }
                }
                if ((num > 0xe0) || (num < 0x21))
                {
                    for (int num33 = 0; num33 <= num11; num33++)
                    {
                        ushort num34 = Convert.ToUInt16((int)(z - num33));
                        if (num34 == 0)
                        {
                            return;
                        }
                        ushort num35 = Convert.ToUInt16((int)(num34 - 1));
                        Thread.Sleep(250);
                        p.level.Blockchange(p, x, y, num34, 0x27);
                        if (p.level.GetTile(x, y, num35) != 0)
                        {
                            byte num36 = p.level.GetTile(x, y, num35);
                            for (int num37 = 0; num37 <= num9; num37++)
                            {
                                if (p.level.GetTile(x, y, z) != num36)
                                {
                                    ushort num38 = Convert.ToUInt16((int)((z + num37) - num33));
                                    ushort num39 = Convert.ToUInt16((int)(num38 - 1));
                                    Convert.ToUInt16((int)(num38 + 1));
                                    Thread.Sleep(250);
                                    p.level.Blockchange(p,x, y, num38, num36);
                                    p.level.Blockchange(p,x, y, num39, 0);
                                }
                                else
                                {
                                    return;
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }
    }

}


