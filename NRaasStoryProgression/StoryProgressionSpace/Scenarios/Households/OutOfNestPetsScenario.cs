using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public class OutOfNestPetsScenario : HouseholdScenario
    {
        bool mPassedInspection = false;

        public OutOfNestPetsScenario()
        { }
        protected OutOfNestPetsScenario(OutOfNestPetsScenario scenario)
            : base (scenario)
        {
            mPassedInspection = scenario.mPassedInspection;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "PetsOutOfNest";
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<NewZooScenario.Option, bool>()) return false;

            if (GetValue<RoomToLeaveOption, int>() <= 0) return false;

            return base.Allow();
        }

        protected override bool Allow(Household house)
        {
            if (!Households.Allow(this, house, 0))
            {
                IncStat("User Denied");
                return false;
            }
            else if (!GetValue<AllowAdoptionOption, bool>(SimTypes.HeadOfFamily(house)))
            {
                IncStat("Adoption Denied");
                return false;
            }
            else
            {
                SimDescription head = SimTypes.HeadOfFamily(house);
                if (AddScoring("Cooldown: Between Adoptions", TestElapsedTime<DayOfLastPetOption, PetAdoptionScenario.MinTimeBetweenAdoptionOption>(head)) < 0)
                {
                    AddStat("Too Soon After Adoption", GetElapsedTime<DayOfLastPetOption>(head));
                    return false;
                }
            }

            mPassedInspection = Lots.PassesHomeInspection(this, House.LotHome, HouseholdsEx.Pets(House), ManagerLot.FindLotFlags.InspectPets);

            if ((mPassedInspection) && (!HouseholdsEx.IsFull(this, House, CASAgeGenderFlags.Dog, GetValue<RoomToLeaveOption, int>(), false)))
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(house);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            ScoringList<SimDescription> scoring = new ScoringList<SimDescription>();

            SimDescription head = SimTypes.HeadOfFamily(House);

            SimDescription choice = null;
            int minLiking = int.MaxValue;

            foreach (SimDescription sim in HouseholdsEx.Pets(House))
            {
                // Don't move parent animals out if their children live with them
                bool found = false;
                foreach (SimDescription child in Relationships.GetChildren(sim))
                {
                    if (child.Household == sim.Household)
                    {
                        found = true;
                        break;
                    }
                }

                if (found) continue;

                int liking = ManagerSim.GetLTR(sim, head);
                if (liking < minLiking)
                {
                    choice = sim;
                    minLiking = liking;
                }
            }

            if (choice == null)
            {
                IncStat("No Choice");
                return false;
            }

            Add(frame, new PetAdoptionScenario(choice, true), ScenarioResult.Start);
            Add(frame, new PostScenario(head, choice, mPassedInspection), ScenarioResult.Success);

            return false;
        }

        public override Scenario Clone()
        {
            return new OutOfNestPetsScenario(this);
        }

        public class PostScenario : DualSimScenario
        {
            bool mPassedInspection;

            public PostScenario(SimDescription owner, SimDescription sim, bool passedInspection)
                : base(owner, sim)
            {
                mPassedInspection = passedInspection;
            }
            protected PostScenario(PostScenario scenario)
                : base(scenario)
            {
                mPassedInspection = scenario.mPassedInspection;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type == PrefixType.Pure)
                {
                    return "PetsOutOfNestPost";
                }
                else
                {
                    if (mPassedInspection)
                    {
                        return "PetOvercrowdingMove";
                    }
                    else
                    {
                        return "PetInspectionMove";
                    }
                }
            }

            protected override bool CheckBusy
            {
                get { return false; }
            }

            protected override bool Progressed
            {
                get { return true; }
            }

            protected override ICollection<SimDescription> GetSims()
            {
                return null;
            }

            protected override ICollection<SimDescription> GetTargets(SimDescription sim)
            {
                return null;
            }

            protected override bool AllowSpecies(SimDescription sim, SimDescription target)
            {
                return true;
            }

            protected override bool PrivateUpdate(ScenarioFrame frame)
            {
                return true;
            }

            public override Scenario Clone()
            {
                return new PostScenario(this);
            }
        }

        public class RoomToLeaveOption : IntegerScenarioOptionItem<ManagerHousehold, OutOfNestPetsScenario>
        {
            public RoomToLeaveOption()
                : base(1)
            { }

            public override string GetTitlePrefix()
            {
                return "RoomToLeavePets";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<NewZooScenario.Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
