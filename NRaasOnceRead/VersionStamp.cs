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
        public static readonly string sNamespace = "NRaas.OnceRead";

        public class Version : ProtoVersion<GameObject>
        { }

        /* TODO
         * 
         * 
         */

        /* Changes
         * 
 Must release all injection modules for base-mod compatibility
         * Changes made to handle ITUN changes made by [[Retuner]]
         * "Choose Book" on the tablet now lists all books in town (Tablet)
         * 
         */
        public static readonly int sVersion = 12;
    }
}
