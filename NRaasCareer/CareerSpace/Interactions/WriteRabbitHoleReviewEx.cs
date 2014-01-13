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
    public class WriteRabbitHoleReviewEx : Computer.WriteRabbitHoleReview, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Computer, Computer.WriteRabbitHoleReview.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Computer, Computer.WriteRabbitHoleReview.Definition>(Singleton);
        }

        public new void LoopDel(StateMachineClient smc, Interaction<Sim, Computer>.LoopData loopData)
        {
            Journalism job = OmniCareer.Career<Journalism>(Actor.Occupation);

            Definition def = InteractionDefinition as Definition;
            if (job.UpdateReview(def.Review))
            {
                Actor.AddExitReason(ExitReason.Finished);
            }
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
                Target.StartVideo(Computer.VideoType.WordProcessor);

                BeginCommodityUpdates();

                bool succeeded = false;

                try
                {
                    Definition def = InteractionDefinition as Definition;
                    def.Review.IsReviewNegative = def.IsNegative;
                    ProgressMeter.ShowProgressMeter(Actor, def.Review.ReviewCompletion / 100f, ProgressMeter.GlowType.Weak);
                    AnimateSim("WorkTyping");
                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), new Interaction<Sim, Computer>.InsideLoopFunction(LoopDel), null, 1f);
                    ProgressMeter.HideProgressMeter(Actor, succeeded);
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
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

        public new class Definition : Computer.WriteRabbitHoleReview.Definition
        {
            public Definition()
            { }
            public Definition(string text, string[] path, Journalism.ReviewedRabbitHole review, bool isNegative)
                : base(text, path, review, isNegative)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new WriteRabbitHoleReviewEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Computer target, List<InteractionObjectPair> results)
            {
                Journalism job = OmniCareer.Career<Journalism>(actor.Occupation);

                if ((target.IsComputerUsable(actor, true, false, false) && (job != null)) && job.CanWriteReview())
                {
                    foreach (Journalism.ReviewedRabbitHole hole in job.RabbitHolesReviewed)
                    {
                        if ((hole.ReviewCompletion == 0f) || ((hole.ReviewCompletion < 100f) && !hole.IsReviewNegative))
                        {
                            string[] path = new string[] { Computer.LocalizeString("Writing", new object[0x0]), Computer.LocalizeString("GoodReview", new object[0x0]) };
                            results.Add(new InteractionObjectPair(new Definition(job.GetLocalizedEventName(hole.EventWatched), path, hole, false), iop.Target));
                        }

                        if ((hole.ReviewCompletion == 0f) || ((hole.ReviewCompletion < 100f) && hole.IsReviewNegative))
                        {
                            string[] strArray2 = new string[] { Computer.LocalizeString("Writing", new object[0x0]), Computer.LocalizeString("BadReview", new object[0x0]) };
                            results.Add(new InteractionObjectPair(new Definition(job.GetLocalizedEventName(hole.EventWatched), strArray2, hole, true), iop.Target));
                        }
                    }
                }
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                Journalism journalism = OmniCareer.Career<Journalism>(a.Occupation);

                return ((target.IsComputerUsable(a, true, false, isAutonomous) && (journalism != null)) && journalism.CanWriteReview());
            }
        }
    }
}
