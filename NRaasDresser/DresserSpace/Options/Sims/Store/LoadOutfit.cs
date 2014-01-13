using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.Options;
using NRaas.DresserSpace.Helpers;
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

namespace NRaas.DresserSpace.Options.Sims.Store
{
    public class LoadOutfit : OutfitBase, IStoreOption
    {
        public readonly static OutfitCategories[] sCategories = new OutfitCategories[] { OutfitCategories.Everyday, OutfitCategories.Athletic, OutfitCategories.Formalwear, OutfitCategories.Swimwear, OutfitCategories.Sleepwear, OutfitCategories.Career };

        public override string GetTitlePrefix()
        {
            return "LoadOutfit";
        }

        protected override IEnumerable<OutfitCategories> GetCategories(SimDescription sim)
        {
            List<OutfitCategories> results = new List<OutfitCategories>();
            results.Add(OutfitCategories.None);
            return results;
        }

        protected override bool Allow(SimDescription sim, ref CASParts.Key outfitKey, ref bool alternate, ref CASParts.Key displayKey)
        {
            displayKey = new CASParts.Key(sCategories[outfitKey.GetIndex()], 0);

            outfitKey = new CASParts.Key(Dresser.GetStoreOutfitKey(sCategories[outfitKey.GetIndex()], sim.IsUsingMaternityOutfits));
            alternate = true;

            if (CASParts.GetOutfit(sim, outfitKey, true) == null) return false;

            return true;
        }

        protected override int GetOutfitCount(SimDescription sim, OutfitCategories category)
        {
            return sCategories.Length;
        }

        protected override void Perform(SimDescription sim, CASParts.Key outfitKey, CASParts.Key displayKey)
        {
            SimOutfit geneOutfit = CASParts.GetOutfit(sim, CASParts.sPrimary, false);

            SimOutfit source = CASParts.GetOutfit(sim, outfitKey, true);

            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, new CASParts.Key(displayKey.mCategory, sim), geneOutfit))
            {
                new SavedOutfit(source).Apply(builder, false, null, CASParts.GeneticBodyTypes);
            }
        }
    }
}


