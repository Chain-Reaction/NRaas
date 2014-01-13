using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Households;
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

namespace NRaas.StoryProgressionSpace.Scenarios.Romances
{
    public abstract class BreakupScenario : DualSimScenario 
    {
        bool mAffair = false;

        bool mRelatedStay = false;

        public BreakupScenario(SimDescription sim, SimDescription target, bool affair, bool relatedStay)
            : base (sim, target)
        {
            mAffair = affair;
            mRelatedStay = relatedStay;
        }
        protected BreakupScenario(bool affair, bool relatedStay)
        {
            mAffair = affair;
            mRelatedStay = relatedStay;
        }
        protected BreakupScenario(BreakupScenario scenario)
            : base (scenario)
        {
            mAffair = scenario.mAffair;
            mRelatedStay = scenario.mRelatedStay;
        }

        protected int MinTimeFromRomanceToBreakup
        {
            get { return GetValue<MinTimeFromRomanceToBreakupOption, int>(); }
        }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mAffair = row.GetBool("Affair");

            mRelatedStay = row.GetBool("RelatedStay");

            return base.Parse(row, ref error);
        }

        protected override ICollection<SimDescription> GetTargets(SimDescription sim)
        {
            List<SimDescription> partner = new List<SimDescription>();
            if (sim.Partner != null)
            {
                partner.Add(sim.Partner);
            }

            return partner;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Romances.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }
            else if (!Romances.AllowBreakup(this, sim, Managers.Manager.AllowCheck.None))
            {
                IncStat("User Denied");
                return false;
            }
            else if (AddScoring("Breakup Cooldown", GetElapsedTime<DayOfLastRomanceOption>(sim) - MinTimeFromRomanceToBreakup) < 0)
            {
                AddStat("Too Early", GetElapsedTime<DayOfLastRomanceOption>(sim));
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected bool RenameDivorcee(SimDescription sim)
        {
            string parentNames = null;

            List<SimDescription> parents = Relationships.GetParents(sim);
            if (parents.Count > 0)
            {
                object[] objParents = null;
                if (parents.Count == 2)
                {
                    objParents = new object[] { parents[0], parents[1] };
                }
                else
                {
                    objParents = new object[] { parents[0] };
                }

                parentNames = Common.Localize("RenameDivorcee:PromptParents", sim.IsFemale, objParents);
            }

            List<object> parameters = Stories.AddGenderNouns(sim);

            string text = StringInputDialog.Show(Common.Localize("RenameDivorcee:MenuName"), Common.Localize("RenameDivorcee:Prompt", sim.IsFemale, parameters.ToArray()) + parentNames, sim.LastName);
            if (string.IsNullOrEmpty(text)) return false;

            sim.LastName = text;

            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            bool wasMarried = Sim.IsMarried;

            if (!Romances.BumpToLowerState(this, Sim, Target, false))
            {
                IncStat("Bump Fail");
                return false;
            }

            if (wasMarried)
            {
                if (GetValue<RenameDivorceeOption, bool>())
                {
                    RenameDivorcee(Sim);
                    RenameDivorcee(Target);
                }
            }

            if (Sim.Household == Target.Household)
            {
                SimDescription go = Sim;
                SimDescription stay = Target;

                SimDescription head = SimTypes.HeadOfFamily(Sim.Household);
                if (head != null)
                {
                    if (Relationships.IsCloselyRelated(Target, head, false))
                    {
                        stay = Target;
                        go = Sim;
                    }
                    else if (Relationships.IsCloselyRelated(Sim, head, false))
                    {
                        stay = Sim;
                        go = Target;
                    }
                    else if (RandomUtil.CoinFlip())
                    {
                        stay = Sim;
                        go = Target;
                    }
                }

                HouseholdBreakdown.ChildrenMove goScore = HouseholdBreakdown.ChildrenMove.Scoring;
                HouseholdBreakdown.ChildrenMove stayScore = HouseholdBreakdown.ChildrenMove.Scoring;
                if (mAffair)
                {
                    // Sim is cheater, so keep children with other parent
                    if (go == Sim)
                    {
                        if (mRelatedStay)
                        {
                            goScore = HouseholdBreakdown.ChildrenMove.RelatedStay;
                            stayScore = HouseholdBreakdown.ChildrenMove.RelatedGo;
                        }
                        else
                        {
                            goScore = HouseholdBreakdown.ChildrenMove.Stay;
                            stayScore = HouseholdBreakdown.ChildrenMove.Go;
                        }
                    }
                    else
                    {
                        if (mRelatedStay)
                        {
                            goScore = HouseholdBreakdown.ChildrenMove.RelatedGo;
                            stayScore = HouseholdBreakdown.ChildrenMove.RelatedStay;
                        }
                        else
                        {
                            goScore = HouseholdBreakdown.ChildrenMove.Go;
                            stayScore = HouseholdBreakdown.ChildrenMove.Stay;
                        }
                    }
                }

                Add(frame, new BreakupMoveOutScenario(go, stay, goScore), ScenarioResult.Failure);
                Add(frame, new BreakupMoveOutScenario(stay, go, stayScore), ScenarioResult.Failure);
            }

            return true;
        }

        protected class BreakupMoveOutScenario : MoveOutScenario
        {
            HouseholdBreakdown.ChildrenMove mChildMove;

            public BreakupMoveOutScenario(SimDescription go, SimDescription stay, HouseholdBreakdown.ChildrenMove childMove)
                : base(go, stay)
            {
                mChildMove = childMove;
            }
            protected BreakupMoveOutScenario(BreakupMoveOutScenario scenario)
                : base(scenario)
            {
                mChildMove = scenario.mChildMove;
            }

            public override string GetTitlePrefix(PrefixType type)
            {
                if (type != PrefixType.Pure) return null;

                return "BreakupMoveOut";
            }

            protected override HouseholdBreakdown.ChildrenMove ChildMove
            {
                get { return mChildMove; }
            }

            protected override MoveInLotScenario GetMoveInLotScenario(List<SimDescription> going)
            {
                return new StandardMoveInLotScenario(going, 0);
            }

            protected override ScoredMoveInScenario GetMoveInScenario(List<SimDescription> going)
            {
                return new InspectedScoredMoveInScenario(Sim, going);
            }

            public override Scenario Clone()
            {
                return new BreakupMoveOutScenario(this);
            }
        }

        public class MinTimeFromRomanceToBreakupOption : Manager.CooldownOptionItem<ManagerRomance>
        {
            public MinTimeFromRomanceToBreakupOption()
                : base(2)
            { }

            public override string GetTitlePrefix()
            {
                return "CooldownPartnertoBreakup";
            }
        }

        public class RenameDivorceeOption : BooleanManagerOptionItem<ManagerRomance>
        {
            public RenameDivorceeOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "RenameDivorcee";
            }
        }

        public class Installer : ExpansionInstaller<ManagerRomance>
        {
            protected override bool PrivateInstall(ManagerRomance main, bool initial)
            {
                main.OnAllowBreakup += OnAllow;
                return true;
            }

            protected static bool OnAllow(Common.IStatGenerator stats, SimData sim, Managers.Manager.AllowCheck check)
            {
                SimDescription simDesc = sim.SimDescription;

                if (!sim.GetValue<AllowBreakupOption, bool>())
                {
                    stats.IncStat("Allow: Breakup Denied");
                    return false;
                }
                else if (!StoryProgression.Main.GetValue<AllowBreakupOption, bool>(simDesc.Partner))
                {
                    stats.IncStat("Allow: Breakup Partner Denied");
                    return false;
                }

                return true;
            }
        }
    }
}
