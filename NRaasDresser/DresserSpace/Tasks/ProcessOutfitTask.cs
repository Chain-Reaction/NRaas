using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NRaas.DresserSpace.Tasks
{
    public class ProcessOutfitTask : Common.FunctionTask
    {
        Sim mSim;

        List<BodyTypes> mTypes;

        CASParts.Key mSourceKey;

        List<OutfitCategories> mIgnore;

        protected ProcessOutfitTask(Sim sim, List<BodyTypes> types, CASParts.Key sourceKey, List<OutfitCategories> ignore)
        {
            mSim = sim;
            mTypes = types;
            mSourceKey = sourceKey;
            mIgnore = ignore;
        }

        public static void Perform(Sim sim, List<BodyTypes> types, CASParts.Key sourceKey, List<OutfitCategories> ignore)
        {
            new ProcessOutfitTask(sim, types, sourceKey, ignore).AddToSimulator();
        }

        protected override void OnPerform()
        {
            try
            {
                ProgressDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Global:Processing", new object[0x0]), false);

                SimOutfit sourceOutfit = CASParts.GetOutfit(mSim.SimDescription, mSourceKey, false);

                List<CASParts.PartPreset> presets = new List<CASParts.PartPreset>();

                foreach (CASPart part in sourceOutfit.Parts)
                {
                    if (!mTypes.Contains(part.BodyType)) continue;

                    presets.Add(CASParts.OutfitBuilder.GetPartPreset(part, sourceOutfit));
                }

                SavedOutfit.Cache cache = new SavedOutfit.Cache(mSim.SimDescription);

                foreach (SavedOutfit.Cache.Key outfit in cache.Outfits)
                {
                    if (outfit.Category == OutfitCategories.Special) continue;

                    if ((mIgnore != null) && (mIgnore.Contains(outfit.Category))) continue;

                    if (outfit.mKey == mSourceKey) continue;

                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(mSim.SimDescription, outfit.mKey))
                    {
                        builder.Builder.RemoveParts(mTypes.ToArray());

                        foreach (CASParts.PartPreset preset in presets)
                        {
                            builder.ApplyPartPreset(preset);
                        }
                    }
                }

                SimOutfit currentOutfit = mSim.CurrentOutfit;
                if (currentOutfit != null)
                {
                    ThumbnailManager.GenerateHouseholdSimThumbnail(currentOutfit.Key, currentOutfit.Key.InstanceId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, mSim.SimDescription.AgeGenderSpecies);
                }
            }
            finally
            {
                ProgressDialog.Close();
            }
        }
    }
}


