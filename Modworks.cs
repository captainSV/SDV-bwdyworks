using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace bwdyworks
{
    public static class Modworks
    {
        internal static AssetEditor Assets;
        internal static IModHelper Helper;

        internal static Random _RNG;
        public static Random RNG
        {
            get { return _RNG; }
        }
        internal static Log _Log;
        public static Log Log
        {
            get { return _Log; }
        }

        internal static API.NPCs _NPCs;
        public static API.NPCs NPCs
        {
            get { return _NPCs; }
        }

        internal static API.Player _Player;
        public static API.Player Player
        {
            get { return _Player; }
        }

        internal static API.Items _Items;
        public static API.Items Items
        {
            get { return _Items; }
        }

        internal static API.Menus _Menus;
        public static API.Menus Menus
        {
            get { return _Menus; }
        }

        private static HashSet<string> Modules;

        internal static void Startup(Mod bwdyworks)
        {
            _Log = new Log(bwdyworks.Monitor);
            _RNG = new Random(DateTime.Now.Millisecond * DateTime.Now.GetHashCode());

            Helper = bwdyworks.Helper;
            Modules = new HashSet<string>();
            Assets = new AssetEditor();
            Helper.Content.AssetEditors.Add(Assets);

            //setup APIs
            _NPCs = new API.NPCs();
            _Player = new API.Player();
            _Items = new API.Items();
            _Menus = new API.Menus();
        }

        public static bool InstallModule(string id, bool debug)
        {
            if (Modules.Contains(id))
            {
                Log.Alert("Duplicate module detected: " + id + "\nThe duplicate module will not be loaded. Please only install one copy of each mod.");
                return false;
            }
            Log.Debug("bwdyworks: Module " + id + " loaded" + (debug ? " in debug mode." : "."));
            Modules.Add(id);
            return true;
        }

        public static bool IsModuleInstalled(string id)
        {
            return Modules.Contains(id);
        }
    }
}
