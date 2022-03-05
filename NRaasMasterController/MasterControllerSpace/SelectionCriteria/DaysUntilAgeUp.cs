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
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class DaysUntilAgeUp : SelectionTestableOptionList<DaysUntilAgeUp.Item, int, int>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.DaysUntilAgeUp";
        }

        public class Item : TestableOption<int, int>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<int,int> results)
            {
                try
                {
                    float agingYearsSinceLastAgeTransition = me.AgingYearsSinceLastAgeTransition;

                    int daysToTransition = (int)(AgingManager.Singleton.AgingYearsToSimDays(AgingManager.GetMaximumAgingStageLength(me)) - AgingManager.Singleton.AgingYearsToSimDays(agingYearsSinceLastAgeTransition));

                    results[daysToTransition] = daysToTransition;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<int, int> results)
            {
                try
                {
                    float agingYearsSinceLastAgeTransition = me.AgingYearsSinceLastAgeTransition;

                    int daysToTransition = (int)(AgingManager.Singleton.AgingYearsToSimDays(AgingManager.GetMaximumAgingStageLength(me)) - AgingManager.Singleton.AgingYearsToSimDays(agingYearsSinceLastAgeTransition));

                    results[daysToTransition] = daysToTransition;
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public override void SetValue(int value, int storeType)
            {
                mValue = value;

                mName = EAText.GetNumberString(value);
            }
        }
    }
}
