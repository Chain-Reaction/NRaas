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
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class DigThroughEx : JunkPile.DigThrough, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<JunkPile, JunkPile.DigThrough.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<JunkPile, JunkPile.DigThrough.Definition>(Singleton);
        }

        // Methods
        public new void DigThroughLoopCallback(StateMachineClient smc, Interaction<Sim, JunkPile>.LoopData ld)
        {
            try
            {
                EventTracker.SendEvent(EventTypeId.kLootAJunkPile, Actor, Target);
                if (!Autonomous)
                {
                    int numScraps = 0x1;
                    InventingSkill skill = Actor.SkillManager.GetSkill<InventingSkill>(SkillNames.Inventing);
                    if ((skill != null) && skill.OppTheScrapperCompleted)
                    {
                        numScraps += (int) Math.Ceiling((double) (numScraps * InventingSkill.kTheScraperScrapMultiplier));
                    }
                    ScrapInitParameters initData = new ScrapInitParameters(numScraps);
                    Scrap scrap = GlobalFunctions.CreateObject("scrapPile", ProductVersion.EP2, Vector3.Origin, 0x0, Vector3.UnitZ, null, initData) as Scrap;
                    
                    if (Inventories.TryToMove(scrap, Actor))
                    {
                        mScrapCollected++;
                    }

                    Target.DecrementScrapAmount();
                    if (Target.IsEmpty)
                    {
                        Actor.AddExitReason(ExitReason.Finished);
                    }
                }
                else
                {
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

        public override bool Run()
        {
            try
            {
                int num;
                if (!Actor.RouteToSlotList(Target, Target.GetRoutingSlots(), out num))
                {
                    return false;
                }

                StandardEntry();
                EnterStateMachine("junkpile", "Enter", "x", "JunkPile");
                AnimateSim("DigThroughLoop");
                BeginCommodityUpdates();

                bool succeeded = false;
                try
                {
                    float kMinutesBetweenReceivingScrap = JunkPile.kMinutesBetweenReceivingScrap;
                    InventingSkill skill = Actor.SkillManager.GetSkill<InventingSkill>(SkillNames.Inventing);
                    if ((skill != null) && skill.OppTheScrapperCompleted)
                    {
                        kMinutesBetweenReceivingScrap *= JunkPile.kTimeMultiplierIfTheScrapper;
                    }
                    if (Autonomous)
                    {
                        kMinutesBetweenReceivingScrap = RandomUtil.RandomFloatGaussianDistribution(0f, (float)JunkPile.kMinutesSimsShouldAutonomouslyDigThrough);
                    }

                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, JunkPile>.InsideLoopFunction(DigThroughLoopCallback), mCurrentStateMachine, kMinutesBetweenReceivingScrap);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                AnimateSim("Exit");
                StandardExit();
                return succeeded;
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

        private new sealed class Definition : JunkPile.DigThrough.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new DigThroughEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, JunkPile target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}

