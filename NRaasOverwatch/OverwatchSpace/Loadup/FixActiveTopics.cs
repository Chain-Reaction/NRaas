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
                         // Not sure why these don't work on their own, it may be the active
                        // topic limit. Either way readding them should troop the limit and fix
                        // them if they are truly broken
                        case SkillNames.Bartending:
                            ActiveTopic.AddToSim(sim.CreatedSim, "Bartending Skill");
                            Overwatch.Log("Readded Active Topic - Bartending Skill: " + sim.FullName);
                            break;
                        case SkillNames.Charisma:
                            ActiveTopic.AddToSim(sim.CreatedSim, "Smooth Recovery");
                            Overwatch.Log("Readded Active Topic - Charisma Skill: " + sim.FullName);
                            break;                                               
                        case SkillNames.Logic:
                            if (skill.SkillLevel >= LogicSkill.kSkillLevelForSchoolTutor)
                            {
                                ActiveTopic.AddToSim(sim.CreatedSim, "Logic Skill");
                                Overwatch.Log("Readded Active Topic - Logic Skill: " + sim.FullName);
                            }
                            break;                        
                        case SkillNames.MartialArts:
                            ActiveTopic.AddToSim(sim.CreatedSim, "Martial Arts Skill");
                            Overwatch.Log("Readded Active Topic - Martial Arts Skill: " + sim.FullName);
                            break;
                        // hopelessly broken regardless
                        case SkillNames.Science:
                            ActiveTopic.AddToSim(sim.CreatedSim, "Science Skill");
                            Overwatch.Log("Readded Active Topic - Science Skill: " + sim.FullName);
                            break; 
                    }
                }
            }            
        }
    }
}