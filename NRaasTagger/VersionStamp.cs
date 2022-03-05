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
        public static readonly string sNamespace = "NRaas.Tagger";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        { }

        public static void ResetSettings()
        {
            Tagger.ResetSettings();
        }

        /* TODO
         *         
         */

        /* Changes
         * 
         * 
         * 
         */
        public static readonly int sVersion = 5;
    }
}