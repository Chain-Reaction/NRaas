using NRaas.CommonSpace.Converters;
using NRaas.CommonSpace.Helpers;
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
    public class SaveHour : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>, IPersistence
    {
        public override string GetTitlePrefix()
        {
            return "SaveHour";
        }

        public string PersistencePrefix
        {
            get { return ""; }
        }

        protected static float Convert(string value)
        {
            float result;
            if (!float.TryParse(value, out result)) return 0;
            return result;
        }

        public void Import(Persistence.Lookup settings)
        {
            Saver.Settings.mSimSaveHour = settings.GetList<float>("SaveHour", Convert);

            Saver.RestartTimers();
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("SaveHour", Saver.Settings.mSimSaveHour, Saver.Settings.mSimSaveHour.Count);
        }

        public override string DisplayValue
        {
            get
            {
                if (Saver.Settings.mSimSaveHour.Count <= 4)
                {
                    return GetString();
                }
                else
                {
                    return "...";
                }
            }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (Saver.Settings.SaveStyle != SaveStyle.SimHour) return false;

            return base.Allow(parameters);
        }

        protected string GetString()
        {
            string result = null;

            foreach (float value in Saver.Settings.mSimSaveHour)
            {
                if (result != null)
                {
                    result += ", ";
                }

                result += value;
            }

            return result;
        }

        public static bool ToFloat(string value, out float result)
        {
            return float.TryParse(value.Trim(), out result);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            string result = GetString();

            result = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt"), result);
            if (string.IsNullOrEmpty(result)) return OptionResult.Failure;

            Saver.Settings.mSimSaveHour = StringToList<float>.StaticConvert(result, ToFloat);

            Saver.RestartTimers();

            return OptionResult.SuccessRetain;
        }
    }
}