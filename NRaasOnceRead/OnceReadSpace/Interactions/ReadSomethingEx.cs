using NRaas.CommonSpace.Helpers;
using NRaas.OnceReadSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Interactions
{
    public class ReadSomethingEx : Bookshelf_ReadSomething, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Bookshelf, Bookshelf_ReadSomething.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Bookshelf, Bookshelf_ReadSomething.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public static IGameObject ChooseBook(Sim actor, List<IGameObject> books)
        {
            if (books.Count == 0x0)
            {
                return null;
            }

            float minValue = float.MinValue;
            List<Book> list = new List<Book>();
            foreach (IGameObject obj in books)
            {
                Book book = obj as Book;
                if (book == null) continue;

                ReadBookData data;
                if (actor.ReadBookDataList.TryGetValue(book.Data.ID, out data))
                {
                    if (data.TimesRead > 0) continue;
                }

                float interestInBook = BookEx.GetInterestInBook(actor, book);
                if (interestInBook == 0f) continue;

                if (interestInBook > minValue)
                {
                    list.Clear();
                    list.Add(book);
                    minValue = interestInBook;
                }
                else if (interestInBook == minValue)
                {
                    list.Add(book);
                }
            }

            if (list.Count == 0x0)
            {
                return null;
            }

            return list[RandomUtil.GetInt(list.Count - 0x1)];
        }

        public override bool Run()
        {
            try
            {
                if (!Target.Line.WaitForTurn(this, SimQueue.WaitBehavior.Default, ~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached), Bookshelf.kTimeToWaitInLine))
                {
                    return false;
                }

                try
                {
                    if (!Actor.RouteToSlot(Target, Slots.Hash("Route")))
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

                IGameObject result = null;
                List<IGameObject> topStackItems = Target.Inventory.GetTopStackItems();
                result = ChooseBook(Actor, topStackItems);
                if (result == null)
                {
                    Actor.AddExitReason(ExitReason.FailedToRemoveFromInventory);
                    return false;
                }
                return Target.Read_GetBook(Actor, result, this);
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

        public new class Definition : Bookshelf_ReadSomething.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ReadSomethingEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, Bookshelf target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Bookshelf target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                bool found = false;

                List<IGameObject> topStackItems = target.Inventory.GetTopStackItems();
                foreach (IGameObject obj in topStackItems)
                {
                    Book book = obj as Book;
                    if (book == null) continue;

                    if ((book is BookGeneral) || (book is BookWritten))
                    {
                        ReadBookData data;
                        if (a.ReadBookDataList.TryGetValue(book.Data.ID, out data))
                        {
                            if (data.TimesRead == 0)
                            {
                                found = true;
                                break;
                            }
                        }
                        else
                        {
                            found = true;
                            break;
                        }
                    }
                }

                return found;
            }
        }
    }
}


