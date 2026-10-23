using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Settings
{
    public class AlarmSpacing : IntegerOption
    {
        public override string GetTitlePrefix()
        {
            return "AlarmSpacing";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override int Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mAlarmSpacing;
            }
            set
            {
                NRaas.Overwatch.Settings.mAlarmSpacing = value;
            }
        }
    }
}
