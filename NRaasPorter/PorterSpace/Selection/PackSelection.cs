using NRaas.CommonSpace.Dialogs;
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
    public class PackSelection : ProtoSimSelection<SimDescription>
    {
        public PackSelection(ICollection<SimDescription> list)
            : base(Common.Localize("Pack:Title"), null, list, true, true)
        {
            AddColumn(new PackedColumn());
        }

        public class PackedColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
        {
            public PackedColumn()
                : base("NRaas.Porter.Packed:ListTitle", "NRaas.Porter.Packed:ListTooltip", 20)
            { }

            public override ObjectPicker.ColumnInfo GetValue(SimDescription item)
            {
                if (item == null)
                {
                    return new ObjectPicker.TextColumn("");
                }
                else
                {
                    return new ObjectPicker.TextColumn(Porter.GetExportCount(item).ToString());
                }
            }
        }
    }
}
