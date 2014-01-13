using NRaas.CommonSpace.Options;
using NRaas.AnimatorSpace.Interactions;
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
    public class StopLooking : OperationSettingOption<Sim>, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "StopLooking";
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!base.Allow(parameters)) return false;

            if (parameters.mTarget.InteractionQueue == null) return false;

            return (parameters.mTarget.InteractionQueue.GetCurrentInteraction() is LoopingAnimationBase);
        }

        protected override OptionResult Run(GameHitParameters< Sim> parameters)
        {
            LoopingAnimationBase interaction = parameters.mActor.InteractionQueue.GetCurrentInteraction() as LoopingAnimationBase;

            interaction.DisposeLookAt();

            return OptionResult.SuccessClose;
        }
    }
}