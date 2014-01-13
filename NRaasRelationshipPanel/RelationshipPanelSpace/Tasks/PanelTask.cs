using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Proxies;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelationshipPanelSpace.Tasks
{
    public class PanelTask : RepeatingTask
    {
        protected static void OnSimMouseUp(WindowBase sender, UIMouseEventArgs eventArgs)
        {
            try
            {
                Window window = sender as Window;
                if ((window != null) && (window.Parent != null))
                {
                    if (eventArgs.MouseKey == MouseKeys.kMouseLeft)
                    {
                        if ((eventArgs.Modifiers & Modifiers.kModifierMaskControl) == Modifiers.kModifierMaskControl)
                        {
                            Sim activeActor = Sim.ActiveActor;

                            if (window.Parent.Tag is SimDescription)
                            {
                                Relationship relation = Relationship.Get((window.Parent.Tag as SimDescription), activeActor.SimDescription, false);
                                if (relation != null)
                                {
                                    Relationship.RemoveRelationship(relation);
                                }
                            }
                            else 
                            {
                                MiniSimDescriptionProxy proxy = window.Parent.Tag as MiniSimDescriptionProxy;

                                if ((proxy != null) && (proxy.mSim is GameObjectDescription))
                                {
                                    GameObjectDescription choice = proxy.mSim as GameObjectDescription;

                                    for (int i = activeActor.SimDescription.GameObjectRelationships.Count - 1; i >= 0; i--)
                                    {
                                        GameObjectRelationship relation = activeActor.SimDescription.GameObjectRelationships[i];

                                        if (relation.GameObjectDescription.GameObject == choice.GameObject)
                                        {
                                            activeActor.SimDescription.GameObjectRelationships.RemoveAt(i);
                                        }
                                    }                                    
                                }
                                else
                                {
                                    IMiniSimDescription miniSim = window.Parent.Tag as IMiniSimDescription;

                                    RemoveMiniRelationship(miniSim.SimDescriptionId, activeActor.SimDescription.SimDescriptionId);
                                    RemoveMiniRelationship(activeActor.SimDescription.SimDescriptionId, miniSim.SimDescriptionId);
                                }
                            }

                            RelationshipsPanel.Instance.Repopulate(RelationshipsPanel.Instance.mHudModel.CurrentRelationships);
                        }
                        else if (window.Parent.Tag is SimDescription)
                        {
                            SimDescriptionEx.OnPickFromPanel(window.Parent.Tag as SimDescription, eventArgs, new GameObjectHit(GameObjectHitType.Object));
                        }
                        else if (window.Parent.Tag is MiniSimDescriptionProxy)
                        {
                            MiniSimDescriptionProxy proxy = window.Parent.Tag as MiniSimDescriptionProxy;

                            if (proxy.mSim is GameObjectDescription)
                            {
                                (proxy.mSim as GameObjectDescription).OnPickFromPanel(eventArgs, new GameObjectHit(GameObjectHitType.Object));
                            }
                            else
                            {
                                MiniSimDescriptionEx.OnPickFromPanel(proxy.mSim as MiniSimDescription, eventArgs, new GameObjectHit(GameObjectHitType.Object));
                            }
                        }
                        else
                        {
                            (window.Parent.Tag as IMiniSimDescription).OnPickFromPanel(eventArgs, new GameObjectHit(GameObjectHitType.Object));
                        }
                    }
                    else if (eventArgs.MouseKey == MouseKeys.kMouseRight)
                    {
                        ObjectGuid objectGuid = (window.Tag == null) ? ObjectGuid.InvalidObjectGuid : ((ObjectGuid)window.Tag);
                        if (objectGuid != ObjectGuid.InvalidObjectGuid)
                        {
                            RelationshipsPanel.Instance.mHudModel.MoveCameraToSim(objectGuid);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnSimMouseUp", exception);
            }
        }

        protected static void RemoveMiniRelationship(ulong a, ulong b)
        {
            MiniSimDescription description = MiniSimDescription.Find(a);
            if (description != null)
            {
                for (int i = 0; i < description.mMiniRelationships.Count; i++)
                {
                    if (description.mMiniRelationships[i].GetOtherSimDescriptionId(description) == b)
                    {
                        description.mMiniRelationships.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        protected override bool OnPerform()
        {
            RelationshipsPanel panel = RelationshipsPanel.Instance;
            if (panel == null) return true;

            if (!panel.Visible) return true;

            if (panel.mHudModel != null)
            {
                if (!(panel.mHudModel is HudModelProxy))
                {
                    panel.mHudModel.SimChanged -= panel.OnSimChanged;
                    panel.mHudModel.SimAgeChanged -= panel.OnSimAgeChanged;
                    panel.mHudModel.CareerUpdated -= panel.OnCareerUpdated;
                    panel.mHudModel.OccultUpdated -= panel.OnOccultChanged;
                    panel.mHudModel.RelationshipsChanged -= panel.OnRelationshipsChanged;
                    panel.mHudModel.SimAppearanceChanged -= panel.OnSimAppearanceChanged;
                    panel.mHudModel.VisitorsChanged -= panel.OnVisitorsChanged;
                    panel.mHudModel.SimCurrentWorldChanged -= panel.OnSimCurrentWorldChanged;

                    panel.mHudModel = new HudModelProxy(panel.mHudModel);

                    panel.mHudModel.SimChanged += panel.OnSimChanged;
                    panel.mHudModel.SimAgeChanged += panel.OnSimAgeChanged;
                    panel.mHudModel.CareerUpdated += panel.OnCareerUpdated;
                    panel.mHudModel.OccultUpdated += panel.OnOccultChanged;
                    panel.mHudModel.RelationshipsChanged += panel.OnRelationshipsChanged;
                    panel.mHudModel.SimAppearanceChanged += panel.OnSimAppearanceChanged;
                    panel.mHudModel.VisitorsChanged += panel.OnVisitorsChanged;
                    panel.mHudModel.SimCurrentWorldChanged += panel.OnSimCurrentWorldChanged;

                    panel.Repopulate(panel.mHudModel.CurrentRelationships);
                }
            }

            if ((panel.mRelItemGrid != null) && (panel.mRelItemGrid.mGrid != null))
            {
                for (int i = 0; i < panel.mRelItemGrid.mGrid.ColumnCount; i++)
                {
                    WindowBase window = panel.mRelItemGrid.mGrid.GetCellWindow(i, 0);

                    Window childByID = window.GetChildByID(0x2, true) as Window;
                    childByID.MouseUp -= panel.OnSimOnLotMouseUp;
                    childByID.MouseUp -= panel.OnSimNotOnLotMouseUp;

                    childByID.MouseUp -= OnSimMouseUp;
                    childByID.MouseUp += OnSimMouseUp;

                    float value = 0;

                    FillBarController controller = window.GetChildByID(0x5, true) as FillBarController;
                    if (controller != null)
                    {
                        value = controller.Value;
                    }

                    childByID = window.GetChildByID(0x24, true) as Window;
                    if (childByID != null)
                    {
                        if ((childByID.TooltipText != null) && (!childByID.TooltipText.EndsWith(")")))
                        {
                            childByID.TooltipText += " (" + EAText.GetNumberString(value) + ")";
                        }
                    }
                }
            }

            return true;
        }
    }
}
