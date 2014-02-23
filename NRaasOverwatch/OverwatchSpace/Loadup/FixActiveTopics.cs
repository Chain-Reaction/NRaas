using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class FixActiveTopics : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("FixActiveTopics");

            Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);
            foreach (SimDescription sim in sims.Values)
            {
                if (sim.CreatedSim == null) continue;
                if (sim.SkillManager == null) continue;

                foreach (Skill skill in sim.SkillManager.List)
                {
                    if (skill.SkillLevel < 1) continue;
                    switch(skill.Guid)
                    {
                        // EA appears to handle the other skills correctly
                        case SkillNames.Science:
                            ActiveTopic.AddToSim(sim.CreatedSim, "Science Skill");
                            Overwatch.Log("Fixed Science Skill: " + sim.FullName);
                            break;
                    }
                }
            }            
        }
    }
}