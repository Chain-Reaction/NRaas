using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupCareers : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupCareers");

            foreach (List<SimDescription> sims in SimListing.AllSims<SimDescription>(null, true).Values)
            {
                foreach (SimDescription sim in sims)
                {
                    if (sim.CareerManager == null) continue;

                    if (sim.CareerManager.mCoworkerAt != null)
                    {
                        List<Occupation> remove = new List<Occupation>();

                        foreach (Occupation occupation in sim.CareerManager.mCoworkerAt.Keys)
                        {
                            if (!Corrections.IsValidCoworker(occupation.OwnerDescription, occupation is School))
                            {
                                remove.Add(occupation);
                            }
                        }

                        foreach (Occupation occ in remove)
                        {
                            sim.CareerManager.ClearCoworkerAtOccupation(occ);

                            Overwatch.Log(" CoworkerAt Removed " + sim.FullName);
                        }
                    }

                    Corrections.FixCareer(sim.Occupation, true, Overwatch.Log);

                    Corrections.FixCareer(sim.CareerManager.RetiredCareer, false, Overwatch.Log);

                    Corrections.FixCareer(sim.CareerManager.School, true, Overwatch.Log);

                    if (sim.CareerManager.QuitCareers != null)
                    {
                        foreach (Occupation occ in sim.CareerManager.QuitCareers.Values)
                        {
                            Corrections.FixCareer(occ, false, Overwatch.Log);
                        }
                    }
                }
            }

            foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
            {
                foreach(CareerLocation location in hole.CareerLocations.Values)
                {
                    for (int i=location.Workers.Count-1; i>=0; i--)
                    {
                        SimDescription worker = location.Workers[i];

                        if (!Corrections.IsValidCoworker(worker, location.Career is School))
                        {
                            Overwatch.Log(" Invalid Worker Dropped " + worker.FullName);

                            location.Workers.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}
