using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupMountedFish : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupMountedFish");

            foreach (MountedFish mount in Sims3.Gameplay.Queries.GetObjects<MountedFish>())
            {
                if (mount.mFish == null) continue;

                if (!mount.mFish.HasBeenDestroyed) continue;

                FishData data;
                if (!Fish.sFishData.TryGetValue(mount.mFish.Type, out data)) continue;

                FishInitParameters initData = new FishInitParameters(mount.mFish.Type, mount.mFish.mWeight, mount.mFish.mFishingSim);
                Fish fish = GlobalFunctions.CreateObject(data.MedatorName, data.IngredientData.RequiredCodeVersion, Vector3.OutOfWorld, 0x0, Vector3.UnitZ, null, initData) as Fish;
                if (fish == null) continue;

                mount.mFish = fish;
                fish.Mounted = true;

                Overwatch.Log(" Fixed: " + fish.Type);
            }
        }
    }
}
