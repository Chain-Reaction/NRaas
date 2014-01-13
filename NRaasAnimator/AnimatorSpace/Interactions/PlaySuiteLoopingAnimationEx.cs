using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.AnimatorSpace.Interactions;
using NRaas.AnimatorSpace.Tones;
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

namespace NRaas.AnimatorSpace.Interactions
{
    public class PlaySuiteLoopingAnimationEx : LoopingAnimationBase
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override string GetInteractionName()
        {
            return (InteractionDefinition as Definition).InteractionName;
        }

        public override bool Run()
        {
            try
            {
                StateMachineClient client = StateMachineClient.Acquire(Actor, "single_animation");
                client.SetActor("x", Actor);
                client.EnterState("x", "Enter");

                Definition definition = InteractionDefinition as Definition;

                Sim.AnimationClipDataForCAS rcas = definition.mAnimation;

                while (!Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                {
                    if (!AnimationTone.ControlLoop(this)) break;

                    client.SetParameter("AnimationClip", rcas.AnimationClipName, rcas.ProductVersion);
                    client.RequestState("x", "Animate");
                    client.RequestState(false, "x", "Enter");
                }
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
            return false;
        }

        [DoesntRequireTuning]
        public class Definition : InteractionDefinition<Sim, Sim, PlaySuiteLoopingAnimationEx>, IAnimationDefinition
        {
            public Sim.AnimationClipDataForCAS mAnimation;

            string mType;

            public Definition()
            {}
            public Definition(Sim.AnimationClipDataForCAS animation, string type)
            {
                mAnimation = animation;
                mType = type;
            }

            public string InteractionName
            {
                get { return mAnimation.AnimationClipName; }
            }

            public string ClipName
            {
                get { return mAnimation.AnimationClipName; }
            }

            public string Type
            {
                get { return mType; }
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return InteractionName;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { mType };
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }

            public override string ToString()
            {
                string text = "ClipName: " + ClipName;
                text += Common.NewLine + "InteractionName: " + InteractionName;

                return text;
            }
        }
    }
}