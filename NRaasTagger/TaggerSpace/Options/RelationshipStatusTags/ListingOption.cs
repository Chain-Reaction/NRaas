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

namespace NRaas.TaggerSpace.Options.RelationshipStatusTags
{
    public class ListingOption : InteractionOptionList<RelationshipStatusTagListingOption, GameObject>, IPrimaryOption<GameObject>
    {
        public ListingOption()
        { }

        public override string GetTitlePrefix()
        {
            return "RelationshipStatusTagsRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<RelationshipStatusTagListingOption> GetOptions()
        {
            List<RelationshipStatusTagListingOption> results = new List<RelationshipStatusTagListingOption>();            
            
            foreach (SimType flag in Enum.GetValues(typeof(SimType)))
            {
                switch (flag)
                {
                    case SimType.Married:
                    case SimType.Partnered:
                    case SimType.Pregnant:
                    case SimType.Single:                    
                        results.Add(new RelationshipStatusTagListingOption(flag));
                    break;
                    default:
                    break;
                }
            }

            return results;
        }
    }
}