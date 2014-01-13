using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.Objects.HobbiesSkills;
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
    public class PaintEx : Easel.Paint, Common.IPreLoad
    {
        public static readonly new InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Easel, Easel.Paint.Definition, Definition>(false);
        }

        public override void ConfigureInteraction()
        {
            mSize = (PaintingSkill.CanvasSize)RandomUtil.GetInt(0x0, 0x2);

            base.ConfigureInteraction();
        }

        public override bool Run()
        {
            try
            {
                if (!UIUtils.IsOkayToStartModalDialog())
                {
                    return false;
                }
                if (!Target.RouteToEasel(Actor))
                {
                    return false;
                }
                Actor.SkillManager.AddElement(SkillNames.Painting);
                StandardEntry();
                Actor.LookAtManager.DisableLookAts();

                bool succeeded = true;
                if (InteractionDefinition is Definition)
                {
                    succeeded = Target.StartPainting(Actor, mSize, Style, false);
                    if (succeeded)
                    {
                        SimpleStage stage = new SimpleStage(GetInteractionName(), Target.GetPaintTimeRemaining(Actor), new SimpleStage.CompletionTest(PaintingProgressTest), false, true, true);
                        Stages = new List<Stage>(new Stage[] { stage });
                    }
                }

                if (!succeeded)
                {
                    StandardExit();
                    Actor.AddExitReason(ExitReason.FailedToStart);
                    return false;
                }

                EnterStateMachine("Easel", "Enter", "x", "easel");
                SetParameter("Canvas", Target.CurrentCanvas.AnimationSize);
                SetActor("canvas", Target.CurrentCanvas);
                if (Actor.SimDescription.Child)
                {
                    mStool = GlobalFunctions.CreateObjectOutOfWorld("ChildStool") as GameObject;
                    SetActor("childstool", mStool);
                }

                AnimateSim("Loop Paint");
                PaintingSkill element = Actor.SkillManager.GetElement(SkillNames.Painting) as PaintingSkill;
                if (Actor.HasTrait(TraitNames.ExtraCreative) || ((element != null) && ((element.IsBrushmaster || element.IsProficientPainter) || element.IsMasterPainter)))
                {
                    mSparkleEffect = VisualEffect.Create("masterpiece");
                    mSparkleEffect.ParentTo(Actor, Sim.ContainmentSlots.RightHand);
                    mSparkleEffect.Start();
                }

                BeginCommodityUpdates();

                try
                {
                    StartStages();
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, Easel>.InsideLoopFunction(LoopHandler), mCurrentStateMachine);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);

                    if (Target.CurrentCanvas != null)
                    {
                        Target.StopPainting(Actor);
                    }
                }

                if (Target.IsPaintingComplete())
                {
                    if (Actor.HasTrait(TraitNames.Perfectionist))
                    {
                        Actor.PlayReaction(ReactionTypes.Mwah, ReactionSpeed.AfterInteraction);
                    }
                    EventTracker.SendEvent(EventTypeId.kFinishedPainting, Actor, Target.CurrentCanvas);
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

        public new class Definition : Easel.Paint.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PaintEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}

