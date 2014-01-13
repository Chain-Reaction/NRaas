using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public class Select : SimFromList, ISimOption
    {       
        public override string Name
        {
            get
            {
                if (NRaas.MasterController.Settings.mDreamCatcher)
                {
                    return Common.Localize("SelectDreamCatcher:MenuName");
                }

                return Common.Localize("Select:MenuName");
            }
        }

        public override string GetTitlePrefix()
        {
            return "Select";
        }

        protected override List<SimSelection.ICriteria> GetCriteria(GameHitParameters<GameObject> parameters)
        {
            return SelectionCriteria.SelectionOption.List;
        }

        public static Sim.Placeholder FindPlaceholderForSim(SimDescription simDesc)
        {
            if (simDesc.LotHome != null)
            {
                foreach (Sim.Placeholder placeholder in simDesc.LotHome.GetObjects<Sim.Placeholder>())
                {
                    if (placeholder.SimDescription == simDesc)
                    {
                        return placeholder;
                    }
                }
            }
            return null;
        }

        protected override bool AllowRunOnActive
        {
            get { return false; }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.IsNeverSelectable) return false;

            if (me.Household == null) return false;

            if (me.Household.IsActive) return false;

            //if (SimTypes.IsSpecial(me)) return false;

            if (me.Household.LotHome == null) return false;

            if (Household.RoommateManager.IsNPCRoommate(me)) return false;

            if (me.CreatedSim != null) return true;

            return (FindPlaceholderForSim(me) != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Sim.Placeholder placeholder = FindPlaceholderForSim(me);
            if (placeholder != null)
            {
                placeholder.Rematerialize();
            }

            return Perform(me);
        }

        public static bool Perform (SimDescription me)
        {
            if (me.IsNeverSelectable) return false;

            if (me.CreatedSim == null) return false;

            if (me.LotHome == null) return false;

            return DreamCatcher.Select(me.CreatedSim, true, MasterController.Settings.mDreamCatcher);
        }
    }
}
