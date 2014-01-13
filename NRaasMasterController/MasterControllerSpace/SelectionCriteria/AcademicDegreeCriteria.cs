using NRaas.CommonSpace.Dialogs;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System.Collections.Generic;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class AcademicDegreeCriteria : SelectionTestableOptionList<AcademicDegreeCriteria.Item, AcademicDegreeCriteria.Values, AcademicDegreeCriteria.Values>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.AcademicDegree";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (me.CareerManager == null) return false;

            if (me.CareerManager.DegreeManager == null) return false;

            return base.Allow(me, actor);
        }

        public struct Values
        {
            public readonly AcademicDegreeNames mDegree;

            public readonly bool mCompleted;

            public Values(AcademicDegreeNames degree, bool completed)
            {
                mDegree = degree;
                mCompleted = completed;
            }
        }

        public class Item : TestableOption<Values, Values>
        {
            public Item()
            { }
            public Item(Values value, int count)
            {
                SetValue(value, value);

                mCount = count;
            }

            public override void SetValue(Values value, Values storeType)
            {
                mValue = value;

                AcademicDegreeStaticData degree;
                if (AcademicDegreeManager.sDictionary.TryGetValue((ulong)value.mDegree, out degree))
                {
                    mName = degree.DegreeName;
                }
                else
                {
                    mName = value.ToString();
                }
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<Values, Values> results)
            {
                if (me.CareerManager == null) return false;

                AcademicDegreeManager manager = me.CareerManager.DegreeManager;
                if (manager == null) return false;

                foreach (AcademicDegree degree in manager.List)
                {
                    Values value = new Values(degree.AcademicDegreeName, degree.IsDegreeCompleted);

                    results[value] = value;
                }

                return true;
            }
        }

        protected override ObjectPickerDialogEx.CommonHeaderInfo<Item> Auxillary
        {
            get { return new AuxillaryColumn(); }
        }

        public class AuxillaryColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
        {
            public AuxillaryColumn()
                : base("NRaas.MasterController.OptionList:CompletedTitle", "NRaas.MasterController.OptionList:CompletedTooltip", 30)
            { }

            public override ObjectPicker.ColumnInfo GetValue(Item item)
            {
                return new ObjectPicker.TextColumn(Common.Localize("Boolean:" + item.Value.mCompleted));
            }
        }
    }
}
