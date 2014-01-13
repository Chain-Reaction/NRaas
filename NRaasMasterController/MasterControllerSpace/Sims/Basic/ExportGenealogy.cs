using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class ExportGenealogy : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "DumpGenealogy";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.Genealogy != null);
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            List<IMiniSimDescription> allSims = new List<IMiniSimDescription>(sims);

            Dictionary<ulong,List<IMiniSimDescription>> lookup = new Dictionary<ulong,List<IMiniSimDescription>>();

            foreach (IMiniSimDescription sim in sims)
            {
                SimListing.AddSim(sim, lookup);
            }

            int index = 0;
            while (index < allSims.Count)
            {
                IMiniSimDescription miniSim = allSims[index];
                index++;

                Genealogy genealogy = miniSim.CASGenealogy as Genealogy;
                if (genealogy == null) continue;

                if (genealogy.Spouse != null)
                {
                    IMiniSimDescription spouseSim = genealogy.Spouse.IMiniSimDescription;
                    if (spouseSim == null) continue;

                    if (SimListing.AddSim(spouseSim, lookup))
                    {
                        allSims.Add(spouseSim);
                    }
                }

                foreach(Genealogy parent in genealogy.Parents)
                {
                    IMiniSimDescription parentSim = parent.IMiniSimDescription;
                    if (parentSim == null) continue;

                    if (SimListing.AddSim(parentSim, lookup))
                    {
                        allSims.Add(parentSim);
                    }
                }

                foreach(Genealogy sibling in genealogy.Siblings)
                {
                    IMiniSimDescription siblingSim = sibling.IMiniSimDescription;
                    if (siblingSim == null) continue;

                    if (SimListing.AddSim(siblingSim, lookup))
                    {
                        allSims.Add(siblingSim);
                    }
                }

                foreach(Genealogy child in genealogy.Children)
                {
                    IMiniSimDescription childSim = child.IMiniSimDescription;
                    if (childSim == null) continue;

                    if (SimListing.AddSim(childSim, lookup))
                    {
                        allSims.Add(childSim);
                    }
                }
            }

            return NRaas.MasterControllerSpace.Town.DumpGenealogy.Perform(lookup);
        }
    }
}
