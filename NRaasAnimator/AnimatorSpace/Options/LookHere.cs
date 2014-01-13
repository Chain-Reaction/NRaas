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
    public class LookHere : OperationSettingOption<GameObject>, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "LookHere";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!base.Allow(parameters)) return false;

            if (parameters.mTarget.InInventory) return false;

            if (parameters.mActor == parameters.mTarget) return false;

            if (parameters.mActor.InteractionQueue == null) return false;

            return (parameters.mActor.InteractionQueue.GetCurrentInteraction() is LoopingAnimationBase);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            LoopingAnimationBase interaction = parameters.mActor.InteractionQueue.GetCurrentInteraction() as LoopingAnimationBase;

            interaction.SetLookAt(parameters.mTarget);
            return OptionResult.SuccessClose;
        }
    }
}