extern alias SP;

using NRaas.MasterControllerSpace.Demographics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class ProgressionDebt : ByValueCriteria<ProgressionDebt.Clumper>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.ProgressionDebt";
        }

        public class Clumper : BaseClumper
        {
            public override int GetValue(Household house)
            {
                return SP::NRaas.StoryProgression.Main.GetValue<SP::NRaas.StoryProgressionSpace.Options.DebtOption, int>(house);
            }
        }
    }
}
