using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
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

namespace NRaas.TaggerSpace.Options.SimTypeTags
{
    public class ListingOption : InteractionOptionList<SimTypeTagListingOption, GameObject>, IPrimaryOption<GameObject>
    {
        public ListingOption()
        { }

        public override string GetTitlePrefix()
        {
            return "SimTypeTagsRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<SimTypeTagListingOption> GetOptions()
        {
            List<SimTypeTagListingOption> results = new List<SimTypeTagListingOption>();            

            foreach (SimType flag in Enum.GetValues(typeof(SimType)))
            {
                switch(flag)
                {
                    case SimType.Service:
                    case SimType.Dead:
                    case SimType.Tourist:
                    case SimType.Mummy:
                    case SimType.SimBot:
                    case SimType.Human:
                    case SimType.Vampire:
                    case SimType.ImaginaryFriend:
                    case SimType.Genie:
                    case SimType.Fairy:
                    case SimType.Werewolf:
                    case SimType.Witch:
                    case SimType.Zombie:
                    case SimType.BoneHilda:
                    case SimType.Alien:
                    case SimType.Hybrid:
                    case SimType.Plantsim:
                    case SimType.Mermaid:
                    case SimType.Plumbot:
                    case SimType.Deer:
                    case SimType.WildHorse:
                    case SimType.Role:
                    case SimType.Dog:
                    case SimType.Cat:
                    case SimType.Horse:
                    case SimType.Raccoon:
                        results.Add(new SimTypeTagListingOption(flag));
                    break;
                    default:
                    break;
                }
            }

            return results;
        }
    }
}