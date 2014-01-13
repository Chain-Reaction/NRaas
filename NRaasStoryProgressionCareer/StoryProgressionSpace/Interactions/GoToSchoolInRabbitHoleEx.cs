using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class GoToSchoolInRabbitHoleEx : GoToSchoolInRabbitHole, Common.IPreLoad
    {
        public static readonly new InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, GoToSchoolInRabbitHole.Definition, Definition>(false);
        }

        protected void OnChangeOutfit()
        {
            bool useFormal = StoryProgression.Main.GetValue<UseForAllOption, bool>();
                
            if ((!StoryProgression.Main.HasValue<PublicAssignSchoolScenario.ConsiderPublicOption,OccupationNames>(Actor.School.Guid)) && (StoryProgression.Main.GetValue<Option, bool>()))
            {
                useFormal = true;
            }

            if (Actor.Posture == Actor.Standing)
            {
                if (useFormal)
                {
                    if (Actor.SimDescription.GetOutfitCount(OutfitCategories.Formalwear) > 1)
                    {
                        Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Formalwear, 1);
                    }
                    else
                    {
                        Actor.SwitchToOutfitWithoutSpin(OutfitCategories.Formalwear);
                    }
                }
                else
                {
                    Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToSchool);
                }
            }
            else if (Actor.LotCurrent != Target.LotCurrent)
            {
                OutfitCategories category = OutfitCategories.Formalwear;
                if (!useFormal)
                {
                    Actor.GetOutfitForClothingChange(Sim.ClothesChangeReason.GoingToSchool, out category);
                }

                Actor.OutfitCategoryToUseForRoutingOffLot = category;
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

        public override bool InRabbitHole()
        {
            try
            {
                if (!GoToSchoolInRabbitHoleHelper.PreInRabbitholeLoop(this)) return false;

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
            public Definition(Vehicle playerChosenVehicle)
                : base(playerChosenVehicle)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance instance = new GoToSchoolInRabbitHoleEx();
                instance.Init(ref parameters);
                return instance;
            }
        }

        public class Option : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.ISchoolOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "SchoolOutfit";
            }

            public override bool Install(ManagerCareer main, bool initial)
            {
                if (initial)
                {
                    CareerPushScenario.OnWorkInteraction += GetWorkInteraction;
                }

                return base.Install(main, initial);
            }

            public static InteractionInstance GetWorkInteraction(Career job)
            {
                if (job is School)
                {
                    return Singleton.CreateInstance(job.CareerLoc.Owner, job.OwnerDescription.CreatedSim, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true);
                }
                else
                {
                    return job.CreateWorkInteractionInstance();
                }
            }
        }

        public class UseForAllOption : BooleanManagerOptionItem<ManagerCareer>, ManagerCareer.ISchoolOption
        {
            public UseForAllOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "SchoolOutfitUseForAll";
            }
        }
    }
}

