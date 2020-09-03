using NRaas.CommonSpace.Options;
using NRaas.ChemistrySpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas
{
    public class VersionStamp : Common.ProtoVersionStamp
    {
        public static readonly string sNamespace = "NRaas.Chemistry";

        public class Version : ProtoVersion<GameObject>
        { }

        public class TotalReset : ProtoResetSettings<GameObject>
        {
            public override string GetTitlePrefix()
            {
                return "ResetSettings";
            }
        }

        public static void ResetSettings()
        {
            Chemistry.ResetSettings();

            Chemistry.Init();

            BlendProfileBooter.Init();
            FilterValueAcquirementBooter.Init();
            ProfileTemplateBooter.Init();
            ResourceClassificationBooter.Init();
        }

        /* TODO
         *         
         */

        /* Changes
         * 
         *          
         * 
         */
        public static readonly int sVersion = 1;
    }
}