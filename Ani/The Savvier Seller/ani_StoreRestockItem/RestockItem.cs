using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Interactions;
using Sims3.UI;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using Sims3.Gameplay.Objects.Decorations.Mimics;
using ani_StoreRestockItem;
using Sims3.Gameplay.Abstracts;
using System.Text;
using ani_StoreSetRegister;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreSetRegister;
using ani_StoreSetBase;
using System.Collections.Generic;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;

namespace Sims3.Gameplay.Objects.TombObjects.ani_StoreRestockItem
{
    public class RestockItem : GameObject
    {
        public RestockInfo info;

        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append(this.info.Name);
            //sb.Append(" ");
            //sb.Append(this.info.Price.ToString());
            //sb.Append(" §");
            return new SimpleTextTooltip(CMStoreSet.LocalizeString("RestockItemTooltip", new object[]{this.info.Name, this.info.Price.ToString()}));
        }

        public override void OnCreation()
        {
            base.OnCreation();

            info = new RestockInfo();
        }

        public override void OnStartup()
        {
            //Get ingredient data
            if (info.Type == ItemType.Ingredient)
            {
                Simulator.ObjectInitParameters objectInitParameters = Simulator.GetObjectInitParameters(base.ObjectId);
                bool trySetModelToDefault = objectInitParameters != null;
                IngredientInitParameters ingredientInitParameters = objectInitParameters as IngredientInitParameters;
                if (ingredientInitParameters != null)
                {
                    IngredientData ingredientData = ingredientInitParameters.IngredientData;
                    info.IngData = ingredientData;
                    info.IngredientKey = ingredientData.Key;
                }
            }

            base.RemoveAllInteractions();
            base.AddInteraction(Restock.Singleton);
        }

        public class Restock : Interaction<Sim, RestockItem>
        {
            public class Definition : InteractionDefinition<Sim, RestockItem, Restock>
            {
                public override string GetInteractionName(Sim a, RestockItem target, InteractionObjectPair interaction)
                {
                    return CMStoreSet.LocalizeString("Restock", new object[0] { });
                }

                public override bool Test(Sim a, RestockItem target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    //If restock from inventory, check does the item exist
                    StoreSetBase sBase = RestockItemHelperClass.FindParentShopBase(target);

                    if (sBase == null)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CMStoreSet.LocalizeString("GrayNeedsPedestal", new object[0] { }));
                        return false;
                    }

                    if(sBase.Charred)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CMStoreSet.LocalizeString("GrayCharred", new object[0] { }));
                        return false;
                    }

                    if (RestockItemHelperClass.RestockFromInventory(target, sBase.Info.RestockCraftable))
                    {
                        if (sBase.Info.RegisterId == ObjectGuid.InvalidObjectGuid && !sBase.Info.RestockCraftable)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CMStoreSet.LocalizeString("GrayNeedsRegister", new object[0] { }));
                            return false;
                        }
                        
                        if (sBase.Info.RegisterId != ObjectGuid.InvalidObjectGuid && RestockItemHelperClass.ReturnRestocableObjectFromRegister(target, sBase.Info.RegisterId) == null)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CMStoreSet.LocalizeString("GrayInventoryEmpty", new object[0] { }));
                            return false;
                        }
                    }
                    else
                    {
                        //Should restock buy items be disabled
                        if (target.info.Type == ItemType.Buy && !sBase.Info.RestockBuyMode)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(CMStoreSet.LocalizeString("GrayRestockBuyModeDisabled", new object[0] { }));
                            return false;
                        }

                    }
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new Restock.Definition();

            public override bool Run()
            {               
                if (!this.Actor.RouteToObjectRadialRange(this.Target, UniversityWelcomeKit.kMinRouteDistance, UniversityWelcomeKit.kMaxRouteDistance))
                {
                    return false;
                }

                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("UniversityWelcomeKit", "EnterUniversity", "x");
                base.AnimateSim("LoopTest");
                bool flag = base.DoTimedLoop(2f, ExitReason.Default);
                base.AnimateSim("ExitAptituteTest");
                base.EndCommodityUpdates(flag);
                base.StandardExit();

                //Replace restock item with the restocked item
                StoreSetBase b = RestockItemHelperClass.FindParentShopBase(this.Target);
                GameObject o = RestockItemHelperClass.RecreateSoldObject(this.Target, this.Actor.SimDescription);
                                     
                //If the sim is tending the register, make him go back to tending                
                if (b != null)
                {
                    b.AddInteractionsToChildObjects();
                    List<StoreSetRegister> registers = new List<StoreSetRegister>(Sims3.Gameplay.Queries.GetObjects<StoreSetRegister>(b.LotCurrent));
                    if (registers != null)
                        foreach (var register in registers)
                        {
                            //Push hired clerk to go back to work. 
                            if (CMStoreSet.IsStoreOpen(register) && register.mPreferredClerk == this.Actor.SimDescription.SimDescriptionId)
                            {
                                base.TryPushAsContinuation(StoreSetRegister.PostureIdle.Singleton, register);
                                break;
                            }
                        }
                }

                return true;
            }

        }

    }
}
