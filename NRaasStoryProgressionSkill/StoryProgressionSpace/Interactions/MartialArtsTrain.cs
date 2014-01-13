using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class MartialArtsTrain : TrainingDummy.PushTrain, Common.IPreLoad
    {
        public static InteractionDefinition TrainSingleton = new TrainDefinition();

        public void OnPreLoad()
        {
            Tunings.Inject<TrainingDummy, TrainingDummy.PushTrain.Definition, Definition>(false);
        }

        public override bool Run()
        {
            try
            {
                bool flag = false;

                Definition definition = InteractionDefinition as Definition;

                if (definition.mSim != null)
                {
                    InteractionInstance entry = TrainingDummy.Practice.Singleton.CreateInstance(Target, definition.mSim, Actor.InheritedPriority(), Autonomous, true);
                    InteractionInstance instance = TrainSingleton.CreateInstance(Target, Actor, Actor.InheritedPriority(), Autonomous, true);

                    entry.LinkedInteractionInstance = instance;

                    flag = definition.mSim.InteractionQueue.Add(entry) && Actor.InteractionQueue.PushAsContinuation(instance, true);
                }
                return flag;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : InteractionDefinition<Sim, TrainingDummy, MartialArtsTrain>
        {
            public readonly Sim mSim;

            public Definition(Sim sim)
            {
                mSim = sim;
            }

            public override string GetInteractionName(Sim actor, TrainingDummy target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(TrainingDummy.Train.Singleton, target));
            }

            public override bool Test(Sim a, TrainingDummy target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }

        protected class TrainDefinition : TrainingDummy.Train.Definition
        {
            public override string GetInteractionName(Sim actor, TrainingDummy target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(TrainingDummy.Train.Singleton, target));
            }

            public override bool Test(Sim a, TrainingDummy target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
