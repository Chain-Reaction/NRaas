using NRaas.StoryProgressionSpace.Managers;
using NRaas.CommonSpace.ScoringMethods;
using Sims3.Gameplay.Careers;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.SimDataElement
{
    public class CareerSimData : OnDemandSimData
    {
        private OccupationNames mCareer = OccupationNames.Undefined;

        private int mCareerLevel = 0;

        private bool mRetire = false;

        public CareerSimData()
        { }

        public bool CareerLevelUnchanged
        {
            get
            {
                if ((Sim == null) || (Sim.Occupation == null))
                {
                    return (mCareerLevel == 0);
                }
                else
                {
                    return (mCareerLevel == Sim.Occupation.CareerLevel);
                }
            }
        }

        public override bool Delayed
        {
            get { return false; }
        }

        public override string ToString()
        {
            Common.StringBuilder text = new Common.StringBuilder(base.ToString());
            text.AddXML("Career", mCareer);
            text.AddXML("Level", mCareerLevel);
            text.AddXML("Retire", mRetire);
            return text.ToString();
        }

        public bool IsCareer(OccupationNames job)
        {
            if (Sim.Occupation == null) return false;

            return (Sim.Occupation.Guid == job);
        }

        public void UpdateCareer()
        {
            if (Sim.Occupation != null)
            {
                mCareer = Sim.Occupation.Guid;
                mCareerLevel = Sim.Occupation.CareerLevel;
            }
            else
            {
                mCareer = OccupationNames.Undefined;
                mCareerLevel = 0;
            }
        }

        public void Retire()
        {
            if (Sim.IsHuman)
            {
                mRetire = true;
            }
        }

        public override void Reset()
        {
            base.Reset();

            UpdateCareer();

            if (Sim.Occupation == null)
            {
                if ((Sim.Elder) && (Sim.CareerManager != null) && (Sim.CareerManager.RetiredCareer != null))
                {
                    if (ManagerCareer.RetiredLocation != null)
                    {
                        mRetire = true;
                    }
                }
            }

            if (mRetire)
            {
                if ((ManagerCareer.RetiredLocation != null) && (!(Sim.CareerManager.Occupation is Careers.Retired)))
                {
                    Occupation retiredCareer = Sim.CareerManager.mRetiredCareer;
                    Sim.CareerManager.mRetiredCareer = null;

                    try
                    {
                        Sim.AcquireOccupation(new AcquireOccupationParameters(ManagerCareer.RetiredLocation, false, false));
                    }
                    catch (Exception e)
                    {
                        Common.DebugException(Sim, e);
                    }
                    finally
                    {
                        Sim.CareerManager.mRetiredCareer = retiredCareer;
                    }
                }
                mRetire = false;
            }
        }
    }
}

