using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Households;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public abstract class MarriageScenario : MarriageBaseScenario
    {
        public MarriageScenario(SimDescription sim, SimDescription target)
            : base(sim, target)
        { }
        protected MarriageScenario(MarriageScenario scenario)
            : base (scenario)
        { }

        public static event UpdateDelegate OnGatheringScenario;

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (!Romances.BumpToHighestState(this, Sim, Target))
            {
                IncStat("Bump Failure");
                return false;
            }
            else
            {
                SetElapsedTime<DayOfLastPartnerOption>(Sim);
                SetElapsedTime<DayOfLastPartnerOption>(Target);

                SetElapsedTime<DayOfLastRomanceOption>(Sim);
                SetElapsedTime<DayOfLastRomanceOption>(Target);

                Romances.HandleMarriageName(Sim, Target, false);

                if (OnGatheringScenario != null)
                {
                    OnGatheringScenario(this, frame);
                }
                return true;
            }
        }

        public class RenameChildrenOption : BooleanManagerOptionItem<ManagerRomance>
        {
            public RenameChildrenOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "RenameChildrenOnMarriage";
            }
        }

        public class RenameOnlyMutualOption : BooleanManagerOptionItem<ManagerRomance>
        {
            public RenameOnlyMutualOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "RenameOnlyMutualOnMarriage";
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<RenameChildrenOption, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class AllowTeenOnlyOnPregnancyOption : BooleanManagerOptionItem<ManagerRomance>
        {
            public AllowTeenOnlyOnPregnancyOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowOnlyTeenPregnancy";
            }
        }
    }
}
