using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using Sims3.Gameplay.ActorSystems;
using Sims3.SimIFace;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Socializing;
using Sims3.UI.Hud;
using Sims3.SimIFace.CAS;
using System.Reflection;

namespace Alcohol
{
    class BuffHelper
    {
        #region Load XML
        
        public static bool Load(string fileName)
        {
           // BuffManager.ParseBuffData(XmlDbData.ReadData("Buffs"), false);
            XmlDbData xmlDbData = XmlDbData.ReadData(fileName);
            if (xmlDbData != null)
            {
               ParseBuffData(xmlDbData, true);
            }            
            UIManager.NewHotInstallStoreBuffData += new UIManager.NewHotInstallStoreBuffCallback(BuffManager.LoadNewlyAddedBuffRecords);
            return true;
        }
        #endregion

        #region Parse Buff XML
        // Sims3.Gameplay.ActorSystems.BuffManager
        public static void ParseBuffData(XmlDbData data, bool bStore)
        {
            try
            {
                XmlDbTable xmlDbTable = null;
                data.Tables.TryGetValue("BuffList", out xmlDbTable);
                if (xmlDbTable == null)
                {
                    xmlDbTable = data.Tables["Buffs"];
                }

                foreach (XmlDbRow current in xmlDbTable.Rows)
                {
                    bool flag = false;
                    string text = current["BuffName"];
                  //  if (!bStore || Localization.HasLocalizationString("Gameplay/Excel/buffs/BuffList:" + text))
                    {
                        string mDescription = current["BuffDescription"];
                        string mHelpText = current["BuffHelpText"];
                        BuffNames guid = (BuffNames)BuffManager.GetGuid(current["Hex"], bStore);
                        double mVersion = 0.0;
                        if (guid == BuffNames.Undefined)
                        {
                            flag = true;
                        }
                        Buff buff = null;
                        
                        if (!flag)
                        {
                            BuffCategory mBuffCategory;
                            ParserFunctions.TryParseEnum<BuffCategory>(current["Category"], out mBuffCategory, BuffCategory.None, true);
                            CommodityKind mSolveCommodity = CommodityKind.None;
                            ParserFunctions.TryParseEnum<CommodityKind>(current["SolveCommodity"], out mSolveCommodity, CommodityKind.None, true);
                            string names = current["IncreasedEffectiveness"];
                            string names2 = current["ReducedEffectiveness"];
                            List<SocialManager.SocialEffectiveness> mIncreasedEffectivenessList;
                            ParserFunctions.TryParseCommaSeparatedList<SocialManager.SocialEffectiveness>(names, out mIncreasedEffectivenessList, SocialManager.SocialEffectiveness.None);
                            List<SocialManager.SocialEffectiveness> mReducedEffectivenessList;
                            ParserFunctions.TryParseCommaSeparatedList<SocialManager.SocialEffectiveness>(names2, out mReducedEffectivenessList, SocialManager.SocialEffectiveness.None);
                            string mTopic = current["Topic"];
                            MoodAxis mAxisEffected;
                            if (ParserFunctions.TryParseEnum<MoodAxis>(current["AxisEffected"], out mAxisEffected, MoodAxis.None, true))
                            {
                                Polarity mPolarityOverride;
                                ParserFunctions.TryParseEnum<Polarity>(current["PolarityOverride"], out mPolarityOverride, Polarity.NoOverride, true);
                                Buff.BuffData buffData = new Buff.BuffData();
                                ParserFunctions.TryParseEnum<ProductVersion>(current["SKU"], out buffData.mProductVersion, (ProductVersion)1);
                                ResourceKey resourceKey = ResourceKey.kInvalidResourceKey;
                                string text2 = null;
                                if (AppDomain.CurrentDomain.GetData("UIManager") != null)
                                {
                                    text2 = current["ThumbFilename"];
                                    resourceKey = ResourceKey.CreatePNGKey(text2, ResourceUtils.ProductVersionToGroupId(buffData.mProductVersion));
                                    if (!World.ResourceExists(resourceKey))
                                    {
                                        resourceKey = ResourceKey.CreatePNGKey(text2, 0u);
                                    }
                                }
                                if (bStore)
                                {
                                    mVersion = Convert.ToDouble(current["Version"]);
                                }
                                buffData.mBuffGuid = guid;
                                buffData.mBuffName = text;
                                buffData.mDescription = mDescription;
                                buffData.mHelpText = mHelpText;
                                buffData.mBuffCategory = mBuffCategory;
                                buffData.mVersion = mVersion;
                                buffData.SetFlags(Buff.BuffData.FlagField.PermaMoodlet, ParserFunctions.ParseBool(current["PermaMoodlet"]));
                                string @string = current.GetString("PermaMoodletColor");
                                ParserFunctions.TryParseEnum<MoodColor>(@string, out buffData.mMoodletColor, MoodColor.Invalid);
                                buffData.mAxisEffected = mAxisEffected;
                                buffData.mPolarityOverride = mPolarityOverride;
                                buffData.mEffectValue = ParserFunctions.ParseInt(current["EffectValue"], 0);
                                buffData.mDelayTimer = (float)ParserFunctions.ParseInt(current["DelayTimer"], 0);
                                buffData.mTimeoutSimMinutes = ParserFunctions.ParseFloat(current["TimeoutLength"], -1f);
                                buffData.mSolveCommodity = mSolveCommodity;
                                buffData.mSolveTime = ParserFunctions.ParseFloat(current["SolveTime"], -3.40282347E+38f);
                                buffData.SetFlags(Buff.BuffData.FlagField.AttemptAutoSolve, ParserFunctions.ParseBool(current["AttemptAutoSolve"]));
                                ParserFunctions.ParseCommaSeparatedString(current["FacialIdle"], out buffData.mFacialIdles);
                                buffData.SetFlags(Buff.BuffData.FlagField.IsExtreme, ParserFunctions.ParseBool(current["IsExtreme"]));
                                buffData.mIncreasedEffectivenessList = mIncreasedEffectivenessList;
                                buffData.mReducedEffectivenessList = mReducedEffectivenessList;
                                buffData.mThumbKey = resourceKey;
                                buffData.mThumbString = text2;
                                buffData.mTopic = mTopic;
                                buffData.SetFlags(Buff.BuffData.FlagField.ShowBalloon, current.GetBool("ShowBallon"));
                                buffData.SetFlags(Buff.BuffData.FlagField.Travel, current.GetBool("Travel"));
                                ParserFunctions.TryParseCommaSeparatedList<OccultTypes>(current["DisallowedOccults"], out buffData.mDisallowedOccults, OccultTypes.None);
                                if (buffData.mDisallowedOccults.Count == 0)
                                {
                                    buffData.mDisallowedOccults = null;
                                }
                                string text3 = current.GetString("JazzStateSuffix");
                                if (string.IsNullOrEmpty(text3))
                                {
                                    text3 = text;
                                }
                                buffData.mJazzStateSuffix = text3;
                                string string2 = current.GetString("SpeciesAvailability");
                                if (string.IsNullOrEmpty(string2))
                                {
                                    buffData.mAvailabilityFlags = (CASAGSAvailabilityFlags)127L;
                                }
                                else
                                {
                                    buffData.mAvailabilityFlags = ParserFunctions.ParseAllowableAgeSpecies(string2);
                                }
                                string text4 = current["CustomClassName"];
                                if (text4.Length > 0)
                                {
                                    text4 = text4.Replace(" ", "");
                                    int num = text4.IndexOf(',');
                                    if (num < 0)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        string str = text4.Substring(0, num);
                                        text4.Substring(num + 1);
                                        Type type = null;
                                        if (bStore)
                                        {
                                            type = Type.GetType(str + ",Sims3StoreObjects");
                                        }
                                        if (type == null)
                                        {
                                            type = Type.GetType(text4);
                                        }
                                        if (type == null)
                                        {
                                            flag = true;
                                        }
                                        else
                                        {
                                            Type[] types = new Type[]
								{
									typeof(Buff.BuffData)
								};
                                            ConstructorInfo constructor = type.GetConstructor(types);
                                            object obj = constructor.Invoke(new object[]
								{
									buffData
								});
                                            buff = (Buff)obj;
                                        }
                                    }
                                }
                                else
                                {
                                    buff = new Buff(buffData);
                                }
                            }
                            if (!flag && buff != null)
                            {
                                BuffInstance value = buff.CreateBuffInstance();
                                if (GenericManager<BuffNames, BuffInstance, BuffInstance>.sDictionary.ContainsKey((ulong)guid))
                                {
                                    if (GenericManager<BuffNames, BuffInstance, BuffInstance>.sDictionary[(ulong)guid].mBuff.BuffVersion < buff.BuffVersion)
                                    {
                                        GenericManager<BuffNames, BuffInstance, BuffInstance>.sDictionary[(ulong)guid] = value;
                                    }
                                }
                                else
                                {
                                    GenericManager<BuffNames, BuffInstance, BuffInstance>.sDictionary.Add((ulong)guid, value);
                                    BuffManager.sBuffEnumValues.AddNewEnumValue(text, guid);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                StyledNotification.Show(new StyledNotification.Format(ex.ToString(), StyledNotification.NotificationStyle.kGameMessageNegative));
            }
        }

        #endregion
    }
}
