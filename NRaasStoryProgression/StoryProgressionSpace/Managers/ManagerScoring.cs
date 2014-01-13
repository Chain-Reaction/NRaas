using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerScoring : Manager
    {
        public ManagerScoring(Main manager)
            : base (manager)
        {
            ScoringLookup.Stats = this;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Scoring";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerScoring>(this).Perform(initial);
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            if (initialPass)
            {
                ScoringLookup.Validate();
            }

            if (fullUpdate)
            {
                ScoringLookup.UnloadCaches(false);
            }

            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public override void Shutdown()
        {
            ScoringLookup.UnloadCaches(true);

            base.Shutdown();
        }

        protected class DebugOption : DebugLevelOption<ManagerScoring>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class DumpStatsOption : DumpStatsBaseOption<ManagerScoring>
        {
            public DumpStatsOption()
                : base(5)
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerScoring>
        {
            public DumpScoringOption()
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerScoring>
        {
            public TicksPassedOption()
            { }
        }

        protected class SpeedOptionV2 : SpeedBaseOption<ManagerScoring>
        {
            public SpeedOptionV2()
                : base(400, false)
            { }
        }

        public class FacialMatchingOption : BooleanManagerOptionItem<ManagerScoring>, IDebuggingOption
        {
            public FacialMatchingOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "FacialMatching";
            }

            public override bool Install(ManagerScoring main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                FacialMatchScoring.sDisabled = !Value;

                return true;
            }

            public override bool Persist()
            {
                FacialMatchScoring.sDisabled = !Value;

                return base.Persist();
            }
        }
    }
}

