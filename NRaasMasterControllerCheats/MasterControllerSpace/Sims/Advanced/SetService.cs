using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Households;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class SetService : SimFromList, IAdvancedOption
    {
        IApplyOptionItem mPool;

        public override string GetTitlePrefix()
        {
            return "SetService";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Household == null) return false;

            if (me.Household.IsServiceNpcHousehold) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<IApplyOptionItem> services = new List<IApplyOptionItem>();

                foreach (ServiceType type in Enum.GetValues(typeof(ServiceType)))
                {
                    int count = 0;

                    bool found = false;

                    foreach (Service service in Services.AllServices)
                    {
                        if (service.ServiceType == type)
                        {
                            count = service.mPool.Count;

                            found = true;
                            break;
                        }
                    }

                    if (!found) continue;

                    services.Add(new StatusServiceType.Item(type, count));
                }

                foreach(PetPoolType type in Enum.GetValues(typeof(PetPoolType)))
                {
                    if (PetPoolManager.IsPoolFull(type)) continue;

                    int count = PetPoolManager.GetPoolSize(type);                    

                    services.Add(new StatusPetPool.Item(type, count));
                }

                if (GameUtils.IsInstalled(ProductVersion.EP10))
                {
                    services.Add(new MermaidServiceItem(CommonSpace.Helpers.Households.NumSims(Household.MermaidHousehold)));
                }

                IApplyOptionItem selection = new CommonSelection<IApplyOptionItem>(Name, me.FullName, services).SelectSingle();
                if (selection == null) return false;

                mPool = selection;
            }

            List<IMiniSimDescription> list = new List<IMiniSimDescription>();
            list.Add(me);

            if (!AddSim.TestForRemainingActive(list))
            {
                Common.Notify(Common.Localize("AddSim:ActiveFail"));
                return false;
            }

            mPool.Apply(me);
            return true;
        }

        public class MermaidServiceItem : CommonOptionItem, IApplyOptionItem
        {
            public MermaidServiceItem()
            { }
            public MermaidServiceItem(int count)
                : base(OccultTypeHelper.GetLocalizedName(Sims3.UI.Hud.OccultTypes.Mermaid), count)
            { }

            public override string DisplayValue
            {
                get { return EAText.GetNumberString(Count); }
            }

            public bool Apply(SimDescription me)
            {
                if (me.CreatedSim == Sim.ActiveActor)
                {
                    LotManager.SelectNextSim();
                }

                Household house = me.Household;
                if (house != null)
                {
                    house.Remove(me, !SimTypes.IsSpecial(house));
                }

                try
                {
                    Household.MermaidHousehold.Add(me);
                }
                catch(Exception e)
                {
                    Common.Exception(me, e);

                    if ((house != null) && (me.Household == null))
                    {
                        house.Add(me);
                    }
                }

                return false;
            }
        }
    }
}
