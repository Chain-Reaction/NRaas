using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.Sims.Advanced;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings.SkillStamps
{
    public class ChangeStampSetting : OptionItem, ISkillStampOption
    {
        Helpers.SkillStamp mStamp;

        public ChangeStampSetting(Helpers.SkillStamp stamp)
        {
            mStamp = stamp;
        }

        public override string GetTitlePrefix()
        {
            return "SkillStamp";
        }

        public override string Name
        {
            get
            {
                return mStamp.Name;
            }
        }

        public override string DisplayValue
        {
            get
            {
                return EAText.GetNumberString(mStamp.Skills.Count);
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Dictionary<SkillNames, bool> lookup = new Dictionary<SkillNames, bool>();

            List<SkillLevel.Item> allOptions = new List<SkillLevel.Item>();
            foreach (Skill skill in SkillManager.SkillDictionary)
            {
                if (lookup.ContainsKey(skill.Guid)) continue;
                lookup.Add(skill.Guid, true);

                int count;
                if (!mStamp.Skills.TryGetValue(skill.Guid, out count))
                {
                    count = -1;
                }

                allOptions.Add(new SkillLevel.Item(skill, count));
            }

            CommonSelection<SkillLevel.Item>.Results selection = new CommonSelection<SkillLevel.Item>(Name, allOptions, new SkillLevel.AuxillaryColumn()).SelectMultiple();
            if ((selection == null) || (selection.Count == 0)) return OptionResult.Failure;

            foreach (SkillLevel.Item item in selection)
            {
                int level;
                if (!mStamp.Skills.TryGetValue(item.Skill, out level))
                {
                    level = -1;
                }

                string text = StringInputDialog.Show(Name, Common.Localize("SkillLevel:Prompt", false, new object[] { item.Name, item.MaximumLevel }), level.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                if (!int.TryParse(text, out level))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return OptionResult.Failure;
                }

                mStamp.Skills.Remove(item.Skill);
                mStamp.Skills.Add(item.Skill, level);
            }

            MasterController.Settings.UpdateStamp();

            return OptionResult.SuccessRetain;
        }
    }
}
