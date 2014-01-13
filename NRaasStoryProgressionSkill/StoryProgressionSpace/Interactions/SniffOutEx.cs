using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
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
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class SniffOutEx : Terrain.SniffOut, Common.IPreLoad, IFormattedStoryScenario
    {
        static InteractionDefinition sOldSingleton;

        int mInitialSkill = -1;

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.Inject<Terrain, Terrain.SniffOut.SniffOutDefinition, Definition>(false);
            if (tuning != null)
            {
                tuning.Availability.SkillThresholdValue = 0;
            }

            sOldSingleton = SniffOutSingleton;
            SniffOutSingleton = new Definition();
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Skills;
        }

        public string UnlocalizedName
        {
            get { return "SniffOut"; }
        }

        public static bool GetHuntableAndAddToInventory(DogHuntingSkill ths)
        {
            IGameObject huntable = null;
            try
            {
                huntable = ths.GetHuntable();
            }
            catch (Exception e)
            {
                Common.DebugException(ths.SkillOwner, e);
            }

            return AddHuntableToInventory(ths, huntable);
        }

        public static bool AddHuntableToInventory(DogHuntingSkill ths, IGameObject huntable)
        {
            if (huntable == null)
            {
                return false;
            }

            bool flag = false;

            if (SimTypes.IsSelectable(ths.SkillOwner))
            {
                flag = Inventories.TryToMove(huntable, ths.SkillOwner.CreatedSim);
            }
            else
            {
                // Add the item to head of family
                SimDescription head = SimTypes.HeadOfFamily(ths.SkillOwner.Household);
                if (head != null)
                {
                    flag = Inventories.TryToMove(huntable, head.CreatedSim);
                }
            }

            if (!flag)
            {
                huntable.RemoveFromWorld();
                huntable.Destroy();
            }
            else
            {
                StoryProgression.Main.Skills.Notify("SniffOut", ths.SkillOwner.CreatedSim, Common.Localize("SniffOut:Success", ths.SkillOwner.IsFemale, new object[] { ths.SkillOwner, huntable.GetLocalizedName() }));
            }

            return flag;
        }

        public override void SniffLoop(StateMachineClient smc, InteractionInstance.LoopData data)
        {
            try
            {
                if (mInitialSkill == -1)
                {
                    mInitialSkill = Actor.SkillManager.GetSkillLevel(SkillNames.DogHunting);
                }
                else if (mInitialSkill == Actor.SkillManager.GetSkillLevel(SkillNames.DogHunting))
                {
                    base.SniffLoop(smc, data);
                }
                else
                {
                    Actor.AddExitReason(ExitReason.RouteFailed);                    
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

        public override void RunPostLoopingBehavior()
        {
            try
            {
                if (DigUpSpot())
                {
                    GetHuntableAndAddToInventory(mSkill);
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

        private class Definition : Terrain.SniffOut.SniffOutDefinition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SniffOutEx();
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
                    if (a.SkillManager.GetSkillLevel(SkillNames.DogHunting) < 1)
                    {
                        greyedOutTooltipCallback = Common.DebugTooltip("Skill Too Low");
                        return false;
                    }
                }

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}

