using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class DebugEnablerPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.DebugEnabler";

        public class Version : ProtoVersion<GameObject>
        { }

        public class Reapply : ProtoReapply<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            DebugEnabler.ResetSettings();
        }

        public void OnPreLoad()
        {
            sPopupMenuStyle = DebugEnablerPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         * 
         * Ability to move "left" or "right" in DebugEnabler
         * Possible to run "Create Gem" for multiple sub selections at the same time ?
         * Ability to alter the tilt of an object ?  Possible ?
         * Option to recreate missing martial arts outfits, up to a sims current skill level
         * Add "Reset Extinguisher" debug interaction to correct the inability to "Upgrade Extinguisgher" in the Firefighter career
         * Add Consignment option
         *
         */

        /* Changes
         * 
         * "Test Menu Interactions" now logs the duration it takes to perform the "Test" for each interaction, if it exceeds 1 time unit
         * "Fix Invisible Sims" now handles mermaids and werewolves properly
         * Added [[Into The Future]] related console commands
         * 
         */
        public static readonly int sVersion = 55;
    }
}
