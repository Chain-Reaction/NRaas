using NRaas.CommonSpace.Helpers;
using NRaas.GoHereSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class GoHereWithEx : Terrain.GoHereWith, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton = null;

        public void OnPreLoad()
        {
            Tunings.Inject<Terrain, Terrain.GoHereWith.GoHereDefinition, Definition>(false);

            sOldSingleton = Terrain.GoHereWith.Singleton;
            Terrain.GoHereWith.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Terrain, Terrain.GoHereWith.GoHereDefinition>(Singleton);
        }

        public override void Init(ref InteractionInstanceParameters parameters)
        {
            try
            {
                base.Init(ref parameters);

                if (GoHere.Settings.mAllowGoHereStack)
                {
                    if (mPriority.Value < 0)
                    {
                        RaisePriority();
                    }
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        private class Definition : Terrain.GoHereWith.GoHereDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoHereWithEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Terrain target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
