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
using Sims3.Gameplay.Pools;
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
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HaveBabyHomeEx : Pregnancy.HaveBabyHome, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Pregnancy.HaveBabyHome.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Pregnancy.HaveBabyHome.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public static void EnsureForeignFather(Pregnancy pregnancy)
        {
            if (SimDescription.Find(pregnancy.DadDescriptionId) == null)
            {
                SimDescription dad = MiniSims.UnpackSim(MiniSimDescription.Find(pregnancy.DadDescriptionId));
                if (dad != null)
                {
                    Household.CreateTouristHousehold();
                    Household.TouristHousehold.AddTemporary(dad);
                }
            }
        }

        public override void Cleanup()
        {
            Pregnancy pregnancy = Actor.SimDescription.Pregnancy;

            bool wasPregnant = (pregnancy != null);

            try
            {
                if (!Actor.SimDescription.IsVampire)
                {
                    Actor.Motives.CreateMotive(CommodityKind.Hunger);
                }

                try
                {
                    // Stops the base call from starting an alarm to redo the "Have Baby" again
                    if (mNewborns == null)
                    {
                        mNewborns = new List<Sim>();
                    }

                    base.Cleanup();
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    if (pregnancy != null)
                    {
                        Common.Exception(Actor, Target, e);
                    }
                }

                Sims3.Gameplay.Gameflow.Singleton.EnableSave(this);


                // EA Standard no longer calls this function for Males, compensate
                if ((Actor.IsMale) && (mNewborns != null))
                {
                    if ((!Actor.HasBeenDestroyed) && (pregnancy != null))
                    {
                        pregnancy.PregnancyComplete(mNewborns, null);
                    }
                }

                if ((mNewborns != null) && (mNewborns.Count == 4))
                {
                    if ((Actor != null) && (Actor.BuffManager != null))
                    {
                        Actor.BuffManager.RemoveElement(BuffNames.ItsABoy);
                        Actor.BuffManager.AddElement(CommonPregnancy.sItsQuadruplets, Origin.FromNewBaby);
                    }

                    if (pregnancy != null)
                    {
                        Sim dad = pregnancy.mDad;
                        if ((dad != null) && (dad.BuffManager != null))
                        {
                            dad.BuffManager.RemoveElement(BuffNames.ItsABoy);
                            dad.BuffManager.AddElement(CommonPregnancy.sItsQuadruplets, Origin.FromNewBaby);
                        }
                    }

                    Actor.BuffManager.RemoveElement(BuffNames.Pregnant);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
            finally
            {
                Sims3.Gameplay.Gameflow.Singleton.EnableSave(this);
            }
        }

        public override bool Run()
        {
            return Run(this);
        }

        public static bool Run(Pregnancy.HaveBabyHome ths)
        {
            try
            {
                if (ths.Actor.LotCurrent != ths.Target)
                {
                    Vector3 point = World.LotGetPtInside(ths.Target.LotId);
                    if (point == Vector3.Invalid)
                    {
                        return false;
                    }

                    if (!ths.Actor.RouteToPointRadius(point, 3f) && (!GlobalFunctions.PlaceAtGoodLocation(ths.Actor, new World.FindGoodLocationParams(point), false) || !SimEx.IsPointInLotSafelyRoutable(ths.Actor, ths.Target, ths.Actor.Position)))
                    {
                        ths.Actor.AttemptToPutInSafeLocation(true);
                    }
                }

                if (ths.Actor.Posture is SwimmingInPool)
                {
                    SwimmingInPool posture = ths.Actor.Posture as SwimmingInPool;
                    if (!posture.ContainerPool.RouteToEdge(ths.Actor))
                    {
                        if (ths.Actor.BridgeOrigin != null)
                        {
                            ths.Actor.BridgeOrigin.MakeRequest();
                        }

                        ths.Actor.PopPosture();
                        IGameObject reservedTile = null;
                        ths.Actor.FindRoutablePointInsideNearFrontDoor(ths.Actor.Household.LotHome, out reservedTile);
                        Vector3 position = reservedTile.Position;
                        Terrain.TeleportMeHere here = Terrain.TeleportMeHere.Singleton.CreateInstance(Terrain.Singleton, ths.Actor, new InteractionPriority(InteractionPriorityLevel.Pregnancy), false, false) as Terrain.TeleportMeHere;
                        here.SetAndReserveDestination(reservedTile);

                        try
                        {
                            here.RunInteractionWithoutCleanup();
                        }
                        catch
                        {
                            ths.Actor.SetPosition(position);
                        }
                        finally
                        {
                            here.Cleanup();
                        }
                        ths.Actor.LoopIdle();
                    }
                }

                Pregnancy pregnancy = ths.Actor.SimDescription.Pregnancy;
                if (pregnancy != null)
                {
                    // Custom
                    EnsureForeignFather(pregnancy);
                }

                ths.GetNewBorns();
                ths.AcquirePregnancyStateMachine();

                ths.mCurrentStateMachine.SetActor("x", ths.Actor);
                for (int i = 0x0; i < ths.mNewborns.Count; i++)
                {
                    Sim sim = ths.mNewborns[i];
                    Relationship.Get(ths.Actor, sim, true).LTR.ForceChangeState(LongTermRelationshipTypes.Friend);
                    if (sim.BridgeOrigin != null)
                    {
                        sim.BridgeOrigin.MakeRequest();
                        sim.BridgeOrigin = null;
                    }

                    Pregnancy.InternalHaveBabyHome instance = Pregnancy.InternalHaveBabyHome.Singleton.CreateInstance(sim, ths.Actor, ths.GetPriority(), ths.Autonomous, ths.CancellableByPlayer) as Pregnancy.InternalHaveBabyHome;
                    instance.TotalCount = ths.mNewborns.Count;
                    instance.BabyIndex = i + 0x1;
                    instance.mCurrentStateMachine = ths.mCurrentStateMachine;
                    ths.Actor.InteractionQueue.PushAsContinuation(instance, true);
                }

                ths.TryDeliverImaginaryFriend();
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(ths.Actor, ths.Target, e);
                return false;
            }
        }

        public override void GetNewBorns()
        {
            Pregnancy pregnancy = Actor.SimDescription.Pregnancy;
            bool isSelectable = Actor.IsSelectable;
            if (isSelectable)
            {
                Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Gameplay/ActorSystems/Pregnancy:DisableSave");
                // Custom
                //mNewborns = HaveBabyHospitalEx.CreateNewborns(new PregnancyProxy(pregnancy), 0f, isSelectable, true);
                mNewborns = new Proxies.PregnancyProxy(pregnancy).CreateNewborns(0f, isSelectable, true);
            }
            else
            {
                try
                {
                    Simulator.YieldingDisabled = true;

                    // Custom
                    //mNewborns = HaveBabyHospitalEx.CreateNewborns(new PregnancyProxy(pregnancy), 0f, isSelectable, true);
                    mNewborns = new Proxies.PregnancyProxy(pregnancy).CreateNewborns(0f, isSelectable, true);
                }
                finally
                {
                    Simulator.YieldingDisabled = false;
                }
            }
        }

        public new class Definition : Pregnancy.HaveBabyHome.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new HaveBabyHomeEx();
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
