using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Status
{
    public class SkillsStatus : SimFromList, IStatusOption
    {
        public override string GetTitlePrefix()
        {
            return "SkillsStatus";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool AutoApplyAll()
        {
            return true;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        public string GetDetails(IMiniSimDescription miniSim)
        {
            SimDescription me = miniSim as SimDescription;
            if (me == null) return null;

            string msg = PersonalStatus.GetHeader(me) + Common.NewLine;

            List<string> skills = new List<string>();

            foreach (Skill skill in me.SkillManager.List)
            {
                if (skill.SkillLevel <= 0) continue;

                skills.Add(Common.Localize("SkillsStatus:Element", me.IsFemale, new object[] { skill.Name, skill.SkillLevel }));
            }

            if (skills.Count == 0)
            {
                msg += Common.Localize("SkillsStatus:NoSkill");
            }
            else
            {
                skills.Sort();

                foreach (string skill in skills)
                {
                    msg += skill;
                }
            }

            return msg;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            SimpleMessageDialog.Show(Name, GetDetails(me));
            return true;
        }
    }
}
