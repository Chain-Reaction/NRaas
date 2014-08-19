using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class FacialBlends
    {
        static List<BlendUnit> sBlendUnits = null;

        static List<FacialBlend> sFaceBlends = null;
        static List<FacialBlend> sFurBlends = null;

        public static List<BlendUnit> BlendUnits
        {
            get
            {
                if (sBlendUnits == null)
                {
                    sBlendUnits = new List<BlendUnit>();

                    KeySearch search = new KeySearch(0xb52f5055);
                    foreach (ResourceKey key in search)
                    {
                        sBlendUnits.Add(new BlendUnit(key));
                    }

                    search.Reset();

                    sBlendUnits.Sort(new BlendUnitComparer());
                }

                return sBlendUnits;
            }
        }

        public static List<FacialBlend> FurBlends
        {
            get
            {
                if (sFurBlends == null)
                {
                    sFurBlends = new List<FacialBlend>();

                    KeySearch search = new KeySearch(0x93d84841);
                    foreach (ResourceKey key in search)
                    {
                        sFurBlends.Add(new FacialBlend(key));
                    }

                    search.Reset();
                }

                return sFurBlends;
            }
        }

        public static List<FacialBlend> FaceBlends
        {
            get
            {
                if (sFaceBlends == null)
                {
                    sFaceBlends = new List<FacialBlend>();

                    KeySearch search = new KeySearch(0x358b08a);
                    foreach (ResourceKey key in search)
                    {
                        sFaceBlends.Add(new FacialBlend(key));
                    }

                    search.Reset();
                }

                return sFaceBlends;
            }
        }

        public static bool CopyGenetics(SimDescriptionCore source, SimDescriptionCore destination, bool onlyNonZero, bool onlySliders)
        {
            SimOutfit sourceOutfit = CASParts.GetOutfit(source, CASParts.sPrimary, false);
            if (sourceOutfit == null) return false;

            SimOutfit sourceWerewolfOutfit = CASParts.GetOutfit(source, new CASParts.Key(OutfitCategories.Supernatural, 0), false);

            SimDescription sourceDesc = source as SimDescription;
            SimDescription destDesc = destination as SimDescription;

            if ((!onlySliders) && (!SimTypes.IsSkinJob(sourceDesc)) && (!SimTypes.IsSkinJob(destDesc)))
            {
                destDesc.SkinToneKey = sourceDesc.SkinToneKey;
                destDesc.SkinToneIndex = sourceDesc.SkinToneIndex;
            }

            destDesc.SecondaryNormalMapWeights = sourceDesc.SecondaryNormalMapWeights.Clone() as float[];

            using (SimBuilder sourceBuilder = new SimBuilder())
            {
                OutfitUtils.SetOutfit(sourceBuilder, sourceOutfit, source);

                using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(destination, CASParts.sPrimary))
                {
                    if (!builder.OutfitValid) return false;

                    if ((!onlySliders) && (destDesc != null))
                    {
                        builder.Builder.SkinTone = destDesc.SkinToneKey;
                        builder.Builder.SkinToneIndex = destDesc.SkinToneIndex;
                    }

                    foreach (FacialBlend blend in FaceBlends)
                    {
                        float amount = GetBlendAmount(sourceBuilder, blend);

                        if (onlyNonZero)
                        {
                            if (amount == 0.0) continue;
                        }

                        SetBlendAmount(builder.Builder, blend, amount);
                    }

                    foreach (FacialBlend blend in FurBlends)
                    {
                        float amount = GetBlendAmount(sourceBuilder, blend);

                        if (onlyNonZero)
                        {
                            if (amount == 0.0) continue;
                        }

                        SetBlendAmount(builder.Builder, blend, amount);
                    }

                    if (!onlySliders)
                    {
                        builder.CopyGeneticParts(sourceOutfit);
                    }
                }

                sourceBuilder.Clear();

                if ((sourceWerewolfOutfit != null) && (destDesc.IsWerewolf))
                {
                    OutfitUtils.SetOutfit(sourceBuilder, sourceWerewolfOutfit, source);

                    using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(destination, new CASParts.Key(OutfitCategories.Supernatural, 0)))
                    {
                        if ((!onlySliders) && (destDesc != null))
                        {
                            builder.Builder.SkinTone = destDesc.SkinToneKey;
                            builder.Builder.SkinToneIndex = destDesc.SkinToneIndex;
                        }

                        foreach (FacialBlend blend in FaceBlends)
                        {
                            float amount = GetBlendAmount(sourceBuilder, blend);

                            if (onlyNonZero)
                            {
                                if (amount == 0.0) continue;
                            }

                            SetBlendAmount(builder.Builder, blend, amount);
                        }

                        builder.Components = CASLogic.sWerewolfPreserveComponents;
                    }
                }
            }

            return true;
        }

        public static float GetBlendAmount(SimBuilder builder, BaseBlend blend)
        {
            FacialBlend blend2 = blend as FacialBlend;
            if (blend2 != null)
            {
                return builder.GetFacialBlend(blend2.GetKey());
            }

            return 0;
        }

        public static float GetValue(SimBuilder builder, BlendUnit unit)
        {
            return GetValue(builder, new FacialBlendData(unit));
        }
        public static float GetValue(SimBuilder builder, FacialBlendData unit)
        {
            if (unit.mBlend2 != null)
            {
                float num2 = GetBlendAmount(builder, unit.mBlend2);
                if (num2 > 0f)
                {
                    return -num2;
                }
            }

            if (unit.mBlend1 != null)
            {
                return GetBlendAmount(builder, unit.mBlend1);
            }
            return 0;
        }

        public delegate int Logger(string text, int value);

        public static void RandomizeBlends(Logger log, SimDescription me, Vector2 rangeIfSet, bool addToExisting, Vector2 rangeIfUnset, bool propagate, bool disallowAlien)
        {
            using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(me, CASParts.sPrimary))
            {
                if (!builder.OutfitValid) return;

                foreach (BlendUnit blend in BlendUnits)
                {
                    switch (me.Species)
                    {
                        case CASAgeGenderFlags.Human:
                            switch (blend.Category)
                            {
                                case FacialBlendCategories.Dog:
                                case FacialBlendCategories.LittleDog:
                                case FacialBlendCategories.Cat:
                                case FacialBlendCategories.PetCommon:
                                case FacialBlendCategories.Horse:
                                    continue;
                            }
                            break;
                        case CASAgeGenderFlags.Cat:
                            if ((blend.Category != FacialBlendCategories.PetCommon) && (blend.Category != FacialBlendCategories.Cat)) continue;
                            break;
                        case CASAgeGenderFlags.LittleDog:
                            if ((blend.Category != FacialBlendCategories.PetCommon) && (blend.Category != FacialBlendCategories.LittleDog)) continue;
                            break;
                        case CASAgeGenderFlags.Dog:
                            if ((blend.Category != FacialBlendCategories.PetCommon) && (blend.Category != FacialBlendCategories.Dog)) continue;
                            break;
                        case CASAgeGenderFlags.Horse:
                            if ((blend.Category != FacialBlendCategories.PetCommon) && (blend.Category != FacialBlendCategories.Horse)) continue;
                            break;
                    }

                    if (disallowAlien)
                    {
                        ResourceKey alienEyeCKey = new ResourceKey(ResourceUtils.HashString64("EyeAlienCorrector"), 0x358b08a, 0);
                        if (blend.mKey == alienEyeCKey)
                        {
                            continue;
                        }

                        ResourceKey alienEyeKey = new ResourceKey(ResourceUtils.HashString64("EyeAlien"), 0x358b08a, 0);
                        if (blend.mKey == alienEyeKey)
                        {
                            continue;
                        }

                        ResourceKey alienEarKey = new ResourceKey(ResourceUtils.HashString64("EarPoint"), 0x358b08a, 0);
                        if (blend.mKey == alienEarKey)
                        {
                            continue;
                        }
                    }

                    float value = GetValue(builder.Builder, blend);

                    if (value == 0)
                    {
                        value = RandomUtil.GetFloat(rangeIfUnset.x, rangeIfUnset.y);

                        if (value > 1)
                        {
                            value = 1;
                        }
                        else if (value < -1)
                        {
                            value = -1;
                        }

                        if (log != null)
                        {
                            log("Unset Final 100s", (int)(value * 100));
                        }
                    }
                    else
                    {
                        if (!addToExisting)
                        {
                            value = 0;
                        }

                        float newValue = RandomUtil.GetFloat(rangeIfSet.x, rangeIfSet.y);

                        if (log != null)
                        {
                            log("Set Delta 100s", (int)(newValue * 100));
                        }

                        value += newValue;

                        if (value > 1)
                        {
                            value = 1;
                        }
                        else if (value < -1)
                        {
                            value = -1;
                        }

                        if (log != null)
                        {
                            log("Set Final 100s", (int)(value * 100));
                        }
                    }

                    SetValue(builder.Builder, blend, value);
                }
            }

            if (propagate)
            {
                new SavedOutfit.Cache(me).PropagateGenetics(me, CASParts.sPrimary);
            }
        }

        protected static void SetBlendAmount(SimBuilder builder, BaseBlend blend, float value)
        {
            FacialBlend blend2 = blend as FacialBlend;
            if (blend2 != null)
            {
                builder.SetFacialBlend(blend2.GetKey(), value);
            }
        }

        public static void SetValue(SimBuilder builder, BlendUnit unit, float value)
        {
            SetValue(builder, new FacialBlendData(unit), value);
        }
        public static void SetValue(SimBuilder builder, FacialBlendData unit, float value)
        {
            if (unit.mBlend2 == null)
            {
                if (value < 0)
                {
                    value = 0;
                }
                SetBlendAmount(builder, unit.mBlend1, value);
            }
            else if (value >= 0)
            {
                SetBlendAmount(builder, unit.mBlend1, value);
                SetBlendAmount(builder, unit.mBlend2, 0);
            }
            else
            {
                SetBlendAmount(builder, unit.mBlend1, 0);
                SetBlendAmount(builder, unit.mBlend2, -value);
            }
        }
    }
}

