using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.SaverSpace.Options
{
    public class SimTimeSaveMinutes : IntegerSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "SimTimeSaveMinutes";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override void Import(CommonSpace.Helpers.Persistence.Lookup settings)
        {
            base.Import(settings);

            Saver.RestartTimers();
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (Saver.Settings.SaveStyle != SaveStyle.SimTime) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);
            if (result != OptionResult.Failure)
            {
                Saver.RestartTimers();
            }

            return result;
        }

        protected override int Value
        {
            get
            {
                return Saver.Settings.mSimMinutesBetweenSaves;
            }
            set
            {
                Saver.Settings.mSimMinutesBetweenSaves = value;
            }
        }
    }
}