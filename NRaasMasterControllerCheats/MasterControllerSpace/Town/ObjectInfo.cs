using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.SelectionCriteria;
using NRaas.MasterControllerSpace.Town;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Town
{
    public class ObjectInfo : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "ObjectInfo";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            return new LotProcessor(GetTitlePrefix(), null).Perform(OnInfo);
        }

        public static bool OnInfo(IGameObject obj)
        {
            Common.StringBuilder noticeText = new Common.StringBuilder();
            Common.StringBuilder logText = new Common.StringBuilder();
            Common.ExceptionLogger.Convert(obj, noticeText, logText);

            Common.Notify(logText.ToString(), obj.ObjectId);

            Common.WriteLog(logText);

            if (CameraController.IsMapViewModeEnabled())
            {
                Sims3.Gameplay.Core.Camera.ToggleMapView();
            }

            Camera.FocusOnGivenPosition(obj.Position, Camera.kDefaultLerpDuration);
            return true;
        }
    }
}
