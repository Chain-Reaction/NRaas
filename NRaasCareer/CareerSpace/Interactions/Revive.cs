using NRaas.CareerSpace.Skills;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.SimIFace;
using System;

namespace NRaas.CareerSpace.Interactions
{
    public class Revive : Interaction<Sim, Sim>, Common.IAddInteraction
    {
        // Fields
        public static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                Urnstone grave = Urnstone.FindGhostsGrave(Actor);
                if (grave != null)
                {
                    grave.GhostToSim(Actor, false, false);
                }
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public class Definition : InteractionDefinition<Sim, Sim, Revive>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.Localize("Revive:MenuName", actor.IsFemale, new object[0]);
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;

                if (a != target) return false;

                if (!target.SimDescription.IsPlayableGhost) return false;

                Assassination skill = a.SkillManager.GetSkill<Assassination>(Assassination.StaticGuid);
                if (skill == null) return false;

                if (!skill.IsGhost()) return false;

                if (Urnstone.FindGhostsGrave(a) == null) return false;

                return true;
            }
        }
    }
}
