using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
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
    public abstract class TransferPropertyScenario : RelationshipScenario, IInvestigationScenario
    {
        string mObjectName = null;

        int mObjectValue = 0;

        bool mFail = false;

        static UpdateDelegate OnInvestigateScenario;

        protected TransferPropertyScenario(int delta)
            : base(delta)
        { }
        protected TransferPropertyScenario(SimDescription sim, int delta)
            : base(sim, delta)
        { }
        protected TransferPropertyScenario(TransferPropertyScenario scenario)
            : base (scenario)
        {
            mObjectName = scenario.mObjectName;
            mObjectValue = scenario.mObjectValue;
            mFail = scenario.mFail;
        }

        protected bool Fail
        {
            get { return mFail; }
        }

        public abstract bool AllowGoToJail
        {
            get;
        }

        public virtual string InvestigateStoryName
        {
            get { return "InvestigateTransferProperty"; }
        }

        public abstract int InvestigateMinimum
        {
            get;
        }

        public abstract int InvestigateMaximum
        {
            get;
        }

        protected virtual int Bail
        {
            get { return Manager.GetValue<GotArrestedScenario.BailOption, int>(); }
        }

        protected virtual int Minimum
        {
            get { return 0; }
        }

        protected abstract int Maximum
        {
            get;
        }

        protected virtual bool ActualTransfer
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return sim.IsHuman;
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if (target.Household == Sim.Household)
            {
                IncStat("Same Home");
                return false;
            }
            else if (target.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }

            return (base.TargetAllow(target));
        }

        protected virtual int GetCost(PropertyData data)
        {
            if (data.PropertyType == RealEstatePropertyType.VacationHome) return 0;

            return data.TotalValue;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Money.Allow(this, sim))
            {
                IncStat("Money Denied");
                return false;
            }

            return (base.CommonAllow(sim));
        }

        protected virtual bool IsFail(SimDescription sim, SimDescription target)
        {
            return false;
        }

        public bool InstallInvestigation(Scenario.UpdateDelegate func)
        {
            OnInvestigateScenario += func;
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<PropertyData> properties = new List<PropertyData>();

            int max = Maximum;
            int min = Minimum;

            foreach (PropertyData property in Sim.Household.RealEstateManager.AllProperties)
            {
                int cost = GetCost(property);
                if (cost <= 0) continue;

                if (cost < min) continue;

                if (cost > max) continue;

                properties.Add(property);
            }

            if (properties.Count == 0) return false;

            mFail = IsFail(Sim, Target);

            if (!mFail)
            {
                PropertyData choice = RandomUtil.GetRandomObjectFromList(properties);

                mObjectName = choice.LocalizedName;
                mObjectValue = GetCost(choice);

                if (ActualTransfer)
                {
                    ManagerMoney.TransferProperty(Sim.Household, Target.Household, choice);
                }
                else
                {
                    Money.AdjustFunds(Target, "PropertyTransfer", -mObjectValue);

                    Money.AdjustFunds(Sim, "PropertyTransfer", mObjectValue);
                }

                if (Delta < 0)
                {
                    TraitFunctions.ItemStolenCallback(Target.Household, Origin.FromTheft);

                    foreach (Sim sim in HouseholdsEx.AllSims(Target.Household))
                    {
                        EventTracker.SendEvent(EventTypeId.kWasRobbed, sim);
                    }
                }
            }

            if (Delta < 0)
            {
                if (OnInvestigateScenario != null)
                {
                    OnInvestigateScenario(this, frame);
                }
            }

            return true;
        }

        protected override bool Push()
        {
            if (mFail)
            {
                if (AllowGoToJail)
                {
                    Manager.AddAlarm(new GoToJailScenario(Sim, Bail));
                }
            }

            return Situations.PushVisit(this, Sim, Target.LotHome);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, Target, mObjectName, mObjectValue };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
