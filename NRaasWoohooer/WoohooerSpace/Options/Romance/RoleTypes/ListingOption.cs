using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Romance.RoleTypes
{
    public class ListingOption : OptionList<RoleTypeSetting>, IRomanceOption
    {
        public override string GetTitlePrefix()
        {
            return "DisallowRoleType";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (NRaas.Woohooer.Settings.mDisallowHomeless) return false;

            return base.Allow(parameters);
        }

        public override List<RoleTypeSetting> GetOptions()
        {
            List<RoleTypeSetting> results = new List<RoleTypeSetting>();

            foreach (Role.RoleType type in Enum.GetValues(typeof(Role.RoleType)))
            {
                if (type == Role.RoleType.None) continue;

                results.Add(new RoleTypeSetting(type));
            }

            return results;
        }
    }
}
