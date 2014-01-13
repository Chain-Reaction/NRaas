using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.TravelerSpace.Dialogs;
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
    public class VampireTransfer : OccultTransfer
    {
        public override OccultTypes OccultType
        {
            get { return OccultTypes.Vampire; }
        }

        public override void Perform(SimDescription sim, OccultBaseClass sourceParam)
        {
            OccultVampire occult = sim.OccultManager.GetOccultType(sourceParam.ClassOccultType) as OccultVampire;
            OccultVampire source = sourceParam as OccultVampire;

            if (source.mSimsDrunkFrom != null)
            {
                occult.mSimsDrunkFrom = new List<ulong>(source.mSimsDrunkFrom);
            }

            if (source.mSimsMadeThoughtAbout != null)
            {
                occult.mSimsMadeThoughtAbout = new List<ulong>(source.mSimsMadeThoughtAbout);
            }

            if (source.mSimsMindRead != null)
            {
                occult.mSimsMindRead = new List<ulong>(source.mSimsMindRead);
            }

            if (source.mSimsTurned != null)
            {
                occult.mSimsTurned = new List<ulong>(source.mSimsTurned);
            }
        }
    }
}
