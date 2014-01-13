using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public abstract class SkillLevelBase : SimFromList
    {
        List<Item> mSelection = null;

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.SkillManager != null);
        }

        public static bool SimCanLearnSkill(SkillNames skillName, SimDescription simDesc)
        {
            Skill skill = SkillManager.GetStaticSkill(skillName);
            if (skill == null) return false;

            if ((CASUtils.CASAGSAvailabilityFlagsFromCASAgeGenderFlags(CASAgeGenderFlags.Adult | simDesc.Species) & skill.AvailableAgeSpecies) != CASAGSAvailabilityFlags.None)
            {
                return true;
            }

            if ((CASUtils.CASAGSAvailabilityFlagsFromCASAgeGenderFlags(CASAgeGenderFlags.Teen | simDesc.Species) & skill.AvailableAgeSpecies) != CASAGSAvailabilityFlags.None)
            {
                return true;
            }

            if ((CASUtils.CASAGSAvailabilityFlagsFromCASAgeGenderFlags(CASAgeGenderFlags.Child | simDesc.Species) & skill.AvailableAgeSpecies) != CASAGSAvailabilityFlags.None)
            {
                return true;
            }

            if ((CASUtils.CASAGSAvailabilityFlagsFromCASAgeGenderFlags(CASAgeGenderFlags.Toddler | simDesc.Species) & skill.AvailableAgeSpecies) != CASAGSAvailabilityFlags.None)
            {
                return true;
            }

            return false;
        }

        protected abstract List<Item> PrivateRun(SimDescription me, IEnumerable<Item> choices);

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                Dictionary<SkillNames, bool> lookup = new Dictionary<SkillNames, bool>();

                List<Item> allOptions = new List<Item>();
                foreach (Skill skill in SkillManager.SkillDictionary)
                {
                    if (lookup.ContainsKey(skill.Guid)) continue;
                    lookup.Add(skill.Guid, true);

                    int level = me.SkillManager.GetSkillLevel(skill.Guid);

                    if (singleSelection)
                    {
                        if (!SimCanLearnSkill(skill.Guid, me)) continue;
                    }

                    allOptions.Add(new Item(skill, level));
                }

                CommonSelection<Item>.Results selection = new CommonSelection<Item>(Name, allOptions, new AuxillaryColumn()).SelectMultiple();
                if ((selection == null) || (selection.Count == 0)) return false;

                mSelection = PrivateRun(me, selection);
            }

            foreach (Item choice in mSelection)
            {
                if (choice == null) continue;

                int currentLevel = me.SkillManager.GetSkillLevel(choice.Skill);

                if ((int)choice.Level == currentLevel) continue;

                Skill mySkill = me.SkillManager.GetElement(choice.Skill);
                if (choice.Level < 0)
                {
                    me.SkillManager.RemoveElement(choice.Skill);
                }
                else if (choice.Level < currentLevel)
                {
                    if ((choice.AllowDrop) && (mySkill != null))
                    {
                        mySkill.SkillLevel = (int)choice.Level;
                        mySkill.SkillPoints = 0;

                        mySkill.ChangeSkillProgress(choice.Level - (int)choice.Level);
                    }
                }
                else
                {
                    if (!SimCanLearnSkill(choice.Skill, me)) continue;

                    if ((mySkill == null) && (choice.Level > 0))
                    {
                        mySkill = me.SkillManager.AddElement(choice.Skill);
                    }

                    if (mySkill != null)
                    {
                        mySkill.ForceSkillLevelUp((int)choice.Level);
                        mySkill.ChangeSkillProgress(choice.Level - (int)choice.Level);
                    }
                }
            }

            return true;
        }

        public class Item : CommonOptionItem
        {
            Skill mSkill;
            
            float mLevel = 0;

            public Item(Skill skill, float level)
                : base(skill.Name, (int)level)
            {
                Skill staticSkill = SkillManager.GetStaticSkill(skill.Guid);
                if (staticSkill.NonPersistableData != null)
                {
                    if (string.IsNullOrEmpty(skill.Name))
                    {
                        mName = staticSkill.NonPersistableData.Name;
                    }

                    SetThumbnail(staticSkill.DreamsAndPromisesIconKey);
                }

                mSkill = skill;
                mLevel = level;
            }
            public Item(Item source)
                : base(source)
            {
                mSkill = source.mSkill;
                mLevel = source.mLevel;
            }

            public virtual bool AllowDrop
            {
                get { return true; }
            }

            public int MaximumLevel
            {
                get { return mSkill.MaxSkillLevel; }
            }

            public override string DisplayValue
            {
                get { return null; }
            }

            public SkillNames Skill
            {
                get { return mSkill.Guid; }
            }

            public virtual float Level
            {
                get { return mLevel; }
                set { mLevel = value; }
            }

            public string Auxillary
            {
                get
                {
                    if (mSkill.IsHiddenSkill(CASAGSAvailabilityFlags.None))
                    {
                        return Common.Localize("Type:Hidden");
                    }

                    return null;
                }
            }
        }

        public class AuxillaryColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
        {
            public AuxillaryColumn()
                : base("NRaas.MasterController.OptionList:TypeTitle", "NRaas.MasterController.OptionList:TypeTooltip", 40)
            { }

            public override ObjectPicker.ColumnInfo GetValue(Item item)
            {
                return new ObjectPicker.TextColumn(item.Auxillary);
            }
        }
    }
}
