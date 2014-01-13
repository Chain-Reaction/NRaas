using NRaas.CommonSpace.Selection;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class ResetToDefaultOption : GenericOptionBase.ListedOptionItem<GenericOptionBase.DefaultableOption, GenericOptionBase.DefaultableOption>, IReadSimLevelOption, IReadLotLevelOption, IHouseLevelSimOption, INotPersistableOption, ISimCasteOption
    {
        public ResetToDefaultOption()
            : base(new List<GenericOptionBase.DefaultableOption>(), new List<GenericOptionBase.DefaultableOption>())
        { }

        public override string GetTitlePrefix()
        {
            return "ResetToDefault";
        }

        protected override string ValuePrefix
        {
            get { return null; }
        }

        public override string GetUIValue(bool pure)
        {
            return null;
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override GenericOptionBase.DefaultableOption ConvertFromString(string value)
        {
            return null;
        }

        protected override GenericOptionBase.DefaultableOption ConvertToValue(GenericOptionBase.DefaultableOption value, out bool valid)
        {
            valid = true;
            return value;
        }

        protected override string GetLocalizedValue(GenericOptionBase.DefaultableOption value, ref ThumbnailKey icon)
        {
            return value.Name;
        }

        public override void ApplyOverride(IGenericLevelOption paramOption, OverrideStyle style)
        {
            if ((style & OverrideStyle.MergeSet) == OverrideStyle.MergeSet)
            {
                ResetToDefaultOption option = paramOption as ResetToDefaultOption;

                foreach (GenericOptionBase.DefaultableOption member in option.Value)
                {
                    member.Clear(Manager);
                }
            }
        }

        protected override IEnumerable<GenericOptionBase.DefaultableOption> GetOptions()
        {
            List<GenericOptionBase.DefaultableOption> results = new List<GenericOptionBase.DefaultableOption>();

            foreach (GenericOptionBase.DefaultableOption option in Manager.GetOptions(StoryProgression.Main, null, true))
            {
                if (!option.Resetable) continue;

                results.Add(option);
            }

            return results;
        }

        protected override bool PrivatePerform()
        {
            List<Item> choices = new List<Item>();

            foreach (GenericOptionBase.DefaultableOption choice in GetOptions())
            {
                ThumbnailKey icon = ThumbnailKey.kInvalidThumbnailKey;
                string name = GetLocalizedValue(choice, ref icon);

                choices.Add(new Item(this, choice, name, icon));
            }

            CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, choices).SelectMultiple();

            Value.Clear();

            foreach (Item item in selection)
            {
                Value.Add(item.Value);
            }

            return true;
        }
    }
}

