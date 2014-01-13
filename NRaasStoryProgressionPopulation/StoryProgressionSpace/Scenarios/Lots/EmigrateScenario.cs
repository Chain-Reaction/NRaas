using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Lots
{
    public abstract class EmigrateScenario : HouseholdScenario
    {
        SimDescription mHead = null;

        public EmigrateScenario()
        { }
        protected EmigrateScenario(EmigrateScenario scenario)
            : base (scenario)
        {
            mHead = scenario.mHead;
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool ForceAlert
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected override bool Allow(Household house)
        {
            if (!base.Allow(house)) return false;

            if (!Households.Allow(this, house, GetValue<ManagerHousehold.MinTimeBetweenMovesOption, int>()))
            {
                IncStat("Cooldown Denied");
                return false;
            }
            else if (GetValue<IsAncestralOption, bool>(House))
            {
                IncStat("Ancestral");
                return false;
            }
            else
            {
                bool checkRelation = GetValue<CheckActiveRelationshipOption, bool>();

                foreach (SimDescription member in HouseholdsEx.All(House))
                {
                    if (!GetValue<EmigrationOption, bool>(member))
                    {
                        IncStat("Emigrate Denied");
                        return false;
                    }
                    else if (IsInvolvedInPregnancy(member))
                    {
                        IncStat("Pregnancy Denied");
                        return false;
                    }
                    else if ((checkRelation) && (Sims3.Gameplay.StoryProgression.Notifications.HasSignificantRelationship(Household.ActiveHousehold, member)))
                    {
                        IncStat("Active Denied");
                        return false;
                    }
                }
            }
            return true;
        }

        protected static bool IsInvolvedInPregnancy(SimDescription sim)
        {
            if (sim.IsPregnant) return true;

            foreach (SimDescription other in SimListing.GetResidents(false).Values)
            {
                if (!other.IsPregnant) continue;

                if (other.Pregnancy.DadDescriptionId == sim.SimDescriptionId) return true;
            }

            return false;
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
            if (parameters == null)
            {
                if (mHead == null) return null;

                parameters = new object[] { mHead };
            }

            ManagerStory.Story story = base.PrintStory(manager, name, parameters, extended, logging);
            if (story != null)
            {
                story.mShowNoImage = true;
            }
            return story;
        }

        public class CheckActiveRelationshipOption : BooleanManagerOptionItem<ManagerLot>, ManagerLot.IImmigrationEmigrationOption
        {
            public CheckActiveRelationshipOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "CheckActiveRelationshipEmigration";
            }
        }
    }
}
