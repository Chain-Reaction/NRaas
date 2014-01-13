using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class WeightOption : SimPersonality.IntegerOption, IWeightable, IScenarioOptionItem
    {
        Scenario mScenario;

        NameOption mName = null;

        bool mVisible = false;

        public WeightOption()
            : base(0)
        { }

        public override string ToString()
        {
            string text = base.ToString();

            if (mName == null)
            {
                text += Common.NewLine + "WeightName=NO NAME";
            }
            else if (!string.IsNullOrEmpty(mName.WeightName))
            {
                text += Common.NewLine + "WeightName=" + mName.WeightName;
            }

            text += Common.NewLine + "Visible=" + mVisible;

            text += Common.NewLine + "-- Scenario --" + Common.NewLine + mScenario + Common.NewLine + "-- End Scenario --";

            return text;
        }

        public override bool Install(SimPersonality main, bool initial)
        {
            if (!base.Install(main, initial)) return false;

            if (mScenario != null)
            {
                mScenario.Manager = Manager;
            }

            return true;
        }

        protected string GetLocalizedWeight()
        {
            return Localize(GetTitlePrefix());
        }

        public override string Name
        {
            get
            {
                return Localize("MenuName", new object[] { GetLocalizedWeight() });
            }
        }

        public override string GetTitlePrefix()
        {
            if (mName == null)
            {
                return GetType().ToString();
            }
            else
            {
                return mName.WeightName;
            }
        }

        public override string GetStoreKey()
        {
            if (Manager == null) return null;

            return Manager.UnlocalizedName + GetTitlePrefix();
        }

        public override string GetLocalizationKey()
        {
            return "ScenarioWeight";
        }

        public float Weight
        {
            get { return Value; }
        }

        public Scenario GetScenario()
        {
            if (mScenario == null) return null;

            return mScenario.Clone();
        }

        public StoryProgressionObject GetManager()
        {
            return Manager;
        }

        public bool IsVisible
        {
            get { return mVisible; }
        }

        public override bool ShouldDisplay()
        {
            if (!IsVisible) return false;

            return base.ShouldDisplay();
        }

        protected override string GetPrompt()
        {
            return Localize("Prompt", new object[] { GetLocalizedWeight() });
        }

        public override bool Parse(XmlDbRow row, SimPersonality personality, ref string error)
        {
            ProductVersion productVersion;
            if (!ParserFunctions.TryParseEnum<ProductVersion>(row.GetString("ProductVersion"), out productVersion, ProductVersion.BaseGame))
            {
                error = "ProductVersion missing";
                return false;
            }

            if (!GameUtils.IsInstalled(productVersion))
            {
                return true;
            }

            string module = row.GetString("Module");
            if ((!string.IsNullOrEmpty(module)) && (!Common.AssemblyCheck.IsInstalled(module)))
            {
                return true;
            }

            // Must be after the product version checks, but before everything else
            if (!base.Parse(row, personality, ref error)) return false;

            if (!row.Exists("Scenario"))
            {
                error = "Scenario missing";
                return false;
            }
            else if (!row.Exists("Weight"))
            {
                error = "Weight missing";
                return false;
            }
            else if (!row.Exists("Name"))
            {
                error = "Name missing";
                return false;
            }

            mName = new NameOption(row);

            Type classType = row.GetClassType("Scenario");
            if (classType == null)
            {
                error = "Scenario class not found";
                return false;
            }

            int weight = row.GetInt("Weight");

            if (weight > 0)
            {
                mVisible = true;
            }
            else
            {
                weight = 1;

                if (!row.Exists("ShouldPush"))
                {
                    error = "ShouldPush missing";
                    return false;
                }
            }

            SetValue (weight);

            try
            {
                mScenario = classType.GetConstructor(new Type[0]).Invoke(new object[0]) as Scenario;
            }
            catch
            {}

            if (mScenario == null)
            {
                error = "Scenario constructor fail";
                return false;
            }

            mScenario.Manager = personality;

            if (!mScenario.Parse(row, ref error))
            {
                return false;
            }

            if (!mScenario.PostParse(ref error))
            {
                return false;
            }

            IViolentScenario violentScenario = mScenario as IViolentScenario;
            if ((violentScenario != null) && (violentScenario.IsViolent))
            {
                PushDeathChanceOption.Installed = true;
            }

            return true;
        }

        public class NameOption
        {
            string mName;

            string mWeightName;

            public NameOption(XmlDbRow row)
            {
                mName = row.GetString("Name");

                mWeightName = row.GetString("WeightName");
            }

            public string WeightName
            {
                get
                {
                    if (!string.IsNullOrEmpty(mWeightName))
                    {
                        return mWeightName;
                    }
                    else
                    {
                        return mName;
                    }
                }
            }

            public override string ToString()
            {
                return mName;
            }
        }
    }
}

