using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options.Immigration
{
    public class ImmigrantOccultOption<TManager> : MultiEnumManagerOptionItem<TManager, OccultTypes>, ISimFromBinOption<TManager>
        where TManager : StoryProgressionObject, ISimFromBinManager
    {
        SimFromBinController mController;

        public ImmigrantOccultOption()
            : base(new OccultTypes[0])
        { }

        public override string GetTitlePrefix()
        {
            return "ImmigrantOccult";
        }

        protected override string GetLocalizationValueKey()
        {
            return "SimType";
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override bool Allow(OccultTypes value)
        {
            switch (value)
            {
                case OccultTypes.None:
                case OccultTypes.Ghost:
                case OccultTypes.Unicorn:
                    return false;
            }

            return base.Allow(value);
        }

        public SimFromBinController Controller
        {
            set { mController = value; }
        }

        public override bool ShouldDisplay()
        {
            if (!mController.ShouldDisplayImmigrantOptions()) return false;

            if (Manager.GetValue<ChanceOfOccultOption<TManager>, int>() <= 0) return false;

            return base.ShouldDisplay();
        }
    }
}

