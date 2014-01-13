using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class RelationNone : RelationBase
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.RelationNone";
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            Genealogy genealogy = actor.CASGenealogy as Genealogy;
            if (genealogy == null) return false;

            if (me.Genealogy.IsSufficientlyRelatedToRuleOutRomance(genealogy)) return false;

            return true;
        }
        protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            Genealogy genealogy = actor.CASGenealogy as Genealogy;
            if (genealogy == null) return false;

            if (me.Genealogy.IsSufficientlyRelatedToRuleOutRomance(genealogy)) return false;

            return true;
        }
    }
}
