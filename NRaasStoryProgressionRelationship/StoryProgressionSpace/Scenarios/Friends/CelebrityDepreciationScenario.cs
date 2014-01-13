using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.SimDataElement;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Friends
{
    public class CelebrityDepreciationScenario : SimScenario, IAlarmScenario
    {
        public CelebrityDepreciationScenario()
        { }
        protected CelebrityDepreciationScenario(CelebrityDepreciationScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "CelebrityDepreciation";
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        public AlarmManagerReference SetupAlarm(IAlarmHandler alarms)
        {
            return alarms.AddAlarmDay(this, 9);
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option, bool>()) return false;

            return base.Allow();
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CelebrityManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else if (sim.CelebrityLevel <= 0)
            {
                IncStat("No Level");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            List<int> levels = GetValue<ByLevelOption, List<int>>();

            int level = (int)Sim.CelebrityLevel;
            if (level >= levels.Count) return false;

            int points = -levels[level];

            if (SimTypes.IsSelectable(Sim))
            {
                points *= GetValue<ActiveAccelerationOption, int>();
            }

            Friends.AccumulateCelebrity(Sim, points);

            return true;
        }

        public override Scenario Clone()
        {
            return new CelebrityDepreciationScenario(this);
        }

        public class Option : BooleanAlarmOptionItem<ManagerFriendship, CelebrityDepreciationScenario>, ManagerFriendship.ICelebrityOption, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityPointsDepreciationAlarm";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }
        }

        public class ActiveAccelerationOption : IntegerManagerOptionItem<ManagerFriendship>, ManagerFriendship.ICelebrityOption
        {
            public ActiveAccelerationOption()
                : base(2)
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityDepreciationActiveAcceleration";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }

        public class ByLevelOption : MultiListedManagerOptionItem<ManagerFriendship, int>, ManagerFriendship.ICelebrityOption
        {
            public ByLevelOption()
                : base(new List<int>(new int[] { 50, 50, 25, 25, 50, 100 }))
            { }

            public override string GetTitlePrefix()
            {
                return "CelebrityPointsDepreciation";
            }

            protected override string PersistLookup(string value)
            {
                return null;
            }

            protected override bool PersistCreate(ref int defValue, string value)
            {
                int.TryParse(value, out defValue);
                return true;
            }

            public override string GetUIValue(bool pure)
            {
                return null;
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }

            protected override List<IGenericValueOption<int>> GetAllOptions()
            {
                List<IGenericValueOption<int>> results = new List<IGenericValueOption<int>>();
                for (int i = 0; i < 6; i++)
                {
                    results.Add(new LevelItem(this, i, Value[i]));
                }

                return results;
            }

            protected override bool PrivatePerform(List<int> values)
            {
                foreach (int value in values)
                {
                    string text = StringInputDialog.Show(Name, Localize("Prompt", new object[] { value }), Value[value].ToString());
                    if (string.IsNullOrEmpty(text)) return false;

                    int newValue;
                    if (!int.TryParse(text, out newValue))
                    {
                        Common.Localize("Numeric:Error");
                        continue;
                    }

                    if (newValue < 0)
                    {
                        newValue = -newValue;
                    }

                    Value[value] = newValue;
                }

                return true;
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }

            public class LevelItem : BaseListItem<ByLevelOption>
            {
                public readonly new int mValue;

                public LevelItem(ByLevelOption option, int level, int value)
                    : base(option, level)
                {
                    mValue = value;
                }

                public override string Name
                {
                    get
                    {
                        return Common.Localize("CelebrityPointsDepreciation:Level", false, new object[] { Value });
                    }
                }

                public override string DisplayValue
                {
                    get
                    {
                        return EAText.GetNumberString(mValue);
                    }
                }
            }
        }
    }
}
