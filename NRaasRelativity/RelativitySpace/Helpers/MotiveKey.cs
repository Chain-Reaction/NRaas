using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Helpers
{
    [Persistable]
    public class MotiveKey
    {
        public readonly CASAgeGenderFlags mAgeSpecies;

        public readonly OccultTypes mOccult;

        public readonly CommodityKind mKind;

        public MotiveKey()
        { }
        public MotiveKey(CommodityKind kind)
            : this(CASAgeGenderFlags.None, OccultTypes.None, kind)
        { }
        public MotiveKey(SimDescription sim, CommodityKind kind)
            : this(sim.Age | sim.Species, FirstOccult(sim), kind)
        { }
        public MotiveKey(CASAgeGenderFlags ageSpecies, OccultTypes occult, CommodityKind kind)
        {
            mAgeSpecies = ageSpecies;
            mOccult = occult;
            mKind = kind;
        }

        protected static OccultTypes FirstOccult(SimDescription sim)
        {
            List<OccultTypes> types = OccultTypeHelper.CreateList(sim);
            if (types.Count == 0) return OccultTypes.None;

            return types[0];
        }

        public float GetMotiveFactor(Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> deltas, out bool exactMatch)
        {
            exactMatch = true;

            Dictionary<OccultTypes, Dictionary<CommodityKind, float>> occults;

            if (!deltas.TryGetValue(mAgeSpecies, out occults))
            {
                exactMatch = false;

                if (!deltas.TryGetValue(CASAgeGenderFlags.None, out occults))
                {
                    return 1f;
                }
            }

            Dictionary<CommodityKind, float> commodities;
            if (!occults.TryGetValue(mOccult, out commodities))
            {
                exactMatch = false;

                if ((mOccult == OccultTypes.None) || (!occults.TryGetValue(OccultTypes.None, out commodities)))
                {
                    return 1f;
                }
            }

            float multiple = 1f;
            if (commodities.TryGetValue(mKind, out multiple))
            {
                return multiple;
            }
            else
            {
                exactMatch = false;
                return 1f;
            }
        }

        public void SetMotiveFactor(Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> deltas, float value)
        {
            Dictionary<OccultTypes, Dictionary<CommodityKind, float>> occults;
            if (!deltas.TryGetValue(mAgeSpecies, out occults))
            {
                occults = new Dictionary<OccultTypes, Dictionary<CommodityKind, float>>();
                deltas.Add(mAgeSpecies, occults);
            }

            Dictionary<CommodityKind, float> commodities;
            if (!occults.TryGetValue(mOccult, out commodities))
            {
                commodities = new Dictionary<CommodityKind, float>();
                occults.Add(mOccult, commodities);
            }

            commodities.Remove(mKind);

            if (value != 0)
            {
                commodities.Add(mKind, value);
            }
        }

        public string GetLocalizedName()
        {
            string result = null;

            if (mOccult != OccultTypes.None)
            {
                result += OccultTypeHelper.GetLocalizedName(mOccult) + " ";
            }

            if (mAgeSpecies != CASAgeGenderFlags.None)
            {
                CASAgeGenderFlags age = mAgeSpecies & CASAgeGenderFlags.AgeMask; ;
                CASAgeGenderFlags species = mAgeSpecies & CASAgeGenderFlags.SpeciesMask;

                result += Common.LocalizeEAString("UI/Feedback/CAS:" + age) + " ";
                result += Common.Localize("Species:" + species) + " ";
            }

            result += CommoditiesEx.GetMotiveLocalizedName(mKind);

            return result;
        }

        public override string ToString()
        {
            return mAgeSpecies + ":" + mOccult + ":" + mKind;
        }
    }
}
