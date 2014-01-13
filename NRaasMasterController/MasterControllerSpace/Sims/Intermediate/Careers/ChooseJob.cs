using NRaas.CommonSpace.Dialogs;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
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
    public class ChooseJob : ChooseCareerOption
    {
        public override string GetTitlePrefix()
        {
            return "ChooseJob";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            return me.TeenOrAbove;
        }

        protected override List<Item> GetOptions(SimDescription me)
        {
            List<Item> results = new List<Item>();

            foreach (Occupation career in CareerManager.sDictionary.Values)
            {
                if (career is School) continue;

                if (career.Guid == OccupationNames.AcademicCareer) continue;

                if (career is Career)
                {
                    GetLocations(career as Career, results);
                }
                else
                {
                    results.Add(new CareerItem(career, null));
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
                : base("NRaas.MasterController.OptionList:CareerTitle", "NRaas.MasterController.OptionList:CareerTooltip", 30)
            { }

            public override ObjectPicker.ColumnInfo GetValue(Item item)
            {
                string result = "";

                CareerItem careerItem = item as CareerItem;
                if (careerItem != null)
                {
                    Career career = careerItem.mCareer as Career;
                    if (career != null)
                    {
                        if (career.IsPartTime)
                        {
                            result += Common.Localize("CareerType:PartTime");
                        }
                        else
                        {
                            result += Common.Localize("CareerType:FullTime");
                        }
                    }
                    else if (careerItem.mCareer is SkillBasedCareer)
                    {
                        result += Common.Localize("CareerType:SelfEmployed");
                    }
                    else if (careerItem.mCareer is ActiveCareer)
                    {
                        result += Common.Localize("CareerType:Active");
                    }
                }

                return new ObjectPicker.TextColumn(result);
            }
        }
    }
}
