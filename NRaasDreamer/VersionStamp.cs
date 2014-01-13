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
        public static readonly string sNamespace = "NRaas.Dreamer";

        public class Version : ProtoVersion<GameObject>
        {
            public override string Prompt
            {
                get
                {
                    string prompt = base.Prompt;

                    prompt += Common.NewLine + Common.NewLine + "kNoTimeOut=" + DreamerTuning.kNoTimeOut;

                    return prompt;
                }
            }
        }

        /* TODO
         * 
         * Active vs. Inactive control of memory creation by replacing the "ProcessLifeEvent" function using Dreamer
         * 
         */

        /* Changes
         * 
         * 
         */
        public static readonly int sVersion = 16;
    }
}
