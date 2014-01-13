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
        public static readonly string sNamespace = "NRaas.Shooless";

        public class Version : ProtoVersion<GameObject>
        {
            protected override bool Allow(GameHitParameters<GameObject> parameters)
            {
                if (!Common.IsRootMenuObject(parameters.mTarget)) return false;

                return base.Allow(parameters);
            }
        }

        /* TODO
         *
         * Issue with shooless and sculpted toilets ?
         * Option to not switch into Naked while on a public lot
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 18;
    }
}
