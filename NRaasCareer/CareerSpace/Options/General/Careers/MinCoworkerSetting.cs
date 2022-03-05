using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.UI;

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
                foreach (OccupationNames name in base.mPicks)
                {
                    string defaultText = string.Empty;
                    if (name != OccupationNames.Any)
                    {
                        NRaas.CareerSpace.PersistedSettings.CareerSettings settings = NRaas.Careers.Settings.GetCareerSettings(name, true);

                        defaultText = settings.mMaxCoworkers.ToString();
                    }

                    string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", false), defaultText);

                    if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                    int value;
                    if (!int.TryParse(text, out value))
                    {
                        SimpleMessageDialog.Show(Name, Common.Localize("InputError:Numeric"));
                        return OptionResult.Failure;
                    }

                    if (name == OccupationNames.Any)
                    {
                        foreach (Career career in CareerManager.CareerList)
                        {
                            PersistedSettings.CareerSettings settings = NRaas.Careers.Settings.GetCareerSettings(career.Guid, true);

                            settings.mMaxCoworkers = value;

                            NRaas.Careers.Settings.SetCareerData(settings);
                        }

                        Common.Notify(GetTitlePrefix() + " " + Common.Localize("Selection:All") + " " + value);
                    }
                    else
                    {
                        PersistedSettings.CareerSettings settings = NRaas.Careers.Settings.GetCareerSettings(name, true);
                        settings.mMaxCoworkers = value;

                        NRaas.Careers.Settings.SetCareerData(settings);

                        Common.Notify(GetTitlePrefix() + " " + CareerManager.GetStaticCareer(name).Name + " " + value);
                    }
                }

                Common.Notify(Common.Localize("Generic:Success"));
                return OptionResult.SuccessLevelDown;
            }

            return result;
        }
    }
}