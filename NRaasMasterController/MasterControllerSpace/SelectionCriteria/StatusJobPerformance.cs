using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class StatusJobPerformance : SelectionTestableOptionList<StatusJobPerformance.Item,string,string>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.StatusJobPerformance";
        }

        public class Item : TestableOption<string,string>
        {
            public override void SetValue(string value, string storeType)
            {
                mValue = value;

                mName = value;
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<string, string> results)
            {
                string performance = Performance(me);
                if (performance == null) return false;

                results[performance] = performance;
                return true;
            }

            public static string Performance(SimDescription me)
            {
                if (me.Occupation == null) return null;

                int iPerf = 0;
                float performance = GetPerformance(me);
                if (performance > 0)
                {
                    iPerf = (int)Math.Ceiling(performance / 10f);
                    if (iPerf > 9)
                    {
                        iPerf = 9;
                    }
                    iPerf *= 10;
                }
                else if (performance < 0)
                {
                    iPerf = (int)Math.Ceiling(-performance / 10f);
                    if (iPerf > 9)
                    {
                        iPerf = 9;
                    }
                    iPerf *= -10;
                }

                if (iPerf >= 0)
                {
                    return "+" + iPerf.ToString("D2");
                }
                else
                {
                    return iPerf.ToString("D2");
                }
            }
        }

        public static float GetPerformance(SimDescription me)
        {
            if (me.Occupation == null) return 0;

            AcademicCareer academic = me.Occupation as AcademicCareer;
            if (academic != null)
            {
                return me.Occupation.Performance;
            }
            else
            {
                XpBasedCareer xpCareer = me.Occupation as XpBasedCareer;
                if (xpCareer != null)
                {
                    return (me.Occupation.Performance * 100);
                }
                else
                {
                    return me.Occupation.Performance;
                }
            }
        }
    }
}
