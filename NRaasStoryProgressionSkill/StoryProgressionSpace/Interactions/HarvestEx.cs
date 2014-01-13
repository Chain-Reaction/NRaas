using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class HarvestEx : HarvestPlant.Harvest, Common.IPreLoad
    {
        public static readonly new InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<HarvestPlant, HarvestPlant.Harvest.Definition, Definition>(false);
        }

        // From HarvestPlant
        protected static void UpdateGardeningSkillJournal(HarvestPlant ths, Gardening gardeningSkill, PlantDefinition harvestPlantDef, List<GameObject> objectsHarvested)
        {
            foreach (GameObject obj2 in objectsHarvested)
            {
                gardeningSkill.Harvested(Plant.GetQuality(obj2.Plantable.QualityLevel), harvestPlantDef);
                if (Plant.IsMushroom(ths.Seed))
                {
                    Collecting collecting = gardeningSkill.SkillOwner.SkillManager.AddElement(SkillNames.Collecting) as Collecting;
                    if (collecting != null)
                    {
                        bool flag;
                        collecting.Collected(obj2 as Ingredient, out flag);
                    }
                }
            }
        }

        private new bool DoHarvest()
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
                foreach (SimDescription description in Households.All(Actor.Household))
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
                DoHarvest(Target, Actor, hasHarvested, this.mBurglarSituation);
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

        private static bool DoHarvest(HarvestPlant ths, Sim actor, bool hasHarvested, BurglarSituation burglarSituation)
        {
            Slot[] containmentSlots = ths.GetContainmentSlots();
            List<GameObject> objectsHarvested = new List<GameObject>();
            foreach (Slot slot in containmentSlots)
            {
                GameObject containedObject = ths.GetContainedObject(slot) as GameObject;
                if ((containedObject != null) && HarvestHarvestable(ths, containedObject, actor, burglarSituation))
                {
                    objectsHarvested.Add(containedObject);
                }
            }

            if (actor.TraitManager.HasElement(TraitNames.GathererTrait) && RandomUtil.RandomChance01(TraitTuning.GathererTraitExtraHarvestablesChance))
            {
                int gathererTraitNumberOfExtraHarvestables = TraitTuning.GathererTraitNumberOfExtraHarvestables;
                for (int i = 0; i < gathererTraitNumberOfExtraHarvestables; i++)
                {
                    GameObject item = ths.Seed.Copy(false) as GameObject;
                    if (item != null)
                    {
                        objectsHarvested.Add(item);
                    }
                }
            }

            if (objectsHarvested.Count <= 0x0)
            {
                return false;
            }
            Gardening skill = actor.SkillManager.GetSkill<Gardening>(SkillNames.Gardening);
            Collecting collecting = actor.SkillManager.GetSkill<Collecting>(SkillNames.Collecting);

            int skillDifficulty = ths.PlantDef.GetSkillDifficulty();
            if (skill != null)
            {
                if (skill.SkillLevel <= skillDifficulty)
                {
                    if (actor.SimDescription.IsFairy && skill.IsFairySkill())
                    {
                        skill.AddPoints(ths.PlantDef.SkillPointsHarvest * Skill.SkillLevelBumpMultiplierForFairies);
                    }
                    else
                    {
                        skill.AddPoints(ths.PlantDef.SkillPointsHarvest);
                    }
                }

                if ((ths is MoneyTree) || (ths is OmniPlant))
                {
                    ths.UpdateGardeningSkillJournal(skill, ths.PlantDef, objectsHarvested);
                }
                else
                {
                    // Custom
                    UpdateGardeningSkillJournal(ths, skill, ths.PlantDef, objectsHarvested);
                }
            }
            if (!hasHarvested)
            {
                actor.ShowTNSIfSelectable(Common.LocalizeEAString(actor.IsFemale, "Gameplay/Objects/Gardening/HarvestPlant/Harvest:FirstHarvest", new object[] { actor, ths.PlantDef.Name }), StyledNotification.NotificationStyle.kGameMessagePositive, ths.ObjectId, actor.ObjectId);
            }

            if (collecting == null)
            {
                collecting = (Collecting)actor.SkillManager.AddElement(SkillNames.Collecting);
            }
            collecting.CollectedFromHarvest(objectsHarvested);

            ths.PostHarvest();
            return true;
        }

        private static bool HarvestHarvestable(HarvestPlant ths, GameObject harvestable, Sim actor, BurglarSituation burglarSituation)
        {
            if (harvestable == null)
            {
                return false;
            }
            harvestable.SetOwnerLot(actor.LotHome);
            harvestable.UnParent();
            harvestable.RemoveFromWorld();
            bool flag = false;
            if (burglarSituation == null)
            {
                if (actor.Inventory.TryToAdd(harvestable, false))
                {
                    flag = true;
                }
            }
            else
            {
                burglarSituation.StolenObjects.Add(harvestable);
                burglarSituation.CurrentValue += harvestable.Value;
                flag = true;
            }
            if (flag)
            {
                harvestable.AddFlags(GameObject.FlagField.WasHarvested);
                harvestable.RemoveComponent<HarvestableComponent>();
                ths.PostHarvestHarvestable(actor, harvestable);
                return flag;
            }
            harvestable.Destroy();
            return flag;
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
                    previousInteractionSuccessful = DoHarvest();
                }
                if (IsChainingPermitted(previousInteractionSuccessful))
                {
                    IgnorePlants.Add(Target);
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
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new HarvestEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}

