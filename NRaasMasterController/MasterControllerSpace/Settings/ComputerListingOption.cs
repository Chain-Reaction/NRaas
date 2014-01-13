using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class ComputerListingOption : OptionList<ISettingOption>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "SettingInteraction";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!base.Allow(parameters)) return false;

            return ((Common.IsRootMenuObject(parameters.mTarget)) || (parameters.mTarget is Computer));
        }
    }
}
