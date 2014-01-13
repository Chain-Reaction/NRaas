using NRaas.CommonSpace.Helpers;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TravelerSpace.Interactions
{
    public class AncientPortalTimeTravelEx : AncientPortal.CatchABeam, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<AncientPortal, AncientPortal.CatchABeam.Definition, Definition>(true);
            if (tuning != null)
            {
                tuning.SetFlags(InteractionTuning.FlagField.DisallowAutonomous, true);
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<AncientPortal>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                bool succeeded = true;
                Target.mRoutingSims.Add(Actor);
                if (!Target.RouteToPortal(Actor))
                {
                    Actor.AddExitReason(ExitReason.RouteFailed);
                    Target.mRoutingSims.Remove(Actor);
                    return false;
                }

                Target.mRoutingSims.Remove(Actor);

                if (!TimePortalTravelEx.PreTimeTravel1(this)) return false;

                mTargetPortal = Target;

                mTargetPortal.AddToUseList(Actor);

                if (!TimePortalTravelEx.PreTimeTravel2(this)) return false;

                mTargetPortal.EnableFootprint(FootprintPlacementHash);
                mTargetPortal.PushSimsFromFootprint(FootprintPlacementHash, Actor, null, false);
                EnterStateMachine("AncientPortal", "Enter", "x", "portal");

                AddOneShotScriptEventHandler(0x65, HideSim);
                AddOneShotScriptEventHandler(0x66, ShowSim);
                AnimateSim("InsidePortal");

                SetActor("portal", mTargetPortal);

                if (!GameUtils.IsFutureWorld())
                {
                    // Custom
                    succeeded = TimePortalEx.TravelToFuture(null, Actor, new List<Sim>(), new List<ulong>());
                }

                Slot slotName = mTargetPortal.GetRoutingSlots()[0x0];
                Vector3 positionOfSlot = mTargetPortal.GetPositionOfSlot(slotName);
                Vector3 forwardOfSlot = mTargetPortal.GetForwardOfSlot(slotName);
                Actor.SetPosition(positionOfSlot);
                Actor.SetForward(forwardOfSlot);
                mTargetPortal.DisableFootprint(FootprintPlacementHash);

                if (!TimePortalTravelEx.PostTimeTravel1(this, succeeded)) return false;

                AnimateSim("Exit");

                for (int i = 0x0; i < kPotentialTravelBuffs.Length; i++)
                {
                    if (RandomUtil.RandomChance(kChanceForEachBuff))
                    {
                        Actor.BuffManager.AddElement(kPotentialTravelBuffs[i], Origin.FromAncientPortal);
                    }
                }

                EndCommodityUpdates(succeeded);
                StandardExit();
                mTargetPortal.RemoveFromUseList(Actor);

                Actor.Wander(1f, 2f, false, RouteDistancePreference.PreferNearestToRouteOrigin, false);

                if (GameUtils.IsFutureWorld())
                {
                    EventTracker.SendEvent(EventTypeId.kTravelToPresent, Actor);

                    // Custom
                    GameStatesEx.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
                }

                return succeeded;
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
            finally
            {
                TravelUtil.PlayerMadeTravelRequest = false;
            }
        }

        public new class Definition : AncientPortal.CatchABeam.Definition
        {
            public override string GetInteractionName(Sim actor, AncientPortal target, InteractionObjectPair iop)
            {
                if (GameUtils.IsFutureWorld())
                {
                    return TimePortal.LocalizeString("TravelHome", new object[0x0]);
                }
                return TimePortal.LocalizeString("Travel", new object[0x0]);
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new AncientPortalTimeTravelEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim actor, AncientPortal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP11))
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Pack Fail");
                    return false;
                }

                return TimePortalTravelEx.Definition.PublicTest(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
