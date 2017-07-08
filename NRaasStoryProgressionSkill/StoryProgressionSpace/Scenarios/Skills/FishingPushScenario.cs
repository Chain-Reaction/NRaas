using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios.Money;
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
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class FishingPushScenario : SimScenario, IHasSkill
    {
        List<FishingSpot> mSpots = null;

        public FishingPushScenario()
        { }
        protected FishingPushScenario(FishingPushScenario scenario)
            : base (scenario)
        {
            mSpots = scenario.mSpots;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "Fishing";
        }

        protected override bool CheckBusy
        {
            get { return true; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 25; }
        }
        /*
        protected override int Rescheduling
        {
            get { return 60; }
        }

        protected override int MaximumReschedules
        {
            get { return 4; }
        }
        */

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Fishing };
            }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return new SimScoringList(this, "Fishing", Sims.All, false).GetBestByMinScore(1);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ToddlerOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skill Denied");
                return false;
            }
            else if (!Sims.AllowInventory(this, sim, Managers.Manager.AllowCheck.None))
            {
                IncStat("Inventory Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }
            else if ((sim.IsEP11Bot) && (!sim.HasTrait(TraitNames.FisherBotChip)))
            {
                IncStat("Chip Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override Scenario.GatherResult Gather(List<Scenario> list, ref int continueChance, ref int maximum, ref bool random)
        {
            mSpots = new List<FishingSpot>(Sims3.Gameplay.Queries.GetObjects<FishingSpot>());
            if (mSpots.Count == 0)
            {
                IncStat("No Spots");
                return GatherResult.Failure;
            }

            return base.Gather(list, ref continueChance, ref maximum, ref random);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return Situations.PushInteraction(this, Sim, RandomUtil.GetRandomObjectFromList(mSpots), FishAutonomouslyEx.Singleton);
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new FishingPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, FishingPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "FishingPush";
            }
        }
    }
}
