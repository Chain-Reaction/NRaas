using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class RetainAgingInventoryScenario : SimEventScenario<Event>
    {
        Sim mSim;

        public RetainAgingInventoryScenario()
            : base ()
        { }
        protected RetainAgingInventoryScenario(RetainAgingInventoryScenario scenario)
            : base (scenario)
        {
            mSim = scenario.mSim;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "RetainAgingInventory";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Immediate; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimDestroyed);
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            mSim = e.TargetObject as Sim;
            if (mSim == null) return null;

            return base.Handle(e, ref result);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }
            else if (mSim == null)
            {
                IncStat("No Sim");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("No Household");
                return false;
            }
            else if (sim.Household.SharedFamilyInventory == null)
            {
                IncStat("No Family Inventory (A)");
                return false;
            }
            else if (sim.Household.SharedFamilyInventory.Inventory == null)
            {
                IncStat("No Family Inventory (B)");
                return false;
            }
            else if (!Inventories.VerifyInventory(mSim.SimDescription))
            {
                IncStat("No Inventory");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            RetainedInventorySimData data = GetData<RetainedInventorySimData>(Sim);

            data.mInventory.Clear();

            Inventory familyInventory = Sim.Household.SharedFamilyInventory.Inventory;

            familyInventory.IgnoreInventoryValidation = true;
            try
            {
                foreach (GameObject obj in Inventories.InventoryFindAll<GameObject>(mSim.SimDescription))
                {
                    if (obj is IHiddenInInventory) continue;

                    try
                    {
                        if (familyInventory.TryToMove(obj))
                        {
                            data.mInventory.Add(obj);

                            IncStat("Retained " + obj.CatalogName);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(mSim, obj, e);
                    }
                }
            }
            finally
            {
                familyInventory.IgnoreInventoryValidation = false;
            }

            AddStat("Inventory", data.mInventory.Count);

            return true;
        }

        public override Scenario Clone()
        {
            return new RetainAgingInventoryScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim, RetainAgingInventoryScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RetainAgingInventory";
            }
        }
    }
}
