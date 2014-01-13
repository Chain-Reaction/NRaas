using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    [Persistable]
    public class CatFishHereEx : Terrain.CatFishHere, Common.IPreLoad, Common.IAddInteraction, IFormattedStoryScenario
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Terrain, Terrain.CatFishHere.Definition, Definition>(true);
            if (tuning != null)
            {
                tuning.Availability.SkillThresholdValue = 0;
            }

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Terrain, Terrain.CatFishHere.Definition>(Singleton);
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Skills;
        }

        public string UnlocalizedName
        {
            get { return "CatFishHere"; }
        }

        public override bool Run()
        {
            try
            {
                CatHuntingSkill skill = Actor.SkillManager.GetSkill<CatHuntingSkill>(SkillNames.CatHunting);
                if (skill == null)
                {
                    skill = Actor.SkillManager.AddElement(SkillNames.CatHunting) as CatHuntingSkill;
                    if (skill == null)
                    {
                        return false;
                    }
                }
                if (!Terrain.DrinkFromPondHelper.RouteToDrinkLocation(Hit.mPoint, Actor, Hit.mType, Hit.mId))
                {
                    return false;
                }

                StandardEntry();
                EnterStateMachine("CatHuntInPond", "Enter", "x");
                AddOneShotScriptEventHandler(0x65, new SacsEventHandler(SnapOnExit));
                BeginCommodityUpdates();
                AnimateSim("PrePounceLoop");

                bool succeeded = false;

                if (skill.SkillLevel < 1)
                {
                    succeeded = DoTimedLoop(RandomUtil.GetFloat(30, 60));
                }
                else
                {
                    succeeded = DoTimedLoop(RandomUtil.GetFloat(kMinMaxPrePounceTime[0x0], kMinMaxPrePounceTime[0x1]));
                }

                if (succeeded)
                {
                    EventTracker.SendEvent(EventTypeId.kGoFishingCat, Actor);
                    AnimateSim("FishLoop");
                    succeeded = RandomUtil.InterpolatedChance(0f, (float)skill.MaxSkillLevel, kMinMaxSuccesChance[0x0], kMinMaxSuccesChance[0x1], (float)skill.SkillLevel);
                    if (succeeded)
                    {
                        FishType type = FishType.None;
                        try
                        {
                            // This function bounces if there are no fish available at the sim's skill level
                            type = GetCaughtFishType();
                        }
                        catch
                        { }

                        if (type != FishType.None)
                        {

                            Fish prey = Fish.CreateFishOfRandomWeight(type, Actor.SimDescription);
                            skill.RegisterCaughtPrey(prey);
                            if (prey.CatHuntingComponent != null)
                            {
                                prey.CatHuntingComponent.SetCatcher(Actor);
                            }
                            prey.UpdateVisualState(CatHuntingComponent.CatHuntingModelState.Carried);
                            SetActor("fish", prey);
                            if (Actor.Motives.GetValue(CommodityKind.Hunger) <= kEatFishHungerThreshold)
                            {
                                string message = Localization.LocalizeString("Gameplay/Abstracts/ScriptObject/CatFishHere:EatFishTns", new object[] { Actor, prey.GetLocalizedName(), prey.Weight });
                                Actor.ShowTNSIfSelectable(message, StyledNotification.NotificationStyle.kGameMessagePositive);
                                AnimateSim("ExitEat");
                                prey.Destroy();
                                Actor.Motives.ChangeValue(CommodityKind.Hunger, kHungerGainFromEating);
                            }
                            else
                            {
                                string str2 = Localization.LocalizeString("Gameplay/Abstracts/ScriptObject/CatFishHere:PutFishInInventoryTns", new object[] { Actor, prey.GetLocalizedName(), prey.Weight });

                                // Custom
                                StoryProgression.Main.Skills.Notify("CatFishHere", Actor, str2);

                                AnimateSim("ExitInventory");
                                prey.UpdateVisualState(CatHuntingComponent.CatHuntingModelState.InInventory);

                                Sim inventoryActor = Actor;

                                if (!SimTypes.IsSelectable(Actor))
                                {
                                    SimDescription head = SimTypes.HeadOfFamily(Actor.Household);
                                    if ((head != null) && (head.CreatedSim != null))
                                    {
                                        inventoryActor = head.CreatedSim;
                                    }
                                }

                                // Custom
                                if (!inventoryActor.Inventory.TryToAdd(prey, false))
                                {
                                    prey.Destroy();
                                }
                            }
                        }
                        else
                        {
                            AnimateSim("ExitFailure");
                        }
                    }
                    else
                    {
                        AnimateSim("ExitFailure");
                    }
                }
                else
                {
                    AnimateSim("ExitPrePounce");
                }
                EndCommodityUpdates(succeeded);
                StandardExit();
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

        public new class Definition : Terrain.CatFishHere.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CatFishHereEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Terrain target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Terrain target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (SimTypes.IsSelectable(a))
                {
                    if (a.SkillManager.GetSkillLevel(SkillNames.CatHunting) < 1)
                    {
                        return false;
                    }

                    CatHuntingSkill skill = a.SkillManager.GetSkill<CatHuntingSkill>(SkillNames.CatHunting);
                    if ((skill == null) || (!skill.CanCatchPreyOfType(CatHuntingSkill.PreyType.Fish)))
                    {
                        return false;
                    }
                }

                if (!a.IsCat) return false;
                
                return PetManager.PetSkillFatigueTest(a, ref greyedOutTooltipCallback);
            }
        }
    }
}

