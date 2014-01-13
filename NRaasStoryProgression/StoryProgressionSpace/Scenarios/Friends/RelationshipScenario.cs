using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Romances;
using NRaas.StoryProgressionSpace.Scoring;
using NRaas.StoryProgressionSpace.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
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
    public abstract class RelationshipScenario : DualSimScenario, IFriendlyScenario, IDeltaScenario, IRomanticScenario
    {
        HitMissResult<SimDescription, SimScoringParameters> mDelta = null;

        bool mRomantic = false;

        protected RelationshipScenario(int delta)
        {
            mDelta = new HitMissResult<SimDescription, SimScoringParameters>(delta);
        }
        protected RelationshipScenario(SimDescription sim, int delta)
            : base(sim)
        {
            mDelta = new HitMissResult<SimDescription, SimScoringParameters>(delta);
        }
        protected RelationshipScenario(SimDescription sim, SimDescription target, int delta)
            : base (sim, target)
        {
            mDelta = new HitMissResult<SimDescription, SimScoringParameters>(delta);
        }
        protected RelationshipScenario(RelationshipScenario scenario)
            : base (scenario)
        {
            mDelta = scenario.mDelta;
            mRomantic = scenario.mRomantic;
        }

        public override string ToString()
        {
            string text = base.ToString();

            text += Common.NewLine + "Delta=" + mDelta;
            text += Common.NewLine + "Romantic=" + mRomantic;

            return text;
        }

        public abstract bool IsFriendly
        {
            get;
        }

        protected override bool TestOpposing
        {
            get { return IsFriendly; }
        }

        public HitMissResult<SimDescription, SimScoringParameters> IDelta
        {
            get
            {
                return mDelta;
            }
            set
            {
                mDelta = value;
            }
        }

        protected int Delta
        {
            get 
            {
                return mDelta.Score(new SimScoringParameters(Target));
            }
        }

        public virtual bool IsRomantic
        {
            get { return mRomantic; }
        }

        protected override bool Progressed
        {
            get { return true; }
        }

        protected override int ContinueChance
        {
            get { return 0; }
        }

        protected abstract bool TestRelationship
        {
            get;
        }

        public override bool Parse(XmlDbRow row, ref string error)
        {
            mDelta = new HitMissResult<SimDescription, SimScoringParameters>(row, "Delta", ref error);
            if (!string.IsNullOrEmpty(error))
            {
                return false;
            }

            mRomantic = row.GetBool("Romantic");

            return base.Parse(row, ref error);
        }

        protected override bool AllowSpecies(SimDescription sim)
        {
            return true;
        }

        protected override bool CommonAllow(SimDescription sim)
        {
            if (!Friends.Allow(this, sim, AllowActive ? Managers.Manager.AllowCheck.None : Managers.Manager.AllowCheck.Active))
            {
                IncStat("User Denied");
                return false;
            }
            else if (SimTypes.IsDead(sim))
            {
                IncStat("Dead");
                return false;
            }

            return base.CommonAllow(sim);
        }

        protected override bool TargetAllow(SimDescription target)
        {
            if ((!IsFriendly) && (Sim.ToddlerOrBelow))
            {
                IncStat("Too Young");
                return false;
            }
            else if ((IsRomantic) && (Sim.TeenOrAbove != target.TeenOrAbove))
            {
                IncStat("Wrong Age");
                return false;
            }
            else if ((IsFriendly) && (TestRelationship) && (ManagerSim.GetLTR(Sim, Target) >= 100))
            {
                IncStat("Max Liking");
                return false;
            }
            else if (Delta < 0)
            {
                if (target.ToddlerOrBelow)
                {
                    IncStat("Too Young");
                    return false;
                }
                else if ((TestRelationship) && (ManagerSim.GetLTR(Sim, Target) <= -100))
                {
                    IncStat("Min Liking");
                    return false;
                }
                else if (!Friends.AllowEnemy(this, Sim, Target, Managers.Manager.AllowCheck.None))
                {
                    return false;
                }
                else if ((!GetValue<AllowEnemyFamilyOption, bool>()) && 
                    ((Sim.Partner == Target) || (Relationships.IsCloselyRelated(Sim, Target, false))))
                {
                    IncStat("Closely Related Denied");
                    return false;
                }
            }
            else if (Delta > 0)
            {
                if ((TestRelationship) && (target.CreatedSim != null) && (SnubManager.IsSnubbing(target.CreatedSim, target)))
                {
                    IncStat("Snubbing");
                    return false;
                }
                else if (!Friends.AllowFriend(this, Sim, Target, Managers.Manager.AllowCheck.None))
                {
                    return false;
                }
                else if ((IsRomantic) && (!Flirts.Allow(this, Sim, Target)))
                {
                    return false;
                }
            }

            return base.TargetAllow(target);
        }

        protected bool AlterRelationship()
        {
            Relationship relation = ManagerSim.GetRelationship(Sim, Target);
            if (relation == null)
            {
                IncStat("No Relation");
                return false;
            }

            int delta = Delta;
            if (delta == 0)
            {
                IncStat("No Delta");
                return false;
            }

            if ((delta > 0) && (delta < 10))
            {
                delta = 10;
            }
            else if ((delta > -10) && (delta < 0))
            {
                delta = -10;
            }

            AddScoring("Delta", delta);

            try
            {
                int celebrity = GetValue<HobnobCelebrityPointsOption, int>();

                if (delta > 0)
                {
                    ManagerSim.AddBuff(Manager, Sim, BuffNames.Delighted, Origin.FromSocialization);
                    ManagerSim.AddBuff(Manager, Target, BuffNames.Delighted, Origin.FromSocialization);

                    delta = RandomUtil.GetInt(delta);

                    float value = 125;
                    if (delta > 75)
                    {
                        value = 1000;

                        celebrity *= 3;
                    }
                    else if (delta > 50)
                    {
                        value = 500;

                        celebrity *= 2;
                    }
                    else if (delta > 25)
                    {
                        value = 250;
                    }

                    if (IsRomantic)
                    {
                        relation.STC.Update(Sim, Target, CommodityTypes.Amorous, value);
                    }

                    relation.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.CelebrityImpressed);

                    relation.LTR.UpdateLiking(delta);
                }
                else
                {
                    if (GetValue<ExtremeHatredOption, bool>())
                    {
                        delta *= 2;
                    }

                    ManagerSim.AddBuff(Manager, Sim, BuffNames.Upset, Origin.FromSocialization);
                    ManagerSim.AddBuff(Manager, Target, BuffNames.Upset, Origin.FromSocialization);

                    delta = -RandomUtil.GetInt(-delta);

                    float value = 125;
                    if (delta < -75)
                    {
                        value = 1000;

                        celebrity *= 3;
                    }
                    else if (delta < -50)
                    {
                        value = 500;

                        celebrity *= 2;
                    }
                    else if (delta < -25)
                    {
                        value = 250;
                    }

                    relation.STC.Update(Sim, Target, CommodityTypes.Insulting, value);

                    relation.LTR.UpdateLiking(delta);
                }

                if (Sim.CelebrityLevel > Target.CelebrityLevel)
                {
                    Friends.AccumulateCelebrity(Target, celebrity);
                }
                else if (Sim.CelebrityLevel < Target.CelebrityLevel)
                {
                    Friends.AccumulateCelebrity(Sim, celebrity);
                }
            }
            catch (Exception e)
            {
                Common.DebugException(Sim, Target, e);
            }

            AddStat("Liking", relation.LTR.Liking);
            return true;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            return AlterRelationship();
        }

        protected static GoToLotSituation.FirstActionDelegate DetermineFirstAction(int delta, bool romantic)
        {
            if (delta < 0)
            {
                return ManagerFriendship.EnemyFirstAction;
            }
            else if (romantic)
            {
                return ManagerFlirt.FirstAction;
            }
            else
            {
                return ManagerFriendship.FriendlyFirstAction;
            }
        }

        protected virtual GoToLotSituation.FirstActionDelegate FirstAction
        {
            get
            {
                return DetermineFirstAction(Delta, IsRomantic);
            }
        }

        protected override bool Push()
        {
            if (Sim.ChildOrAbove)
            {
                if (Target.ChildOrAbove)
                {
                    return Situations.PushMeetUp(this, Sim, Target, ManagerSituation.MeetUpType.Commercial, FirstAction);
                }
                else
                {
                    return Situations.PushVisit(this, Sim, Target.LotHome);
                }
            }
            else if (Target.ChildOrAbove)
            {
                return Situations.PushVisit(this, Target, Sim.LotHome);
            }
            else
            {
                return false;
            }
        }

        public class ExtremeHatredOption : BooleanManagerOptionItem<ManagerFriendship>, IDebuggingOption
        {
            public ExtremeHatredOption()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "ExtremeHatred";
            }
        }

        public class AllowEnemyFamilyOption : BooleanManagerOptionItem<ManagerFriendship>
        {
            public AllowEnemyFamilyOption()
                : base(true)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowEnemyFamily";
            }
        }

        public class HobnobCelebrityPointsOption : IntegerManagerOptionItem<ManagerFriendship>, ManagerFriendship.ICelebrityOption
        {
            public HobnobCelebrityPointsOption()
                : base(25)
            { }

            public override string GetTitlePrefix()
            {
                return "HobnobCelebrityPoints";
            }

            public override bool HasRequiredVersion()
            {
                return GameUtils.IsInstalled(ProductVersion.EP3);
            }
        }
    }
}
