using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using System;

namespace NRaas.CommonSpace.Helpers
{
    public class Routes
    {
        public static bool Contains(Route route, PathType type)
        {
            for (uint i = 0x0; i < route.GetNumPaths(); i++)
            {
                PathData pathData = route.GetPathData(i);
                if (pathData.PathType == type) return true;
            }

            return false;
        }

        public static string RouteToString(Route route)
        {
            string result = "NumPaths: " + route.GetNumPaths();

            for (uint i = 0x0; i < route.GetNumPaths(); i++)
            {
                PathData pathData = route.GetPathData(i);

                result += Common.NewLine + "PathType: " + pathData.PathType;
                result += Common.NewLine + "PortalStartPos: " + pathData.PortalStartPos;
                result += Common.NewLine + "ObjectId: " + pathData.ObjectId;

                GameObject obj = GameObject.GetObject<GameObject>(pathData.ObjectId);
                if (obj != null)
                {
                    result += Common.NewLine + "Object Type: " + obj.GetType();
                }
            }

            return result;
        }
    }
}
