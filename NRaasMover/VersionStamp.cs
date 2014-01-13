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
        public static readonly string sNamespace = "NRaas.Mover";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Mover.ResetSettings();
        }

        /* TODO
         * 
         * Issue with the funds calculation performed during a marriage move in ?  Calculated twice ?
         * 
         * Household import in PlayFlowModel:BeginPlaceLotContents
         * 
         * AlchemyCallback:CurePotionCallback override required to all overstuffing of cured genies
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 29;
    }
}
