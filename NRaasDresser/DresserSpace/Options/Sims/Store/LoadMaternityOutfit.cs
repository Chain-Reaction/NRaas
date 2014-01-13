using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.DresserSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options.Sims.Store
{
    public class LoadMaternityOutfit : OperationSettingOption<Sim>, IStoreOption
    {
        public override string GetTitlePrefix()
        {
            return "LoadMaternityOutfit";
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!parameters.mTarget.SimDescription.IsUsingMaternityOutfits) return false;

            bool found = false;
            foreach (OutfitCategories category in LoadOutfit.sCategories)
            {
                if (CASParts.GetOutfit(parameters.mTarget.SimDescription, new CASParts.Key(Dresser.GetStoreOutfitKey(category, parameters.mTarget.SimDescription.IsUsingMaternityOutfits)), true) != null)
                {
                    found = true;
                    break;
                }
            }

            if (!found) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", parameters.mTarget.IsFemale, new object[] { parameters.mTarget })))
            {
                return OptionResult.Failure;
            }

            SimDescription sim = parameters.mTarget.SimDescription;

            SimOutfit geneOutfit = CASParts.GetOutfit(sim, CASParts.sPrimary, false);

            foreach (OutfitCategories category in LoadOutfit.sCategories)
            {
                CASParts.Key outfitKey = new CASParts.Key(Dresser.GetStoreOutfitKey(category, sim.IsUsingMaternityOutfits));

                SimOutfit source = CASParts.GetOutfit(sim, outfitKey, true);
                if (source == null) continue;

                int index = 0;
                if (category == OutfitCategories.Career)
                {
                    index = 1;
                }

                using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, new CASParts.Key(category, index), geneOutfit))
                {
                    new SavedOutfit(source).Apply(builder, false, null, CASParts.GeneticBodyTypes);
                }

                if (category == OutfitCategories.Career)
                {
                    sim.CareerOutfitIndex = index;
                }
            }

            return OptionResult.SuccessRetain;
        }
    }
}
