using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interactions;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
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
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Skills
{
    public class BuskerPushScenario : SimSingleProcessScenario, IHasSkill
    {
        static Dictionary<Sims3.Gameplay.Skills.SkillNames,InteractionDefinition> sSkills = null;

        public BuskerPushScenario()
        { }
        protected BuskerPushScenario(BuskerPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Busker";
        }

        public Sims3.Gameplay.Skills.SkillNames[] CheckSkills
        {
            get
            {
                return new Sims3.Gameplay.Skills.SkillNames[0];
            }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            Vector2 hours = GetValue<ValidHoursOption,Vector2>();

            if (!SimClock.IsTimeBetweenTimes(hours.x, hours.y))
            {
                IncStat("Not in Hours");
                return false;
            }

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return new SimScoringList(this, "Guitar", Sims.TeensAndAdults, false).GetBestByMinScore(1);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.ChildOrBelow)
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.CreatedSim.LotCurrent == null)
            {
                IncStat("No Lot");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Skills.Allow(this, sim))
            {
                IncStat("Skill Denied");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
                return false;
            }
            else if ((sim.IsEP11Bot) && (!sim.HasTrait(TraitNames.MusicianChip)))
            {
                IncStat("Chip Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            if (sSkills == null)
            {
                sSkills = new Dictionary<Sims3.Gameplay.Skills.SkillNames,InteractionDefinition>();

                sSkills.Add(Sims3.Gameplay.Skills.SkillNames.Guitar, GuitarPlayForTips.Singleton);
                sSkills.Add(Sims3.Gameplay.Skills.SkillNames.BassGuitar, BassGuitarPlayForTips.Singleton);
                sSkills.Add(Sims3.Gameplay.Skills.SkillNames.Piano, PianoPlayForTips.Singleton);
                sSkills.Add(Sims3.Gameplay.Skills.SkillNames.Drums, DrumsPlayForTips.Singleton);
            }

            List<Sims3.Gameplay.Skills.SkillNames> choices = new List<Sims3.Gameplay.Skills.SkillNames>();

            SimData data = Options.GetSim(Sim);

            foreach (Sims3.Gameplay.Skills.SkillNames skill in sSkills.Keys)
            {
                if (!Skills.AllowSkill(this, Sim, data, skill)) continue;

                choices.Add(skill);
            }

            if (choices.Count == 0)
            {
                IncStat("No Choices");
                return false;
            }

            BandInstrument instrument = null;
            InteractionDefinition definition = null;

            RandomUtil.RandomizeListOfObjects(choices);

            foreach(Sims3.Gameplay.Skills.SkillNames skill in choices)
            {
                switch(skill)
                {
                    case Sims3.Gameplay.Skills.SkillNames.Guitar:
                        instrument = Inventories.InventoryFind<Guitar>(Sim);
                        if (instrument == null)
                        {
                            instrument = ManagedBuyProduct<Guitar>.Purchase(Sim, 0, this, UnlocalizedName, null, BuildBuyProduct.eBuyCategory.kBuyCategoryElectronics, BuildBuyProduct.eBuySubCategory.kBuySubCategoryHobbiesAndSkills);
                        }
                        break;
                    case Sims3.Gameplay.Skills.SkillNames.BassGuitar:
                        instrument = Inventories.InventoryFind<BassGuitar>(Sim);
                        if (instrument == null)
                        {
                            instrument = ManagedBuyProduct<BassGuitar>.Purchase(Sim, 0, this, UnlocalizedName, null, BuildBuyProduct.eBuyCategory.kBuyCategoryElectronics, BuildBuyProduct.eBuySubCategory.kBuySubCategoryHobbiesAndSkills);
                        }
                        break;
                    case Sims3.Gameplay.Skills.SkillNames.Piano:
                        instrument = Inventories.InventoryFind<Piano>(Sim);
                        if (instrument == null)
                        {
                            instrument = ManagedBuyProduct<Piano>.Purchase(Sim, 0, this, UnlocalizedName, null, BuildBuyProduct.eBuyCategory.kBuyCategoryElectronics, BuildBuyProduct.eBuySubCategory.kBuySubCategoryHobbiesAndSkills);
                        }
                        break;
                    case Sims3.Gameplay.Skills.SkillNames.Drums:
                        instrument = Inventories.InventoryFind<Drums>(Sim);
                        if (instrument == null)
                        {
                            instrument = ManagedBuyProduct<Drums>.Purchase(Sim, 0, this, UnlocalizedName, null, BuildBuyProduct.eBuyCategory.kBuyCategoryElectronics, BuildBuyProduct.eBuySubCategory.kBuySubCategoryHobbiesAndSkills);
                        }
                        break;
                }

                if (instrument != null)
                {
                    definition = sSkills[skill];
                    break;
                }
            }

            if (instrument == null)
            {
                IncStat("No Instrument");
                return false;
            }

            Lot lot = Sim.CreatedSim.LotCurrent;

            if ((Sim.LotHome == lot) || (lot.IsWorldLot) || (lot.IsResidentialLot))
            {
                if (!Situations.PushVisit(this, Sim, Lots.GetCommunityLot(Sim.CreatedSim, null, true)))
                {
                    IncStat("Push Lot Fail");
                    return false;
                }
            }

            return Situations.PushInteraction(this, Sim, instrument, definition);
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new BuskerPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, BuskerPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "BuskerPush";
            }
        }

        public class ValidHoursOption : RangeManagerOptionItem<ManagerSkill>
        {
            public ValidHoursOption()
                : base(new Vector2(8, 20), new Vector2(0, 24))
            { }

            public override string GetTitlePrefix()
            {
                return "BuskerValidHours";
            }
        }

        public class TimeLimitOption : IntegerManagerOptionItem<ManagerSkill>
        {
            public TimeLimitOption()
                : base(3)
            { }

            public override string GetTitlePrefix()
            {
                return "BuskerTimeLimit";
            }
        }
    }
}
