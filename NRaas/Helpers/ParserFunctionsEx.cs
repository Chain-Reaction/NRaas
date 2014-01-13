using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.CommonSpace.Helpers
{
    public sealed class ParserFunctionsEx
    {
        public static bool Parse(string value, out BuffNames result)
        {
            if (!ParserFunctions.TryParseEnum<BuffNames>(value, out result, BuffNames.Undefined))
            {
                result = (BuffNames)ParserFunctions.ParseUlong(value, 0);
                if (result == BuffNames.Undefined)
                {
                    result = (BuffNames)ResourceUtils.HashString64(value);
                }

                if (!BuffManager.BuffDictionary.ContainsKey((ulong)result))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Parse(string value, out Origin result)
        {
            if (!ParserFunctions.TryParseEnum<Origin>(value, out result, Origin.None))
            {
                result = (Origin)ResourceUtils.HashString64(value);

                if (!Localization.HasLocalizationString("Gameplay/Excel/Buffs/BuffOrigins:" + Simulator.GetEnumSimpleName(typeof(Origin), (ulong)result)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
