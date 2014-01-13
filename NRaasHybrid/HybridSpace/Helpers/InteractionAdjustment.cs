using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Helpers
{
    public class InteractionAdjustment : Common.IPreLoad
    {
        static Dictionary<string, bool> sIgnore = new Dictionary<string, bool>();

        static InteractionAdjustment()
        {
            sIgnore["Sims3.Gameplay.Actors.Sim+MosquitoBeBitten+Definition"] = true;
            sIgnore["Sims3.Gameplay.Objects.Decorations.FishTank+ScoopOut+MermaidDefinition"] = true;
            sIgnore["Sims3.Gameplay.Objects.Seating.LifeguardChair+JoinLifeGuardCareer+Definition"] = true;
            sIgnore["Sims3.Store.Objects.DoorOfLifeAndDeath+SimulateGeneticMerger+Definition"] = true;
        }

        public void OnPreLoad()
        {
            foreach (Dictionary<uint, InteractionTuning> list in AutonomyTuning.sTuning.Values)
            {
                foreach (InteractionTuning tuning in list.Values)
                {
                    if (sIgnore.ContainsKey(tuning.FullInteractionName)) continue;

                    if (tuning.Availability.OccultRestrictionType == OccultRestrictionType.Inclusive)
                    {
                        BooterLogger.AddTrace("Occult Altered: " + tuning.FullInteractionName + " - " + tuning.FullObjectName);
                        BooterLogger.AddTrace("  " + tuning.Availability.OccultRestrictionType);
                        BooterLogger.AddTrace("  " + tuning.Availability.OccultRestrictions);

                        tuning.Availability.OccultRestrictionType = OccultRestrictionType.Ignore;
                    }
                }
            }

            //BooterLogger.AddError("Error");
        }
    }
}
