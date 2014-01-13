using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System.Collections.Generic;

namespace NRaas.MasterControllerSpace.Settings.CAS.Blacklist
{
    public class RemoveBlacklistPart : OptionItem, IBlacklistOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveBlacklistPart";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Dictionary<ResourceKey,CASParts.Wrapper> lookup = new Dictionary<ResourceKey,CASParts.Wrapper>();

            foreach(CASParts.Wrapper part in CASParts.GetParts(null))
            {
                if (lookup.ContainsKey(part.mPart.Key)) continue;

                lookup.Add(part.mPart.Key, part);
            }

            List<Item> choices = new List<Item>();
            foreach (ResourceKey key in MasterController.Settings.BlacklistKeys)
            {
                CASParts.Wrapper part;
                if (lookup.TryGetValue(key, out part))
                {
                    choices.Add(new Item(part));
                }
                else
                {
                    choices.Add(new Item(key));
                }
            }

            CommonSelection<Item>.Results results = new CommonSelection<Item>(Name, choices).SelectMultiple();
            if ((results == null) || (results.Count == 0)) return OptionResult.Failure;

            foreach (Item item in results)
            {
                MasterController.Settings.RemoveBlacklistKey(item.Value);
            }

            MasterController.Settings.ApplyBlacklistParts();

            return OptionResult.SuccessClose;
        }

        public class Item : ValueSettingOption<ResourceKey>
        {
            public Item(ResourceKey key)
                : base(key, "0x" + key.InstanceId.ToString("X16"), 0)
            { }
            public Item(CASParts.Wrapper part)
                : base(part.mPart.Key, "0x" + part.mPart.Key.InstanceId.ToString("X16"), 0)
            {
                CASParts.PartPreset preset = new CASParts.PartPreset(part.mPart);
                if (!preset.Valid)
                {
                    uint num2 = CASUtils.PartDataNumPresets(part.mPart.Key);
                    if (num2 > 0)
                    {
                        preset = new CASParts.PartPreset(part.mPart, 0);
                        if (!preset.Valid)
                        {
                            return;
                        }
                    }
                }

                mThumbnail = new ThumbnailKey(preset.mPart.Key, (preset.mPresetId != uint.MaxValue) ? ((int)preset.mPresetId) : -1, (uint)preset.mPart.BodyType, (uint)preset.mPart.AgeGenderSpecies, ThumbnailSize.Large);
            }
        }
    }
}
