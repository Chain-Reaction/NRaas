using NRaas.CommonSpace.Options;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.KamaSimtra.SkillPoints
{
    public class SkillPointsSetting : IntegerSettingOption<GameObject>, ISkillPointsOption
    {
        int mLevel = 0;

        public SkillPointsSetting(int level)
        {
            mLevel = level;
        }

        protected override int Value
        {
            get
            {
                return Skills.KamaSimtra.Settings.GetSkillPoints (mLevel);
            }
            set
            {
                Skills.KamaSimtra.Settings.SetSkillPoints(mLevel, value);
            }
        }

        public override string GetTitlePrefix()
        {
            return "SkillPoints";
        }

        public override string Name
        {
            get
            {
                return Common.Localize("SkillPoints:Level", false, new object[] { (mLevel + 1) });
            }
        }

        protected override string GetPrompt()
        {
            return Common.Localize("SkillPoints:Prompt", false, new object[] { (mLevel + 1) });
        }

        protected override int Validate(int value)
        {
            if (value <= 0)
            {
                value = 1;
            }

            return base.Validate(value);
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }
    }
}
