using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public abstract class PurchaseDeedsBaseScenario : SimScenario
    {
        public PurchaseDeedsBaseScenario()
        { }
        protected PurchaseDeedsBaseScenario(PurchaseDeedsBaseScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "PurchaseDeeds";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected abstract int MinimumWealth
        {
            get;
        }

        protected virtual bool TestScoring(SimDescription sim)
        {
            return (AddScoring("PurchaseDeeds", sim) > 0);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (Common.IsOnTrueVacation())
            {
                IncStat("Vacation");
                return false;
            }

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (Deaths.IsDying(sim))
            {
                IncStat("Dying");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("No Home");
                return false;
            }
            else if ((sim.FamilyFunds - MinimumWealth) < 0)
            {
                IncStat("No Money");
                return false;
            }
            else if (!Households.AllowGuardian(sim))
            {
                IncStat("Too Young");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }
            else if (!GetValue<AllowPurchaseDeedsOption, bool>(sim))
            {
                IncStat("Purchase Denied");
                return false;
            }
            else if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }
            else if (!TestScoring(sim))
            {
                IncStat("Score Fail");
                return false;
            }
            else if (sim.Household.RealEstateManager == null)
            {
                IncStat("No Manager");
                return false;
            }

            return base.Allow(sim);
        }
    }
}
