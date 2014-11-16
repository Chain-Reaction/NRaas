using System;
using System.Collections.Generic;

using System.Text;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Autonomy;
using Sims3.UI;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.Decorations.WorkingSimsBed;
using Sims3.Gameplay.Services;

namespace Sims3.Gameplay.Objects.Decorations.WorkingSimsBed
{
    public class WooHooBedActions : BedDouble
    {
        #region Add WooHoo Interaction
        public static void AddWooHooToQue(BedDouble bed, Sim actor, Sim recipient, InteractionDefinition definition)
        {           
            actor.GreetSimOnMyLotIfPossible(recipient);
            InteractionPriority priority = actor.CurrentInteraction.GetPriority();
            InteractionInstance entry = OverridedEnterRelaxing.Singleton.CreateInstance(bed, actor, priority, false, true);
            if (actor.InteractionQueue.Add(entry))
            {
                InteractionInstance instance2 = OverridedEnterRelaxing.Singleton.CreateInstance(bed, recipient, priority, false, true);
                recipient.InteractionQueue.Add(instance2);
                InteractionInstance instance3 = definition.CreateInstance(recipient, actor, priority, actor.CurrentInteraction.Autonomous, true);
                instance3.GroupId = entry.GroupId;
                actor.InteractionQueue.Add(instance3);

            }
        }
        #endregion

        #region Enter Relaxing
        public class OverridedEnterRelaxing : InteractionGameObjectHit<Sim, Bed>
        {
            // Fields
            private bool mbInLine;
            private BedData mEntryPart;
            private string mEntryStateName = "";
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override void Cleanup()
            {
                if ((this.mbInLine && (base.Target != null)) && (base.Target.SimLine != null))
                {
                    base.Target.SimLine.RemoveFromQueue(base.Actor);
                    this.mbInLine = false;
                }
                base.Cleanup();
            }

            private void MaybePushRelax()
            {
                if (base.Actor.InteractionQueue.QueueEmptyAfter(this))
                {
                    base.LowerPriority();
                    base.Actor.InteractionQueue.AddNextIfPossible(OverridedBedRelax.Singleton.CreateInstance(base.Target, base.Actor, base.GetPriority(), true, true));
                    base.RaisePriority();
                }
            }

            private bool Route()
            {
                Slot slot;
                // Sim sim;
                BedData partContaining = base.Target.GetPartContaining(base.Actor);
                if (partContaining != null)
                {
                    //if (base.Actor.Posture is SleepingPosture)
                    if (base.Actor.Posture.Satisfies(CommodityKind.Sleeping, null))
                    {
                        StateMachineClient stateMachine = partContaining.StateMachine;
                        if (stateMachine != null)
                        {
                            stateMachine.RequestState("x", "ExitRelax");
                            partContaining.StateMachine = null;
                        }
                        partContaining.BedMade = true;
                    }
                    this.mEntryStateName = "Enter_OnBed_" + partContaining.StateNameSuffix;
                    this.mEntryPart = partContaining;
                    return true;
                }
                if (base.Target.SimLine != null)
                {
                    ExitReason exitReasons = ~(ExitReason.MidRoutePushRequested | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached);
                    if (!base.Target.SimLine.WaitForTurn(this, SimQueue.WaitBehavior.Default, exitReasons, 30f) || base.Actor.HasExitReason(exitReasons))
                    {
                        return false;
                    }
                    this.mbInLine = true;
                }
                PartData partClosestToHit = base.GetPartClosestToHit();
                List<PartData> parts = new List<PartData>();
                foreach (PartData data3 in base.Target.PartComp.PartDataList.Values)
                {
                    parts.Add(data3);
                }
                if (!base.Actor.RouteToUnusedPart(parts, partClosestToHit, out slot))
                {
                    return false;
                }
                BedData entryPart = null;
                // Label_017C:
                foreach (BedData data5 in base.Target.PartComp.PartDataList.Values)
                {
                    for (int i = 0; i < data5.RoutingSlot.Length; i++)
                    {
                        if ((data5.RoutingSlot[i] == slot) && !data5.InUse)
                        {
                            entryPart = data5;
                        }
                    }
                }
                if (!base.Target.SetContainedObject(entryPart, base.Actor))
                {
                    base.Actor.AddExitReason(ExitReason.FailedToStart);
                    return false;
                }
                this.mEntryStateName = "Enter_BedRelax_" + entryPart.StateNameSuffix;
                this.mEntryPart = entryPart;
                return true;
            }

