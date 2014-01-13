using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.States
{
    public class TravelArrivalStateEx : TravelArrivalState
    {
        public TravelArrivalStateEx()
        { }

        public override void Startup()
        {
            Common.StringBuilder msg = new Common.StringBuilder("TravelArrivalStateEx:Startup");
            Traveler.InsanityWriteLog(msg);

            try
            {
                ToInWorldStateEx.StatusCount = 0;

                LoadSaveManager.ObjectGroupLoaded += ToInWorldStateEx.OnObjectGroupLoaded;

                ToInWorldStateEx.sOnWorldLoadFinishedEventHandler = World.sOnWorldLoadFinishedEventHandler;
                World.sOnWorldLoadFinishedEventHandler = ToInWorldStateEx.OnWorldLoadFinishedEx;

                World.sOnWorldLoadStatusEventHandler += ToInWorldStateEx.OnWorldLoadStatusEx;

                base.Startup();

                msg += "A";
                msg += Common.NewLine + "FileName: " + GameStates.LoadFileName;
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }

        public override void Shutdown()
        {
            Common.StringBuilder msg = new Common.StringBuilder("TravelArrivalStateEx:Shutdown");
            Traveler.InsanityWriteLog(msg);

            try
            {
                base.Shutdown();

                LoadSaveManager.ObjectGroupLoaded -= ToInWorldStateEx.OnObjectGroupLoaded;

                World.sOnWorldLoadStatusEventHandler -= ToInWorldStateEx.OnWorldLoadStatusEx;

                World.sOnWorldLoadFinishedEventHandler -= ToInWorldStateEx.OnWorldLoadFinishedEx;
                World.sOnWorldLoadFinishedEventHandler += ToInWorldStateEx.sOnWorldLoadFinishedEventHandler;

                msg += "A";
                msg += Common.NewLine + "StatusCount: " + ToInWorldStateEx.StatusCount;
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }
    }
}
