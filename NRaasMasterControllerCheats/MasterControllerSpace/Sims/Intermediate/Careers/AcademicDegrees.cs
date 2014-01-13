using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
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
    public class AcademicDegrees : SimFromList, ICareerOption
    {
        Dictionary<AcademicDegreeNames, int> mDegrees = new Dictionary<AcademicDegreeNames, int>();

        public override string GetTitlePrefix()
        {
            return "Criteria.AcademicDegree";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CareerManager == null) return false;

            if (me.CareerManager.DegreeManager == null) return false;

            return base.PrivateAllow(me);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            AcademicDegreeManager manager = me.CareerManager.DegreeManager;

            if (!ApplyAll)
            {
                List<Item> choices = new List<Item>();

                foreach (AcademicDegreeStaticData data in AcademicDegreeManager.sDictionary.Values)
                {
                    int value = 0;
                    if (manager != null)
                    {
                        AcademicDegree degree = manager.GetElement(data.AcademicDegreeName);
                        if (degree != null)
                        {
                            value = degree.NumberOfCreditsTowardDegree;
                        }
                    }

                    choices.Add(new Item(data, value));
                }

                CommonSelection<Item>.Results results = new CommonSelection<Item>(Name, choices).SelectMultiple();
                if ((results == null) || (results.Count == 0)) return false;

                foreach (Item item in results)
                {
                    string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me.FullName, item.Value.DegreeName, item.Value.RequiredNumberOfCredit }), item.Count.ToString());
                    if (string.IsNullOrEmpty(text)) continue;

                    int count;
                    if (!int.TryParse(text, out count))
                    {
                        Common.Notify(Common.Localize("Numeric:Error"));
                        continue;
                    }

                    mDegrees[item.Value.AcademicDegreeName] = count;
                }
            }

            foreach (KeyValuePair<AcademicDegreeNames, int> pair in mDegrees)
            {
                AcademicDegree degree = manager.GetElement(pair.Key);
                if (degree == null)
                {
                    if (pair.Value <= 0) continue;

                    manager.AddNewDegree(pair.Key, pair.Value);
                }
                else
                {
                    if (pair.Value > 0)
                    {
                        degree.mEarnedNumberOfCreditsTowardDegree = pair.Value;
                    }
                    else
                    {
                        manager.RemoveElement((ulong)pair.Key);
                    }
                }
            }

            return true;
        }

        public class Item : ValueSettingOption<AcademicDegreeStaticData>
        {
            public Item(AcademicDegreeStaticData value, int count)
                : base(value, value.DegreeName, count)
            { }
        }
    }
}
