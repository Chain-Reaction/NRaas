using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.PerformanceObjects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Helpers
{
    public class ShowStageEx
    {
        private static bool ValidPerformer(SimDescription simDesc)
        {
            if (simDesc.Occupation != null)
            {
                return (simDesc.OccupationAsPerformanceCareer != null);
            }

            if (!simDesc.IsHuman) return false;
            
            if (!simDesc.YoungAdultOrAbove) return false;

            if (simDesc.AssignedRole != null) return false;

            return true;
        }

        public static Sim CreateAnNPCPerformer(ShowStage ths)
        {
            Sim createdSim = null;
            Lot lotCurrent = null;
            OccupationNames[] randomList = new OccupationNames[] { OccupationNames.SingerCareer, OccupationNames.MagicianCareer, OccupationNames.PerformanceArtistCareer };
            AcquireOccupationParameters occupationParameters = new AcquireOccupationParameters(RandomUtil.GetRandomObjectFromList(randomList), false, true);
            bool flag = RandomUtil.CoinFlip();

            // Custom
            List<SimDescription> list = Household.AllTownieSimDescriptions(ValidPerformer);
            if (list.Count == 0x0)
            {
                return null;
            }

            SimDescription randomObjectFromList = RandomUtil.GetRandomObjectFromList(list);
            if (randomObjectFromList.CreatedSim == null)
            {
                lotCurrent = ths.LotCurrent;
                createdSim = randomObjectFromList.Instantiate(lotCurrent);
            }
            else
            {
                createdSim = randomObjectFromList.CreatedSim;
            }

            if (createdSim.CareerManager.OccupationAsPerformanceCareer == null)
            {
                try
                {
                    createdSim.AcquireOccupation(occupationParameters);

                    int num = flag ? RandomUtil.GetInt(0x0, 0x5) : RandomUtil.GetInt(0x5, createdSim.Occupation.HighestLevel);
                    for (int i = 0x1; i < num; i++)
                    {
                        createdSim.CareerManager.Occupation.PromoteSim();
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Common.Exception(createdSim, e);
                    return null;
                }
            }
            return createdSim;
        }

        public static void SetupSimFest(ShowStage ths)
        {
            Common.StringBuilder msg = new Common.StringBuilder("SetupSimFest");

            try
            {
                if ((ths.OwnerProprietor == null) || (ths.OwnerProprietor.HasBeenDestroyed))
                {
                    if (ths.CurrentRole == null) return;

                    ths.OwnerProprietor = ths.CurrentRole.SimInRole;
                    if ((ths.OwnerProprietor == null) || (ths.OwnerProprietor.HasBeenDestroyed))
                    {
                        return;
                    }
                }

                ths.CleanupSimFest();

                CommonSpace.Helpers.ShowStageEx.Cleanup(ths, null);

                msg += "C";

                List<Sim> list = new List<Sim>();
                foreach (Sim sim in LotManager.Actors)
                {
                    if (ths.BasicNPCPerformerTest(sim))
                    {
                        list.Add(sim);
                    }
                }

                if (list.Count > 0)
                {
                    ths.LoadStage();
                    PerformanceCareer.PerformanceAudiencePosture standing = PerformanceCareer.PerformanceAudiencePosture.Standing;
                    ths.PreShowSetup();
                    foreach (IShowFloor floor in ths.LotCurrent.GetObjects<IShowFloor>())
                    {
                        floor.ResetShowFloor();
                        switch (standing)
                        {
                            case PerformanceCareer.PerformanceAudiencePosture.Standing:
                                floor.SetupShowFloorWithStandingLocations();
                                break;

                            case PerformanceCareer.PerformanceAudiencePosture.Sitting:
                                floor.SetupShowFloorWithSittingLocations();
                                break;
                        }
                    }

                    msg += "A";

                    foreach (ICrowdMonster monster in ths.LotCurrent.GetObjects<ICrowdMonster>())
                    {
                        switch (standing)
                        {
                            case PerformanceCareer.PerformanceAudiencePosture.Standing:
                                monster.CreateStandingCrowdMonster();
                                break;

                            case PerformanceCareer.PerformanceAudiencePosture.Sitting:
                                monster.CreateSittingCrowdMonster();
                                break;
                        }
                    }

                    msg += "B";

                    ths.mDoesPerformanceAllowDancing = false;
                    ths.Controller.PostEvent(ShowEventType.kShowSetup);
                    string titleText = ShowStage.LocalizeString(ths.OwnerProprietor.IsFemale, "AnnounceSimFestStart", new object[] { ths.LotCurrent.Name });
                    StyledNotification.Format format = new StyledNotification.Format(titleText, ths.OwnerProprietor.ObjectId, StyledNotification.NotificationStyle.kSystemMessage);
                    StyledNotification.Show(format);
                    ths.AddSimFestMapTags();

                    msg += "D";

                    for (int i = 0x0; i < 0x2; i++)
                    {
                        // Custom
                        Sim item = CreateAnNPCPerformer(ths);
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }

                    msg += "E";

                    foreach (Sim sim3 in list)
                    {
                        if (sim3 != null)
                        {
                            ths.PushWatchTheShowOntoSim(sim3);
                        }
                    }

                    msg += "F";

                    int audienceSize = ths.GetSimFestDesiredAudienceSize() - list.Count;
                    if (audienceSize > 0x0)
                    {
                        ths.PullAudienceToLot(audienceSize);
                    }

                    msg += "G";

                    ths.mSimFestStartAlarm = ths.AddAlarmRepeating(10f, TimeUnit.Minutes, ths.SimFestWaitForPerformers, "SimFestWaitForPerformers", AlarmType.AlwaysPersisted);
                    ShowStage.SimFestWait entry = ShowStage.SimFestWait.Singleton.CreateInstance(ths, ths.OwnerProprietor, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), ths.OwnerProprietor.IsNPC, false) as ShowStage.SimFestWait;

                    msg += "H";

                    entry.mAction = ShowStage.SimFestWait.SimFestWaitAction.WaitForPerformers;
                    ths.OwnerProprietor.InteractionQueue.AddNext(entry);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(msg, e);
            }
        }
    }
}