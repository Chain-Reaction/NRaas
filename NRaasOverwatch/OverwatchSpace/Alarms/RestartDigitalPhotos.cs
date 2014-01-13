using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Alarms
{
    public class RestartDigitalPhotos : AlarmOption
    {
        public RestartDigitalPhotos()
        { }

        public override string GetTitlePrefix()
        {
            return "RestartDigitalPhotos";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new NRaas.OverwatchSpace.Settings.ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return NRaas.Overwatch.Settings.mRestartDigitalPhotos;
            }
            set
            {
                NRaas.Overwatch.Settings.mRestartDigitalPhotos = value;
            }
        }

        protected override void PrivatePerformAction(bool prompt)
        {
            try
            {
                Overwatch.Log(GetTitlePrefix());

                int count = 0;

                List<MultiImageObject> digital = new List<MultiImageObject>(Sims3.Gameplay.Queries.GetObjects<MultiImageObject>());
                foreach (MultiImageObject obj in digital)
                {
                    if (obj.mAlarmNextSlide == AlarmHandle.kInvalidHandle) continue;

                    obj.StopSlideShow();
                    obj.StartSlideShow();

                    count++;
                }

                if (count > 0)
                {
                    Overwatch.AlarmNotify(Common.Localize(GetTitlePrefix () + ":Success", false, new object[] { count }));
                }
            }
            catch (Exception exception)
            {
                Common.Exception(Name, exception);
            }
        }
    }
}
