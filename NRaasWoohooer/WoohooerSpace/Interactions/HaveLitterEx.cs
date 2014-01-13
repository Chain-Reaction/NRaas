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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.PetSystems;
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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HaveLitterEx : Pregnancy.HaveLitter, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Pregnancy.HaveLitter.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Pregnancy.HaveLitter.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override void Cleanup()
        {
            bool wasPregnant = (Actor.SimDescription.Pregnancy != null);

            try
            {
                base.Cleanup();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (wasPregnant)
                {
                    Common.Exception(Actor, Target, e);
                }
                else
                {
                    Common.DebugException(Actor, Target, e);
                }
            }
            finally
            {
                Sims3.Gameplay.Gameflow.Singleton.EnableSave(this);
            }
        }

        public override bool Run()
        {
            try
            {
                if (Actor.LotCurrent != Target)
                {
                    Vector3 point = World.LotGetPtInside(Target.LotId);
                    if (point == Vector3.Invalid)
                    {
                        return false;
                    }
                    if (!Actor.RouteToPointRadius(point, 3f) && (!GlobalFunctions.PlaceAtGoodLocation(Actor, new World.FindGoodLocationParams(point), false) || !SimEx.IsPointInLotSafelyRoutable(Actor, Target, Actor.Position)))
                    {
                        Actor.AttemptToPutInSafeLocation(true);
                    }
                }

                Sim destObj = null;
                float negativeInfinity = float.NegativeInfinity;
                foreach (Sim sim2 in Actor.Household.Sims)
                {
                    if ((sim2.IsHuman && sim2.IsAtHome) && !sim2.IsSleeping)
                    {
                        Relationship relationship = Relationship.Get(Actor, sim2, false);
                        if ((relationship != null) && (relationship.LTR.Liking > negativeInfinity))
                        {
                            destObj = sim2;
                            negativeInfinity = relationship.LTR.Liking;
                        }
                    }
                }

                if (destObj != null)
                {
                    Actor.RouteToDynamicObjectRadius(destObj, kRouteToHighRelSimRange[0x0], kRouteToHighRelSimRange[0x1], null, new Route.RouteOption[] { Route.RouteOption.DoLineOfSightCheckUserOverride });
                }

                string instanceName = null;
                switch (Actor.SimDescription.Species)
                {
                    case CASAgeGenderFlags.Dog:
                        instanceName = "dogPregnantJig";
                        break;

                    case CASAgeGenderFlags.LittleDog:
                        instanceName = "smallDogPregnantJig";
                        break;

                    case CASAgeGenderFlags.Horse:
                        instanceName = "horsePregnantJig";
                        break;

                    case CASAgeGenderFlags.Cat:
                        instanceName = "catPregnantJig";
                        break;
                }

                if (instanceName != null)
                {
                    mPetPregnancyJig = GlobalFunctions.CreateObjectOutOfWorld(instanceName, ProductVersion.EP5) as SocialJigOnePerson;
                    if (mPetPregnancyJig != null)
                    {
                        mPetPregnancyJig.RegisterParticipants(Actor, null);
                        Vector3 position = Actor.Position;
                        Vector3 forwardVector = Actor.ForwardVector;
                        if (GlobalFunctions.FindGoodLocationNearby(mPetPregnancyJig, ref position, ref forwardVector))
                        {
                            mPetPregnancyJig.SetPosition(position);
                            mPetPregnancyJig.SetForward(forwardVector);
                            mPetPregnancyJig.AddToWorld();
                            Route r = mPetPregnancyJig.RouteToJigA(Actor);
                            if (!Actor.DoRoute(r))
                            {
                                mPetPregnancyJig.Destroy();
                                mPetPregnancyJig = null;
                            }
                        }
                        else
                        {
                            mPetPregnancyJig.Destroy();
                            mPetPregnancyJig = null;
                        }
                    }
                }

                StandardEntry();
                BeginCommodityUpdates();
                EnterStateMachine("GiveBirth", "Enter", "x");
                AnimateSim("LieDown");
                if (Actor.IsSelectable)
                {
                    try
                    {
                        Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Gameplay/ActorSystems/PetPregnancy:DisableSave");

                        // Custom
                        HaveBabyHomeEx.EnsureForeignFather(Actor.SimDescription.Pregnancy);

                        mOffspring = new Proxies.PetPregnancyProxy(Actor.SimDescription.Pregnancy).CreateNewborns(0f, true, true);
                        int num2 = 0x0;
                        foreach (Sim sim3 in mOffspring)
                        {
                            sim3.SimDescription.IsNeverSelectable = true;
                            if (mPetPregnancyJig != null)
                            {
                                Slot slotName = kPregnancyJigContainmentSlots[num2++];
                                sim3.SetPosition(mPetPregnancyJig.GetPositionOfSlot(slotName));
                                sim3.SetForward(mPetPregnancyJig.GetForwardOfSlot(slotName));
                                InteractionInstance entry = Pregnancy.BabyPetLieDown.Singleton.CreateInstance(sim3, sim3, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true);
                                entry.Hidden = true;
                                sim3.InteractionQueue.AddNext(entry);
                            }
                            else
                            {
                                World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(Actor.Position);
                                fglParams.RequiredRoomID = Actor.RoomId;
                                if ((!GlobalFunctions.PlaceAtGoodLocation(sim3, fglParams, false) || !sim3.IsPointInLotSafelyRoutable(sim3.LotCurrent, sim3.Position)) && !sim3.AttemptToPutInSafeLocation(true))
                                {
                                    sim3.SetPosition(Actor.Position);
                                }
                            }
                        }
                    }
                    finally
                    {
                        Sims3.Gameplay.Gameflow.Singleton.EnableSave(this);
                    }
                }

                // Custom call
                bool flag = DoLoop(ExitReason.Finished, GiveBirthLoopFunc, mCurrentStateMachine);
                AnimateSim("Exit");
                StandardExit();
                return flag;
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

        private new void GiveBirthLoopFunc(StateMachineClient smc, InteractionInstance.LoopData loopData)
        {
            try
            {
                if (!bHasSpawnedOffspring && (loopData.mLifeTime > kDistressLengthBeforeSpawnNewborns))
                {
                    bHasSpawnedOffspring = true;

                    // Custom call
                    SpawnOffspring();
                }
                if (loopData.mLifeTime > kDistressTotalLength)
                {
                    Actor.AddExitReason(ExitReason.Finished);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        private new void SpawnOffspring()
        {
            Pregnancy pregnancy = Actor.SimDescription.Pregnancy;
            Sim dad = pregnancy.Dad;
            bool isSelectable = Actor.IsSelectable;
            if (isSelectable)
            {
                PlumbBob.SelectActor(Actor);
                Camera.LerpParams kShowPetPregnancyCameraLerp = PetPregnancy.kShowPetPregnancyCameraLerp;
                Camera.FocusOnSim(Actor, kShowPetPregnancyCameraLerp.Zoom, kShowPetPregnancyCameraLerp.Pitch, kShowPetPregnancyCameraLerp.Time, true, false);
                BuffNames newKittensHuman = BuffNames.NewKittensHuman;
                Origin fromNewLitter = Origin.FromNewLitter;
                if (Actor.IsHorse)
                {
                    newKittensHuman = BuffNames.NewFoalHuman;
                    fromNewLitter = Origin.FromNewFoal;
                }
                else if (Actor.IsADogSpecies)
                {
                    newKittensHuman = BuffNames.NewPuppiesHuman;
                }
                PetPregnancy.ReactToPetBirthHelper helper = new PetPregnancy.ReactToPetBirthHelper(newKittensHuman, fromNewLitter, Actor.Household.HouseholdId);
                new Sim.Celebrate.Event(Actor, null, PetPregnancy.kHadLitterCelebrationWitnessTuning, PetPregnancy.kHadLitterCelebrationBroadcasterParams, null, new Callback(helper.OnLitterCelebrationStarted));
                Audio.StartObjectSound(Actor.ObjectId, "sting_pet_baby_born", false);
            }
            else
            {
                try
                {
                    Simulator.YieldingDisabled = true;

                    // Custom
                    HaveBabyHomeEx.EnsureForeignFather(pregnancy);

                    // Custom
                    mOffspring = new Proxies.PetPregnancyProxy(pregnancy).CreateNewborns(0f, isSelectable, true);
                }
                finally
                {
                    Simulator.YieldingDisabled = false;
                }
            }

            Actor.SimDescription.SetPetPregnant(false);
            string effectName = "ep5birthfoalconfetti";
            int count = mOffspring.Count;
            if (Actor.IsCat)
            {
                if (count == 0x1)
                {
                    Actor.ShowTNSIfSelectable(TNSNames.PetPregnancyCatSingleKittenTNS, null, Actor, new object[] { Actor });
                }
                else
                {
                    Actor.ShowTNSIfSelectable(TNSNames.PetPregnancyCatHadLitterTNS, null, Actor, new object[] { Actor, count });
                }
                effectName = "ep5birthkittenconfetti";
            }
            else if (Actor.IsADogSpecies)
            {
                if (count == 0x1)
                {
                    Actor.ShowTNSIfSelectable(TNSNames.PetPregnancyDogSinglePuppyTNS, null, Actor, new object[] { Actor });
                }
                else
                {
                    Actor.ShowTNSIfSelectable(TNSNames.PetPregnancyDogHadLitterTNS, null, Actor, new object[] { Actor, count });
                }
                effectName = "ep5birthpuppyconfetti";
            }

            foreach (Sim sim2 in mOffspring)
            {
                Relationship.Get(Actor, sim2, true).LTR.UpdateLiking(RandomUtil.GetFloat(kInitialChildParentRelLikingRange[0x0], kInitialChildParentRelLikingRange[0x1]));
                if (dad != null)
                {
                    Relationship relationship2 = Relationship.Get(dad, sim2, true);
                    if (relationship2 != null)
                    {
                        relationship2.LTR.UpdateLiking(RandomUtil.GetFloat(kInitialChildParentRelLikingRange[0x0], kInitialChildParentRelLikingRange[0x1]));
                    }
                }
                if (isSelectable)
                {
                    sim2.SimDescription.IsNeverSelectable = false;
                }
                Pregnancy.MakeBabyVisible(sim2);
                VisualEffect effect = VisualEffect.Create(effectName);
                effect.SetPosAndOrient(Slots.GetPositionOfSlot(sim2.ObjectId, (uint)Sim.FXJoints.Spine0), -sim2.ForwardVector, Slots.GetUpOfSlot(sim2.ObjectId, Sim.FXJoints.Spine0));
                effect.SubmitOneShotEffect(VisualEffect.TransitionType.SoftTransition);
            }
        }

        public new class Definition : Pregnancy.HaveLitter.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new HaveLitterEx();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
