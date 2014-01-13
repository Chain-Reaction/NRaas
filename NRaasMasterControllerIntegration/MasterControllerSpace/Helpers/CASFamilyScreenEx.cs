using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Tutorial;
using Sims3.Metadata;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    public class CASFamilyScreenEx
    {
        [Tunable, TunableComment("Whether to allow romantic relationships between Teen and Young Adults")]
        public static bool kAllowAdultTeen = false;

        private static bool IsValidRelationship(IMiniSimDescription sim1, IMiniSimDescription sim2, CASFamilyScreen.RelationshipType relationship)
        {
            switch (relationship)
            {
                case CASFamilyScreen.RelationshipType.Roommate:
                    return true;

                case CASFamilyScreen.RelationshipType.Spouse:
                    if (!sim1.TeenOrAbove) return false;

                    if (!sim2.TeenOrAbove) return false;

                    if (CASFamilyScreenEx.kAllowAdultTeen) return true;

                    return (sim1.Teen == sim2.Teen);
                case CASFamilyScreen.RelationshipType.Parent:
                    if (sim2.Age > sim1.Age) return false;

                    return sim1.TeenOrAbove;
                case CASFamilyScreen.RelationshipType.Child:
                    if (sim1.Age > sim2.Age) return false;

                    return sim2.TeenOrAbove;
                case CASFamilyScreen.RelationshipType.Sibling:
                    return true;
            }
            return false;
        }

        private static void ShowAddRelationshipDialog(CASFamilyScreen ths, CAFThumb sim1Thumb, CAFThumb sim2Thumb)
        {
            ths.mAddRelationshipSimThumb1.SimDescription = sim1Thumb.SimDescription;
            ths.mAddRelationshipSimThumb2.SimDescription = sim2Thumb.SimDescription;
            CASFamilyScreen.RelationshipType relationship = ths.GetRelationship(sim1Thumb.SimDescription, sim2Thumb.SimDescription);
            ths.mAddRelationshipHousematesButton.Selected = true;

            List<Text> captions = new List<Text>();
            captions.Add(ths.mAddRelationshipOther1Button.GetChildByID(0x1, false) as Text);
            captions.Add(ths.mAddRelationshipOther2Button.GetChildByID(0x1, false) as Text);
            captions.Add(ths.mAddRelationshipOther3Button.GetChildByID(0x1, false) as Text);
            captions.Add(ths.mAddRelationshipOther4Button.GetChildByID(0x1, false) as Text);

            List<Button> buttons = new List<Button>();
            buttons.Add(ths.mAddRelationshipOther1Button);
            buttons.Add(ths.mAddRelationshipOther2Button);
            buttons.Add(ths.mAddRelationshipOther3Button);
            buttons.Add(ths.mAddRelationshipOther4Button);

            foreach (Button button in buttons)
            {
                button.Visible = false;
            }

            int index = 0;

            if (IsValidRelationship(sim1Thumb.SimDescription, sim2Thumb.SimDescription, CASFamilyScreen.RelationshipType.Parent))
            {
                buttons[index].Tag = CASFamilyScreen.RelationshipType.Parent;
                buttons[index].Visible = true;
                captions[index].Caption = Sims3.UI.Responder.Instance.LocalizationModel.LocalizeString(sim1Thumb.SimDescription.IsFemale, "Ui/Caption/CAF/AddRelationship:Parent", new object[0x0]);
                if (relationship == CASFamilyScreen.RelationshipType.Parent)
                {
                    ths.mAddRelationshipHousematesButton.Selected = false;
                    buttons[index].Selected = true;
                }

                index++;
            }
            else if (IsValidRelationship(sim1Thumb.SimDescription, sim2Thumb.SimDescription, CASFamilyScreen.RelationshipType.Child))
            {
                buttons[index].Tag = CASFamilyScreen.RelationshipType.Child;
                buttons[index].Visible = true;
                captions[index].Caption = Sims3.UI.Responder.Instance.LocalizationModel.LocalizeString(sim1Thumb.SimDescription.IsFemale, "Ui/Caption/CAF/AddRelationship:Child", new object[0x0]);
                if (relationship == CASFamilyScreen.RelationshipType.Child)
                {
                    ths.mAddRelationshipHousematesButton.Selected = false;
                    buttons[index].Selected = true;
                }

                index++;
            }

            if (IsValidRelationship(sim1Thumb.SimDescription, sim2Thumb.SimDescription, CASFamilyScreen.RelationshipType.Sibling))
            {
                buttons[index].Tag = CASFamilyScreen.RelationshipType.Sibling;
                buttons[index].Visible = true;
                captions[index].Caption = Sims3.UI.Responder.Instance.LocalizationModel.LocalizeString(sim1Thumb.SimDescription.IsFemale, "Ui/Caption/CAF/AddRelationship:Sibling", new object[0x0]);
                if (relationship == CASFamilyScreen.RelationshipType.Sibling)
                {
                    ths.mAddRelationshipHousematesButton.Selected = false;
                    buttons[index].Selected = true;
                }

                index++;
            }

            if (IsValidRelationship(sim1Thumb.SimDescription, sim2Thumb.SimDescription, CASFamilyScreen.RelationshipType.Spouse))
            {
                buttons[index].Tag = CASFamilyScreen.RelationshipType.Spouse;
                captions[index].Caption = Sims3.UI.Responder.Instance.LocalizationModel.LocalizeString(sim1Thumb.SimDescription.IsFemale, "Ui/Caption/CAF/AddRelationship:Spouse", new object[0x0]);
                buttons[index].Visible = true;
                if (relationship == CASFamilyScreen.RelationshipType.Spouse)
                {
                    ths.mAddRelationshipHousematesButton.Selected = false;
                    buttons[index].Selected = true;
                }

                index++;

                buttons[index].Tag = CASFamilyScreen.RelationshipType.BGFriend;
                buttons[index].Visible = true;
                captions[index].Caption = Sims3.UI.Responder.Instance.LocalizationModel.LocalizeString(sim1Thumb.SimDescription.IsFemale, "Ui/Caption/CAF/AddRelationship:BFriend", new object[0x0]);
                if (relationship == CASFamilyScreen.RelationshipType.BGFriend)
                {
                    ths.mAddRelationshipHousematesButton.Selected = false;
                    buttons[index].Selected = true;
                }

                index++;

                if (index < buttons.Count)
                {
                    buttons[index].Tag = CASFamilyScreen.RelationshipType.Fiancee;
                    buttons[index].Visible = true;
                    captions[index].Caption = Sims3.UI.Responder.Instance.LocalizationModel.LocalizeString(sim1Thumb.SimDescription.IsFemale, "Ui/Caption/CAF/AddRelationship:Fiance", new object[0x0]);
                    if (relationship == CASFamilyScreen.RelationshipType.Fiancee)
                    {
                        ths.mAddRelationshipHousematesButton.Selected = false;
                        buttons[index].Selected = true;
                    }

                    index++;
                }
            }

            ths.mAddRelationshipDialogWin.Visible = true;
            Audio.StartSound("ui_hardwindow_open");
        }

        public static void OnCAFThumbDragDrop(WindowBase sender, UIDragEventArgs eventArgs)
        {
            try
            {
                CASFamilyScreen ths = CASFamilyScreen.gSingleton;

                CAFThumb data = eventArgs.Data as CAFThumb;
                if ((data != null) && eventArgs.Result)
                {
                    CAFThumb thumb2 = sender as CAFThumb;
                    if (data != thumb2)
                    {
                        ShowAddRelationshipDialog(ths, data, thumb2);
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnCAFThumbDragDrop", exception);
            }
        }
    }
}
