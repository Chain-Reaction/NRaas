using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Dialogs;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class ChooseSchool : ChooseCareerOption
    {
        public override string GetTitlePrefix()
        {
            return "ChooseSchool";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return ((me.Child) || (me.Teen));
        }

        protected override List<Item> GetOptions(SimDescription me)
        {
            List<Item> results = new List<Item>();

            foreach (Occupation career in CareerManager.sDictionary.Values)
            {
                if (career is SchoolElementary)
                {
                    if (!me.Child) continue;
                }
                else if (career is SchoolHigh)
                {
                    if (!me.Teen) continue;
                }
                else if (career is School)
                {
                    if ((!me.Child) && (!me.Teen)) continue;
                }
                else
                {
                    continue;
                }

                GetLocations(career as Career, results);
            }

            if (GameUtils.IsInstalled(ProductVersion.EP4))
            {
                foreach (BoardingSchool.BoardingSchoolData data in BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList.Values)
                {
                    results.Add(new BoardingItem(data));
                }
            }

            return results;
        }


        protected override ObjectPickerDialogEx.CommonHeaderInfo<ChooseCareerOption.Item> Auxillary
        {
            get { return new AuxillaryColumn(); }
        }

        public class AuxillaryColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
        {
            public AuxillaryColumn()
                : base("NRaas.MasterController.OptionList:CareerTitle", "NRaas.MasterController.OptionList:SchoolTooltip", 25)
            { }

            public override ObjectPicker.ColumnInfo GetValue(Item item)
            {
                string result = "";

                CareerItem careerItem = item as CareerItem;
                if (careerItem != null)
                {
                    switch(careerItem.mCareer.Guid)
                    {
                        case OccupationNames.SchoolElementary:
                        case OccupationNames.SchoolHigh:
                            result += Common.Localize("CareerType:Standard");
                            break;
                        default:
                            result += Common.Localize("CareerType:Custom");
                            break;
                    }
                }
                else
                {
                    BoardingItem boarding = item as BoardingItem;
                    if (boarding != null)
                    {
                        result += Common.Localize("CareerType:BoardingSchool");
                    }
                }

                return new ObjectPicker.TextColumn(result);
            }
        }

        public class BoardingItem : ChooseCareerOption.Item
        {
            public readonly BoardingSchool.BoardingSchoolData mSchool;

            public BoardingItem(BoardingSchool.BoardingSchoolData school)
            {
                mSchool = school;
            }

            public override string GetTitlePrefix()
            {
                return null;
            }

            public override string Name
            {
                get
                {
                    return Common.LocalizeEAString(mSchool.SchoolNameKey);
                }
            }

            protected override bool Allow(SimDescription me, IMiniSimDescription actor)
            {
                if ((!me.Child) && (!me.Teen)) return false;

                if (me.BoardingSchool == null) return false;

                return (me.BoardingSchool.CurrentSchoolType == mSchool.SchoolType);
            }

            public override bool Perform(SimDescription me, bool applyAll)
            {
                SimDescription head = SimTypes.HeadOfFamily(me.Household);
                if (head == null) return false;

                BoardingSchool.SchedulePickUp(head.CreatedSim, me.CreatedSim, mSchool.SchoolType);
                return true;
            }
        }
    }
}
