using NRaas.CommonSpace.Options;
using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.General
{
    public class TestSetting : OptionItem, IGeneralOption
    {
        public override string GetTitlePrefix()
        {
            return "Test";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Woohooer.Settings.Debugging) return false;

            return true;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Common.StringBuilder msg = new Common.StringBuilder("Skill");

            Skill skill = SkillManager.GetStaticSkill(Skills.KamaSimtra.StaticGuid);

            for (int i = 0; i < 10; i++)
            {
                msg += Common.NewLine + "Level " + i + " : " + skill.PointsForNextLevel[i];
            }

            Common.DebugNotify(msg);

            return OptionResult.SuccessClose;
        }
    }
}
