using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Options.General
{
    public class HomeSchoolingPerformancePerHomeworkSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mPerformancePerHomework;
            }
            set
            {
                NRaas.Careers.Settings.mPerformancePerHomework = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "HomeSchoolingPerformancePerHomework";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Localization.HasLocalizationString("NRaas.Careers.HomeSchoolingPerformancePerHomework:MenuName")) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
