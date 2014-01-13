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

namespace NRaas.RelativitySpace.Options.Speeds.Interval
{
    public class Speed : SpeedBase, IIntervalOption
    {
        SpeedInterval mInterval;

        public Speed()
        { }
        public Speed(SpeedInterval interval)
        {
            mInterval = interval;
        }

        protected override int Value
        {
            get
            {
                return mInterval.mSpeed;
            }
            set
            {
                mInterval.mSpeed = value;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(0, mInterval); }
        }

        protected override string GetPrompt()
        {
            return Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { Relativity.sOneMinute });
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);
            if (result != OptionResult.Failure)
            {
                if (Value < 1)
                {
                    Value = 1;
                }

                PersistedSettings.ResetSpeeds();
            }
            return result;
        }

        public IIntervalOption Clone(SpeedInterval interval)
        {
            return new Speed(interval);
        }

        public override string ExportName
        {
            get { return null; }
        }
    }
}
