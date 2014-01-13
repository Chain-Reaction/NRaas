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
    public class IterationsTone : AnimationTone
    {
        int mIterations;

        public IterationsTone()
        { }

        public override void OnSelectionBegin(InteractionInstance interactionInstance)
        {
            try
            {
                new Task(this, interactionInstance).AddToSimulator();

                base.OnSelectionBegin(interactionInstance);
            }
            catch (Exception e)
            {
                Common.Exception(interactionInstance.InstanceActor, e);
            }
        }

        public override bool Test(InteractionInstance ii, out StringDelegate reason)
        {
            try
            {
                LoopingAnimationBase interaction = ii as LoopingAnimationBase;

                mIterations = interaction.Iterations;

                return base.Test(ii, out reason);
            }
            catch (Exception e)
            {
                reason = null;

                Common.Exception(ii.InstanceActor, e);
                return false;
            }
        }

        public override string Name()
        {
            if (mIterations > 100)
            {
                return Common.Localize("Iterations:Infinite");
            }
            else
            {
                return Common.Localize("Iterations:MenuName", false, new object[] { mIterations });
            }
        }

        public override string Description()
        {
            return Common.Localize("Iterations:Description");
        }

        public class Task : Common.FunctionTask
        {
            IterationsTone mTone;

            InteractionInstance mInteractionInstance;

            public Task(IterationsTone tone, InteractionInstance interactionInstance)
            {
                mInteractionInstance = interactionInstance;
                mTone = tone;
            }

            protected override void OnPerform()
            {
                LoopingAnimationBase interaction = mInteractionInstance as LoopingAnimationBase;

                string value = StringInputDialog.Show(Common.Localize("Iterations:MenuName"), Common.Localize("Iterations:Prompt", interaction.TargetSim.IsFemale, new object[] { interaction.TargetSim }), interaction.Iterations.ToString());
                if (string.IsNullOrEmpty(value)) return;

                int iterations = 0;

                if (!int.TryParse(value, out iterations))
                {
                    AcceptCancelDialog.Show(Common.Localize("Numeric:Error"));
                    return;
                }

                if (iterations < 0)
                {
                    iterations = 0;
                }

                interaction.Iterations = iterations;

                mTone.mIterations = iterations;

                Sims3.UI.Hud.InteractionQueue.Instance.UpdateQueueFull();
            }
        }
    }
}