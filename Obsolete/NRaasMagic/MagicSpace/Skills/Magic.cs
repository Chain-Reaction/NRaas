using NRaas.CommonSpace.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NRaas.MagicSpace.Skills
{
    [Persistable]
    public class Magic : CommonSkill<Magic, Magic.MagicMajorStat, Magic.MagicMinorStat, Magic.MagicOpportunity>
    {
        [Tunable, TunableComment("Number of spells cast to receive the Wizard tile")]
        private static int kNumberOfSpellsNeededForWizard = 50;
        [Tunable, TunableComment("Number of spells cast to receive the Warlock tile")]
        private static int kNumberOfSpellsNeededForWarlock = 50;

        [Tunable, TunableComment("Level at which Take Out Trash is unlocked")]
        private static int kTakeOutTrashLevel = 1;
        [Tunable, TunableComment("Level at which Dirty Object is unlocked")]
        private static int kDirtyObjectLevel = 1;
        
        [Tunable, TunableComment("Level at which Set On Fire is unlocked")]
        private static int kSetFireLevel = 2;
        [Tunable, TunableComment("Level at which Clean Object is unlocked")]
        private static int kCleanObjectLevel = 2;

        [Tunable, TunableComment("Level at which Break Object is unlocked")]
        private static int kBreakObjectLevel = 3;
        
        [Tunable, TunableComment("Level at which Repair Object is unlocked")]
        private static int kRepairObjectLevel = 4;
        [Tunable, TunableComment("Level at which Bless Extra Creative is unlocked")]
        private static int kBlessExtraCreativeLevel = 4;

        [Tunable, TunableComment("Level at which Bless Attractive is unlocked")]
        private static int kBlessAttractiveLevel = 5;

        [Tunable, TunableComment("Level at which Destroy Object is unlocked")]
        private static int kDestroyObjectLevel = 6;
        [Tunable, TunableComment("Level at which Bless Fast Learner is unlocked")]
        private static int kBlessFastLearnerLevel = 6;

        [Tunable, TunableComment("Level at which Bless Observant is unlocked")]
        private static int kBlessObservantLevel = 7;

        [Tunable, TunableComment("Level at which Bless Fertility Treatment is unlocked")]
        private static int kBlessFertilityTreatmentLevel = 8;

        [Tunable, TunableComment("Level at which Bless Long Life is unlocked")]
        private static int kBlessLongLifeLevel = 9;

        [Tunable, TunableComment("Level at which Kill Sim is unlocked")]
        private static int kKillSimLevel = 10;

        [Persistable]
        public class MagicSettings
        {
            public int mNumberOfSpellsNeededForWizard = kNumberOfSpellsNeededForWizard;
            public int mNumberOfSpellsNeededForWarlock = kNumberOfSpellsNeededForWarlock;

            public int mDirtyObjectLevel = kDirtyObjectLevel;
            public int mTakeOutTrashLevel = kTakeOutTrashLevel;
            public int mSetFireLevel = kSetFireLevel;
            public int mCleanObjectLevel = kCleanObjectLevel;
            public int mBreakObjectLevel = kBreakObjectLevel;
            public int mRepairObjectLevel = kRepairObjectLevel;
            public int mBlessExtraCreativeLevel = kBlessExtraCreativeLevel;
            public int mDestroyObjectLevel = kDestroyObjectLevel;
            public int mBlessFastLearnerLevel = kBlessFastLearnerLevel;
            public int mBlessAttractiveLevel = kBlessAttractiveLevel;
            public int mBlessObservantLevel = kBlessObservantLevel;
            public int mBlessFertilityTreatmentLevel = kBlessFertilityTreatmentLevel;
            public int mBlessLongLifeLevel = kBlessLongLifeLevel;
            public int mKillSimLevel = kKillSimLevel;
        }

        [PersistableStatic]
        static MagicSettings sSettings = null;

        public int mEvilSpellsCasted;
        public int mGoodSpellsCasted;

        public Magic()
        { }
        public Magic(SkillNames guid)
            : base(guid)
        { }

        public static MagicSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new MagicSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        protected override string LocalizationKey
        {
            get { return "NRaasMagic"; }
        }

        public bool IncreaseEvilSpellCount()
        {
            mEvilSpellsCasted++;
            base.TestForNewLifetimeOpp();
            return true;
        }

        public bool IncreaseGoodSpellCount()
        {
            mGoodSpellsCasted++;
            base.TestForNewLifetimeOpp();
            return true;
        }

        public bool IsMasterWizard()
        {
            return (mGoodSpellsCasted >= Settings.mNumberOfSpellsNeededForWizard);
        }

        public bool IsWarlock()
        {
            return (mEvilSpellsCasted >= Settings.mNumberOfSpellsNeededForWarlock);
        }

        public override Skill Clone(SimDescription owner)
        {
            Magic skill = base.Clone(owner) as Magic;

            skill.MergeTravelData(this);

            return skill;
        }

        public override void MergeTravelData(Skill skill)
        {
            base.MergeTravelData(skill);

            Magic magic = skill as Magic;

            mGoodSpellsCasted = magic.mGoodSpellsCasted;
            mEvilSpellsCasted = magic.mEvilSpellsCasted;
        }

        public abstract class MagicMajorStat : MajorStat
        { }


        public abstract class MagicMinorStat : MinorStat
        { }

        public class EvilSpellsCast : MagicMajorStat
        {
            public EvilSpellsCast()
            { }

            protected override string LocalizationKey
            {
                get { return "EvilSpellsCasted"; }
            }

            public override int Count
            {
                get { return mSkill.mEvilSpellsCasted; }
            }

            public override int Order
            {
                get { return 20; }
            }

            public override MagicMajorStat Clone()
            {
                return new EvilSpellsCast();
            }
        }

        public class GoodSpellsCast : MagicMajorStat
        {
            public GoodSpellsCast()
            { }

            protected override string LocalizationKey
            {
                get { return "GoodSpellsCasted"; }
            }

            public override int Count
            {
                get { return mSkill.mGoodSpellsCasted; }
            }

            public override int Order
            {
                get { return 10; }
            }

            public override MagicMajorStat Clone()
            {
                return new GoodSpellsCast();
            }
        }

        public abstract class MagicOpportunity : CommonOpportunity
        {
            public MagicOpportunity()
            { }
        }

        public class MasterWizard : MagicOpportunity
        {
            public MasterWizard()
            { }

            protected override string LocalizationKey
            {
                get { return "MasterWizard"; }
            }

            public override int MinValue
            {
                get { return Settings.mNumberOfSpellsNeededForWizard; }
            }

            public override int CurrentValue
            {
                get { return mSkill.mGoodSpellsCasted; }
            }

            public override MagicOpportunity Clone()
            {
                return new MasterWizard();
            }
        }

        public class Warlock : MagicOpportunity
        {
            public Warlock()
            { }

            protected override string LocalizationKey
            {
                get { return "Warlock"; }
            }

            public override int MinValue
            {
                get { return Settings.mNumberOfSpellsNeededForWarlock; }
            }

            public override int CurrentValue
            {
                get { return mSkill.mEvilSpellsCasted; }
            }

            public override MagicOpportunity Clone()
            {
                return new Warlock();
            }
        }
    }
}

