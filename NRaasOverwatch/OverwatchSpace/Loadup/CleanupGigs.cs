using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupGigs : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupGigs");

            foreach(SimDescription sim in Household.EverySimDescription())
            {
                PerformanceCareer career = sim.OccupationAsPerformanceCareer;
                if (career == null) continue;

                if (career.mSteadyGigs == null)
                {
                    career.mSteadyGigs = new List<SteadyGig>();

                    Overwatch.Log(" Missing GigList Added: " + sim.FullName);
                }

                for (int i = career.mSteadyGigs.Count - 1; i >= 0; i--)
                {
                    SteadyGig gig = career.mSteadyGigs[i];
                    if (gig == null)
                    {
                        career.mSteadyGigs.RemoveAt(i);

                        Overwatch.Log(" Null Gig removed: " + sim.FullName);
                    }
                    else if (gig.Venue == null)
                    {
                        career.mSteadyGigs.RemoveAt(i);

                        Overwatch.Log(" No Venue Gig removed: " + sim.FullName);
                    }
                    else if (MiniSimDescription.Find (gig.ProprietorId) == null)
                    {
                        career.mSteadyGigs.RemoveAt(i);

                        Overwatch.Log(" No Proprietor Gig removed: " + sim.FullName);
                    }
                }
            }
        }
    }
}
