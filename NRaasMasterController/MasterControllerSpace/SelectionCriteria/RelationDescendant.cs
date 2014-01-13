using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
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
    public class RelationDescendant : RelationGenealogy
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.RelationDescendant";
        }
        
        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            return IsMyAncestor(me.Genealogy, actor.CASGenealogy as Genealogy);
        }
        protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            return IsMyAncestor(me.CASGenealogy as Genealogy, actor.CASGenealogy as Genealogy);
        }
    }
}
