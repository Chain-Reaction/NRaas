using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class CelebrityPromotionScenario : Careers.PromotionBaseScenario<int>
    {
        static PromotionDataSet sDataSet = new PromotionDataSet();

        public CelebrityPromotionScenario (SimDescription sim)
            : base(sim)
        { }
        protected CelebrityPromotionScenario(CelebrityPromotionScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CelebrityPromotion";
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Friends.AllowCelebrity(this, sim))
            {
                IncStat("Celebrity Denied");
                return false;
            }
            else
            {
                Career career = sim.Occupation as Career;
                if ((career != null) && (career.HighestCareerLevelAchieved != null) && (career.Level < career.HighestCareerLevelAchieved.Level))
                {
                    IncStat("Too Low");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected static int Populate(XmlDbRow row)
        {
            return row.GetInt("PointsGiven");
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            int points = sDataSet.GetDataForLevel(Sim);
            AddStat("Points", points);

            Friends.AccumulateCelebrity(Sim, points);
            return true;
        }

        public override Scenario Clone()
        {
            return new CelebrityPromotionScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerFriendship>, ManagerFriendship.ICelebrityOption
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerFriendship main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    StoryProgressionSpace.Scenarios.Careers.PromotedScenario.OnCelebrityScenario += OnPerform;

                    sDataSet.Parse("NRaas.StoryProgression.CelebrityPromotion", Populate);
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "CelebrityPromotion";
            }

            protected static void OnPerform(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new CelebrityPromotionScenario(s.Sim), ScenarioResult.Failure);
            }
        }
    }
}
