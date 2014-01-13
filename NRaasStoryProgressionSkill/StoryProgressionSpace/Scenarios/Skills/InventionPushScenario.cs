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
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
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
    public class InventionPushScenario : SimSingleProcessScenario, IHasSkill
    {
        public InventionPushScenario()
        { }
        protected InventionPushScenario(InventionPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Inventing";
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Inventing };
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
            return new SimScoringList(this, "Inventing", Sims.TeensAndAdults, false).GetBestByMinScore(1);
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
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

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            InventingSkill skill = Sim.SkillManager.GetElement(SkillNames.Inventing) as InventingSkill;

            List<InventionWorkbench> benches = new List<InventionWorkbench>();
            List<InventionWorkbench> empties = new List<InventionWorkbench>();
            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                foreach (InventionWorkbench bench in lot.GetObjects<InventionWorkbench>())
                {
                    if (bench.InUse) continue;

                    if (bench.mIsMakingFrankensim) continue;

                    if (bench.mInventionProgress > 0)
                    {
                        if (skill == null) continue;

                        if (!skill.KnownInventions.Contains(bench.mInventionKey)) continue;

                        benches.Add(bench);
                    }
                    else
                    {
                        empties.Add(bench);
                    }
                }
            }

            if (benches.Count == 0)
            {
                benches.AddRange(empties);

                if (benches.Count == 0)
                {
                    IncStat("No Bench");
                    return false;
                }
            }

            int minimumScrap = InventionWorkbench.kNumScrapsPerDiceRoll * 10;
            int inventoryScrap = Inventories.InventoryFindAll<Scrap>(Sim).Count;
            if (inventoryScrap < minimumScrap)
            {
                List<JunkPile> piles = new List<JunkPile>();
                foreach (JunkPile pile in Sims3.Gameplay.Queries.GetObjects<JunkPile>())
                {
                    if (pile.IsEmpty) continue;

                    if (pile.LotCurrent == Sim.LotHome)
                    {
                        piles.Add(pile);
                    }
                    else if (pile.LotCurrent == null)
                    {
                        continue;
                    }
                    else if (pile.LotCurrent.IsCommunityLot)
                    {
                        piles.Add(pile);
                    }
                }

                bool success = false;

                while (piles.Count > 0)
                {
                    JunkPile junkChoice = RandomUtil.GetRandomObjectFromList(piles);
                    piles.Remove(junkChoice);

                    if (!Situations.PushInteraction(this, Sim, junkChoice, DigThroughEx.Singleton))
                    {
                        break;
                    }

                    IncStat("Push Dig Through");

                    success = true;
                }

                if (!success)
                {
                    Money.AdjustFunds(Sim, "BuyItem", -InventionWorkbench.Restock.GetScrapCost(minimumScrap - inventoryScrap));

                    AddStat("Buy Scrap", minimumScrap - inventoryScrap);

                    CreateScrap(Sim.CreatedSim, minimumScrap - inventoryScrap);
                }
                else
                {
                    return true;
                }
            }

            InventionWorkbench choice = RandomUtil.GetRandomObjectFromList(benches);

            if (choice.mInventionProgress > 0)
            {
                IncStat("Try Continue");

                if (Situations.PushInteraction(this, Sim, choice, MakeInventionEx.Singleton)) return true;

                IncStat("Continue Fail");
                return false;
            }
            else
            {
                if ((skill != null) && (skill.GetUndiscoveredInventions().Count == 0x0) && (skill.ReachedMaxLevel()))
                {
                    IncStat("Try Start New");

                    if (Situations.PushInteraction(this, Sim, choice, MakeInventionEx.Singleton)) return true;

                    IncStat("Start New Fail");
                    return false;
                }
                else
                {
                    if ((skill == null) || (skill.KnownInventions.Count == 0) || (RandomUtil.RandomChance(25)))
                    {
                        IncStat("Try Invent");

                        if (Situations.PushInteraction(this, Sim, choice, new InventionWorkbench.Invent.Definition())) return true;

                        IncStat("Invent Fail");
                        return false;
                    }
                    else
                    {
                        IncStat("Try Start New");

                        if (Situations.PushInteraction(this, Sim, choice, MakeInventionEx.Singleton)) return true;

                        IncStat("Start New Fail");
                        return false;
                    }
                }
            }
        }

        public static bool CreateScrap(Sim sim, int amount)
        {
            ScrapInitParameters initData = new ScrapInitParameters(amount);
            IGameObject obj2 = GlobalFunctions.CreateObjectOutOfWorld("scrapPile", ProductVersion.EP2, null, initData);

            Scrap scrap = obj2 as Scrap;
            if (scrap != null)
            {
                scrap.HasBeenCounted = true;
                if (Inventories.TryToMove(scrap, sim))
                {
                    return true;
                }
                scrap.Destroy();
            }
            else if (obj2 != null)
            {
                obj2.Destroy();
            }
            return false;
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new InventionPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, InventionPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "InventingPush";
            }
        }
    }
}
