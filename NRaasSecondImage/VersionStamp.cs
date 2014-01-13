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
        public static readonly string sNamespace = "NRaas.SecondImage";

        public class Version : ProtoVersion<GameObject>
        { }

        /* TODO
         * 
         * Expand the relationship names shown in the tooltip to second cousin level
         * 
         */

        /* Changes
         *     
         * 
         */
        public static readonly int sVersion = 6;
    }
}
