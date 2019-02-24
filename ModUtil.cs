using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace bwdyworks
{
    public class ModUtil
    {
        public StardewModdingAPI.Mod Mod;
        public Random RNG = new Random(DateTime.Now.Millisecond * DateTime.Now.GetHashCode());

        public ModUtil(StardewModdingAPI.Mod mod)
        {
            this.Mod = mod;
        }

        public bool Debug = false;
        private bool utilactive = false; //safety switch for registry functions

        public void AskQuestion(string question, StardewValley.Response[] responses, StardewValley.GameLocation.afterQuestionBehavior callback)
        {
            StardewValley.Game1.currentLocation.lastQuestionKey = "bwdy_question";
            StardewValley.Game1.currentLocation.createQuestionDialogue(question, responses, callback);
        }

        //call to begin setting up.
        //if returns true, do your setup, then call bwdyfinish.
        //if returns false, there's a problem, do nothing further.
        public bool StartConfig(bool debug)
        {
            this.Debug = debug;
            Mod.Monitor.Log(Mod.ModManifest.Name + " reporting in." + (debug ? " (debug mode)" : ""));
            utilactive = true;
            return true;
        }

        public void WarpNPC(NPC npc, string l, Point p)
        {
            WarpNPC(npc, Game1.getLocationFromName(l), p.X, p.Y);
        }

        public void WarpNPC(NPC npc, string l, int tileX, int tileY)
        {
            WarpNPC(npc, Game1.getLocationFromName(l), tileX, tileY);
        }

        public void WarpNPC(NPC npc, GameLocation l, Point p)
        {
            WarpNPC(npc, l, p.X, p.Y);
        }

        public void WarpNPC(NPC npc, GameLocation l, int tileX, int tileY)
        {
            if (l != npc.currentLocation)
            {
                npc.currentLocation.characters.Remove(npc);
                l.characters.Add(npc);
                npc.currentLocation = l;
            }
            npc.setTilePosition(tileX, tileY);
            //Mod.Monitor.Log("Warped " + npc.Name + " to " + l.Name + " at " + tileX + ", " + tileY);
        }

        public void AddItem(BasicItemEntry entry)
        {
            ItemRegistry.RegisterItem(Mod, entry);
        }

        public int? GetModItemId(string internalName)
        {
            string globalid = Mod.ModManifest.UniqueID + ":" + internalName;
            if (ItemRegistry.LocalRegistry.RegisteredItemIds.ContainsKey(globalid))
            {
                return ItemRegistry.LocalRegistry.RegisteredItemIds[globalid];
            }
            else
            {
                Mod.Monitor.Log("Item id lookup failed for: " + globalid);
                return null;
            }
        }

        public void AddMonsterLoot(MonsterLootEntry loot)
        {
            if (!utilactive)
            {
                bwdyworks.Mod.Instance.Monitor.Log(Mod.ModManifest.Name + " tried to call AddMonsterLoot outside utility window.");
                return;
            }
            bwdyworks.Mod.Instance.assets.MonsterLoot.Add(loot);
            bwdyworks.Mod.Instance.Monitor.Log(Mod.ModManifest.Name + " added a loot drop to " + loot.MonsterID + "s.");
        }

        public StardewValley.Object CreateItemstack(int id, int count)
        {
            StardewValley.Object i = (StardewValley.Object)StardewValley.Objects.ObjectFactory.getItemFromDescription(0, id, count);
            i.IsSpawnedObject = true;
            i.ParentSheetIndex = id;
            return i;
        }

        public void GiveItemToLocalPlayer(int id, int count = 1)
        {
            var i = CreateItemstack(id, count);
            StardewValley.Game1.player.addItemByMenuIfNecessary(i);
        }

        public void RemoveItemFromLocalPlayer(StardewValley.Item which, int count = 1)
        {
            int currentQuantity = which.getStack();
            if(count > currentQuantity)
            {
                StardewValley.Game1.player.removeItemsFromInventory(which.ParentSheetIndex, count);
            } else if(count == currentQuantity)
            {
                StardewValley.Game1.player.removeItemFromInventory(which);
            } else {
                which.Stack = which.Stack - count;
            }
        }

        public int[] GetLocalPlayerStandingTileCoordinate()
        {
            var f = StardewValley.Game1.player;
            return new int[] { f.getTileX(), f.getTileY() };
        }

        public int[] GetLocalPlayerFacingTileCoordinate()
        {
            var f = StardewValley.Game1.player;
            int target_x = f.getTileX();
            int target_y = f.getTileY();
            int d = f.FacingDirection;
            switch (d)
            {
                case 0: target_y -= 1; break; //up
                case 1: target_x += 1; break; //right
                case 2: target_y += 1; break; //down
                case 3: target_x -= 1; break; //left
            }
            return new int[] { target_x, target_y };
        }

        public int GetFriendshipPoints(string NPC)
        {
            StardewValley.Farmer f2 = StardewValley.Game1.player;
            if (f2.friendshipData.ContainsKey(NPC)) return f2.friendshipData[NPC].Points;
            else return 0;
        }

        public void SetFriendshipPoints(string NPC, int points)
        {
            StardewValley.Farmer f2 = StardewValley.Game1.player;
            if (!f2.friendshipData.ContainsKey(NPC)) f2.friendshipData[NPC] = new StardewValley.Friendship(points);
            else f2.friendshipData[NPC].Points = points;
        }

        public void EndConfig()
        {
            utilactive = false;
        }
    }
}
