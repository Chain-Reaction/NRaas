using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class ServiceCleanupScenario : ScheduledSoloScenario
    {
        public ServiceCleanupScenario()
        { }
        protected ServiceCleanupScenario(ServiceCleanupScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ServiceCleanup";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Household house = Household.NpcHousehold;
            if (house == null) return false;

            if (house.SharedFamilyInventory == null)
            {
                IncStat("Inventory Restored");

                house.mSharedFamilyInventory = Sims3.Gameplay.SharedFamilyInventory.Create(house);
            }
            else if (house.SharedFamilyInventory.Inventory == null)
            {
                IncStat("Inventory Restored");

                house.SharedFamilyInventory.OnStartup();
            }
            else
            {
                List<InventoryItem> items = house.SharedFamilyInventory.Inventory.DestroyInventoryAndStoreInList();
                if (items != null)
                {
                    foreach (InventoryItem item in items)
                    {
                        try
                        {
                            item.Object.Destroy();
                            IncStat("Inventory Item Disposed");
                        }
                        catch (Exception e)
                        {
                            Common.DebugException(item.Object.GetType().ToString(), e);
                        }
                    }
                }
            }

            return true;
        }

        public override Scenario Clone()
        {
            return new ServiceCleanupScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerHousehold, ServiceCleanupScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ServiceHouseholdCleanup";
            }
        }
    }
}
