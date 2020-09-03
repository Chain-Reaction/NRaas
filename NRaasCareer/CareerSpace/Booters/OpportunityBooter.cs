using NRaas.CommonSpace.Booters;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.Opportunities;
using NRaas.Gameplay.Rewards;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.CareerSpace.Booters
{
    public class OpportunityBooter : BooterHelper.ListingBooter
    {
        public OpportunityBooter()
            : this(VersionStamp.sNamespace + ".Opportunities", true)
        { }
        public OpportunityBooter(string reference, bool testDirect)
            : base("OpportunityFile", "Opportunities", reference, testDirect)
        { }

        protected override void PerformFile(BooterHelper.BootFile file)
        {
            BooterHelper.DataBootFile dataFile = file as BooterHelper.DataBootFile;
            if (dataFile == null) return;

            XmlDbTable table = dataFile.GetTable("OpportunitiesSetup");
            if (table == null)
            {
                if (file.mPrimary)
                {
                    BooterLogger.AddTrace(file + ": No OpportunitiesSetup");
                }
                else
                {
                    BooterLogger.AddError(file + ": No OpportunitiesSetup");
                }
                return;
            }

            BooterLogger.AddTrace(file + ": Found List = " + table.Rows.Count);

            foreach (XmlDbRow row in table.Rows)
            {
                Opportunity.OpportunitySharedData data = new Opportunity.OpportunitySharedData();
                ParserFunctions.TryParseEnum<ProductVersion>(row.GetString("ProductVersion"), out data.mProductVersion, ProductVersion.BaseGame);

                data.mGuid = GenericManager<OpportunityNames, Opportunity, Opportunity>.ParseGuid(row["GUID"]);

                // New Code
                if (data.mGuid == OpportunityNames.Undefined)
                {
                    data.mGuid = unchecked((OpportunityNames)ResourceUtils.HashString64(row["GUID"]));
                }

                if (GameUtils.IsInstalled(data.mProductVersion))
                {
                    string str2 = row.GetString("Icon");
                    if (!string.IsNullOrEmpty(str2))
                    {
                        data.mIconKey = ResourceKey.CreatePNGKey(str2, 0x0);
                    }
                    ParserFunctions.TryParseEnum<Repeatability>(row["RepeatLevel"], out data.mRepeatLevel, Repeatability.Undefined);
                    data.SetFlags(Opportunity.OpportunitySharedData.FlagField.Ordered, row.GetBool("IsOrdered"));
                    data.SetFlags(Opportunity.OpportunitySharedData.FlagField.Counted, row.GetBool("IsCounted"));
                    data.SetFlags(Opportunity.OpportunitySharedData.FlagField.Totaled, row.GetBool("IsTotaled"));
                    data.mCountOrTotal = row.GetFloat("CountOrTotal", 0f);
                    string str3 = row["Timeout"];
                    if (str3 != string.Empty)
                    {
                        ParserFunctions.TryParseEnum<Opportunity.OpportunitySharedData.TimeoutCondition>(row["Timeout"], out data.mTimeout, Opportunity.OpportunitySharedData.TimeoutCondition.None);
                    }
                    if (data.mTimeout == Opportunity.OpportunitySharedData.TimeoutCondition.SimTime)
                    {
                        if (row.GetString("TimeoutData") != null)
                        {
                            data.mTimeoutData = ParserFunctions.ParseTime(row.GetString("TimeoutData"));
                        }
                        else
                        {
                            BooterLogger.AddError(file + ": TimeoutData missing " + row["GUID"]);
                        }
                    }
                    else
                    {
                        data.mTimeoutData = row.GetFloat("TimeoutData");

                        if (row.GetString("TimeoutEnd") != null)
                        {
                            data.mTimeoutEnd = ParserFunctions.ParseTime(row.GetString("TimeoutEnd"));
                        }
                        else
                        {
                            BooterLogger.AddError(file + ": TimeoutEnd missing " + row["GUID"]);
                        }
                    }
                    string name = row["Loss"];
                    if (name != string.Empty)
                    {
                        ParserFunctions.TryParseEnum<Opportunity.OpportunitySharedData.TimeoutCondition>(name, out data.mLoss, Opportunity.OpportunitySharedData.TimeoutCondition.None);
                    }
                    if (data.mLoss == Opportunity.OpportunitySharedData.TimeoutCondition.SimTime)
                    {
                        if (row.GetString("LossData") != null)
                        {
                            data.mLossData = ParserFunctions.ParseTime(row.GetString("LossData"));
                        }
                        else
                        {
                            BooterLogger.AddError(file + ": LossData missing " + row["GUID"]);
                        }
                    }
                    else
                    {
                        data.mLossData = row.GetFloat("LossData");
                    }

                    data.mChanceToGetOnPhone = row.GetFloat("ChanceToGetOnPhone");
                    row.TryGetEnum<OpportunityAvailability>("Availability", out data.mOpportunityAvailability, OpportunityAvailability.Undefined);
                    data.mOpportunityType = OpportunityType.Undefined;
                    row.TryGetEnum<OpportunityType>("OpportunityType", out data.mOpportunityType, OpportunityType.Undefined);
                    data.mEventList = OpportunityManager.ParseEvents(row, data.mGuid);
                    List<string> entry = row.GetStringList("CompletionEvent", ',', false);
                    if (entry.Count == 0x0)
                    {
                        data.mCompletionEvent = null;
                    }
                    else
                    {
                        data.mCompletionEvent = OpportunityManager.ParseOneEvent(entry, row, data.mGuid);
                        if (data.mOpportunityType == OpportunityType.Career)
                        {
                            data.mCompletionEvent.mEventDelegate = new ProcessEventDelegate(Opportunity.ProcessCompletionEventDelegateCareer);
                        }
                        else if (data.mOpportunityType == OpportunityType.Skill)
                        {
                            data.mCompletionEvent.mEventDelegate = new ProcessEventDelegate(Opportunity.ProcessCompletionEventDelegateSkill);
                        }
                        else if (data.mOpportunityType == OpportunityType.Location)
                        {
                            data.mCompletionEvent.mEventDelegate = new ProcessEventDelegate(Opportunity.ProcessCompletionEventDelegateSpecial);
                        }
                        else if (data.mOpportunityType == OpportunityType.AdventureChina)
                        {
                            data.mCompletionEvent.mEventDelegate = new ProcessEventDelegate(Opportunity.ProcessCompletionEventDelegateAdventureChina);
                        }
                        else if (data.mOpportunityType == OpportunityType.AdventureEgypt)
                        {
                            data.mCompletionEvent.mEventDelegate = new ProcessEventDelegate(Opportunity.ProcessCompletionEventDelegateAdventureEgypt);
                        }
                        else if (data.mOpportunityType == OpportunityType.AdventureFrance)
                        {
                            data.mCompletionEvent.mEventDelegate = new ProcessEventDelegate(Opportunity.ProcessCompletionEventDelegateAdventureFrance);
                        }
                    }
                    data.SetFlags(Opportunity.OpportunitySharedData.FlagField.AllowPhoneCompletion, row.GetBool("AllowPhoneCompletion"));
                    string str5 = row.GetString("CompletionProceduralTestFunction");
                    if (!string.IsNullOrEmpty(str5) && (OpportunityManager.sOpportunitiesTestClassType != null))
                    {
                        MethodInfo method = OpportunityManager.sOpportunitiesTestClassType.GetMethod(str5, BindingFlags.Public | BindingFlags.Static);
                        if (method != null)
                        {
                            data.mTargetProceduralTestDelegate = Delegate.CreateDelegate(typeof(OpportunityTargetProceduralTestDelegate), method) as OpportunityTargetProceduralTestDelegate;
                        }
                    }
                    Opportunity opportunity = null;
                    string typeName = row.GetString("CustomOpportunityClass");
                    if (typeName.Length > 0x0)
                    {
                        Type type = Type.GetType(typeName);
                        if (type != null)
                        {
                            Type[] types = new Type[] { typeof(Opportunity.OpportunitySharedData) };

                            try
                            {
                                opportunity = (Opportunity)type.GetConstructor(types).Invoke(new object[] { data });
                            }
                            catch
                            {
                                BooterLogger.AddError(file + ": Constructor Fail " + typeName);
                            }
                        }
                    }
                    else if (data.HasFlags(Opportunity.OpportunitySharedData.FlagField.Counted))
                    {
                        opportunity = new CountedOpportunity(data);
                    }
                    else if (data.HasFlags(Opportunity.OpportunitySharedData.FlagField.Totaled))
                    {
                        opportunity = new TotaledOpportunity(data);
                    }
                    else if (data.HasFlags(Opportunity.OpportunitySharedData.FlagField.Ordered))
                    {
                        opportunity = new OrderedOpportunity(data);
                    }
                    else
                    {
                        opportunity = new Opportunity(data);
                    }

                    if (opportunity != null)
                    {
                        // New code
                        GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary[(ulong)opportunity.Guid] = opportunity;

                        if (opportunity.IsLocationBased && (opportunity.Guid != OpportunityNames.Special_RestoreGhost))
                        {
                            // New code
                            OpportunityManager.sLocationBasedOpportunityList[opportunity.Guid] = opportunity;
                            continue;
                        }
                        if (opportunity.IsSkill || (opportunity.Guid == OpportunityNames.Special_RestoreGhost))
                        {
                            // New code
                            OpportunityManager.sSkillOpportunityList[opportunity.Guid] = opportunity;
                            continue;
                        }
                        if (opportunity.IsAdventureChina)
                        {
                            // New code
                            OpportunityManager.sAdventureChinaOpportunityList[opportunity.Guid] = opportunity;
                        }
                        else
                        {
                            if (opportunity.IsAdventureEgypt)
                            {
                                // New code
                                OpportunityManager.sAdventureEgyptOpportunityList[opportunity.Guid] = opportunity;
                                continue;
                            }
                            if (opportunity.IsAdventureFrance)
                            {
                                // New code
                                OpportunityManager.sAdventureFranceOpportunityList[opportunity.Guid] = opportunity;
                            }
                        }
                    }
                }
            }

            ParseOpportunityNames(dataFile, table.Rows.Count);
            ParseOpportunityRequirements(dataFile, table.Rows.Count);
            ParseOpportunitySetup(dataFile, table.Rows.Count);
            ParseOpportunityCompletion(dataFile, table.Rows.Count);
        }

        private static void ParseOpportunityNames(BooterHelper.DataBootFile file, int total)
        {
            XmlDbTable table = file.GetTable("Names");
            if (table == null) return;

            BooterLogger.AddTrace(file + ": Found Names = " + table.Rows.Count);

            if (total != table.Rows.Count)
            {
                BooterLogger.AddError(file + ": Found Too few Names " + total + " < " + table.Rows.Count);
            }

            foreach (XmlDbRow row in table.Rows)
            {
                OpportunityNames names = GenericManager<OpportunityNames, Opportunity, Opportunity>.ParseGuid(row["GUID"]);

                // new Code
                if (names == OpportunityNames.Undefined)
                {
                    names = unchecked((OpportunityNames)ResourceUtils.HashString64(row["GUID"]));
                }

                bool sDoValidation = OpportunityManager.sDoValidation;
                if (GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary.ContainsKey((ulong)names))
                {
                    Opportunity.OpportunitySharedData sharedData = GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary[(ulong)names].SharedData;
                    sharedData.mName.Add("Gameplay/Excel/Opportunities/Names:" + row.GetString("OpportunityName"));
                    sharedData.mDescription.Add("Gameplay/Excel/Opportunities/Names:" + row.GetString("OpportunityDescription"));
                    string str3 = row.GetString("OpportunityHint");
                    sharedData.mHint.Add(string.IsNullOrEmpty(str3) ? string.Empty : ("Gameplay/Excel/Opportunities/Names:" + str3));
                    string str4 = row.GetString("CompletionText");
                    sharedData.mCompletionText.Add(string.IsNullOrEmpty(str4) ? string.Empty : ("Gameplay/Excel/Opportunities/Names:" + str4));
                    string str5 = row.GetString("SecondaryCompletionText");
                    sharedData.mSecondaryCompletionText.Add(string.IsNullOrEmpty(str5) ? string.Empty : ("Gameplay/Excel/Opportunities/Names:" + str5));
                    string str6 = row.GetString("FailureText");
                    sharedData.mFailureText.Add(string.IsNullOrEmpty(str6) ? string.Empty : ("Gameplay/Excel/Opportunities/Names:" + str6));
                    string str7 = row.GetString("ProgressText");
                    sharedData.mProgressText.Add(string.IsNullOrEmpty(str7) ? string.Empty : ("Gameplay/Excel/Opportunities/Names:" + str7));
                }
                else
                {
                    BooterLogger.AddError(file + ": Names Unknown Opp " + row["GUID"]);
                }
            }
        }

        private static void ParseOpportunityRequirements(BooterHelper.DataBootFile file, int total)
        {
            XmlDbTable table = file.GetTable("OpportunitiesRequirements");
            if (table == null) return;

            BooterLogger.AddTrace(file + ": Found Req = " + table.Rows.Count);

            if (total != table.Rows.Count)
            {
                BooterLogger.AddError(file + ": Found Too few Req " + total + " < " + table.Rows.Count);
            }

            foreach (XmlDbRow row in table.Rows)
            {
                OpportunityNames opportunityKey = GenericManager<OpportunityNames, Opportunity, Opportunity>.ParseGuid(row["GUID"]);

                // New code
                if (opportunityKey == OpportunityNames.Undefined)
                {
                    opportunityKey = unchecked((OpportunityNames)ResourceUtils.HashString64(row["GUID"]));
                }

                bool sDoValidation = OpportunityManager.sDoValidation;
                if (GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary.ContainsKey((ulong)opportunityKey))
                {
                    Opportunity opp = GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary[(ulong)opportunityKey];

                    Opportunity.OpportunitySharedData sharedData = opp.SharedData;
                    sharedData.mRequirementList = ParseRequirements(row, opportunityKey);
                    List<string> stringList = row.GetStringList("CustomRequirementFunction", ',');
                    if (stringList.Count > 0)
                    {
                        sharedData.mRequirementDelegate = (OpportunityRequirementDelegate)OpportunityManager.ParseDelegatePair(stringList[0], stringList[1], typeof(OpportunityRequirementDelegate));
                    }
                }
                else
                {
                    BooterLogger.AddError(file + ": Req Unknown Opp " + row["GUID"]);
                }
            }
        }

        private static Opportunity.OpportunitySharedData.RequirementInfo ParseCareerRequirements(List<string> entry)
        {
            if (entry.Count == 4)
            {
                OccupationNames names;
                Opportunity.OpportunitySharedData.RequirementInfo info = new Opportunity.OpportunitySharedData.RequirementInfo();
                info.mType = RequirementType.Career;
                ParserFunctions.TryParseEnum<OccupationNames>(entry[1], out names, OccupationNames.Undefined);

                // New Code
                if (names == OccupationNames.Undefined)
                {
                    names = unchecked((OccupationNames)ResourceUtils.HashString64(entry[1]));
                }

                info.mGuid = (ulong)names;
                bool flag = int.TryParse(entry[2], out info.mMinLevel);
                bool flag2 = int.TryParse(entry[3], out info.mMaxLevel);
                if (flag && flag2)
                {
                    return info;
                }
            }
            return null;
        }

        public static Opportunity.OpportunitySharedData.RequirementInfo ParseNotInCareerRequirement(List<string> entry)
        {
            if (entry.Count == 2)
            {
                OccupationNames names;
                Opportunity.OpportunitySharedData.RequirementInfo info = new Opportunity.OpportunitySharedData.RequirementInfo();
                info.mType = RequirementType.NotInCareer;
                ParserFunctions.TryParseEnum<OccupationNames>(entry[1], out names, OccupationNames.Undefined);

                // New Code
                if (names == OccupationNames.Undefined)
                {
                    names = unchecked((OccupationNames)ResourceUtils.HashString64(entry[1]));
                }

                info.mGuid = (ulong)names;
                return info;
            }
            return null;
        }

        private static Opportunity.OpportunitySharedData.RequirementInfo ParseSkillRequirements(List<string> entry)
        {
            if (entry.Count == 4)
            {                
                Opportunity.OpportunitySharedData.RequirementInfo info = new Opportunity.OpportunitySharedData.RequirementInfo();
                info.mType = RequirementType.Skill;

                // Custom
                SkillNames names = SkillManager.sSkillEnumValues.ParseEnumValue(entry[1]);

                info.mGuid = (ulong)names;
                bool flag = int.TryParse(entry[2], out info.mMinLevel);
                bool flag2 = int.TryParse(entry[3], out info.mMaxLevel);
                if (flag && flag2)
                {
                    return info;
                }
            }
            return null;
        }

        public static Opportunity.OpportunitySharedData.RequirementInfo ParseOpportunityNotCompleteRequirement(List<string> entry)
        {
            if (entry.Count == 2)
            {
                OpportunityNames names;
                Opportunity.OpportunitySharedData.RequirementInfo info = new Opportunity.OpportunitySharedData.RequirementInfo();
                info.mType = RequirementType.OpportunityNotComplete;
                ParserFunctions.TryParseEnum<OpportunityNames>(entry[1], out names, OpportunityNames.Undefined);

                // New Code
                if (names == OpportunityNames.Undefined)
                {
                    names = unchecked((OpportunityNames)ResourceUtils.HashString64(entry[1]));
                }

                info.mGuid = (ulong)names;
                return info;
            }
            return null;
        }

        public static Opportunity.OpportunitySharedData.RequirementInfo ParseOpportunityCompleteRequirement(List<string> entry)
        {
            if (entry.Count == 2)
            {
                OpportunityNames names;
                Opportunity.OpportunitySharedData.RequirementInfo info = new Opportunity.OpportunitySharedData.RequirementInfo();
                info.mType = RequirementType.OpportunityComplete;
                ParserFunctions.TryParseEnum<OpportunityNames>(entry[1], out names, OpportunityNames.Undefined);

                // New Code
                if (names == OpportunityNames.Undefined)
                {
                    names = unchecked((OpportunityNames)ResourceUtils.HashString64(entry[1]));
                }
                
                info.mGuid = (ulong)names;
                return info;
            }
            return null;
        }

        private static ArrayList ParseRequirements(XmlDbRow dr, OpportunityNames opportunityKey)
        {
            ArrayList list = new ArrayList();
            for (int i = 1; i <= 8; i++)
            {
                List<string> entry = dr.GetStringList("Requirement" + i, ',', false);
                if (entry.Count > 0)
                {
                    RequirementType type;
                    ParserFunctions.TryParseEnum<RequirementType>(entry[0], out type, RequirementType.Undefined);

                    Opportunity.OpportunitySharedData.RequirementInfo info = null;

                    if (type == RequirementType.Undefined)
                    {
                        info = new Opportunity.OpportunitySharedData.RequirementInfo();
                        info.mData = dr.GetString("Requirement" + i);
                        info.mType = RequirementType.Undefined;
                    }
                    else
                    {
                        switch (type)
                        {
                            case RequirementType.Career:
                                info = ParseCareerRequirements(entry);
                                break;

                            case RequirementType.Skill:
                                info = ParseSkillRequirements(entry);
                                break;

                            case RequirementType.Trait:
                                info = OpportunityManager.ParseTraitRequirements(entry);
                                break;

                            case RequirementType.WorldHasRabbitHoleType:
                                info = OpportunityManager.ParseWorldHasRabbitHoleRequirement(entry);
                                break;

                            case RequirementType.LearnableRecipeExists:
                            case RequirementType.HasAcquaintanceOrHigher:
                            case RequirementType.HasYAAcquaintanceOrHigher:
                            case RequirementType.HasNonFriendCoworker:
                            case RequirementType.WorldHasPregnantSim:
                            case RequirementType.HasChildOrTeenAcquaintanceOrHigher:
                            case RequirementType.IsElder:
                            case RequirementType.WorldHasCommunityLot:
                            case RequirementType.HasHighRelationshipWithDeadSim:
                            case RequirementType.Unemployed:
                            case RequirementType.HasSingleSim:
                                info = new Opportunity.OpportunitySharedData.RequirementInfo();
                                info.mType = type;
                                break;

                            case RequirementType.HasObjectOnHomeLot:
                            case RequirementType.WorldHasLotWith:
                                info = OpportunityManager.ParseObjectRequirement(entry);
                                info.mType = type;
                                break;

                            case RequirementType.KnowsRecipe:
                                info = OpportunityManager.ParseKnowsRecipeRequirement(entry);
                                break;

                            case RequirementType.HasStrangers:
                                info = OpportunityManager.ParseHasStrangersRequirement(entry);
                                break;

                            case RequirementType.HasNonHouseholdAdultSims:
                                info = OpportunityManager.ParseHasNonHouseholdAdultSimsRequirement(entry);
                                break;

                            case RequirementType.WorldHasInvitableSimInCareer:
                            case RequirementType.WorldHasSimInCareer:
                            case RequirementType.WorldHasSkillTutorableSimInCareer:
                                info = OpportunityManager.ParseWorldHasSimInCareerRequirement(entry);
                                info.mType = type;
                                break;

                            case RequirementType.HasInvitableCoworker:
                                info = OpportunityManager.ParseHasInvitableCoworkerRequirement(entry);
                                break;

                            case RequirementType.HasGrade:
                                info = OpportunityManager.ParseHasGradeRequirement(entry);
                                break;

                            case RequirementType.HasPlanted:
                                info = OpportunityManager.ParseHasPlantedRequirement(entry);
                                break;

                            case RequirementType.HasWrittenXBooksOfGenre:
                                info = OpportunityManager.ParseHasWrittenXBooksOfGenreRequirement(entry);
                                break;

                            case RequirementType.OpportunityNotComplete:
                                info = ParseOpportunityNotCompleteRequirement(entry);
                                break;

                            case RequirementType.OpportunityComplete:
                                info = ParseOpportunityCompleteRequirement(entry);
                                break;

                            case RequirementType.CanWriteGenre:
                                info = OpportunityManager.ParseCanWriteGenreRequirement(entry);
                                break;

                            case RequirementType.WorldHasSpecificSim:
                                info = OpportunityManager.ParseWorldHasSpecificSimRequirement(entry);
                                break;

                            case RequirementType.WorldHasCommunityLotType:
                                info = OpportunityManager.ParseWorldHasCommunityLotTypeRequirement(entry, type);
                                break;

                            case RequirementType.NotInCareer:
                                info = ParseNotInCareerRequirement(entry);
                                break;

                            case RequirementType.HasVacationLocal:
                                info = OpportunityManager.ParseHasVacationLocalRequirement(entry);
                                break;

                            case RequirementType.WorldHasSimInRole:
                                info = OpportunityManager.WorldHasSimInRoleRequirement(entry);
                                break;

                            case RequirementType.WorldHasSimOnSpecificLot:
                                info = OpportunityManager.WorldHasSimOnSpecificLot(entry);
                                break;

                            case RequirementType.WorldHasEmptyTreasureSpawner:
                                info = OpportunityManager.WorldHasEmptyTreasureSpawner(entry);
                                break;

                            case RequirementType.VisaLevel:
                                info = OpportunityManager.ParseVisaLevelRequirements(entry, opportunityKey);
                                break;

                            case RequirementType.HasBuff:
                                info = OpportunityManager.ParseBuffRequirements(entry);
                                break;

                            case RequirementType.KnowsInvention:
                                info = OpportunityManager.ParseKnownInventionRequirement(entry);
                                break;

                            case RequirementType.KnowsSculpture:
                                info = OpportunityManager.ParseKnownSculptureRequirement(entry);
                                break;

                            case RequirementType.Celebrity:
                                info = OpportunityManager.ParseCelebrityRequirements(entry);
                                break;

                            case RequirementType.HasCompletedNGigs:
                                info = OpportunityManager.ParseHasCompletedNGigsRequirements(entry);
                                break;

                            case RequirementType.HasPet:
                                info = OpportunityManager.ParseHasPetRequirement(entry);
                                break;
                        }
                    }
                    
                    if (info != null)
                    {
                        list.Add(info);
                    }
                }
            }
            return list;
        }

        private static void ParseOpportunitySetup(BooterHelper.DataBootFile file, int total)
        {
            XmlDbTable table = file.GetTable("OpportunitiesSetup");
            if (table == null) return;

            BooterLogger.AddTrace(file + ": Found Setup = " + table.Rows.Count);

            if (total != table.Rows.Count)
            {
                BooterLogger.AddError(file + ": Found Too few Setup " + total + " < " + table.Rows.Count);
            }

            foreach (XmlDbRow row in table.Rows)
            {
                OpportunityNames names = GenericManager<OpportunityNames, Opportunity, Opportunity>.ParseGuid(row["GUID"]);

                #region NEWCODE
                if (names == OpportunityNames.Undefined)
                {
                    names = unchecked((OpportunityNames)ResourceUtils.HashString64(row["GUID"]));
                }
                #endregion

                if (GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary.ContainsKey((ulong)names))
                {
                    string[] strArray5;
                    string[] strArray6;
                    Opportunity opportunity = GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary[(ulong) names];
                    Opportunity.OpportunitySharedData sharedData = opportunity.SharedData;
                    string name = row.GetString("Object");
                    if (name != string.Empty)
                    {
                        ParserFunctions.TryParseEnum<OpportunityObjectTypes>(name, out sharedData.mSetupObjectType, OpportunityObjectTypes.Undefined);
                    }
                    sharedData.mSetupObjectData = row.GetString("ObjectData");
                    if (sharedData.mSetupObjectType == OpportunityObjectTypes.Keystone)
                    {
                        OpportunityManager.sKeystonesThatComeFromAdventures[sharedData.mSetupObjectData] = true;
                    }
                    sharedData.mSetupObjectTreasureSpawner = row.GetString("ObjectTreasureSpawner");
                    sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.SetupObjectRequiredOnCompletion, row.GetBool("RequiredOnCompletion"));
                    sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.DeleteSetupObjectOnCompletion, row.GetBool("DeleteOnCompletion"));
                    if (!string.IsNullOrEmpty(row.GetString("DeleteOnFailure")))
                    {
                        sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.DeleteSetupObjectOnFailure, row.GetBool("DeleteOnFailure"));
                    }
                    else
                    {
                        sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.DeleteSetupObjectOnFailure, sharedData.HasFlags(Opportunity.OpportunitySharedData.FlagField.DeleteSetupObjectOnCompletion));
                    }
                    if (sharedData.HasFlags(Opportunity.OpportunitySharedData.FlagField.DeleteSetupObjectOnCompletion))
                    {
                        sharedData.HasFlags(Opportunity.OpportunitySharedData.FlagField.DeleteSetupObjectOnFailure);
                    }
                    string str2 = row.GetString("ObjectStateForCompletion");
                    if (str2 != string.Empty)
                    {
                        string[] strArray = null;
                        if (ParserFunctions.ParseCommaSeparatedString(str2, out strArray))
                        {
                            ParserFunctions.TryParseEnum<OpportunityObjectState>(strArray[0x0], out sharedData.mSetupObjectCompletionState, OpportunityObjectState.Undefined);
                            if (strArray.Length == 0x2)
                            {
                                sharedData.mSetupObjectCompletionStateData = strArray[0x1];
                            }
                        }
                    }
                    row.TryGetEnum<OpportunityCompletionTNSType>("CompletionTNSType", out sharedData.mCompletionTNSType, OpportunityCompletionTNSType.Info);
                    Opportunity.OpportunitySharedData.DestinationInventoryType simInventory = Opportunity.OpportunitySharedData.DestinationInventoryType.SimInventory;
                    row.TryGetEnum<Opportunity.OpportunitySharedData.DestinationInventoryType>("DestinationInventory", out simInventory, Opportunity.OpportunitySharedData.DestinationInventoryType.SimInventory);
                    sharedData.mDestinationInventoryType = simInventory;
                    string str3 = row.GetString("Target");
                    if (str3 != string.Empty)
                    {
                        ParserFunctions.TryParseEnum<OpportunityTargetTypes>(str3, out sharedData.mTargetType, OpportunityTargetTypes.Undefined);
                    }
                    else if (name != string.Empty)
                    {
                        sharedData.mTargetType = OpportunityTargetTypes.SetupObject;
                    }
                    sharedData.mTargetData = row.GetString("TargetData");
                    string str4 = row.GetString("Source");
                    if (str4 != string.Empty)
                    {
                        ParserFunctions.TryParseEnum<OpportunityTargetTypes>(str4, out sharedData.mSourceType, OpportunityTargetTypes.Undefined);
                    }
                    sharedData.mSourceData = row.GetString("SourceData");
                    string str5 = row.GetString("TargetInteractionName");
                    if (str5 != string.Empty)
                    {
                        sharedData.mTargetInteractionName = "Gameplay/Excel/Opportunities/OpportunitiesSetup:" + str5;
                    }
                    sharedData.mSocialTextKeys = row.GetStringList("TargetSocialTextKeys", ',');
                    sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.TargetInteractionIgnoreTarget, row.GetBool("TargetInteractionIgnoreTarget"));
                    sharedData.mTargetInteractionLikingToAccept = row.GetFloat("TargetInteractionLikingToAccept");
                    ParserFunctions.TryParseEnum<Role.RoleType>(row.GetString("TargetRoleRequired"), out sharedData.mTargetRoleRequired, Role.RoleType.None);
                    string str6 = row.GetString("TargetWorldRequired");
                    if ((str6 == "HomeWorld") || (sharedData.mProductVersion == ProductVersion.BaseGame))
                    {
                        sharedData.mTargetWorldRequired = WorldName.SunsetValley;
                    }
                    else
                    {
                        ParserFunctions.TryParseEnum<WorldName>(str6, out sharedData.mTargetWorldRequired, WorldName.Undefined);
                    }
                    sharedData.mTargetInteractionLength = row.GetFloat("TargetInteractionLength");
                    sharedData.mTargetInteractionDays = row.GetString("TargetInteractionDays");
                    sharedData.mTargetInteractionStartTime = ParserFunctions.ParseTime(row.GetString("TargetInteractionStartTime"));
                    sharedData.mTargetInteractionEndTime = ParserFunctions.ParseTime(row.GetString("TargetInteractionEndTime"));
                    string str7 = row.GetString("TargetInteractionCommodity");
                    if (!string.IsNullOrEmpty(str7))
                    {
                        string[] strArray2 = null;
                        if (ParserFunctions.ParseCommaSeparatedString(str7, out strArray2) && (strArray2.Length == 0x2))
                        {
                            ParserFunctions.TryParseEnum<CommodityKind>(strArray2[0x0], out sharedData.mTargetCommodity, CommodityKind.None);
                            float.TryParse(strArray2[0x1], out sharedData.mTargetCommodityDesiredValue);
                        }
                    }
                    string str8 = row.GetString("TargetInteractionItemRequired");
                    if (str8 != string.Empty)
                    {
                        string[] strArray3 = null;
                        if (ParserFunctions.ParseSemicolonSeparatedString(str8, out strArray3, false))
                        {
                            for (int i = 0x0; i < strArray3.Length; i++)
                            {
                                string[] strArray4 = null;
                                if (ParserFunctions.ParseCommaSeparatedString(strArray3[i].Trim(), out strArray4, false))
                                {
                                    for (int j = 0x0; j < strArray4.Length; j++)
                                    {
                                        strArray4[j] = strArray4[j].Trim();
                                    }
                                    if ((!OpportunityManager.ParseTargetInteractionItemInfo(strArray4, ref sharedData) && OpportunityManager.sDoValidation) && (sharedData.mRequiredItems != null))
                                    {
                                        int count = sharedData.mRequiredItems.Count;
                                    }
                                }
                            }
                        }
                    }
                    int num3 = (sharedData.mRequiredItems == null) ? 0x0 : sharedData.mRequiredItems.Count;
                    sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.UseCollectibleProgressText, row.GetBool("TargetUseCollectibleProgressText"));
                    string str10 = row.GetString("TargetDeletionSortingFunction");
                    if ((!string.IsNullOrEmpty(str10) && (OpportunityManager.sOpportunitiesSortClassType != null)) && ParserFunctions.ParseSemicolonSeparatedString(str10, out strArray5))
                    {
                        int length = strArray5.Length;
                        for (int k = 0x0; k < strArray5.Length; k++)
                        {
                            MethodInfo method = OpportunityManager.sOpportunitiesSortClassType.GetMethod(strArray5[k], BindingFlags.Public | BindingFlags.Static);
                            if (method != null)
                            {
                                sharedData.mRequiredItems[k].mInventorySortFn = Delegate.CreateDelegate(typeof(InventoryItemSortDelegate), method) as InventoryItemSortDelegate;
                            }
                        }
                    }
                    if (ParserFunctions.ParseSemicolonSeparatedString(row.GetString("TargetInteractionNumberItemsRequired"), out strArray6))
                    {
                        int num10 = strArray6.Length;
                        for (int m = 0x0; m < strArray6.Length; m++)
                        {
                            List<int> list;
                            int num6 = 0x0;
                            int num7 = 0x0;
                            ParserFunctions.ParseCommaSeperatedInt(strArray6[m], out list);
                            if (list.Count == 0x1)
                            {
                                num6 = num7 = list[0x0];
                            }
                            else if (list.Count == 0x2)
                            {
                                num6 = Math.Min(list[0x0], list[0x1]);
                                num7 = Math.Max(list[0x0], list[0x1]);
                            }
                            sharedData.mRequiredItems[m].mTargetInteractionNumberItemsRequiredMin = num6;
                            sharedData.mRequiredItems[m].mTargetInteractionNumberItemsRequiredMax = num7;
                        }
                    }
                    if ((sharedData.mRequiredItems == null) || ((sharedData.mRequiredItems.Count == 0x1) && (sharedData.mRequiredItems[0x0].mTargetInteractionNumberItemsRequiredMin <= 0x0)))
                    {
                        sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.UseCollectibleProgressText, false);
                    }
                    sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.TargetInteractionItemsDeleted, row.GetBool("TargetInteractionItemsDeleted"));
                    string str12 = row.GetString("TargetProceduralDeletionFunction");
                    if (!string.IsNullOrEmpty(str12) && (OpportunityManager.sOpportunitiesDeletionDelegateClassType != null))
                    {
                        MethodInfo info2 = OpportunityManager.sOpportunitiesDeletionDelegateClassType.GetMethod(str12, BindingFlags.Public | BindingFlags.Static);
                        if (info2 != null)
                        {
                            sharedData.mTargetProceduralDeletionDelegate = Delegate.CreateDelegate(typeof(OpportunityTargetProceduralDeletionDelegate), info2) as OpportunityTargetProceduralDeletionDelegate;
                        }
                    }
                    if ((OpportunityManager.sDoValidation && (sharedData.mRequiredItems != null)) && (sharedData.mRequiredItems.Count > 0x0))
                    {
                        for (int n = 0x0; n < sharedData.mRequiredItems.Count; n++)
                        {
                            if (((sharedData.TargetInteractionItemRequired(n) != null) && (sharedData.mTargetProceduralDeletionDelegate == null)) && ((sharedData.mRequiredItems[n].mTargetInteractionNumberItemsRequiredMin <= 0x0) && (sharedData.mGuid != OpportunityNames.EP1_Quest_Necteaux6)))
                            {
                                OpportunityNames mGuid = sharedData.mGuid;
                            }
                        }
                    }
                    string str13 = row.GetString("AdventureMapTagTarget");
                    if (!string.IsNullOrEmpty(str13))
                    {
                        string[] strArray7;
                        ParserFunctions.ParseCommaSeparatedString(str13, out strArray7);
                        if (strArray7.Length == 0x2)
                        {
                            ParserFunctions.TryParseEnum<AdventureMapTagTargetType>(strArray7[0x0], out sharedData.mAdventureMapTagTargetType, AdventureMapTagTargetType.None);
                            if (sharedData.mAdventureMapTagTargetType != AdventureMapTagTargetType.None)
                            {
                                sharedData.mAdventureMapTagTarget = strArray7[0x1];
                            }
                        }
                    }
                    string str14 = row.GetString("OppMapDagDisplayType");
                    if (!string.IsNullOrEmpty(str14))
                    {
                        ParserFunctions.TryParseEnum<Opportunity.OpportunitySharedData.MapTagOppDisplayType>(str14, out sharedData.MapTagDisplayType, Opportunity.OpportunitySharedData.MapTagOppDisplayType.Normal);
                    }
                    DaysOfTheWeek none = DaysOfTheWeek.None;
                    if (row.TryGetEnum<DaysOfTheWeek>("EventDay", out none, DaysOfTheWeek.None))
                    {
                        sharedData.mEventDay = none;

                        if (row.GetString("EventStartTime") != null)
                        {
                            sharedData.mEventStartTime = ParserFunctions.ParseTime(row.GetString("EventStartTime"));
                        }
                        else
                        {
                            BooterLogger.AddError(file + ": EventStartTime missing " + row["GUID"]);
                        }

                        if (row.GetString("EventEndTime") != null)
                        {
                            sharedData.mEventEndTime = ParserFunctions.ParseTime(row.GetString("EventEndTime"));
                        }
                        else
                        {
                            BooterLogger.AddError(file + ": EventEndTime missing " + row["GUID"]);
                        }

                        sharedData.mEventSetup = row.GetString("EventSetup");
                    }
                    opportunity.SourceData = sharedData.mSourceData;
                    opportunity.SourceType = sharedData.mSourceType;
                    opportunity.TargetData = sharedData.mTargetData;
                    opportunity.TargetType = sharedData.mTargetType;
                }
            }
        }

        public static ArrayList ParseRewards(XmlDbRow dr, string[] columnKeys, int numRewards)
        {
            try
            {
                ArrayList list = new ArrayList();
                for (int i = 0; i < numRewards; i++)
                {
                    bool flag = false;
                    List<string> entry = dr.GetStringList(columnKeys[i], ',', false);
                    if (entry.Count > 0)
                    {
                        RewardType type;
                        for (int j = 0; j < entry.Count; j++)
                        {
                            entry[j] = entry[j].Trim();
                        }

                        if (ParserFunctions.TryParseEnum<RewardType>(entry[0], out type, RewardType.Undefined))
                        {
                            RewardInfo info = null;
                            switch (type)
                            {
                                case RewardType.BookSkill:
                                    info = RewardsManager.ParseBookSkillRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.Buff:
                                    info = RewardsManager.ParseBuffRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.CareerDemotion:
                                case RewardType.CareerFired:
                                case RewardType.CareerPromotion:
                                case RewardType.UnknownRecipe:
                                case RewardType.CompedMeal:
                                case RewardType.BookQualityIncrease:
                                case RewardType.ConcertsPerformed:
                                case RewardType.RandomInvention:
                                case RewardType.AgeUp:
                                case RewardType.AgeDown:
                                    info = new RewardInfo();
                                    info.mType = type;
                                    flag = true;
                                    break;

                                case RewardType.Ingredient:
                                    info = RewardsManager.ParseIngredientRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.Money:
                                    info = RewardsManager.ParseMoneyRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RandomGroupMeal:
                                    info = RewardsManager.ParseRandomGroupMealRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RandomPlantable:
                                    info = RewardsManager.ParseRandomPlantableRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RandomObject:
                                    info = RewardsManager.ParseRandomObjectReward(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.SkillPercentage:
                                    info = ParseSkillRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RandomFish:
                                    info = RewardsManager.ParseRandomFishRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.PaintingValueBoost:
                                    info = RewardsManager.ParsePaintingValueBoostRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.UnlearnedComposition:
                                    info = RewardsManager.ParseUnlearnedCompositionInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.GroupMeal:
                                    info = RewardsManager.ParseGroupMealReward(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.ObjectInMail:
                                    info = RewardsManager.ParseObjectInMailReward(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.Harvestable:
                                    info = RewardsManager.ParseHarvestableRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.OmniPlantSeeds:
                                    info = RewardsManager.ParseOmniPlantSeedsRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.AncientCoin:
                                    info = RewardsManager.ParseAncientCoinInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.VisaPoints:
                                    info = RewardsManager.ParseVisaPointsInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.TreasureComponentRow:
                                    info = RewardsManager.ParseTreasureComponentRow(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.ActiveCareerPerformance:
                                    info = RewardsManager.ParseActiveCareerPerformance(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.MedatorInstance:
                                    info = RewardsManager.ParseMedatorInstance(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.Scrap:
                                    info = RewardsManager.ParseScrapInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.CelebrityPoints:
                                case RewardType.CelebrityPointsFromGig:
                                    if (GameUtils.IsInstalled(ProductVersion.EP3))
                                    {
                                        info = RewardsManager.ParseCelebrityPointsInfo(type, entry);
                                        flag = info != null;
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                    break;

                                case RewardType.RockBandGigPay:
                                    info = RewardsManager.ParseRockBandGigPayRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.PetSkillPercentage:
                                    info = RewardsManager.ParsePetSkillPercentageRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RemoveBuff:
                                    info = RewardsManager.ParseRemoveBuffRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.CommodityDecayModifier:
                                    info = RewardsManager.ParseCommodityDecayModifierRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.SetCommodity:
                                    info = RewardsManager.ParseSetCommodityRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.AddToCommodity:
                                    info = RewardsManager.ParseAddToCommodityRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.AddOccult:
                                    info = RewardsManager.ParseAddOccultRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RemoveOccult:
                                    info = RewardsManager.ParseRemoveOccultRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.SetBodyShapeFitness:
                                    info = RewardsManager.ParseSetBodyShapeFitnessRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.SetBodyShapeWeight:
                                    info = RewardsManager.ParseSetBodyShapeWeightRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.Trait:
                                    info = RewardsManager.ParseTraitRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RandomizeTraits:
                                    info = RewardsManager.ParseRandomizeTraitsRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.OppositeTraits:
                                    info = RewardsManager.ParseOppositeTraitsRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.IncreaseToSkillLevel:
                                    info = RewardsManager.ParseIncreaseToSkillLevelRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.BookGeneral:
                                    info = RewardsManager.ParseBookGeneralRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RandomAlchemyPotion:
                                    info = RewardsManager.ParseRandomAlchemyPotionRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.UnchartedIslandDiscovery:
                                    info = RewardsManager.ParseUnchartedIslandDiscoveryRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RequestCauseEffectWorldType:
                                    info = RewardsManager.ParseRequestCauseEffectWorldTypeRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.TemporaryTrait:
                                    info = RewardsManager.ParseTemporaryTraitRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.SkillMultiplier:
                                    info = RewardsManager.ParseSkillMultiplierRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RandomBuff:
                                    info = RewardsManager.ParseRandomBuffRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.RemoveBuffs:
                                    info = RewardsManager.ParseRemoveBuffsRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                case RewardType.TraitChip:
                                    info = RewardsManager.ParseTraitChipRewardInfo(entry);
                                    flag = info != null;
                                    break;

                                default:
                                    info = new RewardInfo();
                                    info.mType = type;
                                    if (entry.Count == 0x2)
                                    {
                                        flag = int.TryParse(entry[0x1], out info.mAmount);
                                    }
                                    break;
                            }
                            if (flag)
                            {
                                list.Add(info);
                            }
                        }
                        else
                        {
                            Type rewardType = dr.GetClassType(columnKeys[i]);
                            if (rewardType != null)
                            {
                                RewardInfoEx reward = (RewardInfoEx)rewardType.GetConstructor(new Type[0]).Invoke(new object[0]);
                                if (reward != null)
                                {
                                    list.Add(reward);
                                }
                                else
                                {
                                    BooterLogger.AddError("Unknown reward: " + dr.GetString(columnKeys[i]));
                                }
                            }
                            else
                            {
                                BooterLogger.AddError("Unknown reward: " + dr.GetString(columnKeys[i]));
                            }
                        }
                    }
                }

                if (list.Count != 0)
                {
                    return list;
                }
            }
            catch (Exception e)
            {
                Common.Exception("ParseRewards", e);
            }
            return null;
        }

        private static RewardInfo ParseSkillRewardInfo(List<string> entry)
        {
            if (entry.Count == 0x3)
            {
                RewardInfo info = new RewardInfo();
                info.mType = RewardType.SkillPercentage;

                // Custom
                SkillNames names = SkillManager.sSkillEnumValues.ParseEnumValue(entry[1]);

                info.mGuid = (ulong)names;
                if (int.TryParse(entry[0x2], out info.mAmount))
                {
                    return info;
                }
            }
            return null;
        }

        private static ArrayList ParseModifiers(XmlDbRow dr, string[] columnKeys, int numRewards)
        {
            ArrayList list = new ArrayList();
            for (int i = 0x0; i < numRewards; i++)
            {
                bool flag = false;
                List<string> stringList = dr.GetStringList(columnKeys[i], ',');
                if (stringList.Count > 0x0)
                {
                    ModifierType type;
                    ParserFunctions.TryParseEnum<ModifierType>(stringList[0x0], out type, ModifierType.Undefined);
                    ModifierInfo info = null;
                    switch (type)
                    {
                        case ModifierType.Mood:
                        case ModifierType.RelationshipWithBoss:
                        case ModifierType.RelationshipWithCoworkers:
                            info = new ModifierInfo();
                            info.mType = type;
                            flag = int.TryParse(stringList[0x1], out info.mAmount);
                            break;

                        case ModifierType.Skill:
                            info = ParseSkillModifierInfo(stringList);
                            flag = info != null;
                            break;
                    }
                    if (flag)
                    {
                        list.Add(info);
                    }
                }
            }
            if (list.Count != 0x0)
            {
                return list;
            }
            return null;
        }

        private static ModifierInfo ParseSkillModifierInfo(List<string> entry)
        {
            if (entry.Count == 0x3)
            {
                ModifierInfo info = new ModifierInfo();
                info.mType = ModifierType.Skill;

                // Custom
                SkillNames names = SkillManager.sSkillEnumValues.ParseEnumValue(entry[1]);

                info.mGuid = (ulong)names;
                int.TryParse(entry[0x2], out info.mAmount);
                return info;
            }
            return null;
        }

        private static void ParseOpportunityCompletion(BooterHelper.DataBootFile file, int total)
        {
            XmlDbTable table = file.GetTable("OpportunitiesCompletion");
            if (table == null) return;

            BooterLogger.AddTrace(file + ": Found Completion = " + table.Rows.Count);

            if (total != table.Rows.Count)
            {
                BooterLogger.AddError(file + ": Found Too few Completion " + total + " < " + table.Rows.Count);
            }

            foreach (XmlDbRow row in table.Rows)
            {
                OpportunityNames names = GenericManager<OpportunityNames, Opportunity, Opportunity>.ParseGuid(row["GUID"]);

                // New code
                if (names == OpportunityNames.Undefined)
                {
                    names = unchecked((OpportunityNames)ResourceUtils.HashString64(row["GUID"]));
                }

                OpportunityNames names2 = GenericManager<OpportunityNames, Opportunity, Opportunity>.ParseGuid(row["TriggerOpportunityOnCompletion"]);
                bool sDoValidation = OpportunityManager.sDoValidation;
                if (GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary.ContainsKey((ulong)names))
                {
                    Opportunity.OpportunitySharedData sharedData = GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary[(ulong)names].SharedData;
                    sharedData.mCompletionTriggerOpportunity = names2;
                    if (names2 != OpportunityNames.Undefined)
                    {
                        GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary[(ulong)names2].SharedData.mParentOpportunity = names;
                    }
                    sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.TriggerQuietly, row.GetBool("TriggerQuietly"));
                    sharedData.SetFlags(Opportunity.OpportunitySharedData.FlagField.ShowRewardText, row.GetBool("ShowRewardText"));
                    sharedData.mCompletionWinChance = row.GetFloat("CompletionWinChance");
                    sharedData.mWinRewardsList = ParseRewards(row, OpportunityManager.sCompletionWinRewardColumns, 0x4);
                    sharedData.mLossRewardsList = ParseRewards(row, OpportunityManager.sCompletionLossRewardColumns, 0x4);
                    sharedData.mFailureRewardsList = ParseRewards(row, OpportunityManager.sFailureRewardColumns, 0x4);
                    sharedData.mModifierList = ParseModifiers(row, OpportunityManager.sCompletionModifierColumns, 0x4);
                    List<string> stringList = row.GetStringList("CustomOnWinCompletionFunction", ',');
                    if (stringList.Count > 0x0)
                    {
                        bool flag2 = OpportunityManager.sDoValidation;
                        sharedData.mWinCompletionDelegate = (OpportunityCompletionDelegate)OpportunityManager.ParseDelegatePair(stringList[0x0], stringList[0x1], typeof(OpportunityCompletionDelegate));
                    }
                    List<string> list2 = row.GetStringList("CustomOnLossCompletionFunction", ',');
                    if (list2.Count > 0x0)
                    {
                        bool flag3 = OpportunityManager.sDoValidation;
                        sharedData.mLossCompletionDelegate = (OpportunityCompletionDelegate)OpportunityManager.ParseDelegatePair(list2[0x0], list2[0x1], typeof(OpportunityCompletionDelegate));
                    }
                }
            }

            foreach (Opportunity opportunity in GenericManager<OpportunityNames, Opportunity, Opportunity>.sDictionary.Values)
            {
                OpportunityTargetTypes types;
                string str;
                OpportunityTargetTypes types2;
                string str2;
                if (((opportunity.SharedData.mSourceType == OpportunityTargetTypes.PreviousSource) || (opportunity.SharedData.mSourceType == OpportunityTargetTypes.PreviousTarget)) && (OpportunityManager.FindOriginalSourceOrTarget(opportunity, opportunity.SharedData.mSourceType, out types, out str) != null))
                {
                    opportunity.SharedData.mOriginalSourceType = types;
                }
                if (((opportunity.SharedData.mTargetType == OpportunityTargetTypes.PreviousSource) || (opportunity.SharedData.mTargetType == OpportunityTargetTypes.PreviousTarget)) && (OpportunityManager.FindOriginalSourceOrTarget(opportunity, opportunity.SharedData.mTargetType, out types2, out str2) != null))
                {
                    opportunity.SharedData.mOriginalTargetType = types2;
                }
            }
        }
    }
}
