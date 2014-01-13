using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class InstaBaby : DualSimFromList, IIntermediateOption
    {
        protected int mNumChildren = 1;

        protected CASAgeGenderFlags mGender = CASAgeGenderFlags.None;

        public override string GetTitlePrefix()
        {
            return "InstaBaby";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("Pollinate:Mother");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("Pollinate:Father");
        }

        protected override int GetMaxSelectionA()
        {
            return 0;
        }

        protected override IEnumerable<CASAgeGenderFlags> GetSpeciesFilterB()
        {
            return null;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (IsFirst)
            {
                if (me.LotHome == null) return false;
            }

            return base.PrivateAllow(me);
        }

        protected List<Sim> GenerateHumanChildren(SimDescription woman, SimDescription man, int numChildren)
        {
            Random pregoRandom = new Random();

            List<Sim> babies = new List<Sim>();

            for (int i = 0; i < numChildren; i++)
            {
                try
                {
                    SimDescription newBaby = Genetics.MakeBaby(woman, man, NRaas.MasterControllerSpace.Helpers.Baby.InterpretGender(mGender), 100, pregoRandom, false);
                    woman.Household.Add(newBaby);

                    string name = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":NamePrompt", newBaby.IsFemale, new object[0]), newBaby.FirstName);
                    if (!string.IsNullOrEmpty(name))
                    {
                        newBaby.FirstName = name;
                    }

                    Sim babySim = Instantiation.Perform(newBaby, null);
                    if (babySim != null)
                    {
                        babies.Add(babySim);

                        SimOutfit currentOutfit = babySim.CurrentOutfit;
                        if (currentOutfit != null)
                        {
                            ThumbnailManager.GenerateHouseholdSimThumbnail(currentOutfit.Key, currentOutfit.Key.InstanceId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, newBaby.AgeGenderSpecies);
                        }

                        Pregnancy.MakeBabyVisible(babySim);

                        if (i == 0)
                        {
                            EventTracker.SendEvent(new SimDescriptionEvent(EventTypeId.kNewBaby, newBaby));
                        }

                        if (woman != null)
                        {
                            MidlifeCrisisManager.OnHadChild(woman);

                            if (woman.CreatedSim != null)
                            {
                                EventTracker.SendEvent(EventTypeId.kNewOffspring, woman.CreatedSim, babySim);
                                EventTracker.SendEvent(EventTypeId.kParentAdded, babySim, woman.CreatedSim);
                            }
                        }

                        if (man != null)
                        {
                            MidlifeCrisisManager.OnHadChild(man);

                            if (man.CreatedSim != null)
                            {
                                EventTracker.SendEvent(EventTypeId.kNewOffspring, man.CreatedSim, babySim);
                                EventTracker.SendEvent(EventTypeId.kParentAdded, babySim, man.CreatedSim);
                            }
                        }

                        EventTracker.SendEvent(EventTypeId.kChildBornOrAdopted, null, babySim);

                        if (newBaby.IsHuman)
                        {
                            new SocialWorkerAdoptionSituation.InstantiateNewKid().GiveImaginaryFriendDoll(newBaby);
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(woman, man, e);
                }
            }

            if (babies.Count > 0)
            {
                if (woman.CreatedSim != null)
                {
                    EventTracker.SendEvent(new PregnancyEvent(EventTypeId.kHadBaby, woman.CreatedSim, man.CreatedSim, null, babies));
                }

                if (man.CreatedSim != null)
                {
                    EventTracker.SendEvent(new PregnancyEvent(EventTypeId.kHadBaby, man.CreatedSim, woman.CreatedSim, null, babies));
                }
            }

            return babies;
        }
        protected List<Sim> GeneratePetChildren(SimDescription woman, SimDescription man, int numChildren)
        {
            Random pregoRandom = new Random();

            float[] chanceOfTwin = new float[] { 1f, PetPregnancy.kChanceEggSizeTwo, PetPregnancy.kChanceEggSizeThree, PetPregnancy.kChanceEggSizeFour };

            int index = 0;

            SimDescription eggLead = null;

            GeneticsPet.SetName nameSet = GeneticsPet.SetName.SetNameNonInteractive;
            if (woman.IsHorse)
            {
                nameSet = GeneticsPet.SetName.SetNameInteractive;
            }

            List<Sim> babies = new List<Sim>();

            for (int i = 0; i < numChildren; i++)
            {
                try
                {
                    OccultTypes occult = RandomUtil.CoinFlip() ? woman.OccultManager.CurrentOccultTypes : man.OccultManager.CurrentOccultTypes;
                    if (!OccultManager.DoesOccultTransferToOffspring(occult))
                    {
                        occult = OccultTypes.None;
                    }

                    SimDescription newBaby = null;
                    if (pregoRandom.NextDouble() > chanceOfTwin[index])
                    {
                        index = 0x0;
                    }

                    if ((index == 0x0) || (woman.IsHorse))
                    {
                        CASAgeGenderFlags species = woman.Species;
                        if ((man != null) && (pregoRandom.NextDouble() > 0.5))
                        {
                            species = man.Species;
                        }

                        newBaby = GeneticsPet.MakePetDescendant(man, woman, CASAgeGenderFlags.Child, NRaas.MasterControllerSpace.Helpers.Baby.InterpretGender(mGender), species, pregoRandom, true, nameSet, i, occult);
                        if (newBaby != null)
                        {
                            if (RandomUtil.CoinFlip())
                            {
                                newBaby.SetDeathStyle(woman.DeathStyle, true);
                            }
                            else
                            {
                                newBaby.SetDeathStyle(man.DeathStyle, true);
                            }
                        }
                    }
                    else
                    {
                        newBaby = GeneticsPet.MakeSameEggDescendant(eggLead, man, woman, NRaas.MasterControllerSpace.Helpers.Baby.InterpretGender(mGender), pregoRandom, true, nameSet, i);
                        if (newBaby != null)
                        {
                            newBaby.SetDeathStyle(eggLead.DeathStyle, true);
                        }
                    }

                    eggLead = newBaby;

                    index++;
                    if (index >= chanceOfTwin.Length)
                    {
                        index = chanceOfTwin.Length - 1;
                    }

                    if (newBaby == null) continue;

                    newBaby.WasCasCreated = false;

                    woman.Household.Add(newBaby);

                    Vector3 position = woman.CreatedSim.Position;

                    Sim babyToHide = Instantiation.Perform(newBaby, position, null, null);

                    babies.Add(babyToHide);

                    if (newBaby.DeathStyle != SimDescription.DeathType.None)
                    {
                        Urnstone.SimToPlayableGhost(babyToHide);
                    }

                    if (i == 0x0)
                    {
                        EventTracker.SendEvent(new SimDescriptionEvent(EventTypeId.kNewBaby, newBaby));
                    }

                    if (woman.CreatedSim != null)
                    {
                        EventTracker.SendEvent(EventTypeId.kNewOffspringPet, woman.CreatedSim, babyToHide);
                    }

                    if (man.CreatedSim != null)
                    {
                        EventTracker.SendEvent(EventTypeId.kNewOffspringPet, man.CreatedSim, babyToHide);
                    }

                    foreach (Sim sim in CommonSpace.Helpers.Households.AllHumans(woman.Household))
                    {
                        EventTracker.SendEvent(EventTypeId.kNewPet, sim, babyToHide);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(woman, man, e);
                }
            }

            return babies;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            if (b == a)
            {
                return Run(a, (SimDescription)null);
            }
            else if (a == null)
            {
                if (b == null) return true;

                return Run(b, a);
            }

            Sim womanSim = a.CreatedSim;

            Sim manSim = null;
            if (b != null)
            {
                manSim = b.CreatedSim;
            }

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt"), "1");
                if ((text == null) || (text == "")) return false;

                mNumChildren = 1;
                if (!int.TryParse(text, out mNumChildren))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }

                mGender = NRaas.MasterControllerSpace.Helpers.Baby.SelectGender();
                if (mGender == CASAgeGenderFlags.None) return false;
            }

            int numChildren = mNumChildren;
            if (!NRaas.MasterController.Settings.mAllowOverStuffed)
            {
                int maxChildren = 0;
                if (womanSim.IsHuman)
                {
                    maxChildren = 8 - CommonSpace.Helpers.Households.NumHumansIncludingPregnancy(a.Household);
                }
                else
                {
                    maxChildren = 6 - CommonSpace.Helpers.Households.NumPetsIncludingPregnancy(a.Household);
                }

                if (numChildren > maxChildren)
                {
                    numChildren = maxChildren;
                }
            }

            if (numChildren <= 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("InstaBaby:Failure", a.IsFemale, new object[] { a }));
            }

            if (a.CelebrityManager == null)
            {
                a.Fixup();
            }

            if (b.CelebrityManager == null)
            {
                b.Fixup();
            }

            List<Sim> babies = null;
            if (a.IsHuman)
            {
                babies = GenerateHumanChildren(a, b, numChildren);
            }
            else
            {
                babies = GeneratePetChildren(a, b, numChildren);
            }

            if (!ApplyAll)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("InstaBaby:Success", a.IsFemale, new object[] { a, numChildren }));
            }
            else
            {
                Common.Notify(Common.Localize("InstaBaby:Success", a.IsFemale, new object[] { a, numChildren }));
            }
            return true;
        }
    }
}
