using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.SimDataElement
{
    public class StoredNetWorthSimData : OnDemandSimData
    {
        private Household mHouse = null;

        private Lot mLot = null;

        private int mNetWorth = 0;

        private int mLotHomeCost = 0;

        public StoredNetWorthSimData()
        { }

        public int NetWorth
        {
            get { return mNetWorth; }
        }

        public Household Household
        {
            get { return mHouse; }
        }

        public override bool Delayed
        {
            get { return false; }
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder(base.ToString());

            if (mHouse != null)
            {
                text.AddXML("House", mHouse.Name);
            }
            else
            {
                text.AddXML("House", "Null");
            }

            text.AddXML("NetWorth", mNetWorth);

            return text.ToString();
        }

        public override void Reset()
        {
            base.Reset();

            if (SimTypes.IsDead(Sim)) return;

            mHouse = Sim.Household;

            mNetWorth = 0;
            if (mHouse != null)
            {
                if (mHouse.LotHome != null)
                {
                    if (mHouse.LotHome != mLot)
                    {
                        mLot = mHouse.LotHome;

                        mLotHomeCost = StoryProgression.Main.Lots.GetLotCost(mLot);
                    }

                    mNetWorth = mHouse.FamilyFunds + mLotHomeCost;
                }
                else
                {
                    mNetWorth = mHouse.NetWorth();

                    mLot = null;
                    mLotHomeCost = 0;
                }
            }
        }
    }
}

