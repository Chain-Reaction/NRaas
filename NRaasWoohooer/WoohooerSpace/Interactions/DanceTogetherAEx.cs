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
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class DanceTogetherAEx : Stereo.DanceTogetherA, Common.IPreLoad, Common.IAddInteraction
    {
        new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Stereo.DanceTogetherA.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public bool BaseRunEx()
        {
            mRelationship = Relationship.Get(Actor, Target, true);
            if (mRelationship == null)
            {
                return false;
            }
            bool flag = false;
            if (IsSlowDance)
            {
                string reason;
                GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                if (CommonSocials.CanGetRomantic(Actor, Target, false, false, true, ref greyedOutTooltipCallback, out reason))
                {
                    flag = true;
                }
            }
            else
            {
                foreach (CommodityTypes types in kAcceptableCommodities)
                {
                    if (mRelationship.STC.CurrentCommodity == types)
                    {
                        flag = true;
                        break;
                    }
                }
            }

            Definition interactionDefinition = InteractionDefinition as Definition;
            IDanceable danceObject = interactionDefinition.DanceObject;
            if (flag)
            {
                mDanceFloor = DanceFloor.FindAndRouteOntoADanceFloorWithFollower(Actor, Target, danceObject);
                if (mDanceFloor != null)
                {
                    mDanceFloor.AddToUseList(Actor);
                    if (!mDanceFloor.IsActorUsingMe(Target))
                    {
                        mDanceFloor.AddToUseList(Target);
                    }
                }
            }

            Stereo targetStereo = interactionDefinition.TargetStereo;
            if (targetStereo != null)
            {
                targetStereo.AddSimListener(Actor);
                targetStereo.AddSimListener(Target);
            }

            string instanceName = "SocialJigTwoPerson";
            ProductVersion baseGame = ProductVersion.BaseGame;
            if (IsSlowDance)
            {
                instanceName = "SlowdanceJig";
                baseGame = ProductVersion.EP8;
            }

            SocialJig = GlobalFunctions.CreateObjectOutOfWorld(instanceName, baseGame) as SocialJigTwoPerson;
            bool succeeded = false;
            Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
            Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
            string name = (Stereo.PartySimIsGettingMusicFromPartyLocation(Actor, danceObject) || Stereo.PartySimIsGettingMusicFromPartyLocation(Target, danceObject)) ? LocalizeString("BeRockingDancingTogether", new object[0x0]) : LocalizeString("BeDancingTogether", new object[0x0]);
            if (BeginSocialInteraction(new SocialInteractionB.Definition(null, name, false), true, false))
            {
                IGlass objectInRightHand = Actor.GetObjectInRightHand() as IGlass;
                if (objectInRightHand != null)
                {
                    objectInRightHand.PutGlassAway();
                }
                objectInRightHand = Target.GetObjectInRightHand() as IGlass;
                if (objectInRightHand != null)
                {
                    objectInRightHand.PutGlassAway();
                }

                string stateMachineName = IsSlowDance ? "slowdance_together" : "dance_together";
                StateMachineClient smc = StateMachineClient.Acquire(Actor, stateMachineName);
                if (IsSlowDance)
                {
                    smc.SetActor("x", Actor);
                    smc.SetActor("y", Target);
                }
                else
                {
                    smc.SetActor("x", Target);
                    smc.SetActor("y", Actor);
                }
                smc.EnterState("x", "enter");
                smc.EnterState("y", "enter");
                if (GameUtils.IsInstalled(ProductVersion.EP7))
                {
                    smc.SetParameter("xIsProper", Target.HasTrait(TraitNames.Proper));
                    smc.SetParameter("yIsProper", Actor.HasTrait(TraitNames.Proper));
                }
                BeginCommodityUpdates();
                smc.RequestState(false, "x", "Ask_To_Dance");
                smc.RequestState(true, "y", "Ask_To_Dance");
                if (flag)
                {
                    smc.RequestState(false, "x", "Last_Sync");
                    smc.RequestState(true, "y", "Last_Sync");
                    smc.RequestState(false, "x", "dance");
                    smc.RequestState(true, "y", "dance");
                    Actor.SkillManager.StartGainAndAddSkillIfNeeded(SkillNames.Dancing, Stereo.kRateOfSkillGainForDancing);
                    Target.SkillManager.StartGainAndAddSkillIfNeeded(SkillNames.Dancing, Stereo.kRateOfSkillGainForDancing);
                    if (IsSlowDance)
                    {
                        EventTracker.SendEvent(EventTypeId.kSlowDanced, Actor, Target);
                        EventTracker.SendEvent(EventTypeId.kSlowDanced, Target, Actor);
                    }
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new InteractionInstance.InsideLoopFunction(DanceTogetherDelegate), smc);
                    Actor.SkillManager.StopSkillGain(SkillNames.Dancing);
                    Target.SkillManager.StopSkillGain(SkillNames.Dancing);
                    smc.RequestState(false, "x", "friendly");
                    smc.RequestState(true, "y", "friendly");
                }
                else
                {
                    smc.RequestState(false, "x", "awkward");
                    smc.RequestState(true, "y", "awkward");
                    mRelationship.LTR.UpdateLiking(Stereo.kLtrDecreaseAfterRejectingDanceTogether);
                    succeeded = false;
                }
            }

            Definition definition2 = InteractionDefinition as Definition;
            if ((definition2 != null) && (definition2.DanceObject is DJTurntable))
            {
                EventTracker.SendEvent(EventTypeId.kDanceToDJMusic, Target);
                EventTracker.SendEvent(EventTypeId.kDanceToDJMusic, Actor);
            }

            EndCommodityUpdates(succeeded);
            Actor.ClearSynchronizationData();
            return succeeded;
        }

        public override bool Run()
        {
            try
            {
                Definition interactionDefinition = InteractionDefinition as Definition;
                IDanceable danceObject = interactionDefinition.DanceObject;
                if (danceObject != null)
                {
                    IPeripheralStereoSpeaker speaker = danceObject as IPeripheralStereoSpeaker;
                    if (speaker != null)
                    {
                        speaker.AddDancer(Actor);
                        speaker.AddDancer(Target);
                    }
                }
                springDanceFloor = SpringDanceFloor.FindSpringDanceFloor(Actor);
                if (IsClubDance)
                {
                    return ClubRun();
                }
                return BaseRunEx();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : Stereo.DanceTogetherA.Definition
        {
            public Definition()
            { }
            public Definition(IDanceable danceObject)
                : base(danceObject)
            { }
            public Definition(string text, string[] menu, Stereo.DanceTogetherA.DanceType danceType, IDanceable danceObject)
                : base(text, menu, danceType, danceObject)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new DanceTogetherAEx();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, iop);
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                try
                {
                    mTargetDanceObject = null;

                    foreach (InteractionObjectPair pair in actor.Interactions)
                    {
                        if (pair.InteractionDefinition.GetType() == typeof(Stereo.DanceTogetherA.Definition))
                        {
                            Stereo.DanceTogetherA.Definition definition = pair.InteractionDefinition as Stereo.DanceTogetherA.Definition;
                            mTargetDanceObject = definition.mTargetDanceObject;
                            break;
                        }
                    }

                    if (mTargetDanceObject == null) return;

                    bool allowDance = mTargetDanceObject.AllowsDance && (Stereo.SimCanBaseDance(actor, mTargetDanceObject) && Stereo.SimCanBaseDance(target, mTargetDanceObject));
                    bool allowClubDance = mTargetDanceObject.AllowsClubDance && (Stereo.SimCanClubDance(actor, mTargetDanceObject) && Stereo.SimCanClubDance(target, mTargetDanceObject));
                    bool canSlowDance = Stereo.SimsCanSlowDanceTogether(actor, target, mTargetDanceObject);
                    int num = 0x0;
                    num += allowDance ? 0x1 : 0x0;
                    num += allowClubDance ? 0x1 : 0x0;
                    num += canSlowDance ? 0x1 : 0x0;
                    bool inClub = Stereo.ObjectIsInDanceClub(target);
                    string[] menu = (num > 0x1) ? new string[] { Stereo.DanceTogetherA.LocalizeString("DanceMenu", new object[0x0]) } : new string[0x0];
                    bool canClubDance = allowClubDance && inClub;
                    Stereo.DanceTogetherA.DanceType dance = Stereo.DanceTogetherA.DanceType.Dance;
                    if (allowDance || canClubDance)
                    {
                        if (canClubDance)
                        {
                            dance = Stereo.DanceTogetherA.DanceType.Club;
                        }

                        if (Stereo.PartySimIsGettingMusicFromPartyLocation(actor, mTargetDanceObject))
                        {
                            results.Add(new InteractionObjectPair(new Definition(Stereo.DanceTogetherA.LocalizeString("RockingInteractionName", new object[0x0]), menu, dance, mTargetDanceObject), iop.Target));
                        }
                        else
                        {
                            results.Add(new InteractionObjectPair(new Definition(Stereo.DanceTogetherA.LocalizeString("InteractionName", new object[0x0]), menu, dance, mTargetDanceObject), iop.Target));
                        }
                    }

                    if (allowClubDance && !canClubDance)
                    {
                        if (Stereo.PartySimIsGettingMusicFromPartyLocation(actor, mTargetDanceObject))
                        {
                            results.Add(new InteractionObjectPair(new Definition(Stereo.DanceTogetherA.LocalizeString("ClubRockingInteractionName", new object[0x0]), menu, Stereo.DanceTogetherA.DanceType.Club, mTargetDanceObject), iop.Target));
                        }
                        else
                        {
                            results.Add(new InteractionObjectPair(new Definition(Stereo.DanceTogetherA.LocalizeString("ClubDanceTogetherA", new object[0x0]), menu, Stereo.DanceTogetherA.DanceType.Club, mTargetDanceObject), iop.Target));
                        }
                    }

                    if (canSlowDance)
                    {
                        results.Add(new InteractionObjectPair(new Definition(Stereo.DanceTogetherA.LocalizeString("SlowDance", new object[0x0]), menu, Stereo.DanceTogetherA.DanceType.Slow, mTargetDanceObject), iop.Target));
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(actor, target, e);
                }
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (mTargetDanceObject == null)
                {
                    return false;
                }

                if (!a.BuffManager.HasElement(BuffNames.EnjoyingMusic))
                {
                    return false;
                }

                SimDescription simDescription = target.SimDescription;
                if (((target.IsLeavingLot || (target.Service != null)) || ((target.Posture != null) && !target.Posture.AllowsNormalSocials())) || ((target.IsDying() || (a == target)) || ((!simDescription.TeenOrAbove || (a.GetObjectInRightHand() is IGuitar)) || (target.CurrentInteraction is IPlayInstrumentInteraction))))
                {
                    return false;
                }

                if (isAutonomous)
                {
                    string reason;
                    if (!CommonSocials.CanGetRomantic(a, target, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason))
                    {
                        return false;
                    }
                    else if (a.CanHaveRomanceWith(target))
                    {
                        // EA Standard interaction is visible
                        return false;
                    }

                    if (!a.SimDescription.CheckAutonomousGenderPreference(target.SimDescription))
                    {
                        return false;
                    }

                    if (IsSlowDance && !mTargetDanceObject.AllowsSlowDance)
                    {
                        return false;
                    }
                }

                if (IsSlowDance)
                {
                    string reason;
                    if (!CommonSocials.CanGetRomantic(a, target, isAutonomous, false, true, ref greyedOutTooltipCallback, out reason))
                    {
                        //greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Stereo.DanceTogetherA.LocalizeString("CantSlowdanceWithSim", new object[0x0]));
                        return false;
                    }
                    else if (a.CanHaveRomanceWith(target))
                    {
                        // EA Standard interaction is visible
                        return false;
                    }
                }
                else if (!isAutonomous)
                {
                    // EA Standard interaction is visible
                    return false;
                }

                return (CelebrityManager.CanSocialize(a, target) && CelebrityManager.CanSocialize(target, a));
            }
        }
    }
}
