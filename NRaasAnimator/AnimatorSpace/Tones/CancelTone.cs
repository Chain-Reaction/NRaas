using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.AnimatorSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.AnimatorSpace.Tones
{
    public class CancelTone : AnimationTone
    {
        public CancelTone()
        { }

        public override void OnSelectionBegin(InteractionInstance interactionInstance)
        {
            try
            {
                LoopingAnimationBase interaction = interactionInstance as LoopingAnimationBase;

                interaction.Cancel();

                base.OnSelectionBegin(interactionInstance);
            }
            catch (Exception e)
            {
                Common.Exception(interactionInstance.InstanceActor, e);
            }
        }

        public override string Name()
        {
            return Common.Localize("Cancel:MenuName");
        }

        public override string Description()
        {
            return Common.Localize("Cancel:Description");
        }
    }
}