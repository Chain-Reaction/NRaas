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
    public class StatusDriverType : SelectionTestableOptionList<StatusDriverType.Item, NpcDriversManager.NpcDrivers, NpcDriversManager.NpcDrivers>, IDoesNotNeedSpeciesFilter
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.StatusDriverType";
        }

        public class Item : TestableOption<NpcDriversManager.NpcDrivers,NpcDriversManager.NpcDrivers>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<NpcDriversManager.NpcDrivers, NpcDriversManager.NpcDrivers> results)
            {
                NpcDriversManager.NpcDrivers driverType;
                if (!SimTypes.InCarPool(me, out driverType)) return false;

                results[driverType] = driverType;
                return true;
            }

            public override void SetValue(NpcDriversManager.NpcDrivers value, NpcDriversManager.NpcDrivers storeType)
            {
                mValue = value;

                mName = Common.Localize("CarpoolType:" + value.ToString());
            }
        }
    }
}
