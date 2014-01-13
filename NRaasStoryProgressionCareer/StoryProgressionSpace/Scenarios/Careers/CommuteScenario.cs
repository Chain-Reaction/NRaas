using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Scenarios.Situations;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class CommuteScenario : CareerAlarmScenario
    {
        bool mPush = true;

        public CommuteScenario(SimDescription sim, bool push)
            : base (sim)
        {
            mPush = push;
        }
        protected CommuteScenario(CommuteScenario scenario)
            : base (scenario)
        {
            mPush = scenario.mPush;
        }

        protected bool Pushing
        {
            get { return mPush; }
        }

        protected override float GetTime()
        {
            Career job = Job;

            DateAndTime nowTime = SimClock.CurrentTime();

            AddStat("StartTime", job.CurLevel.StartTime);
            AddStat("NowTime", nowTime.Hour);
            AddStat("AvgTime", job.AverageTimeToReachWork);

            float time = ((job.CurLevel.StartTime - nowTime.Hour) + 24f) % 24f;
            time -= job.AverageTimeToReachWork;
            if ((time < 0f) || (time > 2f))
            {
                time = 0f;
            }

            return time;
        }

        protected override bool PrivateUpdate(ScenarioFrame frame)
        {
            base.PrivateUpdate(frame);

            if (mPush)
            {
                Sim createdSim = Job.OwnerDescription.CreatedSim;
                if (createdSim != null)
                {
                    if ((SimTypes.IsSelectable(Job.OwnerDescription)) || (!NpcParty.IsHostAtNpcParty(createdSim)))
                    {
                        foreach(InteractionInstance instance in createdSim.InteractionQueue.InteractionList)
                        {
                            if (instance is ICountsAsWorking)
                            {
                                IncStat("Already Queued");
                                return false;
                            }
                        }

                        VisitSituation.AnnounceTimeToGoToWork(createdSim);
                        createdSim.InteractionQueue.Add(CareerPushScenario.GetWorkInteraction(Job));
                    }
                }

                if (GetValue<AllowGoHomePushOption, bool>(Sim))
                {
                    Manager.AddAlarm(new GoHomePushScenario(Sim));
                }
            }
            return true;
        }
    }
}
