using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class ReconstituteTheDead : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "ReconstituteTheDead";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Dictionary<ulong,List<IMiniSimDescription>> sims = SimListing.AllSims<IMiniSimDescription>(null, true);
            if ((sims == null) || (sims.Count == 0)) return OptionResult.Failure;

            List<IMiniSimDescription> dead = new List<IMiniSimDescription>();

            foreach (List<IMiniSimDescription> miniSims in sims.Values)
            {
                foreach (IMiniSimDescription miniSim in miniSims)
                {
                    SimDescription sim = miniSim as SimDescription;
                    if (sim == null) continue;

                    if ((!sim.IsDead) && (!sim.IsGhost) && (sim.Household != null)) continue;

                    if (sim.IsPlayableGhost) continue;

                    Urnstone urnstone = Sims3.Gameplay.Objects.Urnstone.FindGhostsGrave(sim);
                    if (urnstone != null)
                    {
                        if ((urnstone.InInventory) || (urnstone.InWorld))
                        {
                            continue;
                        }
                    }

                    dead.Add(sim);
                }
            }

            int count = 0;
            if (dead.Count > 0)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("ReconstituteTheDead:Prompt", false, new object[] { dead.Count })))
                {
                    return OptionResult.Failure;
                }

                foreach (SimDescription sim in dead)
                {
                    if (Urnstones.CreateGrave(sim, false) != null)
                    {
                        count++;
                    }
                }
            }

            SimpleMessageDialog.Show(Name, Common.Localize("ReconstituteTheDead:Success", false, new object[] { count }));
            return OptionResult.SuccessClose;
        }
    }
}
