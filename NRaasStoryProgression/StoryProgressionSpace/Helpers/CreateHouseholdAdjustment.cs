using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class CreateHouseholdAdjustment : Common.IPreLoad, Common.IWorldQuit
    {
        public void OnPreLoad()
        {
            StoryProgressionServiceEx.DisableCreateHousehold();
        }

        public void OnWorldQuit()
        {
            StoryProgressionServiceEx.DisableCreateHousehold();
        }
    }
}

