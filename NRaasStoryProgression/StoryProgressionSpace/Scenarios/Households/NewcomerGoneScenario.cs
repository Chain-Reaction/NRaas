using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class NewcomerGoneScenario : HouseholdEventScenario
    {
        SimDescription mHead = null;

        bool mReport = true;

        public NewcomerGoneScenario()
        { }
        public NewcomerGoneScenario(Household house)
            : base (house)
        { }
        protected NewcomerGoneScenario(NewcomerGoneScenario scenario)
            : base (scenario)
        {
            mHead = scenario.mHead;
            mReport = scenario.mReport;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "NewcomerGone";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool ShouldReport
        {
            get { return mReport; }
        }

        protected override bool ForceAlert
        {
            get { return true; }
        }

        protected override Scenario Handle(Event e, ref ListenerAction result)
        {
            mReport = false;

            return base.Handle(e, ref result);
        }

        public static bool HasActiveRelation(Household home)
        {
            if (Household.ActiveHousehold == null) return false;

            foreach (SimDescription sim in HouseholdsEx.All(home))
            {
                foreach (SimDescription active in HouseholdsEx.All(Household.ActiveHousehold))
                {
                    if (Relationships.IsCloselyRelated(sim, active, false))
                    {
                        return true;
                    }

                    Relationship relation = Relationship.Get(sim, active, false);
                    if (relation == null) continue;

                    if (relation.AreFriendsOrRomantic()) return true;
                }
            }

            return false;
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kFamilyMovedToTown);
        }

        protected override bool Allow()
        {
            // Intentionally not negated
            if (GetValue<AllowHomelessMoveInOptionV2, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (house.InWorld)
            {
                IncStat("Resident");
                return false;
            }
            else if (HasActiveRelation(house))
            {
                IncStat("Relation Save");
                return false;
            }
            else if (HouseholdsEx.IsActiveDaycare(house))
            {
                IncStat("Active Daycare");
                return false;
            }
            else if (HouseholdsEx.IsPassport(house))
            {
                IncStat("Passport");
                return false;
            }
            else if (HouseholdsEx.IsRole(house))
            {
                IncStat("Role House");
                return false;
            }
            else if (HouseholdsEx.IsLunarCycleZombie(house))
            {
                IncStat("Zombie House");
                return false;
            }
            else if (HouseholdsEx.IsActiveCoworker(house))
            {
                IncStat("Active Coworker");
                return false;
            }
            else if (SimTypes.IsSpecial(house))
            {
                IncStat("Special");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Households.MatchesAlertLevel(HouseholdsEx.All(House)))
            {
                mHead = SimTypes.HeadOfFamily(House);
            }

            return Households.EliminateHousehold(UnlocalizedName, House);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (manager == null)
            {
                manager = Households;
            }

            if (parameters == null)
            {
                if (mHead == null) return null;

                parameters = new object [] { mHead };
            }

            ManagerStory.Story story = base.PrintStory(manager, name, parameters, extended, logging);
            if (story != null)
            {
                story.mShowNoImage = true;
            }
            return story;
        }

        public override Scenario Clone()
        {
            return new NewcomerGoneScenario(this);
        }

        public class AllowHomelessMoveInOptionV2 : BooleanEventOptionItem<ManagerHousehold, NewcomerGoneScenario>, IDebuggingOption
        {
            public AllowHomelessMoveInOptionV2()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowHomelessMoveIn";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<ScheduledHomelessScenario.OptionV2, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
