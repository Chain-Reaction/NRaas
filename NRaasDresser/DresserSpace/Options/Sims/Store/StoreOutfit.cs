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
    public class StoreOutfit : OutfitBase, IStoreOption
    {
        public override string GetTitlePrefix()
        {
            return "StoreOutfit";
        }

        protected override IEnumerable<OutfitCategories> GetCategories(SimDescription sim)
        {
            return new List<OutfitCategories>(LoadOutfit.sCategories);
        }

        protected override bool Allow(SimDescription sim, ref CASParts.Key outfitKey, ref bool alternate, ref CASParts.Key displayKey)
        {
            return true;
        }

        protected override void Perform(SimDescription sim, CASParts.Key outfitKey, CASParts.Key displayKey)
        {
            SimOutfit source = CASParts.GetOutfit(sim, outfitKey, false);

            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, new CASParts.Key(Dresser.GetStoreOutfitKey(outfitKey.mCategory, sim.IsUsingMaternityOutfits)), source, true))
            { }
        }
    }
}


