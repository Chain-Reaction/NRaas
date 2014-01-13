using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
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
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class QueueUpTrainSim : Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<AthleticGameObject>(Singleton);
        }

        public class Definition : AthleticGameObject.QueueUpTrainSim.Definition
        {
            public Definition()
            { }
            public Definition(Sim trainableSim)
                : base(trainableSim)
            { }
            public Definition(string menuText, string[] menuPath, Sim trainableSim)
                : base(menuText, menuPath, trainableSim)
            { }

            public override string GetInteractionName(ref InteractionInstanceParameters parameters)
            {
                return base.GetInteractionName(ref parameters) + (Common.kDebugging ? " (NRaas)" : "");
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, AthleticGameObject target, List<InteractionObjectPair> results)
            {
                foreach (Sim sim in actor.LotCurrent.GetSims())
                {
                    if ((actor != sim) && sim.SimDescription.TeenOrAbove)
                    {
                        results.Add(new InteractionObjectPair(new Definition(sim.SimDescription.FirstName, new string[] { AthleticGameObject.QueueUpTrainSim.LocalizeString(actor.IsFemale, "TrainSimMenuText", new object[0x0]) }, sim), iop.Target));
                    }
                }
            }

            public override bool Test(Sim a, AthleticGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous)
                {
                    if (TrainableSim.Service != null) return false;
                }

                if (SkillBasedCareerBooter.GetSkillBasedCareer(a, SkillNames.Athletic) == null)
                {
                    return false;
                }

                if ((!target.CanObjectTrainSim() || (a.SkillManager.GetSkillLevel(SkillNames.Athletic) >= AthleticGameObject.TrainSim.AthleticSkillLevelGate)) || ((target.ActorsUsingMe.Count != 0x0) || isAutonomous))
                {
                    return false;
                }

                if (target.Cardio)
                {
                    return AthleticGameObject.WorkOut.CardioSingleton.CreateInstance(target, TrainableSim, a.InheritedPriority(), false, true).Test();
                }
                return AthleticGameObject.WorkOut.StrengthSingleton.CreateInstance(target, TrainableSim, a.InheritedPriority(), false, true).Test();
            }
        }
    }
}
