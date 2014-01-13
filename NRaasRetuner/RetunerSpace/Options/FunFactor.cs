using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options
{
    public class FunFactor : IntegerSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        static float sPreviousFactor = 1f;

        protected override int Value
        {
            get
            {
                return Retuner.Settings.mFunFactor;
            }
            set
            {
                Retuner.Settings.mFunFactor = value;

                ApplyFunFactor();
            }
        }

        public override string GetTitlePrefix()
        {
            return "FunFactor";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public static void ApplyFunFactor()
        {
            float funFactor = Retuner.Settings.mFunFactor / 100f;
            if (funFactor == sPreviousFactor) return;

            float factor = funFactor / sPreviousFactor;
            if ((factor == 1f) || (factor == 0f)) return;

            foreach (InteractionTuning tuning in InteractionTuning.sAllTunings.Values)
            {
                foreach (CommodityChange change in tuning.mTradeoff.mOutputs)
                {
                    if (change.Commodity == CommodityKind.Fun)
                    {
                        change.mConstantChange *= factor;
                    }
                }
            }

            sPreviousFactor = funFactor;
        }
    }
}
