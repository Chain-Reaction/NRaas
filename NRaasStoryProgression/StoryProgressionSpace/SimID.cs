using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace
{
    [Persistable]
    public class SimID
    {
        private ulong mID = 0;

        public SimID()
        { }
        public SimID(ulong sim)
        {
            mID = sim;
        }
        public SimID(SimDescription sim)
        {
            if (sim != null)
            {
                mID = sim.SimDescriptionId;
            }
        }
        public SimID(Sim sim)
        {
            if ((sim != null) && (sim.SimDescription != null))
            {
                mID = sim.SimDescription.SimDescriptionId;
            }
        }

        public static bool Matches(SimID id, SimDescription sim)
        {
            if (sim == null)
            {
                if (id == null) return true;

                return (id.mID == 0);
            }
            else
            {
                return Matches(id, sim.SimDescriptionId);
            }
        }
        public static bool Matches(SimID id, ulong sim)
        {
            if (id == null) return false;

            return (id.mID == sim);
        }

        public SimDescription SimDescription
        {
            get
            {
                return ManagerSim.Find(mID);
            }
        }

        public override bool Equals(object obj)
        {
            SimID sim = obj as SimID;
            if (sim == null) return false;

            return (mID == sim.mID);
        }

        public override int GetHashCode ()
        {
            return (int)mID;
        }

        public override string ToString()
        {
            return mID.ToString();
        }
    }
}
