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
    public class PlaySpecificLoopingAnimationEx : LoopingAnimationBase
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override string GetInteractionName()
        {
            return (InteractionDefinition as Definition).InteractionName;
        }

        public void Looped(StateMachineClient smc, IEvent evt)
        {
            if (!AnimationTone.ControlLoop(this))
            {
                Target.AddExitReason(ExitReason.Finished);
            }
        }

        public override bool Run()
        {
            try
            {
                mCurrentStateMachine = StateMachineClient.Acquire(Target, "single_looping_animation");
                mCurrentStateMachine.SetActor(mActorName, Target);
                mCurrentStateMachine.EnterState(mActorName, "EnterExit");

                Definition interactionDefinition = InteractionDefinition as Definition;

                mCurrentStateMachine.AddPersistentStateEnteredEventHandler("Animate", new SacsEventHandler(Looped));

                mCurrentStateMachine.SetParameter("AnimationClip", interactionDefinition.ClipName, interactionDefinition.mProductVersion);
                mCurrentStateMachine.RequestState(mActorName, "Animate");
                DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                mCurrentStateMachine.RequestState(mActorName, "EnterExit");
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
        public class Definition : InteractionDefinition<Sim, Sim, PlaySpecificLoopingAnimationEx>, IAnimationDefinition
        {
            // Fields
            private string mClipName;
            private string mInteractionName;
            public ProductVersion mProductVersion;

            string[] mPath;

            public Definition()
            {}
            public Definition(Sim.PlaySpecificLoopingAnimation.Definition definition)
            {
                mInteractionName = definition.InteractionName;
                mProductVersion = definition.ProductVersion;
                mClipName = definition.ClipName;

                List<string> path = new List<string>(definition.MenuPath);
                path.RemoveAt(0);

                mPath = path.ToArray();
            }

            public string InteractionName
            {
                get { return mInteractionName; }
            }

            public string ClipName
            {
                get { return mClipName; }
            }

            public string Type
            {
                get { return "CAS"; }
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return mInteractionName;
            }

            public override string[] GetPath(bool isFemale)
            {
                return mPath;
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