using Sims3.SimIFace;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace.CustomContent;
using System;
using Sims3.Store.Objects;
using Sims3.Gameplay.Core;
using Sims3.UI;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ActorSystems;
using System.Collections.Generic;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.UI.Controller;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.RouteDestinations;
using Sims3.SimIFace.Enums;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.EventSystem;
using ani_StoreSetRegister;
using ani_StoreSetBase;
using System.Text;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreRestockItem;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase;
using ani_StoreRestockItem;
using Sims3.Gameplay.Seasons;

namespace Sims3.Gameplay.Objects.TombObjects.ani_StoreSetRegister
{
    //[RuntimeExport]
    public class StoreSetRegister : GameObject, IShoppingRegister, ICounterSurfaceAppliance, IGameObject, IScriptObject, IScriptLogic, IHasScriptProxy, IObjectUI, IExportableContent
    {
        [Persistable]
        public ulong mManualyTendingRegister;

        [Persistable]
        public bool mCustomerPaying;
        // public string mTreasureChestInfoKey;

        public RegisterInfo Info;

        public float ManualStartHour;
        public float ManualEndHour;

        public struct SellableObjectData
        {
            public ObjectGuid shop;
            public ObjectGuid sellable;
            public int cost;
            public ulong simDescriptionId;
            public SellableObjectData(ObjectGuid shop, ObjectGuid sellable, int cost, ulong simDescriptionID)
            {
                this.shop = shop;
                this.sellable = sellable;
                this.cost = cost;
                this.simDescriptionId = simDescriptionID;
            }
        }

        #region Customer Interactions
        [Persistable]
        public class DoingCustomerStuffPosture : Posture
        {
            public Sim Actor;
            public StoreSetRegister Target;

            public int StartHour;

            public override bool PerformIdleLogic
            {
                get
                {
                    return false;
                }
            }
            public override IGameObject Container
            {
                get
                {
                    return this.Target;
                }
            }
            public override string Name
            {
                get
                {
                    return CMStoreSet.LocalizeString("CustomerPosture", new object[0] { });
                }
            }
            public DoingCustomerStuffPosture()
            {
            }
            public DoingCustomerStuffPosture(Sim actor, StoreSetRegister target, StateMachineClient swingStateMachine)
                : base(swingStateMachine)
            {
                this.Actor = actor;
                this.Target = target;
                this.StartHour = -1;
                if (this.Target != null)
                {
                    target.AddToUseList(this.Actor);
                }
            }
            public override bool AllowsReactionOverlay()
            {
                return true;
            }
            public override bool AllowsNormalSocials()
            {
                return false;
            }
            public override bool AllowsRouting()
            {
                return true;
            }
            public override void OnInteractionQueueEmpty()
            {
                if (this.Actor != null)
                {
                    try
                    {
                        if (this.StartHour == -1)
                        {
                            this.StartHour = (int)SimClock.CurrentTime().Hour;
                        }
                        else
                        {
                            if ((int)SimClock.CurrentTime().Hour - this.StartHour >= 2)
                            {
                                CMStoreSet.PrintMessage(CMStoreSet.LocalizeString("LeftWithoutPaying", new object[] { this.Actor.FullName }));

                                this.Target.Info.ShoppingData.Remove(this.Actor.SimDescription);
                                this.Target.Info.CurrentCustomer = 0uL;

                                this.Actor.PopPosture();
                                Sim.MakeSimGoHome(this.Actor, false);
                                return;
                            }
                        }

                        this.Actor.GreetSimOnLot(this.Target.LotCurrent);

                        if (!this.Actor.IsActiveSim)
                        {
                            InteractionInstance interactionInstance = StoreSetRegister.WaitForTurn.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
                            if (interactionInstance != null)
                            {
                                this.Actor.InteractionQueue.Add(interactionInstance);
                            }
                        }
                        else
                        {
                            //Exit this if active
                            this.Actor.PopPosture();
                        }
                    }
                    catch (Exception ex)
                    {
                        CMStoreSet.PrintMessage(ex.Message);
                    }
                }
            }
            public override void PopulateInteractions()
            {
            }
            public override float Satisfaction(CommodityKind condition, IGameObject target)
            {
                if ((condition == CommodityKind.Standing || condition == CommodityKind.IsTarget) && (target == this.Actor || target == this.Container))
                {
                    return 1f;
                }
                return 0f;
            }
            public override InteractionInstance GetStandingTransition()
            {
                InteractionInstance headInteraction = this.Actor.InteractionQueue.GetHeadInteraction();
                if (headInteraction is StoreSetRegister.WaitForTurn || headInteraction is StoreSetRegister.CancelBeingAClerk)
                {
                    return null;
                }
                return StoreSetRegister.CancelBeingAClerk.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
            }
            public override InteractionInstance GetCancelTransition()
            {
                InteractionInstance headInteraction = this.Actor.InteractionQueue.GetHeadInteraction();
                if (headInteraction is StoreSetRegister.WaitForTurn || headInteraction is StoreSetRegister.CancelBeingAClerk)
                {
                    return null;
                }
                return StoreSetRegister.CancelBeingAClerk.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
            }
            public override void Shoo(bool yield, List<Sim> shooedSims)
            {
                throw new Exception("The method or operation is not implemented.");
            }
            public override ScriptPosture GetSacsPostureParameter()
            {
                return ScriptPosture.NoAnimation;
            }
            public override void OnExitPosture()
            {
                if (this.Target != null)
                {
                    // this.Target.ChangeSimToPreviousOutfit(this.Actor);
                    if (this.Target.IsActorUsingMe(this.Actor))
                    {
                        this.Target.RemoveFromUseList(this.Actor);
                    }
                }
                base.OnExitPosture();
            }
            public override Posture OnReset(IGameObject objectBeingReset)
            {
                if (this.Container == this.Target && this.Target.ActorsUsingMe.Contains(this.Actor))
                {
                    this.Target.RemoveFromUseList(this.Actor);
                }
                if (this.CurrentStateMachine != null)
                {
                    this.CurrentStateMachine.Dispose();
                    this.CurrentStateMachine = null;
                }

                return null;
            }
            public override void AddInteractions(IActor actor, IActor target, List<InteractionObjectPair> results)
            {
                base.AddInteractions(actor, target, results);
            }

        }

        public class WaitForTurn : Interaction<Sim, StoreSetRegister>
        {
            public class Definition : InteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.WaitForTurn>
            {
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    return CMStoreSet.LocalizeString("WaitForTurn", new object[0] { });
                }
            }
            public static InteractionDefinition Singleton = new StoreSetRegister.WaitForTurn.Definition();
            public override bool Run()
            {
                try
                {
                    if (!(this.Actor.Posture is StoreSetRegister.DoingCustomerStuffPosture))
                    {
                        StoreSetRegister.DoingCustomerStuffPosture posture = new StoreSetRegister.DoingCustomerStuffPosture(this.Actor, this.Target, null);
                        this.Actor.Posture = posture;
                    }

                    //Look at random object while waiting for turn
                    StoreSetBase[] objects = this.Target.LotCurrent.GetObjects<StoreSetBase>();
                    if (objects == null || objects.Length == 0)
                    {
                        return false;
                    }
                    StoreSetBase randomObjectFromList = RandomUtil.GetRandomObjectFromList<StoreSetBase>(objects);
                    if (randomObjectFromList == null)
                    {
                        return false;
                    }

                    base.StandardEntry(false);
                    base.BeginCommodityUpdates();
                    if (!this.Actor.RouteToObjectRadialRange(randomObjectFromList, 1f, UniversityWelcomeKit.kMaxRouteDistance))
                    {
                        return false;
                    }
                    this.Actor.RouteTurnToFace(randomObjectFromList.Position);
                    List<ObjectGuid> objectsICanBuyInDisplay = randomObjectFromList.GetObjectIDsICanBuyInDisplay(this.Actor, base.Autonomous);
                    RandomUtil.RandomizeListOfObjects<ObjectGuid>(objectsICanBuyInDisplay);

                    if (objectsICanBuyInDisplay != null && objectsICanBuyInDisplay.Count > 0)
                    {
                        ObjectGuid guid = objectsICanBuyInDisplay[0];
                        GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(guid);

                        this.Actor.RouteTurnToFace(gameObject.Position);
                        int priority = 100;
                        this.Actor.LookAtManager.SetInteractionLookAt(gameObject, priority, LookAtJointFilter.TorsoBones | LookAtJointFilter.HeadBones);
                        bool flag = RandomUtil.RandomChance01(StoreSetBase.kBrowseChanceOfDislikingObject);
                        ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.BalloonData(gameObject.GetThumbnailKey());
                        if (flag)
                        {
                            balloonData.LowAxis = ThoughtBalloonAxis.kDislike;
                        }
                        this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                        string state = "1";
                        if (flag)
                        {
                            state = RandomUtil.GetRandomStringFromList(new string[]
                            {
                                "3", 
                                "5", 
                                "CantStandArtTraitReaction"
                            });
                        }
                        else
                        {
                            state = RandomUtil.GetRandomStringFromList(new string[]
                            {
                                "0", 
                                "1", 
                                "2"
                            });
                        }
                        base.EnterStateMachine("viewobjectinteraction", "Enter", "x");
                        base.AnimateSim(state);
                        base.AnimateSim("Exit");
                        this.Actor.LookAtManager.ClearInteractionLookAt();
                    }


                    base.EndCommodityUpdates(true);
                    base.StandardExit(false, false);

                }

                catch (Exception ex)
                {
                    CMStoreSet.PrintMessage("Wait For Turn: " + ex.Message);
                }

                return true;
            }
        }

