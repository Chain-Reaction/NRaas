using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using NRaas.OverwatchSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Interactions
{
    public class HarvestEx : HarvestPlant.Harvest, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<HarvestPlant, HarvestPlant.Harvest.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<HarvestPlant, HarvestPlant.Harvest.Definition>(Singleton);
        }

        private bool DoHarvestEx()
        {
            Soil soil;
            Target.RemoveHarvestStateTimeoutAlarm();
            StandardEntry();
            BeginCommodityUpdates();
            StateMachineClient stateMachine = Target.GetStateMachine(Actor, out soil);
            mDummyIk = soil;
            bool hasHarvested = true;
            if (Actor.IsInActiveHousehold)
            {
                hasHarvested = false;
                foreach (SimDescription description in Actor.Household.SimDescriptions)
                {
                    Gardening skill = description.SkillManager.GetSkill<Gardening>(SkillNames.Gardening);
                    if ((skill != null) && skill.HasHarvested())
                    {
                        hasHarvested = true;
                        break;
                    }
                }
            }

            if (stateMachine != null)
            {
                stateMachine.RequestState("x", "Loop Harvest");
            }

            Plant.StartStagesForTendableInteraction(this);
            while (!Actor.WaitForExitReason(Sim.kWaitForExitReasonDefaultTime, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
            {
                if ((ActiveStage != null) && ActiveStage.IsComplete((InteractionInstance)this))
                {
                    Actor.AddExitReason(ExitReason.StageComplete);
                }
            }

            Plant.PauseTendGardenInteractionStage(Actor.CurrentInteraction);
            if (Actor.HasExitReason(ExitReason.StageComplete))
            {
                HarvestPlantEx.DoHarvest(Target, Actor, hasHarvested, mBurglarSituation);
            }

            if (stateMachine != null)
            {
                stateMachine.RequestState("x", "Exit Standing");
            }

            EndCommodityUpdates(true);
            StandardExit();
            Plant.UpdateTendGardenTimeSpent(this, new Plant.UpdateTendGardenTimeSpentDelegate(HarvestPlant.Harvest.SetHarvestTimeSpent));
            return Actor.HasExitReason(ExitReason.StageComplete);
        }

        public override bool Run()
        {
            try
            {
                bool previousInteractionSuccessful = false;
                if (Target.RouteSimToMeAndCheckInUse(Actor) && HarvestPlant.HarvestTest(Target, Actor))
                {
                    ConfigureInteraction();
                    Plant.TryConfigureTendGardenInteraction(Actor.CurrentInteraction);
                    previousInteractionSuccessful = DoHarvestEx();
                }
                if (base.IsChainingPermitted(previousInteractionSuccessful))
                {
                    IgnorePlants.Add(Target);
                    if ((Target.LotCurrent != null) && (Target.LotCurrent.IsWorldLot))
                    {
                        PushNextInteractionInChain(Singleton, HarvestPlant.HarvestTestWorldLot, Target.LotCurrent);
                        return previousInteractionSuccessful;
                    }
                    PushNextInteractionInChain(Singleton, HarvestPlant.HarvestTest, Target.LotCurrent);
                }
                return previousInteractionSuccessful;
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

        public new class Definition : HarvestPlant.Harvest.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new HarvestEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, HarvestPlant target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}