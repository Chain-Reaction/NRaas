using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Sims
{
    public class AgeUpScenario : AgeUpBaseScenario
    {
        public AgeUpScenario()
        { }
        protected AgeUpScenario(AgeUpScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type == PrefixType.Story) return null;

            return "AgeUp";
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            ManagerSim.ForceRecount();
            return true;
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (Sim.Species == CASAgeGenderFlags.Human)
            {
                text = AgingManager.LocalizeString(Sim.IsFemale, "AgeTo" + Sim.Age, new object[] { Sim });
            }
            else if (Sim.IsADogSpecies)
            {
                text = Common.Localize("AgeUpDog:" + Sim.Age, Sim.IsFemale, new object[] { Sim });
            }
            else
            {
                text = Common.Localize("AgeUp" + Sim.Species + ":" + Sim.Age, Sim.IsFemale, new object[] { Sim });
            }

            if (string.IsNullOrEmpty(text)) return null;

            if (parameters == null)
            {
                parameters = new object[] { Sim };
            }

            if (extended == null)
            {
                extended = new string[] { Common.LocalizeEAString("UI/Feedback/CAS:" + Sim.Age) };
            }
            
            logging |= ManagerStory.StoryLogging.Full;

            return base.PrintFormattedStory(manager, text, summaryKey, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new AgeUpScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerSim, AgeUpScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SimAgeUp";
            }
        }
    }
}
