using NRaas.CommonSpace.Booters;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class PersonalityLookup : BooterHelper.ByRowListingBooter
    {
        private static Dictionary<string, SimPersonality> sLookup = new Dictionary<string, SimPersonality>();

        int mIndex = 0;

        public PersonalityLookup(string reference)
            : base("Personality", "PersonalityFile", "File", reference)
        { }

        public static List<SimPersonality> Personalities
        {
            get { return new List<SimPersonality> (sLookup.Values); }
        }

        public static SimPersonality GetPersonality(string name)
        {
            SimPersonality personality;
            if (sLookup.TryGetValue(name.ToLower(), out personality))
            {
                return personality;
            }
            else
            {
                return null;
            }
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            BooterHelper.DataBootFile dataFile = file as BooterHelper.DataBootFile;
            if (dataFile == null) return;

            mIndex++;

            string personalityName = row.GetString("Name");
            if (string.IsNullOrEmpty(personalityName))
            {
                BooterLogger.AddError(file + " : Method " + mIndex + " Unnamed");
                return;
            }

            BooterLogger.AddTrace("Found " + personalityName);

            if (GetPersonality(personalityName) != null)
            {
                BooterLogger.AddError(personalityName + " Name already in use");
                return;
            }

            Type classType = row.GetClassType("FullClassName");
            if (classType == null) 
            {
                BooterLogger.AddError(personalityName + " No Class");
                return;
            }

            SimPersonality personality = null;
            try
            {
                personality = classType.GetConstructor(new Type[0]).Invoke(new object[0]) as SimPersonality;
            }
            catch
            { }

            if (personality == null)
            {
                BooterLogger.AddError(personalityName + ": Constructor Fail " + row.GetString("FullClassName"));
            }
            else
            {
                XmlDbTable optionTable = dataFile.GetTable(personalityName);
                if (personality.Parse(row, optionTable))
                {
                    sLookup.Add(personalityName.ToLower(), personality);
                }
                else
                {
                    BooterLogger.AddError(personalityName + ": Parsing Fail");
                }
            }
        }
    }
}