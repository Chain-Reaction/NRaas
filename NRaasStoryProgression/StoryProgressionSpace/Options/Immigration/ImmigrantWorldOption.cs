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
    public class ImmigrantWorldOption<TManager> : MultiEnumManagerOptionItem<TManager, WorldName>, ISimFromBinOption<TManager>
        where TManager : StoryProgressionObject, ISimFromBinManager
    {
        SimFromBinController mController;

        public ImmigrantWorldOption()
            : base(new WorldName[0])
        { }

        public override string GetTitlePrefix()
        {
            return "ImmigrantWorld";
        }

        protected override string LocalizeValue(string key, object[] parameters)
        {
            return Common.LocalizeEAString(IsFemaleLocalization(), "Ui/Caption/Global/WorldName/EP01:" + key, parameters);
        }

        protected override string GetLocalizationUIKey()
        {
            return null;
        }

        protected override bool Allow(WorldName value)
        {
            switch (value)
            {
                case WorldName.China:
                case WorldName.France:
                case WorldName.Egypt:
                case WorldName.UserCreated:
                    return true;
            }

            return false;
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

