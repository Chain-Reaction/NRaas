using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;

namespace NRaas.VectorSpace.Stages
{
    public class TestSettingStage : HitMissStage
    {
        string mSetting;

        public TestSettingStage(XmlDbRow row)
            : base(row)
        {
            if (BooterLogger.Exists(row, "Setting", Name))
            {
                mSetting = row.GetString("Setting");
            }
        }

        public override void GetSettings(System.Collections.Generic.List<string> settings)
        {
            if (settings.Contains(mSetting)) return;

            settings.Add(mSetting);
        }

        protected override bool IsSuccess(SimDescription sim, DiseaseVector vector)
        {
            return Vector.Settings.IsSet(mSetting);
        }

        public override string ToString()
        {
            string result = base.ToString();

            result += Common.NewLine + " Setting: " + mSetting;

            return result;
        }
    }
}
