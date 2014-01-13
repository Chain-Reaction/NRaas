using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RelativitySpace.Options.Speeds
{
    public class DumpSpeeds : OperationSettingOption<GameObject>, ISpeedsOption
    {
        public override string GetTitlePrefix()
        {
            return "DumpSpeeds";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Relativity.Settings.Debugging) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            string msg = null;

            foreach (DaysOfTheWeek day in Enum.GetValues(typeof(DaysOfTheWeek)))
            {
                if ((day == DaysOfTheWeek.None) || (day == DaysOfTheWeek.All)) continue;

                for (int hour = 0; hour < 24; hour++)
                {
                    msg += Common.NewLine + day.ToString () + " - " + hour + ": " + Relativity.Settings.GetSpeed(day, hour, 0);
                }
            }

            Common.WriteLog(msg);

            return OptionResult.SuccessRetain;
        }
    }
}
