using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class SecondImage : Common, Common.IPreLoad
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static SecondImage()
        {
            Bootstrap();
        }

        public void OnPreLoad()
        {
            ImageTask.Create<ImageTask>();
        }

        private static Tooltip CreateKnownInfoToolTip(Vector2 mousePosition, WindowBase parent, ref Vector2 tooltipPosition)
        {
            try
            {
                if (Sim.ActiveActor == null) return null;

                NotificationManager manager = NotificationManager.Instance;
                if (manager != null)
                {
                    if ((manager.mNotifications[manager.mCurrentCategory].Count > manager.mCurrentNotification) && (manager.mCurrentNotification >= 0x0))
                    {
                        StyledNotification notice = manager.mNotifications[manager.mCurrentCategory][manager.mCurrentNotification] as StyledNotification;
                        if (notice != null)
                        {
                            if (notice.mNotificationWindow != null)
                            {
                                Sim sim = null;

                                if (notice.mNotificationWindow.GetChildByID(0x5, true) == parent)
                                {
                                    if (notice.mIDOne != ObjectGuid.InvalidObjectGuid)
                                    {
                                        sim = GameObject.GetObject<Sim>(notice.mIDOne);
                                    }
                                }
                                else
                                {
                                    if (notice.mIDTwo != ObjectGuid.InvalidObjectGuid)
                                    {
                                        sim = GameObject.GetObject<Sim>(notice.mIDTwo);
                                    }
                                }

                                if (sim != null)
                                {
                                    IMiniSimDescription tag = sim.SimDescription;
                                    if (tag != null)
                                    {
                                        IHudModel hudModel = Sims3.Gameplay.UI.Responder.Instance.HudModel;

                                        Tooltip result = new KnownInfoTooltip(tag.FullName, hudModel.GetLTRRelationshipString(Sim.ActiveActor.SimDescription, tag), tag.HomeWorld, HudModelEx.GetKnownInfo(hudModel as HudModel, tag));

                                        tooltipPosition = (tooltipPosition - (mousePosition - parent.Position)) + new Vector2(-result.TooltipWindow.Area.Width, parent.Area.Height);

                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("CreateKnownInfoToolTip", e);
            }
            return null;
        }

        protected class ImageTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                NotificationManager manager = NotificationManager.Instance;
                if (manager != null)
                {
                    if ((manager.mNotifications[manager.mCurrentCategory].Count > manager.mCurrentNotification) && (manager.mCurrentNotification >= 0x0))
                    {
                        StyledNotification notice = manager.mNotifications[manager.mCurrentCategory][manager.mCurrentNotification] as StyledNotification;
                        if (notice != null)
                        {
                            if (notice.mIDOne != ObjectGuid.InvalidObjectGuid)
                            {
                                if (notice.mNotificationWindow != null)
                                {
                                    Button childByID = notice.mNotificationWindow.GetChildByID(0x5, true) as Button;
                                    if (childByID != null)
                                    {
                                        childByID.Enabled = true;
                                        childByID.CreateTooltipCallbackFunction = CreateKnownInfoToolTip;
                                    }
                                }
                            }

                            if (notice.mIDTwo != ObjectGuid.InvalidObjectGuid)
                            {
                                if (notice.mContainer != null)
                                {
                                    Button childByID = notice.mContainer.GetChildByID(0x7, true) as Button;
                                    if (childByID != null)
                                    {
                                        childByID.Enabled = true;
                                        childByID.CreateTooltipCallbackFunction = CreateKnownInfoToolTip;
                                    }
                                }
                            }
                        }
                    }
                }

                return true;
            }
        }
    }
}
