using Sims3.SimIFace;
using System;

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

