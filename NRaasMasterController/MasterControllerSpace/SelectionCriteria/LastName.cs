using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class LastName : SelectionOption, IDoesNotNeedSpeciesFilter
    {
        string mPrefix;

        public override string GetTitlePrefix()
        {
            return "Criteria.LastName";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            if (me.LastName == null) return false;

            return me.LastName.Trim().ToLower().StartsWith(mPrefix);
        }
        protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            if (me.LastName == null) return false;

            return me.LastName.Trim().ToLower().StartsWith(mPrefix);
        }

        public override SimSelection.UpdateResult Update(IMiniSimDescription actor, IEnumerable<SimSelection.ICriteria> criteria, List<IMiniSimDescription> allSims, bool secondStage)
        {
            if (secondStage) return SimSelection.UpdateResult.Success;

            mPrefix = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", actor.IsFemale), "");
            if (string.IsNullOrEmpty(mPrefix)) return SimSelection.UpdateResult.Failure;

            mPrefix = mPrefix.Trim().ToLower();

            return base.Update(actor, criteria, allSims, secondStage);
        }
    }
}
