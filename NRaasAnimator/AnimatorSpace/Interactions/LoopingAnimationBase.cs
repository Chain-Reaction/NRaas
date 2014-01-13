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
    public interface IAnimationDefinition
    {
        string ClipName
        {
            get;
        }

        string InteractionName
        {
            get;
        }

        string Type
        {
            get;
        }
    }

    public class LoopingAnimationBase : Interaction<Sim, Sim>
    {
        bool mPaused;

        float mTiming = 0;

        int mIterations = int.MaxValue;

        protected string mActorName = "x";

        ILookAtTask mLookAt;

        public int Iterations
        {
            get { return mIterations; }
            set { mIterations = value; }
        }

        public float Timing
        {
            get { return mTiming; }
            set { mTiming = value; }
        }

        public bool Paused
        {
            get { return mPaused; }
            set 
            { 
                mPaused = value;
                /*
                if ((mCurrentStateMachine != null) && (mCurrentStateMachine.CanPause(mActorName)))
                {
                    if (value)
                    {
                        mCurrentStateMachine.Pause(mActorName);
                    }
                    else
                    {
                        mCurrentStateMachine.Continue(mActorName);
                    }
                }
                */
            }
        }

        public Sim TargetSim
        {
            get { return Target; }
        }

        public void SetLookAt(GameObject obj)
        {
            DisposeLookAt();

            mLookAt = Target.LookAtManager.CreateLookAt(obj, 1000, LookAtJointFilter.HeadBones | LookAtJointFilter.TorsoBones);
        }

        public void Cancel()
        {
            Target.AddExitReason(ExitReason.Finished);
        }

        public override string ToString()
        {
            string text = InteractionDefinition.ToString();

            text += Common.NewLine + "Paused: " + mPaused;
            text += Common.NewLine + "Timing: " + mTiming;
            text += Common.NewLine + "Iterations: " + mIterations;

            return text;
        }

        public override void ConfigureInteraction()
        {
            try
            {
                SetAvailableTones(AnimationTone.GetTones ());

                base.ConfigureInteraction();
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public void DisposeLookAt()
        {
            if (mLookAt != null)
            {
                mLookAt.StopIt(true);
                mLookAt.Dispose();

                mLookAt = null;
            }
        }

        public override void Cleanup()
        {
            try
            {
                DisposeLookAt();
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
            base.Cleanup();
        }
    }
}