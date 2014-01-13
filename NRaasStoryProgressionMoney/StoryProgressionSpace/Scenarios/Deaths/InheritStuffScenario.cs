using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Deaths
{
    public class InheritStuffScenario : SimScenario, IFormattedStoryScenario
    {
        Dictionary<SimDescription, bool> mInheritors = new Dictionary<SimDescription, bool>();

        public InheritStuffScenario(SimDescription sim)
            : base (sim)
        { }
        protected InheritStuffScenario(InheritStuffScenario scenario)
            : base (scenario)
        {
            //mInheritors = scenario.mInheritors;
        }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "InheritStuff";
        }

        public Manager GetFormattedStoryManager()
        {
            return StoryProgression.Main.Deaths;
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override bool ShouldReport
        {
            get
            {
                return !SimTypes.IsSelectable(Sim);
            }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow()
        {
            if (!GetValue<Option,bool>()) return false;

            return base.Allow();
        }

        protected override bool Allow(SimDescription sim)
        {
            if (sim.CreatedSim == null)
            {
                IncStat("Vanished");
                return false;
            }
            else if (!Manager.Deaths.IsDying(sim))
            {
                IncStat("Saved");
                return false;
            }

            Household house = sim.Household;
            if (house == null)
            {
                IncStat("No House");
                return false;
            }
            else if (SimTypes.IsSpecial(house))
            {
                IncStat("Special");
                return false;
            }

            return base.Allow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            Household house = Sim.Household;

            Sim sim = Sim.CreatedSim;

            Dictionary<SimDescription, float> inheritors = Deaths.GetInheritors(Sim, GetValue<InheritCashScenario.InheritorsOption,ManagerDeath.Inheritors>(), false);

            List<Sim> choices = new List<Sim>();
            foreach (SimDescription other in inheritors.Keys)
            {
                if (other.CreatedSim == null) continue;

                if (!other.ChildOrAbove) continue;

                choices.Add(other.CreatedSim);
            }

            if (choices.Count == 0)
            {
                foreach (Sim other in HouseholdsEx.AllHumans(house))
                {
                    if (other == sim) continue;

                    choices.Add(other);
                }

                if (choices.Count == 0)
                {
                    IncStat("No Choices");
                    return false;
                }
            }

            bool found = false;

            if (HouseholdsEx.NumHumans(house) == 1)
            {
                if (house.RealEstateManager.AllProperties.Count > 0)
                {
                    Dictionary<Household, Sim> houses = new Dictionary<Household, Sim>();

                    foreach (Sim choice in choices)
                    {
                        if (choice.Household == null) continue;

                        houses[choice.Household] = choice;
                    }

                    if (houses.Count > 0)
                    {
                        List<KeyValuePair<Household, Sim>> houseChoices = new List<KeyValuePair<Household, Sim>>(houses);

                        foreach (PropertyData data in house.RealEstateManager.AllProperties)
                        {
                            KeyValuePair<Household, Sim> choice = RandomUtil.GetRandomObjectFromList(houseChoices);

                            ManagerMoney.TransferProperty(house, choice.Key, data);

                            mInheritors[choice.Value.SimDescription] = true;

                            IncStat("Property Transferred");
                            found = true;
                        }
                    }
                }
            }

            if (!SimTypes.IsSelectable(Sim))
            {
                Lots.PackupVehicles(sim, (HouseholdsEx.NumHumans(house) > 1));

                foreach (GameObject obj in Inventories.QuickFind<GameObject>(sim.Inventory))
                {
                    Sim choice = RandomUtil.GetRandomObjectFromList(choices);

                    if ((obj is INotTransferableOnDeath) || (obj is IHiddenInInventory) || (obj is DeathFlower) || (obj is Diploma))
                    {
                        IncStat("NonTrans " + obj.GetLocalizedName());
                        continue;
                    }

                    found = true;

                    if (Inventories.TryToMove(obj, choice))
                    {
                        IncStat("Transferred " + obj.GetLocalizedName());

                        mInheritors[choice.SimDescription] = true;
                    }
                    else
                    {
                        IncStat("Unremovable " + obj.GetLocalizedName());
                    }
                }
            }

            Writing oldSkill = Sim.SkillManager.GetSkill<Writing>(SkillNames.Writing);
            if (oldSkill != null)
            {
                Writing.RoyaltyAlarm alarm = oldSkill.mRoyaltyAlarm;

                if (alarm != null)
                {
                    List<Sim> royaltyChoices = new List<Sim>(choices);

                    while (royaltyChoices.Count > 0)
                    {
                        Sim choice = RandomUtil.GetRandomObjectFromList(royaltyChoices);
                        royaltyChoices.Remove(choice);

                        Writing newSkill = choice.SkillManager.GetSkill<Writing>(SkillNames.Writing);
                        if ((newSkill != null) && (newSkill.mRoyaltyAlarm != null)) continue;

                        newSkill = choice.SkillManager.AddElement(SkillNames.Writing) as Writing;
                        if (newSkill != null)
                        {
                            alarm.RemoveRoyaltyAlarm();

                            alarm.mAlarmHandle = AlarmManager.Global.AddAlarmDay(Writing.kRoyaltyPayHour, DaysOfTheWeek.Sunday, new AlarmTimerCallback(alarm.AlarmCallBack), "Royalty Alarm", AlarmType.AlwaysPersisted, newSkill.SkillOwner);
                            alarm.mSkill = newSkill;

                            newSkill.mRoyaltyAlarm = alarm;

                            IncStat("Transferred Royalties");

                            mInheritors[choice.SimDescription] = true;

                            found = true;
                        }

                        break;
                    }
                }
            }

            if (!found) return false;

            return (mInheritors.Count > 0);
        }

        protected override ManagerStory.Story PrintStory(StoryProgressionObject manager, string name, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mInheritors.Count > 0) return null;

            return base.PrintStory(manager, name, parameters, extended, logging);
        }

        protected override ManagerStory.Story PrintFormattedStory(StoryProgressionObject manager, string text, string summaryKey, object[] parameters, string[] extended, ManagerStory.StoryLogging logging)
        {
            if (mInheritors.Count == 0) return null;

            if (manager == null)
            {
                manager = Deaths;
            }

            if (text == null)
            {
                text = GetTitlePrefix(PrefixType.Summary);
            }

            return Stories.PrintFormattedSummary(Deaths, text, summaryKey, Sim, new List<SimDescription>(mInheritors.Keys), extended, logging);
        }

        public override Scenario Clone()
        {
            return new InheritStuffScenario(this);
        }

        public class Option : BooleanManagerOptionItem<ManagerDeath>
        {
            public Option()
                : base(true)
            { }

            public override bool Install(ManagerDeath main, bool initial)
            {
                if (!base.Install(main, initial)) return false;

                if (initial)
                {
                    GettingOldScenario.OnInheritStuffScenario += OnRun;
                }

                return true;
            }

            public override string GetTitlePrefix()
            {
                return "Inheritance";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public static void OnRun(Scenario scenario, ScenarioFrame frame)
            {
                SimScenario s = scenario as SimScenario;
                if (s == null) return;

                scenario.Add(frame, new InheritStuffScenario(s.Sim), ScenarioResult.Start);
            }
        }

        public class InheritRoyaltiesOption : BooleanManagerOptionItem<ManagerDeath>
        {
            public InheritRoyaltiesOption()
                : base (true)
            { }

            public override string GetTitlePrefix()
            {
                return "InheritRoyalties";
            }

            public override bool Progressed
            {
                get { return false; }
            }

            public override bool ShouldDisplay()
            {
                if (!Manager.GetValue<Option, bool>()) return false;

                return base.ShouldDisplay();
            }
        }
    }
}
