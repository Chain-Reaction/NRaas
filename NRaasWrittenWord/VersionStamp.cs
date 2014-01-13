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
        public static readonly string sNamespace = "NRaas.WrittenWord";

        public class Version : ProtoVersion<GameObject>
        { }

        /* TODO
         * 
         * [WrittenWord] adds journalist reports to the bookstore ?
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 11;
    }
}
