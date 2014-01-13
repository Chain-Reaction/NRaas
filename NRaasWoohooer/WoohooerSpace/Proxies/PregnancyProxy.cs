using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Proxies
{
    public class PregnancyProxy : Pregnancy
    {
        public PregnancyProxy(Pregnancy src)
        {
            CopyPregnancy(this, src);
        }

        public static void CopyPregnancy(Pregnancy dst, Pregnancy src)
        {
            dst.DadDescriptionId = src.DadDescriptionId;
            dst.mBabySexOffset = src.mBabySexOffset;
            dst.mChanceOfRandomOccultMutation = src.mChanceOfRandomOccultMutation;
            dst.mContractionBroadcast = src.mContractionBroadcast;
            dst.mContractionsAlarm = src.mContractionsAlarm;
            dst.mCurrentMoodScore = src.mCurrentMoodScore;
            dst.mDad = src.mDad;
            dst.mDadDeathType = src.mDadDeathType;
            dst.mDadOccultType = src.mDadOccultType;
            dst.mDadWasGhostFromPotion = src.mDadWasGhostFromPotion;
            dst.mDoctorAdviceGivenBonus = src.mDoctorAdviceGivenBonus;
            dst.mFixupForeignPregnancy = src.mFixupForeignPregnancy;
            dst.mForcedTrait = src.mForcedTrait;
            dst.mGender = src.mGender;
            dst.mHasRequestedWalkStyle = src.mHasRequestedWalkStyle;
            dst.mHasShownPregnancy = src.mHasShownPregnancy;
            dst.mHourOfPregnancy = src.mHourOfPregnancy;
            dst.mMom = src.mMom;
            dst.mMomDeathType = src.mMomDeathType;
            dst.mMomOccultType = src.mMomOccultType;
            dst.mMomWasGhostFromPotion = src.mMomWasGhostFromPotion;
            dst.mMultipleBabiesMultiplier = src.mMultipleBabiesMultiplier;
            dst.mPregnancyScore = src.mPregnancyScore;
            dst.mRandomGenSeed = src.mRandomGenSeed;
            dst.mStereoStartTime = src.mStereoStartTime;
            dst.mTimeMoodSampled = src.mTimeMoodSampled;
            dst.mTvStartTime = src.mTvStartTime;
            dst.PreggersAlarm = src.PreggersAlarm;
        }

        public override int GetNumForBirth(SimDescription dadDescription, Random pregoRandom, int numSimMembers, int numPetMembers)
        {
            try
            {
                int desiredNumChilderen = 0x1;
                if (mMom.TraitManager.HasElement(TraitNames.WishedForLargeFamily))
                {
                    mMom.TraitManager.RemoveElement(TraitNames.WishedForLargeFamily);
                    desiredNumChilderen = 0x4;
                }

                if ((dadDescription != null) && (dadDescription.TraitManager.HasElement(TraitNames.WishedForLargeFamily)))
                {
                    dadDescription.TraitManager.RemoveElement(TraitNames.WishedForLargeFamily);
                    desiredNumChilderen = 0x4;
                }

                if (desiredNumChilderen != 4)
                {
                    mMultipleBabiesMultiplier = Math.Min(mMultipleBabiesMultiplier, kMaxBabyMultiplier);

                    if (mMom.HasTrait(TraitNames.FertilityTreatment))
                    {
                        mMultipleBabiesMultiplier *= TraitTuning.kFertilityMultipleBabiesMultiplier;
                    }
                    else if ((mMom.BuffManager != null) && mMom.BuffManager.HasElement(BuffNames.ATwinkleInTheEye))
                    {
                        mMultipleBabiesMultiplier *= TraitTuning.kFertilityMultipleBabiesMultiplier;
                    }

                    if (dadDescription != null)
                    {
                        if (dadDescription.HasTrait(TraitNames.FertilityTreatment))
                        {
                            mMultipleBabiesMultiplier *= TraitTuning.kFertilityMultipleBabiesMultiplier;
                        }
                        else if ((dadDescription.CreatedSim != null) && (dadDescription.CreatedSim.BuffManager != null) && dadDescription.CreatedSim.BuffManager.HasElement(BuffNames.ATwinkleInTheEye))
                        {
                            mMultipleBabiesMultiplier *= TraitTuning.kFertilityMultipleBabiesMultiplier;
                        }
                    }

                    double num2 = pregoRandom.NextDouble();
                    if (num2 < (kChanceOfTwins * mMultipleBabiesMultiplier))
                    {
                        desiredNumChilderen++;
                        if (num2 < (kChanceOfTriplets * mMultipleBabiesMultiplier))
                        {
                            desiredNumChilderen++;
                            if (num2 < (Woohooer.Settings.mChanceOfQuads * mMultipleBabiesMultiplier))
                            {
                                desiredNumChilderen++;
                            }
                        }
                    }
                }

                return desiredNumChilderen;

                // Greater than Eight check
                //return Household.GetAllowableNumChildren(simDescription, desiredNumChilderen, numSimMembers, numPetMembers);
            }
            catch (Exception e)
            {
                Common.Exception(Mom.SimDescription, dadDescription, e);
                return 1;
            }
        }
    }
}
