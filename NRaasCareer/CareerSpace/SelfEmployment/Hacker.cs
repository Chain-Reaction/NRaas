using NRaas.CareerSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.SelfEmployment
{
    public class Hacker : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kHacked, OnHack);
        }

        public static void OnHack(Event e)
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

            IncrementalEvent iEvent = e as IncrementalEvent;
            if (iEvent != null)
            {
                SkillBasedCareerBooter.UpdateExperience(actor, SkillNames.Hacking, (int)iEvent.mIncrement);
            }
        }
    }
}
