using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.API
{
    public class NPCs
    {
        //attempt to return the name of every character in the game in a list
        public HashSet<string> GetAllCharacterNames(bool onlyDateable = false, bool onlyVillager = false, GameLocation onlyThisLocation = null)
        {
            HashSet<string> characters = new HashSet<string>(); //hashset ensures only unique values exist
            if (onlyThisLocation != null)
            {
                foreach (var c in onlyThisLocation.characters)
                {
                    if (!string.IsNullOrWhiteSpace(c.Name))
                    {
                        if (!onlyVillager || c.isVillager())
                            if (!onlyDateable || c.datable.Value) characters.Add(c.Name);
                    }
                }
                return characters; //only checking the one location
            }
            //start with NPCDispositions
            Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
            foreach (string s in dictionary.Keys)
            {
                var c = Game1.getCharacterFromName(s, onlyVillager);
                if (c != null) //simple nullcheck to ensure they weren't removed
                {
                    if (!onlyDateable || c.datable.Value)
                        if (!string.IsNullOrWhiteSpace(c.Name))
                            characters.Add(c.Name);
                }
            }
            //iterate locations for mod-added NPCs that aren't in the data
            foreach (var loc in Game1.locations)
                foreach (var c in loc.characters)
                {
                    if (!string.IsNullOrWhiteSpace(c.Name))
                    {
                        if (!onlyVillager || c.isVillager())
                            if (!onlyDateable || c.datable.Value) characters.Add(c.Name);
                    }
                }

            //return the list
            return characters;
        }

        public void Warp(NPC npc, string l, Point p)
        {
            Warp(npc, Game1.getLocationFromName(l), p.X, p.Y);
        }

        public void Warp(NPC npc, string l, int tileX, int tileY)
        {
            Warp(npc, Game1.getLocationFromName(l), tileX, tileY);
        }

        public void Warp(NPC npc, GameLocation l, Point p)
        {
            Warp(npc, l, p.X, p.Y);
        }

        public void Warp(NPC npc, GameLocation l, int tileX, int tileY)
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
    }
}
