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
    public class DisallowCasteLotOption : CasteListOption, IReadLotLevelOption, IWriteLotLevelOption
    {
        public DisallowCasteLotOption()
        { }

        public override string GetTitlePrefix()
        {
            return "DisallowCaste";
        }

        protected override string ValuePrefix
        {
            get { return "Disallowed"; }
        }

        protected override bool PrivatePerform()
        {
            if (!base.PrivatePerform()) return false;

            ManagerLot.LotOptionsChanged(Manager.Lot);
            return true;
        }
    }
}

