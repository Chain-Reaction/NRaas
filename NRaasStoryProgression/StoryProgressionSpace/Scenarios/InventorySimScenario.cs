using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios
{
    public abstract class InventorySimScenario : RabbitHolePushScenario
    {
        protected int mFunds;

        public InventorySimScenario()
            : base ()
        { }
        protected InventorySimScenario(InventorySimScenario scenario)
            : base (scenario)
        { }

        protected abstract List<GameObject> GetInventory(SimDescription sim);

        protected override bool ShouldReport
        {
            get { return (mFunds > 0); }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.Household == null)
            {
                IncStat("No Home");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!Inventories.VerifyInventory(sim))
            {
                IncStat("No Inventory");
                return false;
            }
            else if (!Sims.AllowInventory(this, sim, AllowActive ? Managers.Manager.AllowCheck.None : Managers.Manager.AllowCheck.Active))
            {
                IncStat("Inventory Denied");
                return false;
            }
            /* Time consuming
            else if (GetInventory(sim).Count == 0)
            {
                IncStat("Empty");
                return false;
            }
            */

            return base.Allow(sim);
        }

        protected override RabbitHoleType GetRabbitHole()
        {
            return RabbitHoleType.Grocery;
        }

        protected override bool Push()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP2))
            {
                if (Situations.PushToObject<ConsignmentRegister>(this, Sim))
                {
                    return true;
                }
            }
            return base.Push();
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, mFunds };
            }

            if (extended == null)
            {
                extended = new string[] { EAText.GetNumberString(mFunds) };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
