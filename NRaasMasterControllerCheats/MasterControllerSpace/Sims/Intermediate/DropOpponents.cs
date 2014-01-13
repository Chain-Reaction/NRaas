using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class DropOpponents : SimFromList, IIntermediateOption
    {
        public override string GetTitlePrefix()
        {
            return "DropOpponents";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (me.SkillManager == null) return false;

            LogicSkill logic = me.SkillManager.GetSkill<LogicSkill>(SkillNames.Logic);
            if ((logic == null) || (logic.mChessRankingNextChallenger == null))
            {
                MartialArts martial = me.SkillManager.GetSkill<MartialArts>(SkillNames.MartialArts);
                if ((martial == null) || (martial.mTournamentChallenger == 0))
                {
                    return false;
                }
            }

            return base.PrivateAllow(me);
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize("DropOpponents:Prompt", me.IsFemale, new object[] { me })))
                {
                    return false;
                }
            }

            LogicSkill logic = me.SkillManager.GetSkill<LogicSkill>(SkillNames.Logic);
            if (logic != null) 
            {
                logic.mChessRankingNextChallenger = null;
            }

            MartialArts martial = me.SkillManager.GetSkill<MartialArts>(SkillNames.MartialArts);
            if (martial != null)
            {
                martial.mTournamentChallenger = 0;
            }

            return true;
        }
    }
}
