using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Pregnancies
{
    public class BabyGenderScenario : SimEventScenario<Event>
    {
        public enum FirstBornGender
        {
            Either = 0,
            Male = 1,
            Female = 2,
            Balanced = 3
        }

        public BabyGenderScenario()
        { }
        protected BabyGenderScenario(BabyGenderScenario scenario)
            : base (scenario)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            if (type != PrefixType.Pure) return null;

            return "BabyGender";
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
            get { return false; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return null;
        }

        public override bool SetupListener(IEventHandler events)
        {
            return events.AddListener(this, EventTypeId.kGotPregnant);
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!sim.IsPregnant)
            {
                IncStat("Not Pregnant");
                return false;
            }
            else if (sim.Pregnancy.mGender != CASAgeGenderFlags.None)
            {
                IncStat("Already Set");
                return false;
            }

            return base.Allow(sim);
        }

        public static CASAgeGenderFlags CalculateGenderBalancer(StoryProgressionObject manager, bool isPet)
        {
            int males = 0, females = 0;

            foreach (SimDescription sim in manager.Sims.GetSpecies(isPet))
            {
                if (sim.LotHome == null) continue;

                if (sim.IsPregnant)
                {
                    if (sim.Pregnancy.mGender == CASAgeGenderFlags.Male)
                    {
                        males += 7;
                    }
                    else
                    {
                        females += 7;
                    }
                }

                if (sim.IsMale)
                {
                    males++;
                }
                else
                {
                    females++;
                }

                if (sim.AdultOrBelow)
                {
                    if (sim.IsMale)
                    {
                        males++;
                    }
                    else
                    {
                        females++;
                    }
                }

                if (sim.YoungAdultOrBelow)
                {
                    if (sim.IsMale)
                    {
                        males++;
                    }
                    else
                    {
                        females++;
                    }
                }

                if (sim.TeenOrBelow)
                {
                    if (sim.IsMale)
                    {
                        males++;
                    }
                    else
                    {
                        females++;
                    }
                }

                if (sim.ChildOrBelow)
                {
                    if (sim.IsMale)
                    {
                        males++;
                    }
                    else
                    {
                        females++;
                    }
                }

                if (sim.ToddlerOrBelow)
                {
                    if (sim.IsMale)
                    {
                        males++;
                    }
                    else
                    {
                        females++;
                    }
                }

                if (sim.Baby)
                {
                    if (sim.IsMale)
                    {
                        males++;
                    }
                    else
                    {
                        females++;
                    }
                }
            }

            if (males > females)
            {
                manager.IncStat("First Born Balanced Female");

                return CASAgeGenderFlags.Female;
            }
            else
            {
                manager.IncStat("First Born Balanced Male");

                return CASAgeGenderFlags.Male;
            }
        }

        public static CASAgeGenderFlags GetGenderByFirstBorn(StoryProgressionObject manager, FirstBornGender firstBorn, bool isPet)
        {
            if (firstBorn == FirstBornGender.Male)
            {
                return CASAgeGenderFlags.Male;
            }
            else if (firstBorn == FirstBornGender.Female)
            {
                return CASAgeGenderFlags.Female;
            }
            else if (firstBorn == FirstBornGender.Balanced)
            {
                return CalculateGenderBalancer(manager, isPet);
            }
            else if (RandomUtil.CoinFlip())
            {
                return CASAgeGenderFlags.Male;
            }
            else
            {
                return CASAgeGenderFlags.Female;
            }
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            SimDescription dad = ManagerSim.Find(Sim.Pregnancy.DadDescriptionId);
            if ((GetValue<SameSexSameGenderOption, bool>()) && (dad != null) && (dad.Gender == Sim.Gender))
            {
                IncStat("Same Sex");

                Sim.Pregnancy.mGender = Sim.Gender;
                return true;
            }

            FirstBornGender firstBorn = GetValue<FirstBornGenderOption, FirstBornGender>();

            if (firstBorn == FirstBornGender.Male)
            {
                IncStat("First Born Male");

                Sim.Pregnancy.mGender = CASAgeGenderFlags.Male;
                return true;
            }
            else if (firstBorn == FirstBornGender.Female)
            {
                IncStat("First Born Female");

                Sim.Pregnancy.mGender = CASAgeGenderFlags.Female;
                return true;
            }
            else
            {
                if ((GetValue<OneOfEachGenderOption, bool>()) && (Sim.Genealogy != null))
                {
                    bool male = false, female = false;

                    foreach (SimDescription child in Relationships.GetChildren(Sim))
                    {
                        if (SimTypes.IsDead(child)) continue;

                        if (child.IsMale)
                        {
                            male = true;
                        }
                        else if (child.IsFemale)
                        {
                            female = true;
                        }
                    }

                    if (male) 
                    {
                        if (!female)
                        {
                            Sim.Pregnancy.mGender = CASAgeGenderFlags.Female;
                            return true;
                        }
                    }
                    else if (female)
                    {
                        Sim.Pregnancy.mGender = CASAgeGenderFlags.Male;
                        return true;
                    }
                }

                if (firstBorn == FirstBornGender.Balanced)
                {
                    Sim.Pregnancy.mGender = CalculateGenderBalancer(Pregnancies, Sim.IsPet);
                    return true;
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new BabyGenderScenario(this);
        }

        public class Option : BooleanEventOptionItem<ManagerPregnancy, BabyGenderScenario>, IDebuggingOption
        {
            public Option()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "BabyGender";
            }
        }

        public class FirstBornGenderOption : FirstBornGenderOptionBase<ManagerPregnancy>
        {
            public FirstBornGenderOption()
            { }

            public override string GetTitlePrefix()
            {
                return "NewbornGender";
            }
        }

        public abstract class FirstBornGenderOptionBase<TManager> : EnumManagerOptionItem<TManager, FirstBornGender>
            where TManager : Manager
        {
            public FirstBornGenderOptionBase()
                : base(FirstBornGender.Balanced, FirstBornGender.Balanced)
            { }

            protected override string GetLocalizationValueKey()
            {
                return "FirstBornGender";
            }

            protected override FirstBornGender Convert(int value)
            {
                return (FirstBornGender)value;
            }

            protected override FirstBornGender Combine(FirstBornGender original, FirstBornGender add, out bool same)
            {
                same = (original == add);
                return add;
            }
        }

        public class SameSexSameGenderOption : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public SameSexSameGenderOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "SameSexSameGender";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }

        public class OneOfEachGenderOption : BooleanManagerOptionItem<ManagerPregnancy>
        {
            public OneOfEachGenderOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "OneOfEachGender";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
