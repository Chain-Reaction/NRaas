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
    public class QueueTrainSimEx : AthleticGameObject.QueueTrainSim, Common.IPreLoad
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<AthleticGameObject, AthleticGameObject.QueueTrainSim.Definition, Definition>(false);
        }

        public override bool Run()
        {
            try
            {
                return base.Run();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                string msg = null;

                if (Trainee != null)
                {
                    msg += "Trainee: " + Trainee;
                }
                else
                {
                    msg += "Trainee: null";
                }

                Sim otherSim = Target.OtherActor(Actor);
                if (otherSim != null)
                {
                    msg += "Other Sim: " + otherSim;
                }
                else
                {
                    msg += "Other Sim: null";
                }

                Common.Exception(Actor, Target, msg, e);
                return false;
            }
        }

        public new class Definition : InteractionDefinition<Sim, AthleticGameObject, QueueTrainSimEx>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, AthleticGameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(AthleticGameObject.QueueTrainSim.Singleton, target));
            }

            public override bool Test(Sim a, AthleticGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
