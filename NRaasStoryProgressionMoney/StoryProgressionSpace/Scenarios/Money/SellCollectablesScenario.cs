using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.MinorPets;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Money
{
    public class SellCollectablesScenario : SellObjectsScenario
    {
        public SellCollectablesScenario()
            : base ()
        { }
        protected SellCollectablesScenario(SellCollectablesScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Pure)
            {
                return "SellCollectables";
            }
            else
            {
                return "SellStuff";
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override List<GameObject> GetInventory(SimDescription sim)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject obj in Inventories.InventoryFindAll<GameObject>(sim))
            {
                if ((obj is RockGemMetalBase) || (obj is IHazInsect) || (obj is MinorPet))
                {
                    list.Add(obj);
                }
            }

            return list;
        }

        protected bool CutGem(Gem gem)
        {
            if (!gem.mCollected) return false;

            Collecting skill = Sim.SkillManager.AddElement(SkillNames.Collecting) as Collecting;

            List<Gem.CutData> choices = new List<Gem.CutData>();

            int num = 0x0;
            if (skill != null)
            {
                num = skill.GemsCut();

                if (skill.IsGemCollector())
                {
                    choices.Add(Gem.sToughestCut);
                }
            }

            foreach (Gem.CutData data in Gem.sCutData)
            {
                if ((data.NumberOfCuts <= num) && gem.ValidCut(data, Sim.CreatedSim))
                {
                    choices.Add(data);
                }
            }

            List<Gem.CutData> valid = new List<Gem.CutData>();

            foreach (Gem.CutData data in choices)
            {
                if (Sim.FamilyFunds >= data.CostToCut)
                {
                    valid.Add(data);
                }
            }

            if (valid.Count == 0)
            {
                IncStat("No Gem Cut Choices");
                return false;
            }

            Gem.CutData choice = RandomUtil.GetRandomObjectFromList(valid);

            Money.AdjustFunds(Sim, "GemCut", -choice.CostToCut);

            GemEx.CutGem(gem, choice, Sim);

            IncStat("Gem Cut");

            if (skill != null)
            {
                skill.GemCut();
            };

            return true;
        }

        protected bool SmeltMetal(Metal metal)
        {
            if (!metal.mCollected) return false;

            if (Sim.FamilyFunds < Metal.GetSmelt.kCostToSmelt)
            {
                IncStat("Smelt Cost");
                return false;
            }

            Money.AdjustFunds(Sim, "Smelting", -Metal.GetSmelt.kCostToSmelt);

            float chanceOfExtraIngot = Metal.GetSmelt.kChanceOfExtraIngot;

            Collecting skill = Sim.SkillManager.AddElement(SkillNames.Collecting) as Collecting;
            if (skill.IsMetalCollector())
            {
                chanceOfExtraIngot = Metal.GetSmelt.kMetalCollectorExtraIngotChance;
            }

            int num2 = 0x1;
            while ((num2 < Metal.GetSmelt.kMaxNumberOfIngots) && RandomUtil.RandomChance01(chanceOfExtraIngot))
            {
                num2++;
                Metal metal2 = RockGemMetalBase.Make(metal.Guid, true) as Metal;
                MetalEx.SmeltMetal(metal2, Sim);
                IncreaseIngotsCreatedForMetal(skill, metal2.Guid);

                Inventories.TryToMove(metal2, Sim.CreatedSim);
            }

            MetalEx.SmeltMetal(metal, Sim);
            IncreaseIngotsCreatedForMetal(skill, metal.Guid);

            skill.MetalSmelt();

            IncStat("Metal Smelt");

            return true;
        }

        protected static void IncreaseIngotsCreatedForMetal(Collecting skill, RockGemMetal metal)
        {
            if (!skill.mMetalData.ContainsKey(metal))
            {
                skill.mMetalData.Add(metal, new Collecting.MetalStats(0));
            }

            skill.IncreaseIngotsCreatedForMetal(metal);
        }

        protected bool PlacePet(MinorPet pet)
        {
            foreach (MinorPetTerrarium terrarium in Sim.LotHome.GetObjects<MinorPetTerrarium>())
            {
                if (terrarium.Pet != null) continue;

                if (!terrarium.AllowsPetType(pet.Data.MinorPetType)) continue;

                try
                {
                    Sim.CreatedSim.Inventory.RemoveByForce(pet);
                    pet.ParentToSlot(terrarium, terrarium.PetContainmentSlot);
                    pet.SetObjectToReset();
                    Simulator.Wake(pet.ObjectId, false);

                    IncStat("Pet Placed");
                    return true;
                }
                catch (Exception e)
                {
                    Common.Exception(pet, e);
                }
            }

            return false;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool success = false;
            foreach (GameObject obj in GetInventory(Sim))
            {
                Gem gem = obj as Gem;
                if ((gem != null) && (string.IsNullOrEmpty(gem.mCutName)))
                {
                    if (CutGem(gem))
                    {
                        continue;
                    }
                }
                else
                {
                    Metal metal = obj as Metal;
                    if ((metal != null) && (!metal.mHasBeenSmelt))
                    {
                        if (SmeltMetal(metal))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Rock rock = obj as Rock;
                        if ((rock != null) && (!rock.mAnalyzed))
                        {
                            if (rock.AnalyzeMe(Sim.CreatedSim))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            MinorPet pet = obj as MinorPet;
                            if (pet != null)
                            {
                                if (PlacePet(pet))
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }

                SimDescription sim = Sim;
                if (!sim.IsHuman)
                {
                    sim = SimTypes.HeadOfFamily(Sim.Household);
                }

                int value = Money.Sell(sim, obj);

                mFunds += value;

                AddStat("Sold", value);
                success = true;
            }

            return success;
        }

        public override Scenario Clone()
        {
            return new SellCollectablesScenario(this);
        }

        public class Option : BooleanScenarioOptionItem<ManagerMoney, SellCollectablesScenario>, ManagerMoney.ISalesOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SellCollectables";
            }
        }
    }
}
