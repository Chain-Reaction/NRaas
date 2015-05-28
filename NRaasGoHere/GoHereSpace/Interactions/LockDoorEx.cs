using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.GoHereSpace.Helpers;
using NRaas.GoHereSpace.Options.DoorFilters;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay;
using Sims3.Gameplay.Objects.Door;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.Store.Objects;
using Sims3.UI;
using Sims3.UI.View;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class LockDoorEx : CommonDoor.LockDoor, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<CommonDoor, CommonDoor.LockDoor.Definition>(Singleton);
        }

        public override bool Run()
        {            
            Definition interactionDefinition = base.InteractionDefinition as Definition;
            if (interactionDefinition.LockType != CommonDoor.tLock.OwnerList)
            {
                // eh, probably need to mash these into one interaction...
                if ((uint)interactionDefinition.LockType == 7)
                {
                    new InteractionOptionList<IDoorOption, GameObject>.AllList(GoHere.Localize("DoorOptions:MenuName"), false).Perform(new GameHitParameters<GameObject>(this.Actor, this.Target, GameObjectHit.NoHit));
                }                
                else
                {
                    base.Target.SetLockTypeAndOwner(interactionDefinition.LockType, base.Actor.SimDescription);

                    GoHere.Settings.ClearActiveDoorFilters(base.Target.ObjectId);
                }
            }
            else
            {
                List<PhoneSimPicker.SimPickerInfo> collection = new List<PhoneSimPicker.SimPickerInfo>();
                foreach (Sim sim in LotManager.Actors)
                {
                    SimDescription description = sim.SimDescription;
                    if (!description.IsHorse && !description.IsDeer && !description.IsRaccoon)
                    {
                        PhoneSimPicker.SimPickerInfo item = CreateSimPickerInfo(base.Actor.SimDescription, description);
                        collection.Add(item);
                    }
                }
                List<PhoneSimPicker.SimPickerInfo> alreadySelectedSim = new List<PhoneSimPicker.SimPickerInfo>();
                switch (base.Target.LockType)
                {
                    case CommonDoor.tLock.SelectedActor:
                        foreach (PhoneSimPicker.SimPickerInfo info3 in collection)
                        {
                            if (info3.SimDescription == base.Target.mLockOwner)
                            {
                                alreadySelectedSim.Add(info3);
                            }
                        }
                        break;

                    case CommonDoor.tLock.SelectedHousehold:
                        foreach (PhoneSimPicker.SimPickerInfo info4 in collection)
                        {
                            if ((info4.SimDescription as SimDescription).Household == base.Target.mLockOwner.Household)
                            {
                                alreadySelectedSim.Add(info4);
                            }
                        }
                        break;

                    case CommonDoor.tLock.Anybody:
                        alreadySelectedSim.AddRange(collection);
                        break;

                    case CommonDoor.tLock.Pets:
                        foreach (PhoneSimPicker.SimPickerInfo info2 in collection)
                        {
                            if ((info2.SimDescription as SimDescription).IsPet)
                            {
                                alreadySelectedSim.Add(info2);
                            }
                        }
                        break;

                    case CommonDoor.tLock.OwnerList:
                        foreach (PhoneSimPicker.SimPickerInfo info5 in collection)
                        {
                            if (base.Target.mOwnerList.Contains((info5.SimDescription as SimDescription).SimDescriptionId))
                            {
                                alreadySelectedSim.Add(info5);
                            }
                        }
                        break;                       
                }
                List<PhoneSimPicker.SimPickerInfo> list3 = DualPaneSimPicker.Show(collection, alreadySelectedSim, Localization.LocalizeString("Gameplay/Abstracts/Doors/Locking:CanOpen", new object[0]), Localization.LocalizeString("Gameplay/Abstracts/Doors/Locking:CannotOpen", new object[0]));
                if (list3 != null)
                {
                    base.Target.SetLockTypeOwnerList(list3.ConvertAll<SimDescription>(new Converter<PhoneSimPicker.SimPickerInfo, SimDescription>(CommonDoor.LockDoor.SimPickerInfoToSimDescription)));
                    GoHere.Settings.ClearActiveDoorFilters(base.Target.ObjectId);
                }
            }
            return true;
        }

        [DoesntRequireTuning]
        public new class Definition : CommonDoor.LockDoor.Definition
        {
            public Definition()
            { }

            public Definition(CommonDoor.tLock lockType)
            {
                base.mLockType = lockType;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, CommonDoor target, List<InteractionObjectPair> results)
            {
                foreach (CommonDoor.tLock @lock in Enum.GetValues(typeof(CommonDoor.tLock)))
                {
                    if (((@lock != CommonDoor.tLock.VIPRoom) && ((target.LockType != @lock) || (@lock == CommonDoor.tLock.OwnerList))))
                    {
                        results.Add(new InteractionObjectPair(new LockDoorEx.Definition(@lock), target));
                    }
                }

                if (!(target is Turnstile) && !(target is MysteriousDeviceDoor))
                {
                    results.Add(new InteractionObjectPair(new LockDoorEx.Definition((CommonDoor.tLock)7), target));                    
                }
            }

            public override string GetInteractionName(Sim actor, CommonDoor target, InteractionObjectPair iop)
            {
                if ((uint)this.mLockType == 7)
                {
                    return Common.Localize("DoorOptions:MenuName");
                }                

                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new LockDoorEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, CommonDoor target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (/*!target.CanBeLocked ||*/target.IsInPublicResidentialRoom)
                {
                    return false;
                }
                if (target.GetContainedObject(unchecked((Slot)(-1474234202))) is IVelvetRopes)
                {
                    return false;
                }
                Door door = target as Door;
                if ((door == null) || door.IsNPCDoor)
                {
                    return false;
                }
                if ((this.mLockType == CommonDoor.tLock.Anybody) && (target.LockType == CommonDoor.tLock.Anybody))
                {
                    return false;
                }                
                if ((this.mLockType == CommonDoor.tLock.Pets) && (!GameUtils.IsInstalled(ProductVersion.Undefined | ProductVersion.EP5) /*|| target.LotCurrent.IsOwned*/))
                {
                    return false;
                }                
                /*
                if (a.LotHome != target.LotCurrent)
                {
                    return false;
                }
                if (!a.LotHome.IsResidentialLot)
                {
                    return a.LotHome.IsDormitoryLot;
                }
                 */
                return true;
            }
        }
    }
}