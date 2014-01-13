using NRaas.CareerSpace.Interactions;
using NRaas.CareerSpace.Interfaces;
using NRaas.CareerSpace.Skills;
using NRaas.CommonSpace.Booters;
using NRaas.Gameplay.Careers;
using NRaas.Gameplay.OmniSpace;
using NRaas.Gameplay.OmniSpace.Metrics;
using NRaas.Gameplay.Tones;
using Sims3.Gameplay.Abstracts;
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
    public class CareersBooter : BooterHelper.TableBooter, Common.IWorldLoadFinished
    {
        private static List<CareerBooterElement> sCareers = new List<CareerBooterElement> ();

        public CareersBooter()
            : this(VersionStamp.sNamespace + ".Careers", true)
        { }
        public CareersBooter(string reference, bool testDirect)
            : base("CareerFile", reference, testDirect)
        { }

        public override BooterHelper.BootFile GetBootFile(string reference, string name, bool primary)
        {
            return new BooterHelper.DataBootFile(reference, name, primary);
        }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            BooterHelper.DataBootFile careerFile = new BooterHelper.DataBootFile(file.ToString(), row.GetString("Careers"), false);
            if (!careerFile.IsValid)
            {
                BooterLogger.AddError(file.ToString() + ": Unknown Careers File " + row.GetString("Careers"));
                return;
            }

            BooterHelper.DataBootFile careerEventsFile = new BooterHelper.DataBootFile(careerFile.ToString(), row.GetString("CareerEvents"), false);

            if (careerEventsFile.IsValid)
            {
                foreach (Career career in CareerManager.CareerList)
                {
                    XmlDbTable table3 = careerEventsFile.GetTable(career.Guid.ToString ());
                    if (table3 != null)
                    {
                        LoadCareerEvents(career, careerEventsFile, table3);
                    }
                }
            }

            BooterHelper.DataBootTable table = new BooterHelper.DataBootTable(careerFile, "CareerList");
            if (!table.IsValid)
            {
                BooterLogger.AddError(file.ToString() + ": No CareerList " + careerFile.ToString());
                return;
            }

            table.Load(new CareerLoader(careerEventsFile).LoadCareer);
        }

        protected static void LoadCareerEvents(Career career, BooterHelper.BootFile eventsFile, XmlDbTable eventDataTable)
        {
            if (eventDataTable == null) return;

            BooterLogger.AddTrace(eventsFile + ": Found " + career.Name + " Events = " + eventDataTable.Rows.Count);

            foreach (XmlDbRow row in eventDataTable.Rows)
            {
                ProductVersion version;
                row.TryGetEnum<ProductVersion>("ProductVersion", out version, ProductVersion.BaseGame);

                if (GameUtils.IsInstalled(version))
                {
                    string str3 = row.GetString("EventName");
                    string str4 = row.GetString("OpportunityName");
                    bool flag = row.GetInt("SureShotEvent") == 1;

                    Type classType;
                    if ((str4 != string.Empty) && (str3 == string.Empty))
                    {
                        classType = typeof(Career.EventOpportunity);
                    }
                    else
                    {
                        classType = row.GetClassType("EventName");
                    }

                    Type[] types = new Type[] { typeof(XmlDbRow), typeof(Dictionary<string, Dictionary<int, CareerLevel>>), typeof(string) };
                    object obj = null;

                    try
                    {
                        obj = classType.GetConstructor(types).Invoke(new object[] { row, career.SharedData.CareerLevels, eventDataTable.Name });
                    }
                    catch(Exception e)
                    {
                        BooterLogger.AddError(eventsFile + ": Constructor Fail " + row.GetString("EventName"));

                        Common.Exception(eventsFile + ": Constructor Fail " + row.GetString("EventName"), e);
                    }

                    if (obj == null)
                    {
                        BooterLogger.AddError(eventsFile + ": Bad Class " + row.GetString("EventName"));
                    }
                    else
                    {
                        Career.EventOpportunity opportunityEvent = obj as Career.EventOpportunity;

                        // new Code
                        if ((opportunityEvent != null) && (opportunityEvent.mOpportunity == OpportunityNames.Undefined))
                        {
                            opportunityEvent.mOpportunity = unchecked((OpportunityNames)ResourceUtils.HashString64(str4));
                        }

                        Career.EventDaily dailyEvent = obj as Career.EventDaily;
                        if (dailyEvent != null)
                        {
                            if (dailyEvent.AvailableLevels(career).Count == 0)
                            {
                                BooterLogger.AddError(eventsFile + ": No AvailableLevels " + row.GetString("EventType"));
                            }
                            else
                            {
                                // new Code
                                if (dailyEvent.mEventType == Career.CareerEventType.None)
                                {
                                    dailyEvent.mEventType = unchecked((Career.CareerEventType)ResourceUtils.HashString64(row.GetString("EventType")));
                                }

                                if (career.SharedData == null)
                                {
                                    BooterLogger.AddError(eventsFile + ": Career SharedData missing " + career.mCareerGuid);
                                }
                                else
                                {
                                    if (flag)
                                    {
                                        career.SharedData.SureShotEvent = dailyEvent;
                                    }
                                    else
                                    {
                                        career.SharedData.CareerEventList.Add(dailyEvent);

                                        try
                                        {
                                            CareerEventManager.sAllEvents.Add(dailyEvent.EventType, dailyEvent);
                                        }
                                        catch
                                        {
                                            BooterLogger.AddError(eventsFile + ": Duplicate Event " + row.GetString("EventType"));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            BooterLogger.AddError(eventsFile + ": Not EventDaily " + row.GetString("EventType"));
                        }
                    }
                }
            }
        }

        public void OnWorldLoadFinished()
        {
            foreach (CareerBooterElement element in sCareers)
            {
                Career staticCareer = CareerManager.GetStaticCareer(element.mCareer);
                if (staticCareer == null) continue;

                string lotDesignator = null;

                ILotDesignator lotDesignatorClass = staticCareer as ILotDesignator;
                if (lotDesignatorClass != null)
                {
                    lotDesignator = lotDesignatorClass.LotDesignator;
                }

                foreach (RabbitHole hole in RabbitHole.GetRabbitHolesOfType(element.mType))
                {
                    if (!string.IsNullOrEmpty(lotDesignator))
                    {
                        if (hole.LotCurrent == null) continue;

                        if (!hole.LotCurrent.Name.Contains(lotDesignator)) continue;
                    }

                    CareerLocation location = new CareerLocation(hole, staticCareer);
                    if (!hole.CareerLocations.ContainsKey((ulong)staticCareer.Guid))
                    {
                        hole.CareerLocations.Add((ulong)staticCareer.Guid, location);
                    }
                }
            }
        }

        public class CareerLoader
        {
            BooterHelper.DataBootFile mCareerEventsFile;

            public CareerLoader(BooterHelper.DataBootFile careerEventsFile)
            {
                mCareerEventsFile = careerEventsFile;
            }

            public void LoadCareer(BooterHelper.BootFile file, XmlDbRow row)
            {
                BooterHelper.DataBootFile dataFile = file as BooterHelper.DataBootFile;
                if (dataFile == null) return;

                string careerName = row.GetString("CareerName");
                if (careerName == null)
                {
                    BooterLogger.AddError(file.ToString() + ": No CareerName");
                }
                else
                {
                    Type classType = row.GetClassType("FullClassName");
                    if (classType != null)
                    {
                        string key = row.GetString("TableName");

                        XmlDbTable levelTable = dataFile.GetTable(key);
                        if (levelTable != null)
                        {
                            foreach (XmlDbRow levelRow in levelTable.Rows)
                            {
                                XmlDbData.XmlDbRowFast level = levelRow as XmlDbData.XmlDbRowFast;
                                if (level == null) continue;

                                if (!level.Exists("DowntownWakeupTime"))
                                {
                                    level.mData.Add("DowntownWakeupTime", level.GetString("WakeupTime"));
                                }

                                if (!level.Exists("DowntownStartTime"))
                                {
                                    level.mData.Add("DowntownStartTime", level.GetString("StartTime"));
                                }

                                if (!level.Exists("DowntownDayLength"))
                                {
                                    level.mData.Add("DowntownDayLength", level.GetString("DayLength"));
                                }
                            }

                            Type[] types = new Type[] { typeof(XmlDbRow), typeof(XmlDbTable), typeof(XmlDbTable) };
                            Career career = null;

                            try
                            {
                                career = classType.GetConstructor(types).Invoke(new object[] { row, levelTable, null }) as Career;
                            }
                            catch (Exception e)
                            {
                                BooterLogger.AddError(careerName + ": Constructor Fail " + row.GetString("FullClassName"));

                                Common.Exception(careerName + ": Constructor Fail " + row.GetString("FullClassName"), e);
                                return;
                            }

                            if (career != null)
                            {
                                if (mCareerEventsFile.IsValid)
                                {
                                    XmlDbTable table3 = mCareerEventsFile.GetTable(key);
                                    if (table3 != null)
                                    {
                                        LoadCareerEvents(career, mCareerEventsFile, table3);
                                    }
                                }

                                career.mCareerGuid = unchecked((OccupationNames)ResourceUtils.HashString64(row.GetString("AltGuid")));

                                if (career.Guid == OccupationNames.Undefined)
                                {
                                    BooterLogger.AddError(careerName + ": No AltGuid");
                                }
                                else if (CareerManager.GetStaticCareer(career.mCareerGuid) != null)
                                {
                                    BooterLogger.AddError(careerName + ": Duplicate GUID");
                                }
                                else
                                {
                                    RabbitHoleType type = RabbitHoleType.None;
                                    ParserFunctions.TryParseEnum<RabbitHoleType>(row.GetString("RabbitholeType"), out type, RabbitHoleType.None);
                                    if (type != RabbitHoleType.None)
                                    {
                                        sCareers.Add(new CareerBooterElement(career.Guid, type));

                                        CareerManager.AddStaticOccupation(career);

                                        BooterLogger.AddTrace(careerName + ": Added to Rabbithole " + type);
                                    }
                                    else
                                    {
                                        BooterLogger.AddError(careerName + ": Unknown Rabbithole");
                                    }
                                }
                            }
                            else
                            {
                                BooterLogger.AddError(careerName + ": Constructor Fail " + row.GetString("FullClassName"));
                            }
                        }
                        else
                        {
                            BooterLogger.AddError(careerName + ": No TableName");
                        }
                    }
                    else
                    {
                        BooterLogger.AddError(careerName + ": Invalid FullClassName " + row.GetString("FullClassName"));
                    }
                }
            }
        }

        protected class CareerBooterElement
        {
            public readonly OccupationNames mCareer;
            public readonly RabbitHoleType mType;

            public CareerBooterElement(OccupationNames career, RabbitHoleType type)
            {
                mCareer = career;
                mType = type;
            }
        }
    }
}
