using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks
{
    public static class ItemRegistry
    {
        public static Dictionary<int, BasicItemEntry> RegisteredItems { get; set; } = new Dictionary<int, BasicItemEntry>();
        public static Dictionary<string, BasicItemEntry> GlobalRegistry { get; set; } = new Dictionary<string, BasicItemEntry>();
        internal static ItemRegistrySaveDataModel LocalRegistry = new ItemRegistrySaveDataModel();
        internal static bool Loaded = false;
        private static int BaseItemId;

        internal static void RegisterItem(StardewModdingAPI.Mod mod, BasicItemEntry itemdata)
        {
            string globalid = mod.ModManifest.UniqueID + ":" + itemdata.InternalName;
            GlobalRegistry[globalid] = itemdata;
        }

        internal static void Load()
        {
            Loaded = true;
            BaseItemId = 1600; //reset the base item id
            LocalRegistry = Mod.Instance.Helper.Data.ReadJsonFile<ItemRegistrySaveDataModel>("bwdyItemIds.json");
            if (LocalRegistry == null) {
                LocalRegistry = new ItemRegistrySaveDataModel();
                Mod.Instance.Monitor.Log("Item registry created.");
            } else Mod.Instance.Monitor.Log("Item registry loaded.");
            foreach (KeyValuePair<string, BasicItemEntry> entry in GlobalRegistry)
            {
                int integerId;
                //does data exist already? use existing integerId to maintain savegame integrity.
                if (LocalRegistry.RegisteredItemIds.ContainsKey(entry.Key)) integerId = LocalRegistry.RegisteredItemIds[entry.Key];
                else integerId = BaseItemId++;
                while (LocalRegistry.RegisteredItemIds.ContainsValue(BaseItemId)) BaseItemId++; //skip existing ids
                entry.Value.IntegerId = integerId;
                entry.Value.GlobalId = entry.Key;
                LocalRegistry.RegisteredItemIds[entry.Key] = integerId;
                RegisteredItems[integerId] = entry.Value;
                Mod.Instance.Monitor.Log("Registered item configured: " + entry.Value.GlobalId);
            }
        }

        internal static void Save()
        {
            Mod.Instance.Monitor.Log("Item registry saved.");
            Mod.Instance.Helper.Data.WriteJsonFile("bwdyItemIds.json", LocalRegistry);
        }
    }

    public class ItemRegistrySaveDataModel
    {
        public Dictionary<string, int> RegisteredItemIds { get; set; } = new Dictionary<string, int>();
    }
}
