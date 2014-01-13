using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class IntegerOption : SimPersonality.IntegerOption, SimPersonality.IResetOnLeaderChangeOption
    {
        string mName = null;

        bool mResetOnLeaderChange = false;

        public IntegerOption()
            : base(0)
        { }

        public override string GetTitlePrefix()
        {
            return mName;
        }

        public void ResetOnLeaderChange()
        {
            if (mResetOnLeaderChange)
            {
                SetValue(0);
            }
        }

        public override bool Parse(XmlDbRow row, SimPersonality personality, ref string error)
        {
            if (!base.Parse(row, personality, ref error)) return false;

            if (!row.Exists("Name"))
            {
                error = "Name missing";
                return false;
            }
            else if (!row.Exists("Default"))
            {
                error = "Default missing";
                return false;
            }

            mName = row.GetString("Name");

            SetValue (row.GetInt("Default"));

            mResetOnLeaderChange = row.GetBool("ResetOnLeaderChange");

            return true;
        }

        public class ValueTest : SimScenarioFilter.IValueTestOption
        {
            IntegerOption mOption;

            readonly int mMinimum = int.MinValue;
            readonly int mMaximum = int.MaxValue;

            public ValueTest(IntegerOption option, int min, int max)
            {
                mOption = option;
                mMinimum = min;
                mMaximum = max;
            }

            public bool Satisfies()
            {
                if (mOption.Value < mMinimum) return false;

                if (mOption.Value > mMaximum) return false;

                return true;
            }

            public override string ToString()
            {
                return (mOption.Name + ", Min: " + mMinimum + ", Max: " + mMaximum);
            }
        }

        public class ResetValue : SimPersonality.IAccumulatorValue
        {
            IntegerOption mOption;

            public ResetValue()
            { }
            public ResetValue(IntegerOption option)
            {
                mOption = option;
            }

            public override string ToString()
            {
                return mOption + "=Reset";
            }

            public void ApplyAccumulator()
            {
                mOption.SetValue(0);
            }
        }

        public class OptionValue : IUpdateManagerOption, SimPersonality.IAccumulatorValue
        {
            IntegerOption mOption;

            int mValue;

            public OptionValue()
            { }
            public OptionValue(int value)
            {
                mValue = value;
            }
            public OptionValue(IntegerOption option, int value)
            {
                mOption = option;
                mValue = value;
            }

            public override string ToString()
            {
                return mOption + "=" + mValue + " (" + Value + ")";
            }

            public bool Parse(XmlDbRow row, string name, StoryProgressionObject manager, IUpdateManager updater, ref string error)
            {
                string value = row.GetString(name);

                if (string.IsNullOrEmpty(value))
                {
                    error = "IntegerOption " + name + " missing";
                    return false;
                }

                if (!int.TryParse(value, out mValue))
                {
                    mOption = manager.GetOption<IntegerOption>(value);
                    if (mOption == null)
                    {
                        error = "IntegerOption " + value + " invalid";
                        return false;
                    }
                }

                updater.AddUpdater(this);
                return true;
            }

            public void UpdateManager(StoryProgressionObject manager)
            {
                if (mOption == null) return;

                mOption = manager.GetOption<IntegerOption>(mOption.GetTitlePrefix());
            }

            public void ApplyAccumulator()
            {
                mOption.SetValue(mOption.Value + mValue);
            }

            public int Value
            {
                get
                {
                    if (mOption == null)
                    {
                        return mValue;
                    }
                    else
                    {
                        return mOption.Value;
                    }
                }
            }
        }
    }
}

