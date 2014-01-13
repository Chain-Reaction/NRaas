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
    public abstract class RelationGenealogy : RelationBase, IDoesNotNeedSpeciesFilter
    {
        protected static bool IsMyAncestor(Genealogy descendant, Genealogy ancestor)
        {
            if ((descendant == null) || (ancestor == null)) return false;

            List<Genealogy> ancestors = new List<Genealogy>(descendant.Parents);

            int index = 0;
            while (index < ancestors.Count)
            {
                Genealogy parent = ancestors[index];
                index++;

                if (parent == ancestor) return true;

                ancestors.AddRange(parent.Parents);
            }

            return false;
        }
    }
}
