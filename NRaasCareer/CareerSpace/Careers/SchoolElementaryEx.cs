using NRaas.CommonSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Careers
{
    [Persistable]
    public class SchoolElementaryEx : SchoolElementary, ICoworkerPool, ILotDesignator
    {
        [Persistable(false)]
        protected SchoolHighEx.NonPersistableData mOther = new SchoolHighEx.NonPersistableData();

        public SchoolElementaryEx()
        { }
        public SchoolElementaryEx(XmlDbRow myRow, XmlDbTable levelTable, XmlDbTable eventDataTable)
            : base(myRow, levelTable, eventDataTable)
        {
            try
            {
                mOther.Parse(myRow, levelTable);
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }

        public string LotDesignator
        {
            get { return mOther.mLotDesignator; }
        }

        public string CoworkerPool
        {
            get { return mOther.mCoworkerPool; }
        }

        public override void SetCoworkersAndBoss()
        {
            CheckCoworkers();
        }

        public override bool CareerAgeTest(SimDescription sim)
        {
            return sim.Child;
        }

        public override bool AddCoworker()
        {
            try
            {
                return OmniCareer.AddCoworker(this);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
                return false;
            }
        }

        public override bool OnLoadFixup(bool isQuitCareer)
        {
            try
            {
                SchoolElementaryEx staticCareer = CareerManager.GetStaticCareer(Guid) as SchoolElementaryEx;
                if (staticCareer != null)
                {
                    mOther = staticCareer.mOther;
                }

                return base.OnLoadFixup(isQuitCareer);
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
                return false;
            }
        }

        public override void FinishWorking()
        {
            try
            {
                if (mOther.mPaySims)
                {
                    float dayLength = SimClock.ElapsedTime(TimeUnit.Hours, mTimeStartedWork);
                    mTimeStartedWork.Ticks = 0x0L;
                    if (dayLength >= 24f)
                    {
                        dayLength = DayLength;
                    }

                    int amount = (int)(PayPerHourOrStipend * dayLength);

                    PayOwnerSim(amount, GotPaidEvent.PayType.kCareerNormalPay);
                }

                base.FinishWorking();
            }
            catch (Exception e)
            {
                Common.Exception(OwnerDescription, e);
            }
        }

        public override void AddAnyUniformsForCurrentLevelToWardrobe()
        {
            mOther.mOutfits.AddAnyUniformsForCurrentLevelToWardrobe(this, base.AddAnyUniformsForCurrentLevelToWardrobe);
        }

        public override ResourceKey GetWorkOutfitForToday(CASAgeGenderFlags age, CASAgeGenderFlags gender)
        {
            return mOther.mOutfits.GetWorkOutfitForToday(age, gender, base.GetWorkOutfitForToday);
        }

        public override bool TryGetUniformForCurrentLevel(CASAgeGenderFlags age, CASAgeGenderFlags gender, out SimOutfit uniform)
        {
            return mOther.mOutfits.TryGetUniformForCurrentLevel(age, gender, out uniform, base.TryGetUniformForCurrentLevel);
        }
    }
}
