using NRaas.OverwatchSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
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
    public class CleanupSituations : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupSituations");

            Dictionary<Situation, bool> existing = new Dictionary<Situation, bool>();

            List<Situation> removed = new List<Situation>();

            for (int i = Situation.sAllSituations.Count - 1; i >= 0; i--)
            {
                Situation situation = Situation.sAllSituations[i];

                bool remove = false;

                GroupingSituation grouping = situation as GroupingSituation;
                if (grouping != null)
                {
                    if ((grouping.mLeader == null) || (grouping.mLeader.HasBeenDestroyed))
                    {
                        remove = true;
                    }
                    else if (grouping.Participants != null)
                    {
                        foreach (Sim sim in grouping.Participants)
                        {
                            if (sim == null)
                            {
                                grouping.mParticipants.Clear();
                                remove = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    HostedSituation hosted = situation as HostedSituation;
                    if (hosted != null)
                    {
                        if ((hosted.Host == null) || (hosted.Host.HasBeenDestroyed))
                        {
                            remove = true;
                        }
                    }
                }

                if (remove)
                {
                    removed.Add(situation);

                    Situation.sAllSituations.RemoveAt(i);

                    Overwatch.Log("Removed Broken Situation: " + situation.GetType());
                }
                else
                {
                    existing[situation] = true;
                }
            }

            foreach (Situation situation in removed)
            {
                try
                {
                    situation.Exit();
                }
                catch (Exception e)
                {
                    Common.DebugException(situation.GetType().ToString(), e);
                }
            }

            foreach (Sim sim in LotManager.Actors)
            {
                if (sim.Autonomy == null) continue;

                if (sim.Autonomy.SituationComponent == null) continue;

                List<Situation> situations = sim.Autonomy.SituationComponent.Situations;
                if (situations == null) continue;

                for (int i = situations.Count - 1; i >= 0; i--)
                {
                    if (existing.ContainsKey(situations[i])) continue;

                    Overwatch.Log("Dropped Broken Situation: " + sim.FullName);

                    situations.RemoveAt(i);
                }
            }
        }
    }
}
