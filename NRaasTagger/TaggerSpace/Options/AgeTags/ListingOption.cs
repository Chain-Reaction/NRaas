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

namespace NRaas.TaggerSpace.Options.AgeTags
{
    public class ListingOption : InteractionOptionList<AgeTagListingOption, GameObject>, IPrimaryOption<GameObject>
    {
        public ListingOption()
        { }

        public override string GetTitlePrefix()
        {
            return "AgeTagsRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<AgeTagListingOption> GetOptions()
        {
            List<AgeTagListingOption> results = new List<AgeTagListingOption>();

            foreach (CASAgeGenderFlags flag in Enum.GetValues(typeof(CASAgeGenderFlags)))
            {
                switch (flag)
                {
                    case CASAgeGenderFlags.Baby:
                    case CASAgeGenderFlags.Toddler:
                    case CASAgeGenderFlags.Child:
                    case CASAgeGenderFlags.Teen:
                    case CASAgeGenderFlags.YoungAdult:
                    case CASAgeGenderFlags.Adult:
                    case CASAgeGenderFlags.Elder:
                        results.Add(new AgeTagListingOption(flag));
                        break;
                    default:
                        break;
                }              
            }

            return results;
        }
    }
}