using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CommonSpace.Options
{
    public interface IScorableOption
    {
        string OptionName { get; }
        string OptionValue { get; } 
        bool CanBeRandomValue { get; set; }
        int OptionHitValue { get; set; }
        int OptionMissValue { get; set; }
        int MinHitValue { get; set; }
        int MaxHitValue { get; set; }
        int MinMissValue { get; set; }
        int MaxMissValue { get; set; }
        Dictionary<int, float> ChanceAtOptionLevel { get; set; }
        int GetScoreValue(IMiniSimDescription me, IMiniSimDescription actor, bool satisfies, int divisior);
        OptionScoreData OptionScoreData { get; set; }
    }

    [Persistable]
    public class OptionScoreData
    {
        public int mOptionHitValue;
        public int mOptionMissValue;

        public int mMinHitValue;
        public int mMaxHitValue;

        public int mMinMissValue;
        public int mMaxMissValue;

        public bool mCanBeRandomValue;

        public Dictionary<int, float> mChanceAtOptionLevel = new Dictionary<int, float>();

        public OptionScoreData()
        {
        }

        public bool CanBeRandomValue
        {
            get { return mCanBeRandomValue; }
            set { mCanBeRandomValue = value; }
        }

        public int OptionHitValue
        {
            get { return mOptionHitValue; }
            set { mOptionHitValue = value; }
        }

        public int OptionMissValue
        {
            get { return mOptionMissValue; }
            set { mOptionMissValue = value; }
        }

        public int MinHitValue
        {
            get { return mMinHitValue; }
            set { mMinHitValue = value; }
        }

        public int MaxHitValue
        {
            get { return mMaxHitValue; }
            set { mMaxHitValue = value; }
        }

        public int MinMissValue
        {
            get { return mMinMissValue; }
            set { mMinMissValue = value; }
        }

        public int MaxMissValue
        {
            get { return mMaxMissValue; }
            set { mMaxMissValue = value; }
        }

        public Dictionary<int, float> ChanceAtOptionLevel
        {
            get { return mChanceAtOptionLevel; }
            set { mChanceAtOptionLevel = value; }
        }

        public float GetChanceForOption(int option)
        {
            float chance = 0f;
            mChanceAtOptionLevel.TryGetValue(option, out chance);

            return chance;
        }
    }
}