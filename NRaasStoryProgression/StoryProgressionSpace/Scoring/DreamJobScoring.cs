using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Scoring
{
    public class DreamJobScoring : HitMissScoring<SimDescription, SimScoringParameters, SimScoringParameters>
    {
        OccupationNames mOccupation;

        public DreamJobScoring()
        { }
        public DreamJobScoring(int hit, int miss, OccupationNames occupation)
            : base(hit, miss)
        {
            mOccupation = occupation;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            if (!row.Exists("Occupation"))
            {
                error = "Occupation missing";
                return false;
            }
            else if (!ParserFunctions.TryParseEnum<OccupationNames>(row.GetString("Occupation"), out mOccupation, OccupationNames.Undefined))
            {
                error = "Unknown Occupation " + row.GetString("Occupation");
                return false;
            }

            return base.Parse(row, ref error);
        }

        public override bool IsHit(SimScoringParameters parameters)
        {
            List<DreamJob> dreamJobs = Managers.ManagerCareer.GetDreamJob(parameters.Actor);

            if (DreamJob.Contains(dreamJobs, mOccupation)) return true;

            if (dreamJobs.Count == 0)
            {
                return false;
            }
            else
            {
                return (mOccupation == OccupationNames.Any);
            }
        }
    }
}

