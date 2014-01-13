using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace.Metrics;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class RunForensicAnalysis : Computer.RunForensicAnalysis, Common.IPreLoad, Common.IAddInteraction
    {
        // Fields
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Computer, Computer.RunForensicAnalysis.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
        }

        // Methods
        public override void ConfigureInteraction()
        {
            LawEnforcement job = OmniCareer.Career<LawEnforcement>(base.Actor.Occupation);
            if (job == null) return;

            TimedStage stage = new TimedStage(this.GetInteractionName(), Computer.RunForensicAnalysis.kInteractionStageLength - job.MinutesSpentDoingForensicAnalysis, false, true, true);
            base.Stages = new List<Stage>(new Stage[] { stage });
        }

        public static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, "RunForensicAnalysis:" + name, "Gameplay/Objects/Electronics/Computer/RunForensicAnalysis:" + name, parameters);
        }

        public override bool Run()
        {
            try
            {
                StandardEntry();
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    StandardExit();
                    return false;
                }
                DateAndTime firstDate = SimClock.CurrentTime();
                Target.StartVideo(Computer.VideoType.WordProcessor);

                StartStages();
                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    AnimateSim("WorkTyping");
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                LawEnforcement job = OmniCareer.Career<LawEnforcement>(Actor.Occupation);

                float minutes = SimClock.ElapsedTime(TimeUnit.Minutes, firstDate, SimClock.CurrentTime());
                job.UpdateTimeSpentOnForensicAnalysis(minutes);
                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                if (job.IsForensicAnalysisComplete(Computer.RunForensicAnalysis.kTotalTimeToFinishAnalysis))
                {
                    job.ResetForensicAnalysisStatistics();
                    Actor.ModifyFunds(Computer.RunForensicAnalysis.kRewardOnFinishingAnalysis);
                    Actor.ShowTNSIfSelectable(LocalizeString(Actor.SimDescription, "AnalysisComplete", new object[] { Actor, Computer.RunForensicAnalysis.kRewardOnFinishingAnalysis }), StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid, Actor.ObjectId);
                }

                StandardExit();
                return true;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        // Nested Types
        public new class Definition : InteractionDefinition<Sim, Computer, RunForensicAnalysis>
        {
            // Methods
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                LawEnforcement job = OmniCareer.Career<LawEnforcement>(actor.Occupation);
                if ((job != null) && (!job.IsCurrentlyRunningforensicAnalysis))
                {
                    return RunForensicAnalysis.LocalizeString(actor.SimDescription, "InteractionName", new object[0x0]);
                }
                return RunForensicAnalysis.LocalizeString(actor.SimDescription, "ContinueAnalysis", new object[0x0]);
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.IsComputerUsable(a, true, false, isAutonomous))
                {
                    return false;
                }

                if (!(a.Occupation is OmniCareer)) return false;

                LawEnforcement job = OmniCareer.Career<LawEnforcement>(a.Occupation);

                return ((job != null) && (job.CurLevel.PromotionRewardData == "UnlockComputerInteraction"));
            }
        }
    }
}
