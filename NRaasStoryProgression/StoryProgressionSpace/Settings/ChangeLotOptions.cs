using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class ChangeLotOptions : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "LotOptions";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!base.Allow(parameters)) return false;

            if (!StoryProgression.Main.GetValue<Managers.ManagerLot.AddInteractionsOption, bool>()) return false;

            if (parameters.mTarget is Sim) return true;

            if (parameters.mTarget is RabbitHole) return true;

            if (parameters.mTarget is Lot) return true;

            if (parameters.mTarget is BuildableShell) return true;

            return false;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Lot lot = parameters.mTarget as Lot;
            if (lot == null)
            {
                lot = parameters.mTarget.LotCurrent;
            }

            if (lot != null)
            {
                StoryProgression.Main.GetLotOptions(lot).ShowOptions(StoryProgression.Main, Common.Localize("LotOptions:MenuName"));
                return OptionResult.SuccessRetain;
            }

            return OptionResult.Failure;
        }
    }
}

