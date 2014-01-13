using NRaas.CommonSpace.Options;
using NRaas.DebugEnablerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Options
{
    public class Purge : OperationSettingOption<GameObject>, IObjectOption
    {
        public override string GetTitlePrefix()
        {
            return "RadiusPurge";
        }

        public static bool Allow(IGameObject target)
        {
            if (target is Lot) return false;

            if (target is PlumbBob) return false;

            if (target == Sim.ActiveActor) return false;

            return true;
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!parameters.mTarget.InInventory)
            {
                if (CameraController.IsMapViewModeEnabled()) return false;
            }

            if (!Allow(parameters.mTarget)) return false;

            return DebugEnabler.Settings.mEnabled;
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            int radius = 0;
            if ((parameters.mTarget == null) || (!parameters.mTarget.InInventory))
            {
                string text = StringInputDialog.Show(Name, Common.Localize("RadiusPurge:Prompt"), "0", 1, StringInputDialog.Validation.Number);
                if (string.IsNullOrEmpty(text)) return OptionResult.Failure;

                if (!int.TryParse(text, out radius))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error", false, new object[] { text }));
                    return OptionResult.Failure;
                }
            }

            Dictionary<string, uint> purged = new Dictionary<string, uint>();

            uint num = 0;
            if ((radius == 0) && (parameters.mTarget != null))
            {
                List<IGameObject> objs = new List<IGameObject>();

                if (parameters.mTarget.InInventory)
                {
                    Inventory inventory = Inventory.ParentInventory(parameters.mTarget);

                    InventoryStack stack;
                    if (inventory.FindItemStack(parameters.mTarget, out stack) != 0)
                    {
                        foreach (InventoryItem item in stack.List)
                        {
                            objs.Add(item.mObject);
                        }
                    }
                    else
                    {
                        objs.Add(parameters.mTarget);
                    }

                    foreach(IGameObject obj in objs)
                    {
                        inventory.TryToRemove(obj);
                    }
                }

                foreach (IGameObject obj in objs)
                {
                    if (!Allow(obj)) continue;

                    if (obj is Sim)
                    {
                        obj.Destroy();
                    }
                    else
                    {
                        obj.Destroy();
                        obj.Dispose();
                    }

                    num++;
                }
            }
            else
            {
                foreach (GameObject obj in Sims3.Gameplay.Queries.GetObjects<GameObject>(parameters.mHit.mPoint, radius))
                {
                    if (!Allow(obj)) continue;

                    try
                    {
                        if (obj is Sim)
                        {
                            obj.Destroy();
                        }
                        else
                        {
                            obj.Destroy();
                            obj.Dispose();
                        }

                        if (purged.ContainsKey(obj.CatalogName))
                        {
                            purged[obj.CatalogName]++;
                        }
                        else
                        {
                            purged.Add(obj.CatalogName, 0x1);
                        }
                        num++;
                    }
                    catch (Exception exception)
                    {
                        Common.Exception(parameters.mActor, obj, exception);
                    }
                }
            }

            if (purged.Count > 0x0)
            {
                string msg = "Objects Purged:";
                foreach (KeyValuePair<string, uint> pair in purged)
                {
                    msg += Common.NewLine + pair.Key + ": " + pair.Value.ToString();
                }

                StyledNotification.Format format = new StyledNotification.Format(msg, ObjectGuid.InvalidObjectGuid, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage);
                format.mTNSCategory = NotificationManager.TNSCategory.Lessons;
                StyledNotification.Show(format);
            }

            return OptionResult.SuccessClose;
        }
    }
}