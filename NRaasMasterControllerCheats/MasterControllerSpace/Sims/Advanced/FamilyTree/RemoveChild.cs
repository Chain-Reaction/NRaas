using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.FamilyTree
{
    public class RemoveChild : GenealogyOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveChild";
        }

        protected override bool Allow(Genealogy me)
        {
            if (IsFirst)
            {
                if (me == null) return false;

                if (me.Children.Count == 0) return false;
            }

            return true;
        }

        protected override string GetTitleA()
        {
            return Common.Localize("FamilyTreeInteraction:Parent");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("FamilyTreeInteraction:Child");
        }

        protected override int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return 0;
        }

        protected override ICollection<SimSelection.ICriteria> GetCriteriaB(GameHitParameters<GameObject> parameters)
        {
            return null;
        }

        protected override bool AllowSpecies(IMiniSimDescription a, IMiniSimDescription b)
        {
            return true;
        }

        protected override bool Allow(Genealogy a, Genealogy b)
        {
            if (Genealogy.IsChild(b, a)) return true;

            if (Genealogy.IsParent(a, b)) return true;

            return false;
        }

        protected override bool Run(Genealogy a, Genealogy b)
        {
            b.RemoveDirectRelation(a);
            a.RemoveDirectRelation(b);
            return true;
        }
    }
}
