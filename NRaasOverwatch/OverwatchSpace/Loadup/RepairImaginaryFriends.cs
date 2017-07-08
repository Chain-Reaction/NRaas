using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Miscellaneous;
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
    public class RepairImaginaryFriends : DelayedLoadupOption
    {
        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("RepairImaginaryFriends");

            Trait trait;
            if (TraitManager.sDictionary.TryGetValue((ulong)TraitNames.ImaginaryFriendHiddenTrait, out trait))
            {
                trait.mNonPersistableData.mCanBeLearnedRandomly = false;
            }

            foreach (ImaginaryDoll doll in Sims3.Gameplay.Queries.GetObjects<ImaginaryDoll>())
            {
                if (doll.mLiveStateSimDescId == 0) continue;

                if (doll.GetLiveFormSimDescription() != null) continue;

                doll.CreateLiveStateForm();

                Overwatch.Log("Missing Imaginary Doll Repaired");
            }

            foreach (SimDescription sim in Households.All(Household.NpcHousehold))
            {
                if (sim.OccultManager == null) continue;

                OccultImaginaryFriend occult = sim.OccultManager.GetOccultType(OccultTypes.ImaginaryFriend) as OccultImaginaryFriend;
                if (occult == null) continue;

                Overwatch.Log(sim.FullName);

                if (occult.IsReal) continue;

                SimDescription owner = SimDescription.Find(occult.OwnerSimDescriptionId);
                if (owner == null) continue;

                if (owner.LotHome == null) continue;

                IScriptProxy proxy = Simulator.GetProxy(occult.mDollId);
                if (proxy == null)
                {
                    IGameObject obj = GlobalFunctions.CreateObjectOutOfWorld("ImaginaryFriendDoll", ProductVersion.EP4);
                    if (obj != null)
                    {
                        ImaginaryDoll doll = obj as ImaginaryDoll;
                        if (doll == null)
                        {
                            obj.Destroy();
                        }
                        else
                        {
                            occult.UpdateDollGuid(obj.ObjectId);

                            doll.SetOwner(owner);

                            doll.mLiveStateSimDescId = sim.SimDescriptionId;
                            doll.mIsFemale = sim.IsFemale;
                            doll.mGenderSet = true;
                            doll.EstablishState(ImaginaryDoll.OwnershipState.Live);

                            Sim ownerSim = owner.CreatedSim;
                            if (ownerSim != null)
                            {
                                if (Inventories.TryToMove(obj, ownerSim))
                                {
                                    Overwatch.Log("Imaginary Friend Doll Added To Sim Inventory");
                                }
                                else
                                {
                                    obj.Destroy();
                                }
                            }
                            else
                            {
                                if (owner.Household != null && Inventories.TryToMove(obj, owner.Household.SharedFamilyInventory.Inventory))
                                {
                                    Overwatch.Log("Imaginary Friend Doll Added To Family Inventory");
                                }
                                else
                                {
                                    obj.Destroy();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
