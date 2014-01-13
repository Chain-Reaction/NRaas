using NRaas.CareerSpace.Interactions;
using NRaas.CareerSpace.Interfaces;
using NRaas.CareerSpace.Skills;
using NRaas.CommonSpace.Booters;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace;
using NRaas.Gameplay.OmniSpace.Metrics;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
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
    public class JobsBooter : BooterHelper.ListingBooter
    {
        static Dictionary<JobId, string> sIdToJobs = new Dictionary<JobId, string>();

        public JobsBooter()
            : this(VersionStamp.sNamespace + ".Jobs", true)
        { }
        public JobsBooter(string reference, bool testDirect)
            : base("JobsFile", "File", reference, testDirect)
        { }

        public static string JobToString(JobId id)
        {
            string result;
            if (!sIdToJobs.TryGetValue(id, out result)) return null;

            return result;
        }

        protected override void PerformFile(BooterHelper.BootFile paramFile)
        {
            BooterHelper.DataBootFile file = paramFile as BooterHelper.DataBootFile;

            XmlDbTable jobsTable = file.GetTable ("Jobs");
            if (jobsTable == null)
            {
                if (file.mPrimary)
                {
                    BooterLogger.AddTrace(file + ": No Jobs Table");
                }
                else
                {
                    BooterLogger.AddError(file + ": No Jobs Table");
                }

                return;
            }

            Dictionary<ulong, Dictionary<uint, JobStaticData>> careerToJobStaticDataMap = GenerateOccupationToJobStaticDataMap(jobsTable);

            BooterLogger.AddTrace("Jobs: " + careerToJobStaticDataMap.Count);

            foreach(KeyValuePair<ulong, Dictionary<uint, JobStaticData>> occupationPair in careerToJobStaticDataMap)
            {
                OccupationStaticData data;
                if (!Occupation.sOccupationStaticDataMap.TryGetValue(occupationPair.Key, out data)) continue;

                foreach (KeyValuePair<uint, JobStaticData> jobPair in occupationPair.Value)
                {
                    data.JobStaticDataMap[jobPair.Key] = jobPair.Value;
                }
            }
        }

        // From ActiveCareer::GenerateOccupationToJobStaticDataMap
        private static Dictionary<ulong, Dictionary<uint, JobStaticData>> GenerateOccupationToJobStaticDataMap(XmlDbTable jobsTable)
        {
            Dictionary<ulong, Dictionary<uint, JobStaticData>> careerToJobStaticDataMap = new Dictionary<ulong, Dictionary<uint, JobStaticData>>();

            bool flag = true;
            bool hasActiveCareer = false;
            string jobTitle = string.Empty;
            int hoursAvailable = 0x0;
            DaysOfTheWeek daysAvailable = ~DaysOfTheWeek.None;
            float jobStartTime = -1f;
            int budget = 0x0;
            int cash = 0x0;
            float experience = 0f;
            List<JobId> parents = null;
            List<TaskCreationStaticData> tasks = null;
            string mapTagText = null;
            string mapTagIcon = null;
            bool mapTagShowInOffHours = false;
            JobId jobId = JobId.Invalid;
            JobDestinaitonType destinationType = JobDestinaitonType.Invalid;
            CommercialLotSubType[] commercialLotSubTypes = null;
            RabbitHoleType none = RabbitHoleType.None;
            RabbitHoleType[] rabbitholeTypes = null;
            int[] durationMinMax = null;
            OccupationNames activeCareer = OccupationNames.Undefined;
            JobCooldownDefinition[] jobCooldownDefinitions = null;
            string jobInteractionName = null;
            string jobIntroductionKey = null;
            string jobIntroductionWithSimKey = null;
            TNSNames tnsID = TNSNames.Invalid;
            string musicClipName = null;
            List<string> list3 = new List<string>();
            List<CooldownSpecificity> list4 = new List<CooldownSpecificity>();
            foreach (string str8 in jobsTable.DefaultRow.ColumnNames)
            {
                if (str8.StartsWith("Cooldown_"))
                {
                    CooldownSpecificity specificity;
                    if (!ParserFunctions.TryParseEnum<CooldownSpecificity>(str8.Substring(0x9).Replace('_', ','), out specificity, CooldownSpecificity.None))
                    {
                        return careerToJobStaticDataMap;
                    }
                    list3.Add(str8);
                    list4.Add(specificity);
                }
            }

            int count = list3.Count;
            foreach (XmlDbRow row in jobsTable.Rows)
            {
                List<string> stringList;

                OccupationNames undefined = OccupationNames.Undefined;
                if (!row.TryGetEnum<OccupationNames>("ActiveCareer", out undefined, OccupationNames.Undefined))
                {
                    undefined = unchecked((OccupationNames)ResourceUtils.HashString64(row.GetString("ActiveCareer")));
                }

                BooterLogger.AddTrace("ActiveCareer: " + row.GetString("ActiveCareer"));

                if (undefined != OccupationNames.Undefined)
                {
                    hasActiveCareer = true;
                    OccupationNames names4 = undefined;
                    if (undefined != activeCareer)
                    {
                        names4 = activeCareer;
                    }
                    activeCareer = undefined;
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        ActiveCareer.AddNewJob(careerToJobStaticDataMap, ref hoursAvailable, ref daysAvailable, ref jobStartTime, ref budget, ref cash, ref experience, ref parents, ref tasks, ref mapTagText, ref mapTagIcon, ref mapTagShowInOffHours, jobId, names4, ref destinationType, ref commercialLotSubTypes, ref none, ref durationMinMax, ref rabbitholeTypes, ref jobTitle, jobCooldownDefinitions, ref jobInteractionName, ref jobIntroductionKey, ref jobIntroductionWithSimKey, ref tnsID, ref musicClipName);
                    }
                }
                else if (hasActiveCareer)
                {
                    hasActiveCareer = false;
                }
                else if (!string.IsNullOrEmpty(row.GetString("ActiveCareer")))
                {
                    return careerToJobStaticDataMap;
                }

                if (hasActiveCareer)
                {
                    if (!row.TryGetEnum<JobId>("Job_Id", out jobId, JobId.Invalid))
                    {
                        jobId = unchecked((JobId)ResourceUtils.HashString64(row.GetString("Job_Id")));

                        sIdToJobs[jobId] = row.GetString("Job_Id");
                    }

                    BooterLogger.AddTrace("Job_Id: " + row.GetString("Job_Id"));

                    if (!row.IsInstalled("SKU_Installed") || !row.IsRegistered("SKU_Registered"))
                    {
                        if (Occupation.sValidJobsNotInstalled == null)
                        {
                            Occupation.sValidJobsNotInstalled = new List<KeyValuePair<OccupationNames, JobId>>();
                        }
                        Occupation.sValidJobsNotInstalled.Add(new KeyValuePair<OccupationNames, JobId>(undefined, jobId));
                        continue;
                    }

                    jobTitle = row.GetString("Job_Title");
                    hoursAvailable = row.GetInt("Hours_Available", 0x0);
                    daysAvailable = ParserFunctions.ParseDayListToEnum(row.GetString("Days_Available"));
                    jobStartTime = row.GetFloat("Start_Time", -1f);
                    budget = row.GetInt("Budget", 0x0);
                    cash = row.GetInt("Payout_Cash", 0x0);
                    experience = row.GetInt("Payout_XP", 0x0);
                    mapTagText = row.GetString("Map_Tag_Text");
                    mapTagIcon = row.GetString("Map_Tag_Icon");
                    mapTagShowInOffHours = row.GetString("Show_Map_Tag_In_Off_Hours") == "x";
                    jobInteractionName = row.GetString("Job_InteractionName");
                    jobIntroductionKey = row.GetString("Job_IntroductionKey");
                    jobIntroductionWithSimKey = row.GetString("Job_IntroductionWithSimKey");
                    row.TryGetEnum<TNSNames>("Job_CompletionTNS", out tnsID, TNSNames.Invalid);
                    musicClipName = row.GetString("Music");
                    string str11 = row.GetString("Valid_Destination");
                    bool flag4 = false;
                    if (!string.IsNullOrEmpty(str11))
                    {
                        if (!row.TryGetEnum<JobDestinaitonType>("Valid_Destination", out destinationType, JobDestinaitonType.Invalid))
                        {
                            flag4 = true;
                        }

                        switch (destinationType)
                        {
                            case JobDestinaitonType.Commercial:
                            case JobDestinaitonType.ResidentialAndCommercial:
                                string str12 = row.GetString("Destination_Arguments");
                                if (!string.IsNullOrEmpty(str12))
                                {
                                    List<CommercialLotSubType> list5;
                                    if (!ParserFunctions.TryParseCommaSeparatedList<CommercialLotSubType>(str12, out list5, CommercialLotSubType.kCommercialUndefined))
                                    {
                                        flag4 = true;
                                    }
                                    else
                                    {
                                        commercialLotSubTypes = list5.ToArray();
                                    }
                                }
                                break;
                            case JobDestinaitonType.RabbitHole:
                                stringList = row.GetStringList("Destination_Arguments", ',');
                                if ((stringList != null) && (stringList.Count == 0x3))
                                {
                                    if (ParserFunctions.TryParseEnum<RabbitHoleType>(stringList[0x0], out none, RabbitHoleType.None))
                                    {
                                        int num7 = ParserFunctions.ParseInt(stringList[0x1], -1);
                                        int num8 = ParserFunctions.ParseInt(stringList[0x2], -1);
                                        durationMinMax = new int[] { num7, num8 };
                                    }
                                    else
                                    {
                                        flag4 = true;
                                    }
                                }
                                else
                                {
                                    flag4 = true;
                                }
                                break;
                            case JobDestinaitonType.RabbitHoleLot:
                                string str13 = row.GetString("Destination_Arguments");
                                if (!string.IsNullOrEmpty(str13))
                                {
                                    List<RabbitHoleType> list7;
                                    if (!ParserFunctions.TryParseCommaSeparatedList<RabbitHoleType>(str13, out list7, RabbitHoleType.None))
                                    {
                                        flag4 = true;
                                    }
                                    else
                                    {
                                        rabbitholeTypes = list7.ToArray();
                                    }
                                }
                                break;
                        }
                    }

                    jobCooldownDefinitions = null;
                    List<JobCooldownDefinition> list8 = new List<JobCooldownDefinition>(count);
                    for (int i = 0x0; i < count; i++)
                    {
                        int @int = row.GetInt(list3[i]);
                        if (@int != 0x0)
                        {
                            list8.Add(new JobCooldownDefinition(list4[i], (float)@int));
                        }
                    }

                    if (list8.Count > 0x0)
                    {
                        jobCooldownDefinitions = list8.ToArray();
                    }

                    if (flag4)
                    {
                        continue;
                    }
                }

                if (!string.IsNullOrEmpty(row.GetString("Parent_Job_Ids")))
                {
                    JobId id2;
                    if (!row.TryGetEnum<JobId>("Parent_Job_Ids", out id2, JobId.Invalid))
                    {
                        id2 = unchecked((JobId)ResourceUtils.HashString64(row.GetString("Parent_Job_Ids")));

                        sIdToJobs[id2] = row.GetString("Parent_Job_Ids");
                    }

                    if (parents == null)
                    {
                        parents = new List<JobId>();
                    }
                    parents.Add(id2);
                }

                if (!string.IsNullOrEmpty(row.GetString("Task_Id")))
                {
                    TaskId id3;
                    if (!row.TryGetEnum<TaskId>("Task_Id", out id3, TaskId.Invalid))
                    {
                        id3 = unchecked((TaskId)ResourceUtils.HashString64(row.GetString("Task_Id")));
                    }

                    if (tasks == null)
                    {
                        tasks = new List<TaskCreationStaticData>();
                    }
                    int lowerBound = row.GetInt("Task_Lower_Bound");
                    int upperBound = row.GetInt("Task_Upper_Bound");
                    TaskCreationStaticData item = new TaskCreationStaticData(id3, lowerBound, upperBound);
                    tasks.Add(item);
                }
            }

            ActiveCareer.AddNewJob(careerToJobStaticDataMap, ref hoursAvailable, ref daysAvailable, ref jobStartTime, ref budget, ref cash, ref experience, ref parents, ref tasks, ref mapTagText, ref mapTagIcon, ref mapTagShowInOffHours, jobId, activeCareer, ref destinationType, ref commercialLotSubTypes, ref none, ref durationMinMax, ref rabbitholeTypes, ref jobTitle, jobCooldownDefinitions, ref jobInteractionName, ref jobIntroductionKey, ref jobIntroductionWithSimKey, ref tnsID, ref musicClipName);

            return careerToJobStaticDataMap;
        }
    }
}
