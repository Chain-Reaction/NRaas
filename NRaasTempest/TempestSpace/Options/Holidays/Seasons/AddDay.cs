using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TempestSpace.Options.Holidays.Seasons
{
    public class AddDay : OperationSettingOption<GameObject>, ISeasonOption
    {
        Season mSeason;

        public AddDay(Season season)
        {
            mSeason = season;
        }

        public override string GetTitlePrefix()
        {
            return "Add";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Tempest.Settings.GetHolidays(mSeason).Add(mSeason);
            return OptionResult.SuccessRetain;
        }
    }
}
