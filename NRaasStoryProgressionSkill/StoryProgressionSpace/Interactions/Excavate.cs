using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class Excavate : DigSite.Excavate, Common.IPreLoad
    {
        public new static readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<DigSite, DigSite.Excavate.Definition, Definition>(false);
        }

        // Methods
        public new void EventCallbackExcavate(StateMachineClient sender, IEvent evt)
        {
            try
            {
                mFoundLast = Target.NumTreasuresRemaining == 0x1;
                ExcavateSomething(Target, this);
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

        protected static void ExcavateSomething(DigSite ths, InteractionInstance inst)
        {
            Sim instanceActor = inst.InstanceActor;
            List<IGameObject> standardTreasure = ths.TreasureComponent.GetStandardTreasure(instanceActor);
            if (standardTreasure.Count > 0x0)
            {
                ths.mNumTreasuresRemaining--;
                GiveTreasuresToSim(standardTreasure, instanceActor, true);
            }
        }

        protected static void GiveTreasuresToSim(List<IGameObject> treasures, Sim actor, bool playCollectionBehavior)
        {
            if (actor != null)
            {
                if (playCollectionBehavior)
                {
                    TreasureComponent.PlayTreasureCollectionBehavior(treasures, actor);
                }
                GiveTreasuresToSimInternal(treasures, actor);
            }
        }

        private static void GiveTreasuresToSimInternal(List<IGameObject> treasures, Sim actor)
        {
            foreach (IGameObject obj2 in treasures)
            {
                IControlHowSimsAcquireMe me = obj2 as IControlHowSimsAcquireMe;
                if (me != null)
                {
                    me.GetAcquiredBySim(actor);
                }
                else
                {
                    if (Inventories.TryToMove(obj2, actor))
                    {
                        ITreasureSpawnableObject obj3 = obj2 as ITreasureSpawnableObject;
                        if ((obj3 != null) && !obj3.HasBeenCollected)
                        {
                            obj3.OnTreasureCollected(actor, false);
                        }
                        continue;
                    }
                    obj2.Destroy();
                }
            }
        }

        public override void ExcavateLoopCallback(StateMachineClient smc, Interaction<Sim, DigSite>.LoopData loopData)
        {
            try
            {
                mMinsUntilFindSomething -= loopData.mDeltaTime;
                if (mMinsUntilFindSomething <= 0f)
                {
                    AddOneShotScriptEventHandler(0x64, new SacsEventHandler(EventCallbackExcavate));
                    AnimateSim("Found Something");
                    SetTimeToFindSomething();
                }
                if (base.Target.NumTreasuresRemaining <= 0x0)
                {
                    if (mFoundLast)
                    {
                        Actor.SimDescription.RelicStats.IncrementDigSitesExcavated();
                    }
                    Actor.AddExitReason(ExitReason.Finished);
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

        // Nested Types
        private new class Definition : InteractionDefinition<Sim, DigSite, Excavate>
        {
            public override string GetInteractionName(Sim actor, DigSite target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(DigSite.Excavate.Singleton, target));
            }

            // Methods
            public override bool Test(Sim a, DigSite target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return (target.NumTreasuresRemaining > 0x0);
            }
        }
    }
}

