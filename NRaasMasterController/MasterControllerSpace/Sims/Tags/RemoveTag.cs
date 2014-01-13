using NRaas.CommonSpace.Options;
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
    public class RemoveTag : SimFromList, ITagsOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveTag";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            MapTagManager manager = MapTagManager.ActiveMapTagManager;
            if (manager == null) return false;

            return base.Allow(parameters);
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            MapTagManager manager = MapTagManager.ActiveMapTagManager;
            if (manager == null) return false;

            AddTag.TagItem tag = manager.GetTag(me.CreatedSim) as AddTag.TagItem;
            return (tag != null);
        }

        public static bool Perform(Sim me)
        {
            foreach (Sim sim in CommonSpace.Helpers.Households.AllSims(Household.ActiveHousehold))
            {
                MapTagManager manager = sim.MapTagManager;
                if (manager == null) continue;

                AddTag.TagItem tag = manager.GetTag(me) as AddTag.TagItem;
                if (tag == null) continue;

                manager.RemoveTag(me);
            }

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return Perform(me.CreatedSim);
        }
    }
}
