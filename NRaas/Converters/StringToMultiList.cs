using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Converters
{
    public abstract class StringToMultiList<T>
    {
        protected abstract List<T> PrivateConvert(string value);

        public delegate List<T> OnConvert(string value);

        public List<T> Convert(string value)
        {
            return StaticConvert(value, PrivateConvert);
        }

        public static List<T> StaticConvert(string value, OnConvert convert)
        {
            List<T> results = new List<T>();

            if (!string.IsNullOrEmpty(value))
            {
                foreach (string val in value.Split(','))
                {
                    if (string.IsNullOrEmpty(val)) continue;

                    List<T> values = convert(val);
                    if (values == null) return null;

                    results.AddRange(values);
                }
            }

            return results;
        }
    }
}
