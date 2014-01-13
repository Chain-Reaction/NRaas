using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using NRaas.TravelerSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.States
{
    public class ToInWorldStateEx : ToInWorldState
    {
        static int sStatusCount = 0;

        public static EventHandler sOnWorldLoadFinishedEventHandler;

        public ToInWorldStateEx()
        { }

        public static int StatusCount
        {
            get
            {
                return sStatusCount;
            }
            set
            {
                sStatusCount = value;
            }
        }

        public static void OnObjectGroupLoaded(IScriptObjectGroup group)
        {
            Common.StringBuilder msg = new Common.StringBuilder("ToInWorldState:OnObjectGroupLoaded");

            try
            {
                sStatusCount++;

                msg += Common.NewLine + "Count: " + sStatusCount;
                msg += Common.NewLine + "Group: " + group;

                IDictionary objects = group.AttachedObjects;
                if (objects != null)
                {
                    msg += Common.NewLine + "Count: " + objects.Count;

                    if (objects.Count > 0)
                    {
                        foreach (object obj in objects.Keys)
                        {
                            msg += Common.NewLine + "First Key: " + obj.GetType();
                            break;
                        }

                        foreach (object obj in objects.Values)
                        {
                            if (obj != null)
                            {
                                msg += Common.NewLine + "First Value: " + obj.GetType();
                            }
                            break;
                        }

                        Traveler.InsanityWriteLog(msg);
                    }
                }
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }

        public static void OnWorldLoadStatusEx(object sender, EventArgs args)
        {
            Common.StringBuilder msg = new Common.StringBuilder("ToInWorldState:OnWorldLoadStatusEx");

            try
            {
                sStatusCount++;
                msg += Common.NewLine + "Count: " + sStatusCount;
                msg += Common.NewLine + "Sender: " + sender;
                msg += Common.NewLine + "Args: " + args;
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }

        public static void OnWorldLoadFinishedEx(object sender, EventArgs args)
        {
            Common.StringBuilder msg = new Common.StringBuilder("ToInWorldState:OnWorldLoadFinishedEx <Begin>");
            Traveler.InsanityWriteLog(msg);

            try
            {
                using (Common.TestSpan span = new Common.TestSpan(Common.ExternalTimeSpanLogger.sLogger, "OnWorldLoadFinishedEx", Common.DebugLevel.Low))
                {
                    msg += Common.NewLine + "Order";

                    foreach (Delegate del in sOnWorldLoadFinishedEventHandler.GetInvocationList())
                    {
                        msg += Common.NewLine + del.Method + " : " + del.Method.DeclaringType.AssemblyQualifiedName + " : " + del.Target;
                    }

                    msg += Common.NewLine + "Actual";

                    foreach (Delegate del in sOnWorldLoadFinishedEventHandler.GetInvocationList())
                    {
                        using (Common.TestSpan subSpan = new Common.TestSpan(Common.ExternalTimeSpanLogger.sLogger, "OnWorldLoadFinishedEx: " + del.Method + " : " + del.Method.DeclaringType.AssemblyQualifiedName, Common.DebugLevel.Low))
                        {
                            msg += Common.NewLine + del.Method + " : " + del.Method.DeclaringType.AssemblyQualifiedName + " : " + del.Target;
                            Traveler.InsanityWriteLog(msg);

                            try
                            {
                                del.DynamicInvoke(new object[] { sender, args });
                            }
                            catch (Exception e)
                            {
                                Traveler.InsanityException(msg, e);
                            }
                        }
                    }
                }

                msg += Common.NewLine + "ToInWorldState:OnWorldLoadFinishedEx <End>";
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }

        public override void Startup()
        {
            Common.StringBuilder msg = new Common.StringBuilder("ToInWorldStateEx:Startup");
            Traveler.InsanityWriteLog(msg);

            try
            {
                sStatusCount = 0;

                LoadSaveManager.ObjectGroupLoaded += OnObjectGroupLoaded;

                sOnWorldLoadFinishedEventHandler = World.sOnWorldLoadFinishedEventHandler;
                World.sOnWorldLoadFinishedEventHandler = OnWorldLoadFinishedEx;

                World.sOnWorldLoadStatusEventHandler += OnWorldLoadStatusEx;

                base.Startup();

                msg += Common.NewLine + "FileName: " + GameStates.LoadFileName;
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }

        public override void Shutdown()
        {
            Common.StringBuilder msg = new Common.StringBuilder("ToInWorldStateEx:Shutdown");
            Traveler.InsanityWriteLog(msg);

            try
            {
                base.Shutdown();

                LoadSaveManager.ObjectGroupLoaded -= OnObjectGroupLoaded;

                World.sOnWorldLoadStatusEventHandler -= OnWorldLoadStatusEx;

                World.sOnWorldLoadFinishedEventHandler -= OnWorldLoadFinishedEx;
                World.sOnWorldLoadFinishedEventHandler += sOnWorldLoadFinishedEventHandler;

                msg += Common.NewLine + "StatusCount: " + sStatusCount;
                Traveler.InsanityWriteLog(msg);
            }
            catch (Exception e)
            {
                Traveler.InsanityException(msg, e);
            }
        }
    }
}
