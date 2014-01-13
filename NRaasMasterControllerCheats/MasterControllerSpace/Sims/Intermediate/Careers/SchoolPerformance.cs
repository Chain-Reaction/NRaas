using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class SchoolPerformance : PerformanceBase
    {
        public override string GetTitlePrefix()
        {
            return "SchoolPerformance";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override float GetValue(SimDescription me)
        {
            return me.CareerManager.School.Performance;
        }

        protected override void SetValue(SimDescription me, float value)
        {
            me.CareerManager.School.mPerformance = value;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CareerManager == null) return false;

            if (me.CareerManager.School == null) return false;

            return true;
        }
    }
}
