using NRaas.CareerSpace.Interfaces;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using System;

namespace NRaas.CareerSpace.Careers
{
    [Persistable]
    public class SchoolHighEx : SchoolHigh, ICoworkerPool, ILotDesignator
    {
        [Persistable(false)]
        protected NonPersistableData mOther = new NonPersistableData();

        public SchoolHighEx()
        { }
        public SchoolHighEx(XmlDbRow myRow, XmlDbTable levelTable, XmlDbTable eventDataTable)
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
            return sim.Teen;
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
                SchoolHighEx staticCareer = CareerManager.GetStaticCareer(Guid) as SchoolHighEx;
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

        [Persistable(false)]
        public class NonPersistableData
        {
            public OmniCareer.OutfitData mOutfits;

            public string mCoworkerPool = null;

            public bool mPaySims = false;

            public string mLotDesignator = null;

            public void Parse(XmlDbRow myRow, XmlDbTable levelTable)
            {
                if ((levelTable != null) && (levelTable.Rows.Count > 0))
                {
                    mOutfits = new OmniCareer.OutfitData(levelTable.Rows[0]);
                }
                else
                {
                    mOutfits = new OmniCareer.OutfitData();
                }

                mCoworkerPool = myRow.GetString("CoworkerPool");

                if (myRow.Exists("PaySims"))
                {
                    mPaySims = myRow.GetBool("PaySims");
                }

                if (myRow.Exists("LotDesignator"))
                {
                    mLotDesignator = myRow.GetString("LotDesignator");
                }
            }
        }
    }
}
