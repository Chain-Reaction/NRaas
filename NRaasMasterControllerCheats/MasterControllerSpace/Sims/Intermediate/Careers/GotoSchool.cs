using NRaas.CommonSpace.Helpers;
using NRaas.MasterControllerSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class GotoSchool : CareerOption
    {
        public override string GetTitlePrefix()
        {
            return "GotoSchool";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CareerManager == null) return false;

            School career = me.CareerManager.School;
            if (career == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            return me.CreatedSim.InteractionQueue.Add(GoToSchoolInRabbitHoleEx.SingletonEx.CreateInstance(me.CareerManager.School.CareerLoc.Owner, me.CreatedSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true));
        }

        public class GoToSchoolInRabbitHoleEx : GoToSchoolInRabbitHole, Common.IPreLoad
        {
            public static readonly InteractionDefinition SingletonEx = new Definition();

            public void OnPreLoad()
            {
                Tunings.Inject<RabbitHole, GoToSchoolInRabbitHole.Definition, Definition>(false);
            }

            protected void OnChangeOutfit()
            {
                if (Actor.Posture == Actor.Standing)
                {
                    Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToSchool);
                }
                else if (Actor.LotCurrent != Target.LotCurrent)
                {
                    OutfitCategories categories;
                    Actor.GetOutfitForClothingChange(Sim.ClothesChangeReason.GoingToSchool, out categories);
                    Actor.OutfitCategoryToUseForRoutingOffLot = categories;
                }
            }

            public override bool RouteNearEntranceAndIntoBuilding(bool canUseCar, Route.RouteMetaType routeMetaType)
            {
                try
                {
                    GoToSchoolInRabbitHoleHelper.PreRouteNearEntranceAndIntoBuilding(this, canUseCar, routeMetaType, OnChangeOutfit);

                    return Target.RouteNearEntranceAndEnterRabbitHole(Actor, this, BeforeEnteringRabbitHole, canUseCar, routeMetaType, true);
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

            public override bool BeforeEnteringRabbitHole()
            {
                /*
                Career school = Actor.School;
                if (!school.IsAllowedToWork() && school.IsAllowedToWork())
                {
                    return school.WaitForWork(Actor, Target);
                }
                */
                return true;
            }

            public override void ConfigureInteraction()
            {
                try
                {
                    Career school = Actor.School;
                    school.SetTones(this);

                    StringDelegate delegate2;
                    if (((school.LastTone != null) && school.LastTone.ShouldAddTone(school)) && school.LastTone.Test(this, out delegate2))
                    {
                        CurrentTone = school.LastTone;
                    }

                    if (Actor.TraitManager.HasElement(TraitNames.Schmoozer))
                    {
                        foreach (CareerTone tone in school.Tones)
                        {
                            if (tone is SuckUpToBossTone)
                            {
                                CurrentTone = tone;
                                break;
                            }
                        }
                    }
                    float maxDurationInHours = (school.DayLength + Career.kNumHoursEarlyCanShowUpForWork) + Actor.School.SchoolTuning.DetentionLengthInHours;

                    float finishTime = (SimClock.Hours24 + school.DayLength) % 24;

                    WorkInRabbitHoleStage stage = new WorkInRabbitHoleStage(Common.LocalizeEAString(Actor.IsFemale, "Gameplay/Careers/WorkInRabbitHole:SchoolAttend"), finishTime, maxDurationInHours, school);
                    Stages = new List<Stage>(new Stage[] { stage });
                    ActiveStage = stage;
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

            public override bool InRabbitHole()
            {
                try
                {
                    if (!GoToSchoolInRabbitHoleHelper.PreInRabbitholeLoop(this, true)) return false;

                    bool succeeded = DoLoop(ExitReason.StageComplete, LoopDelegate, null);

                    AfterschoolActivity activity = null;
                    bool hasAfterschoolActivity = false;

                    bool detention = false;
                    bool fieldTrip = false;

                    GoToSchoolInRabbitHoleHelper.PostInRabbitHoleLoop(this, ref succeeded, ref detention, ref fieldTrip, ref activity, ref hasAfterschoolActivity);

                    if (detention && !fieldTrip)
                    {
                        succeeded = DoLoop(ExitReason.StageComplete, LoopDelegate, null);
                    }

                    InteractionInstance.InsideLoopFunction afterSchoolLoop = null;
                    GoToSchoolInRabbitHoleHelper.PostDetentionLoop(this, succeeded, detention, fieldTrip, activity, hasAfterschoolActivity, ref afterSchoolLoop);

                    if (afterSchoolLoop != null)
                    {
                        succeeded = DoLoop(ExitReason.StageComplete, afterSchoolLoop, mCurrentStateMachine);
                    }
                    else
                    {
                        succeeded = DoLoop(ExitReason.StageComplete);
                    }

                    GoToSchoolInRabbitHoleHelper.PostAfterSchoolLoop(this, succeeded, activity, afterSchoolLoop);

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

            public new class Definition : GoToSchoolInRabbitHole.Definition
            {
                public Definition()
                { }

                public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
                {
                    InteractionInstance na = new GoToSchoolInRabbitHoleEx();
                    na.Init(ref parameters);
                    return na;
                }

                public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
                {
                    return base.GetInteractionName(actor, target, new InteractionObjectPair(GoToSchoolInRabbitHole.Singleton, target));
                }

                public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
    }
}
