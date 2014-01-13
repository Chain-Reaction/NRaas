using NRaas.CommonSpace.Helpers;
using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
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
    public class TestDoesObjectNeedCleaning : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("TestDoesObjectNeedCleaning");

            foreach (Lot lot in LotManager.AllLots)
            {
                foreach (GameObject obj in lot.GetObjects<GameObject>())
                {
                    try
                    {
                        Lot.DoesObjectNeedCleaning(obj);
                    }
                    catch
                    {
                        Overwatch.Log(" Fail: " + obj.GetType());

                        try
                        {
                            Overwatch.Log(" Name: " + obj.CatalogName);
                        }
                        catch
                        { }
                    }
                }
            }
        }
    }
}
