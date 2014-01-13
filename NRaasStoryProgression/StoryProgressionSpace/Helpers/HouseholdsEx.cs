using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Pregnancies;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class HouseholdsEx : CommonSpace.Helpers.Households
    {
        public static bool IsFull(Scenario scenario, Household house, CASAgeGenderFlags species, int additional, bool testScoring)
        {
            return IsFull(scenario, house, species, additional, testScoring, RandomUtil.RandomChance(scenario.GetValue<PetAdoptionBaseScenario.ChanceOfAnotherOptionV2, int>()));
        }
        public static bool IsFull(Scenario scenario, Household house, CASAgeGenderFlags species, int additional, bool testScoring, bool allowAnother)
        {
            if (species == CASAgeGenderFlags.Human)
            {
                if (IsFull(house, false, scenario.GetValue<MaximumSizeOption, int>(house) - additional))
                {
                    scenario.IncStat("House Full Human");
                    return true;
                }
            }
            else
            {
                if ((species == CASAgeGenderFlags.Horse) && 
                         ((house.LotHome == null) || (house.LotHome.CountObjects<IBoxStall>() == 0)))
                {
                    scenario.IncStat("No Stalls");
                    return true;
                }
                else if (IsFull(house, true, scenario.GetValue<MaximumPetSizeOption, int>(house) - additional))
                {
                    scenario.IncStat("House Full Pets");
                    return true;
                }
            }

            if ((testScoring) && (species != CASAgeGenderFlags.Human))
            {
                SimDescription head = SimTypes.HeadOfFamily(house);

                int petCount = HouseholdsEx.AllPets(house).Count;
                if (petCount >= scenario.AddScoring("PreferredPetCount", scenario.GetValue<BaseNumberOfPetsOption, int>(head), ScoringLookup.OptionType.Unbounded, head))
                {
                    scenario.IncStat("Enough Pets");
                    return true;
                }
                else if ((petCount > 0) && (!allowAnother))
                {
                    scenario.IncStat("Extra Pet Chance Fail");
                    return true;
                }
            }

            return false;
        }
    }
}

