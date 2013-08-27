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
using System.IO;
using MCForge.SQL;
namespace MCForge.Commands
{
    public sealed class CmdRenameLvl : Command
    {
        public override string name { get { return "renamelvl"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdRenameLvl() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }
            Level foundLevel = Level.Find(message.Split(' ')[0]);
            if (foundLevel == null)
            {
                Player.SendMessage(p, "Level not found");
                return;
            }

            string newName = message.Split(' ')[1];

            if (File.Exists("levels/" + newName + ".lvl")) { Player.SendMessage(p, "Level already exists."); return; }
            if (foundLevel == Server.mainLevel) { Player.SendMessage(p, "Cannot rename the main level."); return; }

            foundLevel.Unload();

            try
            {
                File.Move("levels/" + foundLevel.name + ".lvl", "levels/" + newName + ".lvl");
                File.Move("levels/" + foundLevel.name + ".lvl.backup", "levels/" + newName + ".lvl.backup");

                try
                {
                    File.Move("levels/level properties/" + foundLevel.name + ".properties", "levels/level properties/" + newName + ".properties");
                }
                catch { }
                try
                {
                    File.Move("levels/level properties/" + foundLevel.name, "levels/level properties/" + newName + ".properties");
                }
                catch { }

                //Move and rename backups
                try
                {
                    string foundLevelDir, newNameDir;
                    for (int i = 1; ; i++)
                    {
                        foundLevelDir = @Server.backupLocation + "/" + foundLevel.name + "/" + i + "/";
                        newNameDir = @Server.backupLocation + "/" + newName + "/" + i + "/";

                        if (File.Exists(foundLevelDir + foundLevel.name + ".lvl"))
                        {
                            Directory.CreateDirectory(newNameDir);
                            File.Move(foundLevelDir + foundLevel.name + ".lvl", newNameDir + newName + ".lvl");
                            if (DirectoryEmpty(foundLevelDir))
                                Directory.Delete(foundLevelDir);
                        }
                        else
                        {
                            if (DirectoryEmpty(@Server.backupLocation + "/" + foundLevel.name + "/"))
                                Directory.Delete(@Server.backupLocation + "/" + foundLevel.name + "/");
                            break;
                        }
                    }
                }
                catch { }

                //safe against SQL injections because foundLevel is being checked and,
                //newName is being split and partly checked on illegal characters reserved for Windows.
                if (Server.useMySQL)
                    Database.executeQuery(String.Format("RENAME TABLE `Block{0}` TO `Block{1}`, " +
                                                                     "`Portals{0}` TO `Portals{1}`, " +
                                                                     "`Messages{0}` TO `Messages{1}`, " +
                                                                     "`Zone{0}` TO `Zone{1}`", foundLevel.name.ToLower(), newName.ToLower()));
                else {
                    using (DatabaseTransactionHelper helper = SQLiteTransactionHelper.Create()) { // ensures that it's either all work, or none work.
                        helper.Execute(String.Format("ALTER TABLE Block{0} RENAME TO Block{1}", foundLevel.name.ToLower(), newName.ToLower()));
                        helper.Execute(String.Format("ALTER TABLE Portals{0} RENAME TO Portals{1}", foundLevel.name.ToLower(), newName.ToLower()));
                        helper.Execute(String.Format("ALTER TABLE Messages{0} RENAME TO Messages{1}", foundLevel.name.ToLower(), newName.ToLower()));
                        helper.Execute(String.Format("ALTER TABLE Zone{0} RENAME TO Zone{1}", foundLevel.name.ToLower(), newName.ToLower()));
                        helper.Commit();
                    }
                }
                try { Command.all.Find("load").Use(p, newName); }
                catch { }
                Player.GlobalMessage("Renamed " + foundLevel.name + " to " + newName);
            }
            catch (Exception e) { Player.SendMessage(p, "Error when renaming."); Server.ErrorLog(e); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/renamelvl <level> <new name> - Renames <level> to <new name>");
            Player.SendMessage(p, "Portals going to <level> will be lost");
        }

        public static bool DirectoryEmpty(string dir)
        {
            if (!Directory.Exists(dir))
                return true;
            if (Directory.GetDirectories(dir).Length > 0)
                return false;
            if (Directory.GetFiles(dir).Length > 0)
                return false;

            return true;
        }
    }
}