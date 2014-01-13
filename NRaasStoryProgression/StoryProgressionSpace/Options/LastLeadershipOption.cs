using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Options
{
    public class LastLeadershipOption : GenericOptionBase.StringOption, IReadSimLevelOption, IWriteSimLevelOption, IDebuggingOption, INotCasteLevelOption
    {
        public LastLeadershipOption()
            : base(null)
        { }

        public override string GetTitlePrefix()
        {
            return "LastLeadership";
        }

        protected override bool PrivatePerform()
        {
            SetValue (StringInputDialog.Show(Name, Localize("Prompt"), Value));
            return true;
        }

        public override bool Persist()
        {
            ScoringLookup.UnloadCaches<IClanScoring>();

            return base.Persist();
        }
    }
}

