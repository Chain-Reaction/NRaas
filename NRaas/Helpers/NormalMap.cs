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
    public class NormalMap
    {
        static FacialBlendData sBreastSizeBlendData = null;

        public static FacialBlendData BustSlider
        {
            get
            {
                if (sBreastSizeBlendData == null)
                {
                    sBreastSizeBlendData = new FacialBlendData(new BlendUnit(new ResourceKey(0x611c8950a00fb041L, 0xb52f5055, 0x0)));
                }

                return sBreastSizeBlendData;
            }
        }

        public static void ApplyBustValue(SimBuilder builder, float value)
        {
            if (value > 0)
            {
                builder.SetFacialBlend(BustSlider.mBlend1.GetKey(), value);
                builder.SetFacialBlend(BustSlider.mBlend2.GetKey(), 0);
            }
            else
            {
                builder.SetFacialBlend(BustSlider.mBlend1.GetKey(), 0);
                builder.SetFacialBlend(BustSlider.mBlend2.GetKey(), -value);
            }

            if (value < 0f)
            {
                value = 0f;
            }
            else if (value < 0.5f)
            {
                value *= 2f;
            }
            else
            {
                value = 1.5f - value;
            }

            ApplyValue(builder, 1, value);
        }
        public static void ApplyMuscleValue(SimBuilder builder, float value)
        {
            ApplyValue(builder, 0, value);
        }

        private static void ApplyValue(SimBuilder builder, uint index, float value)
        {
            builder.mSecondaryNormalMapWeights[index] = value;
            builder.mUtils.SimBuilderSetSecondaryNormalMapWeight(builder.mHandle, index, builder.mSecondaryNormalMapWeights[index]);
        }
    }
}

