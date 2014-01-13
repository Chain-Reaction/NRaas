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

namespace NRaas.TempestSpace.Options.Weather.Seasons.Profiles
{
    public class CurrentDay : OperationSettingOption<GameObject>, IProfileOption
    {
        Season mSeason;

        public CurrentDay(Season season)
        {
            mSeason = season;
        }

        public override string DisplayValue
        {
            get
            {
                if (SeasonsManager.Enabled)
                {
                    if (SeasonsManager.CurrentSeason == mSeason)
                    {
                        return EAText.GetNumberString(Tempest.GetCurrentSeasonDay());
                    }
                }

                return Common.Localize("CurrentDay:OutOfSeason");
            }
        }

        public override string GetTitlePrefix()
        {
            return "CurrentDay";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return OptionResult.Failure;
        }
    }
}
