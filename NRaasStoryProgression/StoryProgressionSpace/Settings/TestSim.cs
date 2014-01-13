using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.StoryProgressionSpace.Booters;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
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
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Settings
{
    public class TestSim : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public delegate void OnPerformFunc(Sim target);

        public static event OnPerformFunc OnPerform;

        public override string GetTitlePrefix()
        {
            return "Test";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!(parameters.mTarget is Sim)) return false;

            if (!StoryProgression.Main.DebuggingEnabled) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder msg = new Common.StringBuilder("Run");

            try
            {
                Sim sim = parameters.mTarget as Sim;
                if (sim == null) return OptionResult.Failure;

                if (OnPerform != null)
                {
                    OnPerform(sim);
                }

                /*
                foreach (PropertyData deed in RealEstateManager.AllPropertiesFromAllHouseholds())
                {
                    msg += Common.NewLine + "Name: " + deed.LocalizedName;
                    msg += Common.NewLine + " Owner: " + deed.Owner.OwningHousehold.Name;
                }*/
            }
            catch (Exception e)
            {
                Common.Exception(parameters.mActor, parameters.mTarget, msg, e);
            }
            finally
            {
                Common.WriteLog(msg);
            }

            return OptionResult.SuccessRetain;
        }
    }
}
