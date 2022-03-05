using NRaas.CareerSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;

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
