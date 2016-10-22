using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Elevator;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class CleanupElevators : AlarmOption
    {
        public override string GetTitlePrefix()
        {
            return "CleanupElevators";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mCleanupElevators;
            }
            set
            {
                NRaas.Overwatch.Settings.mCleanupElevators = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            Overwatch.Log(Name);

            int count = 0;

            foreach (ElevatorDoors obj in Sims3.Gameplay.Queries.GetObjects<ElevatorDoors>())
            {
                ElevatorInterior.ElevatorPortalComponent comp = obj.InteriorObj.ElevatorPortal as ElevatorInterior.ElevatorPortalComponent;
                if (comp != null)
                {
                    if (comp.mAssignedSims != null)
                    {
                        foreach (SimDescription sim in new List<SimDescription>(comp.mAssignedSims.Keys))
                        {
                            if (sim.CreatedSim != null)
                            {
                                ResetSimTask.Perform(sim.CreatedSim, true);
                            }
                        }
                    }
                }

                obj.SetObjectToReset();
                count++;
            }

            Overwatch.Log("Elevators Reset: " + count);
        }
    }
}
