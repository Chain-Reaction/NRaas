using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class TrainSim : AthleticGameObject.TrainSim, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            MethodInfo stopMethod = typeof(TrainSim).GetMethod("OnDenyTest");

            ActionData data = ActionData.Get("Train Sim");
            if (data != null)
            {
                data.ProceduralTest = stopMethod;
            }

            Tunings.Inject<AthleticGameObject, AthleticGameObject.TrainSim.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<AthleticGameObject>(Singleton);
        }

        public static bool OnDenyTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return false;
        }

        public static bool CallbackTest(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (!SocialTest.TestTrainSim(actor, target, topic, isAutonomous, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                if (SkillBasedCareerBooter.GetSkillBasedCareer(actor, SkillNames.Athletic) == null)
                {
                    if (actor.SkillManager.GetSkillLevel(SkillNames.Athletic) < AthleticGameObject.TrainSim.kAthleticSkillLevelGate)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
                return false;
            }
        }

        public static void OnAccepted(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                AthleticGameObject[] objects = actor.LotCurrent.GetObjects<AthleticGameObject>();
                float maxValue = float.MaxValue;
                AthleticGameObject obj2 = null;
                foreach (AthleticGameObject obj3 in objects)
                {
                    if (!obj3.InUse && obj3.CanObjectTrainSim())
                    {
                        float distanceToObject = obj3.GetDistanceToObject(actor);
                        if (distanceToObject < maxValue)
                        {
                            maxValue = distanceToObject;
                            obj2 = obj3;
                        }
                    }
                }
                if (obj2 != null)
                {
                    QueueUpTrainSim.Definition definition = new QueueUpTrainSim.Definition(target);
                    actor.InteractionQueue.AddNext(definition.CreateInstance(obj2, actor, actor.InheritedPriority(), false, true));
                }
            }
            catch(ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        protected new class Definition : InteractionDefinition<Sim, AthleticGameObject, TrainSim>
        {
            // Methods
            public override string GetInteractionName(Sim a, AthleticGameObject target, InteractionObjectPair interaction)
            {
                Sim sim = target.OtherActor(a);
                if (sim != null)
                {
                    return AthleticGameObject.TrainSim.LocalizeString(a.IsFemale, "InteractionName", new object[] { sim.SimDescription.FirstName });
                }
                return AthleticGameObject.TrainSim.LocalizeString(a.IsFemale, "InteractionName", new object[] { "" });
            }

            public override bool Test(Sim a, AthleticGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.CanObjectTrainSim())
                {
                    return false;
                }

                if (a.SkillManager.GetSkillLevel(SkillNames.Athletic) >= AthleticGameObject.TrainSim.kAthleticSkillLevelGate)
                {
                    return false;
                }

                if (target.OtherActor(a) == null)
                {
                    return false;
                }

                return (SkillBasedCareerBooter.GetSkillBasedCareer(a, SkillNames.Athletic) != null);
            }
        }
    }
}
