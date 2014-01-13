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
    public abstract class StringToList<T>
    {
        protected abstract bool PrivateConvert(string value, out T result);

        public delegate bool OnConvert(string value, out T result);

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

                    T result;
                    if (!convert(val, out result)) continue;

                    results.Add(result);
                }
            }

            return results;
        }
    }
}
