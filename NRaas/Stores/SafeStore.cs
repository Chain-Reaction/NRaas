using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Roles;
using Sims3.SimIFace;
using System;

namespace NRaas.CommonSpace.Stores
{
    public class SafeStore : IDisposable
    {
        public enum Flag : uint
        {
            None = 0x00,
            LoadFixup = 0x01,
            Selectable = 0x02,
            Unselectable = 0x04,
            StoreOpportunities = 0x08,
            School = 0x10,
            OnlyAcademic = 0x20,
        }

        public readonly SimDescription mSim;

        Role mRole;

        Occupation mCareer;
        School mSchool;

        Flag mFlags;

        OpportunityStore mOpportunities = null;

        bool mNeverSelectable = false;

        public SafeStore(SimDescription sim, Flag flags)
        {
            mSim = sim;

            mNeverSelectable = mSim.IsNeverSelectable;
            mSim.IsNeverSelectable = false;

            mFlags = flags;

            if ((mFlags & Flag.StoreOpportunities) == Flag.StoreOpportunities)
            {
                mOpportunities = new OpportunityStore(mSim, false);
            }

            mRole = mSim.AssignedRole;

            if ((flags & Flag.OnlyAcademic) == Flag.OnlyAcademic)
            {
                if (!GameUtils.IsUniversityWorld())
                {
                    // Bypass for a removal during OnBecameSelectable()
                    mCareer = mSim.OccupationAsAcademicCareer;
                }
            }
            else
            {
                mCareer = mSim.Occupation;
            }

            if (mCareer != null)
            {
                mSim.CareerManager.mJob = null;
            }

            if ((mSim.CareerManager != null) && ((flags & Flag.School) == Flag.School))
            {
                mSchool = mSim.CareerManager.School;
                if (mSchool != null)
                {
                    mSim.CareerManager.mSchool = null;
                }
            }

            mSim.AssignedRole = null;
        }

        public void Dispose()
        {
            try
            {
                mSim.AssignedRole = mRole;

                mSim.IsNeverSelectable = mNeverSelectable;

                if (mSim.CelebrityManager != null)
                {
                    mSim.CelebrityManager.ScheduleOpportunityCallIfNecessary();
                }

                if (mOpportunities != null)
                {
                    mOpportunities.Dispose();
                }

                if ((mSchool != null) && (mSim.CareerManager != null))
                {
                    mSim.CareerManager.mSchool = mSchool;
                }

                if ((mCareer != null) && (mSim.CareerManager != null) && (mSim.Occupation == null))
                {
                    mSim.CareerManager.mJob = mCareer;

                    if (mSim.Occupation != null)
                    {
                        using (BaseWorldReversion reversion = new BaseWorldReversion())
                        {
                            if ((mFlags & Flag.LoadFixup) == Flag.LoadFixup)
                            {
                                mSim.Occupation.OnLoadFixup(false);
                            }

                            if (((mFlags & Flag.Selectable) == Flag.Selectable) && (SimTypes.IsSelectable(mSim)))
                            {
                                using (StoryProgressionServiceEx.SuppressCreateHousehold suppress = new StoryProgressionServiceEx.SuppressCreateHousehold())
                                {
                                    bool careerLoc = true;
                                    if (mSim.Occupation is Career)
                                    {
                                        careerLoc = (mSim.Occupation.CareerLoc != null);
                                    }

                                    if (careerLoc)
                                    {
                                        Corrections.FixCareer(mSim.Occupation, false, null);

                                        mSim.Occupation.OnOwnerBecameSelectable();
                                    }
                                }
                            }
                            else if ((mFlags & Flag.Unselectable) == Flag.Unselectable)
                            {
                                using (StoryProgressionServiceEx.SuppressCreateHousehold suppress = new StoryProgressionServiceEx.SuppressCreateHousehold())
                                {
                                    // fix for script error in GhostHunter:ApplyCareerSpecificModifiersToXp where
                                    // ownersim is not null checked and breaks when this is ran during age up.
                                    ActiveCareerLevelStaticData data = null;
                                    int experience = 0;
                                    GhostHunter hunter = null;
                                    try
                                    {
                                        hunter = mSim.Occupation as GhostHunter;
                                        if (hunter != null)
                                        {
                                            data = hunter.GetCurrentLevelStaticDataForActiveCareer();
                                            if (data != null)
                                            {
                                                experience = data.DailyPerfectJobBonusExperience;
                                                data.DailyPerfectJobBonusExperience = 0;
                                            }
                                        }

                                    mSim.Occupation.OnOwnerBecameUnselectable();
                                    }
                                    catch (Exception e)
                                    {
                                        Common.Exception(mSim, e);
                                    }
                                    finally
                                    {
                                        if (hunter != null && data != null)
                                        {
                                            data.DailyPerfectJobBonusExperience = experience;
                                        }
                                    }
                                }
                            }
                        }

                        mSim.CareerManager.UpdateCareerUI();
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(mSim, e);
            }
        }
    }
}