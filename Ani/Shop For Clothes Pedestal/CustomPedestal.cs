using Sims3.Gameplay.Objects.Miscellaneous.Shopping;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.Gameplay.WorldBuilderUtil;
using Sims3.SimIFace.CustomContent;
using System.Collections.Generic;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Autonomy;
using System;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay;
using Sims3.Gameplay.Tutorial;
using Sims3.UI;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.EventSystem;
using System.Collections;
using Sims3.Gameplay.Objects.Misc;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Abstracts;
using ani_ClothingPedestal;
using System.Text;
using NRaas;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace;
using Sims3.Gameplay.ObjectComponents;
using Sims3.SimIFace.Enums;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.ThoughtBalloons;

namespace Sims3.Gameplay.Objects.Miscellaneous.Shopping.ani_ClothingPedestal
{
    public class CustomPedestal : ShoppingPedestal, IClothingPedestal, IGameObject, IScriptObject, IScriptLogic, IHasScriptProxy, IObjectUI, IBuildBuyListener, IHasWorldBuilderData, IExportableContent
    {
        public ClothingInfo Info;

        //Interactions
        public class PurchaseCustomOutfit : Interaction<Sim, CustomPedestal>
        {
            public class Definition : InteractionDefinition<Sim, CustomPedestal, CustomPedestal.PurchaseCustomOutfit>
            {
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (a.SimDescription.IsUsingMaternityOutfits)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/Objects/ShelvesStorage/Dresser/ChangeClothes:PregnantTooltip", new object[0]));
                        return false;
                    }
                    if (a.FamilyFunds < target.GetSalePrice(target.Info.Price))
                    {
                        string localizedString = CustomPedestal.LocalizeString(a.IsFemale, "LackTheSimoleans", new object[0]);
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(localizedString);
                        return false;
                    }
                    return target.CustomPurchaseTest(a, isAutonomous, ref greyedOutTooltipCallback);
                }
                public override string GetInteractionName(Sim actor, CustomPedestal target, InteractionObjectPair iop)
                {
                    return CMShopping.LocalizeString("PurchaseOutfitInCas", new object[0]);
                }
            }
            public static InteractionDefinition Singleton = new CustomPedestal.PurchaseCustomOutfit.Definition();
            public bool mTookSemaphore;
            public override void Cleanup()
            {
                if (this.mTookSemaphore)
                {
                    GameStates.ReleaseInteractionStateChangeSemaphore();
                }
                base.Cleanup();
            }
            public override bool Run()
            {
                if (IntroTutorial.IsRunning && !IntroTutorial.AreYouExitingTutorial())
                {
                    return false;
                }
                bool result = true;
                if (!this.Target.RouteToPedestal(this.Actor, true))
                {
                    return false;
                }
                if (Sims3.Gameplay.UI.Responder.Instance.OptionsModel.SaveGameInProgress)
                {
                    return false;
                }
                this.mTookSemaphore = GameStates.WaitForInteractionStateChangeSemaphore(this.Actor, ExitReason.Default);
                if (!this.mTookSemaphore)
                {
                    return false;
                }
                if (!this.Actor.IsSelectable)
                {
                    return false;
                }
                while (Sims3.Gameplay.Gameflow.CurrentGameSpeed == Sims3.SimIFace.Gameflow.GameSpeed.Pause || MoveDialog.InProgress())
                {
                    Simulator.Sleep(0u);
                }

                //Save outfit info
                Dictionary<OutfitCategories, List<SimOutfit>> beforeCas = CMShopping.ReturnClothingInfo(this.Actor);

                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("ShoppingPedestal", "Enter", "x");
                base.SetParameter("x:Age", this.Actor.SimDescription.Age);
                base.AnimateSim("Change Item");
                SimDescription simDescription = null;
                simDescription = this.Actor.SimDescription;
                if (this.Actor.FamilyFunds >= this.Target.GetSalePrice(this.Target.Info.Price))
                {
                    simDescription = this.Actor.SimDescription;
                    if (simDescription == null)
                    {
                        throw new Exception("ChangeOutfit:  sim doesn't have a description!");
                    }
                    CASChangeReporter.Instance.ClearChanges();
                    //GameStates.TransitionToCASDresserMode();
                    //CASLogic singleton = CASLogic.GetSingleton();
                    float agingYearsSinceLastAgeTransition = simDescription.AgingYearsSinceLastAgeTransition;
                    try
                    {
                        //singleton.LoadSim(simDescription, this.Target.DisplayCategory, 0);

                        //Twallan
                        if (GameUtils.IsInstalled(ProductVersion.EP2))
                        {
                            new NRaas.MasterControllerSpace.Sims.Stylist(NRaas.MasterControllerSpace.Sims.CASBase.EditType.Mannequin, Target.DisplayCategory).Perform(new GameHitParameters<SimDescriptionObject>(Sim.ActiveActor, new SimDescriptionObject(this.Actor.SimDescription), GameObjectHit.NoHit));
                        }
                        else
                        {
                            new NRaas.MasterControllerSpace.Sims.Dresser(NRaas.MasterControllerSpace.Sims.CASBase.EditType.Mannequin, Target.DisplayCategory).Perform(new GameHitParameters<SimDescriptionObject>(Sim.ActiveActor, new SimDescriptionObject(this.Actor.SimDescription), GameObjectHit.NoHit));
                        }
                        //End of Twallan

                        while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                        {
                            Simulator.Sleep(0u);
                        }
                        CASChangeReporter.Instance.SendChangedEvents(this.Actor);
                        if (!CASChangeReporter.Instance.CasCancelled && !this.Actor.IsOnHomeLot(this.Target))
                        {
                            //Calculate how many outfits changed
                            int outfits = CMShopping.CalculateChangedOutfits(this.Actor, beforeCas);
                            int price = this.Target.GetSalePrice(this.Target.Info.Price) * outfits;

                            CMShopping.PrintMessage(this.Actor.FullName + "\nOutfits: " + outfits + "\nPrice: " + price);
                            simDescription.Household.ModifyFamilyFunds(-price);

                            if (this.Target.Info.OwnerId != 0uL)
                            {
                                SimDescription owner = SimDescription.Find(this.Target.Info.OwnerId);
                                if (owner != null && owner.Household != null)
                                {
                                    if (owner.Household != simDescription.Household)
                                    {
                                        owner.Household.ModifyFamilyFunds(price);
                                        simDescription.Household.ModifyFamilyFunds(-price);
                                    }
                                }
                            }

                            this.Target.TriggerEvents(this.Actor, price);
                        }
                        CASChangeReporter.Instance.ClearChanges();
                        this.Actor.InteractionQueue.CancelAllInteractionsByType(ClothingPedestal.PurchaseCustomOutfit.Singleton);
                    }
                    finally
                    {
                        simDescription.AgingYearsSinceLastAgeTransition = agingYearsSinceLastAgeTransition;
                    }
                }
                base.AnimateSim("Exit");
                base.EndCommodityUpdates(true);
                base.StandardExit();
                if (this.Actor.CurrentOutfitIndex > simDescription.GetOutfitCount(this.Actor.CurrentOutfitCategory))
                {
                    this.Actor.UpdateOutfitInfo();
                }
                this.Actor.RecreateOccupationOutfits();
                HudModel hudModel = Sims3.UI.Responder.Instance.HudModel as HudModel;
                hudModel.NotifySimChanged(this.Actor.ObjectId);

                //  EventTracker.SendEvent(EventTypeId.kBoughtOutfitFromPedestal, this.Actor, this.Target);
                return result;
            }
        }

        public class PurchasePedestalOutfit : Interaction<Sim, CustomPedestal>
        {
            public class Definition : InteractionDefinition<Sim, CustomPedestal, CustomPedestal.PurchasePedestalOutfit>
            {
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    CustomPedestal objectPedestal = target as CustomPedestal;
                    if (objectPedestal != null)
                    {
                        IGameObject gameObject = target.CurrentObject();
                        if (gameObject != null)
                        {
                            ItemComponent itemComp = gameObject.ItemComp;
                            if (itemComp != null && !itemComp.CanAddToInventory(a.Inventory))
                            {
                                string localizedString = ShoppingPedestal.LocalizeString(a.IsFemale, "CantAddToInventory", new object[0]);
                                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(localizedString);
                                return false;
                            }
                        }
                    }
                    return target.CustomPurchaseTest(a, isAutonomous, ref greyedOutTooltipCallback);
                }
                public override string GetInteractionName(Sim actor, CustomPedestal target, InteractionObjectPair interaction)
                {
                    int objectCostForSim = target.GetSalePrice(target.GetObjectCostForSim(actor));
                    return CMShopping.LocalizeString("PurchasePedestalOutfit", new object[] { target.GetSalePrice(objectCostForSim) });



                }
            }
            public static InteractionDefinition Singleton = new CustomPedestal.PurchasePedestalOutfit.Definition();
            public override ThumbnailKey GetIconKey()
            {
                if (this.Target.CurrentObject() != null && !(this.Target.CurrentObject() is Mannequin))
                {
                    ThumbnailKey thumbnailKey = this.Target.CurrentObject().GetThumbnailKey();
                    thumbnailKey.mSize = ThumbnailSize.Large;
                    return thumbnailKey;
                }
                return base.GetIconKey();
            }
            public override bool Run()
            {
                if (!this.Target.RouteToPedestal(this.Actor, false))
                {
                    return false;
                }
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("ShoppingPedestal", "Enter", "x");
                base.SetParameter("x:Age", this.Actor.SimDescription.Age);
                base.AnimateSim("Purchase");
                bool flag = this.Target.PurchaseCurrentObjectOnPedestal(this.Actor);
                if (flag)
                {
                    VisualEffect visualEffect = VisualEffect.Create("ep11ShopPurchase_main");
                    visualEffect.ParentTo(this.Target, this.Target.ObjectSlot);
                    visualEffect.SubmitOneShotEffect(VisualEffect.TransitionType.SoftTransition);
                }
                base.AnimateSim("Exit");
                base.EndCommodityUpdates(flag);
                base.StandardExit();
                return true;
            }
            public override void Cleanup()
            {
                base.Cleanup();
            }
        }

        public class DisplayOutfit : ImmediateInteraction<Sim, CustomPedestal>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.DisplayOutfit>
            {
                public string MenuText = string.Empty;
                public OutfitCategories mClothingCat = OutfitCategories.Outerwear;
                public Definition()
                {
                }
                public Definition(string text, OutfitCategories cat)
                {
                    this.MenuText = text;
                    this.mClothingCat = cat;
                }
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, CustomPedestal target, List<InteractionObjectPair> results)
                {
                    results.Add(new InteractionObjectPair(new CustomPedestal.DisplayOutfit.Definition(OutfitCategories.All.ToString(), target.DisplayCategory), iop.Target));
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return CMShopping.LocalizeString("SelectOutfit", new object[0]);
                }
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    OutfitPicker.PopulatePieMenuPickerWithOutfits(ref parameters, out listObjs, out headers, out NumSelectableRows);
                }
            }
            public static InteractionDefinition Singleton = new CustomPedestal.DisplayOutfit.Definition();

            public override bool Run()
            {
                CustomPedestal.DisplayOutfit.Definition definition = base.InteractionDefinition as CustomPedestal.DisplayOutfit.Definition;
                this.Target.DisplayCategory = definition.mClothingCat;

                ResourceKey key = ResourceKey.kInvalidResourceKey;
                if (base.SelectedObjects == null || base.SelectedObjects.Count != 1)
                {
                    return false;
                }
                key = (ResourceKey)base.SelectedObjects[0];

                OutfitCategoryMap pedestalOutfits = this.Target.GetPedestalOutfits(this.Target.mDisplayType);
                //ArrayList arrayList = pedestalOutfits[definition.mClothingCat] as ArrayList;
                ArrayList arrayList = new ArrayList();

                arrayList.AddRange(pedestalOutfits[OutfitCategories.Everyday] as ArrayList);
                arrayList.AddRange(pedestalOutfits[OutfitCategories.Formalwear] as ArrayList);
                arrayList.AddRange(pedestalOutfits[OutfitCategories.Outerwear] as ArrayList);
                arrayList.AddRange(pedestalOutfits[OutfitCategories.Sleepwear] as ArrayList);
                arrayList.AddRange(pedestalOutfits[OutfitCategories.Swimwear] as ArrayList);
                arrayList.AddRange(pedestalOutfits[OutfitCategories.Athletic] as ArrayList);
                //
                ulong instanceId = key.InstanceId;
                SimOutfit displayOutfit = null;

                foreach (SimOutfit simOutfit in arrayList)// simDescription.GetOutfits(category))
                {
                    if (instanceId == simOutfit.Key.InstanceId)
                    {
                        displayOutfit = simOutfit;
                        break;
                    }
                }

                //
                if (displayOutfit != null)
                    this.Target.ChangeOutfit(displayOutfit);
                //  this.Target.ChangeOutfit();

                return true;
            }
        }

        public class TestInteraction : ImmediateInteraction<Sim, CustomPedestal>
        {
            private sealed class Definition : ImmediateInteractionDefinition<Sim, IGameObject, CustomPedestal.TestInteraction>
            {
                public override string GetInteractionName(Sim a, IGameObject target, InteractionObjectPair interaction)
                {
                    return "Test Interaction";
                }

                //public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                //{
                //    NumSelectableRows = 1;
                //    Sim actor = parameters.Actor as Sim;
                //    GameObject obj = parameters.Target as GameObject;
                //    List<Sim> sims = new List<Sim>();
                //    foreach (Sim sim2 in obj.LotCurrent.GetSims())
                //    {
                //        if (sim2 != actor)
                //        {
                //            sims.Add(sim2);
                //        }
                //    }
                //    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                //}

                public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                ////Sim selectedSim = base.GetSelectedObject() as Sim;

                ////if (selectedSim != null)
                //{
                //    Dictionary<OutfitCategories, Dictionary<int, List<ResourceKey>>> dict = CMShopping.ReturnClothingInfo(this.Actor);
                //    CMShopping.CalculateChangedOutfits(this.Actor, dict);

                //    //foreach (KeyValuePair<OutfitCategories, List<ResourceKey>> item in dict)
                //    //{

                //    //    foreach (ResourceKey key in item.Value)
                //    //    {
                //    //        CMShopping.PrintMessage(item.Key.ToString() + " " + key);
                //    //    }
                //    //}
                //}

                return true;
            }
        }

        //For client
        public class SuggestOutfit : ImmediateInteraction<Sim, CustomPedestal>
        {
            private sealed class Definition : ImmediateInteractionDefinition<Sim, IGameObject, CustomPedestal.SuggestOutfit>
            {
                public override string GetInteractionName(Sim a, IGameObject target, InteractionObjectPair interaction)
                {
                    return CMShopping.LocalizeString("SuggestOutfit", new object[0]);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    CustomPedestal pedestal = parameters.Target as CustomPedestal;
                    List<Sim> sims = new List<Sim>();

                    if (pedestal != null)
                        foreach (Sim sim in pedestal.LotCurrent.GetSims())
                        {
                            if (sim.SimDescription.Age == pedestal.Info.Age && sim.SimDescription.Gender == pedestal.Info.Gender)
                                if (sim != actor)
                                {
                                    //Can the sim afford it
                                    if (sim.FamilyFunds >= pedestal.GetSalePrice(pedestal.Info.Price))
                                        sims.Add(sim);
                                }
                        }
                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }

                public override bool Test(Sim a, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    bool match = false;
                    CustomPedestal pedestal = target as CustomPedestal;

                    if (pedestal != null)
                    {
                        foreach (Sim sim in target.LotCurrent.GetSims())
                        {
                            if (sim != a && sim.SimDescription.Age == pedestal.Info.Age && sim.SimDescription.Gender == pedestal.Info.Gender)
                            {
                                match = true;
                                break;
                            }
                        }

                        if (!match)
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("No sim on lot match the gender or age of the display outfit.");

                        return match;
                    }
                    return false;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                Sim targetSim = base.GetSelectedObject() as Sim;

                if (targetSim != null)
                {
                    CMShopping.PerformeInteraction(targetSim, this.Target, PurchasePedestalOutfit.Singleton);
                }
                return true;
            }


        }

        public class Chat : Sim.WonderlandBase
        {
            [DoesntRequireTuning]
            protected new class Definition : Sim.WonderlandBase.Definition
            {
                public Definition()
                    : base(CustomPedestal.Chat.kChatSocialActionKey)
                {
                }
            }
            private static string kChatSocialActionKey = "Chat";
            public static readonly InteractionDefinition Singleton = new Sim.Chat.Definition();

            public override bool Run()
            {
                base.Run();
                CMShopping.PrintMessage("Chat");
                return true;
            }
        }

        //Settings       

        //Mannequin
        public class PlanOutfit : ImmediateInteraction<Sim, CustomPedestal>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.PlanOutfit>
            {
                public string mAgeText = string.Empty;
                public string mGenderText = string.Empty;
                public string mClothingCatText = string.Empty;
                public CASAgeGenderFlags mAge = CASAgeGenderFlags.Adult;
                public CASAgeGenderFlags mGender = CASAgeGenderFlags.Female;
                public OutfitCategories mClothingCat = OutfitCategories.Formalwear;
                public Definition()
                {
                }
                public Definition(string ageText, CASAgeGenderFlags age, string genderText, CASAgeGenderFlags gender)
                {
                    this.mAgeText = ageText;
                    this.mGenderText = genderText;
                    this.mGender = gender;
                    this.mAge = age;
                }
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;// target.LotCurrent == a.LotHome || (a.Household != null && a.Household.RealEstateManager != null && a.Household.RealEstateManager.FindProperty(target.LotCurrent) != null);
                }

                public override string[] GetPath(bool isFemale)
                {
                    string s = Localization.LocalizeString(isFemale, "Gameplay/Objects/ShelvesStorage/Dresser/CreateAnOutfit:InteractionName", new object[0]);
                    return new string[]
					{
                        CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                        CMShopping.LocalizeString(CMShopping.MenuMannequin, new object[0])
					};
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return CMShopping.LocalizeString("PlanOutfit", new object[] { 
                       Localization.LocalizeString("Ui/Tooltip/CAS/Basics:" + target.DisplayGender.ToString()),  
                      Localization.LocalizeString("UI/Feedback/CAS:" + target.DisplayAge.ToString())
                    });
                }
            }
            public static InteractionDefinition Singleton = new CustomPedestal.PlanOutfit.Definition();
            public override bool Run()
            {
                try
                {

                    Definition interactionDefinition = InteractionDefinition as Definition;
                    if (Sims3.UI.Responder.Instance.OptionsModel.SaveGameInProgress)
                    {
                        return false;
                    }

                    SimDescription simDesc = new SimDescription();
                    if (simDesc == null)
                    {
                        throw new Exception("ChangeOutfit:  sim doesn't have a description!");
                    }

                    Target.PedestalOutfitsSaveTo(simDesc);

                    CMShopping.PrintMessage(Target.DisplayCategory.ToString());

                    Household.CreateTouristHousehold();
                    Household.TouristHousehold.AddTemporary(simDesc);

                    try
                    {
                        if (GameUtils.IsInstalled(ProductVersion.EP2))
                        {
                            new NRaas.MasterControllerSpace.Sims.Stylist(NRaas.MasterControllerSpace.Sims.CASBase.EditType.Mannequin, Target.DisplayCategory).Perform(new GameHitParameters<SimDescriptionObject>(Sim.ActiveActor, new SimDescriptionObject(simDesc), GameObjectHit.NoHit));
                        }
                        else
                        {
                            new NRaas.MasterControllerSpace.Sims.Dresser(NRaas.MasterControllerSpace.Sims.CASBase.EditType.Mannequin, Target.DisplayCategory).Perform(new GameHitParameters<SimDescriptionObject>(Sim.ActiveActor, new SimDescriptionObject(simDesc), GameObjectHit.NoHit));
                        }

                        while (GameStates.NextInWorldStateId != InWorldState.SubState.LiveMode)
                        {
                            SpeedTrap.Sleep(0x0);
                        }
                        Actor.InteractionQueue.CancelAllInteractionsByType(Singleton);
                    }
                    catch (Exception ex)
                    {
                        CMShopping.PrintMessage("Plan outfit: " + ex.Message);
                    }
                    finally
                    {
                        Household.TouristHousehold.RemoveTemporary(simDesc);
                    }

                    Target.PedestalOutfitsLoadFrom(simDesc, Actor);
                    this.Target.ChangeOutfit();

                    return true;
                }
                catch (ResetException re)
                {
                    CMShopping.PrintMessage(re.Message);
                }
                return false;
            }
        }

        public class SetAge : ImmediateInteraction<Sim, CustomPedestal>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.SetAge>
            {
                public string mAgeText = string.Empty;
                public string mGenderText = string.Empty;
                public string mClothingCatText = string.Empty;
                public CASAgeGenderFlags mAge = CASAgeGenderFlags.Adult;
                public CASAgeGenderFlags mGender = CASAgeGenderFlags.Female;
                public OutfitCategories mClothingCat = OutfitCategories.Formalwear;
                public Definition()
                {
                }
                public Definition(string ageText, CASAgeGenderFlags age)
                {
                    this.mAgeText = ageText;
                    this.mAge = age;
                }
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override void AddInteractions(InteractionObjectPair iop, Sim sim, CustomPedestal target, List<InteractionObjectPair> results)
                {
                    string[] array = new string[]
					{
						"Child", 
						"Teen", 
						"YoungAdult", 
						"Adult", 
						"Elder"
					};
                    CASAgeGenderFlags[] array2 = new CASAgeGenderFlags[]
					{
						CASAgeGenderFlags.Child, 
						CASAgeGenderFlags.Teen, 
						CASAgeGenderFlags.YoungAdult, 
						CASAgeGenderFlags.Adult, 
						CASAgeGenderFlags.Elder
					};
                    for (int i = 0; i < array2.Length; i++)
                    {
                        results.Add(new InteractionObjectPair(new CustomPedestal.SetAge.Definition(Localization.LocalizeString("UI/Feedback/CAS:" + array[i], new object[0]), array2[i]), iop.Target));
                    }
                }
                public override string[] GetPath(bool isFemale)
                {
                    string s = CMShopping.LocalizeString("SetAge", new object[0]);

                    return new string[]
					{
                        CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                        CMShopping.LocalizeString(CMShopping.MenuMannequin, new object[0]),
						s
						
					};
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return this.mAgeText;
                }
            }
            public static InteractionDefinition Singleton = new CustomPedestal.SetAge.Definition();
            public override bool Run()
            {
                bool result = true;

                CustomPedestal.SetAge.Definition definition = base.InteractionDefinition as CustomPedestal.SetAge.Definition;
                if (Sims3.Gameplay.UI.Responder.Instance.OptionsModel.SaveGameInProgress)
                {
                    return false;
                }

                CASAgeGenderFlags oldAge = this.Target.DisplayAge;

                this.Target.DisplayAge = definition.mAge;
                this.Target.Info.Age = definition.mAge;

                if (this.Target.DisplayAge != oldAge)
                {
                    this.Target.mDisplayType = this.Target.DisplayAge | this.Target.DisplayGender;
                    this.Target.ChangeOutfit();
                }
                return result;
            }
        }

        public class SetGender : ImmediateInteraction<Sim, CustomPedestal>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.SetGender>
            {
                public string mAgeText = string.Empty;
                public string mGenderText = string.Empty;
                public string mClothingCatText = string.Empty;
                public CASAgeGenderFlags mAge = CASAgeGenderFlags.Adult;
                public CASAgeGenderFlags mGender = CASAgeGenderFlags.Female;
                public OutfitCategories mClothingCat = OutfitCategories.Formalwear;
                public Definition()
                {
                }
                public Definition(string genderText, CASAgeGenderFlags gender)
                {
                    this.mGenderText = genderText;
                    this.mGender = gender;

                }

                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
                public override void AddInteractions(InteractionObjectPair iop, Sim sim, CustomPedestal target, List<InteractionObjectPair> results)
                {
                    string[] array = new string[]
                    {
                        "Male", 
                        "Female"
                    };
                    CASAgeGenderFlags[] array2 = new CASAgeGenderFlags[]
                    {
                        CASAgeGenderFlags.Male, 
                        CASAgeGenderFlags.Female
                    };
                    for (int i = 0; i < array2.Length; i++)
                    {
                        results.Add(new InteractionObjectPair(new CustomPedestal.SetGender.Definition(Localization.LocalizeString("Ui/Tooltip/CAS/Basics:" + array[i], new object[0]), array2[i]), iop.Target));
                    }
                }
                public override string[] GetPath(bool isFemale)
                {
                    string s = CMShopping.LocalizeString("SetGender", new object[0]);
                    return new string[]
					{
                        CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                        CMShopping.LocalizeString(CMShopping.MenuMannequin, new object[0]),
						s
						
					};
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return this.mGenderText;
                }
            }
            public static InteractionDefinition Singleton = new CustomPedestal.SetGender.Definition();
            public override bool Run()
            {
                bool result = true;

                CustomPedestal.SetGender.Definition definition = base.InteractionDefinition as CustomPedestal.SetGender.Definition;
                if (Sims3.Gameplay.UI.Responder.Instance.OptionsModel.SaveGameInProgress)
                {
                    return false;
                }

                CASAgeGenderFlags oldGender = this.Target.DisplayGender;

                this.Target.DisplayGender = definition.mGender;
                this.Target.Info.Gender = definition.mGender;

                if (this.Target.DisplayGender != oldGender)
                {
                    this.Target.mDisplayType = this.Target.DisplayAge | this.Target.DisplayGender;
                    this.Target.ChangeOutfit();
                }
                return result;
            }
        }

        public class SetOutfitCategory : ImmediateInteraction<Sim, CustomPedestal>
        {
            public class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.SetOutfitCategory>
            {

                public string mClothingCatText = string.Empty;
                public OutfitCategories mClothingCat = OutfitCategories.Swimwear;
                public Definition()
                {
                }
                public Definition(string text, OutfitCategories category)
                {
                    this.mClothingCatText = text;
                    this.mClothingCat = category;

                }

                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim sim, CustomPedestal target, List<InteractionObjectPair> results)
                {

                    foreach (OutfitCategories outfitCategories in Enum.GetValues(typeof(OutfitCategories)))
                    {
                        CASAgeGenderFlags ags = (sim != null) ? sim.SimDescription.AgeGenderSpecies : CASAgeGenderFlags.None;
                        if (target.IsAvailableDisplayCategory(ags, outfitCategories))
                        {
                            results.Add(new InteractionObjectPair(new CustomPedestal.SetOutfitCategory.Definition(outfitCategories.ToString(), outfitCategories), iop.Target));
                        }
                    }
                }

                public override string[] GetPath(bool isFemale)
                {
                    string s = CMShopping.LocalizeString("SetClothingCategory", new object[0]);
                    return new string[]
					{
                        CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                        CMShopping.LocalizeString(CMShopping.MenuMannequin, new object[0]),
						s
						
					};
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return Localization.LocalizeString("Gameplay/Abstracts/ScriptObject/DisplayOutfit:" + this.mClothingCatText, new object[0]);
                }
            }
            public static InteractionDefinition Singleton = new CustomPedestal.SetOutfitCategory.Definition();
            public override bool Run()
            {
                bool result = true;

                CustomPedestal.SetOutfitCategory.Definition definition = base.InteractionDefinition as CustomPedestal.SetOutfitCategory.Definition;
                if (Sims3.Gameplay.UI.Responder.Instance.OptionsModel.SaveGameInProgress)
                {
                    return false;
                }

                this.Target.Info.OutfitCategory = definition.mClothingCat;
                this.Target.DisplayCategory = definition.mClothingCat;

                SimDescription simDesc = new SimDescription();
                if (simDesc == null)
                {
                    throw new Exception("ChangeOutfit:  sim doesn't have a description!");
                }
                Target.PedestalOutfitsSaveTo(simDesc);

                this.Target.CustomRefreshProductIfNecessary(simDesc);

                this.Target.ChangeOutfit();

                return result;
            }
        }

        public class SetPose : ImmediateInteraction<Sim, CustomPedestal>
        {
            private sealed class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.SetPose>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                    CMShopping.LocalizeString(CMShopping.MenuMannequin, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return CMShopping.LocalizeString("SetPose", new object[] { target.Info.SelectedPose.ToString()});
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    headers = new List<ObjectPicker.HeaderInfo>();
                    listObjs = new List<ObjectPicker.TabInfo>();
                    GameObject pedestal = parameters.Target as GameObject;
                    if (pedestal == null)
                    {
                        return;
                    }
                    headers.Add(new ObjectPicker.HeaderInfo(string.Empty, string.Empty, 500));
                    List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();
                    for (int i = 1; i <= CustomPedestal.kNumPoses; i++)
                    {
                        List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();
                        list2.Add(new ObjectPicker.TextColumn(i.ToString()));
                        ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(i, list2);
                        list.Add(item);
                    }
                    ObjectPicker.TabInfo item2 = new ObjectPicker.TabInfo(string.Empty, string.Empty, list);
                    listObjs.Add(item2);
                }

                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                if (base.SelectedObjects != null)
                {
                    int index = (int)base.SelectedObjects[0];

                    if (index > 0)
                    {
                        this.Target.Info.SelectedPose = index;
                        this.Target.SetSpesificPose();
                    }
                }
                return true;
            }
        }

        //Misc
        public sealed class TogglePoseRotation : ImmediateInteraction<Sim, CustomPedestal>
        {
            public sealed class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.TogglePoseRotation>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                    CMShopping.LocalizeString(CMShopping.MenuMisc, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    if (target.Info.PoseRotation)
                        return CMShopping.LocalizeString("DisablePoseRotation", new object[0]);
                    return CMShopping.LocalizeString("EnablePoseRotation", new object[0]);
                }
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new CustomPedestal.TogglePoseRotation.Definition();
            public override bool Run()
            {
                try
                {
                    this.Target.Info.PoseRotation = !this.Target.Info.PoseRotation;
                }
                catch (Exception ex)
                {
                    CMShopping.PrintMessage(ex.Message);
                }

                return true;
            }
        }

        //Not used
        public class ToggleAutonomosy : ImmediateInteraction<Sim, CustomPedestal>
        {
            private sealed class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.ToggleAutonomosy>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                    CMShopping.LocalizeString(CMShopping.MenuMisc, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    //if (target.Info.AllowAutonomous)
                    //    return "Disable Autonous Shopping"; // CMShopping.LocalizeString("ToggleAutonomosy", new object[0]);
                    return "Enable Autonomous Shopping"; // CMShopping.LocalizeString("ToggleAutonomosy", new object[0]);
                }



                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                //this.Target.Info.AllowAutonomous = !this.Target.Info.AllowAutonomous;
                return true;
            }


        }

        public sealed class SetOwner : ImmediateInteraction<Sim, CustomPedestal>
        {
            public sealed class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.SetOwner>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                    CMShopping.LocalizeString(CMShopping.MenuMisc, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return CMShopping.LocalizeString("SetOwner", new object[0] { });
                }
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new CustomPedestal.SetOwner.Definition();
            public override bool Run()
            {
                try
                {
                    SimDescription sd = CMShopping.ReturnSimsInHousehold(this.Actor.SimDescription, true, true);

                    if (sd != null)
                    {
                        base.Target.Info.OwnerId = sd.SimDescriptionId;
                        base.Target.Info.OwnerName = sd.FullName;
                    }
                    else
                    {
                        base.Target.Info.OwnerId = 0uL;
                        base.Target.Info.OwnerName = string.Empty;
                        CMShopping.PrintMessage(CMShopping.LocalizeString("ResetOwner", new object[0] { }));
                    }
                }
                catch (Exception ex)
                {
                    CMShopping.PrintMessage(ex.Message);
                }

                return true;
            }
        }

        public sealed class SetPrice : ImmediateInteraction<Sim, CustomPedestal>
        {
            public sealed class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.SetPrice>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                    CMShopping.LocalizeString(CMShopping.MenuMisc, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return CMShopping.LocalizeString("SetPrice", new object[] { target.Info.Price });
                }
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new CustomPedestal.SetPrice.Definition();
            public override bool Run()
            {
                try
                {
                    int.TryParse(CMShopping.ShowDialogueNumbersOnly("Price for Cloting", "Set the price for clothing", this.Target.Info.Price.ToString()), out this.Target.Info.Price);
                }
                catch (Exception ex)
                {
                    CMShopping.PrintMessage(ex.Message);
                }

                return true;
            }
        }

        //Clothing data
        public class SetName : ImmediateInteraction<Sim, CustomPedestal>
        {
            private sealed class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.SetName>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                    CMShopping.LocalizeString(CMShopping.MenuClothingData, new object[0])
                };
                }

                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return CMShopping.LocalizeString("SetName", new object[0]);
                }



                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                this.Target.Info.Name = CMShopping.ShowDialogue(CMShopping.LocalizeString("SetName", new object[0]), CMShopping.LocalizeString("SetNameDescription", new object[0]), this.Target.Info.Name);
                return true;
            }


        }

        public sealed class ToggleIsVisible : ImmediateInteraction<Sim, CustomPedestal>
        {
            public sealed class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.ToggleIsVisible>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                    CMShopping.LocalizeString(CMShopping.MenuClothingData, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    if (target.Info.IsVisible)
                        return CMShopping.LocalizeString("DisableVisibility", new object[0]);
                    return CMShopping.LocalizeString("EnableVisibility", new object[0]);
                }
                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
            public static readonly InteractionDefinition Singleton = new CustomPedestal.ToggleIsVisible.Definition();
            public override bool Run()
            {
                try
                {
                    this.Target.Info.IsVisible = !this.Target.Info.IsVisible;
                }
                catch (Exception ex)
                {
                    CMShopping.PrintMessage(ex.Message);
                }

                return true;
            }
        }

        public class TransferClothingData : ImmediateInteraction<Sim, CustomPedestal>
        {
            private sealed class Definition : ImmediateInteractionDefinition<Sim, CustomPedestal, CustomPedestal.TransferClothingData>
            {
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]{
                    CMShopping.LocalizeString(CMShopping.MenuSettingsPath, new object[0]),
                    CMShopping.LocalizeString(CMShopping.MenuClothingData, new object[0])
                };
                }
                public override string GetInteractionName(Sim a, CustomPedestal target, InteractionObjectPair interaction)
                {
                    return CMShopping.LocalizeString("TransferClothingDataTo", new object[0]);
                }

                public override bool Test(Sim a, CustomPedestal target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    headers = new List<ObjectPicker.HeaderInfo>();
                    listObjs = new List<ObjectPicker.TabInfo>();
                    List<CustomPedestal> tList = new List<CustomPedestal>(Sims3.Gameplay.Queries.GetObjects<CustomPedestal>());

                    if (tList != null)
                    {
                        GameObject pedestal = parameters.Target as GameObject;
                        if (pedestal == null)
                        {
                            return;
                        }
                        headers.Add(new ObjectPicker.HeaderInfo(string.Empty, string.Empty, 500));
                        List<ObjectPicker.RowInfo> list = new List<ObjectPicker.RowInfo>();

                        foreach (CustomPedestal p in tList)
                        {
                            if (p.Info.IsVisible && p.ObjectId != pedestal.ObjectId)
                            {
                                List<ObjectPicker.ColumnInfo> list2 = new List<ObjectPicker.ColumnInfo>();
                                list2.Add(new ObjectPicker.TextColumn(p.Info.Name));
                                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(p, list2);
                                list.Add(item);
                            }
                        }
                        ObjectPicker.TabInfo item2 = new ObjectPicker.TabInfo(string.Empty, string.Empty, list);
                        listObjs.Add(item2);
                    }
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                if (base.SelectedObjects != null)
                {
                    CustomPedestal pedestal = base.SelectedObjects[0] as CustomPedestal;

                    if (pedestal != null)
                    {
                        SimDescription sd = new SimDescription();
                        this.Target.PedestalOutfitsSaveTo(sd);

                        pedestal.PedestalOutfitsLoadFrom(sd, this.Actor);

                        //Refresh thumnails
                        //OutfitCategoryMap pedestalOutfits = pedestal.GetPedestalOutfits(this.Target.mDisplayType);

                        //foreach (OutfitCategories outfitCategories in pedestalOutfits.Keys)
                        //{
                        //    ArrayList arrayList = pedestalOutfits[outfitCategories] as ArrayList;
                        //    if (arrayList.Count > 1)
                        //    {
                        //        for (int i = 0; i < arrayList.Count; i++)
                        //        {
                        //            SimOutfit outfit = arrayList[i] as SimOutfit;
                        //            if (outfit != null)
                        //            {
                        //                outfit.k
                        //            }
                        //        }
                        //    }
                        //}


                        CMShopping.PrintMessage(CMShopping.LocalizeString("TransferClothingDataResult", new object[]{this.Target.Info.Name, pedestal.Info.Name}));


                    }
                }

                return true;
            }


        }

        public override void OnCreation()
        {
            base.OnCreation();
            Info = new ClothingInfo();
            this.mDisplayType = Info.Age | Info.Gender;
            this.mDisplayCategory = Info.OutfitCategory;
        }

        public override void OnStartup()
        {
            base.OnStartup();
            base.RemoveInteractionByType(Purchase.Singleton);

            //Settings
            base.AddInteraction(CustomPedestal.SetOwner.Singleton);
            base.AddInteraction(CustomPedestal.SetPrice.Singleton);
            base.AddInteraction(CustomPedestal.SetAge.Singleton);
            base.AddInteraction(CustomPedestal.SetGender.Singleton);
            base.AddInteraction(CustomPedestal.SetOutfitCategory.Singleton);
            base.AddInteraction(CustomPedestal.PlanOutfit.Singleton);
            base.AddInteraction(CustomPedestal.SetPose.Singleton);
            base.AddInteraction(CustomPedestal.TogglePoseRotation.Singleton);
            //base.AddInteraction(CustomPedestal.ToggleAutonomosy.Singleton);
            base.AddInteraction(CustomPedestal.TransferClothingData.Singleton);
            base.AddInteraction(CustomPedestal.ToggleIsVisible.Singleton);
            base.AddInteraction(CustomPedestal.SetName.Singleton);

            //Shopping
            base.AddInteraction(CustomPedestal.DisplayOutfit.Singleton);
            base.AddInteraction(CustomPedestal.PurchasePedestalOutfit.Singleton);
            base.AddInteraction(CustomPedestal.PurchaseCustomOutfit.Singleton);
            base.AddInteraction(CustomPedestal.SuggestOutfit.Singleton);
            // base.AddInteraction(CustomPedestal.TestInteraction.Singleton);

            CustomPedestal.ParseMannequinClothingData();

        }
        public override void Dispose()
        {
            this.DestroyMannequin();
        }

        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Localization.LocalizeString("UI/Feedback/CAS:" + this.DisplayAge.ToString(), new object[0]));
            sb.Append("\n");

            sb.Append(Localization.LocalizeString("Ui/Tooltip/CAS/Basics:" + this.DisplayGender.ToString()));
            sb.Append("\n");
            sb.Append(this.DisplayCategory.ToString());
            sb.Append("\n");
            sb.Append("Price: ");
            sb.Append(base.GetSalePrice(Info.Price));
            if (this.Info.OwnerId != 0uL)
            {
                sb.Append("\n");
                sb.Append("Owner: ");
                sb.Append(this.Info.OwnerName);
            }

            return new SimpleTextTooltip(sb.ToString());
        }


        #region Other Stuff

        public void TriggerEvents(Sim sim, int price)
        {
            if (this.mIsSaleActive)
                EventTracker.SendEvent(EventTypeId.kBoughtSomethingOnSale, sim, this);

            EventTracker.SendEvent(EventTypeId.kBoughtOutfitFromPedestal, sim, this);

            int orgPrice = this.mPurchasedPrice;
            this.mPurchasedPrice = price;
            EventTracker.SendEvent(EventTypeId.kBoughtObject, sim, this);
            this.mPurchasedPrice = orgPrice;


        }

        public class OutfitNameVersion
        {
            public string outfitName;
            public ProductVersion DataVersion;
            public OutfitNameVersion(string theoutfitName, ProductVersion pVer)
            {
                this.outfitName = theoutfitName;
                this.DataVersion = pVer;
            }
        }
        public class MannequinRowInfo
        {
            public string mNakedOutfitName;
            public Dictionary<OutfitCategories, int> mOutfitCost = new Dictionary<OutfitCategories, int>();
            public Dictionary<OutfitCategories, List<CustomPedestal.OutfitNameVersion>> sDefaultOutfitsForCategory = new Dictionary<OutfitCategories, List<CustomPedestal.OutfitNameVersion>>();
        }
        public const string kWorldBuilderDataName = "Clothing Pedestal";
        public const uint kHashOutfitCategoryKeyNumber = 1481885306u;
        public new static string sLocalizationKey;
        public OutfitCategories mDisplayCategory;
        public CASAgeGenderFlags mDisplayType;
        public int mDisplayIdx;
        public Dictionary<CASAgeGenderFlags, OutfitCategoryMap> mPedestalOutfits = new Dictionary<CASAgeGenderFlags, OutfitCategoryMap>();
        public ulong mPlannerSimId;
        [Persistable(false)]
        public Mannequin mMannequin;
        [TunableComment("Tuning class for routing."), Tunable]
        public static ShoppingPedestal.PedestalRoutingTuningClass kPedestalRoutingTuningClass;
        [Tunable, TunableComment("Name of the VFX effect to play at specified location.")]
        public static string kObjectSwapEffectName;
        //[Tunable, TunableComment("Description: The cost to purchase a custom outfit from the pedestal.")]
        //public static int kCostToPlanPurchaseOutfit;
        [Tunable, TunableComment("Description: The number of poses that are supported by animation gallery show pedestal clothing poses.")]
        public static int kNumPoses;
        public static Dictionary<CASAgeGenderFlags, CustomPedestal.MannequinRowInfo> sDefaultOutfitInfo;
        public static ulong sUniformComponents;
        public static OutfitCategories[] kOutfitCategories;
        public Mannequin Mannequin
        {
            get
            {
                if (this.mMannequin == null)
                {
                    this.mMannequin = (base.GetContainedObject(this.ObjectSlot) as Mannequin);
                }
                return this.mMannequin;
            }
        }
        public CASAgeGenderFlags DisplayAge
        {
            get
            {
                return this.mDisplayType & CASAgeGenderFlags.AgeMask;
            }
            set
            {
                this.mDisplayType &= ~(CASAgeGenderFlags.Baby | CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Child | CASAgeGenderFlags.Teen | CASAgeGenderFlags.YoungAdult | CASAgeGenderFlags.Adult | CASAgeGenderFlags.Elder);
                this.mDisplayType |= (CASAgeGenderFlags.AgeMask & value);
                this.CreateMannequin();
            }
        }
        public CASAgeGenderFlags DisplayGender
        {
            get
            {
                return this.mDisplayType & CASAgeGenderFlags.GenderMask;
            }
            set
            {
                this.mDisplayType &= ~(CASAgeGenderFlags.Male | CASAgeGenderFlags.Female);
                this.mDisplayType |= (CASAgeGenderFlags.GenderMask & value);
                this.CreateMannequin();
            }
        }
        public OutfitCategories DisplayCategory
        {
            get
            {
                return this.mDisplayCategory;
            }
            set
            {
                if (value != this.mDisplayCategory)
                {
                    this.mDisplayCategory = value;
                    this.mDisplayIdx = 0;
                }
            }
        }
        public override Slot ObjectSlot
        {
            get
            {
                return (Slot)2820733094u;
            }
        }
        public override int ObjectCost
        {
            get
            {
                int fullPrice = 0;
                if (this.Mannequin != null)
                {
                    CustomPedestal.MannequinRowInfo mannequinRowInfo = null;
                    CustomPedestal.sDefaultOutfitInfo.TryGetValue(this.mDisplayType, out mannequinRowInfo);
                    if (mannequinRowInfo != null)
                    {
                        fullPrice = this.Info.Price;
                    }
                }
                return base.GetSalePrice(fullPrice);
            }
        }
        public override string ObjectName
        {
            get
            {
                return CustomPedestal.LocalizeString("DisplayedObject", new object[0]);
            }
        }
        public override ShoppingPedestal.PedestalRoutingTuningClass PedestalRoutingTuning
        {
            get
            {
                return CustomPedestal.kPedestalRoutingTuningClass;
            }
        }
        public override string ObjectSwapVfxName
        {
            get
            {
                return CustomPedestal.kObjectSwapEffectName;
            }
        }
        public new static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString(CustomPedestal.sLocalizationKey + ":" + name, parameters);
        }
        public new static string LocalizeString(bool isFemale, string name, params object[] parameters)
        {
            return Localization.LocalizeString(isFemale, CustomPedestal.sLocalizationKey + ":" + name, parameters);
        }
        static CustomPedestal()
        {
            CustomPedestal.sLocalizationKey = "ani_CustomPedestal:";
            CustomPedestal.kPedestalRoutingTuningClass = new ShoppingPedestal.PedestalRoutingTuningClass();
            CustomPedestal.kObjectSwapEffectName = "ep11ShopClothes_main";
            //CustomPedestal.kCostToPlanPurchaseOutfit = 500;
            CustomPedestal.kNumPoses = 4;
            CustomPedestal.sDefaultOutfitInfo = new Dictionary<CASAgeGenderFlags, CustomPedestal.MannequinRowInfo>();
            CustomPedestal.sUniformComponents = 0uL;
            CustomPedestal.kOutfitCategories = new OutfitCategories[]
			{
				OutfitCategories.Everyday, 
				OutfitCategories.Formalwear, 
				OutfitCategories.Sleepwear, 
				OutfitCategories.Swimwear, 
				OutfitCategories.Athletic, 
				OutfitCategories.Outerwear
			};
            foreach (BodyTypes bodyTypes in Enum.GetValues(typeof(BodyTypes)))
            {
                if (BodyTypeCategoryUtils.IsTypeClothing(bodyTypes) || BodyTypeCategoryUtils.IsTypeAccessory(bodyTypes) || bodyTypes == BodyTypes.Shoes)
                {
                    CustomPedestal.sUniformComponents |= 1uL << (int)bodyTypes;
                }
            }
        }
        public void OnLoadFixup()
        {
            if (this.DisplayCategory == OutfitCategories.Outerwear && !GameUtils.IsInstalled(ProductVersion.EP8))
            {
                this.DisplayCategory = CustomPedestal.kOutfitCategories[RandomUtil.GetInt(CustomPedestal.kOutfitCategories.Length - 2)];
                this.ChangeOutfit();
            }
        }

        public void SetSpesificPose()
        {
            if (this.Mannequin != null)
            {
                string text = "a2o_";
                if ((this.mDisplayType & CASAgeGenderFlags.Child) > CASAgeGenderFlags.None)
                {
                    text = "c2o_";
                }
                string pose = string.Concat(new object[]
				{
					text, 
					"galleryPedestal_clothingPose", 
					this.Info.SelectedPose, 
					"_x"
				});
                this.Mannequin.PlayPose(pose, ProductVersion.EP11, this.mDisplayType);
            }
        }

        public void SetRandomPose()
        {
            if (this.Mannequin != null)
            {
                if (this.Info.PoseRotation)
                {
                    string text = "a2o_";
                    if ((this.mDisplayType & CASAgeGenderFlags.Child) > CASAgeGenderFlags.None)
                    {
                        text = "c2o_";
                    }
                    string pose = string.Concat(new object[]
				{
					text, 
					"galleryPedestal_clothingPose", 
					RandomUtil.GetInt(1, CustomPedestal.kNumPoses), 
					"_x"
				});
                    this.Mannequin.PlayPose(pose, ProductVersion.EP11, this.mDisplayType);
                }
                else
                {
                    SetSpesificPose();
                }
            }

        }

        public void CreateMannequin()
        {
            this.DestroyMannequin();
            ResourceKey baseOutfitKey = this.GetBaseOutfitKey();
            this.mMannequin = Mannequin.CreateMannequin(this, baseOutfitKey);

            if (this.Mannequin != null)
            {
                this.Mannequin.SetHiddenFlags(HiddenFlags.Nothing);
                this.Mannequin.ParentToSlot(this, this.ObjectSlot);
                this.ChangeOutfit();
            }
        }
        public void DestroyMannequin()
        {
            if (this.Mannequin != null)
            {
                this.Mannequin.UnParent();
                this.Mannequin.Destroy();
            }
            IGameObject containedObject = base.GetContainedObject(this.ObjectSlot);
            if (containedObject != null)
            {
                containedObject.UnParent();
            }
            this.mMannequin = null;
        }
        public ResourceKey GetBaseOutfitKey()
        {
            ResourceKey resourceKey = ResourceKey.kInvalidResourceKey;
            CustomPedestal.MannequinRowInfo mannequinRowInfo = null;
            CustomPedestal.sDefaultOutfitInfo.TryGetValue(this.mDisplayType, out mannequinRowInfo);
            if (mannequinRowInfo != null)
            {
                resourceKey = this.GetPedestalOutfitKey(mannequinRowInfo.mNakedOutfitName, ProductVersion.EP11);
                this.ValidatePedestalOutfit(resourceKey, mannequinRowInfo.mNakedOutfitName, true);
            }
            return resourceKey;
        }
        public ResourceKey GetPedestalOutfitKey(string outfitName, ProductVersion version)
        {
            return ResourceKey.CreateOutfitKeyFromProductVersion(outfitName, version);
        }
        public void AddPedestalOutfit(CASAgeGenderFlags ags, OutfitCategories category, string outfitName, ProductVersion version)
        {
            ResourceKey pedestalOutfitKey = this.GetPedestalOutfitKey(outfitName, version);
            if (this.ValidatePedestalOutfit(pedestalOutfitKey, outfitName, false))
            {
                SimOutfit outfit = new SimOutfit(pedestalOutfitKey, true);
                this.AddPedestalOutfit(ags, category, outfit, 0);
            }
        }
        public void AddPedestalOutfit(CASAgeGenderFlags ags, OutfitCategories category, SimOutfit outfit, int index)
        {
            CASAgeGenderFlags key = ags & (CASAgeGenderFlags)4294914303u;
            OutfitCategoryMap outfitCategoryMap;
            this.mPedestalOutfits.TryGetValue(key, out outfitCategoryMap);
            ArrayList arrayList = outfitCategoryMap[category] as ArrayList;
            if (arrayList == null)
            {
                arrayList = new ArrayList();
                outfitCategoryMap[category] = arrayList;
            }
            arrayList.Insert(index, outfit);
        }
        public bool ValidatePedestalOutfit(ResourceKey outfitKey, string outfitName, bool isNakedOutfit)
        {
            return true;
        }
        public OutfitCategoryMap AddPedestalOutfits(CASAgeGenderFlags ags)
        {
            CASAgeGenderFlags cASAgeGenderFlags = ags & (CASAgeGenderFlags)4294914303u;
            OutfitCategoryMap outfitCategoryMap;
            this.mPedestalOutfits.TryGetValue(cASAgeGenderFlags, out outfitCategoryMap);
            if (outfitCategoryMap == null)
            {
                outfitCategoryMap = new OutfitCategoryMap();
                this.mPedestalOutfits.Add(cASAgeGenderFlags, outfitCategoryMap);
            }

            if (outfitCategoryMap.Count == 0)
            {
                OutfitCategories[] array = CustomPedestal.kOutfitCategories;
                for (int i = 0; i < array.Length; i++)
                {
                    OutfitCategories outfitCategories = array[i];
                    CustomPedestal.MannequinRowInfo mannequinRowInfo = null;
                    CustomPedestal.sDefaultOutfitInfo.TryGetValue(cASAgeGenderFlags, out mannequinRowInfo);

                    if (mannequinRowInfo != null)
                    {
                        List<CustomPedestal.OutfitNameVersion> list = null;
                        mannequinRowInfo.sDefaultOutfitsForCategory.TryGetValue(outfitCategories, out list);
                        if (list != null)
                        {
                            foreach (CustomPedestal.OutfitNameVersion current in list)
                            {
                                this.AddPedestalOutfit(cASAgeGenderFlags, outfitCategories, current.outfitName, current.DataVersion);
                            }
                        }
                    }
                }
            }
            return outfitCategoryMap;
        }
        public void RemoveAllPedestalsOutfits()
        {
            Dictionary<CASAgeGenderFlags, OutfitCategoryMap>.KeyCollection keys = this.mPedestalOutfits.Keys;
            foreach (CASAgeGenderFlags current in keys)
            {
                OutfitCategoryMap outfitCategoryMap = this.mPedestalOutfits[current];
                CustomPedestal.RemovePedestalOutfits(outfitCategoryMap);
                outfitCategoryMap.Clear();
            }
            this.mPedestalOutfits.Clear();
        }
        public OutfitCategoryMap GetPedestalOutfits(CASAgeGenderFlags ags)
        {
            CASAgeGenderFlags cASAgeGenderFlags = ags & (CASAgeGenderFlags)4294914303u;
            OutfitCategoryMap outfitCategoryMap;
            this.mPedestalOutfits.TryGetValue(cASAgeGenderFlags, out outfitCategoryMap);
            if (outfitCategoryMap == null)
            {
                outfitCategoryMap = this.AddPedestalOutfits(cASAgeGenderFlags);
            }
            return outfitCategoryMap;
        }
        public SimOutfit GetDisplayOutfit()
        {
            SimOutfit result = null;
            OutfitCategoryMap outfitCategoryMap;
            this.mPedestalOutfits.TryGetValue(this.mDisplayType, out outfitCategoryMap);
            if (outfitCategoryMap != null)
            {
                ArrayList arrayList = outfitCategoryMap[this.mDisplayCategory] as ArrayList;
                if (arrayList != null && this.mDisplayIdx < arrayList.Count)
                {
                    result = (SimOutfit)arrayList[this.mDisplayIdx];
                }
            }
            return result;
        }


        public void PedestalOutfitsSaveTo(SimDescription simDesc)
        {
            //Make the current outfit index 0 outfit
            SimOutfit currentOutfit = this.GetDisplayOutfit();
            SimOutfit zeroIndexOutfit = null;

            simDesc.ClearOutfits(false);

            SimOutfit defaultOutfit = new SimOutfit(this.GetBaseOutfitKey());
            simDesc.Init(defaultOutfit);

            OutfitCategoryMap pedestalOutfits = this.GetPedestalOutfits(this.mDisplayType);

            foreach (OutfitCategories outfitCategories in pedestalOutfits.Keys)
            {
                ArrayList arrayList = pedestalOutfits[outfitCategories] as ArrayList;
                if (arrayList.Count > 1)
                {
                    for (int i = 0; i < arrayList.Count; i++)
                    {
                        if (this.mDisplayCategory == outfitCategories)
                        {
                            if (i == 0)
                            {
                                zeroIndexOutfit = (SimOutfit)arrayList[i];
                                simDesc.AddOutfit(currentOutfit, outfitCategories);
                            }
                            else
                            {
                                if (((SimOutfit)arrayList[i]).Key.InstanceId == currentOutfit.Key.InstanceId && zeroIndexOutfit != null)
                                {
                                    simDesc.AddOutfit(zeroIndexOutfit, outfitCategories);
                                }
                                else if (this.mDisplayCategory == outfitCategories)
                                {
                                    simDesc.AddOutfit((SimOutfit)arrayList[i], outfitCategories);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < arrayList.Count; i++)
                    {
                        simDesc.AddOutfit((SimOutfit)arrayList[i], outfitCategories);
                    }
                }
            }
        }

        public void PedestalOutfitsLoadFrom(SimDescription simDesc, Sim planner)
        {
            this.mPlannerSimId = planner.SimDescription.SimDescriptionId;
            OutfitCategoryMap pedestalOutfits = this.GetPedestalOutfits(this.mDisplayType);
            CustomPedestal.RemovePedestalOutfits(pedestalOutfits);
            OutfitCategories[] array = CustomPedestal.kOutfitCategories;
            for (int i = 0; i < array.Length; i++)
            {
                OutfitCategories category = array[i];
                ArrayList outfits = simDesc.GetOutfits(category);
                for (int j = 0; j < outfits.Count; j++)
                {
                    SimOutfit outfit = outfits[j] as SimOutfit;
                    this.AddPedestalOutfit(simDesc.AgeGenderSpecies, category, outfit, j);
                }
            }
        }
        public void ChangeOutfit()
        {
            if (this.Mannequin != null)
            {
                OutfitCategoryMap pedestalOutfits = this.GetPedestalOutfits(this.mDisplayType);
                ArrayList arrayList = pedestalOutfits[this.mDisplayCategory] as ArrayList;
                int num = (arrayList != null) ? arrayList.Count : 0;
                if (++this.mDisplayIdx >= num)
                {
                    this.mDisplayIdx = 0;
                }
                SimOutfit displayOutfit = this.GetDisplayOutfit();
                if (displayOutfit != null && displayOutfit.IsValid)
                {
                    VisualEffect.FireOneShotEffect(CustomPedestal.kObjectSwapEffectName, this, Slot.FXJoint_0, VisualEffect.TransitionType.SoftTransition);
                    this.Mannequin.ApplyUniform(displayOutfit);
                    this.SetRandomPose();

                }
            }
        }
        public void ChangeOutfit(SimOutfit displayOutfit)
        {
            if (this.Mannequin != null)
            {
                OutfitCategoryMap pedestalOutfits = this.GetPedestalOutfits(this.mDisplayType);
                ArrayList arrayList = pedestalOutfits[this.mDisplayCategory] as ArrayList;

                for (int i = 0; i < arrayList.Count; i++)
                {
                    if (displayOutfit.Key.InstanceId == ((SimOutfit)arrayList[i]).Key.InstanceId)
                    {
                        this.mDisplayIdx = i;
                    }
                }

                if (displayOutfit != null && displayOutfit.IsValid)
                {
                    VisualEffect.FireOneShotEffect(CustomPedestal.kObjectSwapEffectName, this, Slot.FXJoint_0, VisualEffect.TransitionType.SoftTransition);
                    this.Mannequin.ApplyUniform(displayOutfit);
                    this.SetRandomPose();
                }
            }
        }
        public bool IsAvailableDisplayCategory(CASAgeGenderFlags ags, OutfitCategories category)
        {
            bool flag = false;
            if (ags != CASAgeGenderFlags.None)
            {
                CASAgeGenderFlags cASAgeGenderFlags = ags & (CASAgeGenderFlags)4294914303u;
                OutfitCategoryMap pedestalOutfits = this.GetPedestalOutfits(cASAgeGenderFlags);

                if (pedestalOutfits != null)
                {
                    int num = 0;
                    if (this.mDisplayCategory == category && this.mDisplayType == cASAgeGenderFlags)
                    {
                        num = 1;
                    }
                    ArrayList arrayList = pedestalOutfits[category] as ArrayList;

                    flag = (arrayList != null && arrayList.Count > num);
                }

                flag &= (category != OutfitCategories.Outerwear || GameUtils.IsInstalled(ProductVersion.EP8));
            }
            else
            {
                OutfitCategories[] array = CustomPedestal.kOutfitCategories;
                for (int i = 0; i < array.Length; i++)
                {
                    OutfitCategories outfitCategories = array[i];
                    if (outfitCategories == category)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        }
        public ulong GetPlannerSimId()
        {
            return this.mPlannerSimId;
        }

        public override void OnHandToolDeleted()
        {
            this.DestroyMannequin();
            base.OnHandToolDeleted();
        }
        public override void DoReset(GameObject.ResetInformation resetInformation)
        {
            base.DoReset(resetInformation);
            if (!Sims3.SimIFace.Environment.HasEditInGameModeSwitch)
            {
                this.CreateMannequin();
            }
        }
        public override List<InteractionObjectPair> GetProxyInteractions(Sim actor, IShoppingPedestal pedestal)
        {
            CustomPedestal clothingPedestal = pedestal as CustomPedestal;
            List<InteractionObjectPair> list = new List<InteractionObjectPair>();
            if (clothingPedestal != null)
            {
                list.AddRange(base.GetProxyInteractions(actor, clothingPedestal));
                InteractionObjectPair iop = new InteractionObjectPair(CustomPedestal.DisplayOutfit.Singleton, this);
                CustomPedestal.DisplayOutfit.Singleton.AddInteractions(iop, actor, list);
                iop = new InteractionObjectPair(CustomPedestal.PlanOutfit.Singleton, this);
                CustomPedestal.PlanOutfit.Singleton.AddInteractions(iop, actor, list);
                list.Add(new InteractionObjectPair(CustomPedestal.PurchaseCustomOutfit.Singleton, clothingPedestal));
            }
            return list;
        }
        public override int GetObjectCostForSim(Sim sim)
        {
            if (sim.IsOnHomeLot(this))
            {
                return 0;
            }
            return this.ObjectCost;
        }
        public static void ParseMannequinClothingData()
        {
            string[] array = new string[]
			{
				"Everyday", 
				"Formalwear", 
				"Sleepwear", 
				"Swimwear", 
				"Athletic", 
				"Outerwear"
			};
            string[] array2 = new string[]
			{
				"CostEV", 
				"CostFO", 
				"CostSL", 
				"CostSW", 
				"CostAT", 
				"CostOU"
			};
            XmlDbData xmlDbData = XmlDbData.ReadData("Mannequins");
            CustomPedestal.sDefaultOutfitInfo.Clear();
            XmlDbTable xmlDbTable = null;
            xmlDbData.Tables.TryGetValue("General", out xmlDbTable);
            CustomPedestal.MannequinRowInfo mannequinRowInfo = null;
            CASAgeGenderFlags key = CASAgeGenderFlags.None;

            foreach (XmlDbRow current in xmlDbTable.Rows)
            {
                string @string = current.GetString("NakedUniform");
                if (!string.IsNullOrEmpty(@string))
                {
                    if (mannequinRowInfo != null)
                    {
                        CustomPedestal.sDefaultOutfitInfo.Add(key, mannequinRowInfo);
                    }
                    mannequinRowInfo = new CustomPedestal.MannequinRowInfo();
                    string string2 = current.GetString("UniformAgeGender");
                    key = ParserFunctions.ParseAgeGenderFlags(string2);
                    mannequinRowInfo.mNakedOutfitName = @string;
                    int num = 0;
                    string[] array3 = array2;
                    for (int i = 0; i < array3.Length; i++)
                    {
                        string column = array3[i];
                        mannequinRowInfo.mOutfitCost.Add(CustomPedestal.kOutfitCategories[num++], current.GetInt(column));
                    }
                }
                if (mannequinRowInfo != null)
                {
                    int num2 = -1;
                    string[] array4 = array;
                    for (int j = 0; j < array4.Length; j++)
                    {
                        string column2 = array4[j];
                        num2++;
                        List<string> stringList = current.GetStringList(column2, ',');
                        if (stringList.Count > 0 && !string.IsNullOrEmpty(stringList[0]))
                        {
                            CustomPedestal.OutfitNameVersion outfitNameVersion = new CustomPedestal.OutfitNameVersion(stringList[0], ProductVersion.BaseGame);
                            if (stringList.Count > 1 && !string.IsNullOrEmpty(stringList[1]))
                            {
                                ProductVersion productVersion;
                                ParserFunctions.TryParseEnum<ProductVersion>(stringList[1], out productVersion, ProductVersion.BaseGame);
                                if (!GameUtils.IsInstalled(productVersion))
                                {
                                    goto IL_23D;
                                }
                                outfitNameVersion.DataVersion = productVersion;
                            }
                            List<CustomPedestal.OutfitNameVersion> list;
                            mannequinRowInfo.sDefaultOutfitsForCategory.TryGetValue(CustomPedestal.kOutfitCategories[num2], out list);
                            if (list == null)
                            {
                                list = new List<CustomPedestal.OutfitNameVersion>();
                                mannequinRowInfo.sDefaultOutfitsForCategory.Add(CustomPedestal.kOutfitCategories[num2], list);
                            }
                            list.Add(outfitNameVersion);
                        }
                    IL_23D: ;
                    }
                }
            }
            if (mannequinRowInfo != null)
            {
                CustomPedestal.sDefaultOutfitInfo.Add(key, mannequinRowInfo);
            }
        }
        public static void PostLoadFixup()
        {
            CustomPedestal[] objects = Sims3.Gameplay.Queries.GetObjects<CustomPedestal>();
            for (int i = 0; i < objects.Length; i++)
            {
                CustomPedestal clothingPedestal = objects[i];
                clothingPedestal.OnLoadFixup();
            }
        }
        public static void RemovePedestalOutfits(OutfitCategoryMap outfitsMap)
        {
            foreach (OutfitCategories outfitCategories in outfitsMap.Keys)
            {
                ArrayList arrayList = outfitsMap[outfitCategories] as ArrayList;
                if (arrayList != null)
                {
                    for (int i = arrayList.Count - 1; i >= 0; i--)
                    {
                        SimOutfit simOutfit = (SimOutfit)arrayList[i];
                        arrayList.RemoveAt(i);
                        simOutfit.Uncache();
                    }
                }
            }
            outfitsMap.Clear();
        }
        //Vaihdoin
        public void SaveWorldBuilderData()
        {
            WorldBuilderData.Set<CASAgeGenderFlags>("Clothing Pedestal", base.ObjectId, "AgeGender", this.mDisplayType, CASAgeGenderFlags.Adult | CASAgeGenderFlags.Female);
            WorldBuilderData.Set<OutfitCategories>("Clothing Pedestal", base.ObjectId, "ClothingCategory", this.mDisplayCategory, this.DisplayCategory);
        }
        public void LoadWorldBuilderData()
        {
            this.mDisplayType = WorldBuilderData.Get<CASAgeGenderFlags>("Clothing Pedestal", base.ObjectId, "AgeGender", CASAgeGenderFlags.Adult | CASAgeGenderFlags.Female);
            this.mDisplayCategory = WorldBuilderData.Get<OutfitCategories>("Clothing Pedestal", base.ObjectId, "ClothingCategory", this.DisplayCategory);
            if (this.Mannequin == null)
            {
                this.CreateMannequin();
            }
        }
        public void OnEnterBuildBuyMode()
        {
            if (this.Mannequin != null)
            {
                this.Mannequin.SetHiddenFlags((HiddenFlags)4294967295u);
                this.Mannequin.UnParent();
            }
        }
        public void OnExitBuildBuyMode()
        {
            if (this.Mannequin == null)
            {
                this.CreateMannequin();
                return;
            }
            this.Mannequin.SetHiddenFlags(HiddenFlags.Nothing);

            bool slot = this.Mannequin.ParentToSlot(this, (Slot)2820733094u);

            if (!this.Mannequin.RefreshPose())
            {
                this.SetRandomPose();
            }
        }
        public override void ProcessMovingOnPedestal()
        {
            this.OnEnterBuildBuyMode();
        }
        public override bool CustomPurchaseTest(Sim Actor, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            //if (!this.Info.AllowAutonomous && isAutonomous)
            //{
            //    return false;
            //}

            if (Actor.SimDescription.IsUsingMaternityOutfits)
            {
                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/Objects/ShelvesStorage/Dresser/ChangeClothes:PregnantTooltip", new object[0]));
                return false;
            }
            if (Actor.IsRobot || Actor.SimDescription.IsMummy || Actor.SimDescription.IsFrankenstein || Actor.SimDescription.IsImaginaryFriend || Actor.SimDescription.IsUnicorn)
            {
                return false;
            }
            if (Actor.TraitManager != null && Actor.TraitManager.HasElement(TraitNames.RobotHiddenTrait))
            {
                return false;
            }
            if (this.mDisplayType != (Actor.SimDescription.Age | Actor.SimDescription.Gender))
            {
                string localizedString = ClothingPedestal.LocalizeString(Actor.IsFemale, "DisplayDoesntMatch", new object[]
				{
					Actor
				});
                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(localizedString);
                return false;
            }
            if (Actor.BuffManager.HasElement(BuffNames.RobotForm))
            {
                string localizedString2 = ClothingPedestal.LocalizeString(Actor.IsFemale, "NotWhileRobotMode", new object[0]);
                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(localizedString2);
                return false;
            }
            return base.CustomPurchaseTest(Actor, isAutonomous, ref greyedOutTooltipCallback);
        }

        public bool CustomRefreshProductIfNecessary(SimDescription simDesc)
        {
            if (this.Mannequin != null && this.mDisplayType == (simDesc.Age | simDesc.Gender))
            {
                return false;
            }
            this.mDisplayType = (simDesc.Age | simDesc.Gender);
            this.CreateMannequin();
            return true;
        }

        //When browsing, do not change the item
        public override bool RefreshProductIfNecessary(SimDescription simDesc)
        {
            return false;
            //if (this.Mannequin != null && this.mDisplayType == (simDesc.Age | simDesc.Gender))
            //{
            //    return false;
            //}
            //this.mDisplayType = (simDesc.Age | simDesc.Gender);
            //this.CreateMannequin();
            //return true;
        }
        public override bool PurchaseCurrentObjectOnPedestal(Sim Actor)
        {
            SimOutfit displayOutfit = this.GetDisplayOutfit();
            if (displayOutfit == null || !displayOutfit.IsValid)
            {
                return false;
            }
            SimDescription simDescription = Actor.SimDescription;
            SimOutfit outfit = simDescription.GetOutfit(this.mDisplayCategory, 0);
            if (outfit == null || !outfit.IsValid)
            {
                return false;
            }
            SimBuilder simBuilder = new SimBuilder();
            OutfitUtils.SetOutfit(simBuilder, outfit, simDescription);
            OutfitUtils.SetOutfit(simBuilder, displayOutfit, null, CustomPedestal.sUniformComponents);
            SimOutfit outfit2 = new SimOutfit(simBuilder.CacheOutfit("ClothingPedestal.Outfit"));

            simDescription.AddOutfit(outfit2, this.mDisplayCategory, true);
            Actor.SwitchToOutfitWithSpin(this.mDisplayCategory, 0);
            if (Actor.IsSelectable && !Actor.IsOnHomeLot(this))
            {
                Actor.ModifyFunds(-this.ObjectCost);
            }
            if (this.Info.OwnerId != 0uL)
            {
                SimDescription owner = SimDescription.Find(this.Info.OwnerId);
                if (owner != null && owner.Household != null && owner.Household != simDescription.Household)
                    owner.Household.ModifyFamilyFunds(this.ObjectCost);
            }

            this.TriggerEvents(Actor, this.ObjectCost);

            //EventTracker.SendEvent(EventTypeId.kBoughtOutfitFromPedestal, Actor, this);
            return true;
        }
        public override bool ExportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamWriter writer)
        {
            writer.WriteInt32(1481885306u, this.mPedestalOutfits.Keys.Count);
            uint num = 0u;
            foreach (CASAgeGenderFlags current in this.mPedestalOutfits.Keys)
            {
                writer.WriteUint32(num, (uint)current);
                IPropertyStreamWriter writer2 = writer.CreateChild((uint)current);
                this.mPedestalOutfits[current].ExportContent(resKeyTable, objIdTable, writer2);
                writer.CommitChild();
                num += 1u;
            }
            return base.ExportContent(resKeyTable, objIdTable, writer);
        }
        public override bool ImportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            if (this.mPedestalOutfits == null)
            {
                this.mPedestalOutfits = new Dictionary<CASAgeGenderFlags, OutfitCategoryMap>();
            }
            int num = 0;
            reader.ReadInt32(1481885306u, out num, 0);
            uint num2 = 0u;
            while ((ulong)num2 < (ulong)((long)num))
            {
                uint num3 = 0u;
                reader.ReadUint32(num2, out num3, 0u);
                CASAgeGenderFlags cASAgeGenderFlags = (CASAgeGenderFlags)num3;
                if (cASAgeGenderFlags != CASAgeGenderFlags.None)
                {
                    IPropertyStreamReader child = reader.GetChild(num3);
                    if (child != null)
                    {
                        OutfitCategoryMap outfitCategoryMap = new OutfitCategoryMap();
                        outfitCategoryMap.ImportContent(resKeyTable, objIdTable, child);
                        this.mPedestalOutfits.Add(cASAgeGenderFlags, outfitCategoryMap);
                    }
                }
                num2 += 1u;
            }
            this.CreateMannequin();
            return base.ImportContent(resKeyTable, objIdTable, reader);
        }
        #endregion Other Stuff
    }
}


