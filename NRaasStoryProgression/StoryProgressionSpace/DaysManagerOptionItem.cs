using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace
{
    public abstract class DaysManagerOptionItem<TManager> : EnumManagerOptionItem<TManager, DaysOfTheWeek>
        where TManager : StoryProgressionObject
    {
        protected static readonly DaysOfTheWeek sAllDays = DaysOfTheWeek.Monday | DaysOfTheWeek.Tuesday | DaysOfTheWeek.Wednesday | DaysOfTheWeek.Thursday | DaysOfTheWeek.Friday | DaysOfTheWeek.Saturday | DaysOfTheWeek.Sunday;

        public DaysManagerOptionItem(DaysOfTheWeek value)
            : base(value)
        { }

        protected override string GetLocalizationValueKey()
        {
            return "Days";
        }

        protected override string GetLocalizationUIKey()
        {
            return "DaysAbbreviation";
        }

        protected override int NumSelectable
        {
            get { return 0; }
        }

        public static DaysOfTheWeek ConvertFromList(DaysOfTheWeek[] days)
        {
            DaysOfTheWeek result = DaysOfTheWeek.None;

            foreach (DaysOfTheWeek day in days)
            {
                result |= day;
            }

            return result;
        }

        protected override DaysOfTheWeek Convert(int value)
        {
            return (DaysOfTheWeek)value;
        }

        protected override DaysOfTheWeek Combine(DaysOfTheWeek original, DaysOfTheWeek add, out bool same)
        {
            DaysOfTheWeek result = original | add;

            same = (result == original);

            return result;
        }

        protected override string ConvertToUIValue(string prefixKey, DaysOfTheWeek list)
        {
            if (list == sAllDays)
            {
                return Common.Localize(prefixKey + ":All");
            }

            return base.ConvertToUIValue(prefixKey, list);
        }

        protected override bool Allow(DaysOfTheWeek value)
        {
            switch (value)
            {
                case DaysOfTheWeek.All:
                case DaysOfTheWeek.None:
                    return false;
            }

            return base.Allow(value);
        }
    }
}

