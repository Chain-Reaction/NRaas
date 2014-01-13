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
    public class InStasisOption : GenericOptionBase.BooleanOption, IReadSimLevelOption, IReadHouseLevelOption, IWriteHouseLevelOption
    {
        public InStasisOption()
            : base(false)
        { }

        public override string GetTitlePrefix()
        {
            return "Stasis";
        }

        public override bool Value
        {
            get
            {
                if (!ShouldDisplay()) return false;

                return base.Value;
            }
        }

        protected override bool PrivatePerform()
        {
            SetValue(!PureValue);
            return true;
        }
    }
}

