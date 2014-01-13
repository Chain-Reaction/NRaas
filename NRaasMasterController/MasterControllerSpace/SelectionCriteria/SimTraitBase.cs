using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public abstract class SimTraitBase<TOption> : SelectionTestableOptionList<TOption, TraitNames, TraitNames>, IDoesNotNeedSpeciesFilter
        where TOption : SimTraitBase<TOption>.ItemBase, new()
    {
        public abstract class ItemBase : TestableOption<TraitNames, TraitNames>
        {
            public ItemBase()
            { }
            public ItemBase(TraitNames guid, int count)
                : base(guid, "", count)
            {
                SetValue(guid, guid);
            }

            public override void SetValue(TraitNames value, TraitNames storeType)
            {
                mValue = value;

                mName = Traits.ProperName (value, false);

                Trait trait = TraitManager.GetTraitFromDictionary(value);
                if (trait != null)
                {
                    SetThumbnail(trait.ThumbnailIcon, trait.ProductVersion);
                }
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<TraitNames, TraitNames> results)
            {
                if (me.TraitManager == null) return false;

                foreach (Trait trait in me.TraitManager.List)
                {
                    results[trait.Guid] = trait.Guid;
                }
                return true;
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<TraitNames,TraitNames> results)
            {
                if (me.Traits == null) return false;

                foreach (TraitNames trait in me.Traits)
                {
                    results[trait] = trait;
                }
                return true;
            }

            public string Auxillary
            {
                get
                {
                    Trait trait = TraitManager.GetTraitFromDictionary(Value);
                    if (trait == null) return null;

                    if (trait.IsReward)
                    {
                        return Common.Localize("Type:Reward");
                    }
                    else if (trait.IsHidden)
                    {
                        return Common.Localize("Type:Hidden");
                    }

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
