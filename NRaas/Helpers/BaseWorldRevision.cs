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
using Sims3.Gameplay.Objects.Vehicles;
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
    public class BaseWorldReversion : IDisposable
    {
        WorldName mPreviousCheatOverride;

        WorldType mPreviousWorldType;

        bool mAltered;

        public BaseWorldReversion()
        {
            mPreviousCheatOverride = GameUtils.CheatOverrideCurrentWorld;
            mPreviousWorldType = GameUtils.GetCurrentWorldType();

            switch (mPreviousWorldType)
            {
                case WorldType.Vacation:
                case WorldType.University:
                case WorldType.Future:
                    GameUtils.CheatOverrideCurrentWorld = GameUtils.GetCurrentWorld();

                    GameUtils.WorldNameToType[GameUtils.CheatOverrideCurrentWorld] = WorldType.Base;

                    mAltered = true;
                    break;
            }
        }

        public void Dispose()
        {
            if (mAltered)
            {
                GameUtils.WorldNameToType[GameUtils.CheatOverrideCurrentWorld] = mPreviousWorldType;
            }

            GameUtils.CheatOverrideCurrentWorld = mPreviousCheatOverride;
        }
    }
}

