using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Deaths;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class FightScenarioHelper
    {
        WeightScenarioHelper mSuccess = null;
        WeightScenarioHelper mFailure = null;
        WeightScenarioHelper mExtremeFailure = null;

        int mBail = 0;

        bool mAllowGoToJail = false;

        bool mActorAllowInjury = false;
        bool mTargetAllowInjury = false;

        string mFightScoring = "Fight";

        Origin mOrigin = Origin.None;

        SimDescription.DeathType mDeathType = SimDescription.DeathType.Starve;

        IntegerOption.OptionValue mChanceOfDeath = null;

        public override string ToString()
        {
            string text = "-- FightScenarioHelper --";

            text += Common.NewLine + "Success" + Common.NewLine + mSuccess;
            text += Common.NewLine + "Failure" + Common.NewLine + mFailure;
            text += Common.NewLine + "ExtremeFailure" + Common.NewLine + mExtremeFailure;
            text += Common.NewLine + "AllowGoToJail=" + mAllowGoToJail;
            text += Common.NewLine + "Bail=" + mBail;
            text += Common.NewLine + "ActorAllowInjury=" + mActorAllowInjury;
            text += Common.NewLine + "TargetAllowInjury=" + mTargetAllowInjury;
            text += Common.NewLine + "FightScoring=" + mFightScoring;
            text += Common.NewLine + "Origin=" + mOrigin;
            text += Common.NewLine + "DeathType=" + mDeathType;
            text += Common.NewLine + "ChanceOfDeath=" + mChanceOfDeath;

            text += "-- End FightScenarioHelper --";

            return text;
        }

        public bool AllowGoToJail
        {
            get { return mAllowGoToJail; }
        }

        public HitMissResult<SimDescription,SimScoringParameters> SuccessDelta
        {
            get { return mSuccess.Delta; }
        }

        public FightScenarioHelper(Origin origin, SimDescription.DeathType deathType)
        {
            mOrigin = origin;
            mDeathType = deathType;
        }

        public bool Parse(XmlDbRow row, StoryProgressionObject manager, IUpdateManager updater, ref string error)
        {
            mFightScoring = row.GetString("FightScoring");

            if (!row.Exists("AllowGoToJail"))
            {
                error = "AllowGoToJail missing";
                return false;
            }

            mAllowGoToJail = row.GetBool("AllowGoToJail");

            mActorAllowInjury = row.GetBool("AllowInjury") || row.GetBool("ActorAllowInjury");
            mTargetAllowInjury = row.GetBool("AllowInjury") || row.GetBool("TargetAllowInjury");

            mChanceOfDeath = new IntegerOption.OptionValue(-1);

            if (row.Exists("ChanceOfDeath"))
            {
                if (!mChanceOfDeath.Parse(row, "ChanceOfDeath", manager, updater, ref error))
                {
                    return false;
                }
            }

            mBail = row.GetInt("Bail");

            mSuccess = new WeightScenarioHelper(mOrigin);
            if (!mSuccess.Parse(row, manager, updater, "Success", ref error))
            {
                return false;
            }

            mFailure = new WeightScenarioHelper(mOrigin);
            if (!mFailure.Parse(row, manager, updater, "Failure", ref error))
            {
                return false;
            }

            mExtremeFailure = new WeightScenarioHelper(mOrigin);
            if (!mExtremeFailure.Parse(row, manager, updater, "ExtremeFailure", ref error))
            {
                return false;
            }

            return true;
        }

        public bool ShouldPush(bool fail, bool defValue)
        {
            if (fail)
            {
                return mFailure.ShouldPush(defValue);
            }
            else
            {
                return mSuccess.ShouldPush(defValue);
            }
        }

        public delegate bool SuccessUpdateDelegate(ScenarioFrame frame);

        public bool Perform(Scenario scenario, ScenarioFrame frame, SimDescription sim, SimDescription target, SuccessUpdateDelegate successUpdate, out bool fail)
        {
            fail = false;

            if (!mSuccess.TestBeforehand(scenario.Manager, sim, target))
            {
                scenario.IncStat("Success TestBeforehand Fail");
                return false;
            }

            if (!mFailure.TestBeforehand(scenario.Manager, sim, target))
            {
                scenario.IncStat("Failure TestBeforehand Fail");
                return false;
            }

            if (!mExtremeFailure.TestBeforehand(scenario.Manager, sim, target))
            {
                scenario.IncStat("ExtremeFailure TestBeforehand Fail");
                return false;
            }

            int score = 0;

            if (!string.IsNullOrEmpty(mFightScoring))
            {
                score = scenario.AddScoring("Fight Sim", ScoringLookup.GetScore(mFightScoring, sim));
                score -= scenario.AddScoring("Fight Target", ScoringLookup.GetScore(mFightScoring, target));
            }

            if (score < 0)
            {
                if ((target.CreatedSim != null) && (target.OccultManager.HasOccultType(OccultTypes.Werewolf)))
                {
                    target.CreatedSim.BuffManager.AddElement(BuffNames.TopDog, Origin.FromWinningFight);
                }

                fail = true;

                if ((sim == scenario.Personalities.GetClanLeader(scenario.Manager)) && (RandomUtil.RandomChance(-score)))
                {
                    mExtremeFailure.Perform(scenario, frame, "ExtremeFailure", sim, target);
                }

                if (mAllowGoToJail)
                {
                    int bail = mBail;
                    if (bail == 0)
                    {
                        bail = scenario.Manager.GetValue<GotArrestedScenario.BailOption, int>() * 2;
                    }

                    scenario.Manager.AddAlarm(new GoToJailScenario(sim, bail));
                }
                else if (mActorAllowInjury)
                {
                    scenario.Manager.AddAlarm(new GoToHospitalScenario(sim, target, "InjuredFight", SimDescription.DeathType.OldAge));
                }

                mFailure.Perform(scenario, frame, "Failure", sim, target);
                return true;
            }
            else if ((successUpdate == null) || (successUpdate(frame)))
            {
                if ((sim.CreatedSim != null) && (sim.OccultManager.HasOccultType(OccultTypes.Werewolf)))
                {
                    sim.CreatedSim.BuffManager.AddElement(BuffNames.TopDog, Origin.FromWinningFight);
                }

                if (mTargetAllowInjury)
                {
                    scenario.Manager.AddAlarm(new GoToHospitalScenario(target, sim, "InjuredFight", mDeathType, mChanceOfDeath.Value));
                }

                scenario.Add(frame, new PropagateWonFightScenario(sim, target), ScenarioResult.Start);
                scenario.Add(frame, new PropagateClanDelightScenario(sim, scenario.Manager, Origin.FromTheft), ScenarioResult.Start);

                mSuccess.Perform(scenario, frame, "Success", sim, target);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
