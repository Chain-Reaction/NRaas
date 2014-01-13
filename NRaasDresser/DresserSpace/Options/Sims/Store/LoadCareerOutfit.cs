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
    public class LoadCareerOutfit : OperationSettingOption<Sim>, IStoreOption
    {
        public override string GetTitlePrefix()
        {
            return "LoadCareerOutfit";
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (parameters.mTarget.SimDescription.IsUsingMaternityOutfits) return false;

            if (CASParts.GetOutfit(parameters.mTarget.SimDescription, new CASParts.Key(Dresser.GetStoreOutfitKey(OutfitCategories.Career, parameters.mTarget.SimDescription.IsUsingMaternityOutfits)), true) == null) return false;

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

            CASParts.Key outfitKey = new CASParts.Key(Dresser.GetStoreOutfitKey(OutfitCategories.Career, sim.IsUsingMaternityOutfits));

            SimOutfit source = CASParts.GetOutfit(sim, outfitKey, true);

            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, new CASParts.Key(OutfitCategories.Career, 1), geneOutfit))
            {
                new SavedOutfit(source).Apply(builder, false, null, CASParts.GeneticBodyTypes);
            }

            sim.CareerOutfitIndex = 1;

            return OptionResult.SuccessRetain;
        }
    }
}
