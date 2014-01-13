using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class StatusServiceType : SelectionTestableOptionList<StatusServiceType.Item, ServiceType, ServiceType>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.StatusServiceType";
        }

        public override bool IsSpecial
        {
            get { return true; }
        }

        public class Item : TestableOption<ServiceType,ServiceType>, IApplyOptionItem
        {
            public Item()
            { }
            public Item(ServiceType type, int count)
                : base(type, GetName(type), count)
            { }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<ServiceType, ServiceType> results)
            {
                if (me.AssignedRole != null) return false;

                if (!SimTypes.InServicePool(me)) return false;

                if (me.CreatedByService == null) return false;

                results[me.CreatedByService.ServiceType] = me.CreatedByService.ServiceType;
                return true;
            }

            public override void SetValue(ServiceType value, ServiceType storeType)
            {
                mValue = value;

                mName = GetName(value);
            }

            protected static string GetName(ServiceType type)
            {
                string serviceName;
                if (!Localization.GetLocalizedString("Ui/Caption/Services/Service:" + type.ToString(), out serviceName))
                {
                    serviceName = "Ui/Caption/Services/Service:" + type.ToString();
                }

                if (!string.IsNullOrEmpty(serviceName))
                {
                    serviceName = serviceName.Replace(Common.NewLine, "").Replace("\n", "");
                }

                return serviceName;
            }

            public bool Apply(SimDescription me)
            {
                Service choice = null;

                foreach (Service service in Services.AllServices)
                {
                    if (service.ServiceType == Value)
                    {
                        choice = service;
                        break;
                    }
                }

                if (choice == null) return false;

                if (me.CreatedSim == Sim.ActiveActor)
                {
                    LotManager.SelectNextSim();
                }

                if (me.Household != null)
                {
                    me.Household.Remove(me);
                }

                Service.InitialServiceNpcSetup(choice, me);

                me.FindSuitableVirtualHome();

                choice.SetServiceNPCProperties(me);
                choice.OverlayUniform(me, choice.ServiceType.ToString());

                choice.AddSimToPool(me);
                return true;
            }
        }
    }
}
