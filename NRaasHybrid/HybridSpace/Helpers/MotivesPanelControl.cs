using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
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
    public class MotivesPanelControl : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            Sims3.UI.Responder.Instance.HudModel.SimMotivesChanged -= OnSimMotivesChanged;
            Sims3.UI.Responder.Instance.HudModel.OccultUpdated -= OnOccultChanged;

            // Ensure that my calls are the last in the list
            Sims3.UI.Responder.Instance.HudModel.SimMotivesChanged += OnSimMotivesChanged;
            Sims3.UI.Responder.Instance.HudModel.OccultUpdated += OnOccultChanged;

            new Common.DelayedEventListener(EventTypeId.kEventSimSelected, OnSimSelected);
        }

        protected static void OnSimMotivesChanged(SimInfo info)
        {
            try
            {
                Common.FunctionTask.Perform(OnUpdateMotives);
            }
            catch (Exception e)
            {
                Common.Exception("OnSimMotivesChanged", e);
            }
        }

        protected static void OnOccultChanged()
        {
            try
            {
                Common.FunctionTask.Perform(OnUpdateMotives);
            }
            catch (Exception e)
            {
                Common.Exception("OnOccultChanged", e);
            }
        }

        protected static void OnSimSelected(Event e)
        {
            OnUpdateMotives();
        }

        protected static void OnUpdateMotives()
        {
            MotivesPanel display = MotivesPanel.Instance;
            if (display == null) return;

            if (display.mHudModel == null) return;

            SimInfo currentSimInfo = display.mHudModel.GetCurrentSimInfo();
            if (currentSimInfo == null) return;

            currentSimInfo.OccultInfo = OccultManagerEx.GetSimOccultInfo();

            MotivesPanelEx.OnMotivesChanged(display, currentSimInfo);
        }
    }
}
