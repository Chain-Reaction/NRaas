using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class HotTubGetOut : HotTubBase.GetOut, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public bool mIsMaster;

        public bool mCompleted;

        public void OnPreLoad()
        {
            Tunings.Inject<HotTubBase, HotTubBase.GetOut.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<HotTubBase, HotTubBase.GetOut.Definition>(Singleton);
        }

        public override void Cleanup()
        {
            try
            {
                if (Target.ActorsUsingMe.Count < 0x1)
                {
                    Target.ForceShutDownEverything();

                    StereoCheap stereo = Target.GetContainedObject(Target.BoomboxSlot) as StereoCheap;
                    if (stereo != null)
                    {
                        stereo.TurnOff();

                        // Custom
                        stereo.FadeOut(false);

                        if (!Actor.IsNPC && Actor.Inventory.TryToAdd(stereo))
                        {
                            stereo.SetOpacity(1f, 0f);
                        }
                        else
                        {
                            Target.ReturnBoomboxToOwnerHousehold();
                        }
                    }
                }

                base.Cleanup();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.DebugException(Actor, Target, e);
            }
        }

        public new class Definition : HotTubBase.GetOut.Definition
        {
            public override string GetInteractionName(Sim actor, HotTubBase target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new HotTubGetOut();
                result.Init(ref parameters);
                return result;
            }
        }
    }
}