            public override bool Run()
            {
                if (this.Route())
                {
                    base.StandardEntry(false);
                    base.Actor.BuffManager.AddElement(BuffNames.Comfy, base.Target.TuningBed.ComfyBuffStrength, unchecked((Origin)(-8359806666160896334L)));

                    if (base.Actor.Posture.Satisfies(CommodityKind.Relaxing, null))
                    {
                        this.MaybePushRelax();
                        base.DoCommodityUpdates();
                        base.StandardExit(false, false);
                        return true;
                    }

                    if (this.mEntryPart.RelaxOnBed(base.Actor, this.mEntryStateName))
                    {
                        if (base.Actor.HasExitReason(ExitReason.UserCanceled))
                        {
                            base.Actor.AddExitReason(ExitReason.CancelledByPosture);
                            base.StandardExit(false, false);
                            return false;
                        }
                        base.DoCommodityUpdates();
                        this.MaybePushRelax();
                        base.StandardExit(false, false);
                        return true;
                    }
                }

                return false;
            }

            public override bool ShouldReplace(InteractionInstance interaction)
            {
                return Posture.TestForShouldReplace(this, interaction);
            }

            // Nested Types
            public sealed class Definition : InteractionDefinition<Sim, Bed, OverridedEnterRelaxing>
            {
                // Methods
                public override bool Test(Sim a, Bed target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (!target.CanBeUsedAsBed)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
        #endregion

        #region BedRelax
        internal sealed class OverridedBedRelax : Interaction<Sim, Bed>
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run()
            {
                base.StandardEntry(false);
                base.BeginCommodityUpdates();
                base.Actor.Posture.CurrentStateMachine.RequestState("x", "RelaxIdle");
                base.Actor.WaitAndGroupTalk();
                base.Actor.Posture.CurrentStateMachine.RequestState("x", "Relax");
                base.EndCommodityUpdates(true);
                base.StandardExit(false, false);
                return true;
            }

            // Nested Types
            public sealed class Definition : InteractionDefinition<Sim, Bed, OverridedBedRelax>
            {
                // Methods
                public override bool Test(Sim a, Bed target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.CanBeUsedAsBed;
                }
            }
        }
        #endregion

        #region Mandatory Interface stuff
        public override bool CanShareBed(Sim s, BedData entryPart, Sims3.Gameplay.Autonomy.CommodityKind use, out Sim incompatibleSim)
        {
            incompatibleSim = null;
            return true;
        }

        public override bool CanShareBed(Sim newSim, Sims3.Gameplay.Autonomy.CommodityKind use, out Sim incompatibleSim)
        {
            incompatibleSim = null;
            return true;
        }

        public override void ClaimOwnership(Sim newOwner, BedData entryPart)
        {
            base.ClaimOwnership(newOwner, entryPart);           
        }
       
        public override int NumberOfSpots()
        {
            return base.PartComp.PartDataList.Count;
        }

        public override Bed.Tuning TuningBed
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region WooHoo
        public class WooHooingForMoney : WooHoo
        {
            // Fields

            private new WooHooPrivacySituation mSituation;
            public static new readonly InteractionDefinition Singleton = new Definition();

            public override string SocialName
            {
                get
                {
                    return "WooHoo"; 
                }
            }

