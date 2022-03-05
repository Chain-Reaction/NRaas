using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Helpers
{
    public class AnnexEx
    {
        public static bool AddSimToGraduationList(Annex ths, SimDescription simDesc, AcademicDegree degree)
        {
            if ((simDesc == null) || (degree == null))
            {
                return false;
            }

            if (IsAGraduationCeremonyInitializedAndIfNecessaryStartOne(ths))
            {
                ths.mGraduatingSims.Add(simDesc, degree);
                if (simDesc.CreatedSim != null)
                {
                    ActiveTopic.AddToSim(simDesc.CreatedSim, "University Graduation");
                }
            }

            return ths.mGraduationCeremonyInitialized;
        }

        private static bool IsAGraduationCeremonyInitializedAndIfNecessaryStartOne(Annex ths)
        {
            if (!ths.mGraduationCeremonyInitialized)
            {
                ths.CleanUpGraduationCeremony();
                float time = SimClock.HoursUntil(Annex.kGraduationTuning.kHourToShowGraduationMessage);
                ths.mDidIGraduationMessageAlarm = ths.AddAlarm(time, TimeUnit.Hours, ths.ShowIGraduationMessagesCallback, "Annex: I Graduated Message Alarm", AlarmType.AlwaysPersisted);

                float num2 = SimClock.HoursUntil(Annex.kGraduationTuning.kHourToShowGraduationInvitationMessage);
                ths.mGraduationInvitationAlarm = ths.AddAlarm(num2, TimeUnit.Hours, ths.ShowGraduationInvitationMessagesCallback, "Annex: Graduation Invitation Message Alarm", AlarmType.AlwaysPersisted);

                //float num3 = SimClock.HoursUntil(Annex.kGraduationTuning.kGraduationCeremonyStartHour);
                //ths.mStartGraduationCeremonyAlarm = ths.AddAlarm(num3, TimeUnit.Hours, ths.StartGraduationCeremonyCallback, "Annex: Graduation Ceremony Start Alarm", AlarmType.AlwaysPersisted);
                new StartGraduationCeremonyTask(ths);

                ths.mGraduationCeremonyInitialized = true;
            }

            return ths.mGraduationCeremonyInitialized;
        }

        public static void OnWorldLoadFinished()
        {
            Annex annex = CollegeGraduation.GetGraduationRabbitHole() as Annex;
            if (annex != null)
            {
                if (annex.mGraduationCeremonyInitialized)
                {
                    if (annex.mStartGraduationCeremonyAlarm == AlarmHandle.kInvalidHandle)
                    {
                        new StartGraduationCeremonyTask(annex);
                    }
                }
            }
        }

        public class StartGraduationCeremonyTask : Common.AlarmTask
        {
            Annex mThs;

            public StartGraduationCeremonyTask(Annex ths)
                : base(SimClock.HoursUntil(Annex.kGraduationTuning.kGraduationCeremonyStartHour), TimeUnit.Hours)
            {
                mThs = ths;
            }

            protected override void OnPerform()
            {
                Common.DebugNotify("StartGraduationCeremonyTask:OnPerform");

                if (mThs.IsAGraduationCeremonyInProgressAndIfNecessaryStartOne())
                {
                    foreach (SimDescription sim in new List<SimDescription>(mThs.mGraduatingSims.Keys))
                    {
                        try
                        {
                            Instantiation.PerformOffLot(sim, mThs.RabbitHoleProxy.LotCurrent, null);

                            if ((sim.CreatedSim != null) && (sim.CreatedSim.InteractionQueue != null))
                            {
                                sim.CreatedSim.ShowTNSIfSelectable(TNSNames.CollegeGraduationStarting, null, sim.CreatedSim, new object[] { sim.CreatedSim });
                                Annex.UniversityGraduationCeremony entry = Annex.UniversityGraduationCeremony.Singleton.CreateInstance(mThs, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.High), false, true) as Annex.UniversityGraduationCeremony;
                                sim.CreatedSim.InteractionQueue.AddNext(entry);
                                ActiveTopic.RemoveTopicFromSim(sim.CreatedSim, "University Graduation");
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, e);
                        }
                    }

                    Dictionary<ulong, SimDescription> loadedSims = SimListing.GetResidents(false);

                    foreach (MiniSimDescription sim in new List<MiniSimDescription>(mThs.mForeignVisitors))
                    {
                        try
                        {
                            // Custom
                            //   EA Standard attempts to reimport existing sims without checking
                            SimDescription local;
                            if (loadedSims.TryGetValue(sim.SimDescriptionId, out local))
                            {
                                if (!mThs.mLocalVisitors.Contains(local))
                                {
                                    mThs.mLocalVisitors.Add(local);
                                }
                            }
                            else
                            {
                                SimDescription simDescription = MiniSims.UnpackSimAndUpdateRel(sim);
                                if (simDescription != null)
                                {
                                    Household.CreateTouristHousehold();
                                    Household.TouristHousehold.AddTemporary(simDescription);
                                    sim.Instantiated = true;
                                    (Responder.Instance.HudModel as Sims3.Gameplay.UI.HudModel).OnSimCurrentWorldChanged(true, sim);

                                    if (simDescription.AgingState != null)
                                    {
                                        simDescription.AgingState.MergeTravelInformation(sim);
                                    }

                                    mThs.mLocalVisitors.Add(simDescription);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, e);
                        }
                    }

                    Dictionary<SimDescription, bool> performed = new Dictionary<SimDescription, bool>();

                    foreach (SimDescription sim in new List<SimDescription>(mThs.mLocalVisitors))
                    {
                        try
                        {
                            if (mThs.mGraduatingSims.ContainsKey(sim)) continue;

                            if (performed.ContainsKey(sim)) continue;
                            performed[sim] = true;

                            Instantiation.PerformOffLot(sim, mThs.RabbitHoleProxy.LotCurrent, null);

                            if ((sim.CreatedSim != null) && (sim.CreatedSim.InteractionQueue != null))
                            {
                                InteractionInstance instance = Annex.AttendGraduationCeremony.Singleton.CreateInstance(mThs, sim.CreatedSim, new InteractionPriority(InteractionPriorityLevel.High), true, true);
                                sim.CreatedSim.InteractionQueue.AddNext(instance);
                            }
                        }
                        catch (Exception e)
                        {
                            Common.Exception(sim, e);
                        }
                    }
                }
            }
        }
    }
}