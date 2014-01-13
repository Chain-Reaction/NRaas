using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.PorterSpace
{
    public class HouseData
    {
        public int mID = 0;
        public int mFunds = 0;
        public string mName = null;

        public int mPreferenceMale = 0;
        public int mPreferenceFemale = 0;

        public List<SimDescription> mSims = new List<SimDescription>();

        public HouseData(int id, SimDescription sim)
        {
            mID = id;

            if (sim.Household != null)
            {
                if (sim.LotHome != null)
                {
                    mFunds = (sim.FamilyFunds + sim.LotHome.Cost);
                }
                else
                {
                    mFunds = sim.Household.NetWorth();
                }
                mName = sim.Household.Name;
            }

            mPreferenceMale = sim.mGenderPreferenceMale;
            mPreferenceFemale = sim.mGenderPreferenceFemale;

            mSims.Add(sim);
        }
        public HouseData(string desc)
        {
            try
            {
                List<string> data = new List<string>(desc.Split(','));

                if (data.Count > 0)
                {
                    int.TryParse(data[0], out mID);
                }

                if (data.Count > 1)
                {
                    int.TryParse(data[1], out mFunds);
                }

                if (data.Count > 2)
                {
                    mName = data[2];
                }

                if (data.Count > 3)
                {
                    int.TryParse(data[3], out mPreferenceMale);
                }

                if (data.Count > 4)
                {
                    int.TryParse(data[4], out mPreferenceFemale);
                }
            }
            catch (Exception e)
            {
                Common.Exception(desc, e);
            }
        }

        public void Reconcile(SimDescription sim)
        {
            sim.mGenderPreferenceMale = mPreferenceMale;
            sim.mGenderPreferenceFemale = mPreferenceFemale;
        }

        public static int SortByCost(HouseData a, HouseData b)
        {
            if (a.mFunds > b.mFunds)
            {
                return 1;
            }
            if (a.mFunds < b.mFunds)
            {
                return -1;
            }
            return 0;
        }

        public override string ToString()
        {
            return ("NRaas.Porter:" + mID + "," + mFunds + "," + mName + "," + mPreferenceMale + "," + mPreferenceFemale);
        }
    }
}
