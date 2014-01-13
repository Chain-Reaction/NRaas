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
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.FoodObjects;
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
    public class NectarPushScenario : SimSingleProcessScenario, IHasSkill
    {
        public NectarPushScenario()
        { }
        protected NectarPushScenario(NectarPushScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Nectar";
        }

        public SkillNames[] CheckSkills
        {
            get
            {
                return new SkillNames[] { SkillNames.Nectar };
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Lots.GetSimsWith<NectarMaker>();
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Households.AllowGuardian(sim))
            {
                IncStat("Too Young");
                return false;
            }
            else if (sim.CreatedSim == null)
            {
                IncStat("Hibernating");
                return false;
            }
            else if (sim.LotHome == null)
            {
                IncStat("Homeless");
                return false;
            }
            else if (!Situations.Allow(this, sim))
            {
                IncStat("Situations Denied");
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
            else if (AddScoring("Nectar", sim) < 0)
            {
                IncStat("Score Fail");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            List<NectarMaker> choices = new List<NectarMaker>();
            NectarMaker best = null, broken = null;

            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                foreach (NectarMaker maker in lot.GetObjects<NectarMaker>())
                {
                    if (!maker.Repairable.Broken)
                    {
                        choices.Add(maker);
                        if (maker.Inventory.IsFull())
                        {
                            best = maker;
                        }
                    }
                    else
                    {
                        broken = maker;
                    }

                    foreach (NectarBottle bottle in maker.mBottles)
                    {
                        Inventories.TryToMove(bottle, Sim.CreatedSim);
                    }

                    AddStat("Collected Bottles", maker.mBottles.Count);

                    maker.mBottles.Clear();
                }
            }

            List<NectarRack> racks = new List<NectarRack>();

            foreach (Lot lot in ManagerLot.GetOwnedLots(Sim))
            {
                foreach (NectarRack rack in lot.GetObjects<NectarRack>())
                {
                    if (rack.HasTreasure()) continue;

                    if (rack.Buyable) continue;

                    if (rack.Tasteable) continue;

                    if (rack.Inventory.IsFull()) continue;

                    racks.Add(rack);
                }
            }

            if (racks.Count > 0)
            {
                List<NectarBottle> bottles = new List<NectarBottle>(Inventories.InventoryFindAll<NectarBottle>(Sim));
                foreach (NectarBottle bottle in bottles)
                {
                    if (!bottle.CanBeSold()) continue;

                    NectarRack rack = RandomUtil.GetRandomObjectFromList(racks);

                    IncStat("Rack Stored");
                    rack.Inventory.TryToMove(bottle);

                    if (rack.Inventory.IsFull())
                    {
                        racks.Remove(rack);
                        if (racks.Count == 0) break;
                    }
                }
            }

            if (best != null)
            {
                choices.Clear();
                choices.Add(best);
            }

            if (choices.Count == 0)
            {
                if (broken != null)
                {
                    IncStat("Attempt Repair");

                    Add(frame, new ScheduledRepairScenario(Sim, broken), ScenarioResult.Start);
                }
                else
                {
                    IncStat("No Choice");
                }
                return false;
            }

            NectarMaker choice = RandomUtil.GetRandomObjectFromList(choices);
            if (!choice.Inventory.IsFull())
            {
                List<Ingredient> list = new List<Ingredient>(Inventories.InventoryFindAll<Ingredient>(Sim));

                while (list.Count > 0)
                {
                    Ingredient ingredient = RandomUtil.GetRandomObjectFromList(list);
                    list.Remove(ingredient);

                    if (!choice.CanAddToInventory(ingredient)) continue;

                    if (choice.Inventory.TryToMove(ingredient))
                    {
                        IncStat("Added: " + ingredient.CatalogName);
                    }

                    if (choice.Inventory.IsFull())
                    {
                        break;
                    }
                }
            }

            if (choice.Inventory.IsFull())
            {
                if ((ManagerCareer.HasSkillCareer(Sim, SkillNames.Nectar)) ||
                    (!ManagerCareer.HasSkillCareer(Sim.Household, SkillNames.Nectar)))
                {
                    int skillLevel = Sim.SkillManager.GetSkillLevel(SkillNames.Nectar);

                    List<NectarMaker.MakeNectarStyle> styles = new List<NectarMaker.MakeNectarStyle>();

                    styles.Add(NectarMaker.MakeNectarStyle.Basic);

                    if (skillLevel >= NectarMaker.kConcentratedUnlockLevel)
                    {
                        styles.Add(NectarMaker.MakeNectarStyle.Concentrated);
                    }
                    if (skillLevel >= NectarMaker.kMassProduceUnlockLevel)
                    {
                        styles.Add(NectarMaker.MakeNectarStyle.MassProduce);
                    }
                    if (skillLevel >= NectarMaker.kExtendedNectarationUnlockLevel)
                    {
                        styles.Add(NectarMaker.MakeNectarStyle.ExtendedNectaration);
                    }

                    return Situations.PushInteraction(this, Sim, choice, new MakeNectarEx.Definition(RandomUtil.GetRandomObjectFromList(styles)));
                }
                else
                {
                    IncStat("Not Job");
                    return false;
                }
            }
            else
            {
                IncStat("Not Full");
                return false;
            }
        }

        protected override bool Push()
        {
            return true;
        }

        public override Scenario Clone()
        {
            return new NectarPushScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerSkill, NectarPushScenario>
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "NectarPush";
            }

            public override bool Value
            {
                get
                {
                    if (!ShouldDisplay()) return false;

                    return base.Value;
                }
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP1);
            }
        }

        public class NameGreatNectarOption : BooleanManagerOptionItem<ManagerSkill>
        {
            public NameGreatNectarOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "NameGreatNectar";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP1);
            }
        }
    }
}
