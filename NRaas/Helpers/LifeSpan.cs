using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class LifeSpan
    {
        static CASAgeGenderFlags[] sAges = new CASAgeGenderFlags[] { CASAgeGenderFlags.Baby, CASAgeGenderFlags.Toddler, CASAgeGenderFlags.Child, CASAgeGenderFlags.Teen, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Adult, CASAgeGenderFlags.Elder };

        public static int GetHumanAgeSpanLength()
        {
            float agingYears = 0f;

            foreach (CASAgeGenderFlags age in sAges)
            {
                agingYears += AgingManager.GetAgingStageLength(CASAgeGenderFlags.Human, age);
            }

            return (int)AgingManager.Singleton.AgingYearsToSimDays(agingYears);
        }
    }
}

