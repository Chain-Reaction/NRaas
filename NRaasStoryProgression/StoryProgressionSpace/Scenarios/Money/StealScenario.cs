using NRaas.CommonSpace.Helpers;
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
    public abstract class StealScenario : RelationshipScenario, IInvestigationScenario
    {
        string mObjectName = null;

        int mObjectValue = 0;

        bool mFail = false;

        static UpdateDelegate OnInvestigateScenario;

        protected StealScenario(int delta)
            : base(delta)
        { }
        protected StealScenario(SimDescription sim, int delta)
            : base(sim, delta)
        { }
        protected StealScenario(StealScenario scenario)
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
            get { return "InvestigateSteal"; }
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

        public abstract bool KeepObject
        {
            get;
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool TestRelationship
        {
            get { return false; }
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim.Household == Sim.Household)
            {
                IncStat("Same Home");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }

            return (base.TargetAllow(sim));
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
            List<GameObject> objects = new List<GameObject>();

            int min = Minimum;
            int max = Maximum;

            AddStat("Minimum", min);
            AddStat("Maximum", max);

            foreach (Lot lot in ManagerLot.GetOwnedLots(Target))
            {
                foreach (GameObject obj in lot.GetObjects<GameObject>())
                {
                    if (!obj.IsStealable()) continue;

                    if (string.IsNullOrEmpty(obj.CatalogName)) continue;

                    if (obj.Value <= 0) continue;

                    if (obj.Value < min) continue;

                    if (obj.Value > max) continue;

                    objects.Add(obj);
                }
            }

            if (objects.Count == 0) return false;

            mFail = IsFail(Sim, Target);

            GameObject choice = RandomUtil.GetRandomObjectFromList(objects);

            mObjectName = choice.CatalogName;
            mObjectValue = choice.Value;

            if (!mFail)
            {
                EventTracker.SendEvent(EventTypeId.kStoleObject, Sim.CreatedSim, choice);

                AddStat("Object Value", mObjectValue);

                if ((KeepObject) && (Sim.CreatedSim != null))
                {
                    if (!Inventories.TryToMove(choice, Sim.CreatedSim))
                    {
                        return false;
                    }

                    choice.SetStealActors(Sim.CreatedSim, Target.CreatedSim);
                }
                else
                {
                    Money.AdjustFunds(Target, "Burgled", -mObjectValue);

                    Money.AdjustFunds(Sim, "Burgled", mObjectValue);
                }

                TraitFunctions.ItemStolenCallback(Target.Household, Origin.FromBurglar);

                foreach (Sim sim in HouseholdsEx.AllSims(Target.Household))
                {
                    EventTracker.SendEvent(EventTypeId.kWasRobbed, sim);
                }
            }

            if (OnInvestigateScenario != null)
            {
                OnInvestigateScenario(this, frame);
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

            if (extended == null)
            {
                extended = new string[] { mObjectName, EAText.GetNumberString(mObjectValue) };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }
    }
}