            // Methods
            public override bool BabyMade()
            {
                WooHooConfig whc = CommonMethods.GetWooHooConfig();

                Boolean tryForBaby = whc.pregnancy > 0 ? true : false;

                //If teen pregnacnies are not allowed, check both participants are ya/adults
                if (tryForBaby && !whc.TeenPregnancy)
                {
                    Boolean actorOk = false;
                    if (base.Actor.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Male || (base.Actor.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Female && base.Actor.SimDescription.YoungAdultOrAdult))
                    {
                        actorOk = true;
                    }

                    Boolean targetOk = false;
                    if (base.Target.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Male || (base.Target.SimDescription.Gender == Sims3.SimIFace.CAS.CASAgeGenderFlags.Female && base.Target.SimDescription.YoungAdultOrAdult))
                    {
                        targetOk = true;
                    }

                    if (!actorOk || !targetOk)
                    {
                        tryForBaby = false;
                    }
                }
                //Check sims are of different gender
                if (base.Actor.SimDescription.Gender == base.Target.SimDescription.Gender)
                {
                    tryForBaby = false;
                }
                //StyledNotification.Show(new StyledNotification.Format("tryForBaby: " + tryForBaby + " " + whc.pregnancy + "%", StyledNotification.NotificationStyle.kGameMessagePositive));
                if (CommonMethods.FuckMeUp(whc.pregnancy) && tryForBaby)
                {
                    float babyMadeChance = ((Bed)base.Actor.Posture.Container).BabyMadeChance;
                    return Pregnancy.ShouldImpregnate(base.Actor, base.Target, babyMadeChance);
                }
                else
                {
                    return false;
                }


            }


            public override void Cleanup()
            {
                if (this.isWooHooing)
                {
                    RockGemMetalBase.HandleNearbyWoohoo(base.Actor, RockGemMetalBase.HowMuchWooHoo.LessWoohoo);
                    this.isWooHooing = false;
                }
                AlarmManager.Global.RemoveAlarm(this.mJealousyAlarm);
                if (this.mWooHooEffect != null)
                {
                    this.mWooHooEffect.Stop();
                    this.mWooHooEffect.Dispose();
                    this.mWooHooEffect = null;
                }
                if (this.mReactToSocialBroadcaster != null)
                {
                    this.mReactToSocialBroadcaster.Dispose();
                    this.mReactToSocialBroadcaster = null;
                }
                if (this.mHelperX != null)
                {
                    this.mHelperX.Dispose();
                    this.mHelperX = null;
                }
                if (this.mHelperY != null)
                {
                    this.mHelperY.Dispose();
                    this.mHelperY = null;
                }
                if (this.mbInvisible)
                {
                    this.UpdateVisibility(true);
                }
                this.CleanupSituation();
                base.Cleanup();
            }

            private new void CleanupSituation()
            {
                if (this.mSituation != null)
                {
                    this.mSituation.Exit();
                    this.mSituation = null;
                }
            }

