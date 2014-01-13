using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Misc;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Helpers
{
    public class MailboxEx
    {
        public static bool GrabMail(Mailbox ths, IActor a)
        {
            bool found = false;

            List<IAmPutInMailbox> list = new List<IAmPutInMailbox>();
            foreach (GameObject obj in Inventories.QuickFind<GameObject>(ths.Inventory))
            {
                bool success = true;

                IAmPutInMailbox item = obj as IAmPutInMailbox;
                if (item != null)
                {
                    try
                    {
                        item.OnRemovalFromMailbox(a as Sim);
                    }
                    catch(ResetException)
                    {
                            throw;
                    }
                    catch (Exception e)
                    {
                        if (obj is LoveLetter)
                        {
                            Common.DebugException(a, obj, e);
                        }
                        else
                        {
                            Common.Exception(a, obj, e);
                        }

                        success = false;
                    }

                    list.Add(item);
                }

                if (success)
                {
                    if (Inventories.TryToMove(obj, a.Inventory))
                    {
                        found = true;
                    }
                }
                else
                {
                    obj.Destroy();
                }
            }

            foreach (IAmPutInMailbox obj in list)
            {
                try
                {
                    obj.OnTransferComplete(a as Sim);
                }
                catch (Exception e)
                {
                    Common.Exception(a, obj, e);
                }
            }

            return found;
        }

        public class MailBoxListener : Common.IDelayedWorldLoadFinished, Common.IWorldQuit
        {
            static MailBoxListener sListener = null;

            Mailbox mBox;

            public MailBoxListener() // Required for runtime creation
            { }
            public MailBoxListener(Mailbox box)
            {
                mBox = box;
            }

            public void OnDelayedWorldLoadFinished()
            {
                Sim activeActor = Sim.ActiveActor;
                if (((GameUtils.GetCurrentWorldType() == WorldType.Vacation) || (GameUtils.GetCurrentWorldType() == WorldType.University)) && (activeActor != null))
                {
                    new Common.DelayedEventListener(EventTypeId.kLotChosenForActiveHousehold, OnLotChanged);

                    UpdateListener();
                }
            }

            protected static void ShutdownListener()
            {
                if (sListener != null)
                {
                    sListener.Dispose();
                }
                sListener = null;
            }

            protected static void UpdateListener()
            {
                ShutdownListener();

                Mailbox box = Mailbox.GetMailboxOnHomeLot(Sim.ActiveActor);
                if (box != null)
                {
                    GameStates.PreReturnHome -= box.PreReturnHome;

                    sListener = new MailBoxListener(box);

                    GameStates.PreReturnHome += sListener.PreReturnHome;
                }
            }

            protected static void OnLotChanged(Event e)
            {
                Lot lot = e.TargetObject as Lot;
                if (lot == null) return;

                if (Sim.ActiveActor == null) return;

                if (lot.Household != Sim.ActiveActor.Household) return;

                UpdateListener();
            }

            public void OnWorldQuit()
            {
                ShutdownListener();
            }

            public void Dispose()
            {
                GameStates.PreReturnHome -= PreReturnHome;
            }

            public void PreReturnHome()
            {
                GameStates.ItemsAddedToTraveller = GrabMail(mBox, Sim.ActiveActor);

                OnWorldQuit();
            }
        }
    }
}
