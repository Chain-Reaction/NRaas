using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Options.TryForBaby;
using NRaas.WoohooerSpace.Options.Woohoo;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.TombObjects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class CommonPregnancy : Common.IPreLoad
    {
        static Common.MethodStore sStoryProgressionAllowImpregnation = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AllowImpregnation", new Type[] { typeof(SimDescription), typeof(bool) });
        static Common.MethodStore sStoryProgressionAllowPregnancy = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AllowPregnancy", new Type[] { typeof(SimDescription), typeof(SimDescription), typeof(bool) });

        public static BuffNames sItsQuadruplets = unchecked((BuffNames)ResourceUtils.HashString64("NRaasItsQuadruplets"));

        // Delegate hook for third-party alteration
        public static GetChanceOfSuccess sGetChanceOfSuccess = OnGetChanceOfSuccess;

        public void OnPreLoad()
        {
            InteractionTuning tuning = tuning = Tunings.GetTuning<RabbitHole, Pregnancy.GoToHospital.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
                tuning.Availability.Adults = true;
                tuning.Availability.Elders = true;
            }

            tuning = Tunings.GetTuning<Lot, Pregnancy.HaveBabyHome.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
                tuning.Availability.Adults = true;
                tuning.Availability.Elders = true;
            }

            tuning = Tunings.GetTuning<RabbitHole, Pregnancy.HaveBabyHospital.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
                tuning.Availability.Adults = true;
                tuning.Availability.Elders = true;
            }
        }

        private static bool HasBlockingBuff(Sim sim)
        {
            if (sim == null) return false;

            if (sim.BuffManager == null) return false;

            return sim.BuffManager.HasAnyElement(new BuffNames[] { BuffNames.ItsABoy, BuffNames.ItsAGirl, BuffNames.ItsTwins, BuffNames.ItsTriplets, sItsQuadruplets });
        }

        public static bool SatisfiesMaximumOccupants(Sim sim, bool autonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (autonomous) return true;

            int count = 0;
            if (sim.IsHuman)
            {
                count = Households.NumHumansIncludingPregnancy(sim.Household);
            }
            else
            {
                count = Households.NumPetsIncludingPregnancy(sim.Household);
            }

            int maximum = Woohooer.Settings.mTryForBabyUserMaximum[PersistedSettings.GetSpeciesIndex(sim)];
            if ((maximum > 0) && (count > maximum))
            {
                greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:UserMaximum", sim.IsFemale); };
                return false;
            }

            return true;
        }

        public static bool CanTryForBaby(Sim a, Sim target, bool autonomous, CommonWoohoo.WoohooStyle style, ref GreyedOutTooltipCallback greyedOutTooltipCallback, out string reason)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration CanTryForBaby", Common.DebugLevel.Stats))
            {
                int chance = 0;
                bool teenCanTry = false;

                int speciesIndex = PersistedSettings.GetSpeciesIndex(a);

                PregnancyChoice pregnancyChoice = PregnancyChoice.Either;

                switch (style)
                {
                    case CommonWoohoo.WoohooStyle.Risky:
                        if ((a.SimDescription.Teen) || (target.SimDescription.Teen))
                        {
                            chance = Woohooer.Settings.mRiskyTeenBabyMadeChance;
                        }
                        else
                        {
                            chance = Woohooer.Settings.mRiskyBabyMadeChanceV2[speciesIndex];
                        }

                        teenCanTry = Woohooer.Settings.mTeenRiskyAutonomous;
                        pregnancyChoice = Woohooer.Settings.mRiskyPregnancyChoice;
                        break;
                    case CommonWoohoo.WoohooStyle.TryForBaby:
                        if ((a.SimDescription.Teen) || (target.SimDescription.Teen))
                        {
                            chance = Woohooer.Settings.mTryForBabyTeenBabyMadeChance;
                        }
                        else
                        {
                            chance = Woohooer.Settings.mTryForBabyMadeChanceV2[speciesIndex];
                        }

                        teenCanTry = Woohooer.Settings.mTeenTryForBabyAutonomous;
                        pregnancyChoice = Woohooer.Settings.mTryForBabyPregnancyChoice;
                        break;
                }

                if (chance <= 0)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Chance Fail");
                    reason = "Chance Fail";
                    return false;
                }

                if (!CommonSocials.CanGetRomantic(a, target, autonomous, true, true, ref greyedOutTooltipCallback, out reason))
                {
                    return false;
                }

                if (autonomous)
                {
                    if ((sStoryProgressionAllowPregnancy.Valid) && (Woohooer.Settings.TestStoryProgression(autonomous)))
                    {
                        reason = sStoryProgressionAllowPregnancy.Invoke<string>(new object[] { a.SimDescription, target.SimDescription, autonomous });
                        if (reason != null)
                        {
                            greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, false);
                            return false;
                        }
                    }
                }

                if (a.SimDescription.Gender == target.SimDescription.Gender)
                {
                    if (!Woohooer.Settings.mAllowSameSexTryForBabyV2[speciesIndex])
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Same Sex Fail");
                        reason = "Same Sex Fail";
                        return false;
                    }

                    if ((!CanGetPreggers(a, autonomous, ref greyedOutTooltipCallback, out reason)) && (!CanGetPreggers(target, autonomous, ref greyedOutTooltipCallback, out reason)))
                    {
                        return false;
                    }
                }
                else
                {
                    if (a.IsFemale)
                    {
                        if (!CanGetPreggers(a, autonomous, ref greyedOutTooltipCallback, out reason))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!CanGetPreggers(target, autonomous, ref greyedOutTooltipCallback, out reason))
                        {
                            return false;
                        }
                    }
                }

                if ((autonomous) || (Woohooer.Settings.mTestAllConditionsForUserDirected[speciesIndex]))
                {
                    if (HasBlockingBuff(a))
                    {
                        greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:BuffBlock"); };
                        reason = "BuffBlock";
                        return false;
                    }

                    if (HasBlockingBuff(target))
                    {
                        greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:BuffBlock"); };
                        reason = "BuffBlock";
                        return false;
                    }

                    if ((a.SimDescription.Gender != target.SimDescription.Gender) || (pregnancyChoice != PregnancyChoice.Either))
                    {
                        if (autonomous)
                        {
                            if (a.SimDescription.IsPregnant)
                            {
                                greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:Pregnant"); };
                                reason = "Pregnant";
                                return false;
                            }

                            if (target.SimDescription.IsPregnant)
                            {
                                greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:Pregnant"); };
                                reason = "Pregnant";
                                return false;
                            }
                        }
                        else
                        {
                            if (a.SimDescription.IsVisuallyPregnant)
                            {
                                greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:Pregnant"); };
                                reason = "Pregnant";
                                return false;
                            }

                            if (target.SimDescription.IsVisuallyPregnant)
                            {
                                greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:Pregnant"); };
                                reason = "Pregnant";
                                return false;
                            }
                        }
                    }

                    if ((a.SimDescription.IsMale) && (target.SimDescription.IsMale))
                    {
                        if (!Woohooer.Settings.mAutonomousMaleMaleTryForBabyV2[speciesIndex])
                        {
                            greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:SameSexDenied"); };
                            reason = "SameSexDenied";
                            return false;
                        }

                        if (a.SimDescription.Elder)
                        {
                            greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:Elder"); };
                            reason = "Elder";
                            return false;
                        }

                        if (target.SimDescription.Elder)
                        {
                            greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:Elder"); };
                            return false;
                        }
                    }

                    if ((a.SimDescription.IsFemale) && (target.SimDescription.IsFemale))
                    {
                        if (!Woohooer.Settings.mAutonomousFemaleFemaleTryForBabyV2[speciesIndex])
                        {
                            greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:SameSexDenied"); };
                            reason = "SameSexDenied";
                            return false;
                        }
                    }

                    if ((a.SimDescription.Elder) && (a.SimDescription.IsFemale))
                    {
                        greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:Elder"); };
                        reason = "Elder";
                        return false;
                    }

                    if ((target.SimDescription.Elder) && (target.SimDescription.IsFemale))
                    {
                        greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:Elder"); };
                        reason = "Elder";
                        return false;
                    }

                    if ((a.SimDescription.Teen) || (target.SimDescription.Teen))
                    {
                        if ((!teenCanTry) && (autonomous))
                        {
                            greyedOutTooltipCallback = delegate
                            {
                                return Common.LocalizeEAString("NRaas.Woohooer:Teenagers");
                            };
                            reason = "Teenagers";
                            return false;
                        }
                    }

                    if ((SimTypes.IsSkinJob(a.SimDescription)) || (SimTypes.IsSkinJob(target.SimDescription)))
                    {
                        greyedOutTooltipCallback = delegate { return Common.Localize("TryForBaby:SkinJob"); };
                        reason = "SkinJob";
                        return false;
                    }
                }

                return true;
            }
        }

        private static bool CanGetPreggers(Sim sim, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback, out string reason)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration CanGetPreggers", Common.DebugLevel.Stats))
            {
                if (SimTypes.IsPassporter(sim.SimDescription))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Passport");
                    reason = "Passport";
                    return false;
                }

                if (isAutonomous)
                {
                    if (sim.SimDescription.Elder)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Elder");
                        reason = "Elder";
                        return false;
                    }
                    else if (Households.IsFull(sim.Household, sim.IsPet, Woohooer.Settings.mMaximumHouseholdSizeForAutonomousV2[PersistedSettings.GetSpeciesIndex(sim)]))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("House Full");
                        reason = "House Full";
                        return false;
                    }
                }
                else
                {
                    if (!SatisfiesMaximumOccupants(sim, isAutonomous, ref greyedOutTooltipCallback))
                    {
                        reason = "MaximumOccupants";
                        return false;
                    }
                }

                if (SimTypes.IsSkinJob(sim.SimDescription))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Skin Job Fail");
                    reason = "Skin Job Fail";
                    return false;
                }
                else if (sim.BuffManager.HasTransformBuff())
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Transform Buff");
                    reason = "Transform Buff";
                    return false;
                }
                else if (sim.SimDescription.IsVisuallyPregnant)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Already Pregnant");
                    reason = "Already Pregnant";
                    return false;
                }

                if ((sim.Household != null) && (sim.Household.IsTouristHousehold))
                {
                    MiniSimDescription description = MiniSimDescription.Find(sim.SimDescription.SimDescriptionId);
                    if (description == null)
                    {
                        greyedOutTooltipCallback = delegate
                        {
                            return Common.LocalizeEAString(sim.IsFemale, "Gameplay/Actors/Sim/TryForBaby:TooManySims", new object[] { sim });
                        };
                        reason = "TooManySims";
                        return false;
                    }
                }
                else if (sim.LotHome == null)
                {
                    greyedOutTooltipCallback = delegate
                    {
                        if (sim.Household.IsAlienHousehold)
                        {
                            return Common.LocalizeEAString(sim.IsFemale, "Gameplay/Actors/Sim/TryForBaby:AlienNPCs", new object[] { sim });
                        }
                        else
                        {
                            return Common.LocalizeEAString(sim.IsFemale, "Gameplay/Actors/Sim/TryForBaby:TooManySims", new object[] { sim });
                        }
                    };
                    reason = "TooManySims";
                    return false;
                }
                else if ((sim.SimDescription.IsDueToAgeUp()) ||
                         ((sim.SimDescription.AgingState != null) && (sim.SimDescription.AgingState.IsAgingInProgress())))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Aging Up Fail");
                    reason = "Aging Up Fail";
                    return false;
                }
                else if (SimTypes.IsLampGenie(sim.SimDescription))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Lamp Genie");
                    reason = "Lamp Genie";
                    return false;
                }

                if ((sStoryProgressionAllowImpregnation.Valid) && (Woohooer.Settings.TestStoryProgression(isAutonomous)))
                {
                    reason = sStoryProgressionAllowImpregnation.Invoke<string>(new object[] { sim.SimDescription, isAutonomous });
                    if (reason != null)
                    {
                        greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, false);
                        return false;
                    }
                }

                reason = null;
                return true;
            }
        }

        public static bool IsSuccess(Sim simA, Sim simB, bool isAutonomous, CommonWoohoo.WoohooStyle style)
        {
            if (Woohooer.Settings.ReplaceWithRisky)
            {
                if (style == CommonWoohoo.WoohooStyle.Safe)
                {
                    style = CommonWoohoo.WoohooStyle.Risky;
                }
            }

            string reason;
            GreyedOutTooltipCallback callBack = null;
            if (!CanTryForBaby(simA, simB, isAutonomous, style, ref callBack, out reason))
            {
                if (callBack != null)
                {
                    Common.DebugNotify("Pregnancy: " + callBack(), simA, simB);
                }
                return false;
            }

            float chance = sGetChanceOfSuccess(simA, simB, style);

            if (!RandomUtil.RandomChance(chance))
            {
                Common.DebugNotify("Pregnancy: Chance Fail " + chance, simA, simB);
                return false;
            }

            Common.DebugNotify("Pregnancy: Chance Success " + chance, simA, simB);
            return true;
        }

        public static float OnGetChanceOfSuccess(Sim a, Sim b, CommonWoohoo.WoohooStyle style)
        {
            float chance = 0;

            bool useFertility = true;

            int speciesIndex = PersistedSettings.GetSpeciesIndex(a);

            switch (style)
            {
                case CommonWoohoo.WoohooStyle.Risky:
                    if ((a.SimDescription.Teen) || (b.SimDescription.Teen))
                    {
                        chance = Woohooer.Settings.mRiskyTeenBabyMadeChance;
                    }
                    else
                    {
                        chance = Woohooer.Settings.mRiskyBabyMadeChanceV2[speciesIndex];
                    }

                    useFertility = Woohooer.Settings.mRiskyFertility[speciesIndex];
                    break;
                case CommonWoohoo.WoohooStyle.TryForBaby:
                    if ((a.SimDescription.Teen) || (b.SimDescription.Teen))
                    {
                        chance = Woohooer.Settings.mTryForBabyTeenBabyMadeChance;
                    }
                    else
                    {
                        chance = Woohooer.Settings.mTryForBabyMadeChanceV2[speciesIndex];
                    }

                    useFertility = Woohooer.Settings.mTryForBabyFertility[speciesIndex];
                    break;
            }

            if (chance <= 0)
            {
                Common.DebugNotify("Pregnancy: No Chance");
                return 0;
            }

            if (useFertility)
            {
                if (a.IsHuman)
                {
                    if ((a.BuffManager != null) && a.BuffManager.HasTransformBuff())
                    {
                        return 0;
                    }

                    if ((b.BuffManager == null) && b.BuffManager.HasTransformBuff())
                    {
                        return 0;
                    }

                    if ((a.TraitManager.HasElement(TraitNames.FertilityTreatment)) || ((a.BuffManager != null) && a.BuffManager.HasElement(BuffNames.ATwinkleInTheEye)))
                    {
                        chance += TraitTuning.kFertilityBabyMakingChanceIncrease;
                    }

                    if ((b.TraitManager.HasElement(TraitNames.FertilityTreatment)) || ((b.BuffManager != null) && b.BuffManager.HasElement(BuffNames.ATwinkleInTheEye)))
                    {
                        chance += TraitTuning.kFertilityBabyMakingChanceIncrease;
                    }
                }
                else
                {
                    if (a.TraitManager.HasElement(TraitNames.FertilityTreatmentPet))
                    {
                        chance += TraitTuning.kFertilityLitterMakingChanceIncrease;
                    }

                    if (b.TraitManager.HasElement(TraitNames.FertilityTreatmentPet))
                    {
                        chance += TraitTuning.kFertilityLitterMakingChanceIncrease;
                    }
                }

                if (a.TraitManager.HasElement(TraitNames.WishedForLargeFamily))
                {
                    chance += 100f;
                    a.BuffManager.RemoveElement(BuffNames.WishForLargeFamily);
                }

                if (b.TraitManager.HasElement(TraitNames.WishedForLargeFamily))
                {
                    chance += 100f;
                    b.BuffManager.RemoveElement(BuffNames.WishForLargeFamily);
                }

                if ((a.BuffManager != null) && a.BuffManager.HasElement(BuffNames.MagicInTheAir))
                {
                    chance += BuffMagicInTheAir.kBabyMakingChanceIncrease * 100f;
                }

                if ((b.BuffManager != null) && b.BuffManager.HasElement(BuffNames.MagicInTheAir))
                {
                    chance += BuffMagicInTheAir.kBabyMakingChanceIncrease * 100f;
                }

                if ((GameUtils.IsInstalled(ProductVersion.EP7)) && (SimClock.IsNightTime()) && (SimClock.IsFullMoon()))
                {
                    chance += Pregnancy.kFullMoonImprovedBabyChance * 100f;
                }
            }

            return chance;
        }

        public delegate float GetChanceOfSuccess(Sim a, Sim b, CommonWoohoo.WoohooStyle style);

        public static Pregnancy Impregnate(Sim actor, Sim target, bool isAutonomous, CommonWoohoo.WoohooStyle style)
        {
            PregnancyChoice choice = Woohooer.Settings.mTryForBabyPregnancyChoice;
            bool playChimes = true;

            if (Woohooer.Settings.ReplaceWithRisky || style == CommonWoohoo.WoohooStyle.Risky)
            {
                choice = Woohooer.Settings.mRiskyPregnancyChoice;
                playChimes = false;
            }

            Sim impregnated;
            Sim inseminator;
            bool switchIfFail = false;

            if (actor.SimDescription.Gender == target.SimDescription.Gender)
            {
                switch (choice)
                {
                    case PregnancyChoice.Initiator:
                        impregnated = actor;
                        inseminator = target;
                        break;
                    case PregnancyChoice.Target:
                        impregnated = target;
                        inseminator = actor;
                        break;
                    default:
                    case PregnancyChoice.Either:
                        impregnated = actor;
                        inseminator = target;
                        switchIfFail = true;
                        break;
                    case PregnancyChoice.Random:
                        if (RandomUtil.CoinFlip())
                        {
                            impregnated = actor;
                            inseminator = target;
                        }
                        else
                        {
                            impregnated = target;
                            inseminator = actor;
                        }
                        switchIfFail = true;
                        break;
                    case PregnancyChoice.TargetThenInitiator:
                        impregnated = target;
                        inseminator = actor;
                        switchIfFail = true;
                        break;
                }
            }
            else
            {
                if (actor.IsFemale)
                {
                    impregnated = actor;
                    inseminator = target;
                }
                else
                {
                    impregnated = target;
                    inseminator = actor;
                }
            }

            Pregnancy pregnancy = StartPregnancy(impregnated, inseminator, isAutonomous, playChimes);
            if (pregnancy == null && switchIfFail)
            {
                pregnancy = StartPregnancy(inseminator, impregnated, isAutonomous, playChimes);
            }
            return pregnancy;
        }

        private static Pregnancy StartPregnancy(Sim woman, Sim man, bool isAutonomous, bool playChimes)
        {
            string reason;
            GreyedOutTooltipCallback callback = null;
            if (!CanGetPreggers(woman, isAutonomous, ref callback, out reason))
            {
                if (callback != null)
                {
                    Common.DebugNotify("Pregnancy: " + callback(), woman);
                }
                return null;
            }
            else if (woman.SimDescription.IsPregnant)
            {
                Common.DebugNotify("Already Pregnant", woman);
                return woman.SimDescription.Pregnancy;
            }

            Pregnancy p = Pregnancies.Start(woman, man.SimDescription, !Woohooer.Settings.mAllowPlantSimPregnancy);

            if (p != null)
            {
                if ((playChimes) && ((woman.IsSelectable) || (man.IsSelectable)))
                {
                    Audio.StartSound("sting_baby_conception");
                }

                Common.DebugNotify("Pregnancy: Success", woman);
            }

            return p;
        }

        public static bool SatisfiesRisky(Sim actor, Sim target, string logName, bool isAutonomous, bool scoreTarget, ref GreyedOutTooltipCallback callback)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration " + logName, Common.DebugLevel.Stats))
            {
                if (Woohooer.Settings.ReplaceWithRisky)
                {
                    callback = Common.DebugTooltip("Replace With Risky Fail");

                    ScoringLookup.IncStat(logName + " Replace With Risky Fail");
                    return false;
                }

                int speciesIndex = PersistedSettings.GetSpeciesIndex(actor);

                if (isAutonomous)
                {
                    if (!Woohooer.Settings.mRiskyAutonomousV2[speciesIndex])
                    {
                        ScoringLookup.IncStat(logName + " Autonomous Fail");
                        return false;
                    }

                    if (!Woohooer.Settings.mAllowOffLotRiskyAutonomous[speciesIndex])
                    {
                        if (actor.LotCurrent != Household.ActiveHousehold.LotHome)
                        {
                            ScoringLookup.IncStat(logName + " OffLot Fail");
                            return false;
                        }
                    }
                }

                if ((!scoreTarget) && (!CommonWoohoo.SatisfiesUserLikingGate(actor, target, isAutonomous, true, logName)))
                {
                    callback = Common.DebugTooltip("Liking Gate Fail");

                    ScoringLookup.IncStat(logName + " Liking Gate Fail");
                    return false;
                }

                if (!CommonSocials.SatisfiedInteractionLevel(actor, target, isAutonomous, ref callback))
                {
                    ScoringLookup.IncStat(logName + " InteractionLevel Fail");
                    return false;
                }

                if (!WoohooInteractionLevelSetting.Satisfies(actor, target, true))
                {
                    ScoringLookup.IncStat(logName + " Interaction Level Fail");

                    callback = Common.DebugTooltip("Interaction Level Fail");
                    return false;
                }

                if (!CommonWoohoo.SatisfiesCooldown(actor, target, isAutonomous, ref callback))
                {
                    return false;
                }

                string reason;
                if (!CanTryForBaby(actor, target, isAutonomous, CommonWoohoo.WoohooStyle.Risky, ref callback, out reason))
                {
                    ScoringLookup.IncStat(logName + " " + reason);
                    return false;
                }

                WoohooScoring.ScoreTestResult result = WoohooScoring.ScoreActor(logName, actor, target, isAutonomous, "InterestInRisky", true);
                if (result != WoohooScoring.ScoreTestResult.Success)
                {
                    ScoringLookup.IncStat(logName + " " + result);

                    callback = Common.DebugTooltip("Actor Scoring Fail " + result);
                    return false;
                }

                if (scoreTarget)
                {
                    result = WoohooScoring.ScoreTarget(logName, target, actor, isAutonomous, "InterestInRisky", true);
                    if (result != WoohooScoring.ScoreTestResult.Success)
                    {
                        ScoringLookup.IncStat(logName + " " + result);

                        callback = Common.DebugTooltip("Target Scoring Fail " + result);
                        return false;
                    }
                }
                else
                {
                    ScoringLookup.IncStat(logName + " Target Not Scored");
                }

                ScoringLookup.IncStat(logName + " Success");
                return true;
            }
        }

        public static bool SatisfiesTryForBaby(Sim actor, Sim target, string logName, bool isAutonomous, bool scoreTarget, ref GreyedOutTooltipCallback callback)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration " + logName, Common.DebugLevel.Stats))
            {
                int speciesIndex = PersistedSettings.GetSpeciesIndex(actor);

                if (isAutonomous)
                {
                    if (!Woohooer.Settings.mTryForBabyAutonomousV2[speciesIndex])
                    {
                        return false;
                    }

                    if (!Woohooer.Settings.mAllowOffLotTryForBabyAutonomous[speciesIndex])
                    {
                        if (actor.LotCurrent != Household.ActiveHousehold.LotHome)
                        {
                            return false;
                        }
                    }
                }

                if ((!scoreTarget) && (!CommonWoohoo.SatisfiesUserLikingGate(actor, target, isAutonomous, true, logName)))
                {
                    callback = Common.DebugTooltip("Liking Gate Fail");
                    return false;
                }

                if (!CommonSocials.SatisfiedInteractionLevel(actor, target, isAutonomous, ref callback))
                {
                    return false;
                }

                if (!WoohooInteractionLevelSetting.Satisfies(actor, target, true))
                {
                    ScoringLookup.IncStat(logName + " Interaction Level Fail");

                    callback = Common.DebugTooltip("Interaction Level Fail");
                    return false;
                }

                if (!CommonWoohoo.SatisfiesCooldown(actor, target, isAutonomous, ref callback))
                {
                    return false;
                }

                string reason;
                if (!CanTryForBaby(actor, target, isAutonomous, CommonWoohoo.WoohooStyle.TryForBaby, ref callback, out reason))
                {
                    ScoringLookup.IncStat(logName + " " + reason);
                    return false;
                }

                WoohooScoring.ScoreTestResult result = WoohooScoring.ScoreActor(logName, actor, target, isAutonomous, "InterestInTryForBaby", true);
                if (result != WoohooScoring.ScoreTestResult.Success)
                {
                    ScoringLookup.IncStat(logName + " " + result);

                    callback = Common.DebugTooltip("Actor Scoring Fail " + result);
                    return false;
                }

                if (scoreTarget)
                {
                    result = WoohooScoring.ScoreTarget(logName, target, actor, isAutonomous, "InterestInTryForBaby", true);
                    if (result != WoohooScoring.ScoreTestResult.Success)
                    {
                        ScoringLookup.IncStat(logName + " " + result);

                        callback = Common.DebugTooltip("Target Scoring Fail " + result);
                        return false;
                    }
                }

                ScoringLookup.IncStat(logName + " Success");
                return true;
            }
        }
    }
}
