using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Tasks;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.SelectorSpace.Tasks
{
    public class SelectTask : SelectObjectTask
    {
        public override void ProcessClick(ScenePickArgs eventArgs)
        {
            try
            {
                if (((eventArgs.mObjectType != ScenePickObjectType.None) && !WasMouseDragged(eventArgs)) && ((eventArgs.mMouseEvent.MouseKey != MouseKeys.kMouseRight) || ((eventArgs.mMouseEvent.Modifiers & Modifiers.kModifierMaskControl) == Modifiers.kModifierMaskNone)))
                {
                    bool flag = false;
                    if ((eventArgs.mObjectType == ScenePickObjectType.Object) || (eventArgs.mObjectType == ScenePickObjectType.Sim))
                    {
                        ObjectGuid objectId = new ObjectGuid(eventArgs.mObjectId);
                        IScriptProxy proxy = Simulator.GetProxy(objectId);
                        if (proxy != null)
                        {
                            IObjectUI target = proxy.Target as IObjectUI;
                            if (target != null)
                            {
                                Vehicle vehicle = target as Vehicle;
                                if (vehicle != null)
                                {
                                    flag = OnSelect(vehicle, eventArgs.mMouseEvent);
                                }
                                else if (target is Sim)
                                {
                                    flag = OnSelect(target as Sim, eventArgs.mMouseEvent);
                                }
                                else
                                {
                                    flag = target.OnSelect(eventArgs.mMouseEvent);
                                }
                            }
                        }
                    }

                    if (!flag)
                    {
                        CameraController.RequestLerpToTarget(eventArgs.mDisplayPos, 1.5f, false);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("ProcessClick", e);
            }
        }

        private bool OnSelect(Vehicle ths, UIMouseEventArgs eventArgs)
        {
            if (ths.Driver != null)
            {
                return OnSelect(ths.Driver, eventArgs);
            }
            return ths.OnSelect(eventArgs);
        }
        private bool OnSelect(Sim sim, UIMouseEventArgs eventArgs)
        {
            if (!UIUtils.IsOkayToStartModalDialog(true)) return false;

            return DreamCatcher.Select(sim, Selector.Settings.mDreamCatcher, true);
        }
    }
}
