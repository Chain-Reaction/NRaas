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
        public static readonly string sNamespace = "NRaas.Porter";

        public class Version : ProtoVersion<GameObject>
        { }

        /* TODO
         * 
         * Porter Doppleganger window gives impression that one can keep the new sim and chuck the old.
         * Porter and how merging with the premade sims dead or alive produces undesired results
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 43;
    }
}
