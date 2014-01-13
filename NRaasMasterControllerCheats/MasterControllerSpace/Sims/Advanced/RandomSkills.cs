using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
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
    public class RandomSkills : SkillLevelBase, IAdvancedOption
    {
        public override string GetTitlePrefix()
        {
            return "RandomSkills";
        }

        protected override List<Item> PrivateRun(SimDescription me, IEnumerable<Item> choices)
        {
            string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":MinPrompt", false, new object[0]), "0");
            if (string.IsNullOrEmpty(text)) return null;

            float min;
            if (!float.TryParse(text, out min))
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                return null;
            }

            text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":MaxPrompt", false, new object[0]), "0");
            if (string.IsNullOrEmpty(text)) return null;

            float max;
            if (!float.TryParse(text, out max))
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                return null;
            }

            Vector2 range = new Vector2(min, max);

            List<Item> selection = new List<Item>();
            foreach (Item choice in choices)
            {
                if (choice == null) continue;

                selection.Add(new RandomItem(choice, range));
            }

            return selection;
        }

        public class RandomItem : Item
        {
            Vector2 mRange;

            public RandomItem(Item item, Vector2 range)
                : base(item)
            {
                mRange = range;
            }

            public override bool AllowDrop
            {
                get { return false; }
            }

            public override float Level
            {
                get
                {
                    float level = base.Level;
                    if (level < 0)
                    {
                        level = RandomUtil.GetFloat(mRange.x, mRange.y);
                    }
                    return level;
                }
            }
        }
    }
}
