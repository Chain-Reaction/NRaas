using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Scuba;
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
    public class PickTask : PickObjectTask
    {
        public override void ProcessClick(ScenePickArgs eventArgs)
        {
            try
            {
                Sims3.Gameplay.UI.PieMenu.ClearInteractions();

                if (!CustomProcessClick(eventArgs))
                {
                    ProcessClick(eventArgs, false);
                }
            }
            catch (Exception e)
            {
                Common.Exception("Type: " + eventArgs.mObjectType, e);
            }
        }

        private bool CustomProcessClick(ScenePickArgs eventArgs)
        {
            switch (eventArgs.mObjectType)
            {
                case ScenePickObjectType.Terrain:
                case ScenePickObjectType.Floor:
                case ScenePickObjectType.RoadStreet:
                case ScenePickObjectType.RoadSidewalk:
                case ScenePickObjectType.WaterPond:
                case ScenePickObjectType.WaterSea:
                case ScenePickObjectType.WaterPool:
                case ScenePickObjectType.WaterFountain:
                    if ((Bin.PlacementHousehold == null) && (Bin.PlacementLot == null))
                    {
                        return GameObjectEx.OnPick(Terrain.Singleton, eventArgs.mMouseEvent, eventArgs.AsGameObjectHit());
                    }
                    break;
                case ScenePickObjectType.Object:
                case ScenePickObjectType.Sim:
                case ScenePickObjectType.Wall:
                    ObjectGuid guid = ObjectGuid.InvalidObjectGuid;
                    if ((eventArgs.mObjectType == ScenePickObjectType.Object) || (eventArgs.mObjectType == ScenePickObjectType.Sim))
                    {
                        guid = new ObjectGuid(eventArgs.mObjectId);
                    }
                    else if (eventArgs.mObjectType == ScenePickObjectType.Wall)
                    {
                        guid = LotManager.GetLotObjectGuid(eventArgs.mObjectId);
                    }

                    IScriptProxy proxy = Simulator.GetProxy(guid);
                    if (proxy != null)
                    {
                        IObjectUI target = proxy.Target as IObjectUI;
                        if (target != null)
                        {
                            Lot lot = target as Lot;
                            if (lot != null)
                            {
                                return OnPickLot(lot, eventArgs.mMouseEvent, eventArgs.AsGameObjectHit());
                            }
                            else
                            {
                                GameObject obj = target as GameObject;
                                if (obj != null)
                                {
                                    return GameObjectEx.OnPick(obj, eventArgs.mMouseEvent, eventArgs.AsGameObjectHit());
                                }
                            }
                        }
                    }
                    break;
            }

            return false;
        }

        private bool OnPickLot(Lot ths, UIMouseEventArgs eventArgs, GameObjectHit gameObjectHit)
        {
            BuildableShell[] buildableShells = ths.BuildableShells;
            int index = 0x0;
            while (index < buildableShells.Length)
            {
                return GameObjectEx.OnPick(buildableShells[index], eventArgs, gameObjectHit);
            }

            return GameObjectEx.OnPick(ths, eventArgs, gameObjectHit);
        }
    }
}
