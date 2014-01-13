using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.States
{
    public class PlayFlowStateEx : PlayFlowState
    {
        public PlayFlowStateEx()
        { }

        public override void Startup()
        {
            Common.StringBuilder msg = new Common.StringBuilder("PlayFlowStateEx:Startup" + Common.NewLine);
            Traveler.InsanityWriteLog(msg);

            try
            {
                msg += Common.NewLine + "A";
                Traveler.InsanityWriteLog(msg);

                ProductVersion prevVersions = EditTownModel.sEPVenuesPlaced;

                base.Startup();

                if (GameStates.HasTravelData)
                {
                    EditTownModel.sEPVenuesPlaced = prevVersions;
                }

                msg += Common.NewLine + "B";
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }
    }
}
