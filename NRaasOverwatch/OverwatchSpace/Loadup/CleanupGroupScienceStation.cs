using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills.Science;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupGroupScienceStation : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupGroupScienceStation");

            foreach (GroupScienceStation station in Sims3.Gameplay.Queries.GetObjects<GroupScienceStation>())
            {
                if (station.mActiveMeteorites != null && station.mActiveMeteorites.Count > 0)
                {
                    foreach (MeteoriteObject obj in station.mActiveMeteorites)
                    {
                        try
                        {
                            //VisualEffect.OnEffectFinishedEventHandler -= new EventHandler(obj.ImpactEffectCallback);
                            obj.mFallEffect.Stop();
                            obj.mFallEffect = null;
                            obj.mOwner.RemoveActiveMeteoriteObject(obj);
                        }
                        catch
                        { }
                    }

                    station.mActiveMeteorites.Clear();
                    Overwatch.Log("Dropped bogus science station effects");
                }
            }
        }
    }
}
