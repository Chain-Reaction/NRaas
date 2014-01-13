using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.CAS;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.SimDataElement
{
    public abstract class OnDemandSimData : ElementalSimData
    {
        SimDescription mSim = null;

        bool mUpdate = false;

        public OnDemandSimData()
        { }

        protected SimDescription Sim
        {
            get { return mSim; }
        }

        public abstract bool Delayed
        {
            get;
        }

        protected bool NeedsUpdate()
        {
            return mUpdate;
        }

        protected void SetToUpdated()
        {
            mUpdate = false;
        }

        public override void Init(ManagerProgressionBase manager, SimDescription sim)
        {
            if (sim == null) return;

            base.Init(manager, sim);

            mSim = sim;

            Reset();
        }

        public virtual void Reset()
        {
            mUpdate = true;
        }
    }
}

