using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CAS.Locale;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Tasks
{
    public class FixInvisibleTask : Common.FunctionTask
    {
        SimDescription mSim = null;

        public enum Approach
        {
            None,
            Rerolled,
            Recovered,
            Reinherited
        }

        public FixInvisibleTask(SimDescription sim)
        {
            mSim = sim;
        }

        protected override void OnPerform()
        {
            Approach approach = Perform(mSim, false);
            if (approach == Approach.None) return;

            Logger.Append("Outfits " + approach + ": " + mSim.FullName);

            /*
            if (mSim.CreatedSim != null)
            {
                mSim.CreatedSim.FadeOut(false, true);
            }
            */
        }

        public static ResourceKey ValidateSkinTone(ResourceKey value)
        {
            List<ResourceKey> choiceTones = new List<ResourceKey>();

            KeySearch tones = new KeySearch(0x0354796a);
            foreach (ResourceKey tone in tones)
            {
                choiceTones.Add(tone);
            }

            tones.Reset();

            if ((value.InstanceId == 0) || (!choiceTones.Contains(value)))
            {
                value = RandomUtil.GetRandomObjectFromList(choiceTones);
            }

            return value;
        }

        public static Sim InstantiateAtHome(SimDescription simDesc, Instantiation.OnReset onReset)
        {
            Sim sim = Instantiation.Perform(simDesc, onReset);
            if (sim == null)
            {
                Perform(simDesc, false);

                sim = Instantiation.Perform(simDesc, onReset);
            }

            return sim;
        }
        public static Sim InstantiateOffLot(SimDescription simDesc, Lot lot, Instantiation.OnReset onReset)
        {
            Sim sim = Instantiation.PerformOffLot(simDesc, lot, onReset);
            if (sim == null)
            {
                Perform(simDesc, false);

                sim = Instantiation.PerformOffLot(simDesc, lot, onReset);
            }

            return sim;
        }

        public static Approach Perform(SimDescription sim, bool force)
        {
            return Perform(sim, force, true);
        }
        public static Approach Perform(SimDescription sim, bool force, bool reset)
        {
            try
            {
                OutfitCategories[] categoriesArray = null;

                switch (sim.Species)
                {
                    case CASAgeGenderFlags.Human:
                        categoriesArray = new OutfitCategories[] { OutfitCategories.Everyday, OutfitCategories.Naked, OutfitCategories.Athletic, OutfitCategories.Formalwear, OutfitCategories.Sleepwear, OutfitCategories.Swimwear };
                        break;
                    case CASAgeGenderFlags.Horse:
                        categoriesArray = new OutfitCategories[] { OutfitCategories.Everyday, OutfitCategories.Naked, OutfitCategories.Racing, OutfitCategories.Bridle, OutfitCategories.Jumping };
                        break;
                    default:
                        categoriesArray = new OutfitCategories[] { OutfitCategories.Everyday, OutfitCategories.Naked };
                        break;
                }

                bool necessary = force;

                if (!necessary)
                {
                    foreach (OutfitCategories category in categoriesArray)
                    {
                        if (sim.IsHuman)
                        {
                            if (category == OutfitCategories.Naked) continue;
                        }

                        SimOutfit outfit2 = sim.GetOutfit(category, 0);
                        if ((outfit2 == null) || (!outfit2.IsValid))
                        {
                            necessary = true;
                        }
                    }
                }
                
                if (!necessary)
                {
                    return Approach.None;
                }

                SimOutfit sourceOutfit = null;

                for(int i=0; i<2; i++)
                {
                    OutfitCategoryMap map = null;
                    if (i == 0)
                    {
                        map = sim.mOutfits;
                    }
                    else
                    {
                        map = sim.mMaternityOutfits;
                    }

                    if (map == null) continue;

                    foreach(OutfitCategories category in Enum.GetValues(typeof(OutfitCategories)))
                    {
                        if (category == OutfitCategories.Supernatural) continue;

                        ArrayList outfits = map[category] as ArrayList;
                        if (outfits == null) continue;

                        foreach(SimOutfit anyOutfit in outfits)
                        {
                            if ((anyOutfit != null) && (anyOutfit.IsValid))
                            {
                                sourceOutfit = anyOutfit;
                                break;
                            }
                        }
                    }
                }

                SimBuilder builder = new SimBuilder();
                builder.UseCompression = true;

                ResourceKey newTone = ValidateSkinTone(sim.SkinToneKey);

                builder.Age = sim.Age;
                builder.Gender = sim.Gender;
                builder.Species = sim.Species;
                builder.SkinTone = newTone;
                builder.SkinToneIndex = sim.SkinToneIndex;
                builder.MorphFat = sim.mCurrentShape.Fat;
                builder.MorphFit = sim.mCurrentShape.Fit;
                builder.MorphThin = sim.mCurrentShape.Thin;

                Approach approach = Approach.Rerolled;

                GeneticsPet.SpeciesSpecificData speciesData = OutfitUtils.GetSpeciesSpecificData(sim);

                try
                {
                    if (sourceOutfit != null)
                    {
                        foreach (SimOutfit.BlendInfo blend in sourceOutfit.Blends)
                        {
                            builder.SetFacialBlend(blend.key, blend.amount);
                        }

                        CASParts.OutfitBuilder.CopyGeneticParts(builder, sourceOutfit);

                        approach = Approach.Recovered;
                    }
                    else
                    {
                        if (sim.Genealogy != null)
                        {
                            List<SimDescription> parents = new List<SimDescription>();
                            List<SimDescription> grandParents = new List<SimDescription>();

                            foreach (SimDescription parent in Relationships.GetParents(sim))
                            {
                                parents.Add(parent);

                                foreach(SimDescription grandParent in Relationships.GetParents(parent))
                                {
                                    grandParents.Add(grandParent);
                                }
                            }

                            if (parents.Count > 0)
                            {
                                if (sim.IsHuman)
                                {
                                    Genetics.InheritFacialBlends(builder, parents.ToArray(), new Random());
                                }
                                else
                                {
                                    GeneticsPet.InheritBodyShape(builder, parents, grandParents, new Random());
                                    GeneticsPet.InheritBasePeltLayer(builder, parents, grandParents, new Random());
                                    GeneticsPet.InheritPeltLayers(builder, parents, grandParents, new Random());
                                }

                                approach = Approach.Reinherited;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(sim, null, "Primary Outfit Creation", e);
                    return Approach.None;
                }

                if (sim.IsRobot)
                {
                    OutfitUtils.AddMissingPartsBots(builder, (OutfitCategories)0x200002, true, sim);

                    Common.Sleep();

                    OutfitUtils.AddMissingPartsBots(builder, OutfitCategories.Everyday, true, sim);

                    Common.Sleep();
                }
                else if (sim.IsHuman)
                {
                    OutfitUtils.AddMissingParts(builder, (OutfitCategories)0x200002, true, sim, sim.IsAlien);

                    Common.Sleep();

                    OutfitUtils.AddMissingParts(builder, OutfitCategories.Everyday, true, sim, sim.IsAlien);

                    Common.Sleep();
                }
                else
                {
                    OutfitUtils.AddMissingPartsPet(builder, OutfitCategories.Everyday | (OutfitCategories)0x200000, true, sim, speciesData);

                    Common.Sleep();

                    OutfitUtils.AddMissingPartsPet(builder, OutfitCategories.Everyday, true, sim, speciesData);

                    Common.Sleep();
                }

                ResourceKey uniformKey = new ResourceKey();
                if (sim.IsHuman)
                {
                    if (LocaleConstraints.GetUniform(ref uniformKey, sim.HomeWorld, builder.Age, builder.Gender, OutfitCategories.Everyday))
                    {
                        OutfitUtils.SetOutfit(builder, new SimOutfit(uniformKey), sim);
                    }
                }

                OutfitUtils.SetAutomaticModifiers(builder);

                sim.ClearOutfits(OutfitCategories.Career, false);
                sim.ClearOutfits(OutfitCategories.MartialArts, false);
                sim.ClearOutfits(OutfitCategories.Special, false);

                foreach (OutfitCategories category in categoriesArray)
                {
                    ArrayList outfits = null;

                    if (!force)
                    {
                        outfits = sim.Outfits[category] as ArrayList;
                        if (outfits != null)
                        {
                            int index = 0;
                            while (index < outfits.Count)
                            {
                                SimOutfit anyOutfit = outfits[index] as SimOutfit;
                                if (anyOutfit == null)
                                {
                                    outfits.RemoveAt(index);
                                }
                                else if (!anyOutfit.IsValid)
                                {
                                    outfits.RemoveAt(index);
                                }
                                else
                                {
                                    index++;
                                }
                            }
                        }
                    }

                    if ((outfits == null) || (outfits.Count == 0))
                    {
                        OutfitUtils.MakeCategoryAppropriate(builder, category, sim);

                        if (sim.IsHuman)
                        {
                            if (LocaleConstraints.GetUniform(ref uniformKey, sim.HomeWorld, builder.Age, builder.Gender, category))
                            {
                                OutfitUtils.SetOutfit(builder, new SimOutfit(uniformKey), sim);
                            }
                        }

                        sim.RemoveOutfits(category, false);

                        CASParts.AddOutfit(sim, category, builder, true);
                    }

                    if (sim.IsUsingMaternityOutfits)
                    {
                        sim.BuildPregnantOutfit(category);
                    }
                }

                if (sim.IsMummy)
                {
                    OccultMummy.OnMerge(sim);
                }
                else if (sim.IsFrankenstein)
                {
                    OccultFrankenstein.OnMerge(sim, sim.OccultManager.mIsLifetimeReward);
                }
                else if (sim.IsGenie)
                {
                    OccultGenie.OverlayUniform(sim, OccultGenie.CreateUniformName(sim.Age, sim.Gender), ProductVersion.EP6, OutfitCategories.Everyday, CASSkinTones.BlueSkinTone, 0.68f);
                }
                else if (sim.IsImaginaryFriend)
                {
                    OccultImaginaryFriend friend = sim.OccultManager.GetOccultType(Sims3.UI.Hud.OccultTypes.ImaginaryFriend) as OccultImaginaryFriend;

                    OccultBaseClass.OverlayUniform(sim, OccultImaginaryFriend.CreateUniformName(sim.Age, friend.Pattern), ProductVersion.EP4, OutfitCategories.Special, CASSkinTones.NoSkinTone, 0f);
                }

                if (sim.IsMermaid)
                {
                    OccultMermaid.AddOutfits(sim, null);
                }


                if (sim.IsWerewolf)
                {
                    if (sim.ChildOrAbove)
                    {
                        SimOutfit newWerewolfOutfit = OccultWerewolf.GetNewWerewolfOutfit(sim.Age, sim.Gender);
                        if (newWerewolfOutfit != null)
                        {
                            sim.AddOutfit(newWerewolfOutfit, OutfitCategories.Supernatural, 0x0);
                        }
                    }
                }

                SimOutfit currentOutfit = null;
                if (sim.CreatedSim != null)
                {
                    if (reset)
                    {
                        ResetSimTask.Perform(sim.CreatedSim, false);
                    }

                    try
                    {
                        sim.CreatedSim.SwitchToOutfitWithoutSpin(Sim.ClothesChangeReason.GoingOutside, OutfitCategories.Everyday, true);
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(sim, e);
                    }

                    currentOutfit = sim.CreatedSim.CurrentOutfit;
                }
                else
                {
                    currentOutfit = sim.GetOutfit(OutfitCategories.Everyday, 0);
                }

                if (currentOutfit != null)
                {
                    ThumbnailManager.GenerateHouseholdSimThumbnail(currentOutfit.Key, currentOutfit.Key.InstanceId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, sim.AgeGenderSpecies);
                }

                return approach;
            }
            catch(Exception e)
            {
                Common.Exception(sim, e);
                return Approach.None;
            }
        }

        public class Logger : Common.Logger<Logger>
        {
            readonly static Logger sLogger = new Logger();

            public static void Append(string msg)
            {
                sLogger.PrivateAppend(msg);
            }

            protected override string Name
            {
                get { return "Fix Outfit Logs"; }
            }

            protected override Logger Value
            {
                get { return sLogger; }
            }
        }
    }
}