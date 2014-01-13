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
        public static readonly string sNamespace = "NRaas.Saver";

        public class Version : ProtoVersion<GameObject>
        { }

        /* TODO
         * 
         * Should [Saver] be disabled during Simporting ?
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 22;
    }
}
