using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.DresserSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.DresserSpace.Options
{
    public abstract class ProcessAllSimsOption : OperationSettingOption<GameObject>
    {
        protected abstract void Process(SimDescription sim);

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt"))) return OptionResult.Failure;

            foreach(SimDescription sim in Household.AllSimsLivingInWorld())
            {
                if (sim.Household == Household.ActiveHousehold)
                {
                    if (!Dresser.Settings.mCheckAffectActive) continue;
                }

                if (Dresser.Settings.IsProtected(sim)) continue;

                Process(sim);
            }

            return OptionResult.SuccessClose;
        }
    }
}