        public sealed class PayForItems : Interaction<Sim, StoreSetRegister>
        {
            public sealed class Definition : InteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.PayForItems>
            {
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.Info.ShoppingData != null && target.Info.ShoppingData.ContainsKey(a.SimDescription))
                        return true;
                    return false;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    if (target.Info.ShoppingData != null && target.Info.ShoppingData.ContainsKey(actor.SimDescription))
                        return CMStoreSet.LocalizeString("PayForItems", new object[] { target.Info.ShoppingData[actor.SimDescription] });
                    else
                        return CMStoreSet.LocalizeString("PayForItems", new object[0] { });
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.PayForItems.Definition();
            public override bool Run()
            {
                if (!this.Target.RouteCustomerToRegister(this.Actor))
                    return false;

                //Pay for items 
                if (this.Target.Info.ShoppingData.ContainsKey(this.Actor.SimDescription))
                {
                    //Find the clerck
                    //Sim clerk = null;
                    //SimDescription sd = null;
                    //if (this.Target.mManualyTendingRegister != null)
                    //    sd = SimDescription.Find(this.Target.mManualyTendingRegister);
                    //else if(CMStoreSet.IsStoreOpen(this.Target) && this.Target.mPreferredClerk != 0uL)
                    //    sd = SimDescription.Find(this.Target.mPreferredClerk);

                    //if (sd != null && sd.CreatedSim != null)
                    //    clerk = sd.CreatedSim;



                    //Leaflet
                    string stateMachine = "ani_register";
                    string entryState = "";
                    string state = "accept";

                    bool succeeded = false;
                    //TODO:Find animations for kids.
                    if (this.Actor.SimDescription.TeenOrAbove)
                    {
                        base.StandardEntry();
                        base.EnterStateMachine(stateMachine, entryState, "x");
                        base.BeginCommodityUpdates();
                        base.AnimateSim(state);
                    }
                    this.Actor.ModifyFunds(-this.Target.Info.ShoppingData[this.Actor.SimDescription]);
                    if (this.Target.Info.OwnerId != 0uL)
                    {
                        SimDescription owner = CMStoreSet.ReturnSim(this.Target.Info.OwnerId);
                        if (owner != null)
                        {
                            owner.ModifyFunds(this.Target.Info.ShoppingData[this.Actor.SimDescription]);
                        }
                    }

                    //Remove customer and send home 
                    this.Target.Info.ShoppingData.Remove(this.Actor.SimDescription);
                    this.Target.Info.CurrentCustomer = 0uL;

                    this.Actor.PopPosture();

                    if (StoreSetBase.ReturnSendHomeAfterPurchase() && !this.Actor.Household.IsActive)
                        Sim.MakeSimGoHome(this.Actor, false);

                    // base.StandardExit();

                    if (this.Actor.SimDescription.TeenOrAbove)
                    {
                        base.EndCommodityUpdates(succeeded);
                        base.AnimateSim("Exit");
                    }
                }


                return true;
            }
        }

        #endregion

        #region Clerck Interactions

