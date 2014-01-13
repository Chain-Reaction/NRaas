using NRaas.CommonSpace.Options;
using NRaas.TempestSpace.Helpers;
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

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles
{
    public class Remove : OperationSettingOption<GameObject>, IProfileOption
    {
        Season mSeason;

        WeatherProfile mProfile;

        public Remove(Season season, WeatherProfile profile)
        {
            mSeason = season;
            mProfile = profile;
        }

        public override string GetTitlePrefix()
        {
            return "RemoveProfile";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (Tempest.Settings.GetWeather(mSeason).NumProfiles <= 1) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { mProfile.Name }))) return OptionResult.Failure;

            Tempest.Settings.GetWeather(mSeason).RemoveProfile(mProfile);

            Tempest.ReapplySettings();
            return OptionResult.SuccessLevelDown;
        }
    }
}
