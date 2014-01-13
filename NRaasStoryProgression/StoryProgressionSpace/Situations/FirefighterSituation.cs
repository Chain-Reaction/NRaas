using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Situations
{
    public class FirefighterSituation : SituationBase
    {
        private int mFailedAttemptsToReachBurningObject;

        // Methods
        protected FirefighterSituation()
        { }
        public FirefighterSituation(Lot lot, Sim worker)
            : base(lot, worker)
        {
            Sims3.Gameplay.Services.FirefighterSituation.SetFirefighterSituationInitialParameters(worker, lot);

            worker.GreetSimOnLot(lot);

            SetState(new RouteToLot<FirefighterSituation, StartBusiness>(this));
        }

        public override void CleanUp()
        {
            try
            {
                Worker.SimDescription.ShowSocialsOnSim = true;

                Firefighter.FirefighterInformation information;
                if ((Lot != null) && Firefighter.sFirefighterDictionary.TryGetValue(Lot.LotId, out information))
                {
                    information.FirefightersActiveOnLot.Remove(Worker);
                }

                foreach(Sim sim in Lot.GetObjects<Sim>(new Predicate<Sim>(Sims3.Gameplay.Services.FirefighterSituation.IsSimOnFire)))
                {
                    sim.BuffManager.RemoveElement(BuffNames.OnFire);
                    sim.BuffManager.RemoveElement(BuffNames.Torched);
                }

                Lot.FireManager.RemoveAllFires();

                foreach (Sim sim in Lot.GetObjects<Sim>())
                {
                    if (sim.Occupation is ActiveFireFighter) continue;

                    if (sim.Service is Firefighter) continue;

                    Occupation.SimCompletedTask(Worker, TaskId.TrappedSims, sim);
                }

                base.CleanUp();
            }
            catch(Exception e)
            {
                Common.Exception(Lot, e);
            }
        }

        public class StartBusiness : ChildSituation<FirefighterSituation>
        {
            // Methods
            private StartBusiness()
            { }
            public StartBusiness(FirefighterSituation parent)
                : base(parent)
            { }

            public override void Init(FirefighterSituation parent)
            {
                if ((parent.Lot == null) || (parent.Worker == null))
                {
                    Exit();
                }

                Firefighter.FirefighterInformation information = null;
                if (!Firefighter.sFirefighterDictionary.TryGetValue(parent.Lot.LotId, out information))
                {
                    Exit();
                }

                if (!parent.TryRestartFirefighting(information))
                {
                    Exit();
                }
            }
        }

        private void SetStateExtinguishSim(Sim burningSim)
        {
            SetState(new ExtinguishSim(this, burningSim));
        }

        private bool TryRestartFirefighting(Firefighter.FirefighterInformation firefighterInfo)
        {
            if (Sims3.Gameplay.Services.FirefighterSituation.DoesFireExistOnLot(Lot))
            {
                SetStateExtinguishSim(null);
                firefighterInfo.ValidFirefighterRequest = true;
                return true;
            }

            return false;
        }

        // Nested Types
        public class ExtinguishFire : ChildSituation<FirefighterSituation>
        {
            // Fields
            private Fire mFireToExtinguish;

            // Methods
            private ExtinguishFire()
            { }
            public ExtinguishFire(FirefighterSituation parent) 
                : base(parent)
            { }

            private void ExtinguishSuccessful(Sim actor, float x)
            {
                Occupation.SimCompletedTask(Parent.Worker, TaskId.IsolatedFires, null);

                Parent.SetState(new ExtinguishFire(Parent));
            }

            private void FailedToExtinguish(Sim actor, float x)
            {
                if (Simulator.CheckYieldingContext(false))
                {
                    Parent.SetState(new HangAroundThenExtinguish(Parent, mFireToExtinguish, true));
                }
            }

            public override void Init(FirefighterSituation parent)
            {
                if ((parent.Lot != null) && (parent.Worker != null))
                {
                    List<Fire> objects = parent.Lot.GetObjects<Fire>(new Predicate<Fire>(Sims3.Gameplay.Services.FirefighterSituation.FireNotBeingExtinguished));
                    if ((objects != null) && (objects.Count > 0))
                    {
                        mFireToExtinguish = RandomUtil.GetRandomObjectFromList<Fire>(objects);
                        ForceSituationSpecificInteraction(mFireToExtinguish, parent.Worker, Fire.Extinguish.Singleton, null, ExtinguishSuccessful, FailedToExtinguish, new InteractionPriority(InteractionPriorityLevel.Fire, 5f));
                    }
                    else
                    {
                        Fire[] fireArray = parent.Lot.GetObjects<Fire>();
                        if ((fireArray != null) && (fireArray.Length > 0))
                        {
                            Parent.SetState(new HangAroundThenExtinguish(Parent, fireArray[0], false));
                        }
                        else
                        {
                            List<Sim> list2 = parent.Lot.GetObjects<Sim>(new Predicate<Sim>(Sims3.Gameplay.Services.FirefighterSituation.IsSimOnFire));
                            if ((list2 != null) && (list2.Count > 0))
                            {
                                Parent.SetState(new HangAroundThenExtinguish(Parent, list2[0], false));
                            }
                            else
                            {
                                Parent.SetState(new HangAroundBeforeLeaving(Parent));
                            }
                        }
                    }
                }
            }
        }

        public class ExtinguishSim : ChildSituation<FirefighterSituation>
        {
            // Fields
            private Sim mSimToExtinguish;

            // Methods
            private ExtinguishSim()
            { }
            public ExtinguishSim(FirefighterSituation parent, Sim simToExtinguish)
                : base(parent)
            {
                mSimToExtinguish = simToExtinguish;
            }

            private void ExtinguishSuccessful(Sim actor, float x)
            {
                try
                {
                    Occupation.SimCompletedTask(Parent.Worker, TaskId.TrappedSims, mSimToExtinguish);

                    EventTracker.SendEvent(EventTypeId.kFirefighterSaveSims, Parent.Worker, mSimToExtinguish);

                    Firefighter.FirefighterInformation information = null;
                    if ((Lot != null) && Firefighter.sFirefighterDictionary.TryGetValue(Lot.LotId, out information))
                    {
                        information.SimsThatAreBeingExtinguished.Remove(mSimToExtinguish);
                    }
                    Parent.SetState(new ExtinguishSim(Parent, null));
                }
                catch (Exception e)
                {
                    Common.Exception(Parent.Worker, Parent.Lot, e);
                }
            }

            private void FailedToExtinguish(Sim actor, float x)
            {
                if (Simulator.CheckYieldingContext(false))
                {
                    Firefighter.FirefighterInformation information = null;
                    if (((Lot != null) && Firefighter.sFirefighterDictionary.TryGetValue(Lot.LotId, out information)) && ((mSimToExtinguish != null) && !mSimToExtinguish.HasBeenDestroyed))
                    {
                        information.SimsThatAreBeingExtinguished.Remove(mSimToExtinguish);
                        if (!information.SimsThatAreBurning.Contains(mSimToExtinguish))
                        {
                            information.SimsThatAreBurning.Add(mSimToExtinguish);
                        }
                    }
                    Parent.SetState(new HangAroundThenExtinguish(Parent, mSimToExtinguish, true));
                }
            }

            public override void Init(FirefighterSituation parent)
            {
                Firefighter.FirefighterInformation information = null;
                if ((parent.Worker != null) && Firefighter.sFirefighterDictionary.TryGetValue(parent.Lot.LotId, out information))
                {
                    RequestWalkStyle(parent.Worker, Sim.WalkStyle.FastRun);
                    parent.Worker.SimDescription.ShowSocialsOnSim = false;
                    if (mSimToExtinguish == null)
                    {
                        UpdateListOfBurningAndCurrentlyExtinguishedSims(parent.Lot);
                        if (information.SimsThatAreBurning.Count > 0x0)
                        {
                            Sim randomObjectFromList = RandomUtil.GetRandomObjectFromList<Sim>(information.SimsThatAreBurning);
                            if (randomObjectFromList != null)
                            {
                                information.SimsThatAreBurning.Remove(randomObjectFromList);
                                if (!information.SimsThatAreBeingExtinguished.Contains(randomObjectFromList))
                                {
                                    information.SimsThatAreBeingExtinguished.Add(randomObjectFromList);
                                }
                                mSimToExtinguish = randomObjectFromList;
                            }
                        }
                    }
                    if (mSimToExtinguish != null)
                    {
                        ForceSituationSpecificInteraction(mSimToExtinguish, parent.Worker, Sim.ExtinguishSim.Singleton, null, ExtinguishSuccessful, FailedToExtinguish, new InteractionPriority(InteractionPriorityLevel.Fire, 6f));
                    }
                    else
                    {
                        parent.SetState(new ExtinguishFire(Parent));
                    }
                }
            }

            private static void UpdateListOfBurningAndCurrentlyExtinguishedSims(Lot lot)
            {
                Firefighter.FirefighterInformation information = null;
                if ((lot != null) && Firefighter.sFirefighterDictionary.TryGetValue(lot.LotId, out information))
                {
                    List<Sim>.Enumerator enumerator = information.SimsThatAreBurning.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Sim current = enumerator.Current;
                        if ((current != null) && (current.HasBeenDestroyed || !Sims3.Gameplay.Services.FirefighterSituation.IsSimOnFireAndNotBeingHelped(current)))
                        {
                            information.SimsThatAreBurning.Remove(current);
                            enumerator = information.SimsThatAreBurning.GetEnumerator();
                        }
                    }
                    List<Sim>.Enumerator enumerator2 = information.SimsThatAreBeingExtinguished.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        if ((enumerator2.Current != null) && enumerator2.Current.HasBeenDestroyed)
                        {
                            information.SimsThatAreBeingExtinguished.Remove(enumerator2.Current);
                            enumerator2 = information.SimsThatAreBeingExtinguished.GetEnumerator();
                        }
                    }
                }
            }
        }

        public class HangAroundBeforeLeaving : ChildSituation<FirefighterSituation>
        {
            // Fields
            private AlarmHandle mAlarmHandle;

            // Methods
            private HangAroundBeforeLeaving()
            { }
            public HangAroundBeforeLeaving(FirefighterSituation parent) 
                : base(parent)
            { }

            public override void CleanUp()
            {
                AlarmManager.RemoveAlarm(mAlarmHandle);
                base.CleanUp();
            }

            public override void Init(FirefighterSituation parent)
            {
                Firefighter.FirefighterInformation information = null;
                if ((parent.Worker != null) && Firefighter.sFirefighterDictionary.TryGetValue(parent.Lot.LotId, out information))
                {
                    if (!parent.TryRestartFirefighting(information))
                    {
                        Exit();
                    }
                }
            }

            private void OnLeaving(Sim actor, float x)
            {
                if (Simulator.CheckYieldingContext(false))
                {
                    Firefighter.FirefighterInformation information = null;
                    if ((Parent.Worker != null) && Firefighter.sFirefighterDictionary.TryGetValue(Parent.Lot.LotId, out information))
                    {
                        if (!information.KnownUnreachableFiresLeftOnLot && Sims3.Gameplay.Services.FirefighterSituation.DoesFireExistOnLot(Parent.Lot))
                        {
                            Parent.SetStateExtinguishSim(null);
                            information.ValidFirefighterRequest = true;
                        }
                        else
                        {
                            information.SimThatRequestedService = null;
                            information.KnownUnreachableFiresLeftOnLot = false;
                            mAlarmHandle = AlarmManager.AddAlarm(Firefighter.DelayBeforeLeaving, TimeUnit.Hours, TimeToGo, "Firefighter waiting to leave", AlarmType.DeleteOnReset, Parent.Worker);
                            Parent.Worker.SimDescription.ShowSocialsOnSim = true;
                        }
                    }
                }
            }

            public override void OnSocializedWith(Sim sim)
            {
                float timeLeft = AlarmManager.GetTimeLeft(mAlarmHandle, TimeUnit.Hours);
                if (timeLeft < Firefighter.ExtraWaitTimeAfterSocializing)
                {
                    float timeDelta = Firefighter.ExtraWaitTimeAfterSocializing - timeLeft;
                    AlarmManager.UpdateAlarmTime(mAlarmHandle, timeDelta, TimeUnit.Hours);
                }
            }

            private void TimeToGo()
            {
                Exit();
            }
        }

        public class HangAroundThenExtinguish : ChildSituation<FirefighterSituation>
        {
            // Fields
            private GameObject mBurningObject;
            private bool mShouldPlayFailReaction;

            private AlarmHandle mFailureAlarm;

            // Methods
            private HangAroundThenExtinguish()
            { }
            public HangAroundThenExtinguish(FirefighterSituation parent, GameObject burningObject, bool shouldPlayFailReaction) 
                : base(parent)
            {
                mBurningObject = burningObject;
                mShouldPlayFailReaction = shouldPlayFailReaction;
            }

            public override void CleanUp()
            {
                AlarmManager.Global.RemoveAlarm(mFailureAlarm);

                base.CleanUp();
            }

            public override void Init(FirefighterSituation parent)
            {
                Sims3.Gameplay.Services.FirefighterSituation.RouteCloseToFire fire = ForceSituationSpecificInteraction(mBurningObject, parent.Worker, Sims3.Gameplay.Services.FirefighterSituation.RouteCloseToFire.Singleton, null, OnStartFailureAlarm, OnStartFailureAlarm) as Sims3.Gameplay.Services.FirefighterSituation.RouteCloseToFire;
                fire.mPlayFailReaction = mShouldPlayFailReaction;
                if (mShouldPlayFailReaction)
                {
                    parent.mFailedAttemptsToReachBurningObject++;
                }
            }

            private void OnStartFailureAlarm(Sim actor, float x)
            {
                mFailureAlarm = AlarmManager.Global.AddAlarm(1, TimeUnit.Minutes, OnFailToAccessFire, "Failure Alarm", AlarmType.NeverPersisted, null);
            }

            private void OnFailToAccessFire()
            {
                if (Parent.mFailedAttemptsToReachBurningObject >= Sims3.Gameplay.Services.FirefighterSituation.kNumAttemptsBeforeFirefighterLeaves)
                {
                    Firefighter.FirefighterInformation information = null;
                    if (Firefighter.sFirefighterDictionary.TryGetValue(Parent.Lot.LotId, out information))
                    {
                        information.KnownUnreachableFiresLeftOnLot = true;
                    }

                    Exit();
                }
                else
                {
                    Parent.SetStateExtinguishSim(mBurningObject as Sim);
                }
            }
        }
    }
}

