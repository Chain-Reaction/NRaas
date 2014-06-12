using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
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
    public class ReturnAgingInventoryScenario : SimEventScenario<Event>
    {
        Sim mSim;

        public ReturnAgingInventoryScenario()
            : base ()
        { }
        protected ReturnAgingInventoryScenario(ReturnAgingInventoryScenario scenario)
            : base (scenario)
        {
            mSim = scenario.mSim;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "ReturnAgingInventory";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override ListenHandleType HandleImmediately
        {
            get { return ListenHandleType.Task; }
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kSimInstantiated);
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
            else if (!Inventories.VerifyInventory(mSim.SimDescription))
            {
                IncStat("No Inventory");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Sim == null) return false;

            RetainedInventorySimData data = GetData<RetainedInventorySimData>(Sim);
            if (data == null || data.mInventory == null)
            {
                IncStat("Retention Missing");
                return false;
            }

            if (data.mInventory.Count == 0)
            {
                IncStat("Inventory Empty");
                return false;
            }

            AddStat("Inventory", data.mInventory.Count);

            foreach (GameObject obj in data.mInventory)
            {
                if (mSim.Inventory == null) continue;

                mSim.Inventory.IgnoreInventoryValidation = true;
                try
                {
                    if (Inventories.TryToMove(obj, mSim))
                    {
                        IncStat("Returned " + obj.CatalogName);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mSim, obj, e);
                }
                finally
                {
                    mSim.Inventory.IgnoreInventoryValidation = false;
                }
            }

            data.mInventory.Clear();

            return true;
        }

        public override Scenario Clone()
        {
            return new ReturnAgingInventoryScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim, ReturnAgingInventoryScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "ReturnAgingInventory";
            }
        }
    }
}
