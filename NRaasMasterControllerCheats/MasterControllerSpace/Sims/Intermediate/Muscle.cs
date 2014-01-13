using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class Muscle : NormalMapValue, IIntermediateOption
    {
        public override string GetTitlePrefix()
        {
            return "MuscleValue";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.ChildOrBelow) return false;

            return true;
        }

        protected override void ApplyValue(SimBuilder builder, float value)
        {
            NormalMap.ApplyMuscleValue(builder, value);
        }

        protected override float GetValue(SimBuilder builder)
        {
            return builder.GetSecondaryNormalMapWeights()[0];
        }
    }
}
