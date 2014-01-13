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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.AnimatorSpace.Tones
{
    public abstract class AnimationTone : Tone
    {
        public AnimationTone()
        { }

        public static List<Tone> GetTones()
        {
            List<Tone> tones = new List<Tone>();

            foreach (AnimationTone tone in Common.DerivativeSearch.Find<AnimationTone>(Common.DerivativeSearch.Caching.NoCache))
            {
                tones.Add(tone);
            }

            return tones;
        }

        public override void OnSelectionBegin(InteractionInstance interactionInstance)
        {
            new ResetTone(interactionInstance).AddToSimulator();

            base.OnSelectionBegin(interactionInstance);
        }

        public static bool ControlLoop(LoopingAnimationBase interaction)
        {
            try
            {
                float start = SimClock.ElapsedTime(TimeUnit.Seconds);
                while (((interaction.Paused) || (interaction.Timing > (SimClock.ElapsedTime(TimeUnit.Seconds) - start))) && (!interaction.TargetSim.HasExitReason()))
                {
                    SpeedTrap.Sleep();
                }
                
                interaction.Iterations--;

                return (interaction.Iterations > 0);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(interaction.TargetSim, interaction.TargetSim, e);
                return false;
            }
        }

        public class ResetTone : Common.FunctionTask
        {
            InteractionInstance mInteraction;

            public ResetTone(InteractionInstance interaction)
            {
                mInteraction = interaction;
            }

            protected override void OnPerform()
            {
                mInteraction.CurrentTone = null;
            }
        }
    }
}