using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.ScoringMethods;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class CareerTraitScoring : TraitBaseScoring<CareerScoringParameters>
    {
        List<OccupationNames> mOccupations = new List<OccupationNames>();

        public CareerTraitScoring()
        { }

        public override bool IsCombinable
        {
            get { return false; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!base.Parse(row, ref error)) return false;

            if (row.Exists("Occupations"))
            {
                ToOccupationNames converter = new ToOccupationNames();

                mOccupations = converter.Convert(row.GetString("Occupations"));
                if (mOccupations == null)
                {
                    error = converter.mError;
                    return false;
                }
            }

            if (row.Exists("CustomOccupations"))
            {
                mOccupations = new ToCustomOccupation().Convert(row.GetString("CustomOccupations"));
                if (mOccupations == null)
                {
                    return false;
                }
            }

            if (mOccupations.Count == 0)
            {
                error = "No Occupations";
                return false;
            }

            return true;
        }

        public override int Score(CareerScoringParameters parameters)
        {
            int result = base.Score(parameters);

            foreach (OccupationNames occupation in mOccupations)
            {
                parameters.Add(occupation, result);
            }

            return 0;
        }

        public override string ToString()
        {
            string occupations = null;

            foreach (OccupationNames occupation in mOccupations)
            {
                occupations += "," + occupation;
            }

            return base.ToString() + occupations;
        }

        public class ToOccupationNames : StringToList<OccupationNames>
        {
            public string mError;

            protected override bool PrivateConvert(string value, out OccupationNames result)
            {
                if (ParserFunctions.TryParseEnum<OccupationNames>(value.Trim(), out result, OccupationNames.Any)) return true;

                mError = "Unknown Occupation " + value;
                return false;
            }
        }

        public class ToCustomOccupation : StringToList<OccupationNames>
        {
            protected override bool PrivateConvert(string value, out OccupationNames result)
            {
                result = unchecked((OccupationNames)ResourceUtils.HashString64(value.Trim()));
                return true;
            }
        }
    }
}

