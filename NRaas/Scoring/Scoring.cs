using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Scoring
{
    public interface IScoring
    { }

    public interface IScoring<T, SP>
        where T : class
        where SP : ListedScoringParameters<T>
    {
        int Score(SP parameters);

        bool Parse(XmlDbRow row, ref string error);

        void Collect(T obj);

        bool UnloadCaches(bool final);

        bool IsConsumed
        {
            get;
        }

        void SetToConsumed();

        bool Cachable
        {
            get;
        }

        void Validate();
    }

    public abstract class Scoring<T,SP> : IScoring, IScoring<T, SP>
        where T: class
        where SP : ListedScoringParameters<T>
    {
        bool mConsumed;

        public int BaseScore(SP parameters)
        {
            return Score(parameters);
        }

        public abstract int Score(SP parameters);

        public abstract bool Parse(XmlDbRow row, ref string error);

        public virtual void Validate()
        { }

        public virtual void Collect(T obj)
        { }

        public virtual bool UnloadCaches(bool final)
        {
            return false;
        }

        public bool IsConsumed
        {
            get { return mConsumed; }         
        }

        public void SetToConsumed()
        {
            mConsumed = true;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        public virtual bool Cachable
        {
            get { return true; }
        }
    }
}

