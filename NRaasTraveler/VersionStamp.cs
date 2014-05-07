using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class VersionStamp : Common.ProtoVersionStamp
    {
        public static readonly string sNamespace = "NRaas.Traveler";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Traveler.ResetSettings();
        }

        /*
         * 
         * Order during loadup
         * 
         * ToInWorldState
         * LiveModeState
         * 
         */

        /* TODO
         * 
        * PlaceRequiredEPVenues.  Can "Base Camp" be included if available in Library ?
        * Can detective opportunities be unlocked for use in France, Egypt, China ?
        * Option in Traveler to remove the vehicle portion of the transition process
        * Add option to always display the fly-over sequence for a vacation world
        * Option to use local inactive sims as neighbors in [[University]] world ? How would that work exactly ?
         * 
         * Test Ghosthunters and Firefighters pushed by StoryProgression in Traveler worlds
         * Ambitions Career Portfolio completion counter is not transfered when traveling ?
         * Test Store menus while on vacation
         * Issue with double Save Prompt occurring if travel clock is allowed to run out
         * 
         * Issue with transferring the horseman career ?
         * Possible to sell your house as a vacation home, when living in an EA Vacation world ?
         * 
         * Issue with the Shutter Nut challenge ?
         * 
         * Test returning home from vacation with a Home Schooling sim
         * 
         * SeasonsManager.Enabled check at bottom of Sim:OnStartup()
         * 
         * Pregnancy moodlet does not transition properly ?
         * 
         * Test the transfer of celebrity points during transition
         * 
         * [Traveler] Do career outfits require protection during transition ?
         * 
         * Try traveling to Bridgeport, changing homeworlds, and then traveling back to original world ?
         * 
         * Still having issues with transitioning Genies
         * 
         * Blogs don't transition properly?
         * 
         * Verify "Pause Travel" works in University
         * Does "Treat As Vacation" work in University worlds ?
         * 
         * Does Relationship.WeddingAnniversary data need protecting ?
         * 
         * Test the Dusty Old Lamp while the mod is installed
         * 
         * Did EA fix the career minisim unpack issue with University ?
         * 
         * Transfer smartphone photos to inventory automatically on transition
         *
         * 
         * Need genealogy cleanup prior to descendant creation ?
         * 
         * "Ask for DNA Sample" while in the future disabled ?
         */

        /* Changes
         * 
         * The weather stone no longer attempts to spawn if you don't have Supernatural and Seasons installed         
         */
        public static readonly int sVersion = 85;
    }
}
