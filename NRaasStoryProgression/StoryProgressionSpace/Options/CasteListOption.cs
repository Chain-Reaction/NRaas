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
    public abstract class CasteListOption : GenericOptionBase.ListedOptionItem<CasteOptions, ulong>
    {
        public CasteListOption()
            : base(new List<ulong>(), new List<ulong>())
        { }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override string GetLocalizedValue(CasteOptions value, ref ThumbnailKey icon)
        {
            return value.Name;
        }

        protected override IEnumerable<CasteOptions> GetOptions()
        {
            return StoryProgression.Main.Options.AllCastes;
        }

        public bool Contains(IEnumerable<CasteOptions> options)
        {
            if (options == null) return false;

            foreach (CasteOptions option in options)
            {
                if (ContainsDirect(option.ID)) return true;
            }

            return false;
        }

        protected override ulong ConvertToValue(CasteOptions value, out bool valid)
        {
            if (value == null)
            {
                valid = false;
                return 0;
            }
            else
            {
                valid = true;
                return value.ID;
            }
        }

        protected override ulong ConvertFromString(string value)
        {
            ulong result;
            if (!ulong.TryParse(value, out result)) return 0;
            return result;
        }

        public void Validate(Dictionary<ulong, CasteOptions> castes)
        {
            bool changed = false;

            foreach (ulong caste in new List<ulong>(Value))
            {
                if (!castes.ContainsKey(caste))
                {
                    RemoveDirectValue(caste, false);
                    changed = true;
                }
            }

            foreach (ulong caste in new List<ulong>(SetList))
            {
                if (!castes.ContainsKey(caste))
                {
                    RemoveDirectValue(caste, false);
                    changed = true;
                }
            }

            if (changed)
            {
                Persist();
            }
        }

        public override bool ShouldDisplay()
        {
            if ((!(Manager is CasteOptions)) && (!(Manager is LotOptions)))
            {
                if (!GetValue<AllowCasteOption, bool>()) return false;
            }

            return base.ShouldDisplay();
        }
    }
}

