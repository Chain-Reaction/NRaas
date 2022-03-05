using Sims3.SimIFace;
using System;

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
                //NRaas.SpeedTrap.Begin();

                mTimer = StopWatch.Create(StopWatch.TickStyles.Milliseconds);
                mTimer.Start();

                while (true)
                {
                    mTimer.Restart();

                    try
                    {
                        while ((mTimer != null) && (mTimer.GetElapsedTime() < Delay))
                        {
                            Common.Sleep();
                        }

                        if (!OnPerform())
                        {
                            Stop();
                            return;
                        }

                        Common.Sleep();
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

                //NRaas.SpeedTrap.End();
            }
        }

        public override void Stop()
        {
            Dispose();
        }
    }
}

