using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.CAS;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class PlayWithGenetics : SimFromList, IAdvancedOption
    {
        public override string GetTitlePrefix()
        {
            return "PlayWithGenetics";
        }

        protected override int GetMaxSelection()
        {
            return 1;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            List<SimDescription> sims = new List<SimDescription>();

            bool okayed;
            List<IMiniSimDescription> miniSims = FilteredSelection.Selection(me, Common.Localize("FamilyTreeInteraction:Parent"), new ParentTest(me).OnTest, SelectionOption.List, 7, false, out okayed);
            if (miniSims != null)
            {
                foreach (IMiniSimDescription miniSim in miniSims)
                {
                    SimDescription sim = miniSim as SimDescription;
                    if (sim == null) continue;                    

                    sims.Add(sim);
                }
            }

            if (sims.Count == 0)
            {
                sims.AddRange(Relationships.GetParents(me));
            }

            if (sims.Count < 2)
            {
                SimpleMessageDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Failure"));
                return false;
            }

            CASGeneticsEx.InitHousehold(sims);

            CASGeneticsEx.sChoice = me;

            if (CASGenetics.gSingleton == null)
            {
                CASGenetics.Load();
            }

            return true;
        }

        public class ParentTest
        {
            SimDescription mChild;

            public ParentTest(SimDescription child)
            {
                mChild = child;
            }

            public bool OnTest(SimDescription parent)
            {
                if (mChild == parent) return false;

                if (parent.GetOutfitCount(OutfitCategories.Everyday) == 0) return false;

                return (mChild.Species == parent.Species);
            }
        }
    }
}
