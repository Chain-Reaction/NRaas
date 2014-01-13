using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class HomeworkScenario : SimScenario
    {
        bool mPush;

        public HomeworkScenario()
            : base ()
        { }
        protected HomeworkScenario(HomeworkScenario scenario)
            : base (scenario)
        {
            mPush = scenario.mPush;
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected abstract int Minimum
        {
            get;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("No Home");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if (sim.CareerManager.School == null)
            {
                IncStat("No School");
                return false;
            }

            return base.Allow(sim);
        }

        public static bool Perform(Common.IStatGenerator stats, SimDescription sim, int minimum)
        {
            bool push = false;
            return Perform(stats, sim, minimum, ref push);
        }
        protected static bool Perform(Common.IStatGenerator stats, SimDescription sim, int amount, ref bool push)
        {
            if (sim.CreatedSim == null)
            {
                stats.IncStat("Hibernating");
                return false;
            }
            else if (!Inventories.VerifyInventory(sim))
            {
                stats.IncStat("No Inventory");
                return false;
            }

            School school = sim.CareerManager.School;
            if (school != null)
            {
                if (school.OwnersHomework == null)
                {
                    school.OwnersHomework = GlobalFunctions.CreateObjectOutOfWorld("Homework") as Homework;
                    school.OwnersHomework.OwningSimDescription = sim;
                    Inventories.TryToMove(school.OwnersHomework, sim.CreatedSim);

                    stats.IncStat("Homework Created");
                }

                if ((!sim.CreatedSim.Inventory.Contains(school.OwnersHomework)) &&
                    (school.OwnersHomework.LotCurrent != sim.CreatedSim.LotCurrent) &&
                    (school.OwnersHomework.UseCount == 0))
                {
                    Inventories.TryToMove(school.OwnersHomework, sim.CreatedSim);

                    stats.IncStat("Homework Moved");
                }
            }
 
            if (amount > 100)
            {
                amount = 100;
            }

            bool success = false;

            foreach (Homework obj in Inventories.InventoryFindAll<Homework>(sim))
            {
                if (obj.PercentComplete < 0) continue;

                if ((amount < 0) || (obj.PercentComplete < amount))
                {
                    obj.PercentComplete = amount;
                    stats.AddStat("Homework Value", amount);

                    if (amount > 90)
                    {
                        push = true;
                    }

                    success = true;
                }               
            }
            return success;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return Perform(this, Sim, Minimum, ref mPush);
        }

        protected override bool Push()
        {
            if (!mPush) return false;

            return Situations.PushInteraction(this, Sim, Sim.CreatedSim, Sims3.Gameplay.Actors.Sim.GoToLibrary.Singleton as InteractionDefinition);
        }
    }
}
