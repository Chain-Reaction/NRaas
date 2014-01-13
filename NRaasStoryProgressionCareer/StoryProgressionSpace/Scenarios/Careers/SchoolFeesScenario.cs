using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Options;
using NRaas.CommonSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public class SchoolFeesScenario : SimScenario
    {
        int mSchoolFee;

        public SchoolFeesScenario()
            : base ()
        { }
        protected SchoolFeesScenario(SchoolFeesScenario scenario)
            : base (scenario)
        {
            mSchoolFee = scenario.mSchoolFee;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "SchoolFee";
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueReportChance
        {
            get { return 1; }
        }

        protected override bool Allow()
        {
            if (GetValue<Option, int>() <= 0) return false;

            return base.Allow();
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Careers.SchoolChildren;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!Money.ShouldBillHouse(sim.Household))
            {
                IncStat("No Bills");
                return false;
            }
            else if (Sim.CareerManager == null)
            {
                IncStat("No Manager");
                return false;
            }
            else 
            {
                School school = Sim.CareerManager.School;
                if (school == null)
                {
                    IncStat("No School");
                    return false;
                }
                else if (school.CareerLoc == null)
                {
                    IncStat("No CareerLoc");
                    return false;
                }
                else if (school.CareerLoc.Owner == null)
                {
                    IncStat("No Owner");
                    return false;
                }
                else if (HasValue<PublicAssignSchoolScenario.ConsiderPublicOption,OccupationNames>(school.Guid))
                {
                    IncStat("Public School");
                    return false;
                }
                else if (school.PayPerHourBase > 0)
                {
                    IncStat("Paid School");
                    return false;
                }
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            mSchoolFee = GetValue<Option, int>() + GetOption<FeeBySchoolOption>().GetFee(Sim.CareerManager.School.Guid);

            if (GetValue<IsRichOption,bool>(Sim.Household))
            {
                mSchoolFee *= 2;
            }

            Money.AdjustFunds(Sim, "SchoolFee", -mSchoolFee);

            AddStat("Paid", mSchoolFee);
            return true;
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (parameters == null)
            {
                parameters = new object[] { Sim, mSchoolFee };
            }

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        public override Scenario Clone()
        {
            return new SchoolFeesScenario(this);
        }

        public class Option : IntegerScenarioOptionItem<ManagerCareer, SchoolFeesScenario>, ManagerCareer.ISchoolOption
        {
            public Option()
                : base(0)
            { }

            public override string GetTitlePrefix()
            {
                return "PrivateSchoolFee";
            }
        }

        public class SchoolFee
        {
            public readonly OccupationNames mSchool;

            public int mValue;

            public SchoolFee(OccupationNames school, int value)
            { 
                mSchool = school;
                mValue = value;
            }

            public override string ToString()
            {
 	            return mSchool + ":" + mValue;
            }

            public override int GetHashCode()
            {
                return (int)mSchool;
            }

            public override bool Equals(object o)
            {
                SchoolFee fee = o as SchoolFee;

                return (mSchool == fee.mSchool);
            }
        }

        public class FeeBySchoolOption : MultiListedManagerOptionItem<ManagerCareer, SchoolFee>, ManagerCareer.ISchoolOption
        {
            public FeeBySchoolOption()
                : base(new List<SchoolFee>())
            {}

            public override string GetTitlePrefix()
            {
                return "FeeBySchool";
            }

            protected override string PersistLookup(string value)
            {
                string[] values = value.Split(':');

                return values[0];
            }

            public int GetFee(OccupationNames school)
            {
                SchoolFee fee = Value.Find(item => { return (item.mSchool == school); });
                if (fee == null) return 0;

                return fee.mValue;
            }

            protected override bool PersistCreate(ref SchoolFee defValue, string value)
            {
                if (defValue == null) return false;

                string[] values = value.Split(':');

                defValue = new SchoolFee(defValue.mSchool, int.Parse(values[1]));
                return true;
            }

            protected override List<IGenericValueOption<SchoolFee>> GetAllOptions()
            {
                List<IGenericValueOption<SchoolFee>> results = new List<IGenericValueOption<SchoolFee>>();

                foreach (Career school in CareerManager.CareerList)
                {
                    if (!(school is School)) continue;

                    int value = 0;

                    SchoolFee fee = Value.Find(item => { return (item.mSchool == school.Guid); });
                    if (fee != null)
                    {
                        value = fee.mValue;
                    }

                    results.Add(new ListItem(this, new SchoolFee(school.Guid, value)));
                }

                return results;
            }

            protected override bool PrivatePerform(List<SchoolFee> values)
            {
                foreach (SchoolFee fee in values)
                {
                    Occupation school = CareerManager.GetStaticOccupation(fee.mSchool);
                    if (school == null) continue;

                    string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix () + ":Prompt", false, new object[] { school.CareerName }), fee.mValue.ToString());
                    if (string.IsNullOrEmpty(text)) continue;

                    int value;
                    if (!int.TryParse(text, out value)) continue;

                    fee.mValue = value;

                    SchoolFee existing = Value.Find(item => { return (item.mSchool == fee.mSchool); });
                    if (existing != null)
                    {
                        existing.mValue = fee.mValue;
                    }
                    else
                    {
                        Value.Add(fee);
                    }
                }

                return true;
            }

            public override bool ShouldDisplay()
            {
                if (Manager.GetValue<Option, int>() <= 0) return false;

                return base.ShouldDisplay();
            }

            public class ListItem : BaseListItem<FeeBySchoolOption>
            {
                public ListItem(FeeBySchoolOption option, SchoolFee value)
                    : base (option, value)
                {
                    Occupation school = CareerManager.GetStaticOccupation(Value.mSchool);
                    if (school != null)
                    {
                        SetThumbnail(school.CareerIconColored, ProductVersion.BaseGame);
                    }
                }

                public override string Name
                {
                    get
                    {
                        Occupation school = CareerManager.GetStaticOccupation(Value.mSchool);
                        if (school == null) return Value.mSchool.ToString();

                        return school.CareerName;
                    }
                }

                public override string DisplayValue
                {
                    get
                    {
                        return EAText.GetNumberString(Value.mValue);
                    }
                }
            }
        }
    }
}
