using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills;
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
    public class WritingPushScenario : SimSingleProcessScenario, IHasSkill
    {
        public WritingPushScenario()
        { }
        protected WritingPushScenario(WritingPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Writing";
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Writing };
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
            if (GetValue<OnlyWritersOption, bool>())
            {
                return new SimScoringList(this, "ProfessionalWriter", Sims.TeensAndAdults, false).GetBestByMinScore(1);
            }
            else
            {
                return new SimScoringList(this, "Writing", Sims.TeensAndAdults, false).GetBestByMinScore(1);
            }
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
            else if (sim.CreatedSim.Inventory == null)
            {
                IncStat("No Inventory");
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
            else if ((sim.IsEP11Bot) && (!sim.HasTrait(TraitNames.ArtisticAlgorithmsChip)))
            {
                IncStat("Chip Denied");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            ManagedBuyProduct<ComputerLaptop>.Purchase(Sim, 0, this, UnlocalizedName, null, BuildBuyProduct.eBuyCategory.kBuyCategoryElectronics, BuildBuyProduct.eBuySubCategory.kBuySubCategoryComputers);

            List<Computer> computers = new List<Computer>();

            List<Computer> inventory = new List<Computer>();

            foreach (Computer computer in Inventories.InventoryFindAll<Computer>(Sim))
            {
                inventory.Add(computer);

                if (!computer.IsComputerUsable(Sim.CreatedSim, true, false, true)) continue;

                computers.Add(computer);
            }

            if (computers.Count == 0)
            {
                Computer broken = null;

                foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
                {
                    foreach (Computer computer in lot.GetObjects<Computer>())
                    {
                        if (!computer.IsComputerUsable(Sim.CreatedSim, true, false, true))
                        {
                            broken = computer;
                            continue;
                        }

                        computers.Add(computer);
                    }
                }

                if (computers.Count == 0)
                {
                    if (inventory.Count > 0)
                    {
                        foreach (Computer computer in inventory)
                        {
                            Money.Sell(Sim, computer);
                        }

                        IncStat("Sell Broken");
                    }

                    if (broken != null)
                    {
                        IncStat("Push Repair");

                        Add(frame, new ScheduledRepairScenario(Sim, broken), ScenarioResult.Start);
                        return false;
                    }
                    else
                    {
                        IncStat("No Computer");
                        return false;
                    }
                }
            }
            else if ((Sim.CreatedSim.LotCurrent == null) || (!Sim.CreatedSim.LotCurrent.CanSimTreatAsHome(Sim.CreatedSim)))
            {
                Situations.PushGoHome(this, Sim);
            }

            if (GetValue<RefineWritingOption, bool>())
            {
                return Situations.PushInteraction(this, Sim, RandomUtil.GetRandomObjectFromList(computers), Computer.PracticeWriting.Singleton);
            }
            else
            {
                return Situations.PushInteraction(this, Sim, RandomUtil.GetRandomObjectFromList(computers), NRaas.StoryProgressionSpace.Interactions.WriteNovelEx.Singleton);
            }
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new WritingPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, WritingPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "WritingPush";
            }
        }

        public class OnlyWritersOption : BooleanManagerOptionItem<ManagerSkill>
        {
            public OnlyWritersOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "OnlyWriters";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
        public class RefineWritingOption : BooleanManagerOptionItem<ManagerSkill>
        {
            public RefineWritingOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "RefineWriting";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