            protected bool Run(Sim client, Sim solicitor)
            {
                bool flag3 = false;
                
                ObjectGuid bedId = new ObjectGuid();

                #region Initialize relationship data

                //Increase the relationship for a moment, so sims can relax on the same bed     
                Relationship rRelationship = Relationship.Get(base.Actor, base.Target, true);
                float originalLTR = 0;
                Boolean changeLTR = false;
                Boolean lover = false;

                Boolean clientHasLFMoodlet = CommonMethods.HasMoodlet(client, BuffNames.LostAFriend, Origin.FromSocialization);
                Boolean soliciterHasLFMoodlet = CommonMethods.HasMoodlet(solicitor, BuffNames.LostAFriend, Origin.FromSocialization);

                #endregion

                try
                {


                    this.Actor.GreetSimOnMyLotIfPossible(this.Target);
                    if (StartBedCuddleA.GetCuddleType(this.Actor, this.Target) == StartBedCuddleA.CuddleType.CuddleTargetOnDifferentBed)
                    {
                        ChildUtils.SetPosturePrecondition(this, CommodityKind.Relaxing, new CommodityKind[]
				{
					CommodityKind.NextToTarget
				});
                        this.Actor.InteractionQueue.PushAsContinuation(this, true);
                        return true;
                    }
                    if (!this.Actor.Posture.Satisfies(CommodityKind.Relaxing, null))
                    {
                        return false;
                    }
                    if (!base.SafeToSync())
                    {
                        return false;
                    }
                    BedMultiPart bedMultiPart = this.Actor.Posture.Container as BedMultiPart;
                    
                    bedId = bedMultiPart.ObjectId;

                    if (this.IsMaster && this.ReturnInstance == null)
                    {
                        base.EnterStateMachine("BedSocials", "FromRelax", "x", "y");
                        base.AddPersistentScriptEventHandler(0u, new SacsEventHandler(this.EventCallbackChangeVisibility));
                        base.SetActor("bed", bedMultiPart);
                        BedData partSimIsIn = bedMultiPart.PartComp.GetPartSimIsIn(this.Actor);
                        partSimIsIn.SetPartParameters(this.mCurrentStateMachine);
                        WooHoo wooHoo = base.InteractionDefinition.CreateInstance(this.Actor, this.Target, base.GetPriority(), false, base.CancellableByPlayer) as WooHoo;
                        wooHoo.IsMaster = false;
                        wooHoo.LinkedInteractionInstance = this;
                        ChildUtils.SetPosturePrecondition(wooHoo, CommodityKind.Relaxing, new CommodityKind[]
				{
					CommodityKind.NextToTarget
				});
                        this.Target.InteractionQueue.AddNext(wooHoo);
                        if (this.Target.Posture.Container != this.Actor.Posture.Container)
                        {
                            this.Actor.LookAtManager.SetInteractionLookAt(this.Target, 200, LookAtJointFilter.TorsoBones);
                            this.Actor.Posture.CurrentStateMachine.RequestState("x", "callOver");
                        }
                        this.Actor.SynchronizationLevel = Sim.SyncLevel.Started;
                        this.Actor.SynchronizationTarget = this.Target;
                        this.Actor.SynchronizationRole = Sim.SyncRole.Initiator;
                        if (!base.StartSync(this.IsMaster))
                        {
                            return false;
                        }
                        if (!this.Actor.WaitForSynchronizationLevelWithSim(this.Target, Sim.SyncLevel.Routed, 30f))
                        {
                            return false;
                        }
                        base.StartSocialContext();
                    }
                    else
                    {
                        if (!base.StartSync(this.IsMaster))
                        {
                            return false;
                        }
                    }
                    base.StandardEntry(false);
                    base.BeginCommodityUpdates();
                    if (this.IsMaster)
                    {
                        if (!bedMultiPart.InherentlyProvidesPrivacy)
                        {
                            this.mSituation = new WooHoo.WooHooPrivacySituation(this);
                            if (!this.mSituation.Start())
                            {
                                base.FinishLinkedInteraction();
                                base.PostLoop();
                                if (this.ReturnInstance == null)
                                {
                                    InteractionInstance instance = BedRelax.Singleton.CreateInstance(this.Actor.Posture.Container, this.Actor, base.GetPriority(), true, true);
                                    this.Actor.InteractionQueue.PushAsContinuation(instance, true);
                                }
                                else
                                {
                                    base.DoResume();
                                }
                                WooHoo wooHoo2 = this.LinkedInteractionInstance as WooHoo;
                                if (wooHoo2 != null)
                                {
                                    if (this.ReturnInstance == null)
                                    {
                                        InteractionInstance instance2 = BedRelax.Singleton.CreateInstance(this.Target.Posture.Container, this.Target, base.GetPriority(), true, true);
                                        this.Target.InteractionQueue.PushAsContinuation(instance2, true);
                                    }
                                    else
                                    {
                                        wooHoo2.DoResume();
                                    }
                                    wooHoo2.Failed = true;
                                }
                                base.WaitForSyncComplete();
                                base.EndCommodityUpdates(false);
                                base.StandardExit(false, false);
                                return false;
                            }
                        }
                        this.Actor.LookAtManager.ClearInteractionLookAt();
                        this.Target.LookAtManager.ClearInteractionLookAt();
                        if (this.ReturnInstance != null)
                        {
                            this.ReturnInstance.EnsureMaster();
                            this.mCurrentStateMachine = this.ReturnInstance.mCurrentStateMachine;
                        }
                        base.StartSocial(this.SocialName);
                        base.InitiateSocialUI(this.Actor, this.Target);
                        this.Rejected = false;
                        (this.LinkedInteractionInstance as WooHoo).Rejected = this.Rejected;
                        if (this.Rejected)
                        {
                            if (this.Actor.Posture.Container == this.Target.Posture.Container)
                            {
                                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_woohoo", "balloon_question");
                                balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                                this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                                base.AddOneShotScriptEventHandler(404u, new SacsEventHandler(this.ShowRejectBalloonAndEnqueueRouteAway));
                                this.mCurrentStateMachine.RequestState(false, "x", "WooHooReject");
                                this.mCurrentStateMachine.RequestState(true, "y", "WooHooReject");
                                this.mCurrentStateMachine.RequestState(true, null, "ToRelax");
                            }
                            this.Actor.BuffManager.AddElement(BuffNames.WalkOfShame, Origin.FromRejectedWooHooOffHome);
                        }
                        else
                        {
                            if (this.BabyMade())
                            {
                                this.mCurrentStateMachine.AddOneShotScriptEventHandler(110u, new SacsEventHandler(this.EventCallbackPlayBabySound));
                            }
                            this.mCurrentStateMachine.AddOneShotScriptEventHandler(110u, new SacsEventHandler(this.EventCallbackChangeClothes));
                            string wooHooEffectName = bedMultiPart.TuningBed.WooHooEffectName;
                            if (!string.IsNullOrEmpty(wooHooEffectName))
                            {
                                this.mWooHooEffect = VisualEffect.Create(wooHooEffectName);
                                this.mWooHooEffect.ParentTo(bedMultiPart, Slots.Hash("_FX_0"));
                                base.AddOneShotScriptEventHandler(200u, new SacsEventHandler(this.EventCallbackWooHoo));
                                base.AddOneShotScriptEventHandler(201u, new SacsEventHandler(this.EventCallbackWooHoo));
                            }
                            this.mHelperX = new Sim.SwitchOutfitHelper(this.Actor, Sim.ClothesChangeReason.GoingToBed);
                            this.mHelperY = new Sim.SwitchOutfitHelper(this.Target, Sim.ClothesChangeReason.GoingToBed);
                            this.mHelperX.Start();
                            this.mHelperY.Start();
                            this.mJealousyAlarm = AlarmManager.Global.AddAlarm(WooHoo.kJealousyBroadcasterDelay, TimeUnit.Minutes, new AlarmTimerCallback(this.StartJealousyBroadcaster), "StartJealousyBroadcaster", AlarmType.DeleteOnReset, bedMultiPart);
                            bedMultiPart.PreWooHooBehavior(this.Actor, this.Target, this);
                            this.mCurrentStateMachine.RequestState(false, "x", "WooHoo");
                            this.mCurrentStateMachine.RequestState(true, "y", "WooHoo");
                            bedMultiPart.PostWooHooBehavior(this.Actor, this.Target, this);
                            Relationship relationship = Relationship.Get(this.Actor, this.Target, true);
                            relationship.STC.Update(this.Actor, this.Target, CommodityTypes.Amorous, WooHoo.kSTCIncreaseAfterWoohoo);
                            InteractionInstance nextInteraction = this.Actor.InteractionQueue.GetNextInteraction();
                            bool flag = nextInteraction != null;
                            bool flag2 = flag && nextInteraction.PosturePreconditions != null && nextInteraction.PosturePreconditions.ContainsPosture(CommodityKind.Sleeping);
                            nextInteraction = this.Target.InteractionQueue.GetNextInteraction();
                            flag = (flag || nextInteraction != null);
                            flag2 = (flag2 && nextInteraction != null && nextInteraction.PosturePreconditions != null && nextInteraction.PosturePreconditions.ContainsPosture(CommodityKind.Sleeping));
                            if ((this.mSituation != null && this.mSituation.SomeoneDidIntrude) || (flag && !flag2))
                            {
                                this.SleepAfter = false;
                            }
                            else
                            {
                                this.SleepAfter = (BedSleep.CanSleep(this.Actor, true) && BedSleep.CanSleep(this.Target, true));
                            }
                            (this.LinkedInteractionInstance as WooHoo).SleepAfter = this.SleepAfter;
                            if (this.SleepAfter)
                            {
                                this.mCurrentStateMachine.RequestState(null, "ToSleep");
                            }
                            else
                            {
                                this.mCurrentStateMachine.RequestState(null, "ToRelax");
                                bedMultiPart.PartComp.GetPartSimIsIn(this.Actor).BedMade = true;
                                bedMultiPart.PartComp.GetPartSimIsIn(this.Target).BedMade = true;
                            }
                            SocialCallback.RunPostWoohoo(this.Actor, this.Target, bedMultiPart);
                            if (bedMultiPart is BedDoubleHover)
                            {
                                this.Actor.BuffManager.AddElement(BuffNames.MeterHighClub, Origin.FromWooHooOnHoverBed);
                                this.Target.BuffManager.AddElement(BuffNames.MeterHighClub, Origin.FromWooHooOnHoverBed);
                            }
                        }
                        base.FinishSocial(this.SocialName, true);
                        this.CleanupSituation();
                        this.Actor.AddExitReason(ExitReason.Finished);
                    }
                    else
                    {
                        bedMultiPart = (this.Target.Posture.Container as BedMultiPart);
                        if (bedMultiPart == null)
                        {
                            return false;
                        }
                        PartComponent<BedData> partComp = bedMultiPart.PartComp;
                        if (partComp.GetSimInOtherPart(this.Target) == null)
                        {
                            BedData otherPart = partComp.GetOtherPart(partComp.GetPartSimIsIn(this.Target));
                            int num;
                            if (!this.Actor.RouteToSlotListAndCheckInUse(bedMultiPart, otherPart.RoutingSlot, out num))
                            {
                                this.Actor.AddExitReason(ExitReason.RouteFailed);
                                return false;
                            }
                            this.Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                            if (this.Rejected)
                            {
                                this.Actor.PlaySoloAnimation("a2a_bed_relax_cuddle_reject_standing_y", true);
                                this.Actor.RouteAway(WooHoo.kMinDistanceToMoveAwayWhenRejected, WooHoo.kMaxDistanceToMoveAwayWhenRejected, true, new InteractionPriority(InteractionPriorityLevel.Zero), false, true, true, RouteDistancePreference.NoPreference);
                                return true;
                            }
                            if (!otherPart.RelaxOnBed(this.Actor, "Enter_BedRelax_" + otherPart.StateNameSuffix))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            this.Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
                        }
                        base.DoLoop(ExitReason.Default);
                        if (!this.Actor.HasExitReason(ExitReason.Finished))
                        {
                            base.PostLoop();
                            base.WaitForMasterInteractionToFinish();
                        }
                    }
                    base.PostLoop();
                    base.WaitForSyncComplete();
                    flag3 = true;// !this.Failed && !this.Rejected;
                    base.EndCommodityUpdates(flag3);
                    base.StandardExit(false, false);

                    if (flag3)
                    {
                        VisitSituation visitSituation = VisitSituation.FindVisitSituationInvolvingGuest(this.Actor);
                        VisitSituation visitSituation2 = VisitSituation.FindVisitSituationInvolvingGuest(this.Target);
                        if (visitSituation != null && visitSituation2 != null)
                        {
                            visitSituation.GuestStartingInappropriateAction(this.Actor, 3.5f);
                            visitSituation2.GuestStartingInappropriateAction(this.Target, 3.5f);
                        }
                    }
                    if (flag3 && this.SleepAfter)
                    {
                        bedMultiPart.GetPartContaining(this.Actor).StateMachine = null;
                        if (!this.Actor.InteractionQueue.HasInteractionOfType(BedSleep.Singleton))
                        {
                            InteractionInstance instance3 = BedSleep.Singleton.CreateInstance(bedMultiPart, this.Actor, base.GetPriority(), base.Autonomous, base.CancellableByPlayer);
                            this.Actor.InteractionQueue.PushAsContinuation(instance3, true);
                        }
                        VisitSituation visitSituation3 = VisitSituation.FindVisitSituationInvolvingGuest(this.Target);
                        if (visitSituation3 != null && this.Actor.IsAtHome)
                        {
                            SocialCallback.OnStayOver(this.Actor, this.Target, false);
                        }
                        else
                        {
                            visitSituation3 = VisitSituation.FindVisitSituationInvolvingGuest(this.Actor);
                            if (visitSituation3 != null && this.Target.IsAtHome)
                            {
                                SocialCallback.OnStayOver(this.Target, this.Actor, false);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    StyledNotification.Show(new StyledNotification.Format(ex.ToString(), StyledNotification.NotificationStyle.kGameMessageNegative));

                }
                finally
                {

                    try
                    {

                        if (flag3)
                        {
                            #region Ge Payed and such
                                              
                             CommonMethods.HandlePostWoohoo(client, solicitor, bedId);

                            #endregion
                        }
                        #region Restore LTR

                        if (rRelationship != null && changeLTR)
                        {
                            rRelationship.LTR.SetLiking(originalLTR);
                            //Remove the lost friend moodlets if we didn't have them before, but have them now
                            if (!clientHasLFMoodlet && CommonMethods.HasMoodlet(client, BuffNames.LostAFriend, Origin.FromSocialization))
                            {
                                client.BuffManager.RemoveElement(BuffNames.LostAFriend);
                            }
                            if (!soliciterHasLFMoodlet && CommonMethods.HasMoodlet(solicitor, BuffNames.LostAFriend, Origin.FromSocialization))
                            {
                                solicitor.BuffManager.RemoveElement(BuffNames.LostAFriend);
                            }

                            //If we were not lovers, before but are now, remove the icon
                            if (lover)
                            {
                                rRelationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Romantic);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StyledNotification.Show(new StyledNotification.Format("finally: " + ex.Message, StyledNotification.NotificationStyle.kGameMessageNegative));
                    }


                        #endregion
                }
                return flag3;
               
            }

            private void SetBedData(BedData bd)
            {
                // container.PartComp.GetPartSimIsIn(base.Actor).SetPartParameters(base.mCurrentStateMachine);
                PartArea area = bd.Area;
                switch (area)
                {
                    case PartArea.Left:
                        bd.StateNameSuffix = "Route0";
                        bd.IkSuffix = "";
                        bd.IsMirrored = false;
                        bd.AnimationPriority = AnimationPriority.kAPNormal;
                        break;

                    case PartArea.Right:
                        bd.StateNameSuffix = "Route1";
                        bd.IkSuffix = "";
                        bd.IsMirrored = true;
                        bd.AnimationPriority = AnimationPriority.kAPNormalPlus;
                        break;
                }
                base.mCurrentStateMachine.SetParameter("IsMirrored", bd.IsMirrored);

                base.mCurrentStateMachine.SetParameter("IkSuffix", bd.IkSuffix);
                base.mCurrentStateMachine.SetParameter("IsBedMade", bd.BedMade);
                base.mCurrentStateMachine.SetParameter("IsTent", false);
            }

            public override bool Test()
            {
                return true;
            }

        }

        #endregion

        public override BedData[] GetBedData()
        {
            throw new NotImplementedException();
        }
    }
}
