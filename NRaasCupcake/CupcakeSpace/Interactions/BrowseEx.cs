using NRaas.CommonSpace.Helpers;
using NRaas.CupcakeSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Store.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace.Interactions
{
    public class BrowseEx : CraftersConsignment.Browse, Common.IPreLoad
    {
        //static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<CraftersConsignment, CraftersConsignment.Browse.Definition, Definition>(false);

            if (Common.AssemblyCheck.IsInstalled("NRaasEconomizer")) return;

            //sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            if (!base.Actor.RouteToObjectRadialRange(base.Target, 0f, base.Target.MaxProximityBeforeSwiping()))
            {
                //Honestly just plain annoying to watch, especially since sims love this interaction so much
                //base.Actor.PlayRouteFailure();
                return false;
            }
            base.Actor.RouteTurnToFace(base.Target.Position);
            base.StandardEntry();
            base.BeginCommodityUpdates();
            List<ObjectGuid> objectsICanBuyInDisplay = DisplayHelper.GetObjectsICanBuyInDisplay(base.Actor, base.Target);
            RandomUtil.RandomizeListOfObjects<ObjectGuid>(objectsICanBuyInDisplay);
            for (int i = 0; base.Actor.HasNoExitReason() && (i < objectsICanBuyInDisplay.Count); i++)
            {
                ObjectGuid guid = objectsICanBuyInDisplay[i];
                GameObject target = GlobalFunctions.ConvertGuidToObject<GameObject>(guid);
                if (target != null)
                {
                    base.Actor.RouteTurnToFace(target.Position);
                    int priority = 100;
                    base.Actor.LookAtManager.SetInteractionLookAt(target, priority, LookAtJointFilter.HeadBones | LookAtJointFilter.TorsoBones);
                    bool flag = RandomUtil.RandomChance01(CraftersConsignment.kBrowseChanceOfDislikingObject);
                    ThoughtBalloonManager.BalloonData bd = new ThoughtBalloonManager.BalloonData(target.GetThumbnailKey());
                    if (flag)
                    {
                        bd.LowAxis = ThoughtBalloonAxis.kDislike;
                    }
                    base.Actor.ThoughtBalloonManager.ShowBalloon(bd);
                    string state = "1";
                    if (flag)
                    {
                        state = RandomUtil.GetRandomStringFromList(new string[] { "3", "5", "CantStandArtTraitReaction" });
                    }
                    else
                    {
                        state = RandomUtil.GetRandomStringFromList(new string[] { "0", "1", "2" });
                    }
                    base.EnterStateMachine("viewobjectinteraction", "Enter", "x");
                    base.AnimateSim(state);
                    base.AnimateSim("Exit");
                    base.Actor.LookAtManager.ClearInteractionLookAt();
                }
            }
            if (base.Autonomous && !base.Actor.IsSelectable)
            {
                float chance = CraftersConsignment.kBrowseBaseChanceOfBuyingObjectWithoutSale + base.Target.mSaleDiscount;
                if (RandomUtil.RandomChance01(chance))
                {
                    List<ObjectGuid> randomList = DisplayHelper.GetObjectsICanBuyInDisplay(base.Actor, base.Target);
                    if (randomList.Count > 0)
                    {
                        ObjectGuid randomObjectFromList = RandomUtil.GetRandomObjectFromList<ObjectGuid>(randomList);
                        if (randomObjectFromList != ObjectGuid.InvalidObjectGuid)
                        {
                            PurchaseItemEx.Definition continuationDefinition = null;
                            if (Actor.Motives.IsHungry())
                            {
                                PreparedFood food = GlobalFunctions.ConvertGuidToObject<PreparedFood>(randomObjectFromList);
                                if (food != null)
                                {
                                    continuationDefinition = new PurchaseItemEx.BuyFoodDefinition(food);
                                }
                            }
                            if (continuationDefinition == null)
                            {
                                continuationDefinition = new PurchaseItemEx.Definition(randomObjectFromList, false);
                            }
                            base.TryPushAsContinuation(continuationDefinition);
                        }
                    }
                }
            }
            if (!base.Autonomous)
            {
                List<ObjectGuid> list3 = DisplayHelper.GetObjectsICanBuyInDisplay(base.Actor, base.Target);
                if (objectsICanBuyInDisplay.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine(CraftersConsignment.LocalizeString(base.Actor.IsFemale, "BrowseItemsForPurchaseHeading", new object[0]));
                    for (int j = 0; j < list3.Count; j++)
                    {
                        GameObject obj3 = GlobalFunctions.ConvertGuidToObject<GameObject>(list3[j]);
                        if (obj3 != null)
                        {
                            ServingContainer container = obj3 as ServingContainer;
                            if (container != null)
                            {
                                builder.AppendLine(CraftersConsignment.LocalizeString(base.Actor.IsFemale, "BrowseLineItem", new object[] { container.CookingProcess.RecipeNameFinal, DisplayHelper.ComputeFinalPriceOnObject(list3[j]) }));
                            }
                            else
                            {
                                Common.Notify("BrowseEx for " + obj3.CatalogName);
                                builder.AppendLine(CraftersConsignment.LocalizeString(base.Actor.IsFemale, "BrowseLineItem", new object[] { obj3.CatalogName, DisplayHelper.ComputeFinalPriceOnObject(list3[j]) }));
                            }
                        }
                    }
                    base.Actor.ShowTNSIfSelectable(builder.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive);
                }
            }
            base.EndCommodityUpdates(true);
            base.StandardExit();
            return true;
        }

        public new class Definition : CraftersConsignment.Browse.Definition
        {
            /*public override string GetInteractionName(Sim actor, CraftersConsignment target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }*/

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new BrowseEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, CraftersConsignment target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (target.Charred)
                {
                    return false;
                }
                if (DisplayHelper.GetObjectsICanBuyInDisplay(a, target).Count == 0)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CraftersConsignment.LocalizeString(a.IsFemale, "NothingOnDisplay", new object[0]));
                    return false;
                }
                return true;
            }
        }
    }
}