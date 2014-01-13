using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.ConsignerSpace.Interactions;
using NRaas.ConsignerSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.ConsignerSpace.Helpers
{
    public class PetSale
    {
        public static void DisplayDialog(SimDescription actor, Household target, bool allowHorse)
        {
            List<SimDescription> pets = new List<SimDescription>();

            foreach (SimDescription pet in Households.Pets(actor.Household))
            {
                if (target != null)
                {
                    int price = GetPrice(pet);

                    if (price == 0) continue;

                    if (target.FamilyFunds < price) continue;
                }

                pets.Add(pet);
            }

            while (true)
            {
                string title = Common.Localize("PetSelection:Title");
                int maxPrice = int.MaxValue;

                if (target != null)
                {
                    title = target.Name + Common.NewLine + Common.Localize("SellPet:Funds", false, new object[] { target.FamilyFunds });
                    maxPrice = target.FamilyFunds;
                }

                PetSelection.Results results = new PetSelection(actor, title, maxPrice, pets).SelectMultiple((target != null) ? 1 : 0);
                if ((results == null) || (results.Count == 0)) return;

                if (actor.CreatedSim != null)
                {
                    PlumbBob.SelectActor(actor.CreatedSim);
                }

                foreach (SimDescription sim in results)
                {
                    if ((target == null) && (sim.IsHorse) && (!allowHorse))
                    {
                        Common.Notify(sim.CreatedSim, Common.Localize("SellPet:WrongSpecies", sim.IsFemale, new object[] { sim }));
                    }
                    else if (Relationships.GetParents(sim).Count < 0)
                    {
                        Common.Notify(sim.CreatedSim, Common.Localize("SellPet:NoPedigree", sim.IsFemale, new object[] { sim }));
                    }
                    else
                    {
                        int price = GetPrice(sim);
                        if (price > maxPrice)
                        {
                            price = maxPrice;
                        }

                        if (price == 0)
                        {
                            Common.Notify(sim.CreatedSim, Common.Localize("SellPet:NoPrice", sim.IsFemale, new object[] { sim }));
                        }
                        else
                        {
                            actor.ModifyFunds(price);

                            Common.Notify(sim.CreatedSim, Common.Localize("SellPet:Success", sim.IsFemale, new object[] { sim, price }));

                            actor.Household.Remove(sim);

                            pets.Remove(sim);

                            Household choice = target;
                            if (choice == null)
                            {
                                List<Household> choices = new List<Household>();

                                foreach (Household house in Household.GetHouseholdsLivingInWorld())
                                {
                                    if (house == actor.Household) continue;

                                    if (!house.CanAddSpeciesToHousehold(sim.Species)) continue;

                                    if (house.FamilyFunds < price) continue;

                                    choices.Add(house);
                                }

                                if (choices.Count > 0)
                                {
                                    choice = RandomUtil.GetRandomObjectFromList(choices);
                                }
                            }

                            if (choice != null)
                            {
                                choice.ModifyFamilyFunds(-price);

                                choice.Add(sim);

                                sim.LastName = SimTypes.HeadOfFamily(choice).LastName;

                                Instantiation.AttemptToPutInSafeLocation(sim.CreatedSim, false);
                            }
                            else
                            {
                                sim.Dispose(true);
                            }
                        }
                    }
                }

                if (target == null) break;
            }
        }

        public static int GetPrice(SimDescription sim)
        {
            if (sim.IsHuman) return 0;

            List<SimDescription> parents = Relationships.GetParents(sim);
            if (parents.Count < 2)
            {
                return 0;
            }

            Common.StringBuilder msg = new Common.StringBuilder(sim.FullName);

            int price = 0;

            try
            {
                if ((sim.IsHorse) && (sim.AdultOrAbove))
                {
                    price = HorseManager.GetHorseCost(sim, false);
                }
                else
                {
                    List<Pair<SimDescription, int>> parentLevel = new List<Pair<SimDescription, int>>();

                    foreach (SimDescription parent in parents)
                    {
                        parentLevel.Add(new Pair<SimDescription, int>(parent, 0));
                    }

                    Dictionary<SimDescription, bool> lookup = new Dictionary<SimDescription, bool>();

                    int maxLevel = 0;

                    int index = 0;
                    while (index < parentLevel.Count)
                    {
                        Pair<SimDescription, int> level = parentLevel[index];
                        index++;

                        if (lookup.ContainsKey(level.First))
                        {
                            price -= Consigner.Settings.mInbredPenalty;

                            msg += Common.NewLine + "Inbred: -" + Consigner.Settings.mInbredPenalty;

                            continue;
                        }
                        lookup.Add(level.First, true);

                        if (maxLevel < level.Second)
                        {
                            maxLevel = level.Second;
                        }

                        foreach (SimDescription parent in Relationships.GetParents(level.First))
                        {
                            parentLevel.Add(new Pair<SimDescription, int>(parent, level.Second + 1));
                        }
                    }

                    price += maxLevel * Consigner.Settings.mPedigreeBonus;

                    msg += Common.NewLine + "Pedigree: " + maxLevel + " * " + Consigner.Settings.mPedigreeBonus;

                    if (sim.Child)
                    {
                        price += Consigner.Settings.mSellPrice[0];

                        msg += Common.NewLine + "Child: " + Consigner.Settings.mSellPrice[0];
                    }
                    else if (sim.Adult)
                    {
                        price += Consigner.Settings.mSellPrice[1];

                        msg += Common.NewLine + "Adult: " + Consigner.Settings.mSellPrice[1];
                    }
                    else
                    {
                        price += Consigner.Settings.mSellPrice[2];

                        msg += Common.NewLine + "Elder: " + Consigner.Settings.mSellPrice[2];
                    }

                    foreach (Trait trait in sim.TraitManager.List)
                    {
                        if (trait.Score > 0x0)
                        {
                            price += Consigner.Settings.mGoodTraitBonus;

                            msg += Common.NewLine + trait.TraitName(sim.IsFemale) + ": " + Consigner.Settings.mGoodTraitBonus;
                        }
                        else if (trait.Score < 0x0)
                        {
                            price -= Consigner.Settings.mBadTraitPenalty;

                            msg += Common.NewLine + trait.TraitName(sim.IsFemale) + ": -" + Consigner.Settings.mBadTraitPenalty;
                        }
                    }

                    if (sim.IsGhost)
                    {
                        price += Consigner.Settings.mOccultBonus;

                        msg += Common.NewLine + "Occult: " + Consigner.Settings.mOccultBonus;
                    }

                    {
                        CatHuntingSkill skill = sim.SkillManager.GetSkill<CatHuntingSkill>(SkillNames.CatHunting);
                        if ((skill != null) && (skill.IsVisibleInUI()))
                        {
                            price += (int)(skill.SkillLevel * Consigner.Settings.mSkillLevelBonus);

                            msg += Common.NewLine + "Skill: " + skill.SkillLevel + " * " + Consigner.Settings.mSkillLevelBonus;

                            if (skill.mPreyCaughtTypeStats != null)
                            {
                                int count = 0;

                                foreach (int value in skill.mPreyCaughtTypeStats.Values)
                                {
                                    count += value;
                                }

                                price += (int)(count * Consigner.Settings.mPerPreyBonus);

                                msg += Common.NewLine + "Per Prey: " + count + " * " + Consigner.Settings.mPerPreyBonus;
                            }
                        }
                    }

                    {
                        DogHuntingSkill skill = sim.SkillManager.GetSkill<DogHuntingSkill>(SkillNames.DogHunting);
                        if ((skill != null) && (skill.IsVisibleInUI()))
                        {
                            price += (int)(skill.SkillLevel * Consigner.Settings.mSkillLevelBonus);

                            msg += Common.NewLine + "Skill: " + skill.SkillLevel + " * " + Consigner.Settings.mSkillLevelBonus;

                            if (skill.mNumRGMFound != null)
                            {
                                int count = 0;

                                foreach (int value in skill.mNumRGMFound.Values)
                                {
                                    count += value;
                                }

                                price += (int)(count * Consigner.Settings.mPerPreyBonus);

                                msg += Common.NewLine + "RGM: " + count + " * " + Consigner.Settings.mPerPreyBonus;
                            }
                        }
                    }
                }

                msg += Common.NewLine + "Final Price: " + price;

                return Math.Max(price, 0);
            }
            finally
            {
                Common.DebugNotify(msg);
            }
        }
    }
}
