using NRaas.CommonSpace.Helpers;
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

namespace NRaas.CareerSpace.Interactions
{
    public class GoToSchoolInRabbitHoleEx : GoToSchoolInRabbitHole, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, GoToSchoolInRabbitHole.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<RabbitHole, GoToSchoolInRabbitHole.Definition>(Singleton);
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

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoToSchoolInRabbitHoleEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}

