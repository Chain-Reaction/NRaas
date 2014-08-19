using NRaas.CommonSpace.Options;
using NRaas.TempestSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options
{
    public class AutoLightFireplacesTemperature : FloatSettingOption<GameObject>, IPrimaryOption<GameObject>
    {
        public AutoLightFireplacesTemperature()            
        { }

        public override string GetTitlePrefix()
        {
            return "AutoLightFireplacesTemperature";
        }

        protected override float Value
        {
            get
            {
                return Tempest.Settings.mAutoLightFireplacesTemperature;
            }
            set
            {
                Tempest.Settings.mAutoLightFireplacesTemperature = value;                
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override string GetPrompt()
        {
            if (Sims3.UI.Responder.Instance.OptionsModel.IsCelcius)
            {
                return Common.Localize("Temperature:PromptC");
            }
            else
            {
                return Common.Localize("Temperature:PromptF");
            }
        }

        public override string DisplayValue
        {
            get
            {
                float val = Value;
                if (Sims3.UI.Responder.Instance.OptionsModel.IsCelcius)
                {
                    val = SeasonsManager.TemperatureManager.TempAsCelcius(val);
                }
                return EAText.GetNumberString(val);
            }
        }

        protected override float Validate(float value)
        {
            if (Sims3.UI.Responder.Instance.OptionsModel.IsCelcius)
            {
                // Back to Fahrenheit
                value = (value / 0.5555556f) + 32;                
            }
            
            return base.Validate(value);
        }
    }
}