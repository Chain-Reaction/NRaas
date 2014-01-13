using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Interactions
{
    public class GetTattooInRabbitHoleEx : DaySpa.GetTattooInRabbitHole, Common.IPreLoad, Common.IAddInteraction
    {
        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<DaySpa, DaySpa.GetTattooInRabbitHole.Definition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.Children = true;
                tuning.Availability.Teens = true;
            }

            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<DaySpa, DaySpa.GetTattooInRabbitHole.Definition>(Singleton);
        }

        public override bool InRabbitHole()
        {
            try
            {
                bool tookSemaphore = mTookSemaphore;
                GiveTattooEx.DisplayCAS(base.Actor, ref tookSemaphore);
                mTookSemaphore = tookSemaphore;

                if (!CelebrityManager.TryModifyFundsWithCelebrityDiscount(Actor, Target, Tattooing.kCostTattooDaySpa, true))
                {
                    return false;
                }

                EventTracker.SendEvent(EventTypeId.kGotTattoo, Actor);
                Actor.BuffManager.AddElement(BuffNames.FightThePower, Origin.FromTattoo);
                return true;
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

        public new class Definition : DaySpa.GetTattooInRabbitHole.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GetTattooInRabbitHoleEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
