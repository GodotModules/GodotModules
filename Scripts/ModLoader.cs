using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;

namespace Game
{
    public class ModLoader
    {
        private static Dictionary<string, Mod> Mods = new Dictionary<string, Mod>();
        private static MoonSharpVsCodeDebugServer DebugServer { get; set; }
        private static string PathMods { get; set; }
        private static Script Script { get; set; }


        public static void Init()
        {
            FindModsPath();

            DebugServer = new MoonSharpVsCodeDebugServer(); // how does this work in action?
            DebugServer.Start();

            Script = new Script();
            DebugServer.AttachToScript(Script, "Debug");

            var luaGame = new Godot.File();
            luaGame.Open("res://Scripts/Lua/Game.lua", Godot.File.ModeFlags.Read);

            Script.DoString(luaGame.GetAsText());

            Script.Globals["Player", "setHealth"] = (Action<int>)Master.Player.SetHealth;
        }

        public static void FindAllMods()
        {
            Directory.CreateDirectory(PathMods);

            var mods = Directory.GetDirectories(PathMods);

            foreach (var mod in mods)
            {
                var files = Directory.GetFiles(mod);

                var pathInfo = $"{mod}/info.json";
                var pathScript = $"{mod}/script.lua";

                // info.json or script.lua does not exist
                if (!File.Exists(pathInfo) || !File.Exists(pathScript))
                    continue;

                var modInfo = JsonConvert.DeserializeObject<ModInfo>(File.ReadAllText(pathInfo));

                // Mod with this name exists already
                if (Mods.ContainsKey(modInfo.Name))
                    continue;

                try
                {
                    Script.DoFile(pathScript);
                }
                catch (ScriptRuntimeException e)
                {
                    // Mod script did not run right
                    Godot.GD.Print(e.DecoratedMessage);
                    continue;
                }

                Mods.Add(modInfo.Name, new Mod
                {
                    ModInfo = modInfo
                });
            }
        }

        public static void Hook(string v, params object[] args) 
        {
            try 
            {
                Script.Call(Script.Globals[v], args);
            }
            catch (ScriptRuntimeException e)
            {
                Godot.GD.Print(e.DecoratedMessage);
            }
        }

        private static void FindModsPath()
        {
            string pathExeDir;

            if (Godot.OS.HasFeature("standalone")) // check if game is exported
                // set to exported release dir
                pathExeDir = $"{Directory.GetParent(Godot.OS.GetExecutablePath()).FullName}";
            else
                // set to project dir
                pathExeDir = Godot.ProjectSettings.GlobalizePath("res://");

            PathMods = Path.Combine(pathExeDir, "Mods");
        }
    }

    public struct Mod
    {
        public ModInfo ModInfo { get; set; }
    }

    public struct ModInfo
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
    }
}