using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.TrafficSpace.Helpers
{
    public class UpdateIceCreamTruckTaskEx : RepeatingTask
    {
        protected override bool OnPerform()
        {
            if ((IceCreamTruckManager.Singleton == null) || (LotManager.sLots == null))
            {
                return true;
            }

            Simulator.DestroyObject(IceCreamTruckManager.Singleton.mIceCreamTruckManager);
            IceCreamTruckManager.Singleton.mIceCreamTruckManager = ObjectGuid.InvalidObjectGuid;

            bool enable = true;

            if (Common.IsAwayFromHomeworld())
            {
                if (!Traffic.Settings.mEnableIceCreamTruckVacation)
                {
                    enable = false;
                }
            }

            if (Traffic.Settings.mMaxIceCreamTrucks == 0)
            {
                enable = false;
            }

            if (enable)
            {
                IceCreamTruckManagerEx.Update(IceCreamTruckManager.Singleton);

                SpeedTrap.Sleep(0x12c);
            }

            return true;
        }

        public class Loader : Common.IPreLoad, Common.IDelayedWorldLoadFinished
        {
            public void OnPreLoad()
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP5)) return;

                UpdateIceCreamTruckTaskEx.Create<UpdateIceCreamTruckTaskEx>();
            }

            public void OnDelayedWorldLoadFinished()
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP5)) return;

                if (IceCreamTruckManager.Singleton == null)
                {
                    IceCreamTruckManager.Singleton = new IceCreamTruckManager();
                }

                IceCreamTruckManager.Singleton.mIceCreamTrucks = new List<IceCreamTruck>(Sims3.Gameplay.Queries.GetObjects<IceCreamTruck>());
            }
        }
    }
}


