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
    public class DopplegangerSelection : ProtoSelection<SimDescription>
    {
        public DopplegangerSelection(SimDescription sim, ICollection<SimDescription> list)
            : base(Common.Localize("Doppleganger:Title"), ((sim != null) ? sim.FullName : ""), list)
        {
            AddColumn(new NameColumn(sim));
        }

        public class NameColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
        {
            SimDescription mSim;

            public NameColumn(SimDescription sim)
                : base("Ui/Caption/ObjectPicker:Sim", "Ui/Tooltip/ObjectPicker:FirstName", 370)
            {
                mSim = sim;
            }

            public override ObjectPicker.ColumnInfo GetValue(SimDescription item)
            {
                ThumbnailKey thumbnail = ThumbnailKey.kInvalidThumbnailKey;
                if (item.GetOutfit(OutfitCategories.Everyday, 0x0) != null)
                {
                    thumbnail = item.GetThumbnailKey(ThumbnailSize.Large, 0);
                }

                string name = item.LastName + ", " + item.FirstName;
                if (mSim == item)
                {
                    name = Common.Localize("Doppleganger:Clone", item.IsFemale, new object[] { name });
                }

                return new ObjectPicker.ThumbAndTextColumn(thumbnail, name);
            }
        }
    }
}
