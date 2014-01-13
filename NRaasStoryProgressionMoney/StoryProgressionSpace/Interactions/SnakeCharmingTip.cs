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
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
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
    public class SnakeCharmingTip : SnakeCharmingBasket.Tip, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<SnakeCharmingBasket, SnakeCharmingBasket.Tip.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<SnakeCharmingBasket, SnakeCharmingBasket.Tip.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                mSimPlaying = Target.ActorsUsingMe[0x0];
                if (mSimPlaying == null)
                {
                    return false;
                }

                return MusicalInstrumentWatchBase<SnakeCharmingBasket>.Tip(Actor, mSimPlaying, Target, SnakeCharmingBasket.Watch.kWatchTuning.MoneyPerLevel, 0x0);
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

        public new class Definition : InteractionDefinition<Sim, SnakeCharmingBasket, SnakeCharmingTip>
        {
            public override string GetInteractionName(Sim actor, SnakeCharmingBasket target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, SnakeCharmingBasket target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                Sim player = (target.ActorsUsingMe.Count > 0x0) ? target.ActorsUsingMe[0x0] : null;
                if (player == null)
                {
                    return false;
                }

                if (!target.CanTip(player))
                {
                    return false;
                }

                return (a.Household != player.Household);
            }
        }
    }
}
