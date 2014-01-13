using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class Aging
    {
        static CASAgeGenderFlags[] sAges = new CASAgeGenderFlags[] { CASAgeGenderFlags.Baby, CASAgeGenderFlags.Toddler, CASAgeGenderFlags.Child, CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };

        static CASAgeGenderFlags[] sPetAges = new CASAgeGenderFlags[] { CASAgeGenderFlags.Child, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };

        public static IEnumerable<CASAgeGenderFlags> GetAges(CASAgeGenderFlags species)
        {
            if (species == CASAgeGenderFlags.Human)
            {
                return sAges;
            }
            else
            {
                return sPetAges;
            }
        }

        public static float GetCurrentAgeInDays(IMiniSimDescription sim)
        {
            float agingYears = 0;

            foreach (CASAgeGenderFlags age in GetAges(sim.Species))
            {
                if (age >= sim.Age) continue;

                agingYears += AgingManager.GetAgingStageLength(sim.Species, age);
            }

            agingYears += sim.YearsSinceLastAgeTransition;

            return AgingManager.Singleton.AgingYearsToSimDays(agingYears);
        }
    }
}

