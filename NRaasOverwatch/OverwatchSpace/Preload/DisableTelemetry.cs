using NRaas.CommonSpace.Replacers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Preload
{
    public class DisableTelemetry : PreloadOption
    {
        public DisableTelemetry()
        { }

        public override string GetTitlePrefix()
        {
            return "DisableTelemetry";
        }

        public override void OnPreLoad()
        {
            Overwatch.Log("Try DisableTelemetry");

            IOptionsModel optionsModel = Responder.Instance.OptionsModel;
            if (optionsModel != null)
            {
                DeviceConfig.EnableRegistryTelemetry(false);
                DeviceConfig.SetOption(optionsModel.EnableTelemetryKey, 0x0);

                DeviceConfig.SaveOptions();

                Overwatch.Log("Telemetry Disabled");
            }
        }
    }
}
