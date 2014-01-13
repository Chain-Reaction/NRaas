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
        public static readonly string sNamespace = "NRaas.Traffic";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Traffic.ResetSettings();
        }

        /* TODO
         * 
         * Way to stop carpooling ?
         * 
         * Greater variety of fake cars, rather than all taxis
         * 
         * NRaas.Traffic.MaxFoodTrucks:Prompt missing
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 22;
    }
}
