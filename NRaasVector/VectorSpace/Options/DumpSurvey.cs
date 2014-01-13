using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.VectorSpace.Options
{
    public class DumpSurvey : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return Vector.Settings.Debugging;
        }

        public override string GetTitlePrefix()
        {
            return "DumpSurvey";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder builder = new Common.StringBuilder();

            Dictionary<ulong, List<SimDescription>> allSims = SimListing.AllSims<SimDescription>(null, true);

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                Dictionary<string, List<SimDescription>> stages = new Dictionary<string, List<SimDescription>>();

                foreach (KeyValuePair<ulong,List<DiseaseVector>> sims in Vector.Settings.AllVectors)
                {
                    List<SimDescription> choices;
                    if (!allSims.TryGetValue(sims.Key, out choices)) continue;

                    SimDescription sim = choices[0];

                    foreach (DiseaseVector disease in sims.Value)
                    {
                        if (disease.Guid != vector.Guid) continue;

                        string key = disease.StageName + " " + disease.Strain;

                        List<SimDescription> value;
                        if (!stages.TryGetValue(key, out value))
                        {
                            value = new List<SimDescription>();
                            stages.Add(key, value);
                        }

                        value.Add(sim);
                    }
                }

                string result = Common.NewLine + vector.Guid;

                foreach (KeyValuePair<string, List<SimDescription>> stage in stages)
                {
                    result += Common.NewLine + " " + stage.Key + " : " + stage.Value.Count;

                    foreach (SimDescription sim in stage.Value)
                    {
                        result += Common.NewLine + "  " + sim.FullName;
                    }
                }

                builder.Append(result);
            }

            Common.DebugWriteLog(builder);

            return OptionResult.SuccessRetain;
        }
    }
}
