using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class SkateEx : SkatingRink.Skate, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<ISkatableObject, SkatingRink.Skate.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ISkatableObject, SkatingRink.Skate.Definition>(Singleton);
        }

        private bool CalculateIfActorIsOccultSkaterEx()
        {
            return SkateHelper.CalculateIfActorIsOccultSkaterEx(Actor);
        }        

        public override bool Run()
        {
            try
            {
                mIsOccultSkater = CalculateIfActorIsOccultSkaterEx();
                mShouldChangeOutfit = CalculateIfActorShouldChangeOutfit();
                if (!ApproachRink())
                {
                    return false;
                }

                if (!PutOnSkatesAndEnterRink())
                {
                    return false;
                }

                if (!WaitForSpot())
                {
                    return false;
                }

                if (!DoSkate())
                {
                    return false;
                }

                ExitRink();
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

        public new class Definition : SkatingRink.Skate.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SkateEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, ISkatableObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
