using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scenarios.Propagation;
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
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Personalities
{
    public class WeightScenarioHelper : IUpdateManagerOption
    {
        Scenario mScenario;

        HitMissResult<SimDescription, SimScoringParameters> mDelta;

        PropagateBuffScenario.WhichSim mScenarioActor;
        PropagateBuffScenario.WhichSim mScenarioTarget;

        SimRecruitFilter mRecruit = null;

        PropagationScenarioHelper mPropagate = null;

        Origin mOrigin = Origin.None;

        Dictionary<BuffNames, Origin> mActorBuffs = null;
        Dictionary<BuffNames, Origin> mTargetBuffs = null;

        int mActorCelebrity = 0;
        int mTargetCelebrity = 0;

        List<AccumulatorLoadValue> mAccumulators = new List<AccumulatorLoadValue>();
        List<AccumulatorLoadValue> mGatheringFailAccumulators = new List<AccumulatorLoadValue>();

        List<SimPersonality.IAccumulatorValue> mAccumulatorOptions = null;

        List<SimPersonality.IAccumulatorValue> mGatheringFailAccumulatorOptions = null;

        bool mTestBeforehand = false;

        public WeightScenarioHelper(Origin origin)
        {
            mOrigin = origin;
        }

        public HitMissResult<SimDescription, SimScoringParameters> Delta
        {
            get { return mDelta; }
        }

        public void PerformGatheringFailure()
        {
            if (mGatheringFailAccumulatorOptions == null) return;

            foreach (SimPersonality.IAccumulatorValue value in mGatheringFailAccumulatorOptions)
            {
                value.ApplyAccumulator();
            }
        }

        public bool ShouldPush (bool defValue)
        {
            if (mScenario == null) return defValue;

            return !mScenario.ShouldPush;
        }

        public override string ToString()
        {
            string text = "-- WeightScenarioHelper --";

            text += Common.NewLine + "Scenario=";

            if (mScenario != null)
            {
                text += mScenario.UnlocalizedName;
            }

            text += Common.NewLine + "Delta=" + mDelta;
            text += Common.NewLine + "ScenarioActor=" + mScenarioActor;
            text += Common.NewLine + "ScenarioTarget=" + mScenarioTarget;

            text += Common.NewLine + "Recruit" + Common.NewLine + mRecruit;
            text += Common.NewLine + "Propagate" + Common.NewLine + mPropagate;
            text += Common.NewLine + "Origin=" + mOrigin;
            text += Common.NewLine + "ActorCelebrity=" + mActorCelebrity;
            text += Common.NewLine + "TargetCelebrity=" + mTargetCelebrity;
            text += Common.NewLine + "Test=" + mTestBeforehand;

            if (mAccumulatorOptions != null)
            {
                foreach (SimPersonality.IAccumulatorValue value in mAccumulatorOptions)
                {
                    text += Common.NewLine + "Accumulator=" + value;
                }
            }

            if (mGatheringFailAccumulatorOptions != null)
            {
                foreach (SimPersonality.IAccumulatorValue value in mGatheringFailAccumulatorOptions)
                {
                    text += Common.NewLine + "GatheringFail=" + value;
                }
            }

            foreach (KeyValuePair<BuffNames, Origin> value in mActorBuffs)
            {
                text += Common.NewLine + "ActorBuffs=" + value.Key + "," + value.Value;
            }

            foreach (KeyValuePair<BuffNames, Origin> value in mTargetBuffs)
            {
                text += Common.NewLine + "TargetBuffs=" + value.Key + "," + value.Value;
            }

            text += Common.NewLine + "-- End WeightScenarioHelper --";

            return text;
        }

        public void UpdateManager(StoryProgressionObject manager)
        {
            mAccumulatorOptions = new List<SimPersonality.IAccumulatorValue>();

            foreach (AccumulatorLoadValue name in mAccumulators)
            {
                IntegerOption option = manager.GetOption<IntegerOption>(name.mName);
                if (option == null) continue;

                if (name.mReset)
                {
                    mAccumulatorOptions.Add(new IntegerOption.ResetValue(option));
                }
                else
                {
                    mAccumulatorOptions.Add(new IntegerOption.OptionValue(option, name.mValue));
                }
            }

            mGatheringFailAccumulatorOptions = new List<SimPersonality.IAccumulatorValue>();

            foreach (AccumulatorLoadValue name in mGatheringFailAccumulators)
            {
                IntegerOption option = manager.GetOption<IntegerOption>(name.mName);
                if (option == null) continue;

                if (name.mReset)
                {
                    mGatheringFailAccumulatorOptions.Add(new IntegerOption.ResetValue(option));
                }
                else
                {
                    mGatheringFailAccumulatorOptions.Add(new IntegerOption.OptionValue(option, name.mValue));
                }
            }
        }

        public bool Parse(XmlDbRow row, StoryProgressionObject manager, IUpdateManager updater, string prefix, ref string error)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                if (!Parse(row, manager, updater, null, ref error))
                {
                    return false;
                }

                mTestBeforehand = row.GetBool(prefix + "TestBeforehand");

                mDelta = new HitMissResult<SimDescription, SimScoringParameters>(row, prefix + "Delta", ref error);

                bool deltaSet = string.IsNullOrEmpty(error);

                if (!deltaSet)
                {
                    error = null;
                }

                string scenario = row.GetString(prefix + "Scenario");
                if (!string.IsNullOrEmpty(scenario))
                {
                    WeightOption scenarioWeight = manager.GetOption<WeightOption>(scenario);
                    if (scenarioWeight == null)
                    {
                        error = prefix + "Scenario weight " + scenario + " missing";
                        return false;
                    }

                    mScenario = scenarioWeight.GetScenario();
                    if (mScenario == null)
                    {
                        error = prefix + "Scenario " + scenario + " invalid";
                        return false;
                    }

                    if (deltaSet)
                    {
                        IDeltaScenario deltaScenario = mScenario as IDeltaScenario;
                        if (deltaScenario != null)
                        {
                            deltaScenario.IDelta = mDelta;
                        }
                    }
                }

                mRecruit = new SimRecruitFilter();
                if (!mRecruit.Parse(row, manager, updater, prefix, ref error))
                {
                    return false;
                }

                mPropagate = new PropagationScenarioHelper();
                if (!mPropagate.Parse(row, mOrigin, prefix, ref error))
                {
                    return false;
                }

                mActorBuffs = new Dictionary<BuffNames, Origin>();
                if (!ParseBuffs(row, prefix, "Actor", mActorBuffs, ref error))
                {
                    return false;
                }

                mTargetBuffs = new Dictionary<BuffNames, Origin>();
                if (!ParseBuffs(row, prefix, "Target", mTargetBuffs, ref error))
                {
                    return false;
                }
            }

            if (row.Exists(prefix + "ScenarioActor"))
            {
                if (!row.TryGetEnum<PropagateBuffScenario.WhichSim>(prefix + "ScenarioActor", out mScenarioActor, PropagateBuffScenario.WhichSim.Unset))
                {
                    error = prefix + "ScenarioActor Unknown " + row.GetString(prefix + "ScenarioActor");
                    return false;
                }
            }

            if (row.Exists(prefix + "ScenarioTarget"))
            {
                if (!row.TryGetEnum<PropagateBuffScenario.WhichSim>(prefix + "ScenarioTarget", out mScenarioTarget, PropagateBuffScenario.WhichSim.Unset))
                {
                    error = prefix + "ScenarioTarget Unknown " + row.GetString(prefix + "ScenarioTarget");
                    return false;
                }
            }

            mActorCelebrity = row.GetInt(prefix + "ActorCelebrity", mActorCelebrity);
            mTargetCelebrity = row.GetInt(prefix + "TargetCelebrity", mTargetCelebrity);

            if (!ParseAccumulator(row, prefix + "AccumulateValue", mAccumulators, ref error))
            {
                return false;
            }

            if (!ParseAccumulator(row, prefix + "GatheringFailureValue", mGatheringFailAccumulators, ref error))
            {
                return false;
            }

            updater.AddUpdater(this);
            return true;
        }

        protected bool ParseAccumulator(XmlDbRow row, string prefix, List<AccumulatorLoadValue> accumulators, ref string error)
        {
            for (int i = 0; i < 10; i++)
            {
                string key = prefix + i;
                if (!row.Exists(key)) break;

                string value = row.GetString(key);
                if (string.IsNullOrEmpty(value))
                {
                    error = prefix + i + " empty";
                    return false;
                }

                string[] values = value.Split(',');
                if (values.Length != 2)
                {
                    error = prefix + i + " not properly formatted";
                    return false;
                }

                string name = values[0].Trim();

                bool reset = false;

                int accumulator = 0;

                if (values[1].Trim().ToLower() == "reset")
                {
                    reset = true;
                }
                else
                {
                    if (!int.TryParse(values[1].Trim(), out accumulator))
                    {
                        error = prefix + i + " second parameter not integer";
                        return false;
                    }
                }

                mAccumulators.Add(new AccumulatorLoadValue(name, accumulator, reset));
            }

            return true;
        }

        protected bool ParseBuffs(XmlDbRow row, string prefix1, string prefix2, Dictionary<BuffNames,Origin> buffs, ref string error)
        {
            if ((!string.IsNullOrEmpty(prefix2)) && (!ParseBuffs(row, prefix1, null, buffs, ref error)))
            {
                return false;
            }

            string prefix = prefix1 + prefix2;

            int index = 0;
            while (true)
            {
                if (!row.Exists(prefix + "Buff" + index)) break;

                BuffNames buff;
                if (!ParserFunctions.TryParseEnum<BuffNames>(row.GetString(prefix + "Buff" + index), out buff, BuffNames.Undefined))
                {
                    error = prefix + "Buff" + index + " unknown";
                    return false;
                }

                if (buffs.ContainsKey(buff))
                {
                    error = prefix + "Buff " + buff + " already found";
                    return false;
                }

                if (!row.Exists(prefix + "Origin" + index))
                {
                    error = prefix + "Origin" + index + " missing";
                    return false;
                }

                Origin origin;
                if (!ParserFunctions.TryParseEnum<Origin>(row.GetString(prefix + "Origin" + index), out origin, Origin.None))
                {
                    error = prefix + "Origin" + index + " unknown";
                    return false;
                }

                buffs.Add(buff, origin);

                index++;
            }

            return true;
        }

        protected static void HandleBuff(Sim sim, BuffNames buff, Origin origin)
        {
            if (sim == null) return;

            if (sim.BuffManager == null) return;

            switch (buff)
            {
                case BuffNames.Sated:
                case BuffNames.SanguineSnack:
                    sim.BuffManager.RemoveElement(BuffNames.HeatingUp);
                    sim.BuffManager.RemoveElement(BuffNames.TooMuchSun);

                    sim.Motives.SetMax(CommodityKind.VampireThirst);
                    break;
            }

            sim.BuffManager.AddElement(buff, origin);
        }

        protected SimScenario GetNewScenario(StoryProgressionObject manager, SimDescription sim, SimDescription target)
        {
            if (mScenario == null) return null;

            SimScenario simScenario = mScenario.Clone() as SimScenario;

            simScenario.Manager = manager;

            switch (mScenarioActor)
            {
                case PropagateBuffScenario.WhichSim.Actor:
                    simScenario.Sim = sim;
                    break;
                case PropagateBuffScenario.WhichSim.Target:
                    simScenario.Sim = target;
                    break;
                case PropagateBuffScenario.WhichSim.NotActor:
                    simScenario.NotSim = sim;
                    break;
                case PropagateBuffScenario.WhichSim.NotTarget:
                    simScenario.NotSim = target;
                    break;
            }

            DualSimScenario dualSimScenario = simScenario as DualSimScenario;
            if (dualSimScenario != null)
            {
                switch (mScenarioTarget)
                {
                    case PropagateBuffScenario.WhichSim.Actor:
                        dualSimScenario.Target = sim;
                        break;
                    case PropagateBuffScenario.WhichSim.Target:
                        dualSimScenario.Target = target;
                        break;
                    case PropagateBuffScenario.WhichSim.NotActor:
                        dualSimScenario.NotTarget = sim;
                        break;
                    case PropagateBuffScenario.WhichSim.NotTarget:
                        dualSimScenario.NotTarget = target;
                        break;
                }
            }

            return simScenario;
        }

        public bool TestBeforehand(StoryProgressionObject manager, SimDescription sim, SimDescription target)
        {
            if (!mTestBeforehand) return true;

            SimScenario scenario = GetNewScenario(manager, sim, target);
            if (scenario == null) return true;

            if (scenario.GetAllowedSims().Count == 0) return false;

            DualSimScenario dualScenario = scenario as DualSimScenario;
            if (dualScenario != null)
            {
                bool found = false;

                foreach (SimDescription actor in scenario.GetAllowedSims())
                {
                    if (dualScenario.GetAllowedTargets(actor).Count > 0)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
            }

            return true;
        }

        public void Perform(Scenario scenario, ScenarioFrame frame, string name, SimDescription sim, SimDescription target)
        {
            scenario.IncStat(name);

            foreach (KeyValuePair<BuffNames, Origin> value in mActorBuffs)
            {
                HandleBuff(sim.CreatedSim, value.Key, value.Value);
            }

            foreach (KeyValuePair<BuffNames, Origin> value in mTargetBuffs)
            {
                HandleBuff(target.CreatedSim, value.Key, value.Value);
            }

            mRecruit.Perform(scenario, sim, target);

            mPropagate.Perform(scenario, frame, sim, target);

            int minCelebrity = scenario.GetValue<ManagerPersonality.MinCelebrityOption, int>();

            if (minCelebrity > 0)
            {
                SimPersonality personality = scenario.Manager as SimPersonality;

                int actorCelebrity = mActorCelebrity;
                if (actorCelebrity < minCelebrity)
                {
                    actorCelebrity = minCelebrity;
                }

                if (actorCelebrity > 0)
                {
                    scenario.Friends.AccumulateCelebrity(sim, actorCelebrity);
                }

                if ((personality != null) && (personality.Me != sim))
                {
                    scenario.Friends.AccumulateCelebrity(personality.Me, actorCelebrity);
                }

                int targetCelebrity = mTargetCelebrity;
                if (targetCelebrity < minCelebrity)
                {
                    targetCelebrity = minCelebrity;
                }

                if (targetCelebrity > 0)
                {
                    scenario.Friends.AccumulateCelebrity(target, targetCelebrity);
                }
            }

            if (mAccumulatorOptions != null)
            {
                foreach (SimPersonality.IAccumulatorValue value in mAccumulatorOptions)
                {
                    value.ApplyAccumulator();
                }
            }

            Scenario newScenario = GetNewScenario(scenario.Manager, sim, target);
            if (newScenario != null)
            {
                scenario.IncStat(newScenario.UnlocalizedName);

                scenario.Add(frame, newScenario, ScenarioResult.Start);
            }

            SimPersonality clan = scenario.Manager as SimPersonality;
            if ((sim != clan.Me) && (clan != null) && (clan.IsFriendlyLeadership))
            {
                int delta = mDelta.Score(new DualSimScoringParameters(sim, clan.Me));

                if (delta < 0)
                {
                    scenario.IncStat("ExistingEnemy");

                    scenario.Add(frame, new ExistingEnemyManualScenario(sim, clan.Me, delta, 0), ScenarioResult.Start);
                }
                else if (delta > 0)
                {
                    scenario.IncStat("ExistingFriend");

                    scenario.Add(frame, new ExistingFriendManualScenario(sim, clan.Me, delta, 0), ScenarioResult.Start);
                }
            }

            scenario.Add(frame, new SuccessScenario(), ScenarioResult.Start);
        }

        public class AccumulatorLoadValue
        {
            public readonly string mName;

            public readonly int mValue;

            public readonly bool mReset;

            public AccumulatorLoadValue(string name, int value, bool reset)
            {
                mName = name;
                mValue = value;
                mReset = reset;
            }
        }
    }
}
