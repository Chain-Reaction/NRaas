using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Options
{
    public class ChangeHometownSetting : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "ChangeHometown";
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters< GameObject> parameters)
        {
            // Use GameUtils, it tests against CurrentWorldType
            if (!GameUtils.IsOnVacation()) return false;

            if (GameStates.TravelHousehold != Household.ActiveHousehold) return false;

            if (GameStates.TravelHousehold.LotHome == null) return false;

            if (!GameStates.TravelHousehold.LotHome.IsResidentialLot) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< GameObject> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt"))) return OptionResult.Failure;

            // Store for later use
            List<SimDescription> sims = new List<SimDescription>(GameStates.TravelHousehold.AllSimDescriptions);

            GameStates.sTravelData = null;

            WorldData.SetVacationWorld(false, true);

            WorldName currentWorld = GameUtils.GetCurrentWorld();

            foreach (SimDescription sim in sims)
            {
                MiniSimDescription miniSim = MiniSimDescription.Find(sim.SimDescriptionId);
                if (miniSim != null)
                {
                    miniSim.mHomeWorld = currentWorld;
                }

                sim.mHomeWorld = currentWorld;
            }

            if (Sim.ActiveActor != null)
            {
                (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimAgeChanged(Sim.ActiveActor.ObjectId);
            }

            return OptionResult.SuccessClose;
        }
    }
}
