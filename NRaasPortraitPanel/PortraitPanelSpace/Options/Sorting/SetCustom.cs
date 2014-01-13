using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Dialogs;
using NRaas.PortraitPanelSpace.Dialogs;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.PortraitPanelSpace.Options.Sorting
{
    public class SetCustom : OperationSettingOption<Sim>, ISortingOption
    {
        public override string GetTitlePrefix()
        {
            return "SetCustom";
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            if (PortraitPanel.Settings.mSimsV2.Count == 0) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            foreach (SimDescription sim in new SimSelection(Name, PortraitPanel.Settings.SelectedSims, new SortColumn()).SelectMultiple())
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", sim.IsFemale, new object[] { sim }), PortraitPanel.Settings.GetSimSort(sim).ToString());
                if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                int value;
                if (!int.TryParse(text, out value))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    continue;
                }

                PortraitPanel.Settings.SetSimSort(sim, value);
            }

            PortraitPanel.Settings.AddSelectedSimsFilter();

            SkewerEx.Instance.PopulateSkewers();

            return OptionResult.SuccessRetain;
        }

        public class SortColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
        {
            public SortColumn()
                : base("NRaas.PortraitPanel.OptionList:ValueTitle", "NRaas.PortraitPanel.OptionList:ValueTooltip", 20)
            { }

            public override ObjectPicker.ColumnInfo GetValue(SimDescription item)
            {
                return new ObjectPicker.TextColumn(EAText.GetNumberString(PortraitPanel.Settings.GetSimSort(item)));
            }
        }
    }
}
