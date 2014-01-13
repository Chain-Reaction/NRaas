using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic.Band
{
    public class RemoveBandMember : DualSimFromList, IBandOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveBandMember";
        }

        protected override string GetTitleA()
        {
            return Common.Localize("BandMember:Leader");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("BandMember:Member");
        }

        protected override int GetMaxSelectionA()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override ICollection<ProtoSimSelection<IMiniSimDescription>.ICriteria> GetCriteriaB(GameHitParameters<GameObject> parameters)
        {
            return null;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            if (me.SkillManager == null) return false;

            if (me.SkillManager.GetSkill<RockBand>(SkillNames.RockBand) == null) return false;

            return base.PrivateAllow(me);
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            //if (!base.PrivateAllow(a, b)) return false;

            if (a == null) return false;

            if (b == null) return false;

            RockBand skill = a.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
            if (skill == null) return false;

            if (!skill.IsBandMember(b)) return false;

            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            RockBand skill = b.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
            if (skill != null)
            {
                skill.RemoveOwnerFromBand();
            }

            // Just to make sure
            skill = a.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
            if (skill != null)
            {
                if (skill.mBandInfo != null)
                {
                    skill.mBandInfo.RemoveBandMember(b.SimDescriptionId);
                }
            }

            return true;
        }
    }
}
