using NRaas.StoryProgressionSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Xml;

namespace NRaas.StoryProgressionSpace.Careers
{
    [Persistable]
    public class Retired : PartTimeJob, Common.IPreLoad, Common.IWorldLoadFinished
    {
        public Retired()
        {
            mCareerGuid = CareerGuid;
        }
        public Retired(XmlDbRow myRow, XmlDbTable levelTable, XmlDbTable eventDataTable)
            : base (myRow, levelTable, eventDataTable)
        {
            mCareerGuid = CareerGuid;
        } 

        private new static OccupationNames CareerGuid
        {
            get
            {
                return unchecked((OccupationNames)ResourceUtils.HashString64("NRaasStoryProgression.Retired"));
            }
        }

        public override int PensionAmount()
        {
            return 0;
        }

        public override int CareerLevel
        {
            get
            {
                try
                {
                    if (OwnerDescription != null)
                    {
                        Occupation retired = OwnerDescription.CareerManager.RetiredCareer;
                        if ((retired != null) && (!(retired is Retired)))
                        {
                            return retired.CareerLevel;
                        }
                    }

                    return base.CareerLevel;
                }
                catch (Exception e)
                {
                    Common.Exception(OwnerDescription, e);
                    return 0;
                }
            }
        }

        public override string CareerName
        {
            get
            {
                try
                {
                    string name = base.CareerName;

                    if ((OwnerDescription != null) && (OwnerDescription.CareerManager != null))
                    {
                        Occupation retired = OwnerDescription.CareerManager.RetiredCareer;
                        if ((retired != null) && (!(retired is Retired)))
                        {
                            name += ": " + retired.CareerName;
                        }
                    }

                    return name;
                }
                catch (Exception e)
                {
                    Common.Exception(OwnerDescription, e);
                    return "";
                }
            }
        }

        public override bool HasOpenHours
        {
            get { return true; }
        }

        public override string CareerIcon
        {
            get
            {
                try
                {
                    if ((OwnerDescription != null) && (OwnerDescription.CareerManager != null))
                    {
                        Occupation retired = OwnerDescription.CareerManager.RetiredCareer;
                        if ((retired != null) && (!(retired is Retired)))
                        {
                            return retired.CareerIcon;
                        }
                    }

                    return base.CareerIcon;
                }
                catch (Exception e)
                {
                    Common.Exception(OwnerDescription, e);
                    return "";
                }
            }
        }

        public override string CareerIconColored
        {
            get
            {
                try
                {
                    if ((OwnerDescription != null) && (OwnerDescription.CareerManager != null))
                    {
                        Occupation retired = OwnerDescription.CareerManager.RetiredCareer;
                        if ((retired != null) && (!(retired is Retired)))
                        {
                            return retired.CareerIconColored;
                        }
                    }
                    return base.CareerIconColored;
                }
                catch (Exception e)
                {
                    Common.Exception(OwnerDescription, e);
                    return "";
                }
            }
        }

        public override string CurLevelJobTitle
        {
            get
            {
                try
                {
                    string name = base.CareerName;

                    if ((OwnerDescription != null) && (OwnerDescription.CareerManager != null))
                    {
                        Occupation retired = OwnerDescription.CareerManager.RetiredCareer;
                        if ((retired != null) && (!(retired is Retired)))
                        {
                            name += ": " + retired.CurLevelJobTitle;
                        }
                    }

                    return name;
                }
                catch (Exception e)
                {
                    Common.Exception(OwnerDescription, e);
                    return "";
                }
            }
        }

        public override int CalculateBoostedStartingLevel()
        {
            return -1;
        }

        public override void ScheduleCarpool(bool bReschedule)
        {
            try
            {
                if ((OwnerDescription != null) &&
                    (OwnerDescription.CareerManager != null) &&
                    (OwnerDescription.CareerManager.RetiredCareer != null))
                {
                    Career career = OwnerDescription.CareerManager.RetiredCareer as Career;
                    if (career != null)
                    {
                        if (career.CurLevel == null) return;
                    }

                    mPayPerHourExtra = OwnerDescription.CareerManager.RetiredCareer.PensionAmount();
                }

                //base.ScheduleCarpool(bReschedule);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void SetCoworkersAndBoss()
        { 
            /* override of base class */
        }

        public override bool ShouldBeAtWork()
        {
            if ((OwnerDescription != null) &&
               (OwnerDescription.CareerManager != null) &&
               (OwnerDescription.CareerManager.RetiredCareer != null))
            {
                try
                {
                    mPayPerHourExtra = OwnerDescription.CareerManager.RetiredCareer.PensionAmount();
                }
                catch
                { }
            }

            return base.ShouldBeAtWork();
        }

        /*
        public override bool CareerAgeTest(SimDescription sim)
        {
            return sim.Elder;
        }
        */

        public override bool CanRetire()
        {
            return false;
        }

        public void OnPreLoad()
        {
            try
            {
                XmlDbData careerData = XmlDbData.ReadData("NRaas.StoryProgression.Retired");

                XmlDbData careerEventData = XmlDbData.ReadData("CareerEvents");

                if ((careerData != null) &&
                    (careerData.Tables != null) &&
                    (careerEventData != null) &&
                    (careerEventData.Tables != null) &&
                    (careerData.Tables.ContainsKey("CareerList")))
                {
                    XmlDbTable table = careerData.Tables["CareerList"];
                    foreach (XmlDbRow row in table.Rows)
                    {
                        string key = row.GetString("TableName");
                        if (key != "Retired") continue;

                        if (careerData.Tables.ContainsKey(key))
                        {
                            XmlDbTable table2 = careerData.Tables[key];
                            if (table2 != null)
                            {
                                XmlDbTable table3 = null;
                                careerEventData.Tables.TryGetValue(key, out table3);

                                Career career = new Retired(row, table2, table3);

                                if ((career != null) && (career.Guid != OccupationNames.Undefined))
                                {
                                    if (!GenericManager<OccupationNames, Occupation, Occupation>.sDictionary.ContainsKey((ulong)career.Guid))
                                    {
                                        GenericManager<OccupationNames, Occupation, Occupation>.sDictionary.Add((ulong)career.Guid, career);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception("Retired PreLoad", exception);
            }
        }

        public void OnWorldLoadFinished()
        {
            try
            {
                Managers.ManagerCareer.RetiredLocation = null;

                Career staticCareer = CareerManager.GetStaticCareer(CareerGuid);
                if (staticCareer == null) return;

                foreach (RabbitHole hole in RabbitHole.GetRabbitHolesOfType(RabbitHoleType.CityHall))
                {
                    CareerLocation location = new CareerLocation(hole, staticCareer);
                    if (!hole.CareerLocations.ContainsKey((ulong)staticCareer.Guid))
                    {
                        hole.CareerLocations.Add((ulong)staticCareer.Guid, location);

                        if (location.Career != null)
                        {
                            Managers.ManagerCareer.RetiredLocation = location;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception("Retired WorldLoadFinished", exception);
            }
        }
    }
}

