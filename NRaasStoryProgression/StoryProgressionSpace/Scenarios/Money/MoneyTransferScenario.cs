using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
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
using Sims3.Gameplay.Objects;
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
    public abstract class MoneyTransferScenario : RelationshipScenario
    {
        int mFunds = 0;

        public MoneyTransferScenario(int delta)
            : base(delta)
        { }
        protected MoneyTransferScenario(SimDescription sim, int delta)
            : base(sim, delta)
        { }
        protected MoneyTransferScenario(SimDescription sim, SimDescription target, int delta)
            : base(sim, target, delta)
        { }
        protected MoneyTransferScenario(MoneyTransferScenario scenario)
            : base (scenario)
        { }

        protected abstract string AccountingKey
        {
            get;
        }

        protected virtual int Minimum
        {
            get { return 0; }
        }

        protected abstract int Maximum
        {
            get;
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected virtual bool AllowDebt
        {
            get { return false; }
        }

        protected int Funds
        {
            get { return mFunds; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool Allow()
        {
            if (Maximum <= 0)
            {
                IncStat("No Maximum");
                return false;
            }

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsHuman;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Money.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.Household == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (SimTypes.IsService(sim))
            {
                IncStat("Service");
                return false;
            }

            return (base.CommonAllow(sim));
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (Target.Household == Sim.Household)
            {
                IncStat("Same Home");
                return false;
            }

            return (base.TargetAllow(sim));
        }

        protected virtual int GetFunds(int min, int max)
        {
            return RandomUtil.GetInt(min, max);
        }

        protected virtual bool AdjustFunds()
        {
            Money.AdjustFunds(Target, AccountingKey, -mFunds);

            Money.AdjustFunds(Sim, AccountingKey, mFunds);
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int minimum = Minimum;
            int maximum = Maximum;

            AddStat("Minimum", minimum);
            AddStat("Maximum", maximum);

            if (!AllowDebt)
            {
                if (minimum > Target.FamilyFunds)
                {
                    AddStat("High Min", Target.FamilyFunds);
                    return false;
                }

                if (maximum > Target.FamilyFunds)
                {
                    maximum = Target.FamilyFunds;
                }

                AddStat("Maximum Modified", maximum);
            }

            if (maximum <= 0)
            {
                IncStat("No Max");
                return false;
            }
            else if (minimum > maximum)
            {
                IncStat("Min > Max");
                return false;
            }

            mFunds = GetFunds(minimum, maximum);
            if (mFunds < minimum)
            {
                IncStat("Too Little");
                return false;
            }
            else if (mFunds > maximum)
            {
                IncStat("Too Much");
                return false;
            }
            else if (mFunds <= 0)
            {
                IncStat("None");
                return false;
            }

            if (!base.PrivateUpdate(frame)) return false;

            AddStat("Funds", mFunds);

            return AdjustFunds();
        }

        protected override bool Push()
        {
            return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Either, FirstAction);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, Target, mFunds };
            }

            if (extended == null)
            {
                extended = new string[] { EAText.GetNumberString(mFunds) };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
