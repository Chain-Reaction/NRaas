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
    public class WriteReport : Computer.WriteReport, Common.IPreLoad, Common.IAddInteraction
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Computer, Computer.WriteReport.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
        }

        public override bool Run()
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
                Definition interactionDefinition = InteractionDefinition as Definition;

                mJob = Actor.OccupationAsCareer;

                if (interactionDefinition.Scrap)
                {
                    AnimateSim("WorkTyping");
                    string name = (OmniCareer.Career<Journalism>(mJob) != null) ? "ScrapStoryWarning" : "ScrapReportWarning";
                    if (AcceptCancelDialog.Show(Computer.LocalizeString(name, new object[] { mJob.GetCurrentReportSubject() })))
                    {
                        base.AnimateSim("WorkTyping");
                        mJob.SetReportSubject(null);
                    }
                    EndCommodityUpdates(true);
                    Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                    StandardExit();
                    return true;
                }
                if (!interactionDefinition.IsContinuing)
                {
                    Sim selectedObject = base.GetSelectedObject() as Sim;
                    mJob.SetReportSubject(selectedObject.SimDescription);
                    Journalism job = OmniCareer.Career<Journalism>(mJob);
                    if (job != null)
                    {
                        job.IsStoryNegative = interactionDefinition.IsNegativeReport;
                    }
                }
                ProgressMeter.ShowProgressMeter(Actor, mJob.GetCurrentReportCompletion() / 100f, ProgressMeter.GlowType.Weak);
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

        // Nested Types
        public new class Definition : InteractionDefinition<Sim, Computer, WriteReport>
        {
            // Fields
            public bool IsContinuing;
            public bool IsNegativeReport;
            public string[] MenuPath;
            public string MenuText;
            public bool Scrap;

            // Methods
            public Definition()
            { }
            public Definition(string text, string path, bool isNegativeReport, bool isContinuing, bool scrap)
            {
                MenuText = text;
                MenuPath = new string[] { path };
                IsNegativeReport = isNegativeReport;
                IsContinuing = isContinuing;
                Scrap = scrap;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Computer target, List<InteractionObjectPair> results)
            {
                Journalism journalism = OmniCareer.Career<Journalism>(actor.Occupation);
                LawEnforcement enforcement = OmniCareer.Career<LawEnforcement>(actor.Occupation);
                if (journalism != null)
                {
                    if (journalism.StorySubject != null)
                    {
                        results.Add(new InteractionObjectPair(new Definition(Computer.LocalizeString("ContinueStory", new object[] { journalism.StorySubject }), Computer.LocalizeString("Writing", new object[0x0]), true, true, false), iop.Target));
                        results.Add(new InteractionObjectPair(new Definition(Computer.LocalizeString("ScrapStory", new object[] { journalism.StorySubject }), Computer.LocalizeString("Writing", new object[0x0]), true, false, true), iop.Target));
                    }
                    else
                    {
                        if (journalism.ValidNegativeReportSubjects().Count > 0x0)
                        {
                            results.Add(new InteractionObjectPair(new Definition(Computer.LocalizeString("NegativeNewsStory", new object[0x0]), Computer.LocalizeString("Writing", new object[0x0]), true, false, false), iop.Target));
                        }

                        if (journalism.ValidPositiveReportSubjects().Count > 0x0)
                        {
                            results.Add(new InteractionObjectPair(new Definition(Computer.LocalizeString("PositiveNewsStory", new object[0x0]), Computer.LocalizeString("Writing", new object[0x0]), false, false, false), iop.Target));
                        }
                    }
                }
                
                if ((enforcement != null) && (enforcement.ValidPositiveReportSubjects().Count > 0x0))
                {
                    if (enforcement.ReportSubject != null)
                    {
                        results.Add(new InteractionObjectPair(new Definition(Computer.LocalizeString("ContinueReport", new object[] { enforcement.ReportSubject }), Computer.LocalizeString("Writing", new object[0x0]), true, true, false), iop.Target));
                        results.Add(new InteractionObjectPair(new Definition(Computer.LocalizeString("ScrapReport", new object[] { enforcement.ReportSubject }), Computer.LocalizeString("Writing", new object[0x0]), true, false, true), iop.Target));
                    }
                    else
                    {
                        results.Add(new InteractionObjectPair(new Definition(Computer.LocalizeString("LawEnforcementReport", new object[0x0]), Computer.LocalizeString("Writing", new object[0x0]), false, false, false), iop.Target));
                    }
                }
            }

            public override string GetInteractionName(Sim a, Computer target, InteractionObjectPair interaction)
            {
                return MenuText;
            }

            public override string[] GetPath(bool isFemale)
            {
                return MenuPath;
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                Definition interactionDefinition = parameters.InteractionDefinition as Definition;
                if (interactionDefinition.Scrap || interactionDefinition.IsContinuing)
                {
                    listObjs = null;
                    headers = null;
                    NumSelectableRows = 0x0;
                }
                else
                {
                    NumSelectableRows = 0x1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();
                    if (interactionDefinition.IsNegativeReport)
                    {
                        foreach (SimDescription description in actor.OccupationAsCareer.ValidNegativeReportSubjects())
                        {
                            if (description.CreatedSim != null)
                            {
                                sims.Add(description.CreatedSim);
                            }
                        }
                    }
                    else
                    {
                        foreach (SimDescription description2 in actor.OccupationAsCareer.ValidPositiveReportSubjects())
                        {
                            if (description2.CreatedSim != null)
                            {
                                sims.Add(description2.CreatedSim);
                            }
                        }
                    }
                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, false);
                }
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!(a.Occupation is OmniCareer)) return false;

                return (((target.IsComputerUsable(a, true, false, isAutonomous) && (a.OccupationAsCareer != null)) && a.OccupationAsCareer.CanWriteReport()) && a.OccupationAsCareer.CanInterview());
            }
        }
    }
}
