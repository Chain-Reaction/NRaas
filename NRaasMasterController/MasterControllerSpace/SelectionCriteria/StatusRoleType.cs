using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class StatusRoleType : SelectionTestableOptionList<StatusRoleType.Item, Role, Role.RoleType>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.StatusRoleType";
        }

        public override bool IsSpecial
        {
            get { return true; }
        }

        public class Item : TestableOption<Role,Role.RoleType>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<Role.RoleType, Role> results)
            {
                if (me.AssignedRole == null) return false;

                results[me.AssignedRole.Type] = me.AssignedRole;
                return true;
            }

            public override void SetValue(Role value, Role.RoleType storeType)
            {
                mValue = value.Type;

                mName = Localize(Roles.GetLocalizedName(value));
            }

            protected static string Localize(string roleName)
            {
                string name = Common.Localize("Status:Role", false, new object[] { roleName });

                if (!string.IsNullOrEmpty(name))
                {
                    name = name.Replace(Common.NewLine, "").Replace("\n", "");
                }

                return name;
            }
        }
    }
}
