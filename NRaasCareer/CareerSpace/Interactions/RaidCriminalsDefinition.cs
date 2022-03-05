using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace NRaas.CareerSpace.Interactions
{
    public class RaidCriminalDefinition : InteractionDefinition<Sim, RabbitHole, Hideout.RaidCriminals>, Common.IPreLoad, Common.IAddInteraction
    {
        public static RaidCriminalDefinition Singleton = new RaidCriminalDefinition();

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, Hideout.RaidCriminals.Definition, RaidCriminalDefinition>(false);
        }

        public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
        {
            return base.GetInteractionName(actor, target, new InteractionObjectPair(Hideout.RaidCriminals.Singleton, target));
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Hideout>(Singleton);
        }

        // Methods
        public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            if (!(a.Occupation is OmniCareer)) return false;

            GreyedOutTooltipCallback callback = null;
            LawEnforcement job = OmniCareer.Career<LawEnforcement>(a.Occupation);
            if ((job != null) && job.CanRaidCriminalWarehouses)
            {
                Hideout hideout = target as Hideout;
                if (hideout != null)
                {
                    DateAndTime time;
                    if (!hideout.mSimToLastRaidTimestamp.TryGetValue(a, out time))
                    {
                        return true;
                    }
                    if (SimClock.ElapsedTime(TimeUnit.Hours, time, SimClock.CurrentTime()) >= Hideout.kMinimumDurationBetweenCriminalRaids)
                    {
                        return true;
                    }
                    if (callback == null)
                    {
                        callback = delegate
                        {
                            return Hideout.RaidCriminals.LocalizeString("CannotUseHideout", new object[] { a });
                        };
                    }
                    greyedOutTooltipCallback = callback;
                    return false;
                }
            }
            return false;
        }
    }
}
