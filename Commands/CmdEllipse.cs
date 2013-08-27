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
    public sealed class CmdEllipse : Command
    {
        public override string name { get { return "ellipse"; } }
        public override string shortcut { get { return "el"; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdEllipse() { }

        public override void Use(Player p, string message)
        {

            if (message != "") { Help(p); return; }
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/ellipse - creates an ellipse.");
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte block = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, block);
            Position bp;
            bp.x = x; bp.y = y; bp.z = z; bp.type = type; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte block = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, block);
            Position cpos = (Position)p.blockchangeObject;
            unchecked { if (cpos.type != (byte)-1) { type = cpos.type; } }



            double x1 = cpos.x;
            double y1 = cpos.z;
            double x2 = x;
            double y2 = z;
            int height = Math.Abs(cpos.y - y) + 1;

            double xstart = Math.Min(x1, x2);
            double ystart = Math.Min(y1, y2);


            double a = ((Math.Abs(x1 - x2) + 1) / 2);
            double b = ((Math.Abs(y1 - y2) + 1) / 2);



            int dimensionx = (int)(Math.Abs(x1 - x2) + 1);
            bool OVX;
            bool OVY;
            int dimensiony = (int)(Math.Abs(y1 - y2) + 1);

            double[] yc = new double[dimensionx / 2 + 1];
            double[] length = new double[dimensionx / 2 + 1];

            if (dimensionx % 2 == 0)  // x is even
            {

                OVX = false;
            }
            else
            {
                // It's odd

                OVX = true;
            }

            if (dimensiony % 2 == 0)
            {
                OVY = false;
            }
            else { OVY = true; }

            int limit = 0;

            for (int i = 0; i < ((int)a); i++)
            {
                if (i == 0)
                {
                    yc[i] = Math.Ceiling(Math.Abs(Math.Sqrt((Math.Pow(a, 2) - Math.Pow(i + 0.5, 2))) * Math.Abs(b / a)));
                    length[i] = 0;
                    limit++;
                }
                else
                {
                    if (i == (int)a - 1)
                    {
                        yc[i] = 1;
                        if (yc[i - 1] - yc[i] > 1)
                        {
                            length[i] = Math.Round(yc[i - 1]) - Math.Round(yc[i]) - 1;
                            limit++;
                        }
                        else { length[i] = 0; limit++; }
                    }
                    else
                    {
                        yc[i] = Math.Abs(Math.Sqrt((Math.Pow(a, 2) - Math.Pow(i + 0.5, 2))) * Math.Abs(b / a));
                        if (yc[i - 1] - yc[i] > 1)
                        {
                            length[i] = Math.Round(yc[i - 1]) - Math.Round(yc[i]) - 1; limit++;
                        }
                        else { length[i] = 0; limit++; }
                    }
                }



            }


            if ((limit * height) > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to place " + (limit * height) + " blocks.");
                Player.SendMessage(p, "You cannot replace more than " + p.group.maxBlocks + ".");
                return;
            }






            //firststart
            int startx = (int)xstart + (int)a;
            int starty = (int)ystart + (int)b;

            ushort starth = Math.Min(cpos.y, y);
            //int endh = Math.Max(cpos.y,y);

            for (int h = 0; h < height; h++)
            {

                for (int i = 0; i < ((int)a); i++)
                {
                    if (length[i] == 0)
                    {

                        p.level.Blockchange(p, (ushort)(startx + i), starth, (ushort)(starty + ((int)Math.Round(yc[i]) - 1)), type);
                    }
                    if (length[i] != 0)
                    {
                        for (int ii = 0; ii <= length[i]; ii++)
                        {

                            p.level.Blockchange(p, (ushort)(startx + i), starth, (ushort)(starty + ((int)Math.Round(yc[i]) - 1 + ii)), type);
                        }
                    }


                }
                if (OVX)
                {
                    for (int i = 0; i < ((int)a); i++)
                    {
                        if (length[i] == 0)
                        {

                            p.level.Blockchange(p, (ushort)(startx - i), starth, (ushort)(starty + ((int)Math.Round(yc[i]) - 1)), type);
                        }
                        if (length[i] != 0)
                        {
                            for (int ii = 0; ii <= length[i]; ii++)
                            {

                                p.level.Blockchange(p, (ushort)(startx - i), starth, (ushort)(starty + ((int)Math.Round(yc[i]) - 1 + ii)), type);
                            }
                        }


                    }
                }
                else
                {
                    for (int i = 0; i < ((int)a); i++)
                    {
                        if (length[i] == 0)
                        {

                            p.level.Blockchange(p, (ushort)(startx - 1 - i), starth, (ushort)(starty + ((int)Math.Round(yc[i]) - 1)), type);
                        }
                        if (length[i] != 0)
                        {
                            for (int ii = 0; ii <= length[i]; ii++)
                            {

                                p.level.Blockchange(p, (ushort)(startx - 1 - i), starth, (ushort)(starty + ((int)Math.Round(yc[i]) - 1 + ii)), type);
                            }
                        }


                    }
                }
                // OVY STARTS HERE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (OVY)
                {
                    for (int i = 0; i < ((int)a); i++)
                    {
                        if (length[i] == 0)
                        {

                            p.level.Blockchange(p, (ushort)(startx + i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) - 1)), type);
                        }
                        if (length[i] != 0)
                        {
                            for (int ii = 0; ii <= length[i]; ii++)
                            {

                                p.level.Blockchange(p, (ushort)(startx + i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) - 1 + ii)), type);
                            }
                        }


                    }
                    if (OVX)
                    {
                        for (int i = 0; i < ((int)a); i++)
                        {
                            if (length[i] == 0)
                            {

                                p.level.Blockchange(p, (ushort)(startx - i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) - 1)), type);
                            }
                            if (length[i] != 0)
                            {
                                for (int ii = 0; ii <= length[i]; ii++)
                                {

                                    p.level.Blockchange(p, (ushort)(startx - i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) - 1 + ii)), type);
                                }
                            }


                        }
                    }
                    else
                    {
                        for (int i = 0; i < ((int)a); i++)
                        {
                            if (length[i] == 0)
                            {

                                p.level.Blockchange(p, (ushort)(startx - 1 - i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) - 1)), type);
                            }
                            if (length[i] != 0)
                            {
                                for (int ii = 0; ii <= length[i]; ii++)
                                {

                                    p.level.Blockchange(p, (ushort)(startx - 1 - i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) - 1 + ii)), type);
                                }
                            }


                        }
                    }
                }
                // NO OVY !!!!!!!!!!!!!!!!!!!!!!!!
                else
                {
                    for (int i = 0; i < ((int)a); i++)
                    {
                        if (length[i] == 0)
                        {

                            p.level.Blockchange(p, (ushort)(startx + i), starth, (ushort)(starty - ((int)Math.Round(yc[i]))), type);
                        }
                        if (length[i] != 0)
                        {
                            for (int ii = 0; ii <= length[i]; ii++)
                            {

                                p.level.Blockchange(p, (ushort)(startx + i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) + ii)), type);
                            }
                        }


                    }
                    if (OVX)
                    {
                        for (int i = 0; i < ((int)a); i++)
                        {
                            if (length[i] == 0)
                            {

                                p.level.Blockchange(p, (ushort)(startx - i), starth, (ushort)(starty - ((int)Math.Round(yc[i]))), type);
                            }
                            if (length[i] != 0)
                            {
                                for (int ii = 0; ii <= length[i]; ii++)
                                {

                                    p.level.Blockchange(p, (ushort)(startx - i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) + ii)), type);
                                }
                            }


                        }
                    }
                    else
                    {
                        for (int i = 0; i < ((int)a); i++)
                        {
                            if (length[i] == 0)
                            {

                                p.level.Blockchange(p, (ushort)(startx - 1 - i), starth, (ushort)(starty - ((int)Math.Round(yc[i]))), type);
                            }
                            if (length[i] != 0)
                            {
                                for (int ii = 0; ii <= length[i]; ii++)
                                {

                                    p.level.Blockchange(p, (ushort)(startx - 1 - i), starth, (ushort)(starty - ((int)Math.Round(yc[i]) + ii)), type);
                                }
                            }


                        }
                    }
                }
                starth++;
            }
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);

        }

        struct Position
        {
            public byte type;
            public ushort x, y, z;
        }

    }
}