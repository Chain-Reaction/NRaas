using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TravelerSpace.Transfers
{
    public abstract class OccultTransfer
    {
        static Dictionary<OccultTypes, OccultTransfer> sLookup;

        public static OccultTransfer Get(OccultTypes type)
        {
            if (sLookup == null)
            {
                sLookup = new Dictionary<OccultTypes,OccultTransfer>();

                foreach(OccultTransfer transfer in Common.DerivativeSearch.Find<OccultTransfer>())
                {
                    sLookup.Add(transfer.OccultType, transfer);
                }
            }

            OccultTransfer result;
            if (sLookup.TryGetValue(type, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public abstract OccultTypes OccultType
        {
            get;
        }

        public abstract void Perform(SimDescription sim, OccultBaseClass source);
    }
}
