using NRaas.CommonSpace;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Fireplaces;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Helpers
{
    public class FireplaceEx : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.AlarmTask(1, TimeUnit.Minutes, this.ShouldAutoLight, 2, TimeUnit.Hours);           
        }

        public void ShouldAutoLight()
        {
            if (SeasonsManager.sInstance == null)
            {
                return;
            }

            if (!Tempest.Settings.mAutoLightFireplaces)
            {
                return;
            }

            foreach (Fireplace fireplace in Sims3.Gameplay.Queries.GetObjects<Fireplace>())
            {
                if (!fireplace.InWorld) continue;

                Lot lotCurrent = fireplace.LotCurrent;

                if (lotCurrent == null) continue;
                if (lotCurrent.Household == null) continue;

                try
                {
                    if ((fireplace.Upgradable != null || fireplace is FireplaceUltra) && fireplace.CanAutoLight)
                    {
                        if (SeasonsManager.Temperature <= Tempest.Settings.mAutoLightFireplacesTemperature)
                        {
                            if (!fireplace.IsLit && lotCurrent.Household.IsActive && lotCurrent.Household.HasMemberOnLot(lotCurrent))
                            {
                                fireplace.StartFire();
                            }
                        }
                        else
                        {
                            fireplace.UserControlled = true;
                            fireplace.StopFire();
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception("FireplaceEx:ShouldAutoLight", e);
                }
            }
        }
    }
}
