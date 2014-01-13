using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Tasks;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Helpers
{
    /*
    public class NavigationEx : Common.IPreLoad
    {
        public static void OnNavButtonClick(WindowBase sender, UIButtonClickEventArgs eventArgs)
        {
            try
            {
                Navigation ths = Navigation.Instance;

                Common.DebugNotify(delegate { return "OnNavButtonClick " + (ths.mHudModel.GetCurrentSimInfo() != null); });

                uint iD = sender.ID;
                if ((iD >= 0x1ba48c01) && (iD <= 0x1ba48c08))
                {
                    uint num2 = iD - 0x1ba48c01;
                    HudController.Instance.InternalSetInfoState((InfoState)num2);
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnNavButtonClick", e);
            }
        }

        public void OnPreLoad()
        {
            Task.Create<Task>();
        }

        protected class Task : RepeatingTask
        {
            protected override bool OnPerform()
            {
                Navigation navigation = Navigation.Instance;
                if (navigation != null)
                {
                    for (uint i = 0x1ba48c01; i <= 0x1ba48c08; i++)
                    {
                        uint index = i - 0x1ba48c01;
                        if (navigation.mInfoStateButtons[index] != null)
                        {
                            navigation.mInfoStateButtons[index].Click -= navigation.OnNavButtonClick;
                            navigation.mInfoStateButtons[index].Click -= OnNavButtonClick;
                            navigation.mInfoStateButtons[index].Click += OnNavButtonClick;
                        }
                    }
                }

                return true;
            }
        }
    }
    */
}
