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
    public class BuskerLevelToGetPaidForConcertsSetting : IntegerSettingOption<GameObject>, IGeneralOption
    {
        protected override int Value
        {
            get
            {
                return NRaas.Careers.Settings.mBuskerLevelToGetPaidForConcerts;
            }
            set
            {
                NRaas.Careers.Settings.mBuskerLevelToGetPaidForConcerts = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "BuskerLevelToGetPaidForConcerts";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Localization.HasLocalizationString("NRaas.Careers.BuskerLevelToGetPaidForConcerts:MenuName")) return false;

            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
