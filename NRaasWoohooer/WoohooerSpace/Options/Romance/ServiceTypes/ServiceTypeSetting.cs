using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Romance.ServiceTypes
{
    public class ServiceTypeSetting : BooleanSettingOption<GameObject>, IOptionItem
    {
        ServiceType mType;

        public ServiceTypeSetting(ServiceType type)
        {
            mType = type;
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Woohooer.Settings.mDisallowAutonomousServiceTypes.Contains(mType);
            }
            set
            {
                if (value)
                {
                    if (!NRaas.Woohooer.Settings.mDisallowAutonomousServiceTypes.Contains(mType))
                    {
                        NRaas.Woohooer.Settings.mDisallowAutonomousServiceTypes.Add(mType);
                    }
                }
                else
                {
                    NRaas.Woohooer.Settings.mDisallowAutonomousServiceTypes.Remove(mType);
                }
            }
        }

        public override string GetTitlePrefix()
        {
            return "DisallowService";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        public override string Name
        {
            get
            {
                string title;
                if (!Localization.GetLocalizedString("Ui/Caption/Services/Service:" + mType.ToString(), out title))
                {
                    title = "Ui/Caption/Services/Service:" + mType.ToString();
                }
                return title;
            }
        }
    }
}
