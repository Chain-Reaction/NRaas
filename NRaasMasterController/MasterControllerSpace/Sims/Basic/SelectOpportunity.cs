using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class SelectOpportunity : SimFromList, IBasicOption
    {
        public class Item : ValueSettingOption<Opportunity>
        {
            public readonly bool mCompleted;

            public Item(Opportunity item, Sim me, string name, List<Opportunity> otherOpps)
                : base(item, name, -1)
            {
                mCompleted = me.OpportunityManager.HasCompletedOpportunity(item.Guid);

                bool appendID = Common.kDebugging;

                if ((!appendID) && (otherOpps != null))
                {
                    foreach (Opportunity other in otherOpps)
                    {
                        if (other == item) continue;

                        if (item.Name == other.Name)
                        {
                            appendID = true;
                            break;
                        }
                    }
                }

                if (appendID)
                {
                    mName += " (" + item.Guid + ")";
                }
            }
        }

        public override string GetTitlePrefix()
        {
            return "Opportunity";
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.OpportunityManager == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            List<Opportunity> allOpportunities = OpportunityEx.GetAllOpportunities(me.CreatedSim, OpportunityCategory.None);
            if (allOpportunities.Count == 0)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Opportunity:None"));
                return false;
            }

            List<Item> allOptions = new List<Item>();
            foreach (Opportunity item in allOpportunities)
            {
                string name = item.Name;
                if (item.IsCareer)
                {
                    name = Common.LocalizeEAString("Ui/Caption/HUD/CareerPanel:Career") + ": " + name;
                }

                allOptions.Add(new Item(item, me.CreatedSim, name, allOpportunities));
            }

            Item selection = new CommonSelection<Item>(Name, allOptions, new CompletedColumn()).SelectSingle();
            if (selection == null) return false;

            return OpportunityEx.Perform(me, selection.Value.Guid);
        }

        public class CompletedColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
        {
            public CompletedColumn()
                : base("NRaas.MasterController.OptionList:TypeTitle", "NRaas.MasterController.OptionList:TypeTooltip", 30)
            { }

            public override ObjectPicker.ColumnInfo GetValue(Item item)
            {
                if (item.mCompleted)
                {
                    return new ObjectPicker.TextColumn(Common.Localize("Boolean:" + item.mCompleted));
                }
                else if (item.Value.RepeatLevel == Repeatability.Always)
                {
                    return new ObjectPicker.TextColumn(Common.Localize("Repeatable:MenuName"));
                }
                else
                {
                    return new ObjectPicker.TextColumn("");
                }
            }
        }
    }
}
