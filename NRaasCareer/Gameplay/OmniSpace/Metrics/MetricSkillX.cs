using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.OmniSpace.Metrics
{
    [Persistable]
    public class MetricSkillX : Sims3.Gameplay.Careers.MetricSkillX
    {
        // Methods
        public MetricSkillX(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        {
            if (SkillGuid == SkillNames.None)
            {
                SkillGuid = unchecked((SkillNames)ResourceUtils.HashString64(row.GetString("Args" + metricNumber)));
            }
        }

        public override string MetricDescription(Career career)
        {
            if (SkillManager.GetStaticSkill(SkillGuid) == null) return null;

            return base.MetricDescription(career);
        }

        public override string MetricTitle(Career career)
        {
            if (SkillManager.GetStaticSkill(SkillGuid) == null) return null;

            return base.MetricTitle(career);
        }
    }
}
