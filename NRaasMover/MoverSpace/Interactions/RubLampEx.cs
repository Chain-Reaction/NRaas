using NRaas.CommonSpace.Helpers;
using NRaas.MoverSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Rewards;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;

namespace NRaas.MoverSpace.Interactions
{
    public class RubLampEx : GenieLamp.RubLamp, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GenieLamp, GenieLamp.RubLamp.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<GenieLamp, GenieLamp.RubLamp.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                if (!Actor.Inventory.Contains(Target))
                {
                    if (!Actor.RouteToObjectRadiusAndCheckInUse(Target, 0.7f))
                    {
                        return false;
                    }

                    StandardEntry();
                    BeginCommodityUpdates();
                    Actor.PlaySoloAnimation("a2o_object_genericSwipe_x", true);
                    if (!Actor.Inventory.TryToAdd(Target))
                    {
                        EndCommodityUpdates(false);
                        StandardExit();
                        return false;
                    }
                }
                else
                {
                    StandardEntry();
                    BeginCommodityUpdates();
                }

                SocialJigTwoPerson person = GlobalFunctions.CreateObjectOutOfWorld(SocialJig.SocialJigMedatorNames.SocialJigTwoPerson.ToString()) as SocialJigTwoPerson;
                person.RegisterParticipants(Actor, null);
                Sim createdSim = null;
                try
                {
                    World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(Actor.Position);
                    fglParams.BooleanConstraints |= FindGoodLocationBooleans.Routable;
                    if (GlobalFunctions.PlaceAtGoodLocation(person, fglParams, true))
                    {
                        Route r = Actor.CreateRoute();
                        r.PlanToSlot(person, person.GetSlotForActor(Actor, true));
                        r.DoRouteFail = true;
                        if (Actor.DoRoute(r))
                        {
                            bool paramValue = false;
                            mSummonGenieBroadcast = new ReactionBroadcaster(Actor, kSummonGenieBroadcastParams, SummonGenieBroadcastCallback);
                            Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.Gameplay.Gameflow.GameSpeed.Normal, Sims3.Gameplay.Gameflow.SetGameSpeedContext.Gameplay);
                            if (Target.mGenieDescription == null)
                            {
                                Target.mGenieDescription = OccultGenie.CreateGenie(Actor, Target);
                                createdSim = Target.mGenieDescription.CreatedSim;
                                EventTracker.SendEvent(EventTypeId.kCleanLamp, Actor, Target);
                                paramValue = true;
                            }
                            else
                            {
                                createdSim = Target.mGenieDescription.Instantiate(Vector3.OutOfWorld, false);
                                OccultGenie occultType = null;
                                DateAndTime previousDateAndTime = SimClock.CurrentTime();
                                do
                                {
                                    SpeedTrap.Sleep(0xa);
                                    occultType = createdSim.OccultManager.GetOccultType(OccultTypes.None | OccultTypes.Genie) as OccultGenie;
                                }
                                while ((occultType == null) && (SimClock.ElapsedTime(TimeUnit.Minutes, previousDateAndTime) < 120f));
                                
                                if (occultType != null)
                                {
                                    occultType.SetGenieLampRelations(Actor, createdSim, Target);
                                }
                                else
                                {
                                    createdSim.Destroy();
                                    createdSim = null;
                                }
                            }

                            if (createdSim != null)
                            {
                                createdSim.FadeOut(false, false, 0f);
                                createdSim.GreetSimOnLot(Actor.LotCurrent);
                                createdSim.AddToWorld();
                                Slot slotForActor = person.GetSlotForActor(createdSim, false);
                                createdSim.SetPosition(person.GetSlotPosition(slotForActor));
                                createdSim.SetForward(person.GetForwardOfSlot(slotForActor));
                                IGameObject actor = GlobalFunctions.CreateObject("GenieLamp", ProductVersion.EP6, Vector3.OutOfWorld, 0x0, Vector3.UnitZ, null, null);
                                if (!actor.IsActorUsingMe(Actor))
                                {
                                    actor.AddToUseList(Actor);
                                }
                                EnterStateMachine("GenieLampSummon", "Enter", "x");
                                SetActor("lamp", actor);
                                SetParameter("isFirstTime", paramValue);
                                AnimateSim("Exit");
                                actor.Destroy();
                                createdSim.FadeIn();
                                VisualEffect effect = VisualEffect.Create("ep6GenieAppearSmoke_main");
                                effect.SetPosAndOrient(createdSim.Position, Vector3.UnitX, Vector3.UnitZ);
                                effect.SubmitOneShotEffect(VisualEffect.TransitionType.SoftTransition);
                                OpportunityManager opportunityManager = Actor.OpportunityManager;
                                if ((opportunityManager != null) && opportunityManager.HasOpportunity(OpportunityNames.EP6_ReleaseGenie_SummonGenie))
                                {
                                    OccultGenie genie2 = createdSim.OccultManager.GetOccultType(OccultTypes.Genie) as OccultGenie;
                                    if (genie2 == null)
                                    {
                                        createdSim.Destroy();
                                        createdSim = null;
                                    }
                                    else
                                    {
                                        OccultGenieEx.OnFreedFromLamp(genie2, Actor, createdSim, true);
                                        if (opportunityManager.GetLastOpportunity(OpportunityCategory.Special) == OpportunityNames.EP6_ReleaseGenie_SummonGenie)
                                        {
                                            opportunityManager.ClearLastOpportunity(OpportunityCategory.Special);
                                        }

                                        EventTracker.SendEvent(EventTypeId.kGrantedWishToFreeGenie, Actor, Target);
                                        if (Target.InInventory)
                                        {
                                            Actor.Inventory.RemoveByForce(Target);
                                            if (Target.IsOnHandTool)
                                            {
                                                Target.RemoveFromWorld();
                                            }
                                        }
                                        else
                                        {
                                            Target.RemoveFromWorld();
                                        }
                                        EnterStateMachine("FreeTheGenie", "Enter", "x");
                                        SetActor("x", createdSim);
                                        AnimateSim("Exit");
                                    }
                                }
                                else
                                {
                                    Target.mGenieSocializingEvent = EventTracker.AddListener(EventTypeId.kSocialInteraction, OnSocialization);
                                    Target.CheckGenieReturnToLamp = createdSim.AddAlarmRepeating(1f, TimeUnit.Minutes, Target.CheckGenieReturnToLampCallback, "Genie Check to return to lamp", AlarmType.AlwaysPersisted);
                                    Target.mTimeSinceLastSocialWithGenie = SimClock.CurrentTime();
                                }
                            }
                        }
                    }
                }
                finally
                {
                    person.Destroy();
                }

                EndCommodityUpdates(true);
                StandardExit(createdSim == null, createdSim == null);
                return true;
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

        public new class Definition : GenieLamp.RubLamp.Definition
        {
             public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new RubLampEx();
                result.Init(ref parameters);
                return result;
            }
        }
    }
}
