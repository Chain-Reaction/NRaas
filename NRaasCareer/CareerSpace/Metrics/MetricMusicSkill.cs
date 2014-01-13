using NRaas.CommonSpace.Booters;
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

namespace NRaas.CareerSpace.Metrics
{
    [Persistable]
    public class MetricMusicSkill : Sims3.Gameplay.Careers.MetricSkillX
    {
        static List<SkillNames> sSkills = new List<SkillNames>();

        static MetricMusicSkill()
        {
            sSkills.Add(SkillNames.Guitar);
            sSkills.Add(SkillNames.BassGuitar);
            sSkills.Add(SkillNames.Piano);
            sSkills.Add(SkillNames.Drums);
            sSkills.Add(SkillNames.LaserHarp);
            sSkills.Add((SkillNames)0x00c471a4); // Premium Content Violin Skill
        }
        public MetricMusicSkill(MetricSkillX metric)
            : base (new StubXmlDbRow(), 0)
        {
            mMap0 = metric.mMap0;
            mMap3 = metric.mMap3;
            mMapNeg3 = metric.mMapNeg3;

            SkillGuid = metric.SkillGuid;
        }
        public MetricMusicSkill(XmlDbRow row, int metricNumber)
            : base(row, metricNumber)
        { }

        public static IEnumerable<SkillNames> Skills
        {
            get { return sSkills; }
        }

        public override string MetricDescription(Career career)
        {
            return Common.Localize("MusicSkill:Description", career.IsOwnerFemale);
        }

        public override string MetricTitle(Career career)
        {
            return Common.Localize("MusicSkill:Title", career.IsOwnerFemale);
        }

        public override float FindMetricValue(Career career)
        {
            int highestLevel = 0;

            foreach (SkillNames skill in sSkills)
            {
                int level = career.OwnerDescription.SkillManager.GetSkillLevel(skill);

                if (highestLevel < level)
                {
                    highestLevel = level;
                }
            }

            return highestLevel;
        }

        protected class StubXmlDbRow : XmlDbRow
        {
            public override bool TryGetValueInternal(string column, out string value)
            {
                value = "";
                return true;
            }

            public override bool Exists(string column)
            {
                return false;
            }

            public override IEnumerable<string> ColumnNames
            {
                get { return null; }
            }
        }

        public class Loader : Common.IPreLoad
        {
            public void OnPreLoad()
            {
                try
                {
                    Career music = CareerManager.GetStaticCareer(OccupationNames.Music);
                    if (music == null) return;

                    BooterLogger.AddTrace("Replace Metric Music Skill");

                    foreach (Dictionary<int, CareerLevel> value in music.CareerLevels.Values)
                    {
                        foreach (CareerLevel level in value.Values)
                        {
                            List<PerfMetric> metrics = new List<PerfMetric>(level.Metrics);
                            foreach (PerfMetric metric in metrics)
                            {
                                MetricSkillX skillMetric = metric as MetricSkillX;
                                if (skillMetric == null) continue;

                                if (skillMetric.SkillGuid == SkillNames.Guitar)
                                {
                                    BooterLogger.AddTrace("Metric Replaced: " + level.BranchName + " " + level.Level);

                                    level.Metrics.Remove(metric);

                                    level.Metrics.Add(new MetricMusicSkill(skillMetric));
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception("OnPreLoad", e);
                }
            }
        }
    }
}
