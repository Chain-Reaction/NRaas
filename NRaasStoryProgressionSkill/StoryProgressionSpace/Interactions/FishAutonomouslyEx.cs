using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    [Persistable]
    public class FishAutonomouslyEx : Common.IPreLoad, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition ();

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<FishingSpot, FishAutonomously.Definition, Definition>(true);
            if (tuning != null)
            {
                tuning.SetFlags(InteractionTuning.FlagField.DisallowAutonomous, false);
                tuning.SetFlags(InteractionTuning.FlagField.DisallowUserDirected, false);

                tuning.Availability.SkillThresholdValue = 0;
            }
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<FishingSpot, FishAutonomously.Definition>(Singleton);
        }

        public class Definition : FishAutonomously.Definition
        {
            /*
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new FishAutonomouslyEx();
                na.Init(ref parameters);
                return na;
            }
            */

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                //if (parameters.Autonomous)
                {
                    GameObjectHit gameObjectHit = InteractionInstance.CreateFakeGameObjectHit(parameters.Target.Position);
                    InteractionObjectPair iop = new IopWithCustomTuning(FishHere.Singleton, Terrain.Singleton, parameters.InteractionObjectPair.Tuning, parameters.Target.GetType());
                    InteractionInstanceParameters parameters2 = new InteractionInstanceParameters(iop, parameters.Actor, parameters.Priority, parameters.Autonomous, parameters.CancellableByPlayer, gameObjectHit);
                    return FishHere.Singleton.Test(ref parameters2, ref greyedOutTooltipCallback);
                }
                //return InteractionTestResult.Gen_NotAutonomous;
            }
        }
    }
}

