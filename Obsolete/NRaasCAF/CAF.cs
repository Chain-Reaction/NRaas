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

    public class CAF : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        protected static PanelTask sTask = null;

        static CAF()
        {
            Bootstrap();
        }

        public void OnWorldLoadFinished()
        {
            try
            {
                if (sTask == null)
                {
                    sTask = new PanelTask();

                    Simulator.AddObject(sTask);
                }
            }
            catch (Exception exception)
            {
                Common.Exception("OnWorldLoadFinished", exception);
            }
        }

        protected class PanelTask : RepeatingTask
        {
            protected override bool OnPerform()
            {
                CASPuck puck = CASPuck.Instance;
                if (puck == null) return true;

                if (puck.mAcceptButton != null)
                {
                    ICASModel cASModel = Sims3.UI.Responder.Instance.CASModel;
                    if ((cASModel.CASMode == CASMode.Full) && !cASModel.EditingExistingSim())
                    {
                        puck.mAcceptButton.Click -= puck.OnAcceptHousehold;
                        puck.mAcceptButton.Click -= OnAcceptHousehold;
                        puck.mAcceptButton.Click += OnAcceptHousehold;
                    }
                }

                CASFamilyScreen familyScreen = CASFamilyScreen.gSingleton;
                if (familyScreen != null)
                {
                    Window topLevel = familyScreen.mFamilyTopLevelWin;

                    uint index = 0;
                    WindowBase child = topLevel.GetChildByIndex(index);
                    while (child != null)
                    {
                        CAFThumb thumb = child as CAFThumb;
                        if (thumb != null)
                        {
                            thumb.DragDrop -= familyScreen.OnCAFThumbDragDrop;
                            thumb.DragDrop -= OnCAFThumbDragDrop;
                            thumb.DragDrop += OnCAFThumbDragDrop;
                        }

                        index++;
                        child = topLevel.GetChildByIndex(index);
                    }
                }

                return true;
            }
        }
    }
}
