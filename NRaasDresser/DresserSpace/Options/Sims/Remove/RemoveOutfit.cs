using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.DresserSpace.Options.Sims.Remove
{
    public class RemoveOutfit : OutfitBase, IRemoveOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveOutfit";
        }

        protected override IEnumerable<OutfitCategories> GetCategories(SimDescription sim)
        {
            List<OutfitCategories> results = new List<OutfitCategories>();

            foreach (OutfitCategories category in new OutfitCategories[] { OutfitCategories.Everyday, OutfitCategories.Formalwear, OutfitCategories.Sleepwear, OutfitCategories.Swimwear, OutfitCategories.Athletic, OutfitCategories.MartialArts, OutfitCategories.Naked, OutfitCategories.Singed, OutfitCategories.Career, OutfitCategories.Outerwear })
            {
                int count = sim.GetOutfitCount(category);

                switch (category)
                {
                    case OutfitCategories.Naked:
                    case OutfitCategories.Singed:
                        break;
                    default:
                        if (count <= 1) continue;
                        break;
                }

                results.Add(category);
            }

            return results;
        }

        protected override bool Allow(SimDescription sim, ref CASParts.Key outfitKey, ref bool alternate, ref CASParts.Key displayKey)
        {
            CASParts.Key currentKey = new CASParts.Key(sim.CreatedSim);
            if (outfitKey == currentKey) return false;

            return true;
        }

        protected override void Perform(SimDescription sim, CASParts.Key outfitKey, CASParts.Key displayKey)
        {
            CASParts.Key currentKey = new CASParts.Key(sim.CreatedSim);
            if (outfitKey == currentKey) return;

            switch (outfitKey.mCategory)
            {
                case OutfitCategories.Everyday:
                case OutfitCategories.Athletic:
                case OutfitCategories.Formalwear:
                case OutfitCategories.Swimwear:
                case OutfitCategories.Sleepwear:
                case OutfitCategories.Career:
                case OutfitCategories.Outerwear:
                    // Don't allow the user to remove the last of a category
                    if (sim.GetOutfitCount(outfitKey.mCategory) == 1) return;
                    break;
            }

            CASParts.RemoveOutfit(sim, outfitKey, false);
        }
    }
}


