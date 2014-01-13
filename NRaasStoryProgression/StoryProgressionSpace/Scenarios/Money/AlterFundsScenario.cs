using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scoring;
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
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public abstract class AlterFundsScenario : SimScenario
    {
        int mFunds = 0;

        public AlterFundsScenario()
        { }
        protected AlterFundsScenario(AlterFundsScenario scenario)
            : base (scenario)
        {
            mFunds = scenario.mFunds;
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected abstract int Minimum
        {
            get;
        }

        protected abstract int Maximum
        {
            get;
        }

        protected abstract string AccountingKey
        {
            get;
        }

        protected abstract bool Subtraction
        {
            get;
        }

        protected virtual int Funds
        {
            get
            {
                if (Sim == null) return 0;

                if (Sim.Household == null) return 0;

                return Sim.FamilyFunds;
            }
        }

        protected virtual bool AllowDebt
        {
            get { return false; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Not Resident");
                return false;
            }
            else if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (Maximum <= 0)
            {
                IncStat("No Maximum");
                return false;
            }
            else if (!Money.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (SimTypes.IsSpecial(sim))
            {
                IncStat("Special");
                return false;
            }

            return (base.Allow(sim));
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int minimum = Minimum;
            int maximum = Maximum;

            AddStat("Minimum", minimum);
            AddStat("Maximum", maximum);

            if (Subtraction)
            {
                if (!AllowDebt)
                {
                    int availableFunds = Funds;

                    if (minimum > availableFunds)
                    {
                        AddStat("High Min", availableFunds);
                        return false;
                    }

                    if (maximum > availableFunds)
                    {
                        maximum = availableFunds;
                    }

                    AddStat("Maximum Modified", maximum);
                }
            }

            if (maximum <= 0)
            {
                IncStat("No Max");
                return false;
            }
            else if (minimum > maximum)
            {
                IncStat("Min Bigger");
                return false;
            }

            mFunds = RandomUtil.GetInt(minimum, maximum);
            if (mFunds == 0)
            {
                IncStat("No Change");
                return false;
            }

            int funds = mFunds;
            if (Subtraction)
            {
                funds = -funds;

                IncStat("Subtraction");
            }

            AddStat("Funds", funds);

            Money.AdjustFunds(Sim, AccountingKey, funds);
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, mFunds };
            }

            if (extended == null)
            {
                extended = new string[] { EAText.GetMoneyString(mFunds) };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
