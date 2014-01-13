using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Scoring;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Households
{
    public abstract class MoveScenario : DualSimScenario 
    {
        public MoveScenario (SimDescription sim, SimDescription target)
            : base (sim, target)
        { }
        protected MoveScenario()
        { }
        protected MoveScenario(SimDescription sim)
            : base(sim)
        { }
        protected MoveScenario(MoveScenario scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected abstract MoveInLotScenario GetMoveInLotScenario(HouseholdBreakdown breakdown);

        protected abstract MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going);

        protected abstract MoveInScenario GetMoveInScenario(List<SimDescription> going, SimDescription moveInWith);

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            return Sims.All;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Households.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if ((!Households.AllowSoloMove(Sim)) && (!Households.AllowSoloMove(Target)))
            {
                IncStat("Teen Denied");
                return false;
            }
            else if (!Households.Allow(this, Sim, Target, Managers.Manager.AllowCheck.None))
            {
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected int GetScore(SimDescription sim)
        {
            if (sim.LotHome == null) return -1000;

            int score = (GetValue<MaximumSizeOption,int>(sim.Household) - HouseholdsEx.NumHumansIncludingPregnancy(sim.Household)) * 2;

            foreach (SimDescription member in HouseholdsEx.Humans(sim.Household))
            {
                if (Relationships.IsCloselyRelated(member, sim, false))
                {
                    score++;
                }

                if (member.Elder)
                {
                    score++;
                }
                else if (member.ToddlerOrBelow)
                {
                    score--;
                }
            }

            return score;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if (Sim.Household == Target.Household)
            {
                HouseholdBreakdown breakdown = new HouseholdBreakdown(Manager, this, "MoveSame", Sim, Target, HouseholdBreakdown.ChildrenMove.Go, false);

                Add(frame, GetMoveInLotScenario(breakdown), ScenarioResult.Start);
                return false;
            }
            else
            {
                HouseholdBreakdown breakdownA = new HouseholdBreakdown(Manager, this, "MoveA1", Sim, HouseholdBreakdown.ChildrenMove.Scoring, false);
                HouseholdBreakdown breakdownB = new HouseholdBreakdown(Manager, this, "MoveB1", Target, HouseholdBreakdown.ChildrenMove.Scoring, false);

                if ((!breakdownA.SimGoing) && (!breakdownB.SimGoing))
                {
                    if ((Sim.LotHome != null) && (Target.LotHome != null))
                    {
                        breakdownA = new HouseholdBreakdown(Manager, this, "MoveA2", Sim, HouseholdBreakdown.ChildrenMove.Scoring, true);
                        breakdownB = new HouseholdBreakdown(Manager, this, "MoveB2", Target, HouseholdBreakdown.ChildrenMove.Scoring, true);
                    }
                    else if (Sim.LotHome == null)
                    {
                        breakdownA = new HouseholdBreakdown(Manager, this, "MoveA3", Sim, HouseholdBreakdown.ChildrenMove.Scoring, true);
                    }
                    else
                    {
                        breakdownB = new HouseholdBreakdown(Manager, this, "MoveB3", Target, HouseholdBreakdown.ChildrenMove.Scoring, true);
                    }
                }

                int scoreA = 0;
                int scoreB = 0;

                bool moveIn = true;

                NameTakeType type = GetValue<MoveOption, NameTakeType>();
                if (type == NameTakeType.Husband)
                {
                    if (Sim.IsMale)
                    {
                        scoreA = 1000;
                    }
                    else
                    {
                        scoreB = 1000;
                    }
                }
                else if (type == NameTakeType.Wife)
                {
                    if (Sim.IsFemale)
                    {
                        scoreA = 1000;
                    }
                    else
                    {
                        scoreB = 1000;
                    }
                }
/*
                else if ((type == NameTakeType.User) && (Sim.LotHome != null) && (Target.LotHome != null))
                {
                    bool flag = TwoButtonDialog.Show(
                        Households.Localize(Sim.IsFemale, "", new object[] { Sim, Target }),
                        Households.Localize(Sim.IsFemale, Sim.FirstName, new object[0]),
                        Households.Localize(Sim.IsFemale, Target.FirstName, new object[0])
                    );
                }
                else if (type == NameTakeType.None)
                {
                    moveIn = false;
                }
 */ 
                else
                {
                    scoreA = AddScoring("Score A", GetScore(Sim));
                    scoreB = AddScoring("Score B", GetScore(Target));
                }

                if (moveIn)
                {
                    if (scoreA > scoreB)
                    {
                        if ((breakdownA.SimGoing) && (Target.LotHome != null))
                        {
                            IncStat("Try Move A");
                            Add(frame, GetMoveInScenario(breakdownA.Going, Target), ScenarioResult.Failure);
                        }
                        else
                        {
                            if (!breakdownA.SimGoing)
                            {
                                IncStat("Not SimGoing A");
                            }

                            if (Target.LotHome == null)
                            {
                                IncStat("No LotHome A");
                            }
                        }

                        if ((breakdownB.SimGoing) && (Sim.LotHome != null))
                        {
                            IncStat("Try Move B");
                            Add(frame, GetMoveInScenario(breakdownB.Going, Sim), ScenarioResult.Failure);
                        }
                        else
                        {
                            if (!breakdownB.SimGoing)
                            {
                                IncStat("Not SimGoing B");
                            }

                            if (Sim.LotHome == null)
                            {
                                IncStat("No LotHome B");
                            }
                        }
                    }
                    else
                    {
                        if ((breakdownB.SimGoing) && (Sim.LotHome != null))
                        {
                            IncStat("Try Move B");
                            Add(frame, GetMoveInScenario(breakdownB.Going, Sim), ScenarioResult.Failure);
                        }
                        else
                        {
                            if (!breakdownB.SimGoing)
                            {
                                IncStat("Not SimGoing B");
                            }

                            if (Sim.LotHome == null)
                            {
                                IncStat("No LotHome B");
                            }
                        }

                        if ((breakdownA.SimGoing) && (Target.LotHome != null))
                        {
                            IncStat("Try Move A");
                            Add(frame, GetMoveInScenario(breakdownA.Going, Target), ScenarioResult.Failure);
                        }
                        else
                        {
                            if (!breakdownA.SimGoing)
                            {
                                IncStat("Not SimGoing A");
                            }

                            if (Target.LotHome == null)
                            {
                                IncStat("No LotHome A");
                            }
                        }
                    }
                }
                else
                {
                    IncStat("Not MoveIn");
                }

                breakdownA = new HouseholdBreakdown(Manager, this, "MoveLotA", Sim, HouseholdBreakdown.ChildrenMove.Scoring, true);
                breakdownB = new HouseholdBreakdown(Manager, this, "MoveLotB", Target, HouseholdBreakdown.ChildrenMove.Scoring, true);

                if ((breakdownA.SimGoing) && (breakdownB.SimGoing))
                {
                    int humans = 0, pets = 0;
                    breakdownA.GetGoingCount(ref humans, ref pets);
                    breakdownB.GetGoingCount(ref humans, ref pets);

                    if ((humans <= Options.GetValue<MaximumSizeOption, int>()) && (pets <= Options.GetValue<MaximumPetSizeOption,int>()))
                    {
                        List<SimDescription> going = new List<SimDescription>();
                        going.AddRange(breakdownA.Going);
                        going.AddRange(breakdownB.Going);

                        IncStat("Try Move A + B");
                        Add(frame, GetMoveInLotScenario(going), ScenarioResult.Failure);
                    }
                    else
                    {
                        AddStat("Too Many Movers", humans + pets);
                        AddStat("Too Many Movers Humans", humans);
                        AddStat("Too Many Movers Pets", pets);
                    }
                }
                else
                {
                    if (!breakdownA.SimGoing)
                    {
                        IncStat("Not Going A");
                    }

                    if (!breakdownB.SimGoing)
                    {
                        IncStat("Not Going B");
                    }
                }

                return false;
            }
        }

        public class MoveOption : EnumManagerOptionItem<ManagerHousehold, NameTakeType>
        {
            public MoveOption()
                : base(NameTakeType.Either, NameTakeType.Either)
            { }

            public override string GetTitlePrefix()
            {
                return "MoveInPreference";
            }

            protected override string GetLocalizationValueKey()
            {
                return "NameTakeType";
            }

            protected override NameTakeType Convert(int value)
            {
                return (NameTakeType)value;
            }

            protected override NameTakeType Combine(NameTakeType original, NameTakeType add, out bool same)
            {
                same = (original == add);
                return add;
            }

            protected override bool Allow(NameTakeType value)
            {
                switch (value)
                {
                    case NameTakeType.Either:
                    case NameTakeType.Husband:
                    case NameTakeType.Wife:
                        return true;
                }

                return false;
            }
        }
    }
}
