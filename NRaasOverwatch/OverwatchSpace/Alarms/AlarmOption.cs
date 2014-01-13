using NRaas.CommonSpace.Options;
using NRaas.OverwatchSpace.Interfaces;
using NRaas.OverwatchSpace.Settings;
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
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public abstract class AlarmOption : BooleanOption, IAlarmOption, IActionOption
    {
        public void PerformAlarm()
        {
            PerformAction(false);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            PerformAction(true);
            return OptionResult.SuccessClose;
        }

        protected void PerformAction(bool prompt)
        {
            if (!prompt)
            {
                if (!Value) return;
            }

            PrivatePerformAction(prompt);
        }

        protected abstract void PrivatePerformAction(bool prompt);
    }
}
