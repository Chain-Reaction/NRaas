using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios;
using Sims3.Gameplay.CAS;
using Sims3.UI.Hud;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class CustomTests
    {
        public static bool OnTestForVampires(SimScenarioFilter.Parameters personality, SimDescription actor, SimDescription potential)
        {
            foreach (SimDescription sim in SimListing.GetResidents(false).Values)
            {
                if (SimTypes.IsOccult(sim, OccultTypes.Vampire))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool OnTestForOccult(SimScenarioFilter.Parameters personality, SimDescription actor, SimDescription potential)
        {
            foreach (SimDescription sim in SimListing.GetResidents(false).Values)
            {
                if (SimTypes.IsOccult(sim, OccultTypes.Vampire))
                {
                    return true;
                }
                else if (SimTypes.IsOccult(sim, OccultTypes.Mummy))
                {
                    return true;
                }
                else if (SimTypes.IsOccult(sim, OccultTypes.Frankenstein))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
