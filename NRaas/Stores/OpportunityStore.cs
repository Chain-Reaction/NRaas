using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace NRaas.CommonSpace.Stores
{
    public class OpportunityStore : IDisposable
    {
        SimDescription mSim;

        OpportunityHistory.OpportunityExportInfo[] mOpportunities;

        public OpportunityStore(SimDescription sim, bool storeForTravel)
        {
            mSim = sim;

            if ((storeForTravel) &&
                (!mSim.NeedsOpportunityImport) &&
                (mSim.CreatedSim != null) &&
                (mSim.CreatedSim.OpportunityManager != null))
            {
                mSim.CreatedSim.OpportunityManager.StoreOpportunitiesForTravel();
            }

            if (mSim.NeedsOpportunityImport)
            {
                mOpportunities = sim.OpportunityHistory.mCurrentOpportunities;
                mSim.OpportunityHistory.mCurrentOpportunities = new OpportunityHistory.OpportunityExportInfo[9];
                mSim.NeedsOpportunityImport = false;
            }
        }

        public void Dispose()
        {
            if ((mOpportunities != null) && (mSim.OpportunityHistory != null))
            {
                mSim.OpportunityHistory.mCurrentOpportunities = mOpportunities;
                mSim.NeedsOpportunityImport = true;

                if (mSim.CreatedSim != null)
                {
                    if (mSim.CreatedSim.mOpportunityManager == null)
                    {
                        mSim.CreatedSim.mOpportunityManager = new OpportunityManager(mSim.CreatedSim);
                        mSim.CreatedSim.mOpportunityManager.SetupLocationBasedOpportunities();
                    }

                    try
                    {
                        // Due to an odd bit of coding at the bottom of AcceptOpportunityFromTravel(), 
                        //   the expiration time for non-expirying opportunities is checked
                        foreach (OpportunityHistory.OpportunityExportInfo info in mSim.OpportunityHistory.GetCurrentOpportunities())
                        {
                            if (info.ExpirationTime < SimClock.CurrentTime())
                            {
                                Opportunity opp = OpportunityManager.GetStaticOpportunity(info.Guid);
                                if (opp != null)
                                {
                                    bool requiresTimeout = false;

                                    switch (opp.Timeout)
                                    {
                                        case Opportunity.OpportunitySharedData.TimeoutCondition.SimDays:
                                        case Opportunity.OpportunitySharedData.TimeoutCondition.SimHours:
                                        case Opportunity.OpportunitySharedData.TimeoutCondition.SimTime:
                                        case Opportunity.OpportunitySharedData.TimeoutCondition.Gig:
                                        case Opportunity.OpportunitySharedData.TimeoutCondition.AfterschoolRecitalOrAudition:
                                            requiresTimeout = true;
                                            break;
                                    }

                                    if (!requiresTimeout)
                                    {
                                        info.ExpirationTime = SimClock.Add(SimClock.CurrentTime(), TimeUnit.Hours, 1);
                                    }
                                }
                            }
                        }

                        mSim.CreatedSim.OpportunityManager.TravelFixup();
                    }
                    catch (Exception e)
                    {
                        Common.Exception(mSim, e);
                    }

                    mSim.NeedsOpportunityImport = false;
                }
            }
        }
    }
}