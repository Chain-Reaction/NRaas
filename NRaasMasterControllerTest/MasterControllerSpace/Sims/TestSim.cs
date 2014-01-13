using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.Misc;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims
{
    public class TestSim : OptionItem, ISimOption
    {
        public override string GetTitlePrefix()
        {
            return "TestSim";
        }

        public bool Test(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Test(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        public OptionResult Perform(GameHitParameters<SimDescriptionObject> parameters)
        {
            return base.Perform(new GameHitParameters<GameObject>(parameters.mActor, parameters.mTarget, parameters.mHit));
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder msg = new Common.StringBuilder();

            try
            {
                Sim actor = parameters.mActor as Sim;
                Sim target = parameters.mTarget as Sim;

                if (!AcceptCancelDialog.Show("You are about to run 'TestSim', proceed?")) return OptionResult.Failure;

                SimOutfit outfit = new SimOutfit(target.SimDescription.DefaultOutfitKey);

                CASPart part3 = new CASPart();
                foreach (CASPart part4 in outfit.Parts)
                {
                    if (part4.BodyType == BodyTypes.LowerBody)
                    {
                        part3 = part4;
                    }
                }

                msg += Common.NewLine + part3.Key;

                string vFXName = RobotDataManager.GetVFXName(part3.Key.InstanceId, RobotDataManager.RobotParts.InGameHoverVFX);
                msg += Common.NewLine + "A: " + vFXName;

                vFXName = RobotDataManager.GetVFXName(part3.Key.InstanceId, RobotDataManager.RobotParts.CABHoverVFX);
                msg += Common.NewLine + "B: " + vFXName;

                outfit = target.SimDescription.GetOutfit(OutfitCategories.Everyday, 0);

                part3 = new CASPart();
                foreach (CASPart part4 in outfit.Parts)
                {
                    if (part4.BodyType == BodyTypes.LowerBody)
                    {
                        part3 = part4;
                    }
                }

                msg += Common.NewLine + part3.Key;

                vFXName = RobotDataManager.GetVFXName(part3.Key.InstanceId, RobotDataManager.RobotParts.InGameHoverVFX);
                msg += Common.NewLine + "A: " + vFXName;

                vFXName = RobotDataManager.GetVFXName(part3.Key.InstanceId, RobotDataManager.RobotParts.CABHoverVFX);
                msg += Common.NewLine + "B: " + vFXName;

                msg += Common.NewLine + target.CurrentWalkStyle;
            }
            catch (Exception e)
            {
                GameHitParameters<GameObject>.Exception(parameters, msg, e);
            }
            finally
            {
                Common.DebugWriteLog(msg);
            }
            return OptionResult.SuccessClose;
        }
    }
}