        public class DoingShopkeeperStuffPosture : Posture
        {
            public Sim Actor;
            public StoreSetRegister Target;
            public override bool PerformIdleLogic
            {
                get
                {
                    return true;
                }
            }
            public override IGameObject Container
            {
                get
                {
                    return this.Target;
                }
            }
            public override string Name
            {
                get
                {
                    return CMStoreSet.LocalizeString("TendShop", new object[0] { });
                }
            }
            public DoingShopkeeperStuffPosture()
            {
            }
            public DoingShopkeeperStuffPosture(Sim actor, StoreSetRegister target, StateMachineClient swingStateMachine)
                : base(swingStateMachine)
            {
                this.Actor = actor;
                this.Target = target;
                if (this.Target != null)
                {
                    target.AddToUseList(this.Actor);
                }
            }
            public override bool AllowsReactionOverlay()
            {
                return true;
            }
            public override bool AllowsNormalSocials()
            {
                return true;
            }
            public override bool AllowsRouting()
            {
                return true;
            }
            public override void OnInteractionQueueEmpty()
            {
                if (this.Actor != null)
                {
                    this.Actor.GreetSimOnLot(this.Target.LotCurrent);
                    InteractionInstance interactionInstance = StoreSetRegister.PostureIdle.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.Autonomous), false, false);
                    if (interactionInstance != null)
                    {
                        this.Actor.InteractionQueue.Add(interactionInstance);
                    }
                }
            }
            public override void PopulateInteractions()
            {
            }
            public override float Satisfaction(CommodityKind condition, IGameObject target)
            {
                if ((condition == CommodityKind.Standing || condition == CommodityKind.IsTarget) && (target == this.Actor || target == this.Container))
                {
                    return 1f;
                }
                return 0f;
            }
            public override InteractionInstance GetStandingTransition()
            {
                InteractionInstance headInteraction = this.Actor.InteractionQueue.GetHeadInteraction();
                if (headInteraction is StoreSetRegister.PostureIdle || headInteraction is StoreSetRegister.TendRegister || headInteraction is StoreSetRegister.WalkAroundStore || headInteraction is StoreSetRegister.CancelBeingAClerk)
                {
                    return null;
                }
                return StoreSetRegister.CancelBeingAClerk.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
            }
            public override InteractionInstance GetCancelTransition()
            {
                InteractionInstance headInteraction = this.Actor.InteractionQueue.GetHeadInteraction();
                if (headInteraction is StoreSetRegister.PostureIdle || headInteraction is StoreSetRegister.TendRegister || headInteraction is StoreSetRegister.WalkAroundStore || headInteraction is StoreSetRegister.CancelBeingAClerk)
                {
                    return null;
                }
                return StoreSetRegister.CancelBeingAClerk.Singleton.CreateInstance(this.Target, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
            }
            public override void Shoo(bool yield, List<Sim> shooedSims)
            {
                throw new Exception("The method or operation is not implemented.");
            }
            public override ScriptPosture GetSacsPostureParameter()
            {
                return ScriptPosture.NoAnimation;
            }
            public override void OnExitPosture()
            {
                if (this.Target != null)
                {
                    if (this.Target.IsActorUsingMe(this.Actor))
                    {
                        this.Target.RemoveFromUseList(this.Actor);
                    }
                    this.Target.mSimsWantingToBuy.Clear();
                }
                base.OnExitPosture();
            }
            public override Posture OnReset(IGameObject objectBeingReset)
            {
                if (this.Container == this.Target && this.Target.ActorsUsingMe.Contains(this.Actor))
                {
                    this.Target.RemoveFromUseList(this.Actor);
                }
                if (this.CurrentStateMachine != null)
                {
                    this.CurrentStateMachine.Dispose();
                    this.CurrentStateMachine = null;
                }
                return null;
            }
            public override void AddInteractions(IActor actor, IActor target, List<InteractionObjectPair> results)
            {
                base.AddInteractions(actor, target, results);
            }
        }


        #region Posture Tend
        public sealed class TendRegister : Interaction<Sim, StoreSetRegister>
        {
            public sealed class Definition : InteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.TendRegister>
            {
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.mPreferredClerk != a.SimDescription.SimDescriptionId && target.InteractionTestAvailability())
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Can't tend");
                        return false;
                    }
                    return true;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    return "Tend Register";
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.TendRegister.Definition();
            public override bool Run()
            {
                if (!this.Target.RouteMerchantToRegister(this.Actor))
                {
                    return false;
                }

                // CMStoreSet.PrintMessage("this.Target.mTendingSimId " + this.Target.mTendingSimId);
                //  if (this.Target.mTendingSimId == 0uL)
                {
                    this.Target.mTendingSimId = this.Actor.SimDescription.SimDescriptionId;
                    if (this.Actor.SimDescription.SimDescriptionId == this.Target.mPreferredClerk)
                    {
                        this.Target.PayClerkForTodayIfHaventAlready(this.Actor);
                    }
                    bool flag = !(this.Actor.Posture is StoreSetRegister.DoingShopkeeperStuffPosture);
                    base.StandardEntry(flag);

                    //base.EnterStateMachine("ani_register", "EnterScienceStation", "x", "researchStation");

                    //base.AnimateSim("Research");

                    // CMStoreSet.PrintMessage("Tend register");

                    base.EnterStateMachine("tendregister", "Enter", "x");
                    base.BeginCommodityUpdates();
                    base.AnimateSim("TendLoop");
                    bool flag2 = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.LoopCallback), this.mCurrentStateMachine);
                    Household household = this.Target.FindMoneyGetter();
                    if (flag2 && this.Actor.Household != household && !(this.Actor.Posture is StoreSetRegister.DoingShopkeeperStuffPosture))
                    {
                        this.Actor.ModifyFunds(StoreSetRegister.kMoneyForTendingRegister);
                    }
                    base.EndCommodityUpdates(flag2);
                    base.AnimateSim("Exit");
                    base.StandardExit(flag, flag);
                    this.Target.mTendingSimId = 0uL;
                    this.Target.mManualyTendingRegister = 0uL;
                    return flag2;
                }
                // return false;
            }
            public void LoopCallback(StateMachineClient smc, InteractionInstance.LoopData ld)
            {
                //  CMStoreSet.PrintMessage("Customer paying: " + this.Target.mCustomerPaying);
                //Handle customers
                this.Target.CallInNextCustomer(this.Target, this.Actor);

                //UseRegister
                //if (this.Target.mCustomerPaying)
                //{
                //    //  base.AnimateSim("Research");                  
                //}

                if (ld.mLifeTime > (float)10)//GalleryShopRegister.kSimMinBehindRegister)
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                    return;
                }
                if (this.Actor.SimDescription.SimDescriptionId == this.Target.mPreferredClerk)
                {
                    if (!this.Actor.Household.IsActive)
                        this.Actor.Motives.MaxEverything();

                    // if (!SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart, this.Target.mOvenHoursEnd))
                    if (!CMStoreSet.IsStoreOpen(this.Target))
                    {
                        // CMStoreSet.PrintMessage("Tending, exit");
                        this.Actor.AddExitReason(ExitReason.Finished);
                    }
                }
                if (!this.Actor.IsSelectable && this.Actor.SimDescription.SimDescriptionId != this.Target.mPreferredClerk)
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
                if (this.Actor.IsSelectable && this.Actor.Posture is StoreSetRegister.DoingShopkeeperStuffPosture)
                {
                    if (this.Actor.SimDescription.SimDescriptionId == this.Target.mPreferredClerk)
                    {
                        this.Target.SummonClerk();
                    }
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
                else
                {
                    //CMStoreSet.PrintMessage("Not doing posture");
                }
            }
            public override void Cleanup()
            {
                if (base.StandardEntryCalled)
                {
                    this.Target.mTendingSimId = 0uL;
                }
                base.Cleanup();
            }
        }
        #endregion Posture Tend

        public sealed class CheckSalesRecords : ImmediateInteraction<IActor, StoreSetRegister>
        {
            [DoesntRequireTuning]
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.CheckSalesRecords>
            {
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return "Check Sales Records";// GalleryShopRegister.LocalizeString(false, "CheckSalesRecords", new object[0]);
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.CheckSalesRecords.Definition();
            public override bool Run()
            {
                StoreSetBase[] objects = Sims3.Gameplay.Queries.GetObjects<StoreSetBase>(this.Target.LotCurrent);
                ulong num = 0uL;
                ulong num2 = 0uL;
                ulong num3 = 0uL;
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] != null)
                    {
                        num += objects[i].GetLifetimeSaleCount();
                        num2 += objects[i].GetLifetimeRevenue();
                        num3 += objects[i].GetDailyRevenue();
                    }
                    if (num > unchecked((ulong)-1))
                    {
                        num = unchecked((ulong)-1);
                    }
                    if (num2 > unchecked((ulong)-1))
                    {
                        num2 = unchecked((ulong)-1);
                    }
                    if (num3 > unchecked((ulong)-1))
                    {
                        num3 = unchecked((ulong)-1);
                    }
                }
                //TODO
                //StyledNotification.Format format = new StyledNotification.Format(GalleryShopRegister.LocalizeString(false, "RevenueTalley", new object[]
                //{
                //    (uint)num, 
                //    (uint)num3, 
                //    (uint)num2, 
                //    objects.Length, 
                //    (uint)num
                //}), this.Target.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                //StyledNotification.Show(format);
                return true;
            }
        }

        #region Posture Idle

        public class PostureIdle : Interaction<Sim, StoreSetRegister>
        {
            public sealed class Definition : InteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.PostureIdle>
            {
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    return CMStoreSet.LocalizeString("TendRegister", new object[] //GalleryShopRegister.LocalizeString(actor.IsFemale, "TendRegister", new object[]
                    {
                        actor
                    });
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.PostureIdle.Definition();
            public override bool Run()
            {
                Simulator.Sleep(0u);

                //if (CMStoreSet.IsStoreOpen(this.Target))
                this.Target.RestartAlarms();

                if (!this.Actor.Household.IsActive)
                    this.Actor.Motives.MaxEverything();

                this.Target.RetestHoursOfOperation();


                //IF the manual tending sim is stuck, release him.
                if (this.Target.mManualyTendingRegister != 0uL && (this.Target.mManualyTendingRegister != this.Target.mTendingSimId))
                    this.Target.mManualyTendingRegister = 0uL;

                if (!CMStoreSet.IsStoreOpen(this.Target) /*|| this.Target.LotCurrent.FireManager.FireCount > 0*/ || !this.Target.InWorld)// || this.Target.InInventory)
                {
                    //CMStoreSet.PrintMessage("should exit: " + this.Target.mPreferredClerk + " " + this.Target.mTendingSimId);
                    //Should exit work, but not if tending manually
                    //if (this.Target.mPreferredClerk == this.Target.mTendingSimId)
                    {
                        //CMStoreSet.PrintMessage(this.Actor.FullName + " Exit Work");
                        this.Actor.PopPosture();

                        if (this.Target.Info.ChangeToWorkOutfit)
                        {
                            if (SeasonsManager.CurrentSeason == Sims3.SimIFace.Enums.Season.Winter)
                                this.Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Outerwear);
                            else
                                this.Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Everyday);
                        }

                        //Pay hired clerck
                        if (this.Target.Info.HourlyWage > 0 && this.Target.mManualyTendingRegister == 0uL)
                        {
                            CMStoreSet.PayWages(this.Actor, this.Target, this.Target.mOvenHoursStart, this.Target.mOvenHoursEnd);
                        }

                        if (!this.Actor.Household.IsActive)
                            Sim.MakeSimGoHome(this.Actor, false);

                        this.Target.mTendingSimId = 0uL;
                        return true;
                    }
                }

                //Change to work outfit
                if (this.Target.Info.ChangeToWorkOutfit)
                {
                    if (this.Actor.CurrentOutfitCategory != OutfitCategories.Career)
                        this.Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Career);
                }

                if (!(this.Actor.Posture is StoreSetRegister.DoingShopkeeperStuffPosture))
                {
                    StoreSetRegister.DoingShopkeeperStuffPosture posture = new StoreSetRegister.DoingShopkeeperStuffPosture(this.Actor, this.Target, null);
                    this.Actor.Posture = posture;
                }
                this.Actor.GreetSimOnLot(this.Target.LotCurrent);
                if (this.Actor.LotCurrent != this.Target.LotCurrent && CMStoreSet.IsStoreOpen(this.Target))//&& SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart + StoreSetRegister.kChefHoursBeforeWorkToHeadToWork, this.Target.mOvenHoursEnd))
                {
                    World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(this.Target.Position);
                    Vector3 destination;
                    Vector3 vector;
                    if (GlobalFunctions.FindGoodLocation(this.Actor, fglParams, out destination, out vector))
                    {
                        Terrain.Teleport(this.Actor, destination);
                    }
                }
                if (this.Target.Info.ShoppingData.Count > 0)
                {
                    base.TryPushAsContinuation(StoreSetRegister.TendRegister.Singleton);
                }
                else
                {
                    //Restock                    
                    bool restock = false;
                    RestockItem[] restockItems = Sims3.Gameplay.Queries.GetObjects<RestockItem>(this.Target.LotCurrent);

                    if (restockItems != null && restockItems.Length > 0)
                    {
                        for (int i = 0; i < restockItems.Length; i++)
                        {
                            //Restock items in this room, or linked to this register
                            bool isRug = false;
                            bool isMyStore = false;
                            StoreSetBase sBase = RestockItemHelperClass.ReturnStoreSetBase(restockItems[i], out isRug);

                            if (sBase != null)
                            {
                                if (sBase.Info.RegisterId != ObjectGuid.InvalidObjectGuid && sBase.Info.RegisterId == this.Target.ObjectId)
                                    isMyStore = true;
                            }

                            if (restockItems[i].RoomId == this.Target.RoomId || isMyStore)
                            {
                                InteractionInstance entry = RestockItem.Restock.Singleton.CreateInstance(restockItems[i], this.Actor, new InteractionPriority(InteractionPriorityLevel.High), false, false);
                                if (entry.Test())
                                {
                                    restock = true;
                                    this.Actor.PopPosture();
                                    this.Actor.InteractionQueue.AddNext(entry);
                                    break;
                                }
                            }
                        }
                    }
                    if (!restock)
                    {
                        base.TryPushAsContinuation(StoreSetRegister.WalkAroundStore.Singleton);
                    }
                }

                return true;
            }


            public override bool CheckForCancelAndCleanup()
            {
                CMStoreSet.PrintMessage("exit");
                this.Actor.PopPosture();
                return base.CheckForCancelAndCleanup();
            }
        }
        #endregion Posture IDle
        public sealed class CancelBeingAClerk : Interaction<Sim, StoreSetRegister>
        {
            public sealed class Definition : InteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.CancelBeingAClerk>
            {
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.mManualyTendingRegister == 0uL)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("No sim tending register");
                        return false;
                    }

                    return true;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    return CMStoreSet.LocalizeString("CancelBeingClerck", new object[0] { });
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.CancelBeingAClerk.Definition();
            public override bool Run()
            {
                //Find the tending sim 
                if (this.Target.mPreferredClerk != 0uL || this.Target.mManualyTendingRegister != 0uL)
                {
                    ulong simId = this.Target.mPreferredClerk != 0uL ? this.Target.mPreferredClerk : this.Target.mManualyTendingRegister;

                    SimDescription sd = CMStoreSet.ReturnSim(simId);
                    // CMStoreSet.PrintMessage(sd.FullName + " " + sd.CreatedSim.Posture);

                    if (sd != null && sd.CreatedSim != null)
                    {
                        StoreSetRegister.DoingShopkeeperStuffPosture doingShopkeeperStuffPosture = sd.CreatedSim.Posture as StoreSetRegister.DoingShopkeeperStuffPosture;
                        if (doingShopkeeperStuffPosture != null)
                        {
                            sd.CreatedSim.PopPosture();
                            sd.CreatedSim.InteractionQueue.CancelAllInteractions();
                        }
                    }
                }
                return true;
            }
        }

        public sealed class WalkAroundStore : Interaction<Sim, StoreSetRegister>
        {
            public sealed class Definition : InteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.WalkAroundStore>
            {
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    return "Tend Shop";
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.WalkAroundStore.Definition();
            public override bool Run()
            {
                StoreSetBase[] objects = this.Target.LotCurrent.GetObjects<StoreSetBase>();
                List<StoreSetBase> objectsInThisRoom = new List<StoreSetBase>();
                if (objects == null || objects.Length == 0)
                {
                    return false;
                }

                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].RoomId == this.Target.RoomId)
                        objectsInThisRoom.Add(objects[i]);
                }

                StoreSetBase randomObjectFromList = RandomUtil.GetRandomObjectFromList<StoreSetBase>(objectsInThisRoom);
                if (randomObjectFromList == null)
                {
                    return false;
                }
                base.StandardEntry(false);
                base.BeginCommodityUpdates();
                if (!this.Actor.RouteToObjectRadialRange(randomObjectFromList, 0f, UniversityWelcomeKit.kMaxRouteDistance))
                {
                    // CMStoreSet.PrintMessage("can't route");
                    return false;
                }
                this.Actor.RouteTurnToFace(randomObjectFromList.Position);
                List<ObjectGuid> objectsICanBuyInDisplay = randomObjectFromList.GetObjectIDsICanBuyInDisplay(this.Actor, base.Autonomous);
                RandomUtil.RandomizeListOfObjects<ObjectGuid>(objectsICanBuyInDisplay);

                if (objectsICanBuyInDisplay != null && objectsICanBuyInDisplay.Count > 0)
                {
                    ObjectGuid guid = objectsICanBuyInDisplay[0];
                    GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(guid);

                    this.Actor.RouteTurnToFace(gameObject.Position);
                    int priority = 100;
                    this.Actor.LookAtManager.SetInteractionLookAt(gameObject, priority, LookAtJointFilter.TorsoBones | LookAtJointFilter.HeadBones);
                    bool flag = RandomUtil.RandomChance01(StoreSetBase.kBrowseChanceOfDislikingObject);
                    ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.BalloonData(gameObject.GetThumbnailKey());
                    if (flag)
                    {
                        balloonData.LowAxis = ThoughtBalloonAxis.kDislike;
                    }
                    this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                    string state = "1";
                    if (flag)
                    {
                        state = RandomUtil.GetRandomStringFromList(new string[]
                            {
                                "3", 
                                "5", 
                                "CantStandArtTraitReaction"
                            });
                    }
                    else
                    {
                        state = RandomUtil.GetRandomStringFromList(new string[]
                            {
                                "0", 
                                "1", 
                                "2"
                            });
                    }
                    base.EnterStateMachine("viewobjectinteraction", "Enter", "x");
                    base.AnimateSim(state);
                    base.AnimateSim("Exit");
                    this.Actor.LookAtManager.ClearInteractionLookAt();
                }


                base.EndCommodityUpdates(true);
                base.StandardExit(false, false);
                return true;
            }
        }

        public sealed class TendRegisterManually : Interaction<Sim, StoreSetRegister> //PostureIdle
        {
            public sealed class Definition : InteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.TendRegisterManually>
            {
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    //IF the manual tending sim is stuck, release him.
                    if (target.mManualyTendingRegister == a.SimDescription.SimDescriptionId)
                    {
                        target.mTendingSimId = 0uL;
                        target.mManualyTendingRegister = 0uL;
                    }
                    
                    if (target.mManualyTendingRegister != 0uL && (target.mManualyTendingRegister != target.mTendingSimId))
                        target.mManualyTendingRegister = 0uL;                    

                    if (target.mManualyTendingRegister != 0uL || (target.mPreferredClerk != 0uL))// SimClock.IsTimeBetweenTimes(target.mOvenHoursStart, target.mOvenHoursEnd)))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Cleark already tending.");
                        return false;
                    }
                    return true;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    return CMStoreSet.LocalizeString("TendRegisterManually", new object[0] { });
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.TendRegisterManually.Definition();
            public override bool Run()
            {
                if (!this.Target.RouteMerchantToRegister(this.Actor))
                {
                    return false;
                }
                if (this.Target.mTendingSimId == 0uL)
                {
                    this.Target.ManualStartHour = SimClock.CurrentTime().Hour;
                    this.Target.mManualyTendingRegister = this.Actor.SimDescription.SimDescriptionId;

                    this.Target.mTendingSimId = this.Actor.SimDescription.SimDescriptionId;
                    if (this.Actor.SimDescription.SimDescriptionId == this.Target.mPreferredClerk)
                    {
                        this.Target.PayClerkForTodayIfHaventAlready(this.Actor);
                    }
                    bool flag = !(this.Actor.Posture is StoreSetRegister.DoingShopkeeperStuffPosture);
                    base.StandardEntry(flag);
                    base.EnterStateMachine("tendregister", "Enter", "x");
                    base.BeginCommodityUpdates();
                    base.AnimateSim("TendLoop");
                    bool flag2 = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.LoopCallback), this.mCurrentStateMachine);
                    Household household = this.Target.FindMoneyGetter();
                    if (flag2 && this.Actor.Household != household && !(this.Actor.Posture is StoreSetRegister.DoingShopkeeperStuffPosture))
                    {
                        this.Actor.ModifyFunds(StoreSetRegister.kMoneyForTendingRegister);
                    }
                    base.EndCommodityUpdates(flag2);
                    base.AnimateSim("Exit");
                    base.StandardExit(flag, flag);
                    this.Target.mTendingSimId = 0uL;
                    return flag2;
                }
                return false;
                //this.Target.ManualStartHour = SimClock.CurrentTime().Hour;
                //this.Target.mManualyTendingRegister = this.Actor.SimDescription.SimDescriptionId;
                //return base.Run();
            }

            public void LoopCallback(StateMachineClient smc, InteractionInstance.LoopData ld)
            {
                //Call next customer
                this.Target.CallInNextCustomer(this.Target, this.Actor);

                //if (ld.mLifeTime > (float)GalleryShopRegister.kSimMinBehindRegister)
                //{
                //    this.Actor.AddExitReason(ExitReason.Finished);
                //    return;
                //}
                //if (this.Actor.SimDescription.SimDescriptionId == this.Target.mPreferredClerk)
                //{
                //    if (!this.Actor.Household.IsActive)
                //        this.Actor.Motives.MaxEverything();

                //    //if (!SimClock.IsTimeBetweenTimes(this.Target.mOvenHoursStart, this.Target.mOvenHoursEnd))
                //    if (!CMStoreSet.IsStoreOpen(this.Target))
                //    {
                //        this.Actor.AddExitReason(ExitReason.Finished);
                //    }
                //}
                if (!this.Actor.IsSelectable && this.Actor.SimDescription.SimDescriptionId != this.Target.mPreferredClerk)
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                }
                //if (this.Actor.IsSelectable && this.Actor.Posture is GalleryShopRegister.DoingShopkeeperStuffPosture)
                //{
                //    if (this.Actor.SimDescription.SimDescriptionId == this.Target.mPreferredClerk)
                //    {
                //        this.Target.SummonClerk();
                //    }
                //    this.Actor.AddExitReason(ExitReason.Finished);
                //}
            }
            public override void Cleanup()
            {
                if (base.StandardEntryCalled)
                {
                    this.Target.mManualyTendingRegister = 0uL;
                    this.Target.mTendingSimId = 0uL;
                }
                base.Cleanup();
            }


        }

        public sealed class CancelTendingRegister : Interaction<Sim, StoreSetRegister>
        {
            public sealed class Definition : InteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.CancelTendingRegister>
            {
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.mManualyTendingRegister == 0uL)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("No sim tending register");
                        return false;
                    }

                    return true;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    return CMStoreSet.LocalizeString("CancelTendingRegister", new object[0] { });
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.CancelTendingRegister.Definition();
            public override bool Run()
            {
                //Find the tending sim 
                // CMStoreSet.PrintMessage(this.Target.mManualyTendingRegister.ToString());
                try
                {
                    if (this.Target.mManualyTendingRegister != 0uL)
                    {
                        this.Target.ManualEndHour = SimClock.CurrentTime().Hour;
                        SimDescription sd = CMStoreSet.ReturnSim(this.Target.mManualyTendingRegister);

                        if (sd != null && sd.CreatedSim != null)
                        {
                            sd.CreatedSim.PopPosture();
                            sd.CreatedSim.InteractionQueue.CancelAllInteractions();

                            SimDescription owner = null;
                            if (this.Target.Info.OwnerId != 0uL)
                                owner = CMStoreSet.ReturnSim(this.Target.Info.OwnerId);

                            //Pay wage if we are not the owner 
                            if (owner == null || (owner != null && owner.SimDescriptionId != this.Actor.SimDescription.SimDescriptionId))
                                CMStoreSet.PayWages(this.Actor, this.Target, this.Target.ManualStartHour, this.Target.ManualEndHour);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CMStoreSet.PrintMessage("Cancel Tending: " + ex.Message);
                }
                finally
                {
                    this.Target.mManualyTendingRegister = 0uL;
                }

                return true;
            }
        }

        #endregion

        #region Settings

        public sealed class ToggleOpenClose : ImmediateInteraction<IActor, StoreSetRegister>
        {
            [DoesntRequireTuning]
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.ToggleOpenClose>
            {
                //public override string[] GetPath(bool isFemale)
                //{
                //    return new string[]{
                //    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                //};
                //}
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    if (target.Info.Open)
                        return "Close Shop"; // CMStoreSet.LocalizeString("ToggleOpenClose", new object[0] { });
                    else
                        return "Open Shop";// CMStoreSet.LocalizeString("ToggleOpenClose", new object[0] { });
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.ToggleOpenClose.Definition();
            public override bool Run()
            {
                bool open = CMStoreSet.IsStoreOpen(this.Target);
                this.Target.Info.Open = !this.Target.Info.Open;

                this.Target.mManualyTendingRegister = 0uL;

                if (this.Target.Info.Open)
                    this.Target.RestartAlarms();
                else
                {
                    this.Target.DeleteAlarms();

                    //Stop tending
                    //if (open)
                    //{
                    //    SimDescription simDescription = SimDescription.Find(this.Target.mPreferredClerk);
                    //    if (simDescription != null)
                    //    {
                    //        SimDescription sd = CMStoreSet.ReturnSim(this.Target.mPreferredClerk);

                    //        if (sd != null && sd.CreatedSim != null)
                    //        {
                    //            sd.CreatedSim.PopPosture();
                    //        }
                    //    }
                    //}
                }

                return true;
            }
        }

        public sealed class HireClerk : ImmediateInteraction<IActor, StoreSetRegister>
        {
            [DoesntRequireTuning]
            public sealed class Definition : InteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.HireClerk>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return "Hire Clerck";// GalleryShopRegister.LocalizeString(false, "HireNewClerk", new object[0]);
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.mPreferredClerk == 0uL;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.HireClerk.Definition();
            public override bool Run()
            {

                SimDescription simDescription = CMStoreSet.ReturnSimsInHousehold(this.Actor.SimDescription, true, false);
                if (simDescription != null)
                {
                    this.Target.mPreferredClerk = simDescription.SimDescriptionId;

                    if (CMStoreSet.IsStoreOpen(this.Target))
                        this.Target.SummonClerk();

                    StyledNotification.Format format = new StyledNotification.Format("Clerck Hired: " + simDescription.FullName, StyledNotification.NotificationStyle.kGameMessagePositive);
                    StyledNotification.Show(format);
                }

                return true;
            }
        }

        public sealed class FireClerk : ImmediateInteraction<IActor, StoreSetRegister>
        {
            [DoesntRequireTuning]
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.FireClerk>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    SimDescription simDescription = SimDescription.Find(target.mPreferredClerk);
                    if (simDescription == null)
                    {
                        return "Fire Clerck";//  GalleryShopRegister.LocalizeString(false, "FireClerkNoName", new object[0]);
                    }
                    return "Fire Clerck: " + simDescription.FullName;// GalleryShopRegister.LocalizeString(simDescription.IsFemale, "FireClerk", new object[]
                    //{
                    //    simDescription.GetNameProxy()
                    //});
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.mPreferredClerk != 0uL;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.FireClerk.Definition();
            public override bool Run()
            {
                SimDescription simDescription = SimDescription.Find(this.Target.mPreferredClerk);
                if (simDescription != null)
                {
                    this.Target.mRecentlyFired = this.Target.mPreferredClerk;
                    
                    SimDescription sd = CMStoreSet.ReturnSim(this.Target.mPreferredClerk);

                    if (sd != null && sd.CreatedSim != null)
                        sd.CreatedSim.PopPosture();
                }
                this.Target.mPreferredClerk = 0uL;

                if (CMStoreSet.IsStoreOpen(this.Target))
                {
                    this.Target.mTendingSimId = 0uL;
                    this.Target.mManualyTendingRegister = 0uL;
                }

                StyledNotification.Format format;
                if (simDescription == null)
                {
                    format = new StyledNotification.Format("Fire Clerk", this.Target.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                }
                else
                {
                    format = new StyledNotification.Format("Fire Clerk: " + simDescription.FullName, this.Target.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                }
                StyledNotification.Show(format);
                return true;
            }
        }

        public sealed class SetWorkingHours : ImmediateInteraction<IActor, StoreSetRegister>
        {
            [DoesntRequireTuning]
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.SetWorkingHours>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("SetWorkingHours", new object[0] { });
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.SetWorkingHours.Definition();
            public override bool Run()
            {
                string hour = CMStoreSet.ShowDialogueNumbersOnly(
                    CMStoreSet.LocalizeString("SetStartHourTitle", new object[0] { }),
                    CMStoreSet.LocalizeString("SetStartHourDescription", new object[0] { }),
                    this.Target.mOvenHoursStart.ToString());

                float.TryParse(hour, out this.Target.mOvenHoursStart);

                hour = CMStoreSet.ShowDialogueNumbersOnly(
                    CMStoreSet.LocalizeString("SetEndHourTitle", new object[0] { }),
                    CMStoreSet.LocalizeString("SetEndHourDescription", new object[0] { }),
                    this.Target.mOvenHoursEnd.ToString());

                float.TryParse(hour, out this.Target.mOvenHoursEnd);

                this.Target.SummonClerk();


                return true;
            }
        }

        public sealed class SetPayPerHour : ImmediateInteraction<IActor, StoreSetRegister>
        {
            [DoesntRequireTuning]
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.SetPayPerHour>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("SetPayPerHour", new object[0] { });
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.SetPayPerHour.Definition();
            public override bool Run()
            {
                string pay = CMStoreSet.ShowDialogueNumbersOnly(
                    CMStoreSet.LocalizeString("SetPayTitle", new object[0] { }),
                    CMStoreSet.LocalizeString("SetPayDescription", new object[0] { }),
                    this.Target.Info.HourlyWage.ToString());
                int.TryParse(pay, out this.Target.Info.HourlyWage);

                return true;
            }
        }

        public class SetChangeToWorkOutfit : ImmediateInteraction<Sim, StoreSetRegister>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetRegister, SetChangeToWorkOutfit>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])};
                }
                public override string GetInteractionName(Sim a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    if (target.Info.ChangeToWorkOutfit)
                        return CMStoreSet.LocalizeString("DisableChangeToWorkOutfit", new object[0] { });
                    else
                        return CMStoreSet.LocalizeString("EnableChangeToWorkOutfit", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetChangeToWorkOutfit.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.Info.ChangeToWorkOutfit = !base.Target.Info.ChangeToWorkOutfit;
                }
                catch (Exception ex)
                {
                    CMStoreSet.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        public class SetPayWhenActive : ImmediateInteraction<Sim, StoreSetRegister>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetRegister, SetPayWhenActive>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])};
                }
                public override string GetInteractionName(Sim a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    if (target.Info.ChangeToWorkOutfit)
                        return CMStoreSet.LocalizeString("DisablePayWhenActive", new object[0] { });
                    else
                        return CMStoreSet.LocalizeString("EnablePayWhenActive", new object[0] { });
                }

                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new SetPayWhenActive.Definition();

            public override bool Run()
            {
                try
                {
                    base.Target.Info.PayWhenActive = !base.Target.Info.PayWhenActive;
                }
                catch (Exception ex)
                {
                    CMStoreSet.PrintMessage(ex.Message);
                }

                return true;
            }

        }

        public class StandOpenWithoutAnimation : ImmediateInteraction<Sim, StoreSetRegister>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, StoreSetRegister, StandOpenWithoutAnimation>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])};
                }
                public override string GetInteractionName(Sim a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("OpenInventory", new object[0] { });
                }
                public override bool Test(Sim actor, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;// !target.HasTreasureChestInfoData();
                }
            }
            public static InteractionDefinition Singleton = new StandOpenWithoutAnimation.Definition();
            public override bool Run()
            {
                HudModel.OpenObjectInventoryForOwner(this.Target);
                if (this.Target.InventoryComp == null)
                    StyledNotification.Show(new StyledNotification.Format("Inventory component null", StyledNotification.NotificationStyle.kGameMessagePositive));
                return true;
            }
        }

        public sealed class SetName : ImmediateInteraction<IActor, StoreSetRegister>
        {
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.SetName>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("SetName", new object[0] { });
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.SetName.Definition();
            public override bool Run()
            {
                this.Target.Info.RegisterName = CMStoreSet.ShowDialogue(
                    CMStoreSet.LocalizeString("SetNameTitle", new object[0] { }),
                    CMStoreSet.LocalizeString("SetNameDescription", new object[0] { }),
                    this.Target.Info.RegisterName);

                return true;
            }
        }

        public sealed class SetOwner : ImmediateInteraction<Sim, StoreSetRegister>
        {
            public sealed class Definition : ImmediateInteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.SetOwner>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("SetOwner", new object[0] { });
                }
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.SetOwner.Definition();
            public override bool Run()
            {
                try
                {
                    SimDescription sd = CMStoreSet.ReturnSimsInHousehold(this.Actor.SimDescription, true, true);

                    if (sd != null)
                    {
                        base.Target.Info.OwnerId = sd.SimDescriptionId;
                        base.Target.Info.OwnerName = sd.FullName;
                    }
                    else
                    {
                        base.Target.Info.OwnerId = 0uL;
                        base.Target.Info.OwnerName = string.Empty;
                        CMStoreSet.PrintMessage(CMStoreSet.LocalizeString("ResetOwner", new object[0] { }));
                    }
                }
                catch (Exception ex)
                {
                    CMStoreSet.PrintMessage(ex.Message);
                }

                return true;
            }
        }

        public sealed class ClearCustomerList : ImmediateInteraction<Sim, StoreSetRegister>
        {
            public sealed class Definition : ImmediateInteractionDefinition<Sim, StoreSetRegister, StoreSetRegister.ClearCustomerList>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override bool Test(Sim a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.Info.ShoppingData == null || (target.Info.ShoppingData != null && target.Info.ShoppingData.Count == 0))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(
                            CMStoreSet.LocalizeString("GrayNoCustomers", new object[0] { }));
                        return false;
                    }
                    return true;
                }
                public override string GetInteractionName(Sim actor, StoreSetRegister target, InteractionObjectPair iop)
                {
                    return CMStoreSet.LocalizeString("ClearCustomerList", new object[0] { });
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.ClearCustomerList.Definition();
            public override bool Run()
            {
                if (this.Target.Info.ShoppingData != null && this.Target.Info.ShoppingData.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(CMStoreSet.LocalizeString("ClearCustomersTitle", new object[0] { }));
                    sb.Append("\n");
                    foreach (KeyValuePair<SimDescription, int> item in this.Target.Info.ShoppingData)
                    {
                        sb.Append(item.Key.FullName);
                        sb.Append("\n");
                    }
                    CMStoreSet.PrintMessage(sb.ToString());
                }

                this.Target.Info.ShoppingData = new Dictionary<SimDescription, int>();
                return true;
            }
        }

        public sealed class SetServingPrice : ImmediateInteraction<IActor, StoreSetRegister>
        {
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.SetServingPrice>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("SetServingPriceTitle", new object[0] { });
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.SetServingPrice.Definition();
            public override bool Run()
            {
                int.TryParse(CMStoreSet.ShowDialogueNumbersOnly(
                    CMStoreSet.LocalizeString("SetServingPriceTitle", new object[0] { }),
                    CMStoreSet.LocalizeString("SetServingPriceDescription", new object[0] { }),
                    this.Target.Info.ServingPrice.ToString()), out this.Target.Info.ServingPrice);

                return true;
            }
        }

        public sealed class UnSpoil : ImmediateInteraction<IActor, StoreSetRegister>
        {
            public sealed class Definition : ActorlessInteractionDefinition<IActor, StoreSetRegister, StoreSetRegister.UnSpoil>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMStoreSet.LocalizeString(CMStoreSet.MenuSettingsPath, new object[0])
                };
                }
                public override string GetInteractionName(IActor a, StoreSetRegister target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("UnSpoil", new object[0] { });
                }
                public override bool Test(IActor a, StoreSetRegister target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new StoreSetRegister.UnSpoil.Definition();
            public override bool Run()
            {

                //All registers
                StoreSetRegister[] registers = Sims3.Gameplay.Queries.GetObjects<StoreSetRegister>(this.Target.LotCurrent);

                for (int i = 0; i < registers.Length; i++)
                {
                    StoreHelperClass.UnSpoil(registers[i], null, null, registers[i].Info.ServingPrice);
                }

                ////All rugs
                //ani_StoreRug[] rugs = Sims3.Gameplay.Queries.GetObjects<ani_StoreRug>(this.Target.LotCurrent);
                //for (int i = 0; i < rugs.Length; i++)
                //{
                //    int servingPrice = 25;
                //    if(rugs[i].Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                //    {
                //        StoreSetRegister register = CMStoreSet.ReturnRegister(rugs[i].Info.RegisterId, this.Target.LotCurrent);
                //        if (register != null)
                //            servingPrice = register.Info.ServingPrice;
                //    }
                //    StoreHelperClass.UnSpoil(null, null, rugs[i], servingPrice);
                //}

                ////All Pedestals
                //ani_StoreSetPedestal[] pedestals = Sims3.Gameplay.Queries.GetObjects<ani_StoreSetPedestal>(this.Target.LotCurrent);
                //for (int i = 0; i < pedestals.Length; i++)
                //{
                //    int servingPrice = 25;
                //    if (pedestals[i].Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                //    {
                //        StoreSetRegister register = CMStoreSet.ReturnRegister(pedestals[i].Info.RegisterId, this.Target.LotCurrent);
                //        if (register != null)
                //            servingPrice = register.Info.ServingPrice;
                //    }
                //  //  StoreHelperClass.UnSpoil(null, null, rugs[i], servingPrice);
                //}

                ////All Shelves
                //ani_StoreShelf[] shelfs = Sims3.Gameplay.Queries.GetObjects<ani_StoreShelf>(this.Target.LotCurrent);
                //for (int i = 0; i < shelfs.Length; i++)
                //{
                //    int servingPrice = 25;
                //    if (shelfs[i].Info.RegisterId != ObjectGuid.InvalidObjectGuid)
                //    {
                //        StoreSetRegister register = CMStoreSet.ReturnRegister(shelfs[i].Info.RegisterId, this.Target.LotCurrent);
                //        if (register != null)
                //            servingPrice = register.Info.ServingPrice;
                //    }
                //    //  StoreHelperClass.UnSpoil(null, null, rugs[i], servingPrice);
                //}
                return true;
            }
        }

        #endregion Interactions

        #region Stuff from Register

        public ulong mTendingSimId;
        public float mOvenHoursStart = 8f;
        public float mOvenHoursEnd = 18f;
        public float kOneMinuteInHours = 0.017f;
        public AlarmHandle mClerkJobAlarmEarly = AlarmHandle.kInvalidHandle;
        public AlarmHandle mClerkJobAlarmLate = AlarmHandle.kInvalidHandle;
        public AlarmHandle mClerkJobFetchMidJobAlarm = AlarmHandle.kInvalidHandle;
        public DateAndTime mLastPaid = DateAndTime.Invalid;
        public ulong mPreferredClerk;
        public ulong mRecentlyFired;
        public List<ulong> mSimsWantingToBuy = new List<ulong>();
        [TunableComment("Purchased item LTR Boost"), Tunable]
        public static float kPurchasedItemLTRBoost = 20f;
        [Tunable, TunableComment("Ambitious sim: funds boost %")]
        public static float kAmbitiousSimFundsBoost = 1.2f;
        [Tunable, TunableComment("Ambitious sim: LTR boost %")]
        public static float kAmbitiousSimLTRBoost = 1.3f;
        [TunableComment("Ambitious sim: mood boost"), Tunable]
        public static int kAmbitiousSimMoodBoost = 50;
        [Tunable, TunableComment("Range: int 1-x. Description: how long after sim plays look at before exiting browse interaction")]
        public static int kMaxBrowseMaxTimeInMinutes = 10;
        [TunableComment("Range:  float  Description:  Radius sim will route to object"), Tunable]
        public static float kBrowseRouteRadius = 1.5f;
        [TunableComment("Range:  1-x  Description:  Time in Sim minutes sim will stand behind counter playing randomd idle loop"), Tunable]
        public static int kSimMinBehindRegister = 60;
        [Tunable, TunableComment("Hours before the venue hours to send the chef to work")]
        public static float kChefHoursBeforeWorkToHeadToWork = 0.5f;
        [Tunable, TunableComment("Hours in the day a shift can start")]
        public static float[] kWorkHourStart = new float[]
		{
			8f, 
			11f, 
			15f
		};
        [TunableComment("Number of hours to work in a shift, if not a venue"), Tunable]
        public static float kWorkHourDuration = 10f;
        [Tunable, TunableComment("Keep verifying that the chef is actually doing chef things this many minutes.  Probably shouldn't be modified.")]
        public static float kChefAlarmContinuousSummonFrequncy = 15f;
        [TunableComment("Amount to pay a shopkeeper each day"), Tunable]
        public static int kCostOfPaidShopkeeperPerShift = 100;
        [TunableComment("Range:  0 - 100  Description:  Min Route Distance"), Tunable]
        public static float kMinDistance = 0.7f;
        [TunableComment("Range:  0 - 100  Description:  Max Route Distance"), Tunable]
        public static float kMaxDistance = 1f;
        [Tunable, TunableComment("The arc/cone's angle for finding a suitable spot for a sim that will stand to buy items. Units: Radians. Valid range [0, Pi]. Initial GPE Default: (Pi)/6 (30 degreees.")]
        public static float kConeAngle = 0.5235988f;
        [Tunable, TunableComment("Percent chance that a clerk will wander the store after completing their current interaction if no customers are waiting. Default: .60")]
        public static float kClerkChanceOfWanderingStore = 0.6f;
        [TunableComment("Number of minutes to wait for a wandering clerk when I want to buy something.  Default: 60"), Tunable]
        public static float kBuyerMinutesToWaitForWanderingClerk = 60f;
        [Tunable, TunableComment("Amount of money to give for the active sim tending a register.")]
        public static int kMoneyForTendingRegister = 10;
        public RegisterType GetRegisterType
        {
            get
            {
                return RegisterType.General;
            }
        }

        #endregion

        public void CallInNextCustomer(StoreSetRegister register, Sim sim)
        {
            if (register.Info.ShoppingData.Count > 0 && register.Info.CurrentCustomer == 0uL)
            {
                //Call in the next customer
                SimDescription customer = CMStoreSet.ReturnFirstCustomer(register.Info.ShoppingData, register.LotCurrent);

                if (customer != null)
                {
                    register.Info.CurrentCustomer = customer.SimDescriptionId;
                    if (customer.CreatedSim != null && customer.CreatedSim.LotCurrent == register.LotCurrent)
                    {
                        //If the cashier is not the customer, add interaction
                        if (sim.SimDescription.SimDescriptionId != customer.SimDescriptionId)
                        {
                            //If the customer is not working the store, add interaction, otherwise pay 
                            if (!(customer.CreatedSim.Posture is StoreSetRegister.DoingShopkeeperStuffPosture))
                            // if (register.mPreferredClerk != customer.SimDescriptionId)
                            {
                                InteractionInstance interactionInstance = StoreSetRegister.PayForItems.Singleton.CreateInstance(register, customer.CreatedSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
                                if (interactionInstance != null)
                                {
                                    customer.CreatedSim.InteractionQueue.Add(interactionInstance);
                                }
                            }
                            else
                            {
                                CMStoreSet.PrintMessage(CMStoreSet.LocalizeString("PayForOwnPurchase", new object[] { customer.FullName }));

                                sim.ModifyFunds(-register.Info.ShoppingData[customer]);
                                if (register.Info.OwnerId != 0uL)
                                {
                                    SimDescription owner = CMStoreSet.ReturnSim(register.Info.OwnerId);
                                    if (owner != null)
                                    {
                                        owner.ModifyFunds(register.Info.ShoppingData[customer]);
                                    }
                                }

                                //Remove customer from list
                                register.Info.ShoppingData.Remove(customer);
                                register.Info.CurrentCustomer = 0uL;
                            }
                        }
                        else
                        {
                            //Pay for own purhcase 
                            CMStoreSet.PrintMessage(CMStoreSet.LocalizeString("PayForOwnPurchase", new object[] { customer.FullName }));

                            if (register.Info.ShoppingData.ContainsKey(sim.SimDescription))
                            {
                                sim.ModifyFunds(-register.Info.ShoppingData[sim.SimDescription]);
                                if (register.Info.OwnerId != 0uL)
                                {
                                    SimDescription owner = CMStoreSet.ReturnSim(register.Info.OwnerId);
                                    if (owner != null)
                                    {
                                        owner.ModifyFunds(register.Info.ShoppingData[sim.SimDescription]);
                                    }
                                }

                                //Remove customer from list
                                register.Info.ShoppingData.Remove(sim.SimDescription);
                                register.Info.CurrentCustomer = 0uL;
                            }
                        }
                    }
                    else
                    {
                        CMStoreSet.PrintMessage(CMStoreSet.LocalizeString("CustomerNotOnLot", new object[] { customer.FullName }));
                        if (customer != null)
                        {
                            //Remove customer from list
                            register.Info.ShoppingData.Remove(customer);
                            register.Info.CurrentCustomer = 0uL;
                        }
                    }

                }
            }
            register.Info.CurrentCustomer = 0uL;

        }

        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CMStoreSet.LocalizeString("TTRegisterName", new object[0] { }));
            sb.Append(this.Info.RegisterName);
            sb.Append("\n");

            //Working hours
            if (this.Info.Open)
            {
                sb.Append(CMStoreSet.LocalizeString("TTHours", new object[0] { }));
                sb.Append(this.mOvenHoursStart);
                sb.Append(":00 - ");
                sb.Append(this.mOvenHoursEnd);
                sb.Append(":00");
                sb.Append("\n");
            }
            else
            {
                sb.Append("The shop is closed");
                sb.Append("\n");
            }

            //Owner
            if (this.Info.OwnerId != 0uL)
            {
                sb.Append(CMStoreSet.LocalizeString("TTOwner", new object[0] { }));
                sb.Append(this.Info.OwnerName);
                sb.Append("\n");
            }
            else
            {
                sb.Append(CMStoreSet.LocalizeString("TTNoOwner", new object[0] { }));
            }

            //Customers
            if (this.Info.ShoppingData != null && this.Info.ShoppingData.Count > 0)
            {
                sb.Append(CMStoreSet.LocalizeString("TTCustomersWaiting", new object[] { this.Info.ShoppingData.Count.ToString() }));
            }


            return new SimpleTextTooltip(sb.ToString());
        }

        public override void OnCreation()
        {
            base.OnCreation();

            Info = new RegisterInfo();
        }
        public override void OnStartup()
        {
            //Inventory
            base.AddComponent<InventoryComponent>(new object[0]);

            if (this.Inventory != null)
                this.Inventory.IgnoreInventoryValidation = true;

            base.AddComponent<TreasureComponent>(new object[0]);

            //Settings            
            base.AddInteraction(StoreSetRegister.SetName.Singleton);
            base.AddInteraction(StoreSetRegister.SetOwner.Singleton);
            base.AddInteraction(StoreSetRegister.SetServingPrice.Singleton);
            base.AddInteraction(StoreSetRegister.UnSpoil.Singleton);
            base.AddInteraction(StoreSetRegister.ToggleOpenClose.Singleton);
            base.AddInteraction(StoreSetRegister.StandOpenWithoutAnimation.Singleton);

            //  base.AddInteraction(StoreSetRegister.CheckSalesRecords.Singleton);
            base.AddInteraction(StoreSetRegister.FireClerk.Singleton);
            base.AddInteraction(StoreSetRegister.HireClerk.Singleton);

            base.AddInteraction(StoreSetRegister.SetPayPerHour.Singleton);
            base.AddInteraction(StoreSetRegister.SetPayWhenActive.Singleton);
            base.AddInteraction(StoreSetRegister.SetChangeToWorkOutfit.Singleton);
            base.AddInteraction(StoreSetRegister.SetWorkingHours.Singleton);
            base.AddInteraction(StoreSetRegister.ClearCustomerList.Singleton);

            //Clerck - manual
            base.AddInteraction(TendRegisterManually.Singleton);

            //Customer
            base.AddInteraction(StoreSetRegister.PayForItems.Singleton);

            if (this.Info.Open)
                this.RestartAlarms();

            //TODO:Poista tämä ja lisää oma browse
            base.OnStartup();

        }

        #region Other Methods

        public void RetestHoursOfOperation()
        {
            if (!base.LotCurrent.IsCommunityLot)
            {
                return;
            }
            //if (this.Info.Open)
            {
                float num = this.mOvenHoursStart;
                float num2 = this.mOvenHoursEnd;
                if ((num != this.mOvenHoursStart || num2 != this.mOvenHoursEnd))
                {
                    this.mOvenHoursStart = num;
                    this.mOvenHoursEnd = num2;
                    this.RestartAlarms();
                }
            }
        }
        public void DeleteAlarms()
        {
            if (this.mClerkJobAlarmEarly != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mClerkJobAlarmEarly);
                this.mClerkJobAlarmEarly = AlarmHandle.kInvalidHandle;
            }
            if (this.mClerkJobAlarmLate != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mClerkJobAlarmLate);
                this.mClerkJobAlarmLate = AlarmHandle.kInvalidHandle;
            }
            if (this.mClerkJobFetchMidJobAlarm != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mClerkJobFetchMidJobAlarm);
                this.mClerkJobFetchMidJobAlarm = AlarmHandle.kInvalidHandle;
            }
        }
        public void RestartAlarms()
        {
            this.DeleteAlarms();
            float hour = SimClock.CurrentTime().Hour;
            float num = this.mOvenHoursStart;
            float num2 = num - hour - StoreSetRegister.kChefHoursBeforeWorkToHeadToWork;
            float num3 = num - hour + this.kOneMinuteInHours;
            while (num2 < 0f)
            {
                num2 += 24f;
            }
            while (num3 < 0f)
            {
                num3 += 24f;
            }
            this.mClerkJobAlarmEarly = base.AddAlarm(num2, TimeUnit.Hours, new AlarmTimerCallback(this.SummonClerk), "Gallery clerk-summon alarm", AlarmType.AlwaysPersisted);
            this.mClerkJobAlarmLate = base.AddAlarm(num3, TimeUnit.Hours, new AlarmTimerCallback(this.SummonClerk), "Gallery clerk-summon alarm", AlarmType.AlwaysPersisted);
            //if (SimClock.IsTimeBetweenTimes(this.mOvenHoursStart, this.mOvenHoursEnd))
            if (CMStoreSet.IsStoreOpen(this))
            {
                this.mClerkJobFetchMidJobAlarm = base.AddAlarm(StoreSetRegister.kChefAlarmContinuousSummonFrequncy, TimeUnit.Minutes, new AlarmTimerCallback(this.SummonClerk), "Gallery clerk-summon alarm", AlarmType.AlwaysPersisted);
            }
        }
        public void PayClerkForTodayIfHaventAlready(Sim actor)
        {
            bool flag = true;
            if (this.mLastPaid != DateAndTime.Invalid && SimClock.ElapsedTime(TimeUnit.Hours, this.mLastPaid) < 2f * StoreSetRegister.kWorkHourDuration)
            {
                flag = false;
            }
            Household household = this.FindMoneyGetter();
            if (flag && household != null)
            {
                //Don't pay if the clerck lives with the owner
                if (actor.Household != household)
                {
                    int num = StoreSetRegister.kCostOfPaidShopkeeperPerShift;
                    int familyFunds = household.FamilyFunds;
                    if (num > familyFunds)
                    {
                        num = familyFunds;
                        StyledNotification.Format format = new StyledNotification.Format("Insufficiant funds. \nCannot pay clerk.", actor.ObjectId, base.ObjectId, StyledNotification.NotificationStyle.kGameMessageNegative);
                        StyledNotification.Show(format);
                        this.mRecentlyFired = this.mPreferredClerk;
                        this.mPreferredClerk = 0uL;
                    }
                    household.ModifyFamilyFunds(-num);
                    actor.ModifyFunds(num);
                }
                this.mLastPaid = SimClock.CurrentTime();
            }
        }
        public Household FindMoneyGetter()
        {
            Household household = base.LotCurrent.Household;
            if (household == null)
            {
                List<PropertyData> list = RealEstateManager.AllPropertiesFromAllHouseholds();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null && base.LotCurrent.LotId == list[i].LotId && list[i].Owner != null && list[i].Owner.OwningHousehold != null)
                    {
                        household = list[i].Owner.OwningHousehold;
                        break;
                    }
                }
            }
            return household;
        }
        public void SoldItemImproveLTR(Sim sim)
        {
            Sim createdSim = SimDescription.GetCreatedSim(this.mTendingSimId);
            if (createdSim != null)
            {
                Relationship relationship = Relationship.Get(sim, createdSim, true);
                if (relationship != null)
                {
                    LongTermRelationshipTypes currentLTR = relationship.LTR.CurrentLTR;
                    float num = StoreSetRegister.kPurchasedItemLTRBoost;
                    if (sim.HasTrait(TraitNames.Ambitious))
                    {
                        num *= StoreSetRegister.kAmbitiousSimLTRBoost;
                    }
                    relationship.LTR.UpdateLiking(num);
                    LongTermRelationshipTypes currentLTR2 = relationship.LTR.CurrentLTR;
                    SocialComponent.SetSocialFeedback(CommodityTypes.Friendly, sim, true, 0, currentLTR, currentLTR2);
                    SocialComponent.SetSocialFeedback(CommodityTypes.Friendly, createdSim, true, 0, currentLTR, currentLTR2);
                }
                if (createdSim.HasTrait(TraitNames.Ambitious))
                {
                    createdSim.MoodManager.AddEffect(MoodAxis.Happy, StoreSetRegister.kAmbitiousSimMoodBoost);
                }
            }
        }
        public bool ShowShoppingDialog(Sim sim)
        {
            Dictionary<string, List<StoreItem>> dictionary = this.ItemDictionary(sim);
            if (dictionary.Count == 0)
            {
                return false;
            }
            ShoppingModel.CurrentStore = this;
            ShoppingRabbitHole.StartShopping(sim, dictionary, 0f, 0f, 0, null, sim.Inventory, null, new ShoppingRabbitHole.ShoppingFinished(this.FinishedCallBack), new ShoppingRabbitHole.CreateSellableCallback(this.CreateSellableObjectsList), this.GetRegisterType != RegisterType.General);
            return true;
        }
        public void FinishedCallBack(Sim customer, bool boughtItemForFamilyInventory)
        {
            if (customer != null && boughtItemForFamilyInventory)
            {
                customer.ShowTNSIfSelectable("The item purchased was added to your family inventory.", StyledNotification.NotificationStyle.kGameMessagePositive);
                this.SoldItemImproveLTR(customer);
            }
            this.ShoppingCompleted(customer);
        }
        public void ShoppingCompleted(Sim customer)
        {
        }
        public string GetBrowseMessage(Sim actor)
        {
            Dictionary<string, List<StoreItem>> dictionary = this.ItemDictionary(actor);
            int num = 0;
            if (dictionary.Count != 0)
            {
                List<StoreItem> list = dictionary["All"];
                if (list != null)
                {
                    num = list.Count;
                }
            }
            return "Items on sale: " + num;
        }
        public bool InteractionTestAvailability()
        {
            if (this.mTendingSimId != 0uL)
            {
                return true;
            }
            if (this.mPreferredClerk != 0uL)
            {
                Sim createdSim = SimDescription.GetCreatedSim(this.mPreferredClerk);
                if (createdSim != null && createdSim.Posture is StoreSetRegister.DoingShopkeeperStuffPosture)
                {
                    return true;
                }
            }
            return false;
        }
        public void SummonClerk()
        {
            this.RetestHoursOfOperation();
            this.RestartAlarms();
            if (!base.InWorld || base.InInventory)
            {
                return;
            }
            if (this.mPreferredClerk == 0uL)
            {
                return;
            }

            if (!CMStoreSet.IsStoreOpen(this))
            {
                //CMStoreSet.PrintMessage("Store closed, should exit");
                return;
            }
            SimDescription simDescription = CMStoreSet.ReturnSim(this.mPreferredClerk); //SimDescription.Find(this.mPreferredClerk);
            //if (!this.CanBeActiveClerk(simDescription))
            //{
            //    simDescription = null;
            //}
            if (simDescription == null)
            {

                this.mPreferredClerk = 0uL;
                this.DeleteAlarms();

                CMStoreSet.PrintMessage("Sim desc null/ Alarm deleted for: " + this.Info.RegisterName);
                //simDescription = this.HireNewClerk();
                //StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString(simDescription.IsFemale, "Store/Objects/GalleryRegister:ClerkSwitched", new object[]
                //{
                //    simDescription.GetNameProxy()
                //}), base.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
                //StyledNotification.Show(format);
            }
            //if (!this.AnyOneHome(simDescription))
            //{
            //    return;
            //}
            Sim sim = simDescription.CreatedSim;
            if (sim == null)
            {
                sim = simDescription.Instantiate(base.LotCurrent);

                //InteractionInstance instance = StoreSetRegister.TendRegister.Singleton.CreateInstance(this, sim, new InteractionPriority(InteractionPriorityLevel.Autonomous), false, true);
                //sim.InteractionQueue.AddAfterCheckingForDuplicates(instance);

                //Tending Posture
                StoreSetRegister.DoingShopkeeperStuffPosture posture = new StoreSetRegister.DoingShopkeeperStuffPosture(sim, this, null);
                sim.Posture = posture;
            }
            if (sim == null)
            {
                return;
            }
            this.mTendingSimId = sim.SimDescription.SimDescriptionId;
            if (!(sim.Posture is StoreSetRegister.DoingShopkeeperStuffPosture))
            {
                this.mManualyTendingRegister = this.mPreferredClerk;
                sim.GreetSimOnLot(base.LotCurrent);
                InteractionInstance entry = StoreSetRegister.PostureIdle.Singleton.CreateInstance(this, sim, new InteractionPriority(InteractionPriorityLevel.High), false, false);
                sim.InteractionQueue.AddNext(entry);
            }
        }
        public bool CanBeActiveClerk(SimDescription desc)
        {
            if (desc == null)
            {
                return false;
            }
            Sim activeActor = Sim.ActiveActor;
            if (activeActor != null && activeActor.Household == desc.Household)
            {
                return false;
            }
            if (desc.CreatedSim != null)
            {
                if (desc.CreatedSim.Posture is StoreSetRegister.DoingShopkeeperStuffPosture)
                {
                    return true;
                }
                if (desc.CreatedSim.Posture.GetType().ToString().Equals("Sims3.Store.Objects.IndustrialOven+DoingChefStuffPosture"))
                {
                    return false;
                }
            }
            return desc.IsHuman && !desc.TeenOrBelow && !desc.Elder && !desc.IsServicePerson && !desc.HasActiveRole && desc.AssignedRole == null && desc.Occupation == null && !desc.OccultManager.HasAnyOccultType() && !desc.IsZombie && !desc.IsGhost;
        }
        public bool AnyOneHome(SimDescription desc)
        {
            if (!base.LotCurrent.IsResidentialLot)
            {
                return true;
            }
            if (desc.LotHome == base.LotCurrent)
            {
                return true;
            }
            Household household = base.LotCurrent.Household;
            if (household != null)
            {
                foreach (Sim current in household.Sims)
                {
                    if (current.IsAtHome)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        public bool RouteCustomerToRegister(Sim actor)
        {
            RadialRangeDestination radialRangeDestination = new RadialRangeDestination();
            radialRangeDestination.mTargetObject = this;
            radialRangeDestination.mCenterPoint = this.Position;
            radialRangeDestination.mConeVector = -base.ForwardVector;
            radialRangeDestination.mfConeAngle = StoreSetRegister.kConeAngle;
            radialRangeDestination.mfMinRadius = StoreSetRegister.kMinDistance;
            radialRangeDestination.mfMaxRadius = StoreSetRegister.kMaxDistance;
            radialRangeDestination.mFacingPreference = RouteOrientationPreference.TowardsObject;
            radialRangeDestination.ScoreFunctionWeights[(int)((UIntPtr)6)] = 1f;
            Route route = actor.CreateRoute();
            route.AddDestination(radialRangeDestination);
            route.SetValidRooms(base.LotCurrent.LotId, new int[]
			{
				base.RoomId
			});
            route.SetOption(Route.RouteOption.DoLineOfSightCheckUserOverride, true);
            route.Plan();
            return actor.DoRoute(route);
        }
        public bool RouteMerchantToRegister(Sim actor)
        {
            Counter counter = base.Parent as Counter;
            return counter != null && counter.RouteToObjectOnSurface(actor, this);
        }
        public void AutonomousBuyItem(Sim actor)
        {
            Dictionary<string, List<StoreItem>> dictionary = this.ItemDictionary(actor);
            if (dictionary.Count == 0)
            {
                return;
            }
            List<StoreItem> list = dictionary["All"];
            if (list == null)
            {
                return;
            }
            if (list.Count == 0)
            {
                return;
            }
            RandomUtil.RandomizeListOfObjects<StoreItem>(list);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].DisplayBasePrice <= actor.FamilyFunds)
                {
                    StoreSetRegister.SellableObjectData sellableObjectData = (StoreSetRegister.SellableObjectData)list[i].CustomData;
                    GameObject src = GlobalFunctions.ConvertGuidToObject<GameObject>(sellableObjectData.sellable);
                    StoreSetBase galleryShopBase = GlobalFunctions.ConvertGuidToObject<StoreSetBase>(sellableObjectData.shop);
                    if (galleryShopBase != null && actor != null)
                    {
                        IGameObject gameObject = galleryShopBase.MakeClone(src);
                        if (gameObject != null && !(gameObject is FailureObject))
                        {
                            bool flag = false;
                            bool flag2 = false;
                            if (gameObject.IsLiveDraggingEnabled() && !gameObject.InUse && gameObject.ItemComp != null && gameObject.ItemComp.CanAddToInventory(actor.Inventory) && actor.Inventory.CanAdd(gameObject) && actor.Inventory.TryToAdd(gameObject))
                            {
                                flag = true;
                            }
                            else
                            {
                                if (!gameObject.InUse && actor.Household.SharedFamilyInventory.Inventory.TryToAdd(gameObject))
                                {
                                    flag2 = true;
                                }
                            }
                            bool flag3 = flag || flag2;
                            if (flag3)
                            {
                                if (flag2)
                                {
                                    actor.ShowTNSIfSelectable(StoreSetBase.LocalizeString(actor.IsFemale, "PlacedInFamilyInventory", new object[]
									{
										actor, 
										gameObject
									}), StyledNotification.NotificationStyle.kGameMessagePositive);
                                }
                                else
                                {
                                    actor.ShowTNSIfSelectable(StoreSetBase.LocalizeString(actor.IsFemale, "PlacedInPersonalInventory", new object[]
									{
										actor, 
										gameObject
									}), StyledNotification.NotificationStyle.kGameMessagePositive);
                                }
                                //galleryShopBase.GiveMarkupBuffs(actor, gameObject);
                                int num = galleryShopBase.ComputeFinalPriceOnObject(sellableObjectData.sellable, this.Info.ServingPrice);
                                actor.ModifyFunds(-num);
                                galleryShopBase.GiveLotOwnerMoney(num, actor);
                                galleryShopBase.AccumulateRevenue(num);
                                return;
                            }
                        }
                    }
                }
            }
        }
        public bool BuyAndEatFood(Sim sim, bool autonomous)
        {
            return true;
        }
        public Dictionary<string, List<StoreItem>> ItemDictionary(Sim sim)
        {
            Dictionary<string, List<StoreItem>> dictionary = new Dictionary<string, List<StoreItem>>();
            List<StoreItem> list = new List<StoreItem>();
            StoreSetBase[] objects = Sims3.Gameplay.Queries.GetObjects<StoreSetBase>(base.LotCurrent);
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    List<ObjectGuid> objectsICanBuyInDisplay = objects[i].GetObjectIDsICanBuyInDisplay(sim, false);
                    for (int j = 0; j < objectsICanBuyInDisplay.Count; j++)
                    {
                        GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(objectsICanBuyInDisplay[j]);
                        if (gameObject != null)
                        {
                            StoreSetRegister.SellableObjectData sellableObjectData = new StoreSetRegister.SellableObjectData(objects[i].ObjectId, objectsICanBuyInDisplay[j], objects[i].ComputeFinalPriceOnObject(objectsICanBuyInDisplay[j], this.Info.ServingPrice), sim.SimDescription.SimDescriptionId);
                            list.Add(new StoreItem(gameObject.CatalogName, (float)sellableObjectData.cost, sellableObjectData, gameObject.GetThumbnailKey(), sellableObjectData.sellable.ToString(), new CreateObjectCallback(this.CreateShopCloneCallback), null, null)
                            {
                                PlaceInFamilyInventory = true
                            });
                        }
                    }
                }
            }
            dictionary.Add("All", list);
            return dictionary;
        }
        public ObjectGuid CreateShopCloneCallback(object customData, ref Simulator.ObjectInitParameters initData, Quality quality)
        {
            ObjectGuid result = ObjectGuid.InvalidObjectGuid;
            StoreSetRegister.SellableObjectData sellableObjectData = (StoreSetRegister.SellableObjectData)customData;
            GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(sellableObjectData.sellable);
            if (gameObject == null)
            {
                return result;
            }
            StoreSetBase galleryShopBase = GlobalFunctions.ConvertGuidToObject<StoreSetBase>(sellableObjectData.shop);
            Sim createdSim = SimDescription.GetCreatedSim(sellableObjectData.simDescriptionId);
            if (galleryShopBase != null && createdSim != null)
            {
                IGameObject gameObject2 = galleryShopBase.MakeClone(gameObject);
                result = gameObject2.ObjectId;
                //galleryShopBase.GiveMarkupBuffs(createdSim, gameObject2);
                galleryShopBase.GiveLotOwnerMoney(sellableObjectData.cost, createdSim);
                galleryShopBase.AccumulateRevenue(sellableObjectData.cost);
            }
            return result;
        }
        public List<ISellableUIItem> CreateSellableObjectsList(Sim customer)
        {
            return new List<ISellableUIItem>();
        }

        #endregion
    }
}
