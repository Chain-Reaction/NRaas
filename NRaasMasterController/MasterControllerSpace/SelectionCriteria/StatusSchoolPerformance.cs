using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
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
    public class StatusSchoolPerformance : SelectionTestableOptionList<StatusSchoolPerformance.Item,string,string>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.StatusSchoolPerformance";
        }

        public class Item : TestableOption<string,string>
        {
            public override void SetValue(string value, string storeType)
            {
                mValue = value;

                mName = value;
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<string,string> results)
            {
                string performance = Performance(me);
                if (performance == null) return false;

                results[performance] = performance;
                return true;
            }

            public static string Performance(SimDescription me)
            {
                if (me.CareerManager == null) return null;

                if (me.CareerManager.School == null) return null;

                int iPerf = 0;
                if (me.CareerManager.School.Performance > 0)
                {
                    iPerf = (int)Math.Ceiling(me.CareerManager.School.Performance / 10f);
                    if (iPerf > 9)
                    {
                        iPerf = 9;
                    }
                    iPerf *= 10;
                }
                else if (me.CareerManager.School.Performance < 0)
                {
                    iPerf = (int)Math.Ceiling(-me.CareerManager.School.Performance / 10f);
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
    }
}
