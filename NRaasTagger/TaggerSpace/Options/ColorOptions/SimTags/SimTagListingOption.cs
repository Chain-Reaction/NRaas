using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TaggerSpace.Options.ColorOptions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.ColorOptions.SimTags
{
    public class SimTagListingOption : InteractionOptionList<ISimTagColorOption, GameObject>, IColorRootOption
    {
        public override string GetTitlePrefix()
        {
            return "SimTagRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return Tagger.Settings.mEnableSimTags;
        }
    }
}