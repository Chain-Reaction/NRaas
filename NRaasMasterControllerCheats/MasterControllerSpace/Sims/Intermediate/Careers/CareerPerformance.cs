using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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
    public class CareerPerformance : PerformanceBase
    {
        public override string GetTitlePrefix()
        {
            return "CareerPerformance";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override float GetValue(SimDescription me)
        {
            return StatusJobPerformance.GetPerformance(me);
        }

        protected override void SetValue(SimDescription me, float value)
        {
            Sims3.Gameplay.Careers.Career career = me.Occupation as Sims3.Gameplay.Careers.Career;
            if (career != null)
            {
                career.mPerformance = value;
            }
            else
            {
                AcademicCareer academicCareer = me.OccupationAsAcademicCareer;
                if (academicCareer != null)
                {
                    if (me.CreatedSim != null)
                    {
                        me.CreatedSim.Motives.SetValue(CommodityKind.AcademicPerformance, value);
                    }
                }
                else
                {
                    XpBasedCareer xpCareer = me.Occupation as XpBasedCareer;
                    if (xpCareer != null)
                    {
                        xpCareer.mXp = (value / 100f) * xpCareer.GetCurrentLevelGoalXp();
                    }
                }
            }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Occupation == null) return false;

            return true;
        }
    }
}
