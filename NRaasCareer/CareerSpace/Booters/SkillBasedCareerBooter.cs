using NRaas.CommonSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Booters
{
    public class SkillBasedCareerBooter : BooterHelper.ListingBooter
    {
        public SkillBasedCareerBooter()
            : this(VersionStamp.sNamespace + ".SkillBasedCareers", true)
        { }
        public SkillBasedCareerBooter(string reference, bool testDirect)
            : base("SkillBasedCareerFile", "SkillBasedCareers", reference, testDirect)
        { }

        protected override void PerformFile(BooterHelper.BootFile file)
        {
            BooterHelper.DataBootFile dataFile = file as BooterHelper.DataBootFile;
            if (dataFile == null) return;

            XmlDbTable table = dataFile.GetTable("SkillBasedCareers");
            if (table == null)
            {
                if (file.mPrimary)
                {
                    BooterLogger.AddTrace(file + ": No SkillBasedCareers table");
                }
                else
                {
                    BooterLogger.AddError(file + ": No SkillBasedCareers table");
                }
                return;
            }

            XmlDbTable table2 = dataFile.GetTable("CareerLevels");
            if (table2 == null)
            {
                BooterLogger.AddError(file + ": No CareerLevels table");
                return;
            }

            BooterLogger.AddTrace(file + ": Found Setup = " + table.Rows.Count.ToString());

            if (Occupation.sOccupationStaticDataMap == null)
            {
                Occupation.sOccupationStaticDataMap = new Dictionary<ulong, OccupationStaticData>();
            }

            Dictionary<ulong, List<XmlDbRow>> dictionary = GenerateCareerToCareerLevelXmlDataMap(table2, "SkilledProfession", "SkillBasedCareers");
            foreach (XmlDbRow row in table.Rows)
            {
                string guid = row.GetString("SkilledProfession");

                OccupationNames names;

                if (!row.TryGetEnum<OccupationNames>("SkilledProfession", out names, OccupationNames.Undefined))
                {
                    names = unchecked((OccupationNames)ResourceUtils.HashString64(guid));
                }

                if (Occupation.sOccupationStaticDataMap.ContainsKey((ulong)names))
                {
                    BooterLogger.AddError(file + ": Exists " + guid);
                    continue;
                }

                string str = row.GetString("Skill_Name");

                SkillNames skillName = SkillNames.None;

                try
                {
                    skillName = GenericManager<SkillNames, Skill, Skill>.ParseGuid(str);
                }
                catch
                { }

                if (skillName == SkillNames.None)
                {
                    skillName = unchecked((SkillNames)ResourceUtils.HashString64(str));
                }

                int minimumLevel = row.GetInt("Minimum_Skill_Level", -1);
                float gainMultiplier = row.GetFloat("XP_Gain_Multiplier", 0f);
                int highestLevel = row.GetInt("Highest_Level", 0x0);
                string speechBalloonIcon = row.GetString("Speech_Balloon_Image");
                string hudIcon = row.GetString("HUD_Icon");
                string dreamsAndPromisesIcon = row.GetString("Wishes_Icon");
                string careerDescriptionLocalizationKey = "Gameplay/Excel/SkillBasedCareers/SkillBasedCareers:" + row.GetString("Description_Text");
                string careerOfferLocalizationKey = "Gameplay/Excel/SkillBasedCareers/SkillBasedCareers:" + row.GetString("Offer_Text");
                List<string> careerResponsibilityLocalizationKeys = new List<string>();
                for (int i = 0x1; i <= 0x3; i++)
                {
                    string str7 = row.GetString("Career_Responsibility_" + i);
                    if (string.IsNullOrEmpty(str7))
                    {
                        break;
                    }
                    careerResponsibilityLocalizationKeys.Add("Gameplay/Excel/SkillBasedCareers/SkillBasedCareers:" + str7);
                }
                List<string> careerResponsibilityShortLocalizationKeys = new List<string>();
                for (int i = 0x1; i <= 0x3; i++)
                {
                    string str10 = row.GetString("Career_Responsibility_Short_" + i);
                    if (string.IsNullOrEmpty(str10))
                    {
                        break;
                    }
                    careerResponsibilityShortLocalizationKeys.Add("Gameplay/Excel/SkillBasedCareers/SkillBasedCareers:" + str10);
                }
                List<string> careerResponsibilityIcons = new List<string>();
                for (int j = 0x1; j <= 0x3; j++)
                {
                    string str11 = row.GetString("Career_Responsibility_Icon_" + j);
                    if (string.IsNullOrEmpty(str11))
                    {
                        break;
                    }
                    careerResponsibilityIcons.Add(str11);
                }

                List<XmlDbRow> list3;
                if (dictionary.TryGetValue((ulong)names, out list3))
                {
                    Dictionary<int, OccupationLevelStaticData> levelStaticDataMap = SkillBasedCareer.GenerateCareerLevelToStaticLevelDataMap(names, list3);

                    Type type = row.GetClassType("FullClassName");
                    if (type == null)
                    {
                        type = Type.GetType("Sims3.Gameplay.Careers.SkillBasedCareer, Sims3GameplaySystems");
                    }

                    Type[] types = new Type[] { typeof(OccupationNames) };
                    SkillBasedCareer career = (SkillBasedCareer)type.GetConstructor(types).Invoke(new object[] { names });
                    if (career == null)
                    {
                        BooterLogger.AddError(file.ToString() + ": Constructor Fail " + guid);
                        continue;
                    }
                    Dictionary<uint, JobStaticData> jobStaticDataMap = new Dictionary<uint, JobStaticData>();
                    Dictionary<uint, TaskStaticData> taskStaticDataMap = new Dictionary<uint, TaskStaticData>();
                    Dictionary<string, TrackedAsStaticData> trackedAsMappingsStaticDataMap = new Dictionary<string, TrackedAsStaticData>();
                    SkillBasedCareerStaticData data2 = new SkillBasedCareerStaticData(skillName, minimumLevel, gainMultiplier, highestLevel, speechBalloonIcon, hudIcon, dreamsAndPromisesIcon, careerDescriptionLocalizationKey, careerOfferLocalizationKey, careerResponsibilityLocalizationKeys, careerResponsibilityShortLocalizationKeys, careerResponsibilityIcons, levelStaticDataMap, jobStaticDataMap, taskStaticDataMap, trackedAsMappingsStaticDataMap);
                    if (Occupation.sOccupationStaticDataMap == null)
                    {
                        Occupation.sOccupationStaticDataMap = new Dictionary<ulong, OccupationStaticData>();
                    }
                    Occupation.sOccupationStaticDataMap.Add((ulong)names, data2);
                    CareerManager.AddStaticOccupation(career);
                }
                else
                {
                    BooterLogger.AddError(file.ToString() + ": No Levels " + guid);
                }
            }
        }

        public static SkillBasedCareer GetSkillBasedCareer(Sim sim, SkillNames skill)
        {
            SkillBasedCareer career = sim.OccupationAsSkillBasedCareer;
            if (career == null) return null;

            SkillBasedCareerStaticData skillData = career.GetOccupationStaticDataForSkillBasedCareer();
            if (skillData == null) return null;

            if (skillData.CorrespondingSkillName != skill)
            {
                return null;
            }

            return career;
        }

        public static void UpdateExperience(Sim sim, SkillNames skillName, int funds)
        {
            if (sim.SkillManager != null)
            {
                Skill skill = sim.SkillManager.GetElement(skillName);
                if (skill != null)
                {
                    skill.UpdateXpForEarningMoney(funds);

                    if (sim.CareerManager != null)
                    {
                        sim.CareerManager.UpdateCareerUI();
                    }
                }
            }
        }

        public static float GetLevelFactor(Sim sim, SkillNames skill)
        {
            float factor = 1f;

            if (sim.SkillManager != null)
            {
                int skillLevel = sim.SkillManager.GetSkillLevel(skill);
                for (int i = 1; i < skillLevel; i++)
                {
                    factor *= 1.10f;
                }
            }

            return factor;
        }

        public static int GetCareerPay(Sim sim, SkillNames skill)
        {
            SkillBasedCareer career = GetSkillBasedCareer(sim, skill);
            if (career == null)
            {
                return 0;
            }

            SkillBasedCareerStaticData skillData = career.GetOccupationStaticDataForSkillBasedCareer();
            if (skillData == null)
            {
                return 0;
            }

            OccupationLevelStaticData genericData;
            if (!skillData.LevelStaticDataMap.TryGetValue(career.CareerLevel, out genericData))
            {
                return 0;
            }

            XpBasedCareerLevelStaticData data = genericData as XpBasedCareerLevelStaticData;
            if (data == null)
            {
                return 0;
            }

            if (data.Rewards.Length == 0)
            {
                return 0;
            }

            int cash = 0;
            foreach (XpBasedCareerReward reward in data.Rewards)
            {
                if (reward.Token == XpBasedCareerRewardType.Cash)
                {
                    cash += int.Parse(reward.Value);
                }
            }

            return cash;
        }

        private static Dictionary<ulong, List<XmlDbRow>> GenerateCareerToCareerLevelXmlDataMap(XmlDbTable careerLevelTable, string columnName, string spreadsheetName)
        {
            Dictionary<ulong, List<XmlDbRow>> dictionary = new Dictionary<ulong, List<XmlDbRow>>();
            foreach (XmlDbRow row in careerLevelTable.Rows)
            {
                OccupationNames names;
                if (!row.TryGetEnum<OccupationNames>(columnName, out names, OccupationNames.Undefined))
                {
                    names = unchecked((OccupationNames)ResourceUtils.HashString64(row.GetString(columnName)));
                }


                List<XmlDbRow> list;
                if (dictionary.TryGetValue((ulong)names, out list))
                {
                    list.Add(row);
                }
                else
                {
                    List<XmlDbRow> list2 = new List<XmlDbRow>();
                    list2.Add(row);
                    dictionary.Add((ulong)names, list2);
                }
            }
            return dictionary;
        }
    }
}
