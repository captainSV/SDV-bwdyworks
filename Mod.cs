using StardewModdingAPI;
using System.Reflection;

//a simple mod wrapper for logging purposes.
//the real magic is the bwdymod class.
using bwdyworks.Registry;

namespace bwdyworks
{
    public class Mod : StardewModdingAPI.Mod
    {
#if DEBUG
        private static readonly bool DEBUG = true;
#else
        private static readonly bool DEBUG = false;
#endif

        public override void Entry(IModHelper helper)
        {
            Modworks.Startup(this);
            Monitor.Log("bwdy here! let's have some fun <3 " + Assembly.GetEntryAssembly().GetName().Version.ToString() + (DEBUG ? " (DEBUG MODE ACTIVE)":""));
            Helper.Events.GameLoop.Saving += GameLoop_Saving;
        }

        private void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e)
        {
            ItemRegistry.Save();
        }
    }
}