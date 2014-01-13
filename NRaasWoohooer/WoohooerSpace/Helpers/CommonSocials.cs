using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Options.Romance;
using NRaas.WoohooerSpace.Options.Woohoo;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace NRaas.WoohooerSpace.Helpers
{
    public class CommonSocials : Common.IPreLoad, Common.IWorldLoadFinished
    {
        static Common.MethodStore sStoryProgressionAllowBreakup = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AllowBreakup", new Type[] { typeof(SimDescription), typeof(bool) });
        static Common.MethodStore sStoryProgressionAllowAffair = new Common.MethodStore("NRaasStoryProgressionRelationship", "NRaas.StoryProgressionModule", "AllowAffair", new Type[] { typeof(SimDescription), typeof(SimDescription), typeof(bool) });
        static Common.MethodStore sStoryProgressionAllowRomance = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AllowRomance", new Type[] { typeof(SimDescription), typeof(bool) });
        static Common.MethodStore sStoryProgressionAllowSteady = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AllowSteady", new Type[] { typeof(SimDescription), typeof(SimDescription), typeof(bool) });
        static Common.MethodStore sStoryProgressionAllowMarriage = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "AllowMarriage", new Type[] { typeof(SimDescription), typeof(SimDescription), typeof(bool) });
        static Common.MethodStore sStoryProgressionCanRomanticInteract = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "CanRomanticInteract", new Type[] { typeof(SimDescription), typeof(SimDescription), typeof(bool) });
        static Common.MethodStore sStoryProgressionCanFriendInteract = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "CanFriendInteract", new Type[] { typeof(SimDescription), typeof(SimDescription), typeof(bool) });

        static Dictionary<string, JealousyLevel> sRomanticSocials = new Dictionary<string, JealousyLevel>();

        static List<SocialRuleRHS> sKissRules = new List<SocialRuleRHS>();

        static Dictionary<ShortTermContextTypes, ShortTermContextTypes> sRomanticToFriendly = new Dictionary<ShortTermContextTypes, ShortTermContextTypes>();

        static Tracer sTracer = new Tracer();

        static CommonSocials()
        {
            sRomanticToFriendly.Add(ShortTermContextTypes.Flirty, ShortTermContextTypes.Ok);
            sRomanticToFriendly.Add(ShortTermContextTypes.Seductive, ShortTermContextTypes.Friendly);
            sRomanticToFriendly.Add(ShortTermContextTypes.Hot, ShortTermContextTypes.VeryFriendly);
        }

        public static JealousyLevel GetRomanticJealousy(string key, bool pure)
        {
            JealousyLevel level;
            if (!sRomanticSocials.TryGetValue(key, out level)) return JealousyLevel.None;

            if ((!pure) && (level > Woohooer.Settings.mRomanceJealousyLevel))
            {
                return Woohooer.Settings.mRomanceJealousyLevel;
            }

            return level;
        }

        public void OnPreLoad()
        {
            // We use this one because it is ahead of other checks
            ActiveTopicData celebrityTopic = ActiveTopicData.Get("CelebrityTopic");
            if (celebrityTopic != null)
            {
                celebrityTopic.TestFunction = typeof(CommonSocials).GetMethod("AlterSTC");
            }

            ActiveTopicData alwaysOnTopic = ActiveTopicData.Get("Always On");
            if (alwaysOnTopic != null)
            {
                alwaysOnTopic.TestFunction = typeof(CommonSocials).GetMethod("AlterSTC");
            }

            InteractionTuning tuning = Tunings.GetTuning<IWeddingArch, WeddingArch.RouteToWeddingArchSlot.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            tuning = Tunings.GetTuning<WeddingCake, WeddingCake.CutWeddingCake.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            STCData.SetNumSocialsDuringConversation(int.MaxValue);

            if (GameUtils.IsInstalled(ProductVersion.EP4))
            {
                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Be Frisky", "OnBeFriskyAccept"));

                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("BeFriskyProceduralTest"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("CanWatchTheStars"));

                tuning = Tunings.GetTuning<Sim,Sim.WatchTheStars.Definition>();
                if (tuning != null)
                {
                    tuning.RemoveFlags(InteractionTuning.FlagField.DisallowAutonomous);
                }

                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestAskToProm"));
            }

            if (GameUtils.IsInstalled(ProductVersion.EP5))
            {
                BooterLogger.AddTrace(ActionDataReplacer.Perform<CommonSocials>("TestHorseWooHoo", "OnEAHorseWoohooTest"));
                BooterLogger.AddTrace(ActionDataReplacer.Perform<CommonSocials>("TestPetWooHoo", "OnEAPetWoohooTest"));

                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Pet Woohoo", "OnPetWoohooAccepted"));
                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("HorseWooHoo", "OnHorseWooHoo"));

                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Adopt Stray", "OnStrayPetAdopted"));
                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Adopt Wild Horse", "AdoptWildHorseSuccessProcAfter"));
                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Unicorn Invite", "OnUnicornInviteAccept"));
                
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("AdoptStrayTest"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("AdoptWildHorseProceduralTest"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("UnicornInviteTest"));
            }

            if (GameUtils.IsInstalled(ProductVersion.EP7))
            {
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestLunaticBuff"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestNuzzleRomanticCheck"));

                ActionAvailabilityReplacer.Perform("Confess To Watching You While You Sleep", sRomanticToFriendly);
            }

            if (GameUtils.IsInstalled(ProductVersion.EP9))
            {
                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Spicy WooHoo", "OnSpicyWooHoo"));

                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestJuicedKiss"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestJuicedLeapIntoArms"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestJuicedMakeOut"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestJuicedWooHoo"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestKissAndMakeUp"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestSpicyWooHoo"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestCanSketchTargetNude"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestCinnamonKiss"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestHeatOfMomentKiss"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestIsPlantSim"));                
            }

            if (GameUtils.IsInstalled(ProductVersion.EP11))
            {
                BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Jetpack Woohoo", "AfterJetPackWoohoo"));

                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestDipKiss"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestElectrifiedKiss"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestJetPackKiss"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestJetPackSlowDance"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestJetPackWooHoo"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestSparkKiss"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestCanUploadFeelings"));
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestCanComplimentHardware"));
            }
            
            ActionAvailabilityReplacer.Perform("Make Promise To Protect", sRomanticToFriendly);
            ActionAvailabilityReplacer.Perform("Ask To Be Protected", sRomanticToFriendly);

            BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("WooHoo", "OnWooHoo"));
            BooterLogger.AddError(SocialRHSReplacer.Perform<CommonSocials>("Try For Baby", "OnTryForBaby"));

            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestLetsGoOnDate"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestFirstKiss"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestKiss"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestKissOnCheek"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestMakeOut"));
            BooterLogger.AddTrace(ActionDataReplacer.Perform<CommonSocials>("TestWooHoo", "OnEAWoohooTest"));
            BooterLogger.AddTrace(ActionDataReplacer.Perform<CommonSocials>("TestTryForBaby", "OnEATryForBabyTest"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestProposeMarriage"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestHavePrivateWedding"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestGetMarried"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestGetMarriedUsingArch"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestAccuseOfCheating"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestProposeGoingSteady"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestAskNPCToStayOver"));
            BooterLogger.AddError(ActionDataReplacer.PerformKey<CommonSocials>("TestAskToStayOver", "Ask To Stay Over Romantically"));

            ActionData askSign = ActionData.Get("Ask Sign");
            if ((askSign != null) && (askSign.IntendedCommodityString == CommodityTypes.Amorous))
            {
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestAskSign", "TestDisable"));
            }

            ActionData askAboutPartner = ActionData.Get("Ask About Partner");
            if ((askAboutPartner != null) && (askAboutPartner.IntendedCommodityString == CommodityTypes.Amorous))
            {
                BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestAskAboutPartner", "TestDisable"));
            }

            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestCanVibrateBed"));
            BooterLogger.AddError(ActionDataReplacer.Perform<CommonSocials>("TestProposeToMoveInWith"));
            
            MethodInfo testAttraction = typeof(CommonSocials).GetMethod("OnTestAttraction");
            MethodInfo defaultTest = typeof(CommonSocials).GetMethod("OnDefaultTest");
            MethodInfo defaultNonRomanticTest = typeof(CommonSocials).GetMethod("OnDefaultNonRomanticTest");
            MethodInfo stopMethod = typeof(CommonSocials).GetMethod("OnDenyTest");
            MethodInfo testMassage = typeof(CommonSocials).GetMethod("OnTestMassage");
            MethodInfo testBreakup = typeof(CommonSocials).GetMethod("OnTestBreakup");

            CASAGSAvailabilityFlags restrictedAges = CASAGSAvailabilityFlags.HumanBaby | CASAGSAvailabilityFlags.HumanToddler | CASAGSAvailabilityFlags.HumanChild;

            foreach (ActionData data in ActionData.sData.Values)
            {
                if (!data.IsRomantic)
                {
                    switch (data.Key)
                    {
                        case "Ask For Son/Daughters Hand":
                        case "Announce Engagement":
                        case "Announce Pregnancy":
                        case "Ask Determine Gender Of Baby":
                        case "Ask to Move In":
                        case "Cancel Wedding":
                        case "Divorce":
                        case "Fret Over Commitment":
                        case "Request Feel My Tummy":
                        case "Tell About Betrayers Other Relationship":
                        case "WooHoo AutoReject":
                        case "Accuse of Cheating":
                            data.mActorAgeAllowed |= CASAGSAvailabilityFlags.HumanTeen;
                            data.mTargetAgeAllowed |= CASAGSAvailabilityFlags.HumanTeen;
                            break;
                    }

                    if (data.ProceduralTest == null)
                    {
                        data.ProceduralTest = defaultNonRomanticTest;
                    }

                    continue;
                }
                else
                {
                    bool ignore = false;

                    switch (data.Key)
                    {
                        case "Greet Flirty":
                        case "Greet Hot":
                        case "Greet Seductive":
                            ignore = true;
                            break;
                    }

                    if (ignore) continue;
                }

                if (!sRomanticSocials.ContainsKey(data.Key))
                {
                    sRomanticSocials.Add(data.Key, data.JealousyLevel);
                }

                data.mJealousyLevel = JealousyLevel.None;

                data.IsRomantic = false; // Disables CanGetRomantic checks

                data.mActorAgeAllowed |= CASAGSAvailabilityFlags.HumanTeen;
                data.mTargetAgeAllowed |= CASAGSAvailabilityFlags.HumanTeen;

                if (data.Key.Contains("NRaas")) continue;

                if ((data.Key == "WooHoo") || (data.Key == "Pet Woohoo") || (data.Key == "HorseWooHoo"))
                {
                    continue;
                }
                else if (data.Key == "Try For Baby")
                {
                    continue;
                }
                else if (data.Key == "Massage")
                {
                    data.ProceduralTest = testMassage;
                }
                else if (data.ProceduralTest == null)
                {
                    if ((data.Key == "Break Up") || (data.Key == "Divorce"))
                    {
                        data.ProceduralTest = testBreakup;
                    }
                    else
                    {
                        data.ProceduralTest = defaultTest;
                    }
                }
                else if (!data.ProceduralTest.DeclaringType.AssemblyQualifiedName.Contains("NRaas"))
                {
                    switch (data.Key)
                    {
                        case "Vampire Drink":
                        case "Ask About Partner":
                        case "Vampire Offer To Turn":
                        case "Ask To Forsake Vampire Guile":
                        case "Ask To Forsake Witchitude":
                        case "Ask To Forsake Fairy Charms":
                        case "Ask To Forsake Werewolf Powers":
                        case "Demonstrate MtoM":
                            break;
                        default:
                            BooterLogger.AddError(data.Key + ": " + data.ProceduralTest);
                            break;
                    }
                }
                
                bool found = false;
                if (SocialRuleRHS.sDictionary.ContainsKey(data.Key))
                {
                    foreach (SocialRuleRHS rule in SocialRuleRHS.Get(data.Key))
                    {
                        if ((rule.InteractionBitsAdded & LongTermRelationship.InteractionBits.Kissed) == LongTermRelationship.InteractionBits.Kissed)
                        {
                            if ((rule.InteractionBitsAdded & LongTermRelationship.InteractionBits.Romantic) == LongTermRelationship.InteractionBits.Romantic)
                            {
                                sKissRules.Add(rule);

                                BooterLogger.AddTrace("Kiss Rule: " + data.Key);
                            }
                        }

                        /*
                        if ((rule.InteractionBitsAdded & LongTermRelationship.InteractionBits.Romantic) == LongTermRelationship.InteractionBits.Romantic)
                        {
                            if (!sRomanticSetters.ContainsKey(data.Key))
                            {
                                sRomanticSetters.Add(data.Key, true);
                            }

                            rule.mInteractionBitsAdded &= ~LongTermRelationship.InteractionBits.Romantic;
                        }
                        */
                        if (rule.STEffectCommodity == CommodityTypes.Awkward)
                        {
                            found = true;
                        }
                    }

                    if (found)
                    {
                        SocialRuleLHS newRule = new SocialRuleLHS(
                            data.Key, 
                            CommodityTypes.Undefined, 
                            SpeechAct.None, 
                            0, 
                            0, 
                            new Pair<CommodityTypes, bool>(CommodityTypes.Undefined, true), 
                            new Pair<ShortTermContextTypes, bool>(ShortTermContextTypes.Undefined, true), 
                            -101, 
                            new Pair<LongTermRelationshipTypes, bool>(LongTermRelationshipTypes.Undefined, true), 
                            -100, 
                            100, 
                            "", 
                            "", 
                            "", 
                            1, 
                            true,
                            "", 
                            "", 
                            "", 
                            "", 
                            "", 
                            MaybeBool.False, 
                            MaybeBool.False, 
                            MaybeBool.False, 
                            restrictedAges, 
                            restrictedAges, 
                            MaybeBool.False, 
                            1000, 
                            CommodityTypes.Awkward, 
                            0
                        );
                        newRule.ProceduralPrecondition = testAttraction;

                        SocialRuleLHS.Get(data.Key).Sort(new Comparison<SocialRuleLHS>(SocialRuleLHS.SortSocialRules));
                    }
                }
            }
        }

        public static void ToggleKissRules()
        {
            bool value = !Woohooer.Settings.mRemoveRomanceOnKiss;

            foreach (SocialRuleRHS rule in sKissRules)
            {
                if (value)
                {
                    rule.mInteractionBitsAdded |= LongTermRelationship.InteractionBits.Romantic;
                }
                else
                {
                    rule.mInteractionBitsAdded &= ~LongTermRelationship.InteractionBits.Romantic;
                }
            }
        }

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSocialInteraction, OnSocialEvent);
        }

        public static bool TestDisable(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return false;
        }

        public static bool TestAskAboutPartnerFriendly(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (Woohooer.Settings.mInteractionsUnderRomance) return false;

                ActionData askAboutPartner = ActionData.Get("Ask About Partner");
                if ((askAboutPartner == null) || (askAboutPartner.IntendedCommodityString != CommodityTypes.Amorous)) return false;

                return SocialTest.TestAskAboutPartner(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestAskAboutPartnerAmorous(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Woohooer.Settings.mInteractionsUnderRomance) return false;

                ActionData askAboutPartner = ActionData.Get("Ask About Partner");
                if ((askAboutPartner == null) || (askAboutPartner.IntendedCommodityString != CommodityTypes.Amorous)) return false;

                if (!OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestAskAboutPartner(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestProposeToMoveInWith(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestProposeToMoveInWith(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void AfterJetPackWoohoo(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                actor.Motives.SetDecay(CommodityKind.Fun, true);
                target.Motives.SetDecay(CommodityKind.Fun, true);
                actor.Motives.ChangeValue(CommodityKind.Fun, Jetpack.kFunGainJetPackWoohoo);
                target.Motives.ChangeValue(CommodityKind.Fun, Jetpack.kFunGainJetPackWoohoo);
                CommonWoohoo.RunPostWoohoo(actor, target, actor.GetActiveJetpack(), CommonWoohoo.WoohooStyle.Safe, CommonWoohoo.WoohooLocation.Jetpack, true);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static bool TestCanVibrateBed(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestCanVibrateBed(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestAskSignFriendly(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (Woohooer.Settings.mInteractionsUnderRomance) return false;

                ActionData askSign = ActionData.Get("Ask Sign");
                if ((askSign == null) || (askSign.IntendedCommodityString != CommodityTypes.Amorous)) return false;

                return SocialTest.TestAskSign(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestAskSignAmorous(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Woohooer.Settings.mInteractionsUnderRomance) return false;

                ActionData askSign = ActionData.Get("Ask Sign");
                if ((askSign == null) || (askSign.IntendedCommodityString != CommodityTypes.Amorous)) return false;

                if (!OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestAskSign(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestAskToStayOver(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (isAutonomous)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Autonomous");
                    return false;
                }
                if (!target.IsAtHome)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not At Home");
                    return false;
                }
                if (actor.LotCurrent != target.LotCurrent)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not Same Lot");
                    return false;
                }
                if (actor.LotHome == target.LotHome)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Same Home");
                    return false;
                }
                if (!actor.IsSelectable)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("IsSelectable Fail");
                    return false;
                }
                if (!SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, VisitSituation.AskToStayOverTimeStart, VisitSituation.AskToStayOverTimeEnd))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not Time");
                    return false;
                }
                if (!actor.IsGreetedOnLot(target.LotCurrent))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not Greeted");
                    return false;
                }

                Relationship relationship = Relationship.Get(actor, target, false);
                if ((relationship != null) && (((relationship.mWhenAllowedToStayOverSimA.Ticks != 0L) && (SimClock.ElapsedTime(TimeUnit.Days, relationship.mWhenAllowedToStayOverSimA) < VisitSituation.AskToStayOverDaysPermission)) || ((relationship.mWhenAllowedToStayOverSimB.Ticks != 0L) && (SimClock.ElapsedTime(TimeUnit.Days, relationship.mWhenAllowedToStayOverSimB) < VisitSituation.AskToStayOverDaysPermission))))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Relation Fail");
                    return false;
                }

                // Age related code replaced
                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestNuzzleRomanticCheck(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestSimInWerewolfFormTalkingToSimInWerewolfForm(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestIsPlantSim(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestIsPlantSim(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestLunaticBuff(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestLunaticBuff(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestCinnamonKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!DefaultTest(actor, target, topic, isAutonomous, false, false, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestCinnamonKiss(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestHeatOfMomentKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!DefaultTest(actor, target, topic, isAutonomous, false, false, ref greyedOutTooltipCallback)) return false;

                return SocialTest.TestHeatOfMomentKiss(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnWooHoo(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            Interactions.SimWoohoo.OnAccept(actor, target, interaction, topic, i);
        }

        public static void OnTryForBaby(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            Interactions.SimTryForBaby.OnAccept(actor, target, interaction, topic, i);
        }

        public static void OnPetWoohooAccepted(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            Interactions.SimTryForBaby.OnAccept(actor, target, interaction, topic, i);
        }

        public static void OnHorseWooHoo(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            Interactions.SimTryForBaby.OnAccept(actor, target, interaction, topic, i);
        }

        protected static void OnSocialEvent(Event e)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration CommonSocials:OnSocialEvent"))
            {
                SocialEvent socialEvent = e as SocialEvent;
                if ((socialEvent != null) && (!socialEvent.WasRecipient))
                {
                    Sim actor = socialEvent.Actor as Sim;
                    Sim target = socialEvent.TargetObject as Sim;

                    if (SimTypes.IsSelectable(actor))
                    {
                        if (sRomanticSocials.ContainsKey(socialEvent.SocialName))
                        {
                            actor.IncreaseGenderPreferenceAfterAmorousStuff(target);
                        }
                    }

                    // Since the IsRomantic is disabled on all the action data, we must compensate for some coding that is missing
                    if (GetRomanticJealousy(socialEvent.SocialName, true) != JealousyLevel.None)
                    {
                        JealousyLevel level = GetRomanticJealousy(socialEvent.SocialName, false);

                        if (socialEvent.WasAccepted)
                        {
                            // Part of SocialInteractionA:Run()
                            if (actor.SimDescription.IsVampire && target.BuffManager.HasElement(BuffNames.GarlicBreath))
                            {
                                OccultVampire.GarlicEffects(actor, Origin.FromEatingGarlic);
                            }
                            if (target.SimDescription.IsVampire && socialEvent.Actor.BuffManager.HasElement(BuffNames.GarlicBreath))
                            {
                                OccultVampire.GarlicEffects(target, Origin.FromEatingGarlic);
                            }

                            Relationship relation = Relationship.Get(actor, target, false);
                            if (relation != null)
                            {
                                relation.UpdateRomanceVisibilityForPdaIfNecessary(actor, target, level);
                            }
                            GroupingSituation.EndDateIfPdaIsNotWithDatingSim(actor, target);
                        }

                        bool witnessed = false;

                        foreach (Sim witness in actor.LotCurrent.GetSims())
                        {
                            if (witness == actor) continue;

                            if (witness == target) continue;

                            if (!CaresAboutJealousy(actor, target, witness, level, false)) continue;

                            witnessed = true;

                            if (!SocialComponentEx.CheckCheating(witness, actor, target, level))
                            {
                                if (socialEvent.WasAccepted)
                                {
                                    SocialComponentEx.CheckCheating(witness, target, actor, level);
                                }
                            }
                        }

                        SendCheatingEvents(actor, target, witnessed, level, socialEvent.WasAccepted);
                    }
                }
            }
        }

        public static bool TestCanSketchTargetNude(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (actor.Posture == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Posture");
                    return false;
                }
                else if (!actor.Posture.Satisfies(CommodityKind.Standing, null) && !actor.Posture.Satisfies(CommodityKind.PostureSocializing, null))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Wrong Posture");
                    return false;
                }
                else if ((actor.Posture is CarryingChildPosture) || (target.Posture is CarryingChildPosture))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Carrying Child");
                    return false;
                }
                else if (actor.Inventory.Find<ISketchBook>() == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Sketch Book");
                    return false;
                }

                // Custom
                if (actor.SimDescription.ChildOrBelow || target.SimDescription.ChildOrBelow)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Child");
                    return false;
                }

                // Custom
                if (!Woohooer.Settings.AllowTeen(true))
                {
                    if (actor.SimDescription.Teen || target.SimDescription.Teen)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Teen");
                        return false;
                    }
                }

                if (!SocialTest.CanSimSwitchToNakedOutfitForSketching(target))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("CanSimSwitchToNakedOutfitForSketching Fail");
                    return false;
                }

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestSpicyWooHoo(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!actor.BuffManager.HasElement(BuffNames.HerbCinnamon))
                {
                    return false;
                }

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "SpicyWoohoo", isAutonomous, true, false, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestKissAndMakeUp(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                /*
                if ((!actor.SimDescription.Teen || !target.SimDescription.Teen) && (!actor.SimDescription.YoungAdultOrAbove || !target.SimDescription.YoungAdultOrAbove))
                {
                    return false;
                }
                */

                Relationship relationship = Relationship.Get(actor, target, false);
                if (relationship == null)
                {
                    return false;
                }
                if (!relationship.AreRomantic())
                {
                    return false;
                }

                if (!SocialTest.TestKissBase(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                return SocialTest.TestApologize(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestJuicedKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return TestJuiced(actor, target, topic, isAutonomous, false, ref greyedOutTooltipCallback);
        }

        private static bool TestJuiced(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, bool forWoohoo, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                bool flag2 = actor.BuffManager.HasElement(BuffNames.ABitJuiced) | actor.BuffManager.HasElement(BuffNames.SuperJuiced);
                if (!flag2) return false;

                bool flag3 = target.BuffManager.HasElement(BuffNames.ABitJuiced) | target.BuffManager.HasElement(BuffNames.SuperJuiced);
                if (!flag3) return false;

                return DefaultTest(actor, target, topic, isAutonomous, forWoohoo, false, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestJuicedLeapIntoArms(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return TestJuiced(actor, target, topic, isAutonomous, false, ref greyedOutTooltipCallback);
        }

        public static bool TestJuicedMakeOut(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return TestJuiced(actor, target, topic, isAutonomous, false, ref greyedOutTooltipCallback);
        }

        public static bool TestJuicedWooHoo(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return TestJuiced(actor, target, topic, isAutonomous, true, ref greyedOutTooltipCallback);
        }

        public static void SendCheatingEvents(Sim actor, Sim target, bool witnessed, JealousyLevel level, bool wasAccepted)
        {
            if (!witnessed) return;

            if ((actor.Partner != null) && (actor.Partner != target.SimDescription))
            {
                if (!CommonSocials.IsPolyamorous(actor.SimDescription, target.SimDescription, actor.Partner))
                {
                    DisgracefulActionEvent e = new DisgracefulActionEvent(EventTypeId.kSimCommittedDisgracefulAction, actor, DisgracefulActionType.Cheating);
                    e.TargetId = target.SimDescription.SimDescriptionId;
                    EventTracker.SendEvent(e);
                }
            }

            if (((target.Partner != null) && (target.Partner != actor.SimDescription)) && wasAccepted)
            {
                if (!CommonSocials.IsPolyamorous(target.SimDescription, actor.SimDescription, target.Partner))
                {
                    DisgracefulActionEvent event3 = new DisgracefulActionEvent(EventTypeId.kSimCommittedDisgracefulAction, target, DisgracefulActionType.Cheating);
                    event3.TargetId = actor.SimDescription.SimDescriptionId;
                    EventTracker.SendEvent(event3);
                }
            }
        }

        public static bool TestAskNPCToStayOver(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                OccultImaginaryFriend friend;
                if (isAutonomous)
                {
                    return false;
                }
                else if (!actor.IsAtHome)
                {
                    return false;
                }
                else if (actor.LotCurrent != target.LotCurrent)
                {
                    return false;
                }
                else if (actor.LotHome == target.LotHome)
                {
                    return false;
                }
                else if (!actor.IsSelectable)
                {
                    return false;
                }
                else if (!SimClock.IsTimeBetweenTimes(SimClock.HoursPassedOfDay, VisitSituation.AskToStayOverTimeStart, VisitSituation.AskToStayOverTimeEnd))
                {
                    return false;
                }
                else if (target.Service != null)
                {
                    return false;
                }
                else if (!target.IsGreetedOnLot(actor.LotCurrent))
                {
                    return false;
                }
                else if (OccultImaginaryFriend.TryGetOccultFromSim(target, out friend) && !friend.IsReal)
                {
                    return false;
                }

                Relationship relationship = Relationship.Get(actor, target, false);
                if ((relationship != null) && (((relationship.mWhenAllowedToStayOverSimA.Ticks != 0x0L) && (SimClock.ElapsedTime(TimeUnit.Days, relationship.mWhenAllowedToStayOverSimA) < VisitSituation.AskToStayOverDaysPermission)) || ((relationship.mWhenAllowedToStayOverSimB.Ticks != 0x0L) && (SimClock.ElapsedTime(TimeUnit.Days, relationship.mWhenAllowedToStayOverSimB) < VisitSituation.AskToStayOverDaysPermission))))
                {
                    return false;
                }

                SimDescription targetSim = target.SimDescription;
                SimDescription actorSim = actor.SimDescription;
                if (actorSim.Child && !targetSim.Child) //|| (actorSim.Teen && !targetSim.Teen)) || (actorSim.YoungAdultOrAbove && !targetSim.YoungAdultOrAbove))
                {
                    return false;
                }

                SlumberParty situationOfType = target.GetSituationOfType<SlumberParty>();
                if ((situationOfType != null) && situationOfType.Guests.Contains(target))
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void AdoptWildHorseSuccessProcAfter(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                //if (actor.Household.CanAddSpeciesToHousehold(target.SimDescription.Species)

                if (TwoButtonDialog.Show(Localization.LocalizeString(actor.IsFemale, "Gameplay/Actors/Sim/StrayPets:AdoptionConfirmation", new object[] { actor, target }), LocalizationHelper.Yes, LocalizationHelper.No))
                {
                    EventTracker.SendEvent(EventTypeId.kAdoptWildHorse, actor, target);
                    RidingSkill skill = actor.SimDescription.SkillManager.GetSkill<RidingSkill>(SkillNames.Riding);
                    ActiveTopic.RemoveTopicFromSim(target, "Wild Horse");
                    if (actor.IsSelectable)
                    {
                        string titleText = Localization.LocalizeString(target.IsFemale, "Gameplay/Actors/Sim/WildHorse:AdoptHorseTitle", new object[0x0]);
                        string promptText = Localization.LocalizeString(target.IsFemale, "Gameplay/Actors/Sim/WildHorse:AdoptHorseDescription", new object[0x0]);
                        target.SimDescription.FirstName = StringInputDialog.Show(titleText, promptText, target.SimDescription.FirstName, 256, StringInputDialog.Validation.SimNameText);
                        Audio.StartSound("sting_pet_adopt_wild_horse");
                    }

                    target.SimDescription.LastName = actor.SimDescription.LastName;
                    if (skill != null)
                    {
                        if (skill.IsMustangMaster())
                        {
                            AgingManager.AssignNewTrait(target.SimDescription, true, true, true);
                        }
                        skill.OnAdoptHorse(target.SimDescription);
                    }

                    PetPoolManager.RemovePet(PetPoolType.WildHorse, target.SimDescription, true);
                    actor.Household.Add(target.SimDescription);
                    PetAdoption.PetAdoptionEnd(target, actor, false);
                    Relationship.CheckAddHumanParentFlagOnAdoption(actor, target);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static void OnUnicornInviteAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                if (target.IsUnicorn) //&& actor.Household.CanAddSpeciesToHousehold(target.SimDescription.Species))
                {
                    SimDescription simDescription = target.SimDescription;
                    ActiveTopic.RemoveTopicFromSim(target, "Wild Horse");
                    PetPoolManager.RemovePet(PetPoolType.Unicorn, simDescription, true);
                    actor.Household.Add(simDescription);
                    PetAdoption.PetAdoptionEnd(target, actor, false);
                    target.ShowTNSIfSelectable(Localization.LocalizeString(target.IsFemale, "Gameplay/Actors/Sim:UnicornGreeting", new object[] { actor }), StyledNotification.NotificationStyle.kSimTalking, target.ObjectId);
                    Motive motive = target.Motives.GetMotive(CommodityKind.Energy);
                    if (motive != null)
                    {
                        motive.RestoreDecay();
                    }
                    target.BuffManager.RemoveElement(BuffNames.MoonlitUnicorn);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static void OnStrayPetAdopted(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                Household household = actor.Household;
                //if (household.CanAddSpeciesToHousehold(target.SimDescription.Species))
                {
                    if (TwoButtonDialog.Show(Localization.LocalizeString(actor.IsFemale, "Gameplay/Actors/Sim/StrayPets:AdoptionConfirmation", new object[] { actor, target }), LocalizationHelper.Yes, LocalizationHelper.No))
                    {
                        if (actor.IsSelectable)
                        {
                            string titleText = Localization.LocalizeString(target.IsFemale, "Gameplay/Actors/Sim/StrayPets:AdoptStrayNameTitle", new object[0x0]);
                            string promptText = Localization.LocalizeString(target.IsFemale, "Gameplay/Actors/Sim/StrayPets:AdoptStrayNameDescription", new object[0x0]);
                            target.SimDescription.FirstName = StringInputDialog.Show(titleText, promptText, target.SimDescription.FirstName, 256, StringInputDialog.Validation.SimNameText);
                        }

                        target.SimDescription.LastName = actor.SimDescription.LastName;
                        actor.ShowTNSAndPlayStingIfSelectable(Localization.LocalizeString("Gameplay/Actors/Sim/StrayPets:AdoptionAcceptedTNS", new object[] { target, actor.Household.Name }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, target.ObjectId, "sting_pet_adopt");
                        target.BuffManager.RemoveElement(BuffNames.StrayPet);

                        Relationships.CheckAddHumanParentFlagOnAdoption(actor.SimDescription, target.SimDescription);

                        if (actor.Partner != null)
                        {
                            Relationships.CheckAddHumanParentFlagOnAdoption(actor.Partner, target.SimDescription);
                        }

                        PetAdoption.PetAdoptionEnd(target, actor, true);
                        PetPoolManager.RemovePet(target.IsCat ? PetPoolType.StrayCat : PetPoolType.StrayDog, target.SimDescription, true);
                        household.AddSim(target);
                    }
                    else
                    {
                        actor.ShowTNSIfSelectable(Localization.LocalizeString("Gameplay/Actors/Sim/StrayPets:AdoptionDeclinedTNS", new object[] { target, actor.Household.Name }), StyledNotification.NotificationStyle.kGameMessageNegative, ObjectGuid.InvalidObjectGuid, target.ObjectId);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static bool AdoptStrayTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                Relationship relationship = Relationship.Get(actor, target, false);
                if ((relationship == null) || !relationship.AreFriends())
                {
                    return false;
                }

                return target.IsStray; // && actor.Household.CanAddSpeciesToHousehold(target.SimDescription.Species));
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool AdoptWildHorseProceduralTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!WildHorses.IsWildHorse(target))
                {
                    return false;
                }
                if (!RidingSkill.CanAdoptWildHorse(actor.SimDescription))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, "Gameplay/Actors/Sim/AdoptWildHorse:GreyedOutTooltipRidingSkillTooLow", new object[] { actor, target }));
                    return false;
                }

                /*
                if (!actor.Household.CanAddSpeciesToHousehold(target.SimDescription.Species))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, "Gameplay/Actors/Sim/AdoptWildHorse:GreyedOutTooltipTooManyFamilyMembers", new object[] { actor, target }));
                    return false;
                }
                */
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool UnicornInviteTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!target.IsUnicorn)
                {
                    return false;
                }
                if (!OccultUnicorn.IsNPCPoolUnicorn(target))
                {
                    return false;
                }
                Relationship relationship = actor.GetRelationship(target, false);
                if ((relationship == null) || (relationship.CurrentLTRLiking < PetSocialTunables.kUnicornMinLtrForInvite))
                {
                    return false;
                }

                /*
                if (!actor.Household.CanAddSpeciesToHousehold(target.SimDescription.Species))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Ui/Caption/MovingDialog:TooManyPets", new object[0x0]));
                    return false;
                }
                */
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool CheckAutonomousGenderPreference(SimDescription a, SimDescription b)
        {
            if (a.CanAutonomouslyBeRomanticWithGender(b.Gender) && b.CanAutonomouslyBeRomanticWithGender(a.Gender))
            {
                return true;
            }
            else if (a.NotOpposedToRomanceWithGender(b.Gender) && b.NotOpposedToRomanceWithGender(a.Gender))
            {
                return true;
            }

            return false;
        }

        private static bool IsValidDateOrGroup(Sim actor, Sim target, bool isDate)
        {
            if (isDate && !GroupingSituationEx.CanGoOnDate(actor.SimDescription, target.SimDescription))
            {
                return false;
            }
            return true;
        }

        public static bool TestLetsStartGroup(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback, bool isDate)
        {
            try
            {
                OccultImaginaryFriend friend;
                Service service = target.Service;
                if (((target.Service is GrimReaper) || (service is Butler)) || (target.IsNPC && target.SimDescription.IsGhost))
                {
                    return false;
                }
                else if (OccultImaginaryFriend.TryGetOccultFromSim(target, out friend) && !friend.IsReal)
                {
                    return false;
                }
                else if (Punishment.IsSimGrounded(actor) || Punishment.IsSimGrounded(target))
                {
                    return false;
                }

                GroupingSituation situationOfType = actor.GetSituationOfType<GroupingSituation>();
                if (situationOfType != null)
                {
                    if (situationOfType.IsSimInGroup(target))
                    {
                        return false;
                    }
                    else if (actor.HasTrait(TraitNames.Loner) && (situationOfType.Count >= GroupingSituation.kNumSimBeforeLonerQuits))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(GroupingSituation.LocalizeString("FormDisabledForLoner", new object[0x0]));
                        return false;
                    }
                    else if (situationOfType.RomanticGrouping)
                    {
                        return false;
                    }
                    else if (isDate)
                    {
                        return false;
                    }
                    // Custom IsValidDateOrGroup
                    else if (!IsValidDateOrGroup(actor, target, isDate))
                    {
                        return false;
                    }

                    return !situationOfType.MaxGroupSizedReached();
                }

                if (actor.Household == target.Household)
                {
                    situationOfType = target.GetSituationOfType<GroupingSituation>();
                    if (situationOfType != null)
                    {
                        // Custom IsValidDateOrGroup
                        return (((!situationOfType.RomanticGrouping && !isDate) && IsValidDateOrGroup(actor, target, isDate)) && !situationOfType.MaxGroupSizedReached());
                    }
                }

                Sim sim = null;
                foreach (Sim sim2 in actor.Household.Sims)
                {
                    if (sim2 != actor)
                    {
                        situationOfType = sim2.GetSituationOfType<GroupingSituation>();
                        if ((situationOfType != null) && (situationOfType.Leader == sim2))
                        {
                            sim = sim2;
                            break;
                        }
                    }
                }

                // Custom IsValidDateOrGroup
                if (!IsValidDateOrGroup(actor, target, isDate))
                {
                    return false;
                }
                else if (sim != null)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, "Gameplay/Excel/Notifications/Notifications:GroupingAlreadyStarted", new object[] { sim }));
                    return false;
                }

                TraitManager traitManager = actor.TraitManager;
                if ((isDate && (traitManager != null)) && !traitManager.HasElement(TraitNames.HopelessRomantic))
                {
                    Relationship relationship = Relationship.Get(actor, target, false);
                    if ((relationship != null) && (SimClock.ElapsedTime(TimeUnit.Hours, relationship.WhenLastHadBadDate) < GroupingSituation.kBadDateCooldownTime))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(GroupingSituation.LocalizeString("BadDateCooldownTooltip", new object[0x0]));
                        return false;
                    }
                }
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestLetsGoOnDate(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return TestLetsStartGroup(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback, true);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool CanGetRomantic(Sim me, Sim target, bool autonomous, bool tryingToWooHoo, bool testLiking, ref GreyedOutTooltipCallback greyedOutTooltipCallback, out string reason)
        {
            return CanGetRomantic(me.SimDescription, target.SimDescription, autonomous, tryingToWooHoo, testLiking, ref greyedOutTooltipCallback, out reason);
        }
        public static bool CanGetRomantic(SimDescription me, SimDescription target, out string reason)
        {
            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            return CanGetRomantic(me, target, true, true, true, ref greyedOutTooltipCallback, out reason);
        }
        public static bool CanGetRomantic(SimDescription me, SimDescription target, bool autonomous, bool tryingToWooHoo, bool testLiking, ref GreyedOutTooltipCallback greyedOutTooltipCallback, out string reason)
        {
            if (target == me)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Actor is Target");
                reason = "Actor is Target";
                return false;
            }

            if (!SimTypes.IsEquivalentSpecies(me, target))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Improper Species");
                reason = "Improper Species";
                return false;
            }

            if (me.Genealogy == null) 
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Actor Genealogy Fail");
                reason = "Actor Genealogy Fail";
                return false;
            }
            
            if (target.Genealogy == null)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Target Genealogy Fail");
                reason = "Target Genealogy Fail";
                return false;
            }

            if (me.OccultManager.HasOccultType(OccultTypes.TimeTraveler) || target.OccultManager.HasOccultType(OccultTypes.TimeTraveler))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Time Traveler Fail");
                reason = "Time Traveler Fail";
                return false;
            }

            if (tryingToWooHoo && HolographicProjectionSituation.IsSimHolographicallyProjected(target.CreatedSim))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Target Hologram Fail");
                reason = "Hologram Fail";
                return false;
            }

            if ((me.IsEP11Bot) && (!me.TraitManager.HasElement(TraitNames.CapacityToLoveChip)))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Capacity To Love Fail");
                reason = "Capacity To Love Fail";
                return false;
            }

            if ((target.IsEP11Bot) && (!target.TraitManager.HasElement(TraitNames.CapacityToLoveChip)))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Capacity To Love Fail");
                reason = "Capacity To Love Fail";
                return false;
            }

            if ((sStoryProgressionCanRomanticInteract.Valid) && (Woohooer.Settings.TestStoryProgression(autonomous)))
            {
                reason = sStoryProgressionCanRomanticInteract.Invoke<string>(new object[] { me, target, autonomous });
                if (reason != null)
                {
                    greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, true);
                    return false;
                }
            }

            if ((autonomous) || (NRaas.Woohooer.Settings.mGenderPreferenceForUserDirectedV2))
            {
                if (!CheckAutonomousGenderPreference(me, target))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Gender Preference Fail");
                    reason = "Gender Preference Fail";
                    return false;
                }
            }

            if ((sStoryProgressionAllowRomance.Valid) && (Woohooer.Settings.TestStoryProgression(autonomous)))
            {
                reason = sStoryProgressionAllowRomance.Invoke<string>(new object[] { me, autonomous });
                if (reason != null)
                {
                    greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, true);
                    return false;
                }

                reason = sStoryProgressionAllowRomance.Invoke<string>(new object[] { target, autonomous });
                if (reason != null)
                {
                    greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, true);
                    return false;
                }
            }

            if (autonomous)
            {
                if (tryingToWooHoo && (me.IsRobot != target.IsRobot))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Cross Species Robot Fail");
                    reason = "Cross Species Robot Fail";
                    return false;
                }

                if (Woohooer.Settings.mDisallowHomeless)
                {
                    if (me.LotHome == null)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Actor Homeless Fail");
                        reason = "Actor Homeless Fail";
                        return false;
                    }

                    if (target.LotHome == null)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Target Homeless Fail");
                        reason = "Target Homeless Fail";
                        return false;
                    }
                }

                if (me.AssignedRole != null)
                {
                    if (Woohooer.Settings.mDisallowAutonomousRoleTypes.Contains(me.AssignedRole.Type))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Actor Role Fail");
                        reason = "Actor Role Fail";
                        return false;
                    }
                }

                if (target.AssignedRole != null)
                {
                    if (Woohooer.Settings.mDisallowAutonomousRoleTypes.Contains(target.AssignedRole.Type))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Target Role Fail");
                        reason = "Target Role Fail";
                        return false;
                    }
                }

                if (SimTypes.InServicePool(me))
                {
                    if (Woohooer.Settings.mDisallowAutonomousServiceTypes.Contains(me.CreatedByService.ServiceType))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Actor Service Fail");
                        reason = "Actor Service Fail";
                        return false;
                    }
                }

                if (SimTypes.InServicePool(target))
                {
                    if (Woohooer.Settings.mDisallowAutonomousServiceTypes.Contains(target.CreatedByService.ServiceType))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Target Service Fail");
                        reason = "Target Service Fail";
                        return false;
                    }
                }

                if ((me.CreatedSim != null) && (target.CreatedSim != null))
                {
                    BuffBetrayed.BuffInstanceBetrayed betrayed;
                    if (BuffBetrayed.DoesSimFeelBetrayed(me.CreatedSim, target, out betrayed))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Betrayed Fail");
                        reason = "Betrayed Fail";
                        return false;
                    }
                    if (!OccultImaginaryFriend.CanSimGetRomanticWithSim(me.CreatedSim, target.CreatedSim))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Imaginary Fail");
                        reason = "Imaginary Fail";
                        return false;
                    }
                }
            }

            if ((tryingToWooHoo) && (autonomous))
            {
                if (Woohooer.Settings.mMustBeRomanticForAutonomousV2[PersistedSettings.GetSpeciesIndex(me)])
                {
                    if ((!KamaSimtra.IsWhoring(me)) && (!KamaSimtra.IsWhoring(target)))
                    {
                        Relationship relation = me.GetRelationship(target, false);
                        if ((relation == null) || (!relation.AreRomantic()))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Must Be Romantic Fail");
                            reason = "Must Be Romantic Fail";
                            return false;
                        }
                    }
                }
            }

            if (testLiking)
            {
                if ((autonomous) || (Woohooer.Settings.mLikingGateForUserDirected))
                {
                    if (!SatisfiesLikingGate(me, target, tryingToWooHoo))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Liking Gate Fail (2)");
                        reason = "Liking Gate Fail (2)";
                        return false;
                    }
                }
            }

            if ((me.ChildOrBelow) || (target.ChildOrBelow))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Age Fail");
                reason = "Age Fail";
                return false;
            }

            if (!Woohooer.Settings.AllowNearRelation(me.Species, tryingToWooHoo, autonomous))
            {
                if (Relationships.IsCloselyRelated(me, target, false))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Relation Fail");
                    reason = "Relation Fail";
                    return false;
                }
            }

            if ((me.Teen) || (target.Teen))
            {
                if ((!Woohooer.Settings.AllowTeen(tryingToWooHoo)) && (!Woohooer.Settings.AllowTeenAdult(tryingToWooHoo)))
                {
                    greyedOutTooltipCallback = delegate
                    {
                        return Common.LocalizeEAString("NRaas.Woohooer:Teenagers");
                    };
                    reason = "Teenager Fail";
                    return false;
                }

                if ((!me.Teen) || (!target.Teen))
                {
                    if (!Woohooer.Settings.AllowTeenAdult(tryingToWooHoo))
                    {
                        greyedOutTooltipCallback = delegate
                        {
                            return Common.LocalizeEAString("NRaas.Woohooer:Teenagers");
                        };
                        reason = "Teenager Fail (2)";
                        return false;
                    }
                }
            }


            if ((sStoryProgressionAllowAffair.Valid) && (Woohooer.Settings.TestStoryProgression(autonomous)))
            {
                reason = sStoryProgressionAllowAffair.Invoke<string>(new object[] { me, target, autonomous });
                if (reason != null)
                {
                    greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, true);
                    return false;
                }
            }

            reason = null;
            return true;
        }

        public static bool SatisfiesLikingGate(Sim actor, Sim target, bool woohoo)
        {
            return SatisfiesLikingGate(actor.SimDescription, target.SimDescription, woohoo);
        }
        public static bool SatisfiesLikingGate(SimDescription actor, SimDescription target, bool woohoo)
        {
            if ((KamaSimtra.IsWhoring(actor)) || (KamaSimtra.IsWhoring(target)))
            {
                return true;
            }

            int liking = 0;

            Relationship relation = Relationship.Get(actor, target, false);
            if (relation != null)
            {
                liking = (int)relation.CurrentLTRLiking;
            }

            liking += (int)KamaSimtra.GetSkillLevel(actor) * 5;

            if (woohoo)
            {
                return (liking >= Woohooer.Settings.mLikingGatingForAutonomousWoohoo[PersistedSettings.GetSpeciesIndex(actor)]);
            }
            else
            {
                return (liking >= Woohooer.Settings.mLikingGatingForAutonomousRomance[PersistedSettings.GetSpeciesIndex(actor)]);
            }
        }

        public static bool SatisfiesRomance(Sim actor, Sim target, string logName, bool isAutonomous, ref GreyedOutTooltipCallback callback)
        {
            string reason;
            if (!CanGetRomantic(actor, target, isAutonomous, false, true, ref callback, out reason))
            {
                ScoringLookup.IncStat(logName + reason);
                return false;
            }

            WoohooScoring.ScoreTestResult result = WoohooScoring.ScoreActor(logName, actor, target, isAutonomous, "InterestInRomance", false);
            if (result != WoohooScoring.ScoreTestResult.Success)
            {
                ScoringLookup.IncStat(logName + result);
                return false;
            }

            result = WoohooScoring.ScoreTarget(logName, target, actor, isAutonomous, "InterestInRomance", false);
            if (result != WoohooScoring.ScoreTestResult.Success)
            {
                ScoringLookup.IncStat(logName + result);
                return false;
            }

            ScoringLookup.IncStat(logName + "Success");
            return true;
        }

        public static bool AlterSTC(SimDescription parameter, SimDescription active)
        {
            try
            {
                if ((active == null) || (parameter == null)) return true;

                if ((active.Teen != parameter.Teen) || (Relationships.IsCloselyRelated(active, parameter, false)))
                {
                    Relationship relation = Relationship.Get(active, parameter, false);
                    if ((relation != null) && (relation.STC != null))
                    {
                        STCData data = STCData.Get(relation.CurrentSTC);
                        if ((data != null) && (data.Commodity == CommodityTypes.Amorous))
                        {
                            //relation.STC.SetSTC(AmorousCommodity.sAmorous2, active, relation.STC.GetSTC(CommodityTypes.Amorous, active));
                            //relation.STC.SetSTC(AmorousCommodity.sAmorous2, parameter, relation.STC.GetSTC(CommodityTypes.Amorous, parameter));

                            relation.STC.mCurrentCommodityType = AmorousCommodity.sAmorous2;
                        }
                    }
                }

                try
                {
                    return ActiveTopicTestFunctions.IsCelebrityOnCurrentLot(parameter, active);
                }
                catch
                { }

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(parameter, active, e);
                return false;
            }
        } 

        public static bool OnEAPetWoohooTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Woohooer.Settings.mEAStandardWoohoo)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("EA Ruleset Fail");
                    return false;
                }

                return SimTryForBaby.PublicTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnEAHorseWoohooTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Woohooer.Settings.mEAStandardWoohoo)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("EA Ruleset Fail");
                    return false;
                }

                return SimTryForBaby.PublicTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnEAWoohooTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Woohooer.Settings.mEAStandardWoohoo)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("EA Ruleset Fail");
                    return false;
                }

                return SimWoohoo.PublicTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnEATryForBabyTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Woohooer.Settings.mEAStandardWoohoo)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("EA Ruleset Fail");
                    return false;
                }

                return SimTryForBaby.PublicTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnDenyTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return false;
        }

        public static bool TestProposeGoingSteady(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if ((sStoryProgressionAllowSteady.Valid) && (Woohooer.Settings.TestStoryProgression(isAutonomous)))
                {
                    string reason = sStoryProgressionAllowSteady.Invoke<string>(new object[] { actor.SimDescription, target.SimDescription, isAutonomous });
                    if (reason != null)
                    {
                        greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, true);
                        return false;
                    }
                }
                
                if (!PartneringInteractionLevelSetting.Satisfies(actor, target))
                {
                    Relationship relationship = Relationship.Get(actor, target, false);
                    if (relationship == null)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Relationship");
                        return false;
                    }

                    if ((relationship.CurrentSTC != ShortTermContextTypes.Hot) && (relationship.CurrentSTC != AmorousCommodity.sHot2))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("STC Fail " + relationship.CurrentSTC);
                        return false;
                    }
                }

                if (actor.Partner == target.SimDescription)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Already Partnered");
                    return false;
                }
                else if ((actor.Partner != null) && (actor.Partner != target.SimDescription))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Actor already partnered");
                    return false;
                }
                else if ((target.Partner != null) && (target.Partner != actor.SimDescription))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Target already partnered");
                    return false;
                }

                MiniSimDescription description = MiniSimDescription.Find(actor.SimDescription.SimDescriptionId);
                if ((description != null) && (description.HasPartner || description.IsMarried))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Actor MiniSim Partnered or Married");
                    return false;
                }

                description = MiniSimDescription.Find(target.SimDescription.SimDescriptionId);
                if ((description != null) && (description.HasPartner || description.IsMarried))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Target MiniSim Partnered or Married");
                    return false;
                }

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestProposeMarriage(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                /*
                if (isAutonomous)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not User Directed");
                    return false;
                }
                */

                if (!actor.IsRobot && target.IsRobot)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Robot Cross-species");
                    return false;
                }

                Relationship relationship = Relationship.Get(actor, target, false);
                if (relationship == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Relationship");
                    return false;
                }

                if (!PartneringInteractionLevelSetting.Satisfies(actor, target))
                {
                    if ((relationship.CurrentSTC != ShortTermContextTypes.Hot) && (relationship.CurrentSTC != AmorousCommodity.sHot2))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("STC Fail " + relationship.CurrentSTC);
                        return false;
                    }
                }

                if (Woohooer.Settings.mRestrictTeenMarriage)
                {
                    bool success = false;

                    if ((actor.SimDescription.Teen) || (target.SimDescription.Teen))
                    {
                        if ((actor.SimDescription.Pregnancy != null) && (actor.SimDescription.Pregnancy.DadDescriptionId == target.SimDescription.SimDescriptionId))
                        {
                            success = true;
                        }
                        else if ((target.SimDescription.Pregnancy != null) && (target.SimDescription.Pregnancy.DadDescriptionId == actor.SimDescription.SimDescriptionId))
                        {
                            success = true;
                        }
                        else
                        {
                            foreach (SimDescription child in Relationships.GetChildren(actor.SimDescription))
                            {
                                if (Relationships.GetParents(child).Contains(target.SimDescription))
                                {
                                    success = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        success = true;
                    }

                    if (!success)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Teen Denied");
                        return false;
                    }
                }

                if (!actor.TraitManager.HasElement(TraitNames.Inappropriate) && (relationship.LTR.Liking < SocialComponent.LikingThresholdForMarriageAvailability))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Undergate " + relationship.LTR.Liking + " " + SocialComponent.LikingThresholdForMarriageAvailability);
                    return false;
                }

                if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Propose))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Already Proposed");
                    return false;
                }

                if (!CanGetMarriedNow(actor, target, isAutonomous, false, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool CanGetMarriedNow(Sim actor, Sim target, bool autonomous, bool requiresParty, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            // The rest of the code in this function is in regards to "greater than eight"

            return CanGetMarried(actor, target, autonomous, requiresParty, ref greyedOutTooltipCallback);
        }

        public static bool CanGetMarried(Sim actor, Sim target, bool autonomous, bool requiresParty, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (GameUtils.IsUniversityWorld())
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Not University");
                return false;
            }

            if (HolographicProjectionSituation.IsSimHolographicallyProjected(target) || HolographicProjectionSituation.IsSimHolographicallyProjected(actor))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Hologram");
                return false;
            }

            if ((actor.Partner != null) && (actor.Partner != target.SimDescription))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Not Actor Partner");
                return false;
            }

            if ((target.Partner != null) && (target.Partner != actor.SimDescription))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Not Target Partner");
                return false;
            }

            if (actor.IsMarried)            
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Is Actor Married");
                return false;                
            }
            
            if (target.IsMarried)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Is Target Married");
                return false;
            }

            if (actor.SimDescription.HomeWorld != GameUtils.GetCurrentWorld())
            {
                MiniSimDescription description = MiniSimDescription.Find(actor.SimDescription.SimDescriptionId);
                if ((description != null) && description.IsMarried)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Is Foreign Actor Married");
                    return false;
                }
            }

            if (target.SimDescription.HomeWorld != GameUtils.GetCurrentWorld())
            {
                MiniSimDescription description = MiniSimDescription.Find(target.SimDescription.SimDescriptionId);
                if ((description != null) && description.IsMarried)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Is Foreign Target Married");
                    return false;
                }
            }

            if (!actor.SimDescription.Marryable)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Actor Not Marryable");
                return false;                
            }
            
            if (!target.SimDescription.Marryable)
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Target Not Marryable");
                return false;
            }

            if (target.SimDescription != actor.Partner)
            {
                if (target.SimDescription.HomeWorld == GameUtils.GetCurrentWorld())
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Target Not Partner Local");
                    return false;
                }

                MiniSimDescription description = MiniSimDescription.Find(target.SimDescription.SimDescriptionId);
                if ((description != null) && (description.PartnerId != actor.SimDescription.SimDescriptionId))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Target Not Partner Foreign");
                    return false;
                }
            }

            if ((requiresParty && !WeddingParty.IsInvolvedInParty(actor)) && !WeddingParty.IsInvolvedInParty(target))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Party Required");
                return false;
            }

            /*
            if (actor.Household != target.Household)
            {
                if (Households.NumHumansIncludingPregnancy(actor.Household) >= 0x8)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Too Many Sims");
                    return false;
                }
                if ((target.SimDescription.Pregnancy != null) && (Households.NumHumansIncludingPregnancy(actor.Household) >= 0x7))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Too Many Sims Pregnancy");
                    return false;
                }
            }
            */

            if ((actor.SimDescription.IsGhost) && (!actor.SimDescription.IsPlayableGhost))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Actor Dead");
                return false;                
            }

            if ((target.SimDescription.IsGhost) && (!target.SimDescription.IsPlayableGhost))
            {
                greyedOutTooltipCallback = Common.DebugTooltip("Target Dead");
                return false;
            }

            if ((sStoryProgressionAllowMarriage.Valid) && (Woohooer.Settings.TestStoryProgression(autonomous)))
            {
                string reason = sStoryProgressionAllowMarriage.Invoke<string>(new object[] { actor.SimDescription, target.SimDescription, autonomous });
                if (reason != null)
                {
                    greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, true);
                    return false;
                }
            }

            return true;
        }

        private static bool CommonCheatingTest(Sim betrayer, Sim victim)
        {
            BuffBetrayed.BuffInstanceBetrayed betrayed;
            Relationship relationship = Relationship.Get(betrayer, victim, false);
            if (relationship == null) return false;

            if (!relationship.AreRomantic()) return false;

            if (BuffBetrayed.DoesSimFeelBetrayed(victim, betrayer.SimDescription, out betrayed)) return false;

            if (!WoohooScoring.ReactsToJealousy(victim)) return false;

            if (betrayer.HasTrait(TraitNames.NoJealousy)) return false;

            if (betrayer.HasTrait(TraitNames.AboveReproach)) return false;

            if (RomanceVisibilityState.GetActiveRomanceCount(betrayer.SimDescription) <= 1) return false;

            return true;
        }

        public static bool TestAllowBreakup(Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if ((sStoryProgressionAllowBreakup.Valid) && (Woohooer.Settings.TestStoryProgression(isAutonomous)))
            {
                string reason = sStoryProgressionAllowBreakup.Invoke<string>(new object[] { target.SimDescription, isAutonomous });
                if (reason != null)
                {
                    greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, true);
                    return false;
                }
            }

            return true;
        }

        public static bool OnTestBreakup(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!TestAllowBreakup(target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestElectrifiedKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestElectrifiedKiss(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestJetPackWooHoo(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!Jetpack.JetpackTestCommon(actor, target))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("TestCommon Denied");
                    return false;
                }

                if (actor.SkillManager.GetSkillLevel(SkillNames.Future) < SocialTest.kFutureSkillRequiredJetpackWoohoo)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Skill Denied");
                    return false;
                }

                SocialJig socialjig = null;
                if (!Jetpack.CheckSpaceForFlyAroundJig(actor, target, ref socialjig, true, true))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/Objects/EP11/Jetpack:NotEnoughSpace", new object[] { target }));
                    return false;
                }

                if (!Woohooer.Settings.mAutonomousJetPack)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Location Denied");
                    return false;
                }

                return CommonWoohoo.SatisfiesWoohoo(actor, target, "JetPackWooHoo", isAutonomous, true, false, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestJetPackSlowDance(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestJetPackSlowDance(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestJetPackKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestJetPackKiss(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestCanComplimentHardware(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestCanComplimentHardware(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestCanUploadFeelings(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestCanUploadFeelings(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestSparkKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestSparkKiss(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestDipKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestDipKiss(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestAccuseOfCheating(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!TestAllowBreakup(target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return CommonCheatingTest(target, actor);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestGetMarried(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return CanGetMarriedNow(actor, target, isAutonomous, true, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestGetMarriedUsingArch(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                return CanGetMarriedNow(actor, target, isAutonomous, false, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestHavePrivateWedding(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!CanGetMarriedNow(actor, target, isAutonomous, false, ref greyedOutTooltipCallback))
                {
                    return false;
                }
                
                if (Party.IsHostAtAParty(actor))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Actor At Party");
                    return false;                    
                }
                
                if (Party.IsHostAtAParty(target))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Target At Party");
                    return false;
                }

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestAskToProm(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!PromSituation.IsGoingToProm(actor))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not IsGoingToProm");
                    return false;
                }

                if (PromSituation.HasPromDate(actor))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Actor HasPromDate");
                    return false;                    
                }
                
                if (PromSituation.HasPromDate(target))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Target HasPromDate");
                    return false;
                }

                // PromSituation has blocks against non-Teen sims

                if (!actor.SimDescription.Teen)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Actor Not Teen");
                    return false;
                }

                if (!target.SimDescription.Teen)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Target Not Teen");
                    return false;
                }

                string reason;
                return CanGetRomantic(actor.SimDescription, target.SimDescription, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestFirstKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestFirstKiss(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestKiss(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestKiss(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
       }

        public static bool TestKissOnCheek(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestKissOnCheek(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool TestMakeOut(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestMakeOut(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnTestMassage(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!WoohooInteractionLevelSetting.Satisfies(actor, target, false))
                {
                    Relationship relationship = Relationship.Get(actor, target, false);
                    if (relationship == null)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Relationship");
                        return false;
                    }

                    if ((relationship.CurrentSTC != ShortTermContextTypes.Seductive) && (relationship.CurrentSTC != AmorousCommodity.sSeductive2) && 
                        (relationship.CurrentSTC != ShortTermContextTypes.Hot) && (relationship.CurrentSTC != AmorousCommodity.sHot2))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("STC Fail " + relationship.CurrentSTC);
                        return false;
                    }
                }

                return OnDefaultTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnDefaultNonRomanticTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration OnDefaultNonRomanticTest", Common.DebugLevel.Stats))
                {
                    string autoString = null;
                    if (isAutonomous)
                    {
                        autoString = "Autonomous ";
                    }

                    ScoringLookup.IncStat(autoString + "OnDefaultNonRomanticTest Try");

                    if ((sStoryProgressionCanFriendInteract.Valid) && (Woohooer.Settings.TestStoryProgression(isAutonomous)))
                    {
                        string reason = sStoryProgressionCanFriendInteract.Invoke<string>(new object[] { actor.SimDescription, target.SimDescription, isAutonomous });
                        if (reason != null)
                        {
                            greyedOutTooltipCallback = Woohooer.StoryProgressionTooltip(reason, true);

                            ScoringLookup.IncStat(autoString + "OnDefaultNonRomanticTest StoryProgression");
                            return false;
                        }
                    }

                    if (isAutonomous)
                    {
                        Relationship relation = Relationship.Get(actor, target, false);
                        if (relation != null)
                        {
                            try
                            {
                                if (relation.STC.IsRomantic)
                                {
                                    greyedOutTooltipCallback = Common.DebugTooltip("Romantic STC");

                                    ScoringLookup.IncStat(autoString + "OnDefaultNonRomanticTest RomanticSTC");
                                    return false;
                                }
                            }
                            catch (Exception e)
                            {
                                Common.DebugException(actor, target, e);

                                relation.STC.mCurrentStc = ShortTermContextTypes.Friendly;
                                return true;
                            }
                        }
                    }

                    ScoringLookup.IncStat(autoString + "OnDefaultNonRomanticTest Success");
                    //ScoringLookup.IncStat(autoString + "OnDefaultNonRomanticTest Success " + actor.FullName + " " + target.FullName);
                    return true;
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool OnDefaultTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return DefaultTest(actor, target, topic, isAutonomous, false, true, ref greyedOutTooltipCallback);
        }
        public static bool DefaultTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, bool forWoohoo, bool testLiking, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration DefaultTest", Common.DebugLevel.Stats))
                {
                    string autoString = null;
                    if (isAutonomous)
                    {
                        autoString = "Autonomous ";
                    }

                    ScoringLookup.IncStat(autoString + "OnDefaultTest Try");

                    if (!SatisfiedInteractionLevel(actor, target, isAutonomous, ref greyedOutTooltipCallback))
                    {
                        ScoringLookup.IncStat(autoString + "OnDefaultTest SatisfiedInteractionLevel");
                        return false;
                    }

                    string reason = null;
                    if (!CanGetRomantic(actor, target, isAutonomous, forWoohoo, testLiking, ref greyedOutTooltipCallback, out reason))
                    {
                        ScoringLookup.IncStat(autoString + "OnDefaultTest " + reason);
                        return false;
                    }

                    if ((isAutonomous) && (!Woohooer.Settings.mAllowAutonomousRomanceCommLot))
                    {
                        if ((actor.IsSelectable) || (target.IsSelectable))
                        {
                            if ((actor.LotCurrent == null) || (actor.LotCurrent.IsCommunityLot))
                            {
                                greyedOutTooltipCallback = Common.DebugTooltip("OnDefaultTest CommunityLot");

                                ScoringLookup.IncStat(autoString + "OnDefaultTest CommunityLot");
                                return false;
                            }
                        }
                    }

                    if (Woohooer.Settings.UsingTraitScoring)
                    {
                        if ((isAutonomous) || (Woohooer.Settings.TraitScoringForUserDirected))
                        {
                            if (!WoohooScoring.IsSafeFromJealousy(actor, target, false))
                            {
                                ScoringLookup.IncStat(autoString + "OnDefaultTest Jealousy");

                                greyedOutTooltipCallback = Common.DebugTooltip("OnDefaultTest Jealousy");
                                return false;
                            }

                            WoohooScoring.ScoreTestResult result = WoohooScoring.ScoreActor("DefaultTest", actor, target, isAutonomous, "InterestInRomance", false);
                            if (result != WoohooScoring.ScoreTestResult.Success)
                            {
                                ScoringLookup.IncStat(autoString + "OnDefaultTest " + result);

                                greyedOutTooltipCallback = Common.DebugTooltip("OnDefaultTest " + result);
                                return false;
                            }
                        }
                    }

                    ScoringLookup.IncStat(autoString + "OnDefaultTest Success");
                    //ScoringLookup.IncStat(autoString + "OnDefaultTest Success " + actor.FullName + " " + target.FullName);

                    if ((Common.kDebugging) && (Woohooer.Settings.mVerboseDebugging))
                    {
                        if (isAutonomous)
                        {
                            Common.DebugNotify("OnDefaultTest Success" + Common.NewLine + actor.FullName + Common.NewLine + target.FullName, actor, target);
                        }
                    }
                    return true;
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnBeFriskyAccept(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                actor.Motives.SetValue(CommodityKind.Fun, actor.Motives.GetValue(CommodityKind.Fun) + SocialCallback.kBeFriskyFunGain);
                target.Motives.SetValue(CommodityKind.Fun, target.Motives.GetValue(CommodityKind.Fun) + SocialCallback.kBeFriskyFunGain);

                SimWoohoo.OnAccept(actor, target, interaction, topic, i);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static void OnSpicyWooHoo(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                target.BuffManager.AddElement(BuffNames.HerbCinnamon, Origin.FromSocialization);

                SimWoohoo.OnAccept(actor, target, interaction, topic, i);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static bool BeFriskyProceduralTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                Relationship relation = Relationship.Get(actor, target, false);
                if (relation == null)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("No Relation");
                    return false;
                }

                if (!relation.AreRomantic())
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not Romantic");
                    return false;
                }

                switch (Woohooer.Settings.mMyLoveBuffLevel)
                {
                    case Options.Romance.MyLoveBuffLevel.Partner:
                        if (actor.Partner != target.SimDescription)
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Level Fail");
                            return false;
                        }
                        break;
                    case Options.Romance.MyLoveBuffLevel.Default:
                    case Options.Romance.MyLoveBuffLevel.Spouse:
                        if (actor.Partner != target.SimDescription)
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Level Fail");
                            return false;
                        }

                        if (!actor.IsMarried)
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Level Fail");
                            return false;
                        }

                        break;
                }

                if (Woohooer.Settings.mMyLoveBuffLevel == Options.Romance.MyLoveBuffLevel.Default)
                {
                    if ((!actor.BuffManager.HasElement(BuffNames.JustMarried)) || (!target.BuffManager.HasElement(BuffNames.JustMarried)))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("No Moodlet");
                        return false;
                    }
                }

                return SimWoohoo.OnTest(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool CanWatchTheStars(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                string reason;
                if (!CanGetRomantic(actor.SimDescription, target.SimDescription, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason))
                {
                    return false;
                }

                // A result of "True" means failure in OnTestAttraction
                if (OnTestAttraction(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback)) return false;

                if (!SimClock.IsNightTime())
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Not IsNighttime");
                    return false;
                }

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool SatisfiedInteractionLevel(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return SatisfiedInteractionLevel(actor.SimDescription, target.SimDescription, isAutonomous, ref greyedOutTooltipCallback);
        }
        public static bool SatisfiedInteractionLevel(SimDescription actor, SimDescription target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (isAutonomous)
            {
                if (actor.Partner != null)
                {
                    if (actor.IsMarried)
                    {
                        if (!MarriedInteractionLevelSetting.Satisfies(actor, target, true))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Married Actor Interaction Level Fail");
                            return false;
                        }
                    }
                    else
                    {
                        if (!SteadyInteractionLevelSetting.Satisfies(actor, target, true))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Steady Acotr Interaction Level Fail");
                            return false;
                        }
                    }
                }
                else
                {
                    if (!RomanceInteractionLevelSetting.Satisfies(actor, target, true))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Romance Interaction Level Fail");
                        return false;
                    }
                }

                if (target.Partner != null)
                {
                    if (target.IsMarried)
                    {
                        if (!MarriedInteractionLevelSetting.Satisfies(target, actor, true))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Married Target Interaction Level Fail");
                            return false;
                        }
                    }
                    else
                    {
                        if (!SteadyInteractionLevelSetting.Satisfies(target, actor, true))
                        {
                            greyedOutTooltipCallback = Common.DebugTooltip("Steady Target Interaction Level Fail");
                            return false;
                        }
                    }
                }
                else
                {
                    if (!RomanceInteractionLevelSetting.Satisfies(target, actor, true))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Romance Interaction Level Fail");
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool OnTestAttraction(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                string autoString = null;
                if (isAutonomous)
                {
                    autoString = "Autonomous ";
                }

                ScoringLookup.IncStat(autoString + "OnTestAttraction Try");

                WoohooScoring.ScoreTestResult result = WoohooScoring.ScoreTarget("TestAttraction", target, actor, isAutonomous, "InterestInRomance", false, false);
                if (result != WoohooScoring.ScoreTestResult.Success)
                {
                    ScoringLookup.IncStat(autoString + "OnTestAttraction " + result);
                    return true;
                }

                if (actor.SimDescription.Teen != target.SimDescription.Teen)
                {
                    actor.SocialComponent.SetPlayerIntendedSocialCommodity(target, AmorousCommodity.sAmorous2);
                }
                else
                {
                    actor.SocialComponent.SetPlayerIntendedSocialCommodity(target, CommodityTypes.Amorous);
                }

                ScoringLookup.IncStat(autoString + "OnTestAttraction Success");
                //ScoringLookup.IncStat(autoString + "OnTestAttraction Success " + actor.FullName + " " + target.FullName);
                return false;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static bool CaresAboutJealousy(Sim actor, Sim target, Sim witness, JealousyLevel level, bool woohoo)
        {
            if (actor.HasTrait(TraitNames.NoJealousy)) return false;

            if (target.HasTrait(TraitNames.NoJealousy)) return false;

            if (CommonSocials.IsPolyamorous(actor.SimDescription, target.SimDescription, witness.SimDescription)) return false;

            foreach (Situation situation in actor.Autonomy.SituationComponent.Situations)
            {
                if (situation.DoesSituationRuleOutJealousy(witness, actor, target, level))
                {
                    return false;
                }
            }

            if (witness.RoomId == actor.RoomId)
            {
                return true;
            }
            else if (woohoo)
            {
                if (witness.IsOutside != actor.IsOutside)
                {
                    return false;
                }
                else
                {
                    int diff = witness.Level - actor.Level;
                    if (diff < 0)
                    {
                        diff = -diff;
                    }

                    if ((actor.Level < 0) && (diff > 0))
                    {
                        return false;
                    }
                    else if ((actor.Level >= 0) && (diff > 1))
                    {
                        return false;
                    }
                    else if (witness.CurrentInteraction is ISleeping)
                    {
                        return witness.TraitManager.HasElement(TraitNames.LightSleeper);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsPolyamorous(SimDescription actor, SimDescription target, SimDescription witness)
        {
            if (Woohooer.Settings.mPolyamorousWoohooJealousy)
            {
                try
                {
                    Relationship relation = Relationship.Get(witness, actor, false);
                    if ((relation != null) && (relation.AreRomantic()))
                    {
                        relation = Relationship.Get(witness, target, false);
                        if ((relation != null) && (relation.AreRomantic())) return true;
                    }
                }
                catch
                { }
            }

            return false;
        }

        public class Tracer : StackTracer
        {
            public bool mIgnore;

            public Tracer()
            {
                AddTest(typeof(Sims3.Gameplay.Objects.DreamsAndPromises.DreamsAndPromisesDelegateFunctions), "CheckResult SocialCore", OnIgnore); // Must be ahead of "AddAutomaticSkill"
            }

            public override void Reset()
            {
                mIgnore = false;

                base.Reset();
            }

            private bool OnIgnore(StackTrace trace, StackFrame frame)
            {
                mIgnore = true;
                return true;
            }
        }
    }
}
