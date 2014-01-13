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
    public class EndHour : IntegerSettingOption<GameObject>, IIntervalOption
    {
        SpeedInterval mInterval;

        public EndHour()
        { }
        public EndHour(SpeedInterval interval)
        {
            mInterval = interval;
        }

        public override string GetTitlePrefix()
        {
            return "EndHour";
        }

        protected override int Value
        {
            get
            {
                return mInterval.mEndHour;
            }
            set
            {
                mInterval.mEndHour = value;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(0, mInterval); }
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
                else if (Value > 24)
                {
                    Value = 24;
                }

                PersistedSettings.ResetSpeeds();
            }
            return result;
        }

        public IIntervalOption Clone(SpeedInterval interval)
        {
            return new EndHour(interval);
        }

        public override string ExportName
        {
            get { return null; }
        }
    }
}
