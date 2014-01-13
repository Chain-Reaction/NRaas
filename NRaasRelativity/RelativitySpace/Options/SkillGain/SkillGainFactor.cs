using NRaas.CommonSpace.Options;
using NRaas.RelativitySpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Options.SkillGain
{
    public class SkillGainFactor : FloatSettingOption<GameObject>, ISkillGainOption
    {
        Skill mSkill;

        public SkillGainFactor(Skill skill)
        {
            mSkill = skill;

            if (mSkill != null)
            {
                SetThumbnail(mSkill.DreamsAndPromisesIconKey);
            }
            else
            {
                SetThumbnail(ResourceKey.CreatePNGKey("shop_all_r2", ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)));
            }
        }

        public override string GetTitlePrefix()
        {
            return "SkillGainFactor";
        }

        public override string Name
        {
            get
            {
                if (mSkill == null)
                {
                    return "(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")";
                }
                else
                {                   
                    return mSkill.ToString();
                }
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override float Value
        {
            get
            {
                SkillNames guid = SkillNames.None;
                if (mSkill != null)
                {
                    guid = mSkill.Guid;
                }

                float value;
                if (Relativity.Settings.mSkillGains.TryGetValue(guid, out value))
                {
                    return value;
                }
                else if (Relativity.Settings.mSkillGains.TryGetValue(SkillNames.None, out value))
                {
                    return value;
                }
                else
                {
                    return 1f;
                }
            }
            set
            {
                if (mSkill != null)
                {
                    Relativity.Settings.mSkillGains[mSkill.Guid] = value;
                }
                else
                {
                    Relativity.Settings.mSkillGains.Clear();
                    Relativity.Settings.mSkillGains.Add(SkillNames.None, value);
                }
            }
        }

        protected override string GetPrompt()
        {
            return Common.Localize(GetTitlePrefix() + ":Prompt", false, new object[] { Name });
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (mSkill == null)
            {
                if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":AllPrompt"))) return OptionResult.Failure;
            }

            OptionResult result = base.Run(parameters);
            if (result != OptionResult.Failure)
            {
                if (Value < 0)
                {
                    Value = 0;
                }

                PriorValues.sFactorChanged = true;
            }

            return result;
        }
    }
}
