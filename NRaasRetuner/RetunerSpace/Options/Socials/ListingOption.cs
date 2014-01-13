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
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RetunerSpace.Options.Socials
{
    public class ListingOption : InteractionOptionList<ActionDataOption, GameObject>, ITuningOption
    {
        public ListingOption()
        { }

        public override string GetTitlePrefix()
        {
            return "SocialRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<ActionDataOption> GetOptions()
        {
            List<ActionDataOption> results = new List<ActionDataOption>();

            Dictionary<string, ActionDataOption> lookup = new Dictionary<string, ActionDataOption>();

            foreach (ActionData data in ActionData.sData.Values)
            {
                ActionDataOption option = new ActionDataOption(data);

                ActionDataOption original;
                if (lookup.TryGetValue(option.Name, out original))
                {
                    option.AppendKey();

                    original.AppendKey();
                }
                else
                {
                    lookup.Add(option.Name, option);
                }

                results.Add(option);
            }

            return results;
        }
    }
}
