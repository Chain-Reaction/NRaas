using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class PropagationScenarioHelper
    {
        PropagateBuffScenario mScenario;

        public PropagationScenarioHelper()
        { }

        public override string ToString()
        {
            string text = "-- PropagationScenarioHelper --";

            text += Common.NewLine + "Scenario" + Common.NewLine + mScenario;

            text += Common.NewLine + "-- End PropagationScenarioHelper --";

            return text;
        }

        public void Perform(Scenario scenario, ScenarioFrame frame, SimDescription actor, SimDescription target)
        {
            if (mScenario != null)
            {
                PropagateBuffScenario propagation = mScenario.Clone() as PropagateBuffScenario;

                propagation.SetActors(actor, target);

                scenario.IncStat("Propagate");

                scenario.Add(frame, propagation, ScenarioResult.Start);
            }
        }

        public bool Parse(XmlDbRow row, Origin origin, string prefix, ref string error)
        {
            if ((!string.IsNullOrEmpty(prefix)) && (!Parse(row, origin, null, ref error)))
            {
                return false;
            }

            if (row.Exists(prefix + "PropagateFullClassName"))
            {
                Type scenarioType = row.GetClassType(prefix + "PropagateFullClassName");
                if (scenarioType == null)
                {
                    error = prefix + "PropagateFullClassName invalid";
                    return false;
                }

                mScenario = null;
                try
                {
                    mScenario = scenarioType.GetConstructor(new Type[0]).Invoke(new object[0]) as PropagateBuffScenario;
                }
                catch
                { }

                if (mScenario == null)
                {
                    error = prefix + "PropagateFullClassName constructor fail";
                    return false;
                }

                mScenario.Origin = origin;

                if (!mScenario.Parse(row, prefix, true, ref error))
                {
                    return false;
                }

                if (!mScenario.PostParse(ref error))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
