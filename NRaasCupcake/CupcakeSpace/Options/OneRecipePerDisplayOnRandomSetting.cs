using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CupcakeSpace.Options
{
    public class OneRecipePerDisplayOnRandom : BooleanSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Value
        {
            get
            {
                return Cupcake.Settings.mOneRecipePerDisplayOnRandom;
            }
            set
            {
                Cupcake.Settings.mOneRecipePerDisplayOnRandom = value;
            }
        }

        public override string GetTitlePrefix()
        {
            return "OneRecipePerDisplayOnRandom";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }
    }
}