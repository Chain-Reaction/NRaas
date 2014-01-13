using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Friends;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Propagation
{
    public class PropagateEnemyScenario : PropagateBuffScenario
    {
        protected SimDescription mEnemy;

        WhichSim mEnemySim = WhichSim.Unset;

        int mDelta;

        int mRelationshipGate = 40;

        public PropagateEnemyScenario()
            : base(BuffNames.Upset, Origin.FromWatchingSimSuffer)
        { }
        public PropagateEnemyScenario(SimDescription primary, SimDescription enemy, int delta)
            : this(primary, enemy, BuffNames.Upset, Origin.FromWatchingSimSuffer, delta)
        { }
        protected PropagateEnemyScenario(SimDescription primary, SimDescription enemy, BuffNames buff, Origin origin, int delta)
            : base(primary, buff, origin)
        {
            mEnemy = enemy;
            mDelta = delta;
        }
        protected PropagateEnemyScenario(PropagateEnemyScenario scenario)
            : base(scenario)
        {
            mEnemy = scenario.mEnemy;
            mEnemySim = scenario.mEnemySim;
            mDelta = scenario.mDelta;
            mRelationshipGate = scenario.mRelationshipGate;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Delta=" + mDelta;
            text += Common.NewLine + "EnemySim=" + mEnemySim;
            text += Common.NewLine + "RelationshipGate=" + mRelationshipGate;

            return text;
        }

        public override bool IsFriendly
        {
            get { return false; }
        }

        protected SimDescription Enemy
        {
            get { return mEnemy; }
        }

        protected override bool UsesSim(ulong sim)
        {
            if (mEnemy != null)
            {
                if (mEnemy.SimDescriptionId == sim) return true;
            }

            return base.UsesSim(sim);
        }

        public override bool Parse(XmlDbRow row, string prefix, bool firstPass, ref string error)
        {
            mDelta = row.GetInt(prefix + "PropagateDelta", -25);

            if (row.Exists(prefix + "PropagateEnemy"))
            {
                if (!ParserFunctions.TryParseEnum<WhichSim>(row.GetString(prefix + "PropagateEnemy"), out mEnemySim, WhichSim.Actor))
                {
                    error = prefix + "PropagateEnemy unknown";
                    return false;
                }
            }

            SimScenarioFilter.RelationshipLevel relationLevel;
            if (ParserFunctions.TryParseEnum<SimScenarioFilter.RelationshipLevel>(row.GetString(prefix + "PropagateRelationshipGate"), out relationLevel, SimScenarioFilter.RelationshipLevel.Neutral))
            {
                mRelationshipGate = (int)relationLevel;
            }
            else
            {
                mRelationshipGate = row.GetInt(prefix + "PropagateRelationshipGate", mRelationshipGate);
            }

            return base.Parse(row, prefix, firstPass, ref error);
        }

        public override bool PostParse(ref string error)
        {
            if (mEnemySim == WhichSim.Unset)
            {
                error = "PropagateEnemy Unset";
                return false;
            }

            return base.PostParse(ref error);
        }

        public override void SetActors(SimDescription actor, SimDescription target)
        {
            base.SetActors(actor, target);

            if (mEnemySim == WhichSim.Target)
            {
                mEnemy = target;
            }
            else
            {
                mEnemy = actor;
            }
        }

        protected override bool Allow()
        {
            if (!GetValue<ScheduledEnemyScenario.AllowEnemyOption, bool>()) return false;

            return base.Allow();
        }

        protected override bool TargetAllow(SimDescription sim)
        {
            if (sim == mEnemy)
            {
                IncStat("Enemy");
                return false;
            }
            else if (sim.Partner == mEnemy)
            {
                IncStat("Partner");
                return false;
            }

            return base.TargetAllow(sim);
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            if ((Origin == Origin.FromFire) && (ManagerSim.HasTrait(Target, TraitNames.Pyromaniac)))
            {
                ManagerSim.AddBuff(Target, BuffNames.Fascinated, Origin);
            }

            if ((Origin == Origin.FromTheft) && (ManagerSim.HasTrait(Target, TraitNames.Kleptomaniac)))
            {
                ManagerSim.AddBuff(Target, BuffNames.Fascinated, Origin);
            }

            if (ManagerSim.HasTrait(Target, TraitNames.Coward))
            {
                ManagerSim.AddBuff(Target, BuffNames.Scared, Origin);
            }

            if ((ManagerSim.HasTrait(Target, TraitNames.Evil)) ||
                (ManagerSim.HasTrait(Target, TraitNames.MeanSpirited)))
            {
                ManagerSim.AddBuff(Target, BuffNames.FiendishlyDelighted, Origin);
            }
            else
            {
                // Not performed for the main Sim on purpose
                if (Target != Sim)
                {
                    base.PrivateUpdate(frame);

                    bool fail = false;

                    Relationship relation = Relationship.Get(mEnemy, Target, false);
                    if (relation != null)
                    {
                        if (relation.LTR.Liking >= mRelationshipGate)
                        {
                            fail = true;
                        }
                    }

                    if (!fail)
                    {
                        int report = 0;
                        if (mEnemy.Partner == Target)
                        {
                            report = ReportChance;
                        }

                        Add(frame, new ExistingEnemyManualScenario(mEnemy, Target, mDelta, report, GetTitlePrefix(PrefixType.Story)), ScenarioResult.Start);
                    }
                }
            }

            return false;
        }

        public override Scenario Clone()
        {
            return new PropagateEnemyScenario(this);
        }
    }
}
