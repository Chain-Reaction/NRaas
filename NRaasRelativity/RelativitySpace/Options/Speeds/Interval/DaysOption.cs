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
    public class DaysOption : ListedSettingOption<DaysOfTheWeek, GameObject>, IIntervalOption
    {
        SpeedInterval mInterval;

        public DaysOption()
        { }
        public DaysOption(SpeedInterval interval)
        {
            mInterval = interval;
        }

        public override string GetTitlePrefix()
        {
            return "Days";
        }

        protected override string GetValuePrefix()
        {
            return "Day";
        }

        public override string DisplayValue
        {
            get
            {
                return mInterval.GetDays();
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(0, mInterval); }
        }

        protected override bool Allow(DaysOfTheWeek value)
        {
            if ((value == DaysOfTheWeek.None) || (value == DaysOfTheWeek.All)) return false;

            return true;
        }

        protected override Proxy GetList()
        {
            return new ListProxy(mInterval.mDays);
        }

        public override string ConvertToString(DaysOfTheWeek value)
        {
            return value.ToString();
        }

        public override bool ConvertFromString(string value, out DaysOfTheWeek newValue)
        {
            return ParserFunctions.TryParseEnum<DaysOfTheWeek>(value, out newValue, DaysOfTheWeek.None);
        }

        protected override void PrivatePerform(IEnumerable<Item> results)
        {
            base.PrivatePerform(results);

            mInterval.mDays.Sort(new Comparison<DaysOfTheWeek>(OnSort));
        }

        public static int OnSort(DaysOfTheWeek left, DaysOfTheWeek right)
        {
            int leftValue = (int)left;
            int rightValue = (int)right;

            return leftValue.CompareTo(rightValue);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);
            if (result != OptionResult.Failure)
            {
                PersistedSettings.ResetSpeeds();
            }
            return result;
        }

        public IIntervalOption Clone(SpeedInterval interval)
        {
            return new DaysOption(interval);
        }

        public override string ExportName
        {
            get { return null; }
        }
    }
}
