using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims.Status;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Tags
{
    public class ReapplyTags : OptionItem, ITagsOption
    {
        public override string GetTitlePrefix()
        {
            return "ReapplyTags";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (MasterController.Settings.mLastTagFilter == null) return false;

            if (Household.ActiveHousehold == null) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            foreach (Sim sim in LotManager.Actors)
            {
                RemoveTag.Perform(sim);
            }

            AddTag addTag = new AddTag();

            bool criteriaCanceled;
            SimSelection sims = SimSelection.Create(Name, parameters.mActor.SimDescription, addTag, MasterController.Settings.mLastTagFilter.Elements, true, true, true, out criteriaCanceled);

            foreach (Sim sim in CommonSpace.Helpers.Households.AllSims(Household.ActiveHousehold))
            {
                MapTagManager manager = sim.MapTagManager;
                if (manager == null) continue;

                foreach (SimDescription choice in sims.All)
                {
                    if (choice.CreatedSim == null) continue;

                    AddTag.Perform(manager, choice.CreatedSim);
                }
            }


            return OptionResult.SuccessClose;
        }
    }
}
