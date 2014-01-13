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
    public class RequireCasteLotOption : CasteListOption, IReadLotLevelOption, IWriteLotLevelOption
    {
        public RequireCasteLotOption()
        { }

        public override string GetTitlePrefix()
        {
            return "RequireCaste";
        }

        protected override string ValuePrefix
        {
            get { return "YesNo"; }
        }

        protected override bool PrivatePerform()
        {
            if (!base.PrivatePerform()) return false;

            ManagerLot.LotOptionsChanged(Manager.Lot);
            return true;
        }
    }
}

