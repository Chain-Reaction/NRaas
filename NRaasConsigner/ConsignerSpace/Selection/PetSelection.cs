using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.ConsignerSpace.Helpers;
using NRaas.ConsignerSpace.Interactions;
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

namespace NRaas.ConsignerSpace.Selection
{
    public class PetSelection : ProtoSelection<SimDescription>
    {
        public PetSelection(SimDescription sim, string title, int maxPrice, ICollection<SimDescription> list)
            : base(((sim != null) ? sim.FullName : ""), title, list)
        {
            AddColumn(new NameColumn());
            AddColumn(new PriceColumn(maxPrice));
        }

        public class NameColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
        {
            public NameColumn()
                : base("Ui/Caption/ObjectPicker:Sim", "Ui/Tooltip/ObjectPicker:FirstName", 370)
            { }

            public override ObjectPicker.ColumnInfo GetValue(SimDescription item)
            {
                ThumbnailKey thumbnail = ThumbnailKey.kInvalidThumbnailKey;
                if (item.GetOutfit(OutfitCategories.Everyday, 0x0) != null)
                {
                    thumbnail = item.GetThumbnailKey(ThumbnailSize.Large, 0);
                }

                return new ObjectPicker.ThumbAndTextColumn(thumbnail, item.FirstName);
            }
        }

        public class PriceColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
        {
            int mMaxPrice;

            public PriceColumn(int maxPrice)
                : base("NRaas.ConsignerSpace.PetSelection:PriceTitle", "NRaas.ConsignerSpace.PetSelection:PriceTooltip", 20)
            {
                mMaxPrice = maxPrice;
            }

            public override ObjectPicker.ColumnInfo GetValue(SimDescription item)
            {
                int price = PetSale.GetPrice(item);
                if (price > mMaxPrice)
                {
                    price = mMaxPrice;
                }

                return new ObjectPicker.TextColumn(EAText.GetMoneyString(price));
            }
        }
    }
}
