using NRaas.CommonSpace.Options;
using NRaas.DresserSpace.Tasks;
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

namespace NRaas.DresserSpace.Options.Sims
{
    public class Protected : BooleanSettingOption<Sim>, ISimOption
    {
        SimDescription mSim;

        protected override bool Value
        {
            get
            {
                if (mSim == null) return false;

                return Dresser.Settings.IsProtected(mSim);
            }
            set
            {
                if (mSim == null) return;

                Dresser.Settings.SetProtected(mSim, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "Protected";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            mSim = parameters.mTarget.SimDescription;

            return base.Allow(parameters);
        }
    }
}
