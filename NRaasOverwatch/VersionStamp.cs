using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class OverwatchPopupTuning
    {
        [Tunable, TunableComment("Whether to use a popup menu approach when displaying the interactions")]
        public static bool kPopupMenuStyle = false;
    }

    public class VersionStamp : Common.ProtoVersionStamp, Common.IPreLoad
    {
        public static readonly string sNamespace = "NRaas.Overwatch";

        public void OnPreLoad()
        {
            sPopupMenuStyle = OverwatchPopupTuning.kPopupMenuStyle;
        }

        /* TODO
         *
         * Celebrity vehicles being cleaned up ?
         * 
         * Family tree issue when parent of a horse is sold ?
         * HasHouseholdBeenToThisRoom clean up, where is it ?
         * 
         * Force a Hard-Reset on Elevator users during "Reset Elevators"
         *         
         */

        /* Changes:
         *         
         * 
         */
        public static readonly int sVersion = 122;
    }
}
