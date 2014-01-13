using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
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
    public class ClearNotices : OptionItem, ITownOption
    {
        public override string GetTitlePrefix()
        {
            return "ClearNotices";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (NotificationManager.Instance == null) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (!AcceptCancelDialog.Show(Common.Localize("ClearNotices:Prompt"))) return OptionResult.Failure;

            NotificationManager manager = NotificationManager.Instance;
            if (manager == null) return OptionResult.Failure;

            foreach (KeyValuePair<NotificationManager.TNSCategory, List<Notification>> pair in manager.mNotifications)
            {
                foreach (Notification notice in new List<Notification>(pair.Value))
                {
                    manager.Remove(notice);
                }
            }

            return OptionResult.SuccessClose;
        }
    }
}
