using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.PorterSpace.Selection
{
    public class HouseholdSelection : CommonSelection<HouseholdItem>
    {
        protected HouseholdSelection(string name, List<HouseholdItem> houses)
            : base(name, houses, new HouseholdPackedColumn())
        {
            NameColumn nameColumn = GetColumn<NameColumn>();
            if (nameColumn != null)
            {
                ReplaceColumn(nameColumn, new HouseholdNameColumn(nameColumn));
            }

            CountColumn countColumn = GetColumn<CountColumn>();
            if (countColumn != null)
            {
                countColumn.Width = 20;
            }
        }

        public static List<Household> Perform(string name, List<HouseholdItem> allHouses)
        {
            CommonSelection<HouseholdItem>.Results choices = new HouseholdSelection(name, allHouses).SelectMultiple();
            if (choices.Count == 0) return null;

            List<Household> houses = new List<Household>();
            foreach (HouseholdItem house in choices)
            {
                houses.Add(house.Value);
            }

            return houses;
        }

        public class HouseholdNameColumn : NameColumn
        {
            public HouseholdNameColumn(NameColumn column)
                : base(column.mUseThumbnail, column.Width, "NRaas.Porter.Household:ListTitle", "NRaas.Porter.Household:ListTooltip")
            { }
        }

        public class HouseholdPackedColumn : ObjectPickerDialogEx.CommonHeaderInfo<HouseholdItem>
        {
            public HouseholdPackedColumn()
                : base("NRaas.Porter.Packed:ListTitle", "NRaas.Porter.Packed:ListTooltip", 40)
            { }

            public override ObjectPicker.ColumnInfo GetValue(HouseholdItem item)
            {
                Household house = item.Value;

                int total = 0;
                if (house.IsServiceNpcHousehold)
                {
                    foreach (SimDescription sim in SimDescription.GetHomelessSimDescriptionsFromUrnstones())
                    {
                        if (Porter.GetExportCount(sim) > 0)
                        {
                            total++;
                        }
                    }
                }
                else
                {
                    foreach (SimDescription sim in house.AllSimDescriptions)
                    {
                        if (Porter.GetExportCount(sim) > 0)
                        {
                            total++;
                        }
                    }
                }

                if (total == Households.NumSims(house))
                {
                    return new ObjectPicker.TextColumn(Common.Localize("Packed:Yes"));
                }
                else if (total > 0)
                {
                    return new ObjectPicker.TextColumn(Common.Localize("Packed:Partial"));
                }
                else
                {
                    return new ObjectPicker.TextColumn(Common.Localize("Packed:No"));
                }
            }
        }
    }
}
