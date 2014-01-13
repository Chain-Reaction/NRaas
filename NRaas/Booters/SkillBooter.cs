using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CommonSpace.Booters
{
    public class SkillBooter : BooterHelper.ByRowListingBooter, Common.IWorldLoadFinished
    {
        public SkillBooter()
            : this(VersionStamp.sNamespace + ".Skills", true)
        { }
        public SkillBooter(string reference, bool testDirect)
            : base("SkillList", "SkillFile", "Skills", reference, testDirect)
        { }

        public void OnWorldLoadFinished()
        {
            // Legacy requirement.  Skill GUIDs need to be uint in size or SkillManager:ImportContent() gaks
            //   Convert all earlier skill codes to the new ones

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                if (sim.SkillManager == null) continue;

                Dictionary<ulong, Skill> replace = new Dictionary<ulong, Skill>();
                foreach (KeyValuePair<ulong, Skill> skill in sim.SkillManager.mValues)
                {
                    if (skill.Key != (ulong)skill.Value.Guid)
                    {
                        replace.Add(skill.Key, skill.Value);
                    }
                }

                foreach (KeyValuePair<ulong, Skill> skill in replace)
                {
                    sim.SkillManager.RemoveElement((SkillNames)skill.Key);

                    if (sim.SkillManager.mValues.ContainsKey((ulong)skill.Value.Guid)) continue;

                    sim.SkillManager.mValues.Add((ulong)skill.Value.Guid, skill.Value);
                }
            }
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            BooterHelper.DataBootFile dataFile = file as BooterHelper.DataBootFile;
            if (dataFile == null) return;

            ParseSkillData(dataFile.Data, row, true);
        }

        public static void ParseSkillData(XmlDbData data, bool bStore)
        {
            if ((data != null) && (data.Tables != null))
            {
                XmlDbTable table = null;
                data.Tables.TryGetValue("SkillList", out table);
                if (table != null)
                {
                    foreach (XmlDbRow row in table.Rows)
                    {
                        ParseSkillData(data, row, bStore);
                    }
                }
            }
        }

        private static void ParseSkillData(XmlDbData data, XmlDbRow row, bool bStore)
        {
            ProductVersion version;
            bool flag = false;
            SkillNames guid = SkillNames.None;

            string skillHex = row.GetString("Hex");

            try
            {
                guid = (SkillNames)SkillManager.GetGuid(ref skillHex, bStore);
            }
            catch
            { }

            if (guid == SkillNames.None)
            {
                flag = true;

                BooterLogger.AddError("GUID Fail " + skillHex);
            }

            if (!row.TryGetEnum<ProductVersion>("CodeVersion", out version, ProductVersion.BaseGame))
            {
                flag = true;

                BooterLogger.AddError("CodeVersion Fail " + version);
            }
            else if (!GameUtils.IsInstalled(version))
            {
                flag = true;

                BooterLogger.AddError("Install Fail " + version);
            }

            if (!flag)
            {
                Skill skill = null;
                string typeName = row.GetString("CustomClassName");
                bool flag2 = typeName.Length > 0x0;
                if (flag2)
                {
                    Type type = null;
                    /*
                    if (bStore)
                    {
                        string[] strArray = typeName.Split(new char[] { ',' });
                        if (strArray.Length < 0x2)
                        {
                            flag = true;
                        }
                        else
                        {
                            type = Type.GetType(strArray[0x0] + ",Sims3StoreObjects");
                        }
                    }
                    */
                    if (type == null)
                    {
                        type = Type.GetType(typeName);
                    }

                    if (type == null)
                    {
                        flag = true;

                        BooterLogger.AddError("CustomClassName Not Found " + typeName);
                    }
                    else
                    {
                        object[] args = new object[] { guid };
                        ConstructorInfo constructor = type.GetConstructor(Type.GetTypeArray(args));
                        if (constructor == null)
                        {
                            flag = true;

                            BooterLogger.AddError("Constructor Missing " + typeName);
                        }
                        else
                        {
                            try
                            {
                                skill = constructor.Invoke(args) as Skill;
                            }
                            catch (Exception e)
                            {
                                Common.Exception(skillHex, e);
                            }

                            if (skill == null)
                            {
                                flag = true;

                                BooterLogger.AddError("Constructor Fail " + typeName);
                            }
                        }
                    }
                }
                else
                {
                    skill = new Skill(guid);

                    BooterLogger.AddTrace("Generic Skill Used " + skillHex);
                }

                if (!flag)
                {
                    Skill.NonPersistableSkillData data2 = new Skill.NonPersistableSkillData();
                    skill.NonPersistableData = data2;
                    uint group = ResourceUtils.ProductVersionToGroupId(version);
                    data2.SkillProductVersion = version;
                    data2.Name = "Gameplay/Excel/Skills/SkillList:" + row.GetString("SkillName");
                    if (!bStore || Localization.HasLocalizationString(data2.Name))
                    {
                        data2.Description = "Gameplay/Excel/Skills/SkillList:" + row.GetString("SkillDescription");
                        data2.MaxSkillLevel = row.GetInt("MaxSkillLevel", 0x0);
                        //skill.Guid = (SkillNames)(uint)guid; // Performed by the constructor, don't do it again here
                        data2.ThoughtBalloonTopicString = row.GetString("ThoughtBalloonTopic");

                        data2.IconKey = ResourceKey.CreatePNGKey(row.GetString("IconKey"), group);
                        data2.SkillUIIconKey = ResourceKey.CreatePNGKey(row.GetString("SkillUIIcon"), group);
                        string str3 = row.GetString("Commodity");
                        CommodityKind commodity = CommodityKind.None;

                        try
                        {
                            commodity = (CommodityKind)Enum.Parse(typeof(CommodityKind), str3);
                        }
                        catch
                        { }

                        if (commodity == CommodityKind.None)
                        {
                            commodity = unchecked((CommodityKind)ResourceUtils.HashString64(str3));
                        }

                        data2.Commodity = commodity;
                        SkillManager.SkillCommodityMap.Add(commodity, guid);

                        double num = 0;
                        if (bStore)
                        {
                            string versionStr = row.GetString("Version");
                            if (!string.IsNullOrEmpty(versionStr))
                            {
                                num = Convert.ToDouble(versionStr);
                            }
                        }
                        data2.SkillVersion = num;

                        if (row.GetBool("Physical"))
                        {
                            skill.AddCategoryToSkill(SkillCategory.Physical);
                        }
                        if (row.GetBool("Mental"))
                        {
                            skill.AddCategoryToSkill(SkillCategory.Mental);
                        }
                        if (row.GetBool("Musical"))
                        {
                            skill.AddCategoryToSkill(SkillCategory.Musical);
                        }
                        if (row.GetBool("Creative"))
                        {
                            skill.AddCategoryToSkill(SkillCategory.Creative);
                        }
                        if (row.GetBool("Artistic"))
                        {
                            skill.AddCategoryToSkill(SkillCategory.Artistic);
                        }
                        if (row.GetBool("Hidden"))
                        {
                            skill.AddCategoryToSkill(SkillCategory.Hidden);
                        }
                        if (row.GetBool("Certificate"))
                        {
                            skill.AddCategoryToSkill(SkillCategory.Certificate);
                        }
                        if ((row.Exists("HiddenWithSkillProgress") && row.GetBool("HiddenWithSkillProgress")) && skill.HasCategory(SkillCategory.Hidden))
                        {
                            skill.AddCategoryToSkill(SkillCategory.HiddenWithSkillProgress);
                        }

                        int[] numArray = new int[skill.MaxSkillLevel];
                        int num3 = 0x0;
                        for (int i = 0x1; i <= skill.MaxSkillLevel; i++)
                        {
                            string column = "Level_" + i.ToString();
                            num3 += row.GetInt(column, 0x0);
                            numArray[i - 0x1] = num3;
                        }
                        data2.PointsForNextLevel = numArray;
                        data2.AlwaysDisplayLevelUpTns = row.GetBool("AlwaysDisplayTNS");
                        string[] strArray2 = new string[skill.MaxSkillLevel + 0x1];
                        for (int j = 0x2; j <= skill.MaxSkillLevel; j++)
                        {
                            string str6 = "Level_" + j.ToString() + "_Text";
                            strArray2[j - 0x1] = row.GetString(str6);
                            if (strArray2[j - 0x1] != string.Empty)
                            {
                                strArray2[j - 0x1] = "Gameplay/Excel/Skills/SkillList:" + strArray2[j - 0x1];
                            }
                        }
                        strArray2[skill.MaxSkillLevel] = row.GetString("Level_10_Text_Alternate");
                        if (strArray2[skill.MaxSkillLevel] != string.Empty)
                        {
                            strArray2[skill.MaxSkillLevel] = "Gameplay/Excel/Skills/SkillList:" + strArray2[skill.MaxSkillLevel];
                        }
                        data2.LevelUpStrings = strArray2;
                        if (flag2)
                        {
                            XmlDbTable table2 = null;
                            string key = row.GetString("CustomDataSheet");
                            data.Tables.TryGetValue(key, out table2);
                            if ((table2 == null) && (key.Length > 0x0))
                            {
                                flag = true;
                                skill = null;
                            }
                            else if (!skill.ParseSkillData(table2))
                            {
                                flag = true;
                                skill = null;
                            }
                        }
                        data2.AvailableAgeSpecies = ParserFunctions.ParseAllowableAgeSpecies(row, "AvailableAgeSpecies");
                        data2.DreamsAndPromisesIcon = row.GetString("DreamsAndPromisesIcon");
                        data2.DreamsAndPromisesIconKey = ResourceKey.CreatePNGKey(data2.DreamsAndPromisesIcon, group);
                        data2.LogicSkillBoost = row.GetBool("LogicSkillBoost");

                        if (!flag)
                        {
                            GenericManager<SkillNames, Skill, Skill>.sDictionary.Add((uint)guid, skill);

                            SkillManager.sSkillEnumValues.AddNewEnumValue(skillHex, skill.Guid);

                            BooterLogger.AddTrace("Loaded " + skill.Name + " (" + skill.Guid + ")");
                        }
                    }
                }
            }
        }
    }
}
