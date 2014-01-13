using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.GoHereSpace.Interactions
{
    public class Sim_ReadToSleep_GetBookEx : Sim.Sim_ReadToSleep_GetBook, Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.Sim_ReadToSleep_GetBook.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, Sim.Sim_ReadToSleep_GetBook.Definition>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                if ((Book == null) || Book.HasBeenDestroyed)
                {
                    return false;
                }

                Book.AddToReferenceList(Actor);
                mAddedToReferenceList = true;
                InteractionDefinition readToSleepSingleton = null;
                InteractionDefinition beReadToSleepSingleton = null;

                // Custom
                IReadToSleepObject myBed = Target.Bed as IReadToSleepObject;
                if (myBed != null)
                {
                    if (myBed.TestReadToSleep(Actor, Target))
                    {
                        readToSleepSingleton = myBed.GetReadToSleepSingleton();
                        beReadToSleepSingleton = myBed.GetBeReadToSleepSingleton();
                    }
                }

                if (beReadToSleepSingleton == null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        foreach (IReadToSleepObject obj2 in Target.LotCurrent.GetObjects<IReadToSleepObject>())
                        {
                            if ((i == 0) && (myBed != null))
                            {
                                if (myBed.RoomId != obj2.RoomId) continue;
                            }

                            if (obj2.TestReadToSleep(Actor, Target))
                            {
                                readToSleepSingleton = obj2.GetReadToSleepSingleton();
                                beReadToSleepSingleton = obj2.GetBeReadToSleepSingleton();
                                break;
                            }
                        }

                        if (beReadToSleepSingleton != null) break;
                    }
                }

                if ((readToSleepSingleton == null) && (beReadToSleepSingleton == null))
                {
                    return false;
                }
                InteractionInstance entry = null;
                if (beReadToSleepSingleton != null)
                {
                    entry = beReadToSleepSingleton.CreateInstanceWithCallbacks(Actor, Target, GetPriority(), Autonomous, CancellableByPlayer, new Callback(OnChildStarted), new Callback(OnChildCompleted), new Callback(OnChildFailed));
                    entry.LinkedInteractionInstance = this;
                    if (!Target.InteractionQueue.Add(entry))
                    {
                        return false;
                    }
                }
                if (Book.InInventory)
                {
                    if (!Actor.Inventory.Contains(Book))
                    {
                        IGameObject closestBookshelf;
                        Bookshelf_GetBook.Definition definition3 = new Bookshelf_GetBook.Definition(Book);
                        if (kAlwaysUseClosestBookshelf && (ClosestBookshelf != null))
                        {
                            closestBookshelf = ClosestBookshelf;
                        }
                        else
                        {
                            closestBookshelf = Book.ItemComp.InventoryParent.Owner;
                        }
                        if (!definition3.CreateInstance(closestBookshelf, Actor, GetPriority(), Autonomous, CancellableByPlayer).RunInteraction())
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (!CarrySystem.PickUp(Actor, Book))
                    {
                        return false;
                    }
                    if (!CarrySystem.PutInSimInventory(Actor))
                    {
                        return false;
                    }
                }

                if (!Actor.Inventory.Contains(Book))
                {
                    return false;
                }

                if (Actor.HasExitReason(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached)))
                {
                    return false;
                }

                LinkedInteractionInstance = null;
                InteractionInstance instance = readToSleepSingleton.CreateInstance(Target, Actor, GetPriority(), Autonomous, CancellableByPlayer);
                instance.LinkedInteractionInstance = entry;
                (instance as IReadToSleepInteraction).ReservedBook = Book;
                return Actor.InteractionQueue.PushAsContinuation(instance, true);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        private new class Definition : Sim.Sim_ReadToSleep_GetBook.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new Sim_ReadToSleep_GetBookEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
