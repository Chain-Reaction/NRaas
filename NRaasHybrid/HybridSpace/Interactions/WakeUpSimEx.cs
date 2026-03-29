using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.HybridSpace.Interactions
{
    public class WakeUpSimEx : Sim.WakeUpSim, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.WakeUpSim.Definition, Definition>(false);

            sOldSingleton = Sim.WakeUpSim.Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.WakeUpSim.Definition>(Singleton);
        }

        public override bool Run()
        {
			mBed = Target.Posture.Container as IBed;

			PartData partData = null;
			PartData partSimIsIn = null;
			Sim simInOtherPart = null;
			bool paramValue = true;
			bool paramValue2 = true;

			if (!(mBed is FairyHouse))
			{ 
				partSimIsIn = mBed.PartComponent.GetPartSimIsIn(Target);
				simInOtherPart = mBed.PartComponent.GetSimInOtherPart(Target);
				if (Actor.SimDescription.YoungAdultOrAbove && !(mBed is IBedSingle) && simInOtherPart == null && !mBed.IsTent)
				{
					partData = mBed.PartComponent.GetOtherPart(partSimIsIn);
				}
			}

			bool flag = true;
			if (partData != null)
			{
				mBedEntryJig = GlobalFunctions.CreateObjectOutOfWorld("dogJumpOnBed", ProductVersion.EP5) as Jig;
				if (mBedEntryJig == null)
				{
					return false;
				}
				Vector3 positionOfSlot = partData.Container.GetPositionOfSlot(partData.RoutingSlot[0]);
				Vector3 forwardVector = mBed.ForwardVector;
				TestObjectPlacementBooleans options = TestObjectPlacementBooleans.TemporaryPlacement | TestObjectPlacementBooleans.AllowInFrontOfDoors | TestObjectPlacementBooleans.AllowTunnelPortalIntersection | TestObjectPlacementBooleans.IgnoreDiscouragementFootprints | TestObjectPlacementBooleans.AllowOnStairTopAndBottomTiles | TestObjectPlacementBooleans.AllowOffLot;
				ObjectGuid[] objIdsToIgnore = new ObjectGuid[2] { Actor.ObjectId, mBed.ObjectId };
				if (World.TestObjectPlacement(mBedEntryJig.ObjectId, options, objIdsToIgnore, positionOfSlot, forwardVector))
				{
					flag = false;
					mBedEntryJig.SetPosition(positionOfSlot);
					mBedEntryJig.SetForward(forwardVector);
					mBedEntryJig.AddToWorld();
					mBedEntryJig.SetOpacity(0f, 0f);
					if (partData.Area == PartArea.Left)
					{
						mRoutingSlot = Slot.RoutingSlot_1;
					}
					Route route = Actor.CreateRoute();
					route.AddObjectToIgnoreForRoute(mBedEntryJig.ObjectId);
					route.PlanToSlot(mBedEntryJig, mRoutingSlot);
					if (!Actor.DoRoute(route))
					{
						return false;
					}
					if (!Target.IsSleeping)
					{
						return false;
					}
					if (partData.ContainedSim != null)
					{
						CleanUpBedEntryJig();
						partData = null;
						flag = true;
					}
					else
					{
						partData.SetContainedSim(Actor, mBed.PartComponent);
						if (partData.Area == PartArea.Left)
						{
							mContainmentSlot = Slot.ContainmentSlot_1;
							paramValue = false;
						}
						paramValue2 = false;
					}
				}
			}

			int slotIndex = 0;
			if (flag)
			{
				if (!(mBed is FairyHouse))
				{
					Route route2 = Actor.CreateRoute();
					route2.PlanToSlot(partSimIsIn.RouteTarget, partSimIsIn.RoutingSlot[0]);
					if (!Actor.DoRoute(route2))
					{
						return false;
					}
				} else
                {
					FairyHouse house = mBed as FairyHouse;
					if (!Actor.RouteToSlotList(house, house.UseableSlots(Actor, false, true), out slotIndex))
					{
						return false;
					}
					house.SetRoutingSlotInUse(slotIndex);

					if(house.mActorsUsingMe.Count > 1)
                    {
						simInOtherPart = RandomUtil.GetRandomObjectFromList<Sim>(house.mActorsUsingMe);
                    }
				}
			}

			EnterStateMachine("PetWakeUpSim", "Enter", "x");
			mCurrentStateMachine.SetParameter("IsMirrored", paramValue);
			mCurrentStateMachine.SetParameter("IsOnFloor", paramValue2);
			mCurrentStateMachine.AddPersistentSynchronousScriptEventHandler(160u, OnAnimationEvent);
			mCurrentStateMachine.AddPersistentSynchronousScriptEventHandler(102u, OnAnimationEvent);

			if (!flag)
			{
				mBed.AddToUseList(Actor);
				AnimateSim("Jump Up");
			}

			mCurrentStateMachine.SetParameter("isAggressivePet", Actor.TraitManager.HasElement(TraitNames.AggressivePet));
			AnimateSim("Make Noise");
			WakeTargetUp(simInOtherPart);

			if (flag)
			{
				AnimateSim("Floor Idle");
			}
			else
			{
				AnimateSim("Bed Idle");
			}

			Actor.WaitForExitReason(kPetWaitTimeInMinutes, ExitReason.UserCanceled);

			if (!flag)
			{
				AnimateSim("Jump Down");
				mBed.RemoveFromUseList(Actor);
				if (mBedEntryJig != null)
				{
					mBedEntryJig.Destroy();
					mBedEntryJig = null;
				}
				partData.SetContainedSim(null, mBed.PartComponent);
			}

			AnimateSim("Exit");

			if (flag)
			{
				Actor.Wander(2f, 3f, RouteDistancePreference.PreferNearestToRouteOrigin, new int[1] { Actor.RoomId }, false);

				if((mBed is FairyHouse))
                {
					FairyHouse house = mBed as FairyHouse;
					house.SetRoutingSlotUnused(slotIndex);
				}
			}

			if (base.Autonomous)
			{
				if (Actor.IsADogSpecies)
				{
					Actor.SimDescription.DogManager.LastAutonomousWakeUpSimTime = SimClock.CurrentTime();
				}
				else
				{
					Actor.SimDescription.CatManager.LastAutonomousWakeUpSimTime = SimClock.CurrentTime();
				}
			}

			return true;
		}

        public new class Definition : Sim.WakeUpSim.Definition
        {
            public Definition()
            { }

			public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
			{
				InteractionInstance na = new WakeUpSimEx();
				na.Init(ref parameters);
				return na;
			}

			public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
		}
	}
}
