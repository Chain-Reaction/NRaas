using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public abstract class ChooseCareerOption : CareerOption
    {
        Item mChoice;

        public ChooseCareerOption()
        {}

        protected override bool CanApplyAll()
        {
            return true;
        }

        public static Lot FindLotType(CommercialLotSubType type)
        {
            List<Lot> lots = new List<Lot>();

            foreach (Lot lot in LotManager.AllLots)
            {
                if (!lot.IsCommunityLot) continue;

                if (lot.CommercialLotSubType == type)
                {
                    lots.Add(lot);
                }
            }

            if (lots.Count == 0) return null;

            return RandomUtil.GetRandomObjectFromList(lots);
        }

        protected abstract List<Item> GetOptions(SimDescription me);

        public static RabbitHole FindRabbitHole(RabbitHoleType type)
        {
            List<RabbitHole> holes = new List<RabbitHole>();
            foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
            {
                if (hole.Guid != type) continue;

                holes.Add(hole);
            }

            if (holes.Count == 0) return null;

            return RandomUtil.GetRandomObjectFromList(holes);
        }

        protected static bool AcquireOccupation(CareerManager ths, AcquireOccupationParameters occupationParameters, bool prompt)
        {
            CareerLocation location = occupationParameters.Location;
            OccupationNames newJobGuid = occupationParameters.TargetJob;

            if ((ths.mJob != null) && (location != null) && (ths.mJob.Guid == location.Career.Guid) && (ths.mJob.CareerLoc != location))
            {
                Career mJob = ths.mJob as Career;
                return ((mJob != null) && mJob.TransferBetweenCareerLocations(location, false));
            }

            Occupation occupation = null;
            if (!ths.TryGetNewCareer(newJobGuid, out occupation))
            {
                return false;
            }

            if (occupation is Career)
            {
                if (location == null) return false;
            }

            Sim createdSim = ths.mSimDescription.CreatedSim;

            if (ths.mJob != null)
            {
                if (prompt)
                {
                    string newJobName = string.Empty;
                    if (!occupation.TryDisplayingGetHiredUi(location, ref occupationParameters, out newJobName))
                    {
                        return false;
                    }

                    if (!Occupation.ShowYesNoCareerOptionDialog(Common.LocalizeEAString(ths.mSimDescription.IsFemale, "Gameplay/Careers/Career:ConfirmLeavingOldCareer", new object[] { ths.mSimDescription, occupation.CareerName, ths.mJob.CareerName })))
                    {
                        return false;
                    }
                }

                if (createdSim != null)
                {
                    EventTracker.SendEvent(new TransferCareerEvent(createdSim, ths.mJob, occupation));
                }
                ths.mJob.LeaveJob(false, Career.LeaveJobReason.kTransfered);
            }

            if (newJobGuid == OccupationNames.AcademicCareer)
            {
                AcademicCareer.EnrollSimInAcademicCareer(ths.mSimDescription, ths.DegreeManager.EnrollmentAcademicDegreeName, ths.DegreeManager.EnrollmentCouseLoad);
            }
            else
            {
                EventTracker.SendEvent(EventTypeId.kCareerNewJob, createdSim);
                occupation.OwnerDescription = ths.mSimDescription;
                occupation.mDateHired = SimClock.CurrentTime();
                occupation.mAgeWhenJobFirstStarted = ths.mSimDescription.Age;

                occupation.SetAttributesForNewJob(location, occupationParameters.LotId, occupationParameters.CharacterImportRequest);

                EventTracker.SendEvent(new CareerEvent(EventTypeId.kEventCareerHired, occupation));
                EventTracker.SendEvent(new CareerEvent(EventTypeId.kEventCareerChanged, occupation));
                EventTracker.SendEvent(new CareerEvent(EventTypeId.kCareerDataChanged, occupation));
            }

            occupation.RefreshMapTagForOccupation();
            ths.UpdateCareerUI();

            if ((createdSim != null) && createdSim.IsActiveSim)
            {
                HudController.SetInfoState(InfoState.Career);
            }

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<Item> allOptions = GetOptions(me);

                Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(false);

                foreach (Item option in allOptions)
                {
                    foreach (SimDescription sim in sims.Values)
                    {
                        if (option.Test(sim, false, me))
                        {
                            option.IncCount();
                        }
                    }
                }

                Item choice = new CommonSelection<Item>(Name, me.FullName, allOptions, Auxillary).SelectSingle();
                if (choice == null) return false;

                mChoice = choice;
            }

            mChoice.Perform(me, ApplyAll);
            return true;
        }

        protected virtual ObjectPickerDialogEx.CommonHeaderInfo<Item> Auxillary
        {
            get { return null; }
        }

        protected static void GetLocations(Career career, List<Item> items)
        {
            foreach (CareerLocation location in career.Locations)
            {
                items.Add(new CareerItem(career, location));
            }
        }

        public abstract class Item : SelectionOption
        {
            public Item()
            { }

            public override string DisplayValue
            {
                get { return null; }
            }

            public abstract bool Perform(SimDescription me, bool applyAll);

            public override int ValueWidth
            {
                get
                {
                    return 25;
                }
            }
        }

        public class CareerItem : Item
        {
            public readonly Occupation mCareer;

            public readonly CareerLocation mLocation;

            public CareerItem(Occupation career, CareerLocation location)
            {
                mCareer = career;
                mLocation = location;
            }

            public override string GetTitlePrefix()
            {
                return null;
            }

            public override string Name
            {
                get
                {
                    string name = mCareer.CareerName;
                    if ((mLocation != null) && (mLocation.Owner != null) && (mLocation.Owner.LotCurrent != null))
                    {
                        string lotName = mLocation.Owner.LotCurrent.Name;
                        if (string.IsNullOrEmpty(lotName))
                        {
                            lotName = mLocation.Owner.CatalogName;
                        }

                        name += " - " + lotName;
                    }

                    return name;
                }
            }

            protected override bool Allow(SimDescription me, IMiniSimDescription actor)
            {
                if (me.Occupation == null) return false;

                if (me.Occupation.Guid != mCareer.Guid) return false;

                Career rabbitHoleCareer = mCareer as Career;
                if (rabbitHoleCareer != null)
                {
                    CareerLocation testLoc = Career.FindClosestCareerLocation(me, mCareer.Guid);
                    if (testLoc == null) return false;
                }

                return true;
            }

            public override bool Perform(SimDescription me, bool applyAll)
            {
                Occupation job = me.Occupation;
                Occupation retiredJob = me.CareerManager.mRetiredCareer;

                me.CareerManager.mRetiredCareer = null;

                Occupation staticCareer = CareerManager.GetStaticOccupation(mCareer.Guid);
                if (staticCareer is School)
                {
                    me.CareerManager.mJob = null;
                }

                int originaHighest = 0;

                try
                {
                    if (mLocation != null)
                    {
                        AcquireOccupationParameters parameters = new AcquireOccupationParameters(mLocation, false, false);
                        parameters.ForceAutomaticChange = applyAll;

                        me.CareerManager.AcquireOccupation(parameters);
                    }
                    else
                    {
                        if (mCareer.Guid == OccupationNames.Firefighter)
                        {
                            ActiveCareerStaticData activeCareerStaticData = ActiveCareer.GetActiveCareerStaticData(OccupationNames.Firefighter);

                            originaHighest = activeCareerStaticData.HighestLevel;

                            // Required to bypass auto promotion in SetAttributesForNewJob
                            activeCareerStaticData.HighestLevel = 1;
                        }

                        AcquireOccupation(me.CareerManager, new AcquireOccupationParameters(mCareer.Guid, false, false), !applyAll);
                    }
                }
                finally
                {
                    me.CareerManager.mRetiredCareer = retiredJob;

                    if (me.CareerManager.mJob == null)
                    {
                        me.CareerManager.mJob = job;
                    }

                    if (mCareer.Guid == OccupationNames.Firefighter)
                    {
                        ActiveCareerStaticData activeCareerStaticData = ActiveCareer.GetActiveCareerStaticData(OccupationNames.Firefighter);
                        activeCareerStaticData.HighestLevel = originaHighest;
                    }
                }

                return true;
            }
        }
    }
}
