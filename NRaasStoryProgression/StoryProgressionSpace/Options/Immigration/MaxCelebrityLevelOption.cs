using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options.Immigration
{
    public class MaxCelebrityLevelOption<TManager> : IntegerBaseManagerOptionItem<TManager>, ISimFromBinOption<TManager>
        where TManager : StoryProgressionObject, ISimFromBinManager
    {
        SimFromBinController mController;

        public MaxCelebrityLevelOption()
            : base(0)
        { }

        public override string GetTitlePrefix()
        {
            return "ImmigrantMaxCelebrityLevel";
        }

        protected override bool PrivatePerform()
        {
            if (!base.PrivatePerform()) return false;

            if (Value > CelebrityManager.HighestLevel)
            {
                SetValue((int)CelebrityManager.HighestLevel);
            }

            return true;
        }

        public SimFromBinController Controller
        {
            set { mController = value; }
        }

        public override bool ShouldDisplay()
        {
            if (!mController.ShouldDisplayImmigrantOptions()) return false;

            return base.ShouldDisplay();
        }
    }
}

