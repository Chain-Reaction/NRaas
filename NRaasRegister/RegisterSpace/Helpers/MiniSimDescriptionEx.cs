using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Helpers
{
    public class MiniSimDescriptionEx
    {
        public static List<MiniSimDescription> GetVacationWorldSimDescriptions()
        {
            Dictionary<ulong, SimDescription> allSims = SimListing.GetResidents(false);

            List<MiniSimDescription> list = new List<MiniSimDescription>();
            if (MiniSimDescription.sMiniSims != null)
            {
                WorldName currentWorld = GameUtils.GetCurrentWorld();

                foreach (MiniSimDescription description in MiniSimDescription.sMiniSims.Values)
                {
                    // Added to allow for Traveler miniSims
                    if ((description.HomeWorld == WorldName.UserCreated) && (currentWorld == WorldName.UserCreated))
                    {
                        if (allSims.ContainsKey(description.SimDescriptionId)) continue;

                        list.Add(description);
                    }
                    else if (GameUtils.GetWorldType(description.HomeWorld) == WorldType.Vacation)
                    {
                        list.Add(description);
                    }
                    else if (GameUtils.GetWorldType(description.HomeWorld) == WorldType.Future)
                    {
                        list.Add(description);
                    }
                }
            }
            return list;
        }
    }
}
