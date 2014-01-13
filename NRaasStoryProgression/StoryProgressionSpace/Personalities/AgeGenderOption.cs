using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class AgeGenderOption : SimPersonality.BooleanOption
    {
        string mName = null;

        CASAgeGenderFlags mAge = CASAgeGenderFlags.None;
        CASAgeGenderFlags mGender = CASAgeGenderFlags.None;
        
        List<CASAgeGenderFlags> mSpecies = new List<CASAgeGenderFlags>();

        public AgeGenderOption()
            : base(true)
        { }

        public override string GetTitlePrefix()
        {
            return mName;
        }

        public override string ToString()
        {
            string result = "Name=" + mName + Common.NewLine + "Age=" + mAge + Common.NewLine + "Gender=" + mGender;

            result += Common.NewLine + "Species=";
            foreach (CASAgeGenderFlags species in mSpecies)
            {
                result += mSpecies + ",";
            }

            return result;
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
            else
            {
                CASAgeGenderFlags ageGender;
                if (!ParserFunctions.TryParseEnum<CASAgeGenderFlags>(row.GetString("AgeGender"), out ageGender, CASAgeGenderFlags.None))
                {
                    error = "Unknown AgeGender " + row.GetString("AgeGender");
                    return false;
                }

                mAge = ageGender & CASAgeGenderFlags.AgeMask;

                if (mAge == CASAgeGenderFlags.None)
                {
                    mAge = CASAgeGenderFlags.AgeMask;
                }

                mGender = ageGender & CASAgeGenderFlags.GenderMask;

                if (mGender == CASAgeGenderFlags.None)
                {
                    mGender = CASAgeGenderFlags.GenderMask;
                }

                StringToSpeciesList converter = new StringToSpeciesList();
                mSpecies = converter.Convert(row.GetString("Species"));
                if (mSpecies == null)
                {
                    error = converter.mError;
                    return false;
                }

                if (mSpecies.Count == 0)
                {
                    mSpecies.Add(CASAgeGenderFlags.Human);
                }
            }

            mName = row.GetString("Name");

            SetValue (row.GetBool("Default"));

            return true;
        }

        public bool Satisfies(CASAgeGenderFlags age, CASAgeGenderFlags gender, CASAgeGenderFlags species)
        {
            if (!Value)
            {
                if (((age & mAge) == age) && ((gender & mGender) == gender) && ((mSpecies.Contains(species))))
                {
                    return false;
                }
            }

            return true;
        }

        public class OptionValue : IUpdateManagerOption
        {
            AgeGenderOption mOption;

            bool mValue;

            public bool Parse(XmlDbRow row, string name, StoryProgressionObject manager, IUpdateManager updater, ref string error)
            {
                string value = row.GetString(name);

                if (string.IsNullOrEmpty(value))
                {
                    error = "AgeGenderOption " + name + " missing";
                    return false;
                }

                if (!bool.TryParse(value, out mValue))
                {
                    mOption = manager.GetOption<AgeGenderOption>(value);
                    if (mOption == null)
                    {
                        error = "AgeGenderOption " + value + " missing";
                        return false;
                    }
                }

                updater.AddUpdater(this);
                return true;
            }

            public void UpdateManager(StoryProgressionObject manager)
            {
                if (mOption == null) return;

                mOption = manager.GetOption<AgeGenderOption>(mOption.GetTitlePrefix());
            }

            public bool GetValue
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

