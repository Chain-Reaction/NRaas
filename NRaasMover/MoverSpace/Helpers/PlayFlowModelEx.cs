using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MoverSpace.Helpers
{
    public class PlayFlowModelEx
    {
        public static bool ActivateSimScreenMaskedFunc(PlayFlowModel ths, ulong householdId)
        {
            Household selectedHousehold = Household.Find(householdId);
            if (selectedHousehold == null)
            {
                return false;
            }
            Lot lotHome = selectedHousehold.LotHome;
            if (lotHome == null)
            {
                return false;
            }
            bool flag = ActivateSimScreenMaskedFuncFromHousehold(ths, selectedHousehold, lotHome);
            ths.TurnOffMapViewActiveLotMode();
            return flag;
        }

        public static bool ActivateSimScreenMaskedFuncFromHousehold(PlayFlowModel ths, Household selectedHousehold, Lot selectedLot)
        {
            if ((selectedHousehold == null) || (selectedLot == null))
            {
                return false;
            }

            using (DreamCatcher.HouseholdStore store = new DreamCatcher.HouseholdStore(selectedHousehold, Mover.Settings.mDreamCatcher))
            {
                Camera.SetMapViewActiveLotMode(true);
                LotManager.LockActiveLot(selectedLot);

                // Custom
                DreamCatcher.Task.PrepareToBecomeActiveHousehold(selectedHousehold);

                Sim sim = BinCommon.ActivateSim(selectedHousehold, selectedLot);
                if (sim == null)
                {
                    sim = Households.AllSims(selectedHousehold)[0];
                    if (sim != null)
                    {
                        PlumbBob.ForceSelectActor(sim);
                    }
                }

                LotManager.SetWallsViewMode(0x12e);
                selectedLot.SetDisplayLevel(selectedLot.DoesFoundationExistOnLot() ? 0x1 : 0x0);
                if (sim.LotCurrent == sim.LotHome)
                {
                    Camera.FocusOnSelectedSim();
                    Camera.SetView(CameraView.SimView, false, false);
                }
                else
                {
                    Camera.FocusOnLot(sim.LotHome.LotId, 0f);
                    Camera.SetView(CameraView.HouseView, false, false);
                }
            }

            selectedLot.CheckIfLotNeedsBabysitter();
            ths.Sleep(1.5);
            return true;
        }
    }
}
