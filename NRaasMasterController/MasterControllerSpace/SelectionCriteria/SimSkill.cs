using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class SimSkill : SelectionTestableOptionList<SimSkill.Item, SimSkill.Values, SimSkill.Values>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Skill";
        }

        public struct Values
        {
            public readonly SkillNames mSkill;
            public readonly int mLevel;

            public Values(SkillNames skill, int level)
            {
                mSkill = skill;
                mLevel = level;
            }
        }

        public class Item : TestableOption<Values,Values>
        {
            public override void SetValue(Values value, Values storeType)
            {
                mValue = value;

                Skill skill = SkillManager.GetStaticSkill(Value.mSkill);
                if (skill != null)
                {
                    SetThumbnail(skill.DreamsAndPromisesIconKey);

                    mName = skill.ToString();
                    if ((string.IsNullOrEmpty(mName)) && (skill.NonPersistableData != null))
                    {
                        mName = skill.NonPersistableData.Name;
                    }
                }
                else
                {
                    mName = mValue.mSkill.ToString();
                }

                mName += Common.Localize("Criteria.Skill:Level", false, new object[] { Value.mLevel });
            }

            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<Values,Values> results)
            {
                if (me.SkillManager == null) return false;

                foreach (Skill skill in me.SkillManager.List)
                {
                    Values value = new Values(skill.Guid, skill.SkillLevel);

                    results[value] = value;
                }

                return true;
            }
        }
    }
}
