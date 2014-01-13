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
    public enum SaveStyle
    {
        RealTime,
        SimTime,
        SimHour,
        Default
    }

    public class SaveStyleOption : EnumSettingOption<SaveStyle,GameObject>, IPrimaryOption<GameObject>
    {
        public override string GetTitlePrefix()
        {
            return "SaveStyle";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override SaveStyle Value
        {
            get
            {
                return Saver.Settings.SaveStyle;
            }
            set
            {
                Saver.Settings.SaveStyle = value;
            }
        }

        public override SaveStyle Default
        {
            get { return SaveStyle.Default; }
        }

        protected override bool Allow(SaveStyle value)
        {
            if (value == SaveStyle.Default) return false;

            return true;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);
            if (result != OptionResult.Failure)
            {
                switch(Value)
                {
                    case SaveStyle.RealTime:
                        if (Saver.Settings.mRealMinutesBetweenSaves <= 0)
                        {
                            Saver.Settings.mRealMinutesBetweenSaves = 30;
                        }
                        break;
                    case SaveStyle.SimTime:
                        if (Saver.Settings.mSimMinutesBetweenSaves <= 0)
                        {
                            Saver.Settings.mSimMinutesBetweenSaves = 1440;
                        }
                        break;
                    case SaveStyle.SimHour:
                        if (Saver.Settings.mSimSaveHour.Count == 0)
                        {
                            Saver.Settings.mSimSaveHour.Add(5);
                        }
                        break;
                }

                Saver.RestartTimers();
            }

            return result;
        }
    }
}