using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic.Band
{
    public class AddBandMember : DualSimFromList, IBandOption
    {
        public override string GetTitlePrefix()
        {
            return "AddBandMember";
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

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.CreatedSim == null) return false;

            if (me.SkillManager == null) return false;

            if (IsFirst)
            {
                RockBand skill = me.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
                if (skill == null) return false;

                //if (skill.IsBandFull()) return false;
            }

            return base.PrivateAllow(me);
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            //if (!base.PrivateAllow(a, b)) return false;

            if (a == null) return false;

            if (b == null) return false;

            if (a == b) return false;

            return true;
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            RockBand skill = a.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);

            PrivateAddBandMember(skill, b);
            return true;
        }

        protected void PrivateAddBandMember(RockBand ths, SimDescription otherSimDesc)
        {
            if (ths.mBandInfo == null)
            {
                ths.mBandInfo = new RockBandInfo();
                ths.mBandInfo.AddBandMember(ths.SkillOwner.SimDescriptionId);
                Sim createdSim = ths.SkillOwner.CreatedSim;
                if (createdSim != null)
                {
                    ActiveTopic.AddToSim(createdSim, "Rock Band");
                }
            }

            //if (ths.mBandInfo.NumBandMembers < 0x4)
            {
                ths.mBandInfo.AddBandMember(otherSimDesc.SimDescriptionId);
                Sim sim = otherSimDesc.CreatedSim;
                if (sim != null)
                {
                    ActiveTopic.AddToSim(sim, "Rock Band");
                }

                SkillManager skillManager = otherSimDesc.SkillManager;
                RockBand skill = skillManager.GetSkill<RockBand>(SkillNames.RockBand);
                if (skill == null)
                {
                    skillManager.AddAutomaticSkill(SkillNames.RockBand);
                    skill = skillManager.GetSkill<RockBand>(SkillNames.RockBand);
                }

                skill.mBandInfo = ths.mBandInfo;
                skill.BandNameUpdate(ths.mBandInfo.BandName);
            }
        }
    }
}
