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
    public class RemovePartsOutfitTask : Common.FunctionTask
    {
        SimDescription mSim;

        BodyTypes[] mTypes;

        bool mCurrentOnly;

        PostPerform mPostPerform;

        public delegate void PostPerform();

        protected RemovePartsOutfitTask(SimDescription sim, ICollection<BodyTypes> types, bool currentOnly, PostPerform onPostPerform)
        {
            mSim = sim;
            mTypes = new List<BodyTypes>(types).ToArray();
            mCurrentOnly = currentOnly;
            mPostPerform = onPostPerform;
        }

        public static void Perform(Sim sim, ICollection<BodyTypes> types, bool currentOnly, PostPerform onPostPerform)
        {
            Perform(sim.SimDescription, types, currentOnly, onPostPerform);
        }
        public static void Perform(SimDescription sim, ICollection<BodyTypes> types, bool currentOnly, PostPerform onPostPerform)
        {
            new RemovePartsOutfitTask(sim, types, currentOnly, onPostPerform).AddToSimulator();
        }

        protected override void OnPerform()
        {
            try
            {
                if ((mCurrentOnly) && (mSim.CreatedSim != null))
                {
                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(mSim, new CASParts.Key(mSim.CreatedSim)))
                    {
                        builder.Builder.RemoveParts(mTypes);
                    }
                }
                else
                {
                    ProgressDialog.Show(Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/Global:Processing", new object[0x0]), false);

                    SavedOutfit.Cache cache = new SavedOutfit.Cache(mSim);

                    foreach (SavedOutfit.Cache.Key outfit in cache.Outfits)
                    {
                        using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(mSim, outfit.mKey))
                        {
                            builder.Builder.RemoveParts(mTypes);
                        }
                    }
                }

                if (mSim.CreatedSim != null)
                {
                    SimOutfit currentOutfit = mSim.CreatedSim.CurrentOutfit;
                    if (currentOutfit != null)
                    {
                        ThumbnailManager.GenerateHouseholdSimThumbnail(currentOutfit.Key, currentOutfit.Key.InstanceId, 0x0, ThumbnailSizeMask.Large | ThumbnailSizeMask.ExtraLarge | ThumbnailSizeMask.Medium | ThumbnailSizeMask.Small, ThumbnailTechnique.Default, true, false, mSim.AgeGenderSpecies);
                    }
                }

                if (mPostPerform != null)
                {
                    mPostPerform();
                }
            }
            finally
            {
                ProgressDialog.Close();
            }
        }
    }
}


