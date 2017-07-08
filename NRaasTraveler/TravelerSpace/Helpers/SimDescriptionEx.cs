using NRaas.CommonSpace.Booters;
using NRaas.TravelerSpace.CareerMergers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Helpers
{
    public class SimDescriptionEx
    { 
        public static void MergeTravelInformation(SimDescription ths, MiniSimDescription msd, Dictionary<ulong,SimDescription> allSims)
        {
            Common.StringBuilder msg = new Common.StringBuilder("MergeTravelInformation " + ths.FullName + Common.NewLine);

            msg += "A";
            Traveler.InsanityWriteLog(msg);

            foreach (MiniRelationship relationship in msd.MiniRelationships)
            {
                SimDescription y = null;
                if (!allSims.TryGetValue(relationship.SimDescriptionId, out y))
                {
                    y = SimDescription.Find(relationship.SimDescriptionId);
                }

                if (y != null)
                {
                    Relationship unsafely = null;

                    try
                    {
                        unsafely = Relationship.GetUnsafely(ths, y);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(ths, e);
                    }

                    if (unsafely != null)
                    {
                        float change = relationship.Value - unsafely.LTR.Liking;
                        if (change != 0f)
                        {
                            unsafely.LTR.UpdateLiking(change);
                        }
                        RomanceVisibilityState.MergeTravelInformationOnTravelBackHome(relationship, unsafely);
                    }
                }
            }

            msg += "B";
            Traveler.InsanityWriteLog(msg);

            ths.CASGenealogy = msd.CASGenealogy;            

            AgingCheckTask.Add(ths, msd);

            msg += "C";
            Traveler.InsanityWriteLog(msg);
        }

        public static void MergeTravelInformation(SimDescription dest, SimDescription source, bool destHomeworld)
        {
            Common.StringBuilder msg = new Common.StringBuilder("MergeTravelInformation " + dest.FullName + Common.NewLine);

            try
            {
                msg += "A";
                Traveler.InsanityWriteLog(msg);

                OccupationNames sourceGuid = OccupationNames.Undefined;
                if (source.Occupation != null)
                {
                    sourceGuid = source.Occupation.Guid;
                }

                OccupationNames destGuid = OccupationNames.Undefined;
                if (dest.Occupation != null)
                {
                    destGuid = dest.Occupation.Guid;
                }

                if ((!destHomeworld) || (sourceGuid != destGuid))
                {
                    msg += "A1";
                    Traveler.InsanityWriteLog(msg);

                    if ((!destHomeworld) || (source.Occupation != null))
                    {
                        dest.CareerManager.mJob = source.Occupation;
                        if (dest.Occupation != null)
                        {
                            dest.Occupation.OwnerDescription = dest;
                        }

                        source.CareerManager.mJob = null;
                    }
                }
                else
                {
                    msg += "A2";
                    Traveler.InsanityWriteLog(msg);

                    // Update career piece-wise
                    foreach (ICareerMerger merger in Common.DerivativeSearch.Find<ICareerMerger>())
                    {
                        msg += Common.NewLine + merger.GetType().ToString();
                        Traveler.InsanityWriteLog(msg);

                        merger.IPerform(dest.Occupation, source.Occupation);
                    }
                }

                msg += Common.NewLine + "B";
                Traveler.InsanityWriteLog(msg);

                // Custom
                //   The MergeOccults in MergeTravelInformation can fail when removing occults
                OccultManagerEx.MergeOccults(dest.OccultManager, source.OccultManager);

                msg += "C";
                Traveler.InsanityWriteLog(msg);

                // Original Function
                dest.MergeTravelInformation(source);

                msg += "D";
                Traveler.InsanityWriteLog(msg);

                TrickSkill sourceTrickSkill = source.SkillManager.GetSkill<TrickSkill>(SkillNames.Trick);
                if (sourceTrickSkill != null)
                {
                    TrickSkill destTrickSkill = dest.SkillManager.AddElement(SkillNames.Trick) as TrickSkill;
                    if (destTrickSkill != null)
                    {
                        destTrickSkill.mExhibitionsPerformed = sourceTrickSkill.mExhibitionsPerformed;
                        destTrickSkill.mMoneyEarnedFromExhibitions = sourceTrickSkill.mMoneyEarnedFromExhibitions;
                        destTrickSkill.mTricks = sourceTrickSkill.mTricks;
                    }
                }

                msg += "E";
                Traveler.InsanityWriteLog(msg);

                CatHuntingSkill sourceCatSkill = source.SkillManager.GetSkill<CatHuntingSkill>(SkillNames.CatHunting);
                if (sourceCatSkill != null)
                {
                    CatHuntingSkill destCatSkill = dest.SkillManager.AddElement(SkillNames.CatHunting) as CatHuntingSkill;
                    if (destCatSkill != null)
                    {
                        destCatSkill.mOppCreepingAndCrawlingIsNew = sourceCatSkill.mOppCreepingAndCrawlingIsNew;
                        destCatSkill.mOppSlitherStalkerIsNew = sourceCatSkill.mOppSlitherStalkerIsNew;
                        destCatSkill.mOppFelineFisherIsNew = sourceCatSkill.mOppFelineFisherIsNew;
                        destCatSkill.mOppHomingWhiskersIsNew = sourceCatSkill.mOppHomingWhiskersIsNew;
                    }
                }

                msg += "F";
                Traveler.InsanityWriteLog(msg);

                DogHuntingSkill sourceDogSkill = source.SkillManager.GetSkill<DogHuntingSkill>(SkillNames.DogHunting);
                if (sourceDogSkill != null)
                {
                    DogHuntingSkill destDogSkill = dest.SkillManager.AddElement(SkillNames.DogHunting) as DogHuntingSkill;
                    if (destDogSkill != null)
                    {
                        destDogSkill.mOppGemFinderIsNew = sourceDogSkill.mOppGemFinderIsNew;
                        destDogSkill.mOppMetalFinderIsNew = sourceDogSkill.mOppMetalFinderIsNew;
                        destDogSkill.mOppRockFinderIsNew = sourceDogSkill.mOppRockFinderIsNew;
                        destDogSkill.mNumFragmentedItemsCompelted = sourceDogSkill.mNumFragmentedItemsCompelted;
                        destDogSkill.mNumUniqueCollectablesFound = sourceDogSkill.mNumUniqueCollectablesFound;
                        destDogSkill.mNumRGMFound = sourceDogSkill.mNumRGMFound;
                    }
                }

                FutureSkill sourceFutureSkill = source.SkillManager.GetSkill<FutureSkill>(SkillNames.Future);
                if(sourceFutureSkill != null)
                {
                    FutureSkill destFutureSkill = dest.SkillManager.AddElement(SkillNames.Future) as FutureSkill;
                    if(destFutureSkill != null)
                    {
                        destFutureSkill.mFoodSynthesized = sourceFutureSkill.mFoodSynthesized;
                    }
                }

                msg += "G";
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Common.Exception(source, null, msg, e);
            }
        }
    }
}