using NRaas.CareerSpace.Booters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.SelfEmployment
{
    public class Collector : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kSoldObject, OnSoldObject);
        }

        public static void OnSoldObject(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null)
            {
                return;
            }

            if ((actor.Household == null) || (actor.Household.IsSpecialHousehold))
            {
                return;
            }

            if ((e.TargetObject is RockGemMetalBase) || (e.TargetObject is IRelic) || (e.TargetObject is IHazInsect))
            {
                SkillBasedCareerBooter.UpdateExperience(actor, SkillNames.Collecting, e.TargetObject.Value);
            }
        }
    }
}
