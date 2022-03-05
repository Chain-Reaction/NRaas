using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Situations;
using System;
using System.Collections.Generic;


namespace NRaas.OverwatchSpace.Loadup
{    public class CleanupHostlessNPCParties : DelayedLoadupOption
    {
        public CleanupHostlessNPCParties()
        { }

        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupHostlessNPCParties");

            if (Sim.sOnLotChangedDelegates != null)
            {
                List<Delegate> list = new List<Delegate>(Sim.sOnLotChangedDelegates.GetInvocationList());

                foreach (Delegate del in list)
                {
                    NpcParty.Happening npcParty = del.Target as NpcParty.Happening;
                    if (npcParty != null)
                    {
                        if ((npcParty.Parent == null) || (npcParty.Parent.Host == null))
                        {
                            Sim.sOnLotChangedDelegates -= npcParty.OnSimLotChanged;

                            Overwatch.Log("Dropped Unhosted NPCParty: Happening");
                        }
                    }

                    NpcParty.WaitForSelectableGuestToArrive npcPartySelectable = del.Target as NpcParty.WaitForSelectableGuestToArrive;
                    if (npcPartySelectable != null)
                    {
                        if ((npcPartySelectable.Parent == null) || (npcPartySelectable.Parent.Host == null))
                        {
                            Sim.sOnLotChangedDelegates -= npcPartySelectable.OnSimLotChanged;

                            Overwatch.Log("Dropped Unhosted NPCParty: WaitForSelectableGuestToArrive");
                        }
                    }
                }
            }
        }
    }
}
