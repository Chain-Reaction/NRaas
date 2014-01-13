using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public abstract class DreamJob
    {
        public readonly OccupationNames mCareer;

        public DreamJob(OccupationNames career)
        {
            mCareer = career;
        }

        public static bool Contains(List<DreamJob> jobs, OccupationNames career)
        {
            foreach (DreamJob job in jobs)
            {
                if (job == null) continue;

                if (job.mCareer == career)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Satisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (!inspecting)
            {
                Occupation career = CareerManager.GetStaticOccupation(mCareer);
                if (career == null) return false;

                if ((GameUtils.IsFutureWorld()) && (!career.AvailableInFutureWorld)) return false;

                if (sim.IsEP11Bot)
                {
                    if (!sim.HasTrait(TraitNames.ProfessionalChip))
                    {
                        return false;
                    }
                }

                if (sim.CreatedSim != null)
                {
                    if ((sim.Occupation == null) || (sim.Occupation.Guid != mCareer))
                    {
                        GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                        if (!career.CanAcceptCareer(sim.CreatedSim.ObjectId, ref greyedOutTooltipCallback)) return false;
                    }
                }
            }

            return PrivateSatisfies(manager, sim, newLot, inspecting);
        }

        protected abstract bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting);
    }
}

