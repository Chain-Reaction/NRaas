using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.UI.CAS;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ActorSystems;
using TS3Apartments;
using Sims3.UI;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Core;
using System.Text;
using Sims3.UI.View;
using System;

namespace Sims3.Gameplay.Objects.Miscellaneous.TS3Apartments
{
    //Inside Settings menu
    class StopAdvertisingRoommates : ImmediateInteraction<Sim, Controller>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, Controller, StopAdvertisingRoommates>
        {
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethods.LocalizeString("SettingsMenu", new object[0])
    			};
            }

            public override bool Test(Sim a, Controller target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //This only works in residential lots that have a household
                if (target.LotCurrent.IsResidentialLot && target.LotCurrent.Household != null)
                    return true;
                else
                    return false;
            }
            public override string GetInteractionName(Sim a, Controller target, InteractionObjectPair interaction)
            {
                //TODO: Luo localisaatio
                return CommonMethods.LocalizeString("StopRoommates", new object[0]);
            }
        }
        public static InteractionDefinition Singleton = new StopAdvertisingRoommates.Definition();

        public override bool Run()
        {
            ApartmentController.StopAcceptingRoommates();

            return true;
        }

    }

    class ResetLot : ImmediateInteraction<Sim, Controller>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, Controller, ResetLot>
        {
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethods.LocalizeString("SettingsMenu", new object[0])
    			};
            }

            public override bool Test(Sim a, Controller target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //This only works in residential lots that have a household
                if (target.LotCurrent.IsResidentialLot && target.LotCurrent.Household != null)
                    return true;
                else
                    return false;
            }
            public override string GetInteractionName(Sim a, Controller target, InteractionObjectPair interaction)
            {
                return CommonMethods.LocalizeString("ResetLot", new object[0]);
            }
        }
        public static InteractionDefinition Singleton = new ResetLot.Definition();
        public override bool Run()
        {
            try
            {
                if (CommonMethods.ShowConfirmationDialog(CommonMethods.LocalizeString("ResetLotDescription", new object[0])))
                    ApartmentController.ResetLot(this.Target);
            }
            catch (System.Exception ex)
            {
                CommonMethods.PrintMessage("ResetLot: " + ex.Message);
            }

            return true;
        }

    }


    //Inside Family menu
    class CreateFamily : ImmediateInteraction<Sim, Controller>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, Controller, CreateFamily>
        {
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethods.LocalizeString("FamilyMenu", new object[0])
    			};
            }
            public override bool Test(Sim a, Controller target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //This only works in residential lots that have a household
                if (target.LotCurrent.IsResidentialLot && target.LotCurrent.Household != null)
                    return true;
                else
                    return false;
            }
            public override string GetInteractionName(Sim a, Controller target, InteractionObjectPair interaction)
            {
                //TODO: Luo localisaatio
                return CommonMethods.LocalizeString("CreateFamily", new object[0]);
            }
        }
        public static InteractionDefinition Singleton = new CreateFamily.Definition();

        public override bool Run()
        {
            try
            {
                ApartmentController.CleanupFamily(this.Target);

                string familyName = CommonMethods.ShowDialogue(CommonMethods.LocalizeString("FamilyName", new object[0]), CommonMethods.LocalizeString("EnterFamilyName", new object[0]), string.Empty);

                if (!string.IsNullOrEmpty(familyName))
                {
                    string familyFunds = CommonMethods.ShowDialogue(CommonMethods.LocalizeString("FamilyFunds", new object[0]), CommonMethods.LocalizeString("EnterFamilyFunds", new object[0]), "0");

                    if (!string.IsNullOrEmpty(familyFunds))
                    {

                        string rent = CommonMethods.ShowDialogue(CommonMethods.LocalizeString("Rent", new object[0]), CommonMethods.LocalizeString("EnterRent", new object[0]), "0");

                        if (!string.IsNullOrEmpty(rent))
                        {
                            //Sims and pets
                            List<SimDescription> residents = null;

                            List<PhoneSimPicker.SimPickerInfo> homeles = CommonMethods.ShowHomelesSims(base.Actor, base.Target.LotCurrent, base.Target.Families);

                            List<PhoneSimPicker.SimPickerInfo> list3 = DualPaneSimPicker.Show(homeles, new List<PhoneSimPicker.SimPickerInfo>(), CommonMethods.LocalizeString("Residents", new object[0]), CommonMethods.LocalizeString("AvailableSims", new object[0]));
                            if (list3 != null)
                            {
                                residents = list3.ConvertAll<SimDescription>(new Converter<PhoneSimPicker.SimPickerInfo, SimDescription>(CommonDoor.LockDoor.SimPickerInfoToSimDescription));
                            }

                            //Minor Pets
                            List<MinorPet> minorPetList = new List<MinorPet>();
                            if (CommonMethods.ReturnMinorPets(this.Target).Count > 0)
                            {
                                List<ObjectPicker.RowInfo> homelessPets = CommonMethods.ReturnMinorPetsAsRowInfo(CommonMethods.ReturnHomelessPets(this.Target));
                                List<ObjectPicker.RowInfo> list4 = DualPanelMinorPets.Show(homelessPets, new List<ObjectPicker.RowInfo>(), CommonMethods.LocalizeString("ResidentPets", new object[0]), CommonMethods.LocalizeString("AvailableMinorPets", new object[0]));

                                if (list4 != null)
                                {
                                    foreach (var item in list4)
                                    {
                                        minorPetList.Add(item.Item as MinorPet);
                                    }
                                }
                            }

                            //Create Family
                            if (residents != null)
                            {
                                this.Target.Families.Add(ApartmentController.CreateFamily(familyName, familyFunds, rent, residents, minorPetList, Target.LotCurrent.Household));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonMethods.PrintMessage("Create Family: " + ex.Message);
            }
            return true;
        }

    }

    class EditFamily : ImmediateInteraction<Sim, Controller>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, Controller, EditFamily>
        {
            // Methods 
            // Methods  
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethods.LocalizeString("FamilyMenu", new object[0])
    			};
            }
            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 1;
                headers = new List<ObjectPicker.HeaderInfo>();
                listObjs = new List<ObjectPicker.TabInfo>();
                headers.Add(new ObjectPicker.HeaderInfo("Ui/Caption/ObjectPicker:ObjectName", "Ui/Tooltip/ObjectPicker:Name", 250));
                headers.Add(new ObjectPicker.HeaderInfo("Funds", "Funds"));
                headers.Add(new ObjectPicker.HeaderInfo("IsActive", "IsActive"));

                List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
                Sim actor = parameters.Actor as Sim;
                Controller c = parameters.Target as Controller;

                if (actor != null && c != null)
                {
                    //Clean the families first
                    ApartmentController.CleanupFamily(c);

                    try
                    {
                        //Select one family member for the list
                        foreach (ApartmentFamily family in c.Families)
                        {
                            if (!family.IsActive)
                            {
                                List<ObjectPicker.ColumnInfo> columnInfo = new List<ObjectPicker.ColumnInfo>();

                                columnInfo.Add(new ObjectPicker.TextColumn(family.FamilyName));
                                columnInfo.Add(new ObjectPicker.TextColumn(family.FamilyFunds.ToString()));
                                columnInfo.Add(new ObjectPicker.TextColumn(family.IsActive.ToString()));
                                ObjectPicker.RowInfo info = new ObjectPicker.RowInfo(family, columnInfo);
                                rowInfo.Add(info);
                            }
                        }


                    }
                    catch (System.Exception ex)
                    {
                        CommonMethods.PrintMessage(ex.Message);
                    }

                    listObjs.Add(new ObjectPicker.TabInfo("all", "", rowInfo));
                }

            }


            public override bool Test(Sim a, Controller target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //This only works in residential lots that have a household
                if (target.LotCurrent.IsResidentialLot && target.LotCurrent.Household != null)
                    return true;
                else
                    return false;
            }

            public override string GetInteractionName(Sim a, Controller target, InteractionObjectPair interaction)
            {
                //TODO: Luo localisaatio
                return CommonMethods.LocalizeString("EditFamily", new object[0]);
            }
        }
        public static InteractionDefinition Singleton = new EditFamily.Definition();

        public override bool Run()
        {
            //Active Family
            if (base.SelectedObjects != null && base.SelectedObjects.Count > 0)
            {
                ApartmentFamily selectedFamily = base.SelectedObjects[0] as ApartmentFamily;

                try
                {
                    if (selectedFamily != null)
                    {
                        if (selectedFamily.MinorPets == null)
                            selectedFamily.MinorPets = new List<MinorPet>();

                        selectedFamily.FamilyName = CommonMethods.ShowDialogue(CommonMethods.LocalizeString("FamilName", new object[0]), CommonMethods.LocalizeString("EditFamilyName", new object[0]), selectedFamily.FamilyName);

                        if (!string.IsNullOrEmpty(selectedFamily.FamilyName))
                        {
                            string tmp = CommonMethods.ShowDialogue(CommonMethods.LocalizeString("FamilyFunds", new object[0]), CommonMethods.LocalizeString("EditFamilyFunds", new object[0]), selectedFamily.FamilyFunds.ToString());

                            if (!string.IsNullOrEmpty(tmp))
                            {
                                int.TryParse(tmp, out selectedFamily.FamilyFunds);

                                tmp = CommonMethods.ShowDialogue(CommonMethods.LocalizeString("Rent", new object[0]), CommonMethods.LocalizeString("EditRent", new object[0]), selectedFamily.Rent.ToString());

                                if (!string.IsNullOrEmpty(tmp))
                                {
                                    int.TryParse(tmp, out selectedFamily.Rent);

                                    List<SimDescription> residents = new List<SimDescription>();

                                    List<PhoneSimPicker.SimPickerInfo> homeles = CommonMethods.ShowHomelesSims(base.Actor, base.Target.LotCurrent, base.Target.Families);
                                    List<PhoneSimPicker.SimPickerInfo> currentResidents = CommonMethods.ShowResidentSims(base.Actor, selectedFamily.Residents);

                                    //Combine homeless and current, or current won't show up when removing. 
                                    homeles.AddRange(currentResidents);

                                    List<PhoneSimPicker.SimPickerInfo> list3 = DualPaneSimPicker.Show(homeles, currentResidents, CommonMethods.LocalizeString("Residents", new object[0]), CommonMethods.LocalizeString("AvailableSims", new object[0]));
                                    if (list3 != null)
                                    {
                                        residents = list3.ConvertAll<SimDescription>(new Converter<PhoneSimPicker.SimPickerInfo, SimDescription>(CommonDoor.LockDoor.SimPickerInfoToSimDescription));
                                    }

                                    //residents = CommonMethods.ShowSimSelector(Actor, Target.LotCurrent.Household, "Select Sims for Family: ");
                                    if (residents != null)
                                    {
                                        selectedFamily.Residents = residents;
                                    }

                                    //Minor Pets
                                    List<MinorPet> minorPetList = new List<MinorPet>();
                                    if (CommonMethods.ReturnMinorPets(this.Target).Count > 0)
                                    {
                                        List<ObjectPicker.RowInfo> homelessPets = CommonMethods.ReturnMinorPetsAsRowInfo(CommonMethods.ReturnHomelessPets(this.Target));
                                        List<ObjectPicker.RowInfo> currentPets = CommonMethods.ReturnMinorPetsAsRowInfo(selectedFamily.MinorPets);

                                        //Combine homeless and current, or current won't show up when removing. 
                                        homelessPets.AddRange(currentPets);

                                        List<ObjectPicker.RowInfo> list4 = DualPanelMinorPets.Show(homelessPets, currentPets, CommonMethods.LocalizeString("ResidentPets", new object[0]), CommonMethods.LocalizeString("AvailableMinorPets", new object[0]));

                                        if (list4 != null)
                                        {
                                            foreach (var item in list4)
                                            {
                                                minorPetList.Add(item.Item as MinorPet);
                                            }

                                            selectedFamily.MinorPets = minorPetList;
                                        }
                                    }

                                    base.Target.StartUpOrSwitch();
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    CommonMethods.PrintMessage(ex.Message);
                }



            }

            return true;
        }

    }

    class SetActiveFamily : ImmediateInteraction<Sim, Controller>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, Controller, SetActiveFamily>
        {
            // Methods  
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethods.LocalizeString("FamilyMenu", new object[0])
    			};
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 1;
                headers = new List<ObjectPicker.HeaderInfo>();
                listObjs = new List<ObjectPicker.TabInfo>();
                headers.Add(new ObjectPicker.HeaderInfo("Ui/Caption/ObjectPicker:ObjectName", "Ui/Tooltip/ObjectPicker:Name", 250));
                headers.Add(new ObjectPicker.HeaderInfo(CommonMethods.LocalizeString("FamilyFunds", new object[0]), CommonMethods.LocalizeString("FamilyFnds", new object[0])));
                headers.Add(new ObjectPicker.HeaderInfo(CommonMethods.LocalizeString("IsActive", new object[0]), CommonMethods.LocalizeString("IsActive", new object[0])));

                List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
                Sim actor = parameters.Actor as Sim;
                Controller c = parameters.Target as Controller;

                if (actor != null && c != null)
                {
                    try
                    {
                        //Clean the families first
                        ApartmentController.CleanupFamily(c);

                        //Select one family member for the list
                        foreach (ApartmentFamily family in c.Families)
                        {
                            if (family.Residents != null && family.Residents.Count > 0 && !family.IsActive)
                            {
                                List<ObjectPicker.ColumnInfo> columnInfo = new List<ObjectPicker.ColumnInfo>();

                                columnInfo.Add(new ObjectPicker.TextColumn(family.FamilyName));
                                columnInfo.Add(new ObjectPicker.TextColumn(family.FamilyFunds.ToString()));
                                columnInfo.Add(new ObjectPicker.TextColumn(family.IsActive.ToString()));
                                ObjectPicker.RowInfo info = new ObjectPicker.RowInfo(family, columnInfo);
                                rowInfo.Add(info);
                            }
                        }

                        listObjs.Add(new ObjectPicker.TabInfo("all", "", rowInfo));
                    }
                    catch (System.Exception ex)
                    {
                        CommonMethods.PrintMessage(ex.Message);
                    }
                }

            }


            public override bool Test(Sim a, Controller target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //This only works in residential lots that have a household
                if (target.LotCurrent.IsResidentialLot && target.LotCurrent.Household != null)
                    return true;
                else
                    return false;
            }

            public override string GetInteractionName(Sim a, Controller target, InteractionObjectPair interaction)
            {
                return CommonMethods.LocalizeString("SetActiveFamily", new object[0]);
            }
        }
        public static InteractionDefinition Singleton = new SetActiveFamily.Definition();

        public override bool Run()
        {
            try
            {
                //Active Family
                if (base.SelectedObjects != null && base.SelectedObjects.Count > 0)
                {
                    ApartmentFamily selectedFamily = base.SelectedObjects[0] as ApartmentFamily;


                    if (selectedFamily != null)
                    {
                        ApartmentController.LoadActiveHousehold(selectedFamily, base.Target);
                        base.Target.StartUpOrSwitch();
                    }
                }

            }

            catch (System.Exception ex)
            {
                CommonMethods.PrintMessage("Interaction SetActiveFamily: " + ex.Message);
            }

            return true;
        }

    }

    class ShowActiveFamilyInfo : ImmediateInteraction<Sim, Controller>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, Controller, ShowActiveFamilyInfo>
        {
            // Methods  
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethods.LocalizeString("FamilyMenu", new object[0])
    			};
            }

            public override bool Test(Sim a, Controller target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //This only works in residential lots that have a household
                if (target.LotCurrent.IsResidentialLot && target.LotCurrent.Household != null)
                    return true;
                else
                    return false;
            }
            public override string GetInteractionName(Sim a, Controller target, InteractionObjectPair interaction)
            {
                //TODO: Luo localisaatio
                return CommonMethods.LocalizeString("ShowActiveFamilyInfo", new object[0]);
            }
        }
        public static InteractionDefinition Singleton = new ShowActiveFamilyInfo.Definition();

        public override bool Run()
        {
            ApartmentFamily af = this.Target.Families.Find(delegate(ApartmentFamily f) { return f.IsActive == true; });
            if (af != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(CommonMethods.LocalizeString("FamilyName", new object[0]) + ": " + af.FamilyName);
                sb.Append("\n");
                sb.Append(CommonMethods.LocalizeString("Residents", new object[0]) + ": " + af.Residents.Count);
                sb.Append("\n");
                sb.Append(CommonMethods.LocalizeString("Rent", new object[0]) + ": " + af.Rent + " § ");

                CommonMethods.PrintMessage(sb.ToString());
            }

            return true;
        }

    }

    class DeleteFamily : ImmediateInteraction<Sim, Controller>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, Controller, DeleteFamily>
        {
            // Methods  
            public override string[] GetPath(bool isFemale)
            {
                return new string[]{
				CommonMethods.LocalizeString("FamilyMenu", new object[0])
    			};
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 1;
                headers = new List<ObjectPicker.HeaderInfo>();
                listObjs = new List<ObjectPicker.TabInfo>();
                headers.Add(new ObjectPicker.HeaderInfo("Ui/Caption/ObjectPicker:ObjectName", "Ui/Tooltip/ObjectPicker:Name", 250));

                List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
                Sim actor = parameters.Actor as Sim;
                Controller c = parameters.Target as Controller;

                if (actor != null && c != null)
                {
                    try
                    {
                        //Clean the families first
                        ApartmentController.CleanupFamily(c);

                        //Select one family member for the list
                        foreach (ApartmentFamily family in c.Families)
                        {
                            if (family.Residents != null)
                            {
                                List<ObjectPicker.ColumnInfo> columnInfo = new List<ObjectPicker.ColumnInfo>();

                                columnInfo.Add(new ObjectPicker.TextColumn(family.FamilyName));
                                ObjectPicker.RowInfo info = new ObjectPicker.RowInfo(family, columnInfo);
                                rowInfo.Add(info);
                            }
                        }

                        listObjs.Add(new ObjectPicker.TabInfo("all", "", rowInfo));
                    }
                    catch (System.Exception ex)
                    {
                        CommonMethods.PrintMessage(ex.Message);
                    }
                }
            }


            public override bool Test(Sim a, Controller target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //This only works in residential lots that have a household
                if (target.LotCurrent.IsResidentialLot && target.LotCurrent.Household != null)
                    return true;
                else
                    return false;
            }

            public override string GetInteractionName(Sim a, Controller target, InteractionObjectPair interaction)
            {
                return CommonMethods.LocalizeString("DeleteFamily", new object[0]);
            }
        }
        public static InteractionDefinition Singleton = new DeleteFamily.Definition();

        public override bool Run()
        {
            //Active Family
            if (base.SelectedObjects != null && base.SelectedObjects.Count > 0)
            {
                ApartmentFamily selectedFamily = base.SelectedObjects[0] as ApartmentFamily;
                try
                {

                    if (selectedFamily != null)
                    {
                        ApartmentController.DeleteFamily(base.Target, selectedFamily);
                        base.Target.StartUpOrSwitch();
                    }
                }
                catch (System.Exception ex)
                {
                    CommonMethods.PrintMessage(ex.Message);
                }
            }



            return true;
        }

    }


    enum ItemType { familyName = 0, familyFunds }

    class aniMenuItem
    {
        public ulong familyId;
        public ItemType itemType;

        public string familyName;
        public string familyFunds;

        public aniMenuItem(ulong id, string value, ItemType type)
        {
            familyId = id;
            itemType = type;

            switch (type)
            {
                case ItemType.familyName:
                    familyName = value;
                    break;
                case ItemType.familyFunds:
                    familyFunds = value;
                    break;
                default:
                    break;
            }
        }

    }

    //Just for testing, not visible
    class TestInteraction : ImmediateInteraction<Sim, Controller>
    {
        public class Definition : ImmediateInteractionDefinition<Sim, Controller, TestInteraction>
        {
            public override bool Test(Sim a, Controller target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //This only works in residential lots that have a household
                if (target.LotCurrent.IsResidentialLot && target.LotCurrent.Household != null)
                    return true;
                else
                    return false;
            }
            public override string GetInteractionName(Sim a, Controller target, InteractionObjectPair interaction)
            {
                //TODO: Luo localisaatio
                return "Test Interaction";
            }
        }
        public const string sLocalizationKey = "Gameplay/Objects/Electronics/Phone/ChangeRingtone";
        public static InteractionDefinition Singleton = new TestInteraction.Definition();
        public static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("Gameplay/Objects/Electronics/Phone/ChangeRingtone:" + name, parameters);
        }
        public override bool Run()
        {
            try
            {
                //Minor Pets
                List<ObjectPicker.RowInfo> allPets = CommonMethods.ReturnMinorPetsAsRowInfo(CommonMethods.ReturnMinorPets(this.Target));
                List<ObjectPicker.RowInfo> homelessPets = new List<ObjectPicker.RowInfo>();
                List<ObjectPicker.RowInfo> residentPets = DualPanelMinorPets.Show(allPets, homelessPets, CommonMethods.LocalizeString("Residents", new object[0]), CommonMethods.LocalizeString("AvailableSims", new object[0]));

            }
            catch (System.Exception ex)
            {
                CommonMethods.PrintMessage(ex.Message);
            }

            return true;
        }

    }


}
