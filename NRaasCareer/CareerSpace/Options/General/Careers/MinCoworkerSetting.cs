using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

// listing with options for careers (eg, min / max coworkers), then list of careers, finally set option

namespace NRaas.CareerSpace.Options.General.Careers
{
    public class MinCoworkerSetting : CareerSettingOption, ICareerOption
    {
        public override string GetTitlePrefix()
        {
            return "MinCoworkers";
        }               

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            return base.Allow(parameters);
        }

        // this could be further broken down into another class (IntegerSettingOption) but not right now
        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);

            if (result != OptionResult.Failure)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", false), ""); // def

                if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                int value;
                if (!int.TryParse(text, out value))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return OptionResult.Failure;
                }

                foreach (OccupationNames name in base.mPicks)
                {
                    NRaas.CareerSpace.PersistedSettings.CareerSettings settings = NRaas.Careers.Settings.GetCareerSettings(name, true);
                    settings.minCoworkers = value;
                    NRaas.Careers.Settings.UpdateCareerSettings(settings);
                }

                Common.Notify("Generic:Success");
                return OptionResult.SuccessLevelDown;
            }

            return result;
        }
    }
}
