using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Options.PayPerRoles
{
    public class ListingOption : InteractionOptionList<IPayPerRolesOption, GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "PayPerRoles";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IPayPerRolesOption> GetOptions()
        {
            List<IPayPerRolesOption> results = new List<IPayPerRolesOption>();

            foreach (Role.RoleType type in Enum.GetValues(typeof(Role.RoleType)))
            {
                switch (type)
                {
                    case Role.RoleType.None:
                    case Role.RoleType.Deer:
                    case Role.RoleType.Explorer:
                    case Role.RoleType.Raccoon:
                    case Role.RoleType.Tourist:                    
                    case Role.RoleType.TimeTraveler:                    
                    case Role.RoleType.FutureHobo:
                        break;
                    default:
                        results.Add(new PayPerRole(type));
                        break;
                }
            }

            return results;
        }
    }
}
