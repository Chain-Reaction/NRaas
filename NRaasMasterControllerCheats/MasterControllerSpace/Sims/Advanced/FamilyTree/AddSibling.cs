using Sims3.Gameplay.Abstracts;
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
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.FamilyTree
{
    public class AddSibling : GenealogyOption
    {
        public override string GetTitlePrefix()
        {
            return "AddSibling";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("FamilyTreeInteraction:Sibling");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("FamilyTreeInteraction:Sibling");
        }

        protected override bool Allow(Genealogy me)
        {
            if (me == null) return false;

            if (IsFirst)
            {
                if (me.Parents.Count > 0) return false;
            }

            return true;
        }

        protected override bool Allow(Genealogy a, Genealogy b)
        {
            if (a.IsBloodRelated(b)) return false;

            return true;
        }

        protected override bool Run(Genealogy a, Genealogy b)
        {
            a.RemoveDirectRelation(b);
            b.RemoveDirectRelation(a);

            b.AddSibling(a);
            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            if (!base.Run(a, b)) return false;

            Relationship.Get(a, b, true).MakeAcquaintances();
            return true;
        }
    }
}
