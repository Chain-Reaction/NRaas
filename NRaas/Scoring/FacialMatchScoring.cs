using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public class FacialMatchScoring : SimScaledScoring<DualSimScoringParameters>, IScoringCache
    {
        public static bool sDisabled = false;

        static Dictionary<ulong, Dictionary<BlendUnit, float>> sValues = new Dictionary<ulong, Dictionary<BlendUnit, float>>();

        public FacialMatchScoring()
        { }

        public override bool UnloadCaches(bool final)
        {
            if (final)
            {
                sValues.Clear();
            }

            return base.UnloadCaches(final);
        }

        protected Dictionary<BlendUnit, float> GetBlendAmounts(SimDescription sim)
        {
            Dictionary<BlendUnit, float> blends;
            if (!sValues.TryGetValue(sim.SimDescriptionId, out blends))
            {
                using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(sim, CASParts.sPrimary))
                {
                    if (builder.OutfitValid)
                    {
                        blends = new Dictionary<BlendUnit, float>();
                        sValues.Add(sim.SimDescriptionId, blends);

                        foreach (BlendUnit blend in FacialBlends.BlendUnits)
                        {
                            FacialBlendData data = new FacialBlendData(blend);

                            blends.Add(blend, FacialBlends.GetValue(builder.Builder, data));
                        }

                        builder.Invalidate();
                    }
                }
            }

            return blends;
        }

        public override int Score(DualSimScoringParameters parameters)
        {
            if (sDisabled) return 0;

            return base.Score(parameters);
        }

        protected override int GetScaler(DualSimScoringParameters parameters)
        {
            Dictionary<BlendUnit, float> actorValues = GetBlendAmounts(parameters.Actor);
            if (actorValues == null) return 0;

            Dictionary<BlendUnit, float> otherValues = GetBlendAmounts(parameters.Other);
            if (otherValues == null) return 0;

            float total = 0;

            foreach (BlendUnit blend in FacialBlends.BlendUnits)
            {
                float actorValue;
                if (!actorValues.TryGetValue(blend, out actorValue))
                {
                    actorValue = 0;
                }

                float otherValue;
                if (!otherValues.TryGetValue(blend, out otherValue))
                {
                    otherValue = 0;
                }

                if ((actorValue != 0) && (otherValue != 0))
                {
                    float value = Math.Abs(actorValue - otherValue);

                    if (value > 1)
                    {
                        total -= (value * 2);
                    }
                    else
                    {
                        total += value;
                    }
                }
            }

            return (int)(total * 100);
        }
    }
}

