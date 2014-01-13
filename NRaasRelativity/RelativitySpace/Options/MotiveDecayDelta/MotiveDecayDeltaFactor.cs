using NRaas.CommonSpace.Options;
using NRaas.RelativitySpace.Helpers;
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

namespace NRaas.RelativitySpace.Options.MotiveDecayDelta
{
    public class MotiveDecayDeltaFactor : FloatSettingOption<GameObject>, IMotiveDecayDeltaOption
    {
       MotiveKey mKey;

       public MotiveDecayDeltaFactor(MotiveKey key)
        {
            mKey = key;
        }

        public override string GetTitlePrefix()
        {
            return "MotiveDecayDeltaFactor";
        }

        public override string Name
        {
            get { return mKey.GetLocalizedName(); }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override string GetPrompt()
        {
            return Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { Name });
        }

        protected override float Value
        {
            get
            {
                return Relativity.Settings.GetMotiveDecayFactor(mKey);
            }
            set
            {
                Relativity.Settings.SetMotiveDecayFactor(mKey, value);
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);
            if (result != OptionResult.Failure)
            {
                if (Value < 0)
                {
                    Value = 0;
                }

                PriorValues.sFactorChanged = true;
            }

            return result;
        }
    }
}
