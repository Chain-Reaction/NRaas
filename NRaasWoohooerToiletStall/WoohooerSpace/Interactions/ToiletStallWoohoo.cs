using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.WoohooerSpace.Interactions
{
	public class ToiletStallWooHoo : Interaction<Sim, ToiletStall>, Common.IPreLoad, Common.IAddInteraction
	{
		public class ProxyDefinition : CommonWoohoo.PrimaryProxyDefinition<ToiletStall, ToiletStallWooHoo, ToiletStallWooHoo.BaseDefinition>
		{
			public bool Makeout
			{
				get
				{
					return base.Definition.Makeout;
				}
			}
			public ProxyDefinition(ToiletStallWooHoo.BaseDefinition definition) : base(definition)
			{
			}
		}
		public abstract class BaseDefinition : CommonWoohoo.PotentialDefinition<ToiletStall>
		{
			public virtual bool Makeout
			{
				get
				{
					return false;
				}
			}
			public BaseDefinition()
			{
			}
			public BaseDefinition(Sim target) : base(target)
			{
			}
			public override CommonWoohoo.WoohooLocation GetLocation(IGameObject obj)
			{
				return CommonWoohoo.WoohooLocation.ToiletStall;
			}
			public override void PushWooHoo(Sim actor, Sim target, IGameObject obj)
			{
				ToiletStall target2 = obj as ToiletStall;
				InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.UserDirected);
				ToiletStallWooHoo wooHoo = new ToiletStallWooHoo.ProxyDefinition(this).CreateInstance(target2, actor, priority, false, true) as ToiletStallWooHoo;
				wooHoo.WooHooer = actor;
				wooHoo.WooHooee = target;
				actor.InteractionQueue.PushAsContinuation(wooHoo, true);
			}
			protected override bool Satisfies(Sim actor, Sim target, ToiletStall obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
			{
				return LocationControl.TestAccessible(obj, actor) && LocationControl.TestAccessible(obj, target);
			}
		}
		public class MakeOutDefinition : ToiletStallWooHoo.BaseDefinition
		{
			public override bool Makeout
			{
				get
				{
					return true;
				}
			}
			public override bool PushSocial
			{
				get
				{
					return false;
				}
			}
			public MakeOutDefinition()
			{
			}
			public MakeOutDefinition(Sim target) : base(target)
			{
			}
			public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
			{
				return CommonWoohoo.WoohooStyle.Safe;
			}
			public override string GetInteractionName(Sim actor, ToiletStall target, InteractionObjectPair iop)
			{
				return SocialCallback.PartyAnimalFunction (null, actor, null, null);
			}
			protected override bool Satisfies(Sim actor, Sim target, ToiletStall obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
			{
				return base.Satisfies (actor, target, obj, isAutonomous, ref callback) && CommonSocials.SatisfiesRomance(actor, target, "ToiletStallMakeOut ", isAutonomous, ref callback);
			}
			public override InteractionDefinition ProxyClone(Sim target)
			{
				return new ToiletStallWooHoo.ProxyDefinition(new ToiletStallWooHoo.MakeOutDefinition(target));
			}
		}
		public class SafeDefinition : ToiletStallWooHoo.BaseDefinition
		{
			public SafeDefinition()
			{
			}
			public SafeDefinition(Sim target) : base(target)
			{
			}
			public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
			{
				return CommonWoohoo.WoohooStyle.Safe;
			}
			public override string GetInteractionName(Sim actor, ToiletStall target, InteractionObjectPair iop)
			{
				return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasWooHoo", new object[0]);
			}
			protected override bool Satisfies(Sim actor, Sim target, ToiletStall obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
			{
				return base.Satisfies(actor, target, obj, isAutonomous, ref callback) && CommonWoohoo.SatisfiesWoohoo(actor, target, "ToiletStallWooHoo", isAutonomous, true, true, ref callback);
			}
			public override InteractionDefinition ProxyClone(Sim target)
			{
				return new ToiletStallWooHoo.ProxyDefinition(new ToiletStallWooHoo.SafeDefinition(target));
			}
		}
		public class RiskyDefinition : ToiletStallWooHoo.BaseDefinition
		{
			public RiskyDefinition()
			{
			}
			public RiskyDefinition(Sim target) : base(target)
			{
			}
			public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
			{
				return CommonWoohoo.WoohooStyle.Risky;
			}
			public override string GetInteractionName(Sim actor, ToiletStall target, InteractionObjectPair iop)
			{
				return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasRiskyWooHoo", new object[0]) + Common.LocalizeEAString(false, "NRaas.Woohooer:RiskyChance", new object[]
					{
						Woohooer.Settings.GetRiskyBabyMadeChance(actor)
					});
			}
			protected override bool Satisfies(Sim actor, Sim target, ToiletStall obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
			{
				return base.Satisfies(actor, target, obj, isAutonomous, ref callback) && CommonPregnancy.SatisfiesRisky(actor, target, "ToiletStallRisky", isAutonomous, true, ref callback);
			}
			public override InteractionDefinition ProxyClone(Sim target)
			{
				return new ToiletStallWooHoo.ProxyDefinition(new ToiletStallWooHoo.RiskyDefinition(target));
			}
		}
		public class TryForBabyDefinition : ToiletStallWooHoo.BaseDefinition
		{
			public TryForBabyDefinition()
			{
			}
			public TryForBabyDefinition(Sim target) : base(target)
			{
			}
			public override CommonWoohoo.WoohooStyle GetStyle(InteractionInstance paramInteraction)
			{
				return CommonWoohoo.WoohooStyle.TryForBaby;
			}
			public override string GetInteractionName(Sim actor, ToiletStall target, InteractionObjectPair iop)
			{
				return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Excel/Socializing/Action:NRaasTryForBaby", new object[0]);
			}
			protected override bool Satisfies(Sim actor, Sim target, ToiletStall obj, bool isAutonomous, ref GreyedOutTooltipCallback callback)
			{
				return base.Satisfies(actor, target, obj, isAutonomous, ref callback) && CommonPregnancy.SatisfiesTryForBaby(actor, target, "ToiletStallTryForBaby", isAutonomous, true, ref callback);
			}
			public override InteractionDefinition ProxyClone(Sim target)
			{
				return new ToiletStallWooHoo.ProxyDefinition(new ToiletStallWooHoo.TryForBabyDefinition(target));
			}
		}
		public class LocationControl : WoohooLocationControl
		{
			public override CommonWoohoo.WoohooLocation Location
			{
				get
				{
					return CommonWoohoo.WoohooLocation.ToiletStall;
				}
			}
			public override bool Matches(IGameObject obj)
			{
				return obj is ToiletStall;
			}
			public override bool HasWoohooableObject(Lot lot)
			{
				return lot.GetObjects<ToiletStall>(new Predicate<ToiletStall>(TestUse)).Count > 0;
			}
			public override bool HasLocation(Lot lot)
			{
				return lot.CountObjects<ToiletStall>() > 0u;
			}
			public override bool AllowLocation(SimDescription sim, bool testVersion)
			{
				return sim.IsHuman && Woohooer.Settings.mAutonomousToiletStall;
			}
			public bool TestUse(ToiletStall obj)
			{
				return obj.InWorld && WoohooLocationControl.TestRepaired(obj) && obj.UseCount == 0;
			}
			public override List<GameObject> GetAvailableObjects(Sim actor, Sim target, ItemTestFunction testFunc)
			{
				List<GameObject> list = new List<GameObject>();
				foreach (ToiletStall current in actor.LotCurrent.GetObjects<ToiletStall>(new Predicate<ToiletStall>(TestUse)))
				{
					if ((testFunc == null || testFunc(current, null)) && TestAccessible(current, actor) && TestAccessible(current, target))
					{
						list.Add(current);
					}
				}
				return list;
			}
			public override InteractionDefinition GetInteraction(Sim actor, Sim target, CommonWoohoo.WoohooStyle style)
			{
				switch (style)
				{
				case CommonWoohoo.WoohooStyle.Safe:
					return new ToiletStallWooHoo.SafeDefinition(target);
				case CommonWoohoo.WoohooStyle.Risky:
					return new ToiletStallWooHoo.RiskyDefinition(target);
				case CommonWoohoo.WoohooStyle.TryForBaby:
					return new ToiletStallWooHoo.TryForBabyDefinition(target);
				default:
					return null;
				}
			}

			public static bool TestAccessible(Toilet obj, Sim sim)
			{
				int roomId = obj.RoomId;
				if (sim.RoomId == roomId)
				{
					return true;
				}
				foreach(CommonDoor door in obj.LotCurrent.GetObjectsInRoom<CommonDoor>(roomId))
				{
					if (door.IsAllowedThrough (sim))
					{
						return true;
					}
				}
				return false;
			}
		}
		private static readonly InteractionDefinition MakeOutSingleton = new ToiletStallWooHoo.MakeOutDefinition();
		private static readonly InteractionDefinition SafeSingleton = new ToiletStallWooHoo.SafeDefinition();
		private static readonly InteractionDefinition RiskySingleton = new ToiletStallWooHoo.RiskyDefinition();
		private static readonly InteractionDefinition TryForBabySingleton = new ToiletStallWooHoo.TryForBabyDefinition();
		//public bool isMaster;
		public Sim WooHooer;
		public Sim WooHooee;
		public bool isWooHooing;
		public bool isSitting;
		public VisualEffect mWooHooEffect;
		public void AddInteraction(Common.InteractionInjectorList interactions)
		{
			interactions.Add<ToiletStall>(ToiletStallWooHoo.MakeOutSingleton);
			interactions.Add<ToiletStall>(ToiletStallWooHoo.SafeSingleton);
			interactions.Add<ToiletStall>(ToiletStallWooHoo.RiskySingleton);
			interactions.Add<ToiletStall>(ToiletStallWooHoo.TryForBabySingleton);
		}
		public void OnPreLoad()
		{
			Woohooer.InjectAndReset<ToiletStall, ToiletStallWooHoo.SafeDefinition, ToiletStallWooHoo.MakeOutDefinition>(true);
			Woohooer.InjectAndReset<ToiletStall, ToiletStallWooHoo.SafeDefinition, ToiletStallWooHoo.RiskyDefinition>(true);
			Woohooer.InjectAndReset<ToiletStall, ToiletStallWooHoo.SafeDefinition, ToiletStallWooHoo.TryForBabyDefinition>(true);
		}
		public override string GetInteractionName()
		{
			if (Actor == WooHooee)
			{
				return Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Excel/Socializing/Action" + (((ProxyDefinition)base.InteractionDefinition).Makeout ? "Names:MakeOut" : ":BeWooHood"), new object[0]);
			}
			return base.GetInteractionName();
		}
		public override bool Run()
		{
			bool result = false;
			try
			{
				ProxyDefinition proxyDefinition = base.InteractionDefinition as ProxyDefinition;
				Vector3 slotPosition = Target.GetSlotPosition(Slot.RoutingSlot_0);
				if (Actor == WooHooer && !Actor.HasExitReason())
				{
					if (!Target.SimLine.WaitForTurn(this, Actor, SimQueue.WaitBehavior.CutAheadOfLowerPrioritySims, ExitReason.Default, Toilet.kTimeToWaitInLine))
					{
						return result;
					}
					//float value1 = Actor.Motives.GetMotiveValue(CommodityKind.Bladder);
					//float value2 = WooHooee.Motives.GetMotiveValue(CommodityKind.Bladder);
					isSitting = (Actor.Position - slotPosition).Length() < (WooHooee.Position - slotPosition).Length(); //(value1 < Math.Min(0, value2)) || (value2 >= 0 && RandomUtil.CoinFlip());
					ToiletStallWooHoo wooHoo = proxyDefinition.ProxyClone(WooHooee).CreateInstance(Target, WooHooee, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as ToiletStallWooHoo;
					wooHoo.LinkedInteractionInstance = this;
					wooHoo.WooHooer = WooHooer;
					wooHoo.WooHooee = WooHooee;
					wooHoo.isSitting = !isSitting;
					WooHooee.InteractionQueue.AddNext(wooHoo);
				}
				if (base.StartSync(!isSitting))
				{
					if (!isSitting)
					{
						Actor.RouteToPointRadius(slotPosition, 1f);
					}
					if ((isSitting || Actor.WaitForSynchronizationLevelWithSim (LinkedInteractionInstance.InstanceActor, Sim.SyncLevel.Routed, 30f)) && Actor.RouteToSlot(Target, Slot.RoutingSlot_0))
					{
						Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
						base.StandardEntry();
						StateMachineClient stateMachine = Target.GetStateMachine(Actor);

						if (SitDownAndWait(stateMachine))
						{
							BuffInstance element =  Actor.BuffManager.GetElement(BuffNames.ReallyHasToPee);
							if (element != null)
							{
								element.mTimeoutPaused = true;
							}
							base.BeginCommodityUpdates();
							isWooHooing = !proxyDefinition.Makeout;
							bool skipFlush = false;
							if (isSitting)
							{
								Actor.EnableCensor (Sim.CensorType.FullHeight);
								base.FinishLinkedInteraction(false);
								skipFlush = !RelieveSelf(element);
								base.WaitForSyncComplete();
								Actor.AutoEnableCensor();
								Actor.BuffManager.UnpauseBuff (BuffNames.ImaginaryFriendFeelOfPorcelain);
							}
							else
							{
								string stateName = RandomUtil.GetRandomStringFromList (new string[]{ "peeStanding", "clean" });
								stateMachine.RequestState("x", stateName);
								Actor.EnableCensor (Sim.CensorType.FullHeight);
								TurnOnWooHooFx ();
								base.DoTimedLoop (RandomUtil.RandomFloatGaussianDistribution(5f, isWooHooing ? 15f : 10f));
								if (!isWooHooing)
								{
									EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, WooHooer, WooHooee, "Make Out", false, true, false, CommodityTypes.Undefined));
									EventTracker.SendEvent(new SocialEvent(EventTypeId.kSocialInteraction, WooHooee, WooHooer, "Make Out", true, true, false, CommodityTypes.Undefined));
								}
								else
								{
									EventTracker.SendEvent(EventTypeId.kWooHooed, WooHooer, Target);
									EventTracker.SendEvent(EventTypeId.kWooHooed, WooHooee, Target);
									CommonWoohoo.RunPostWoohoo(WooHooer, WooHooee, Target, proxyDefinition.GetStyle(this), proxyDefinition.GetLocation(Target), true);
									if (CommonPregnancy.IsSuccess(WooHooer, WooHooee, base.Autonomous, proxyDefinition.GetStyle(this)))
									{
										CommonPregnancy.Impregnate(WooHooer, WooHooee, base.Autonomous, proxyDefinition.GetStyle(this));
									}
									WooHooer.BuffManager.AddElement(BuffNames.PublicWooHoo, Origin.FromWooHooInPublic);
									WooHooee.BuffManager.AddElement(BuffNames.PublicWooHoo, Origin.FromWooHooInPublic);
								}
								if (stateName == "clean")
								{
									stateMachine.RequestState("x", "putDown");
								}
								RelieveSelf(element);
								base.EndCommodityUpdates(true);
								LinkedInteractionInstance.EndCommodityUpdates(true);
							}
							isWooHooing = false;
							Target.Cleanable.DirtyInc(Actor);
							bool autoFlush = !isSitting || Target.ToiletTuning.AutoFlushes;
							if (autoFlush || (!skipFlush && Target.ShouldFlush(Actor, base.Autonomous)))
							{
								Target.FlushToilet(Actor, stateMachine, !autoFlush);
							}
							if (Target.ShouldWashHands(Actor))
							{
								Sink sink = Toilet.FindClosestSink(Actor);
								if (sink != null)
								{
									InteractionInstance interactionInstance = Sink.WashHands.Singleton.CreateInstance(sink, Actor, base.GetPriority(), false, true);
									Actor.InteractionQueue.PushAsContinuation(interactionInstance, false);
								}
							}
							result = true;
						}
						TurnOffCensorsAndFx();
						stateMachine.RequestState("x", "Exit");
						if (!isSitting)
						{
							base.FinishLinkedInteraction(true);
							if (result)
							{
								Relationship.Get(WooHooer, WooHooee, true).LTR.UpdateLiking(AllInOneBathroom.kLTRGainFromWoohooInAllInOneBathroom);
							}
							Actor.RouteAway(1f, 2f, false, new InteractionPriority(InteractionPriorityLevel.UserDirected), true, false, true, RouteDistancePreference.PreferFurthestFromRouteOrigin);
						}
						base.StandardExit();
					}
				}
			}
			catch (ResetException)
			{
				throw;
			}
			catch (Exception exception)
			{
				Common.Exception(Actor, Target, exception);
				result = false;
			}
			return result;
		}
		public override void Cleanup()
		{
			if (base.StandardEntryCalled && isWooHooing)
			{
				isWooHooing = false;
			}
			TurnOffCensorsAndFx ();
			base.Cleanup();
		}
		public bool RelieveSelf(BuffInstance element)
		{
			if (element != null)
			{
				element.mTimeoutPaused = false;
			}
			bool flag = false;
			Motive motive = Actor.Motives.GetMotive(CommodityKind.Bladder);
			if (motive != null && (element != null || Actor.BuffManager.HasAnyElement(new BuffNames[]{ BuffNames.HasToPee, BuffNames.ReallyHasToPee })))
			{
				motive.PotionBladderDecayOverride = false;
				Actor.Motives.SetValue (CommodityKind.Bladder, motive.GetIdealMotiveValue());
				Target.ToiletVolume++;
				flag = true;
			}
			if (!isSitting || flag)
			{
				SimClockUtils.SleepForNSimMinutes (Toilet.kMaxLengthUseToilet);
			}
			return flag;
		}
		public bool SitDownAndWait (StateMachineClient stateMachine)
		{
			if (isSitting)
			{
				stateMachine.SetParameter("isDirty", Target.Cleanable.NeedsToBeCleaned);
				Target.PutDownSeat(stateMachine);
				stateMachine.RequestState("x", "peeSitting");
				if (Target.SculptureComponent != null && Target.SculptureComponent.Material == Sims3.Gameplay.ObjectComponents.SculptureComponent.SculptureMaterial.Ice)
				{
					Actor.BuffManager.AddElement(BuffNames.Chilly, Origin.FromSittingOnIce);
				}
				if (Target.ToiletTuning.AutoFlushes && RandomUtil.RandomChance((float)Toilet.kChanceOfToiletAutoFlushWhileInUse))
				{
					stateMachine.RequestState("x", "flushReaction");
				}
				OccultImaginaryFriend.GrantMilestoneBuff(Actor, BuffNames.ImaginaryFriendFeelOfPorcelain, Origin.FromImaginaryFriendFirstTime, true, true, false);

				if (!Actor.WaitForSynchronizationLevelWithSim(LinkedInteractionInstance.InstanceActor, Sim.SyncLevel.Routed, 30f))
				{
					FinishLinkedInteraction();
					return false;
				}
			}
			return true;
		}
		public void TurnOnWooHooFx()
		{
			if (mWooHooEffect == null)
			{
				mWooHooEffect = VisualEffect.Create("bedwoohoopetals");
				mWooHooEffect.ParentTo(Target, Target.TrapFxSlot);
				mWooHooEffect.Start();
			}
		}
		public void TurnOffCensorsAndFx()
		{
			if (mWooHooEffect != null)
			{
				mWooHooEffect.Stop();
				mWooHooEffect.Dispose();
				mWooHooEffect = null;
			}
			Actor.AutoEnableCensor ();
		}
	}
}
