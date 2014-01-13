using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class AdjustAutonomy : SimFromList, IAdvancedOption
    {
        int mAutonomous = 0;

        public override string GetTitlePrefix()
        {
            return "AdjustAutonomy";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.LotHome == null) return false;

            Sim sim = me.CreatedSim;
            if (sim == null) return false;

            Autonomy autonomy = sim.Autonomy;
            if (autonomy == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Sim sim = me.CreatedSim;
            if (sim == null) return false;

            Autonomy autonomy = sim.Autonomy;
            if (autonomy == null) return false;

            if (!ApplyAll)
            {
                string msg = "AdjustAutonomy:Prompt";
                if (autonomy.mAutonomyDisabledCount > 0)
                {
                    msg = "AdjustAutonomy:UnPrompt";
                    mAutonomous = 0;
                }
                else
                {
                    mAutonomous = int.MaxValue / 2;
                }

                if (!AcceptCancelDialog.Show(Common.Localize(msg, me.IsFemale, new object[] { me })))
                {
                    return false;
                }
            }

            autonomy.mAutonomyDisabledCount = mAutonomous;
            return true;
        }
    }
}
