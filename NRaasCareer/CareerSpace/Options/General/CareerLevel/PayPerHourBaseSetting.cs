using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.UI;
using System.Collections.Generic;

// listing with options for careers (eg, min / max coworkers), then list of careers, finally set option

namespace NRaas.CareerSpace.Options.General.CareerLevel
{
    public class PayPerHourBaseSetting : CareerLevelSettingOption, ICareerLevelOption
    {
        public override string GetTitlePrefix()
        {
            return "PayPerHourBase";
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
                string defaultPay = string.Empty;
                if (base.mPicks.Count == 1)
                {
                    CareerLevelSettingOption.LevelData data = base.mPicks[0];
                    if (data.mLevel != -1)
                    {
                        Career career = CareerManager.GetStaticCareer(data.mCareer);

                        if (career != null)
                        {
                            defaultPay = career.CareerLevels[data.mBranchName][data.mLevel].PayPerHourBase.ToString();
                        }
                    }
                }
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", false), defaultPay); // def

                if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                int value;
                if (!int.TryParse(text, out value))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("InputError:Numeric"));
                    return OptionResult.Failure;
                }

                foreach (CareerLevelSettingOption.LevelData level in base.mPicks)
                {
                    if (level.mLevel == -1)
                    {
                        foreach (Career career in CareerManager.CareerList)
                        {
                            foreach (string branch in career.CareerLevels.Keys)
                            {
                                foreach (KeyValuePair<int, Sims3.Gameplay.Careers.CareerLevel> levelData in career.CareerLevels[branch])
                                {
                                    PersistedSettings.CareerSettings settings = NRaas.Careers.Settings.GetCareerSettings(level.mCareer, true);
                                    PersistedSettings.CareerLevelSettings levelSettings = settings.GetSettingsForLevel(level.mBranchName, level.mLevel, true);
                                    levelSettings.mPayPerHourBase = value;

                                    levelData.Value.PayPerHourBase = value;
                                }
                            }
                        }
                    }
                    else
                    {
                        PersistedSettings.CareerSettings settings = NRaas.Careers.Settings.GetCareerSettings(level.mCareer, true);
                        PersistedSettings.CareerLevelSettings levelSettings = settings.GetSettingsForLevel(level.mBranchName, level.mLevel, true);
                        levelSettings.mPayPerHourBase = value;

                        NRaas.Careers.Settings.SetCareerData(settings);
                    }
                }

                Common.Notify(Common.Localize("Generic:Success"));
                return OptionResult.SuccessLevelDown;
            }

            return result;
        }
    }
}