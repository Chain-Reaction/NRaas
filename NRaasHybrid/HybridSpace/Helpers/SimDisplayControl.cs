using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Helpers
{
    public class SimDisplayControl : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            Sims3.UI.Responder.Instance.HudModel.SimChanged -= OnSimChanged;
            Sims3.UI.Responder.Instance.HudModel.OccultUpdated -= OnOccultChanged;
            Sims3.UI.Responder.Instance.HudModel.SimMotivesChanged -= OnSimMotivesChanged;
            Sims3.UI.Responder.Instance.HudModel.MagicMotiveChanged -= OnMagicMotiveChanged;

            // Ensure that my calls are the last ones in the list
            Sims3.UI.Responder.Instance.HudModel.SimChanged += OnSimChanged;
            Sims3.UI.Responder.Instance.HudModel.OccultUpdated += OnOccultChanged;
            Sims3.UI.Responder.Instance.HudModel.SimMotivesChanged += OnSimMotivesChanged;
            Sims3.UI.Responder.Instance.HudModel.MagicMotiveChanged += OnMagicMotiveChanged;

            new Common.DelayedEventListener(EventTypeId.kEventSimSelected, OnSimSelected);
        }

        protected static void OnMagicMotiveChanged()
        {
            try
            {
                Common.FunctionTask.Perform(OnUpdateMagicBar);
            }
            catch (Exception e)
            {
                Common.Exception("OnSimMotivesChanged", e);
            }
        }

        protected static void OnSimMotivesChanged(SimInfo info)
        {
            try
            {
                Common.FunctionTask.Perform(OnUpdateMagicBar);
            }
            catch (Exception e)
            {
                Common.Exception("OnSimMotivesChanged", e);
            }
        }

        protected static void OnSimChanged(ObjectGuid id)
        {
            try
            {
                Common.FunctionTask.Perform(OnUpdateMagicBar);
            }
            catch (Exception e)
            {
                Common.Exception("OnSimChanged", e);
            }
        }

        protected static void OnOccultChanged()
        {
            try
            {
                Common.FunctionTask.Perform(OnUpdateMagicBar);
            }
            catch (Exception e)
            {
                Common.Exception("OnOccultChanged", e);
            }
        }

        protected static void OnSimSelected(Event e)
        {
            OnUpdateMagicBar();
        }

        protected static void OnUpdateMagicBar()
        {
            SimDisplay display = SimDisplay.Instance;
            if (display == null) return;

            if (display.mHudModel == null) return;

            SimInfo currentSimInfo = display.mHudModel.GetCurrentSimInfo();
            if (currentSimInfo == null) return;

            SimDisplayEx.UpdateMagicMotiveBar(display, currentSimInfo);
        }
    }
}
