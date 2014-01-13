using NRaas.CommonSpace.Booters;
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
    public class TotalBlacklist : OptionItem, IBlacklistOption
    {
        public override string GetTitlePrefix()
        {
            return "TotalBlacklist";
        }

        public override string DisplayValue
        {
            get
            {
                return EAText.GetNumberString(InvalidPartBooter.InvalidPartsCount);
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Common.kDebugging) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Dictionary<ResourceKey, CASParts.Wrapper> lookup = new Dictionary<ResourceKey, CASParts.Wrapper>();

            foreach (CASParts.Wrapper part in CASParts.GetParts(null))
            {
                if (lookup.ContainsKey(part.mPart.Key)) continue;

                lookup.Add(part.mPart.Key, part);
            }

            List<RemoveBlacklistPart.Item> choices = new List<RemoveBlacklistPart.Item>();
            foreach (ResourceKey key in InvalidPartBooter.InvalidPartKeys)
            {
                CASParts.Wrapper part;
                if (lookup.TryGetValue(key, out part))
                {
                    choices.Add(new RemoveBlacklistPart.Item(part));
                }
                else
                {
                    choices.Add(new RemoveBlacklistPart.Item(key));
                }
            }

            new CommonSelection<RemoveBlacklistPart.Item>(Name, choices).SelectMultiple();

            return OptionResult.Failure;
        }
    }
}
