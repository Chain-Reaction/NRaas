using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class ContinueSculptureEx : SculptingStation.ContinueSculpture, Common.IPreLoad, Common.IAddInteraction
    {
        public static new readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<SculptingStation, SculptingStation.ContinueSculpture.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<SculptingStation, SculptingStation.ContinueSculpture.Definition>(Singleton);
        }

        public new class Definition : SculptingStation.ContinueSculpture.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ContinueSculptureEx();
                na.Init(ref parameters);
                return na;
            }
        }
        public override void Cleanup()
        {
            base.Cleanup();
            if (mCurrentStateMachine != null)
            {
                //mCurrentStateMachine.RequestState ("x", "Exit Sculpt");
                mCurrentStateMachine.Dispose();
                mCurrentStateMachine = null;
            }
            if (mSoundHandle != 0u)
            {
                Audio.StopSound(mSoundHandle);
            }
        }
    }
}