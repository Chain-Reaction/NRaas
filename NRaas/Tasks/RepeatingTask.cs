using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Tasks
{
    public abstract class RepeatingTask : Task
    {
        protected StopWatch mTimer;

        public override void Dispose()
        {
            try
            {
                if (mTimer != null)
                {
                    mTimer.Dispose();
                    mTimer = null;
                }

                if (ObjectId != ObjectGuid.InvalidObjectGuid)
                {
                    Simulator.DestroyObject(ObjectId);
                    ObjectId = ObjectGuid.InvalidObjectGuid;
                }
            }
            catch (Exception e)
            {
                Common.Exception("Dispose", e);
            }

            base.Dispose();
        }

        public static void Create<T>()
            where T : RepeatingTask, new()
        {
            T task = null;
            Create(ref task);
        }
        public static void Create<T>(ref T task)
            where T : RepeatingTask, new()
        {
            if (task != null)
            {
                task.Dispose();
            }
            task = new T();
            task.AddToSimulator();
        }

        public ObjectGuid AddToSimulator()
        {
            return Simulator.AddObject(this);
        }

        protected virtual int Delay
        {
            get { return 250; }
        }

        protected abstract bool OnPerform();

        protected virtual void OnPostSimulate()
        { }

        public override void Simulate()
        {
            try
            {
                NRaas.SpeedTrap.Begin();

                mTimer = StopWatch.Create(StopWatch.TickStyles.Milliseconds);
                mTimer.Start();

                while (true)
                {
                    mTimer.Restart();

                    try
                    {
                        while ((mTimer != null) && (mTimer.GetElapsedTime() < Delay))
                        {
                            SpeedTrap.Sleep();
                        }

                        if (!OnPerform())
                        {
                            Stop();
                            return;
                        }

                        SpeedTrap.Sleep();
                    }
                    catch (ResetException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        Common.Exception("Simulate", exception);
                    }

                    if (mTimer == null) return;
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception("Simulate", e);
            }
            finally
            {
                OnPostSimulate();

                NRaas.SpeedTrap.End();
            }
        }

        public override void Stop()
        {
            Dispose();
        }
    }
}

