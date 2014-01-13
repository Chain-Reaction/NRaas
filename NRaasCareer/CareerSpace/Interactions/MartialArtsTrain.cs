using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
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

namespace NRaas.CareerSpace.Interactions
{
    public class MartialArtsTrain : Common.IAddInteraction
    {
        public static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<TrainingDummy>(Singleton);
        }

        protected class Definition : TrainingDummy.Train.Definition
        {
            public override string GetInteractionName(Sim actor, TrainingDummy target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(TrainingDummy.Train.Singleton, target));
            }

            public override bool Test(Sim a, TrainingDummy target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback))
                {
                    return false;
                }

                if (a.SkillManager.GetSkillLevel(SkillNames.MartialArts) >= 7)
                {
                    return false;
                }

                return (SkillBasedCareerBooter.GetSkillBasedCareer(a, SkillNames.MartialArts) != null);
            }
        }
    }
}
