using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.AnimatorSpace.Options
{
    public class CancelInteraction : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "CancelInteraction";
        }

        protected override bool Allow(GameHitParameters< Sim> parameters)
        {
            if (parameters.mTarget.InteractionQueue == null) return false;

            if (parameters.mTarget.InteractionQueue.GetCurrentInteraction() == null) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            parameters.mTarget.InteractionQueue.CancelAllInteractions();
            return OptionResult.SuccessClose;
        }
    }
}