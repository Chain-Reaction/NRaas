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

namespace NRaas.RelativitySpace.Options.RelativeSkills
{
    public class RelativeSkill : BooleanSettingOption<GameObject>, IRelativeSkillOption
    {
        Skill mSkill;

        public RelativeSkill(Skill skill)
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
            return "RelativeSkill";
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

        protected override bool Value
        {
            get
            {
                SkillNames guid = SkillNames.None;
                if (mSkill != null)
                {
                    guid = mSkill.Guid;
                }

                bool value;
                if (Relativity.Settings.mRelativeSkills.TryGetValue(guid, out value))
                {
                    return value;
                }
                else if (Relativity.Settings.mRelativeSkills.TryGetValue(SkillNames.None, out value))
                {
                    return value;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (mSkill != null)
                {
                    Relativity.Settings.mRelativeSkills[mSkill.Guid] = value;
                }
                else
                {
                    Relativity.Settings.mRelativeSkills.Clear();
                    Relativity.Settings.mRelativeSkills.Add(SkillNames.None, value);
                }
            }
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
                PriorValues.sFactorChanged = true;
            }

            return result;
        }
    }
}
