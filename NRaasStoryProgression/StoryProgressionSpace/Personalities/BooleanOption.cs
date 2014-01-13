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
    public class BooleanOption : SimPersonality.BooleanOption
    {
        string mName = null;

        public BooleanOption()
            : base(false)
        { }

        public override string GetTitlePrefix()
        {
            return mName;
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

            SetValue (row.GetBool("Default"));

            return true;
        }

        public class ValueTest : SimScenarioFilter.IValueTestOption
        {
            BooleanOption mOption;

            bool mMatch = true;

            public ValueTest(BooleanOption option, bool match)
            {
                mOption = option;
                mMatch = match;
            }

            public bool Satisfies()
            {
                return (mOption.Value == mMatch);
            }

            public override string ToString()
            {
                return (mOption.Name + ", Match: " + mMatch);
            }
        }

        public class OptionValue : IUpdateManagerOption
        {
            BooleanOption mOption;

            bool mValue;

            public bool Parse(XmlDbRow row, string name, StoryProgressionObject manager, IUpdateManager updater, ref string error)
            {
                string value = row.GetString(name);

                if (string.IsNullOrEmpty(value))
                {
                    error = "BooleanOption " + name + " missing";
                    return false;
                }

                if (!bool.TryParse(value, out mValue))
                {
                    mOption = manager.GetOption<BooleanOption>(value);

                    if (mOption == null)
                    {
                        error = "BooleanOption" + value + " invalid";
                        return false;
                    }
                }

                updater.AddUpdater(this);
                return true;
            }

            public void UpdateManager(StoryProgressionObject manager)
            {
                if (mOption == null) return;

                mOption = manager.GetOption<BooleanOption>(mOption.GetTitlePrefix());
            }

            public bool Value
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

