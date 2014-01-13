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
    public class MotiveKeyValue : IPersistence
    {
        public MotiveKey mKey;

        public float mValue;

        public MotiveKeyValue()
        { }
        public MotiveKeyValue(MotiveKey key, float value)
        {
            mKey = key;
            mValue = value;
        }

        public void Import(Persistence.Lookup settings)
        {
            mKey = new MotiveKey(
                settings.GetEnum<CASAgeGenderFlags>("AgeSpecies", CASAgeGenderFlags.None),
                settings.GetEnum<OccultTypes>("Occult", OccultTypes.None),
                settings.GetEnum<CommodityKind>("Commodity", CommodityKind.None)
            );
            mValue = settings.GetFloat("Value", 1);
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("AgeSpecies", mKey.mAgeSpecies.ToString());
            settings.Add("Occult", mKey.mOccult.ToString());
            settings.Add("Commodity", mKey.mKind.ToString());
            settings.Add("Value", mValue);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public static List<MotiveKeyValue> ConvertToList(Dictionary<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> deltas)
        {
            List<MotiveKeyValue> results = new List<MotiveKeyValue>();

            foreach (KeyValuePair<CASAgeGenderFlags, Dictionary<OccultTypes, Dictionary<CommodityKind, float>>> ageSpecies in deltas)
            {
                foreach (KeyValuePair<OccultTypes, Dictionary<CommodityKind, float>> occults in ageSpecies.Value)
                {
                    foreach (KeyValuePair<CommodityKind, float> commodities in occults.Value)
                    {
                        results.Add(new MotiveKeyValue(new MotiveKey(ageSpecies.Key, occults.Key, commodities.Key), commodities.Value));
                    }
                }
            }

            return results;
        }
    }
}
