using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    [Persistable]
    public class SkillStamp : IPersistence
    {
        string mName;

        Dictionary<SkillNames, int> mSkills = new Dictionary<SkillNames, int>();

        public SkillStamp()
        { }
        public SkillStamp(string name)
        {
            mName = name;
        }

        public string Name
        {
            get
            {
                return mName;
            }
        }

        public Dictionary<SkillNames, int> Skills
        {
            get { return mSkills; }
        }

        public static List<SkillStamp> Create(List<string> values)
        {
            List<SkillStamp> result = new List<SkillStamp>();

            SkillStamp stamp = null;
            foreach (string value in values)
            {
                string[] split = value.Split(':');

                if (split.Length == 1)
                {
                    stamp = new SkillStamp(split[0]);
                    result.Add(stamp);
                }
                else if (split.Length == 2)
                {
                    if (stamp == null)
                    {
                        stamp = new SkillStamp(Common.Localize("SkillStamp:MenuName"));
                        result.Add(stamp);
                    }

                    SkillNames skill = SkillManager.sSkillEnumValues.ParseEnumValue(split[0]);
                    if (skill == SkillNames.None) continue;

                    int level;
                    if (!int.TryParse(split[1], out level)) continue;

                    if (!stamp.mSkills.ContainsKey(skill))
                    {
                        stamp.mSkills.Add(skill, level);
                    }
                }
            }

            return result;
        }
        public static List<string> Create(List<SkillStamp> values)
        {
            List<string> result = new List<string>();

            foreach (SkillStamp stamp in values)
            {
                result.Add(stamp.mName);

                foreach (KeyValuePair<SkillNames, int> skills in stamp.mSkills)
                {
                    result.Add(skills.Key + ":" + skills.Value);
                }
            }

            return result;
        }

        public void Import(Persistence.Lookup settings)
        {
            mName = settings.GetString("Name");

            List<PersistItem> list = settings.GetList<PersistItem>("Skill");

            mSkills.Clear();

            foreach (PersistItem value in list)
            {
                mSkills.Add(value.mSkill, value.mLevel);
            }
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.Add("Name", mName);

            List<PersistItem> list = new List<PersistItem>();
            foreach (KeyValuePair<SkillNames, int> value in mSkills)
            {
                list.Add(new PersistItem(value.Key, value.Value));
            }

            settings.Add("Skill", list);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public class PersistItem : IPersistence
        {
            public SkillNames mSkill;
            public int mLevel;

            public PersistItem()
            { }
            public PersistItem(SkillNames skill, int level)
            {
                mSkill = skill;
                mLevel = level;
            }

            public void Import(Persistence.Lookup settings)
            {
                mSkill = settings.GetEnum<SkillNames>("Skill", SkillNames.None);
                mLevel = settings.GetInt("Level", 0);
            }

            public void Export(Persistence.Lookup settings)
            {
                settings.Add("Skill", mSkill.ToString());
                settings.Add("Level", mLevel.ToString());
            }

            public string PersistencePrefix
            {
                get { return null; }
            }
        }
    }
}
