using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.Settings;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class ApplySkillStamp : SimFromList, IAdvancedOption
    {
        SkillStamp mChoice;

        public override string GetTitlePrefix()
        {
            return "SkillStamp";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (NRaas.MasterController.Settings.SkillStamps.Count == 0) return false;

            return base.Allow(parameters);
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.SkillManager == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (MasterController.Settings.SkillStamps.Count == 1)
                {
                    if (!AcceptCancelDialog.Show(Common.Localize("SkillStamp:Prompt", me.IsFemale, new object[] { me })))
                    {
                        return false;
                    }

                    mChoice = MasterController.Settings.SkillStamps[0];
                }
                else
                {
                    List<Item> allOptions = new List<Item>();
                    foreach (SkillStamp stamp in MasterController.Settings.SkillStamps)
                    {
                        allOptions.Add(new ApplySkillStamp.Item(stamp));
                    }

                    Item choice = new CommonSelection<ApplySkillStamp.Item>(Name, allOptions).SelectSingle();
                    if (choice == null) return false;

                    mChoice = choice.mStamp;
                }
            }

            foreach (KeyValuePair<SkillNames, int> skill in mChoice.Skills)
            {
                if (me.SkillManager.GetSkillLevel(skill.Key) > skill.Value)
                {
                    continue;
                }

                Skill mySkill = me.SkillManager.GetElement(skill.Key);
                if ((mySkill == null) && (skill.Value >= 0))
                {
                    mySkill = me.SkillManager.AddElement(skill.Key);
                }

                if (mySkill != null)
                {
                    try
                    {
                        mySkill.ForceSkillLevelUp(skill.Value);
                    }
                    catch (Exception e)
                    {
                        Common.Exception(me, null, "Skill: " + skill.Key, e);
                    }
                }
            }

            return true;
        }

        public class Item : CommonOptionItem
        {
            public readonly SkillStamp mStamp;

            public Item(SkillStamp stamp)
                : base(stamp.Name)
            {
                mStamp = stamp;
            }

            public override string DisplayValue
            {
                get
                {
                    return EAText.GetNumberString(mStamp.Skills.Count);
                }
            }
        }
    }
}
