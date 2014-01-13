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
    public class SkillLevel : SkillLevelBase, IAdvancedOption
    {
        public override string GetTitlePrefix()
        {
            return "SkillLevel";
        }

        protected override List<Item> PrivateRun(SimDescription me, IEnumerable<Item> choices)
        {
            List<Item> selection = new List<Item>();
            foreach (Item choice in choices)
            {
                if (choice == null) continue;

                int currentLevel = me.SkillManager.GetSkillLevel(choice.Skill);

                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { choice.Name, choice.MaximumLevel }), currentLevel.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return null;

                float level;
                if (!float.TryParse(text, out level))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return null;
                }

                choice.Level = level;

                if (choice.Level < 0)
                {
                    choice.Level = -1;
                }
                else if (choice.Level > choice.MaximumLevel)
                {
                    choice.Level = choice.MaximumLevel;
                }

                selection.Add(choice);
            }

            return selection;
        }
    }
}
