using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class TestMenuInteractions : DebugEnablerInteraction<GameObject>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            list.Add(new InteractionObjectPair(Singleton, obj));
        }

        public override bool Run()
        {
            bool pieMenuShowFailureReason = PieMenu.PieMenuShowFailureReason;

            try
            {
                PieMenu.PieMenuShowFailureReason = true;

                Common.StringBuilder msg = new Common.StringBuilder();

                Common.StringBuilder unused = new Common.StringBuilder();
                Common.ExceptionLogger.Convert(Actor, unused, msg);
                Common.ExceptionLogger.Convert(Target, unused, msg);

                msg += Common.NewLine + Common.NewLine + "Injected Interactions" + Common.NewLine;

                foreach (InteractionObjectPair interaction in Target.mInteractions)
                {
                    Common.TestSpan span = Common.TestSpan.CreateSimple ();
                    try
                    {
                        msg += Common.NewLine + interaction.InteractionDefinition.GetType();
                        msg += Common.NewLine + " " + interaction.InteractionDefinition.GetType().Assembly.FullName;

                        InteractionInstanceParameters parameters = new InteractionInstanceParameters(interaction, Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), true, true);

                        try
                        {
                            msg += Common.NewLine + " " + interaction.InteractionDefinition.GetInteractionName(ref parameters);
                        }
                        catch
                        {
                            msg += Common.NewLine + " (Exception)";
                        }
                    }
                    finally
                    {
                        long duration = span.Duration;
                        if (duration > 1)
                        {
                            msg += Common.NewLine + " Duration: " + duration;
                        }
                    }
                }

                msg += Common.NewLine + Common.NewLine + "All Interactions" + Common.NewLine;

                List<InteractionObjectPair> interactions = null;

                Common.TestSpan interactionSpan = Common.TestSpan.CreateSimple();
                try
                {
                    interactions = Target.GetAllInteractionsForActor(Actor);
                    interactions.AddRange(Target.GetAllInventoryInteractionsForActor(Actor));
                }
                finally
                {
                    long duration = interactionSpan.Duration;
                    if (duration > 1)
                    {
                        msg += Common.NewLine + "All Interactions Duration: " + duration;
                    }
                }

                foreach (InteractionObjectPair interaction in interactions)
                {
                    Common.TestSpan span = Common.TestSpan.CreateSimple();
                    try
                    {
                        msg += Common.NewLine + interaction.InteractionDefinition.GetType();
                        msg += Common.NewLine + " " + interaction.InteractionDefinition.GetType().Assembly.FullName;

                        GreyedOutTooltipCallback callback = null;

                        InteractionInstanceParameters userDirected = new InteractionInstanceParameters(interaction, Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);

                        InteractionTestResult result = InteractionTestResult.GenericUnknown;

                        try
                        {
                            result = interaction.InteractionDefinition.Test(ref userDirected, ref callback);
                        }
                        catch (Exception e)
                        {
                            msg += Common.NewLine + e.ToString();
                        }

                        try
                        {
                            msg += Common.NewLine + " " + interaction.InteractionDefinition.GetInteractionName(ref userDirected);

                            SocialInteractionA.Definition socialInteractionA = interaction.InteractionDefinition as SocialInteractionA.Definition;
                            if (socialInteractionA != null)
                            {
                                msg += Common.NewLine + "  ActionKey=" + socialInteractionA.ActionKey;
                                msg += Common.NewLine + "  ChecksToSkip=" + socialInteractionA.ChecksToSkip;
                                msg += Common.NewLine + "  mIsInitialGreeting=" + socialInteractionA.mIsInitialGreeting;
                                msg += Common.NewLine + "  mTrait=" + socialInteractionA.mTrait;
                            }
                        }
                        catch (Exception e)
                        {
                            // Only dispaly GetInteractionName() errors if that call is actually used by EA
                            if (IUtil.IsPass(result))
                            {
                                msg += Common.NewLine + e.ToString();
                            }
                            else
                            {
                                msg += Common.NewLine + " (Exception)";
                            }
                        }

                        msg += Common.NewLine + " User Directed = " + result;

                        if (callback != null)
                        {
                            msg += Common.NewLine + "  Tooltip: " + callback();
                        }

                        callback = null;

                        InteractionInstanceParameters autonomous = new InteractionInstanceParameters(interaction, Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), true, true);

                        result = InteractionTestResult.GenericUnknown;
                        try
                        {
                            result = interaction.InteractionDefinition.Test(ref autonomous, ref callback);
                        }
                        catch (Exception e)
                        {
                            msg += Common.NewLine + e.ToString();
                        }

                        msg += Common.NewLine + " Autonomous = " + result;
                        if (callback != null)
                        {
                            msg += Common.NewLine + "  Tooltip: " + callback();
                        }
                    }
                    finally
                    {
                        long duration = span.Duration;
                        if (duration > 1)
                        {
                            msg += Common.NewLine + " Duration: " + duration;
                        }
                    }
                }

                Common.WriteLog(msg);

                SimpleMessageDialog.Show(Common.Localize("TestInteractions:MenuName"), Common.Localize("TestInteractions:Success"));
            }
            finally
            {
                PieMenu.PieMenuShowFailureReason = pieMenuShowFailureReason;
            }

            return true;
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<TestMenuInteractions>
        {
            public override string GetInteractionName(IActor a, GameObject target, InteractionObjectPair interaction)
            {
                return Common.Localize("TestInteractions:MenuName");
            }
        }
    }
}
