using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public sealed class SquishGrapes : NectarMaker.SquishGrapes, Common.IPreLoad
    {
        public static new readonly InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<NectarMaker, NectarMaker.SquishGrapes.Definition, Definition>(false);
        }

        public override bool Run()
        {
            try
            {
                if ((!Target.RouteToBarrelSlot(Actor) || ((Target.CurrentState != NectarMaker.NectarMakerState.FruitAddable) && (Target.CurrentState != NectarMaker.NectarMakerState.SquishingFruit))) || !Target.HasFruit)
                {
                    return false;
                }
                Target.CurrentState = NectarMaker.NectarMakerState.SquishingFruit;
                Actor.SkillManager.AddElement(SkillNames.Nectar);
                foreach (GameObject obj2 in Inventories.QuickFind<GameObject>(Target.Inventory))
                {
                    Target.Inventory.SetInUse(obj2);
                }
                Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToSwim);
                StandardEntry();
                BeginCommodityUpdates();
                AcquireStateMachine("NectarMaker");
                SetActorAndEnter("x", Actor, "Enter Squish Grapes");
                SetActor("nectarMaker", Target);
                AnimateSim("Squish Grapes Loop");
                StartStages();
                bool flag = false;

                try
                {
                    flag = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));

                    if (flag)
                    {
                        Target.CurrentState = NectarMaker.NectarMakerState.SquishedFruit;
                        Target.UpdateVisualState();
                        NectarSkill skill = Actor.SkillManager.GetSkill<NectarSkill>(SkillNames.Nectar);
                        if ((skill != null) && skill.IsFlavorfulFeet())
                        {
                            Target.mMultiplierFromFeet = NectarSkill.kFlavorfulFeetValueMultiplier;
                        }
                        if (skill != null)
                        {
                            skill.SquishGrapes();
                        }
                        Target.mSquishAmountRemaining = 1f;
                    }
                    else if (ActiveStage != null)
                    {
                        Target.mSquishAmountRemaining *= ActiveStage.TimeRemaining((InteractionInstance)this) / ActiveStage.TotalTime();
                    }
                    AnimateSim("Exit");
                }
                finally
                {
                    EndCommodityUpdates(true);
                    StandardExit();
                }

                Actor.SwitchToPreviousOutfitWithSpin();
                if ((this.mMakeStyleToPush != NectarMaker.MakeNectarStyle.None) && flag)
                {
                    Actor.InteractionQueue.PushAsContinuation(new MakeNectarEx.Definition(mMakeStyleToPush), Target, true);
                }
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

        // Nested Types
        public new class Definition : InteractionDefinition<Sim, NectarMaker, SquishGrapes>
        {
            public override bool Test(Sim a, NectarMaker target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return NectarMaker.SquishGrapes.Definition.SquishTest(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}

