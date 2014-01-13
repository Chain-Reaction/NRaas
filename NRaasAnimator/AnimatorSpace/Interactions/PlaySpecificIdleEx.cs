using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.AnimatorSpace.Interactions;
using NRaas.AnimatorSpace.Tones;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.SACS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.AnimatorSpace.Interactions
{
    public class PlaySpecificIdleEx : LoopingAnimationBase
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override string GetInteractionName()
        {
            IdleInfo idle = (InteractionDefinition as Definition).Idle;

            string animationName = idle.AnimationName;
            if (string.IsNullOrEmpty(animationName))
            {
                animationName = idle.CustomJazzGraph;
            }
            if (string.IsNullOrEmpty(animationName))
            {
                animationName = idle.SeatedAnimationName;
            }
            return (animationName);
        }

        public override bool Run()
        {
            try
            {
                IdleInfo info = (InteractionDefinition as Definition).Idle;
                bool flag = Actor.Posture.Satisfies(CommodityKind.Sitting, null);
                if (!string.IsNullOrEmpty(info.CustomJazzGraph))
                {
                    while (!Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                    {
                        if (!AnimationTone.ControlLoop(this)) break;

                        Sim.CustomIdle idle = Sim.CustomIdle.Singleton.CreateInstance(Actor, Actor, GetPriority(), true, true) as Sim.CustomIdle;
                        idle.Hidden = true;
                        idle.JazzGraphName = info.CustomJazzGraph;
                        idle.LoopTimes = info.PickCustomJazzGraphLoopCount();
                        idle.ExtraWaitTime = info.CasOnly ? 0x78 : 0x0;
                        if (idle.JazzGraphName == "TraitWorkaholic")
                        {
                            idle.IdleObject = Actor.Inventory.Find<IPhoneCell>() as GameObject;
                            idle.ObjectName = "phonecell";
                        }
                        idle.RunInteraction();
                    }
                }
                else
                {
                    string animationName = null;
                    if (flag)
                    {
                        animationName = info.SeatedAnimationName;
                    }
                    if (string.IsNullOrEmpty(animationName))
                    {
                        animationName = info.AnimationName;
                    }

                    while (!Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                    {
                        if (!AnimationTone.ControlLoop(this)) break;

                        Actor.PlaySoloAnimation(animationName, true, info.ProductVersion);
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
            return false;
        }

        [DoesntRequireTuning]
        public class Definition : InteractionDefinition<Sim, Sim, PlaySpecificIdleEx>, IAnimationDefinition
        {
            public IdleInfo Idle;
            private string mInteractionName;
            public CommodityKind Posture;
            public CommodityKind[] PostureChecks;

            string[] mPath;

            public Definition()
            {
                Posture = CommodityKind.Standing;
                PostureChecks = new CommodityKind[0x0];
            }
            public Definition(Sim.PlaySpecificIdle.Definition definition)
            {
                Posture = definition.Posture;
                PostureChecks = definition.PostureChecks;
                Idle = definition.Idle;
                mInteractionName = definition.InteractionName;

                if (string.IsNullOrEmpty(mInteractionName))
                {
                    mInteractionName = Idle.AnimationName;
                }

                List<string> path = new List<string> (definition.MenuPath);
                path.RemoveAt(0);

                mPath = path.ToArray();
            }

            public string InteractionName
            {
                get { return mInteractionName; }
            }

            public string ClipName
            {
                get { return Idle.AnimationName; }
            }

            public string Type
            {
                get 
                {
                    List<string> path = new List<string>(GetPath(false));

                    string type = null;
                    if (path.Count > 0)
                    {
                        type = path[path.Count - 1].Replace(" Idles", "").Replace("...", "");
                    }

                    return type; 
                }
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance interaction = base.CreateInstance(ref parameters);
                if (Posture != CommodityKind.Standing)
                {
                    ChildUtils.SetPosturePrecondition(interaction, Posture, PostureChecks);
                    return interaction;
                }
                if (parameters.Actor.Posture.Satisfies(CommodityKind.CarryingChild, null))
                {
                    ChildUtils.SetPosturePrecondition(interaction, CommodityKind.CarryingChild, PostureChecks);
                    return interaction;
                }
                if (parameters.Actor.Posture is SimCarryingObjectPosture)
                {
                    ChildUtils.SetPosturePrecondition(interaction, CommodityKind.CarryingObject, new CommodityKind[] { CommodityKind.CarryingObject });
                }
                return interaction;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return (InteractionName + (Idle.CasOnly ? " (CAS)" : ""));
            }

            public override string[] GetPath(bool isFemale)
            {
                return mPath;
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }

            public override string ToString()
            {
                string text = "AnimationName: " + Idle.AnimationName;
                text += Common.NewLine + "InteractionName: " + InteractionName;

                return text;
            }
        }
    }
}