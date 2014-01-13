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
    public class ClearAcountingOption : HouseBooleanOption, IWriteHouseLevelOption, INotPersistableOption
    {
        public ClearAcountingOption()
            : base(true)
        { }

        public override string GetTitlePrefix()
        {
            return "ClearAccounting";
        }

        public override string GetUIValue(bool pure)
        {
            return null;
        }

        protected override bool PrivatePerform()
        {
            if (!AcceptCancelDialog.Show(Localize("Prompt")))
            {
                return false;
            }

            NRaas.StoryProgression.Main.SetValue<AcountingOption,AccountingData>(Manager.House, new AccountingData());
            return true;
        }
    }
}

