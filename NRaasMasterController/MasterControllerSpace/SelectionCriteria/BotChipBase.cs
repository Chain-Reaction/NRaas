using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public abstract class BotChipBase<TOption> : SelectionTestableOptionList<TOption, TraitChipName, TraitChipName>, IDoesNotNeedSpeciesFilter
        where TOption : BotChipBase<TOption>.ItemBase, new()
    {
        public abstract class ItemBase : TestableOption<TraitChipName, TraitChipName>
        {
            public ItemBase()
            { }
            public ItemBase(TraitChipName guid, int count)
                : base(guid, "", count)
            {
                SetValue(guid, guid);
            }

            public override void SetValue(TraitChipName value, TraitChipName storeType)
            {
                mValue = value;

                TraitChipStaticData data = TraitChipManager.GetStaticElement(value);

                if (data != null)
                {
                    mName = Localization.LocalizeString(data.mTraitChipNameKey, new object[0]);
                    SetThumbnail(data.GetThumbnailKey(ThumbnailSize.Medium));
                }
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<TraitChipName, TraitChipName> results)
            {
                if (me.TraitManager == null || me.TraitChipManager == null) return false;

                foreach (TraitChip chip in me.TraitChipManager.GetInstalledTraitChips())
                {
                    results[chip.Guid] = chip.Guid;
                }
                return true;
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<TraitChipName, TraitChipName> results)
            {
                if (me.Traits == null) return false;

                foreach (TraitChipName chip in me.Traits)
                {
                    results[chip] = chip;
                }
                return true;
            }

            public string Auxillary
            {
                get
                {                  
                    return null;
                }
            }
        }

        protected override ObjectPickerDialogEx.CommonHeaderInfo<TOption> Auxillary
        {
            get { return new AuxillaryColumn(); }
        }

        public class AuxillaryColumn : ObjectPickerDialogEx.CommonHeaderInfo<TOption>
        {
            public AuxillaryColumn()
                : base("NRaas.MasterController.OptionList:TypeTitle", "NRaas.MasterController.OptionList:TypeTooltip", 40)
            { }

            public override ObjectPicker.ColumnInfo GetValue(TOption item)
            {
                return new ObjectPicker.TextColumn(item.Auxillary);
            }
        }
    }
}