using Sims3.Gameplay.StoryProgression;
using System;

namespace NRaas.CommonSpace.Helpers
{
    public class StoryProgressionServiceEx
    {
        static ActionTuning sOriginalDemographicTuning = null;
        static StoryActionFactory sOriginalFactoryTuning = null;

        public static void EnableCreateHousehold()
        {
            try
            {
                if (StoryProgressionService.sService == null) return;

                if (StoryProgressionService.sService.mDemographicTuning.ActionTuning.ContainsKey("Create Household")) return;

                if (sOriginalDemographicTuning != null)
                {
                    StoryProgressionService.sService.mDemographicTuning.ActionTuning["Create Household"] = sOriginalDemographicTuning;
                }

                if (sOriginalFactoryTuning != null)
                {
                    StoryProgressionService.sService.mActionFactories["Create Household"] = sOriginalFactoryTuning;
                }
            }
            catch (Exception e)
            {
                Common.Exception("EnableCreateHousehold", e);
            }
        }

        public static bool IsCreateHouseholdAvailable()
        {
            if (StoryProgressionService.sService == null) return false;

            if (!StoryProgressionService.sService.mDemographicTuning.ActionTuning.ContainsKey("Create Household")) return false;

            return true;
        }

        public static bool DisableCreateHousehold()
        {
            try
            {
                if (!IsCreateHouseholdAvailable()) return false;

                if (StoryProgressionService.sService.mDemographicTuning.ActionTuning.TryGetValue("Create Household", out sOriginalDemographicTuning))
                {
                    StoryProgressionService.sService.mDemographicTuning.ActionTuning.Remove("Create Household");
                }

                if (StoryProgressionService.sService.mActionFactories.TryGetValue("Create Household", out sOriginalFactoryTuning))
                {
                    StoryProgressionService.sService.mActionFactories.Remove("Create Household");
                }
            }
            catch (Exception e)
            {
                Common.Exception("DisableCreateHousehold", e);
            }

            return true;
        }

        public class SuppressCreateHousehold : IDisposable
        {
            public SuppressCreateHousehold()
            {
                DisableCreateHousehold();
            }

            public void Dispose()
            {
                EnableCreateHousehold();
            }
        }
    }
}