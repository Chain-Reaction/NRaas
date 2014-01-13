using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.CommonSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class ScheduledRepairScenario : RepairScenario
    {
        bool mScore = true;

        public ScheduledRepairScenario()
        { }
        public ScheduledRepairScenario(SimDescription sim, GameObject obj)
            : base(sim, obj)
        {
            mScore = false;
        }
        protected ScheduledRepairScenario(ScheduledRepairScenario scenario)
            : base (scenario)
        {
            mScore = scenario.mScore;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "Repair";
            }
            else
            {
                if (TownRepair)
                {
                    return "RepairTown";
                }
                else
                {
                    return "Repair";
                }
            }
        }

        protected override int ContinueChance
        {
            get { return 50; }
        }

        protected override int InitialReportChance
        {
            get { return 25; }
        }

        protected override int ContinueReportChance
        {
            get { return 25; }
        }

        protected override int Rescheduling
        {
            get { return 120; }
        }

        protected override int MaximumReschedules
        {
            get { return 2; }
        }

        protected override bool AlwaysReschedule
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<OptionV2, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            if (mScore)
            {
                return new SimScoringList(this, "Handiness", Sims.TeensAndAdults, false).GetBestByMinScore(1);
            }
            else
            {
                return Sims.TeensAndAdults;
            }
        }

        protected override bool AllowHouse(Household house)
        {
            if (ManagerCareer.HasSkillCareer(Sim, SkillNames.Handiness)) return true;

            if (ManagerCareer.HasSkillCareer(Household.ActiveHousehold, SkillNames.Handiness)) return false;

            bool succcess = false;
            foreach (SimDescription sim in HouseholdsEx.All(house))
            {
                if (ManagerFriendship.AreEnemies(sim, Sim, -25)) return false;

                if (ManagerFriendship.AreFriends(sim, Sim, 25))
                {
                    succcess = true;
                }
            }

            return succcess;
        }

        public static bool PushForRepairman(IScoringGenerator stats, StoryProgressionObject manager, Household house)
        {
            List<SimDescription> choices = manager.Situations.GetFree(stats, new SimScoringList(stats, "Handiness", manager.Sims.TeensAndAdults, false).GetBestByMinScore(1), true);

            Dictionary<Household, List<GameObject>> repairs = new Dictionary<Household, List<GameObject>>();

            foreach (Lot lot in ManagerLot.GetOwnedLots(house))
            {
                GetRepairs(manager, lot.GetObjects<GameObject>(), repairs);
            }

            stats.AddStat("Residents", choices.Count);

            while (choices.Count > 0)
            {
                SimDescription choice = RandomUtil.GetRandomObjectFromList(choices);
                choices.Remove(choice);

                if (choice.CreatedSim == null) continue;

                List<GameObject> repairWork = null;
                if (repairs.TryGetValue(house, out repairWork))
                {
                    if (PushInteractions(manager, choice, repairWork))
                    {
                        stats.IncStat("Resident Repairman");
                        return true;
                    }
                    else
                    {
                        stats.IncStat("Push Fail");
                    }
                }
                else
                {
                    stats.IncStat("No Repairs Fonud");
                }
            }

            stats.IncStat("Service Repairman");

            Repairman instance = Repairman.Instance;
            if (instance != null)
            {
                instance.MakeServiceRequest(house.LotHome, true, ObjectGuid.InvalidObjectGuid);
            }
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (base.PrivateUpdate(frame)) return true;

            if (!mScore)
            {
                PushForRepairman(this, Manager, Sim.Household);
            }
            return false;
        }

        public override Scenario Clone()
        {
            return new ScheduledRepairScenario(this);
        }

        public class OptionV2 : BooleanScenarioOptionItem<ManagerSkill, ScheduledRepairScenario>
        {
            public OptionV2()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RepairPush";
            }
        }
    }
}
