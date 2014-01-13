using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios.Skills;
using NRaas.StoryProgressionSpace.Scoring;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Managers
{
    public class ManagerSkill : Manager
    {
        public ManagerSkill(Main manager)
            : base (manager)
        { }

        public override string GetTitlePrefix(PrefixType type)
        {
            return "Skills";
        }

        public override void InstallOptions(bool initial)
        {
            new Installer<ManagerSkill>(this).Perform(initial);
        }

        protected override string IsOnActiveLot(SimDescription sim, bool testViewLot)
        {
            return null;
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, AllowCheck check)
        {
            check &= ~AllowCheck.Active;

            return base.PrivateAllow(stats, sim, check);
        }

        public bool Allow(IHasSkill stats, Sim sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IHasSkill stats, Sim sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }
        public bool Allow(IHasSkill stats, SimDescription sim)
        {
            return PrivateAllow(stats, sim);
        }
        public bool Allow(IHasSkill stats, SimDescription sim, AllowCheck check)
        {
            return PrivateAllow(stats, sim, check);
        }

        protected override bool PrivateAllow(IScoringGenerator stats, SimDescription sim, SimData settings, AllowCheck check)
        {
            if (!base.PrivateAllow(stats, sim, settings, check)) return false;

            if (sim.IsEP11Bot)
            {
                if (!sim.HasTrait(TraitNames.AbilityToLearnChip))
                {
                    stats.IncStat("Chip Denied");
                    return false;
                }
            }

            IHasSkill option = stats as IHasSkill;
            if (option != null)
            {
                foreach (SkillNames skill in option.CheckSkills)
                {
                    if (!AllowSkill(stats, sim, settings, skill)) return false;
                }
            }

            return true;
        }

        public delegate bool AllowSkillFunc(Common.IStatGenerator stats, SimData settings, SkillNames skill);

        public event AllowSkillFunc OnAllowSkill;

        public bool AllowSkill(Common.IStatGenerator stats, SimDescription sim, SimData settings, SkillNames skill)
        {
            if (OnAllowSkill != null)
            {
                return OnAllowSkill(stats, settings, skill);
            }
            else
            {
                return true;
            }
        }

        protected override void PrivateUpdate(bool fullUpdate, bool initialPass)
        {
            base.PrivateUpdate(fullUpdate, initialPass);
        }

        public InteractionInstance ApplyTimeLimit(InteractionInstance interaction, float hours)
        {
            if (interaction == null) return null;

            interaction.InstanceActor.AddAlarm(hours, TimeUnit.Hours, new TimeOut(interaction).OnProcess, "Timeout", AlarmType.NeverPersisted);
            return interaction;
        }

        public class TimeOut
        {
            Sim mSim;

            InteractionInstance mInteraction;

            public TimeOut(InteractionInstance interaction)
            {
                mSim = interaction.InstanceActor;
                mInteraction = interaction;
            }

            public void OnProcess()
            {
                try
                {
                    if (mSim.HasBeenDestroyed) return;

                    if (mSim.CurrentInteraction == mInteraction)
                    {
                        mSim.InteractionQueue.CancelInteraction(mInteraction, true);
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(mSim, e);
                }
            }
        }

        public void HandleMoochSkill(SimDescription sim, float incSkill)
        {
            Skill skill = sim.SkillManager.AddElement(SkillNames.Mooch);
            if (sim.Child)
            {
                skill.AddSkillPointsLevelClamped(incSkill, TraitTuning.MoochTraitChildSkill);
            }
            else if (sim.Teen)
            {
                skill.AddSkillPointsLevelClamped(incSkill, TraitTuning.MoochTraitTeenSkill);
            }
            else
            {
                skill.AddPoints(incSkill);
            }
        }

        public class Updates : AlertLevelOption<ManagerSkill>
        {
            public Updates()
                : base(AlertLevel.All)
            { }

            public override string GetTitlePrefix()
            {
                return "Stories";
            }
        }

        protected class DebugOption : DebugLevelOption<ManagerSkill>
        {
            public DebugOption()
                : base(Common.DebugLevel.Quiet)
            { }
        }

        protected class TicksPassedOption : TicksPassedBaseOption<ManagerSkill>
        {
            public TicksPassedOption()
            { }
        }

        protected class DumpScoringOption : DumpScoringBaseOption<ManagerSkill>
        {
            public DumpScoringOption()
            { }
        }

        public class SpeedOption : SpeedBaseOption<ManagerSkill>
        {
            public SpeedOption()
                : base(500, false)
            { }
        }
    }
}

