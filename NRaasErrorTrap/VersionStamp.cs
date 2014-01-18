using NRaas.CommonSpace.Options;
using NRaas.ErrorTrapSpace.Dereferences;
using NRaas.ErrorTrapSpace.Dereferences.Controllers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class VersionStamp : Common.ProtoVersionStamp
    {
        public static readonly string sNamespace = "NRaas.ErrorTrap";

        public class Version : ProtoVersion<GameObject>
        {
            public override string Prompt
            {
                get
                {
                    string msg = base.Prompt;

                    msg += Common.NewLine + "kPerformDereferencing=" + ErrorTrapTuning.kPerformDereferencing;
                    msg += Common.NewLine + "kFullStats=" + ErrorTrapTuning.kFullStats;
                    msg += Common.NewLine + "kLogCounting=" + ErrorTrapTuning3.kLogCounting;
                    msg += Common.NewLine + "kRecursionDebugging=" + DereferenceManager.kRecursionDebugging;
                    msg += Common.NewLine + "kShowFullReferencing=" + DereferenceGameObject.kShowFullReferencing;
                    msg += Common.NewLine + "kShowFoundReferences=" + DereferenceManagerTuning.kShowFoundReferences;
                    msg += Common.NewLine + "kForceReferenceLog=" + ErrorTrapTuning4.kForceReferenceLog;                   

                    return msg;
                }
            }
        }

        /* TODO
         * 
         * Issue with ErrorTrap deleting stolen goods as being corrupt inventory items
         * AutonomyManager errors and how to determine which sim
         * 
         * Issue with unicorn fertilized life fruit being deleted by [ErrorTrap] ?
         * 
         */
        
        public static readonly int sVersion = 95;
    }
}
