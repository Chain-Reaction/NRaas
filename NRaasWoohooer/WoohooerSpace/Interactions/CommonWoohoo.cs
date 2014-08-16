using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Options.Woohoo;
using NRaas.WoohooerSpace.Scoring;
using NRaas.WoohooerSpace.Skills;
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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Entertainment;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Elevator;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Misc;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Pets;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.ShelvesStorage;
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
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    // Cannot move due to persistence of enum
    public class CommonWoohoo : Common.IWorldLoadFinished
    {
        public enum WoohooStyle : int
        {
            Safe = 0,
            Risky,
            TryForBaby,
        }

        public enum WoohooLocation : int
        {
            Bed = 0,
            HotTub,
            ActorTrailer,
            Sarcophagus,
            TimeMachine,
            RabbitHole,
            Elevator,
            Tent,
            Treehouse,
            Shower,
            HayStack,
            BoxStall,
            PetHouse,
            Computer,
            PhotoBooth,
            BoxOfMystery,
            Sauna,
            Wardrobe,
            GypsyCaravan,
            FairyHouse,
            Igloo,
            LeafPile,
            OutdoorShower,
            HotAirBalloon,
            AncientPortal,
            AllInOneBathroom,
            Resort,
            Cave,
            Jetpack,
            BotMaker,
            HoverTrain,
            TimePortal,
			EiffelTower,
			ToiletStall
        }

        public void OnWorldLoadFinished()
        {
            // Must be immediate
            new Common.ImmediateEventListener(EventTypeId.kWooHooed, OnWoohooed);
        }

        public static void OnWoohooed(Event e)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration CommonWoohoo:OnWoohooed"))
            {
                WooHooEvent wEvent = e as WooHooEvent;
                if (wEvent == null)
                {
                    return;
                }

                Sim actor = wEvent.Actor as Sim;
                if (actor == null)
                {
                    return;
                }

                SimDescription targetDesc = null;

                Sim target = wEvent.TargetObject as Sim;
                if (target != null)
                {
                    targetDesc = target.SimDescription;
                }
                else if (actor.SimDescription.IsPregnant)
                {
                    targetDesc = SimDescription.Find(actor.SimDescription.Pregnancy.DadDescriptionId);
                }

                if (targetDesc == null)
                {
                    return;
                }

                CommonWoohoo.WoohooLocation location = WoohooLocation.Bed;

                CommonWoohoo.WoohooStyle style = WoohooStyle.Safe;

                IWooHooDefinition woohoo = null;

                NRaasWooHooEvent customEvent = wEvent as NRaasWooHooEvent;
                if (customEvent != null)
                {
                    location = customEvent.Location;
                    style = customEvent.Style;
                }
                else
                {
                    if (actor.CurrentInteraction != null)
                    {
                        woohoo = actor.CurrentInteraction.InteractionDefinition as IWooHooDefinition;
                        if (woohoo == null)
                        {
                            if (actor.CurrentInteraction is Shower.TakeShower)
                            {
                                foreach (Sim sim in actor.LotCurrent.GetAllActors())
                                {
                                    if ((sim.CurrentInteraction != null) && (sim.CurrentInteraction.Target == actor))
                                    {
                                        woohoo = sim.CurrentInteraction.InteractionDefinition as IWooHooDefinition;
                                        if (woohoo != null) break;
                                    }
                                }
                            }
                        }

                        if (woohoo != null)
                        {
                            location = woohoo.GetLocation(wEvent.ObjectUsed);

                            style = woohoo.GetStyle(actor.CurrentInteraction);
                            if ((style == WoohooStyle.Safe) && (Woohooer.Settings.ReplaceWithRisky))
                            {
                                style = WoohooStyle.Risky;
                            }
                        }
                    }

                    if (wEvent.BedUsed != null)
                    {
                        if (wEvent.BedUsed is Tent)
                        {
                            location = WoohooLocation.Tent;
                        }
                        else if (wEvent.BedUsed is Igloo)
                        {
                            location = WoohooLocation.Igloo;
                        }
                        else if (wEvent.BedUsed is FairyHouse)
                        {
                            location = WoohooLocation.FairyHouse;
                        }
                    }
                    else if ((woohoo == null) && (wEvent.ObjectUsed != null))
                    {
                        foreach (WoohooLocationControl check in Common.DerivativeSearch.Find<WoohooLocationControl>())
                        {
                            if (check.Matches(wEvent.ObjectUsed))
                            {
                                location = check.Location;
                                break;
                            }
                        }
                    }
                }

                KamaSimtra.AddNotch(actor.SimDescription, targetDesc, actor.LotCurrent, location, style);

                Woohooer.Settings.AddCount(actor);

                WoohooBuffs.Apply(actor, target, style == WoohooStyle.Risky);
            }
        }

        public static bool NeedPrivacy(bool inherent, Sim actor, Sim target)
        {
            if (inherent)
            {
                return false;
            }
            else if (Woohooer.Settings.mEnforcePrivacy)
            {
                return true;
            }
            else if ((Woohooer.Settings.mTraitScoredPrivacy) && (Woohooer.Settings.UsingTraitScoring))
            {
                if ((!WoohooScoring.TestScoringNormal(actor, target, "Privacy", false)) &&
                    (!WoohooScoring.TestScoringNormal(target, actor, "Privacy", false)))
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool HasWoohooableObject(Lot lot, Sim actor, Sim target)
        {
            if (actor.IsHuman)
            {
                foreach (WoohooLocationControl location in Common.DerivativeSearch.Find<WoohooLocationControl>())
                {
                    if (!location.AllowLocation(actor.SimDescription, false)) continue;

                    if (!location.AllowLocation(target.SimDescription, false)) continue;

                    if (location.HasWoohooableObject(lot)) return true;
                }

                return false;
            }
            else if (actor.IsHorse)
            {
                if (new BoxStallWoohoo.LocationControl().HasWoohooableObject(lot)) return true;
            }
            else
            {
                if (new PetHouseWoohoo.LocationControl().HasWoohooableObject(lot)) return true;
            }

            return false;
        }

        public static bool SatisfiesUserLikingGate(Sim actor, Sim target, bool isAutonomous, bool woohoo, string logName)
        {
            if ((isAutonomous) || (Woohooer.Settings.mLikingGateForUserDirected))
            {
                if (!CommonSocials.SatisfiesLikingGate(actor, target, woohoo))
                {
                    ScoringLookup.IncStat(logName + " LikingGate");
                    return false;
                }
                else
                {
                    Relationship relation = Relationship.Get(actor, target, false);
                    if (relation != null)
                    {
                        if ((relation.CurrentSTC != ShortTermContextTypes.Hot) && (relation.CurrentSTC != AmorousCommodity.sHot2))
                        {
                            ScoringLookup.IncStat(logName + " STC Fail " + relation.CurrentSTC);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static bool SatisfiesCooldown(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback callback)
        {
            if (!isAutonomous) return true;

            DateAndTime time;
            if (Woohooer.Settings.mLastWoohoo.TryGetValue(actor.SimDescription.SimDescriptionId, out time))
            {
                if (SimClock.ElapsedTime(TimeUnit.Minutes, time, SimClock.CurrentTime()) < Woohooer.Settings.mWoohooCooldown[PersistedSettings.GetSpeciesIndex(actor)])
                {
                    callback = Common.DebugTooltip("Actor Cooldown");
                    return false;
                }
            }

            if (Woohooer.Settings.mLastWoohoo.TryGetValue(target.SimDescription.SimDescriptionId, out time))
            {
                if (SimClock.ElapsedTime(TimeUnit.Minutes, time, SimClock.CurrentTime()) < Woohooer.Settings.mWoohooCooldown[PersistedSettings.GetSpeciesIndex(target)])
                {
                    callback = Common.DebugTooltip("Target Cooldown");
                    return false;
                }
            }

            return true;
        }

        public static bool SatisfiesWoohoo(Sim actor, Sim target, string logName, bool isAutonomous, bool scoreTarget, bool testLiking, ref GreyedOutTooltipCallback callback)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration " + logName, Common.DebugLevel.Stats))
            {
                if (isAutonomous)
                {
                    if (!Woohooer.Settings.mWoohooAutonomousV2[PersistedSettings.GetSpeciesIndex(actor)])
                    {
                        callback = Common.DebugTooltip("Autonomous Denied");

                        ScoringLookup.IncStat(logName + " Autonomous Denied");
                        return false;
                    }
                }

                if (!Woohooer.Settings.mAllowZombie)
                {
                    if ((actor.SimDescription.IsZombie) || (target.SimDescription.IsZombie))
                    {
                        callback = Common.DebugTooltip("Zombie");

                        ScoringLookup.IncStat(logName + " Zombie");
                        return false;
                    }
                }

                if (testLiking)
                {
                    if (Woohooer.Settings.mHideWoohoo)
                    {
                        callback = Common.DebugTooltip("Hide Woohoo");

                        ScoringLookup.IncStat(logName + " Hide Woohoo");
                        return false;
                    }

                    if ((!scoreTarget) && (!SatisfiesUserLikingGate(actor, target, isAutonomous, true, logName)))
                    {
                        callback = Common.DebugTooltip("Liking Gate Fail");

                        ScoringLookup.IncStat(logName + " Liking Gate Fail");
                        return false;
                    }

                    if (!WoohooInteractionLevelSetting.Satisfies(actor, target, true))
                    {
                        ScoringLookup.IncStat(logName + " Interaction Level Fail");

                        callback = Common.DebugTooltip("Interaction Level Fail");
                        return false;
                    }
                }

                if (!CommonSocials.SatisfiedInteractionLevel(actor, target, isAutonomous, ref callback))
                {
                    ScoringLookup.IncStat(logName + " InteractionLevel Fail");
                    return false;
                }

                if (!SatisfiesCooldown(actor, target, isAutonomous, ref callback))
                {
                    ScoringLookup.IncStat(logName + " Cooldown Fail");
                    return false;
                }

                string reason;
                if (!CommonSocials.CanGetRomantic(actor, target, isAutonomous, true, testLiking, ref callback, out reason))
                {
                    ScoringLookup.IncStat(logName + " " + reason);
                    return false;
                }

                WoohooScoring.ScoreTestResult result = WoohooScoring.ScoreActor(logName, actor, target, isAutonomous, "InterestInWoohoo", true);
                if (result != WoohooScoring.ScoreTestResult.Success)
                {
                    ScoringLookup.IncStat(logName + " " + result);

                    callback = Common.DebugTooltip("Actor Scoring Fail " + result);
                    return false;
                }

                if (scoreTarget)
                {
                    result = WoohooScoring.ScoreTarget(logName, target, actor, isAutonomous, "InterestInWoohoo", true);
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

        public static string GetSocialName(CommonWoohoo.WoohooStyle style, Sim actor)
        {
            string action = "NRaas";

            if (actor != null)
            {
                if (actor.IsHorse)
                {
                    action += "Horse";
                }
                else if (!actor.IsHuman)
                {
                    action += "Pet";
                }
            }

            switch (style)
            {
                case CommonWoohoo.WoohooStyle.Safe:
                    return action + "WooHoo";
                case CommonWoohoo.WoohooStyle.Risky:
                    return action + "RiskyWooHoo";
                case CommonWoohoo.WoohooStyle.TryForBaby:
                    return action + "TryForBaby";
            }

            return null;
        }

        public abstract class BaseDefinition<TTarget, TInteraction> : InteractionDefinition<Sim, TTarget, TInteraction>, IWooHooDefinition
            where TTarget : class, IGameObject
            where TInteraction : InteractionInstance, new()
        {
            public BaseDefinition()
            { }

            public Sim ITarget(InteractionInstance interaction)
            {
                return GetTarget(interaction.InstanceActor as Sim, interaction.Target as TTarget, interaction);
            }

            public abstract Sim GetTarget(Sim actor, TTarget target, InteractionInstance interaction);

            public abstract int Attempts
            {
                set;
            }

            public abstract WoohooLocation GetLocation(IGameObject obj);

            public abstract WoohooStyle GetStyle(InteractionInstance interaction);

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    Sim actor = parameters.Actor as Sim;
                    if (actor == null) return InteractionTestResult.Root_Null_Actor;

                    if ((actor.LotCurrent == null) || (actor.LotCurrent.IsWorldLot))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("World Lot");
                        return InteractionTestResult.Tuning_LotAvailability;
                    }

                    WoohooLocationControl control = WoohooLocationControl.GetControl(GetLocation(parameters.Target));
                    if (control == null)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Control Denied");
                        return InteractionTestResult.Def_TestFailed;
                    }

                    if (!control.AllowLocation(actor.SimDescription, false))
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Location Denied");
                        return InteractionTestResult.Def_TestFailed;
                    }

                    using (WoohooTuningControl tuningControl = new WoohooTuningControl(parameters.InteractionObjectPair.Tuning, Woohooer.Settings.mAllowTeenWoohoo))
                    {
                        InteractionTestResult result = base.Test(ref parameters, ref greyedOutTooltipCallback);

                        if ((greyedOutTooltipCallback == null) && (Common.kDebugging))
                        {
                            greyedOutTooltipCallback = delegate { return result.ToString(); };
                        }

                        return result;
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(parameters.Actor, parameters.Target, e);
                    return InteractionTestResult.GenericFail;
                }
            }

            public abstract InteractionDefinition ProxyClone(Sim target);
        }

        public abstract class PrimaryProxyDefinition<TTarget, TInteraction, TDefinition> : InteractionDefinition<Sim, TTarget, TInteraction>, IWooHooDefinition
            where TTarget : class, IGameObject
            where TInteraction : InteractionInstance, new()
            where TDefinition : IWooHooProxyDefinition
        {
            TDefinition mDefinition;

            public PrimaryProxyDefinition(TDefinition definition)
            {
                mDefinition = definition;
            }

            public TDefinition Definition
            {
                get { return mDefinition; }
            }

            public override string GetInteractionName(Sim actor, TTarget target, InteractionObjectPair iop)
            {
                return mDefinition.GetName(actor, target, iop);
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return InteractionTestResult.Pass;
            }

            public override bool Test(Sim actor, TTarget target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }

            public virtual Sim ITarget(InteractionInstance interaction)
            {
                return mDefinition.ITarget(interaction);
            }

            public CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
            {
                return mDefinition.GetLocation(obj);
            }

            public CommonWoohoo.WoohooStyle GetStyle(InteractionInstance interaction)
            {
                return mDefinition.GetStyle(interaction);
            }

            public int Attempts
            {
                set { mDefinition.Attempts = value; }
            }

            public InteractionDefinition ProxyClone(Sim target)
            {
                return mDefinition.ProxyClone(target);
            }
        }

        public abstract class PotentialDefinition<TTarget> : BaseDefinition<TTarget, WooHooControlInteraction<TTarget>>, IWooHooProxyDefinition
            where TTarget : class, IGameObject
        {
            Sim mTarget;

            [Persistable(false)]
            Dictionary<SimDescription, List<SimDescription>> mPotentials = new Dictionary<SimDescription, List<SimDescription>>();        

            int mAttempts;

            public PotentialDefinition()
            { }
            public PotentialDefinition(Sim target)
            {
                mTarget = target;
            }

            public override int Attempts
            {
                set { mAttempts = value; }
            }

            public virtual bool PushSocial
            {
                get { return (mTarget == null); }
            }

            public void RemovePotentials(SimDescription sim)
            {
                mPotentials.Remove(sim);
            }

            public override Sim GetTarget(Sim actor, TTarget target, InteractionInstance interaction)
            {
                if (mTarget != null) return mTarget;

                if (interaction == null) return null;

                Sim selected = interaction.GetSelectedObject() as Sim;
                if (selected != null) return selected;

                List<Sim> potentials = GetPotentials(actor, target, interaction.Autonomous, null);
                if ((potentials == null) || (potentials.Count == 0))
                {
                    try
                    {
                        throw new Exception();
                    }
                    catch (Exception e)
                    {
                        Common.Exception(actor, target, "No Potentials", e);
                    }
                    return null;
                }

                interaction.mSelectedObjects = new List<object>();
                interaction.mSelectedObjects.Add(RandomUtil.GetRandomObjectFromList(potentials));

                return interaction.GetSelectedObject() as Sim;
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                try
                {
                    if (GetTarget(parameters.Actor as Sim, parameters.Target as TTarget, null) != null)
                    {
                        listObjs = null;
                        headers = null;
                        NumSelectableRows = 0x0;
                    }
                    else
                    {
                        NumSelectableRows = 0x1;
                        PopulateSimPicker(ref parameters, out listObjs, out headers, GetPotentials(parameters.Actor as Sim, parameters.Target as TTarget, parameters.Autonomous, null), false);
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    listObjs = null;
                    headers = null;
                    NumSelectableRows = 0x0;

                    Common.Exception(parameters.Actor, parameters.Target, exception);
                }
            }

            public override bool Test(Sim a, TTarget target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    Sim targetSim = GetTarget(a, target, null);
                    if ((targetSim != null) && (Satisfies(a, targetSim, target, isAutonomous, ref greyedOutTooltipCallback)))
                    {
                        return true;
                    }
                    else
                    {
                        StringBuilder builder = null;

                        if (Common.kDebugging)
                        {
                            builder = new StringBuilder();
                        }

                        // Remove any previous scoring results
                        RemovePotentials(a.SimDescription);

                        if (GetPotentials(a, target, isAutonomous, builder).Count > 0x0)
                        {
                            return true;
                        }

                        if (builder != null)
                        {
                            greyedOutTooltipCallback = delegate 
                            { 
                                string result = builder.ToString();
                                if (string.IsNullOrEmpty(result))
                                {
                                    result = "No Choices";
                                }

                                return result; 
                            };
                        }

                        return false;
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Common.Exception(a, target, exception);
                    return false;
                }
            }

            protected abstract bool Satisfies(Sim actor, Sim target, TTarget obj, bool isAutonomous, ref GreyedOutTooltipCallback callback);

            public bool IJoinInProgress(Sim actor, Sim target, IGameObject obj, InteractionInstance interaction)
            {
                return JoinInProgress(actor, target, obj as TTarget, interaction);
            }

            public virtual bool JoinInProgress(Sim actor, Sim target, TTarget obj, InteractionInstance interaction)
            {
                return false;
            }

            public abstract void PushWooHoo(Sim actor, Sim target, IGameObject obj);

            public string GetName(Sim actor, IGameObject obj, InteractionObjectPair iop)
            {
                return GetInteractionName(actor, obj as TTarget, iop);
            }

            protected List<Sim> GetPotentials(Sim actor, TTarget obj, bool isAutonomous, StringBuilder results)
            {
                List<SimDescription> potentials;
                if (!mPotentials.TryGetValue(actor.SimDescription, out potentials))
                {
                    potentials = new List<SimDescription>();
                    mPotentials.Add(actor.SimDescription, potentials);

                    try
                    {
                        GreyedOutTooltipCallback callback = null;

                        foreach (Sim target in actor.LotCurrent.GetAllActors())
                        {
                            if (isAutonomous)
                            {
                                if (target.RoomId != actor.RoomId) continue;
                            }

                            if (!Satisfies(actor, target, obj, isAutonomous, ref callback))
                            {
                                if ((results != null) && (callback != null))
                                {
                                    results.Append(Common.NewLine + target.FullName + ": " + callback());
                                }
                                continue;
                            }

                            potentials.Add(target.SimDescription);
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(actor, e);
                    }
                }

                List<Sim> list = new List<Sim>();
                foreach (SimDescription sim in potentials)
                {
                    if ((sim.CreatedSim == null) || (sim.CreatedSim.HasBeenDestroyed)) continue;

                    if (sim.CreatedSim.LotCurrent != actor.LotCurrent) continue;

                    list.Add(sim.CreatedSim);
                }

                return list;
            }

            public void Restart(bool master, Sim actor, Sim target, bool autonomous, GameObject obj)
            {
                if ((master) && (mAttempts > 0))
                {
                    new PushWoohoo(target, actor, autonomous, obj, mAttempts - 1, GetStyle(null));
                }
            }
        }

        private static bool WasWitnessed(Sim actor, Sim target, JealousyLevel level, bool woohoo)
        {
            foreach (Sim sim in actor.LotCurrent.GetAllActors())
            {
                if (sim == actor) continue;

                if (sim == target) continue;

                if (CommonSocials.CaresAboutJealousy(actor, target, sim, level, woohoo)) return true;
            }

            return false;
        }

        public static void CheckForWitnessedCheating(Sim actor, Sim target, bool wasAccepted)
        {
            CommonSocials.SendCheatingEvents(actor, target, WasWitnessed(actor, target, Woohooer.Settings.mWoohooJealousyLevel, true), Woohooer.Settings.mWoohooJealousyLevel, wasAccepted);
        }

        public static void TestNakedOutfit(bool allow, Sim actor)
        {
            TestNakedOutfit(allow, actor, actor);
        }
        public static void TestNakedOutfit(bool allow, Sim actor, Sim target)
        {
            if (allow)
            {
                if ((!actor.OccultManager.DisallowClothesChange()) && (!actor.TraitManager.HasElement(TraitNames.NeverNude)))
                {
                    actor.SwitchToOutfitWithoutSpin(OutfitCategories.Naked, 0);
                }
                Woohooer.Settings.AddChange(actor);

                if (actor != target)
                {
                    if ((!target.OccultManager.DisallowClothesChange()) && (!target.TraitManager.HasElement(TraitNames.NeverNude)))
                    {
                        target.SwitchToOutfitWithoutSpin(OutfitCategories.Naked, 0);
                    }
                    Woohooer.Settings.AddChange(target);
                }
            }
        }

        public static void RunPostWoohoo(Sim actor, Sim target, IBed obj, CommonWoohoo.WoohooStyle style, CommonWoohoo.WoohooLocation location, bool fireDisgraceEvents)
        {
            EventTracker.SendEvent(new NRaasWooHooEvent(EventTypeId.kWooHooed, actor, target, obj, style, location));
            EventTracker.SendEvent(new NRaasWooHooEvent(EventTypeId.kWooHooed, target, actor, obj, style, location));
            RunPostWhoohooInternal(actor, target, obj, fireDisgraceEvents);
        }
        public static void RunPostWoohoo(Sim actor, Sim target, IGameObject obj, CommonWoohoo.WoohooStyle style, CommonWoohoo.WoohooLocation location, bool fireDisgraceEvents)
        {
            EventTracker.SendEvent(new NRaasWooHooEvent(EventTypeId.kWooHooed, actor, target, obj, style, location));
            EventTracker.SendEvent(new NRaasWooHooEvent(EventTypeId.kWooHooed, target, actor, obj, style, location));
            RunPostWhoohooInternal(actor, target, obj, fireDisgraceEvents);
        }

        private static void RunPostWhoohooInternal(Sim actor, Sim target, IGameObject obj, bool fireDisgraceEvents)
        {
            SimDescription actorSim = actor.SimDescription;
            if (!actorSim.HadFirstWooHoo)
            {
                actorSim.SetFirstWooHoo();
                EventTracker.SendEvent(EventTypeId.kHadFirstWoohoo, actor, target);
            }

            Woohooer.Settings.mLastWoohoo[actorSim.SimDescriptionId] = SimClock.CurrentTime();

            SimDescription targetSim = target.SimDescription;
            if (!targetSim.HadFirstWooHoo)
            {
                targetSim.SetFirstWooHoo();
                EventTracker.SendEvent(EventTypeId.kHadFirstWoohoo, target, actor);
            }

            Woohooer.Settings.mLastWoohoo[targetSim.SimDescriptionId] = SimClock.CurrentTime();

            Relationship relationship = Relationship.Get(actor, target, true);
            if (relationship != null)
            {
                relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Kissed);
                relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.WooHoo);
            }

            if ((obj != null) && (obj.LotCurrent.LotType == LotType.Commercial))
            {
                actor.BuffManager.AddElement(BuffNames.PublicWooHoo, Origin.FromWooHooInPublic);
                target.BuffManager.AddElement(BuffNames.PublicWooHoo, Origin.FromWooHooInPublic);
            }

            if (actor.IsHuman)
            {
                if (fireDisgraceEvents)
                {
                    CommonWoohoo.CheckForWitnessedCheating(actor, target, true);
                }

                actor.SimDescription.SetFirstKiss(target.SimDescription);
                target.SimDescription.SetFirstKiss(actor.SimDescription);

                if (fireDisgraceEvents)
                {
                    CommonWoohoo.WoohooDisgraceChecks(actor, target);
                }

                if ((actor.Household != target.Household) && !Relationship.AreFianceeOrSpouse(actor, target))
                {
                    if (Woohooer.Settings.mAllowStrideOfPride)
                    {
                        actor.BuffManager.AddElement(BuffNames.StrideOfPride, Origin.FromWooHooOffHome);
                        target.BuffManager.AddElement(BuffNames.StrideOfPride, Origin.FromWooHooOffHome);
                    }
                }

                SocialCallback.AddIncredibleTimeBuffIfNecessary(actor, target);
                SocialCallback.AddIncredibleTimeBuffIfNecessary(target, actor);
            }
            else
            {
                EventTracker.SendEvent(EventTypeId.kPetWooHooed, actor, target);
                EventTracker.SendEvent(EventTypeId.kPetWooHooed, target, actor);

                if (((actor.Partner == null) && (target.Partner == null)) || (Woohooer.Settings.mForcePetMate[PersistedSettings.GetSpeciesIndex(actor)]))
                {
                    Relationship.RemoveAllPetMateFlags(actor);
                    Relationship.RemoveAllPetMateFlags(target);
                    Relationship.Get(actor, target, false).LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Marry);
                }
            }
        }

        public static void WoohooDisgraceChecks(Sim actor, Sim target)
        {
            if (!WasWitnessed(actor, target, JealousyLevel.High, true)) return;

            if (actor.LotCurrent.IsCommunityLot)
            {
                DisgracefulActionEvent e = new DisgracefulActionEvent(EventTypeId.kSimCommittedDisgracefulAction, actor, DisgracefulActionType.WooHooInPublic);
                e.TargetId = target.SimDescription.SimDescriptionId;
                EventTracker.SendEvent(e);

                e = new DisgracefulActionEvent(EventTypeId.kSimCommittedDisgracefulAction, target, DisgracefulActionType.WooHooInPublic);
                e.TargetId = actor.SimDescription.SimDescriptionId;
                EventTracker.SendEvent(e);
            }

            OccultTypes currentOccultTypes = target.SimDescription.OccultManager.CurrentOccultTypes;
            OccultTypes types2 = actor.SimDescription.OccultManager.CurrentOccultTypes;

            List<KamaSimtra.OccultTypesEx> actorTypes = KamaSimtra.GetOccultType(actor.SimDescription, false);
            List<KamaSimtra.OccultTypesEx> targetTypes = KamaSimtra.GetOccultType(target.SimDescription, false);

            if (actorTypes.Count == 0)
            {
                if (targetTypes.Count > 0)
                {
                    DisgracefulActionEvent event3 = new DisgracefulActionEvent(EventTypeId.kSimCommittedDisgracefulAction, actor, DisgracefulActionType.WooHooWithOccult);
                    event3.TargetId = target.SimDescription.SimDescriptionId;
                    EventTracker.SendEvent(event3);
                }
            }
            else if (targetTypes.Count == 0)
            {
                DisgracefulActionEvent event4 = new DisgracefulActionEvent(EventTypeId.kSimCommittedDisgracefulAction, target, DisgracefulActionType.WooHooWithOccult);
                event4.TargetId = actor.SimDescription.SimDescriptionId;
                EventTracker.SendEvent(event4);
            }
        }

        public static List<WoohooLocationControl> GetValidLocations(SimDescription sim)
        {
            List<WoohooLocationControl> values = new List<WoohooLocationControl>();
            foreach (WoohooLocationControl value in Common.DerivativeSearch.Find<WoohooLocationControl>())
            {
                if (!value.AllowLocation(sim, false)) continue;

                values.Add(value);
            }

            return values;
        }

        public class PushWoohoo : Common.AlarmTask
        {
            Sim mActor;
            Sim mTarget;

            GameObject mObject;
            int mAttempts;

            bool mAutonomous;

            WoohooStyle mStyle;

            public PushWoohoo(Sim actor, Sim target, bool autonomous, WoohooStyle style)
                : this(actor, target, autonomous, null, 2, style)
            { }
            public PushWoohoo(Sim actor, Sim target, bool autonomous, GameObject obj, int attempts, WoohooStyle style)
                : base(1, TimeUnit.Seconds)
            {
                mActor = actor;
                mTarget = target;
                mObject = obj;
                mAttempts = attempts;
                mStyle = style;
                mAutonomous = autonomous;
            }

            protected bool TestInRoom(IGameObject obj, object customData)
            {
                if (obj.RoomId == mActor.RoomId) return true;

                if (obj.RoomId == mTarget.RoomId) return true;

                return false;
            }

            protected bool TestOnLevel(IGameObject obj, object customData)
            {
                if (obj.Level == mActor.Level) return true;

                if (obj.Level == mTarget.Level) return true;

                return false;
            }

            protected GameObject GetRandomObject(SimDescription sim, out WoohooLocationControl control)
            {
                List<WoohooLocationControl> values = GetValidLocations(sim);

                RandomUtil.RandomizeListOfObjects<WoohooLocationControl>(values);

                foreach (WoohooLocationControl value in values)
                {
                    if (!value.AllowLocation(mActor.SimDescription, false)) continue;

                    if (!value.AllowLocation(mTarget.SimDescription, false)) continue;

                    List<GameObject> objs = value.GetAvailableObjects(mActor, mTarget, TestInRoom);

                    if ((objs != null) && (objs.Count > 0))
                    {
                        control = value;

                        return RandomUtil.GetRandomObjectFromList(objs);
                    }
                }

                foreach (WoohooLocationControl value in values)
                {
                    List<GameObject> objs = value.GetAvailableObjects(mActor, mTarget, TestOnLevel);

                    if ((objs != null) && (objs.Count > 0))
                    {
                        control = value;

                        return RandomUtil.GetRandomObjectFromList(objs);
                    }
                }

                foreach (WoohooLocationControl value in values)
                {
                    List<GameObject> objs = value.GetAvailableObjects(mActor, mTarget, null);

                    if ((objs != null) && (objs.Count > 0))
                    {
                        control = value;

                        return RandomUtil.GetRandomObjectFromList(objs);
                    }
                }

                control = null;

                return null;
            }

            protected override void OnPerform()
            {
                WoohooLocationControl location = null;
                if (mObject == null)
                {
                    mObject = GetRandomObject(mActor.SimDescription, out location);
                }

                if ((mObject == null) || (location == null)) return;

                InteractionDefinition interaction = location.GetInteraction(mActor, mTarget, mStyle);
                if (interaction != null)
                {
                    IWooHooDefinition woohooDefinition = interaction as IWooHooDefinition;
                    if (woohooDefinition != null)
                    {
                        woohooDefinition.Attempts = mAttempts;
                    }

                    InteractionInstance instance = interaction.CreateInstance(mObject, mActor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);

                    if ((mActor.InteractionQueue != null) && (mActor.InteractionQueue.Add(instance)))
                    {
                        ScoringLookup.IncStat("Push Success " + location + " " + mStyle);
                    }
                    else
                    {
                        ScoringLookup.IncStat("Push Fail " + location + " " + mStyle);
                    }
                }
            }
        }

        public class NRaasWooHooEvent : WooHooEvent
        {
            CommonWoohoo.WoohooLocation mLocation;

            CommonWoohoo.WoohooStyle mStyle;

            public NRaasWooHooEvent(EventTypeId id, Sim actor, Sim target, IBed obj, CommonWoohoo.WoohooStyle style, CommonWoohoo.WoohooLocation location)
                : base(id, actor, target, obj)
            {
                mStyle = style;
                mLocation = location;
            }
            public NRaasWooHooEvent(EventTypeId id, Sim actor, Sim target, IGameObject obj, CommonWoohoo.WoohooStyle style, CommonWoohoo.WoohooLocation location)
                : base(id, actor, target, obj)
            {
                mStyle = style;
                mLocation = location;
            }

            public CommonWoohoo.WoohooStyle Style
            {
                get { return mStyle; }
            }

            public CommonWoohoo.WoohooLocation Location
            {
                get { return mLocation; }
            }
        }

        public class JetpackLocationControl : WoohooLocationControl
        {
            public override CommonWoohoo.WoohooLocation Location
            {
                get { return CommonWoohoo.WoohooLocation.Jetpack; }
            }

            public override bool Matches(IGameObject obj)
            {
                return obj is Jetpack;
            }

            public override bool HasWoohooableObject(Lot lot)
            {
                return (lot.GetObjects<Jetpack>(new Predicate<Jetpack>(TestUse)).Count > 0);
            }

            public override bool HasLocation(Lot lot)
            {
                return (lot.CountObjects<Jetpack>() > 0);
            }

            public override bool AllowLocation(SimDescription sim, bool testVersion)
            {
                if (!sim.IsHuman) return false;

                if (testVersion)
                {
                    if (!GameUtils.IsInstalled(ProductVersion.EP11))
                    {
                        return false;
                    }
                }

                return Woohooer.Settings.mAutonomousJetPack;
            }

            public bool TestUse(Jetpack obj)
            {
                if (!TestRepaired(obj)) return false;

                return (obj.UseCount == 0);
            }

            public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
            {
                List<GameObject> results = new List<GameObject>();

                foreach (Jetpack obj in actor.LotCurrent.GetObjects<Jetpack>(new Predicate<Jetpack>(TestUse)))
                {
                    if ((testFunc != null) && (!testFunc(obj, null))) continue;

                    results.Add(obj);
                }

                return results;
            }

            public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
            {
                return null;
            }
        }
    }
}
