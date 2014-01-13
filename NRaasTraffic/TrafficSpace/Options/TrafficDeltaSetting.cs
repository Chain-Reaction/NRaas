using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.TrafficSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.TrafficSpace.Options
{
    public class TrafficDeltaSetting : IntegerSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override int Value
        {
            get
            {
                return Traffic.Settings.mTrafficDelta;
            }
            set
            {
                Traffic.Settings.mTrafficDelta = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "TrafficDelta";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);
            if (result != OptionResult.Failure) 
            {
                UpdateTrafficTaskEx.ApplyTrafficDelta();
            }

            return result;
        }
    }
}


