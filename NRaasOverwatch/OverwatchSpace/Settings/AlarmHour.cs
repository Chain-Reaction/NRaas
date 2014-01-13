using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Settings
{
    public class AlarmHour : FloatSettingOption<GameObject>, ISettingOption
    {
        public override string GetTitlePrefix()
        {
            return "AlarmHour";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override float Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mAlarmHour;
            }
            set
            {
                NRaas.Overwatch.Settings.mAlarmHour = value;

                Overwatch.RestartAlarm();
            }
        }

        public OptionResult ChangeSetting(GameHitParameters< GameObject> parameters)
        {
            return Run(parameters);
        }
    }
}
