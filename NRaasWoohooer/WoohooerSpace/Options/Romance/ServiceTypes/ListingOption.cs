using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Romance.ServiceTypes
{
    public class ListingOption : OptionList<ServiceTypeSetting>, IRomanceOption
    {
        public override string GetTitlePrefix()
        {
            return "DisallowServiceType";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (NRaas.Woohooer.Settings.mDisallowHomeless) return false;

            return base.Allow(parameters);
        }

        public override List<ServiceTypeSetting> GetOptions()
        {
            List<ServiceTypeSetting> results = new List<ServiceTypeSetting>();

            foreach (ServiceType type in Enum.GetValues(typeof(ServiceType)))
            {
                switch(type)
                {
                    case ServiceType.None:
                    case ServiceType.NewspaperDelivery:
                        continue;
                }

                results.Add(new ServiceTypeSetting(type));
            }

            return results;
        }
    }
}
