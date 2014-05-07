using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
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
    public class HaveBabyHospitalEx : Pregnancy.HaveBabyHospital, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Pregnancy.HaveBabyHospital.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Pregnancy.HaveBabyHospital.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override void Cleanup()
        {
            bool wasPregnant = (Actor.SimDescription.Pregnancy != null);

            try
            {
                if (!Actor.SimDescription.IsVampire)
                {
                    Actor.Motives.CreateMotive(CommodityKind.Hunger);
                }

                // Stops the base call from starting an alarm to redo the "Have Baby" again
                BabyShouldBeBorn = false;

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

        public override bool InRabbitHole()
        {
            string msg = "HaveBabyHospitalEx:InRabbitHole" + Common.NewLine;

            try
            {
                while (!Actor.WaitForExitReason(Sim.kWaitForExitReasonDefaultTime, ExitReason.CanceledByScript))
                {
                    if (BabyShouldBeBorn && (((mDad == null) || !(mDad.CurrentInteraction is Pregnancy.GoToHospital)) || ((SimFollowers != null) && SimFollowers.Contains(mDad))))
                    {
                        break;
                    }
                }

                msg += "A";

                if (!BabyShouldBeBorn && Actor.HasExitReason(ExitReason.CanceledByScript))
                {
                    return false;
                }

                msg += "B";

                Pregnancy pregnancy = Actor.SimDescription.Pregnancy;
                if (pregnancy != null)
                {
                    // Custom
                    HaveBabyHomeEx.EnsureForeignFather(pregnancy);
                }

                bool isSelectable = Actor.IsSelectable;
                //if (isSelectable)
                //{
                    Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Gameplay/ActorSystems/Pregnancy:DisableSave");
                    //mNewborns = CreateNewborns(new HaveBabyHomeEx.PregnancyProxy(pregnancy), kBonusMoodPointsForHospitalBirth, isSelectable, false);
                    mNewborns = new Proxies.PregnancyProxy(pregnancy).CreateNewborns(kBonusMoodPointsForHospitalBirth, isSelectable, false);
               // }
               // else
               // {
                 //   try
                   // {
                     //   Simulator.YieldingDisabled = true;
                        //mNewborns = CreateNewborns(new HaveBabyHomeEx.PregnancyProxy(pregnancy), kBonusMoodPointsForHospitalBirth, isSelectable, false); - was commented
                       // mNewborns = new Proxies.PregnancyProxy(pregnancy).CreateNewborns(kBonusMoodPointsForHospitalBirth, isSelectable, false);
                   // }
                   //// finally
                   // {
                   //     Simulator.YieldingDisabled = false;
                   // }
               // }

                msg += "C";

                Sim dad = Actor.SimDescription.Pregnancy.mDad;

                Actor.SimDescription.SetPregnancy(0f);
                List<Sim> simFollowers = SimFollowers;
                Actor.SimDescription.Pregnancy.PregnancyComplete(mNewborns, simFollowers);

                if (mNewborns.Count == 4)
                {
                    Actor.BuffManager.RemoveElement(BuffNames.ItsABoy);
                    Actor.BuffManager.AddElement(CommonPregnancy.sItsQuadruplets, Origin.FromNewBaby);

                    if (dad != null)
                    {
                        dad.BuffManager.RemoveElement(BuffNames.ItsABoy);
                        dad.BuffManager.AddElement(CommonPregnancy.sItsQuadruplets, Origin.FromNewBaby);
                    }
                }

                msg += "D";

                SpeedTrap.Sleep(0x0);

                List<Sim> list2 = new List<Sim>();
                list2.Add(Actor);

                if (simFollowers != null)
                {
                    foreach (Sim sim in simFollowers)
                    {
                        if (sim.SimDescription.TeenOrAbove && (sim.GetObjectInRightHand() == null))
                        {
                            list2.Add(sim);
                        }
                    }
                }

                msg += "E";

                if (mNewborns.Count <= list2.Count)
                {
                    for (int i = 0x0; i < mNewborns.Count; i++)
                    {
                        Sim target = list2[i];
                        Posture posture = target.Posture;
                        target.Posture = null;
                        Sim actor = mNewborns[i];
                        InteractionInstance entry = Pregnancy.PregnancyPlaceholderInteraction.Singleton.CreateInstance(target, actor, new InteractionPriority(InteractionPriorityLevel.Zero), false, false);
                        actor.InteractionQueue.Add(entry);

                        while ((actor.CurrentInteraction != entry) && actor.InteractionQueue.HasInteraction(entry))
                        {
                            SpeedTrap.Sleep();
                        }

                        try
                        {
                            ChildUtils.CarryChild(target, actor, false);
                        }
                        catch (Exception e)
                        {
                            Common.Exception(actor, target, e);
                        }

                        target.Posture = posture;
                        AddFollower(mNewborns[i]);
                    }
                }
                else
                {
                    BabyBasket basket = GlobalFunctions.CreateObject("BabyBasket", Vector3.OutOfWorld, 0x0, Vector3.UnitZ) as BabyBasket;
                    basket.AddBabiesToBasket(mNewborns);
                    CarrySystem.EnterWhileHolding(Actor, basket);
                    CarrySystem.VerifyAnimationParent(basket, Actor);
                }

                msg += "F";

                if (Actor.IsSelectable)
                {
                    OccultImaginaryFriend.DeliverDollToHousehold(mNewborns);
                }
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, msg, e);
                return false;
            }
        }

        public new class Definition : Pregnancy.HaveBabyHospital.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new HaveBabyHospitalEx();
                result.Init(ref parameters);
                return result;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
