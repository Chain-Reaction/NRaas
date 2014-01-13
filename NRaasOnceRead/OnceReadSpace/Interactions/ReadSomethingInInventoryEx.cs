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
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Interactions
{
    public class ReadSomethingInInventoryEx : Sim.ReadSomethingInInventory, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.ReadSomethingInInventory.Definition, Definition>(false);

            sOldSingleton = Singleton as InteractionDefinition;
            Singleton = new Definition();
        }

        public static Book ChooseBook(Sim actor, List<Book> books)
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

                if (ReadBookData.HasSimFinishedBook(actor, book.Data.ID)) continue;

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

        private new bool DoReadBook()
        {
            Book target = ChooseBook(Actor, Inventories.QuickFind<Book>(Actor.Inventory));
            if (target != null)
            {
                InteractionInstance instance = ReadBookChooserEx.Singleton.CreateInstance(target, Actor, mPriority, Autonomous, CancellableByPlayer);
                BeginCommodityUpdates();

                bool succeeded = false;
                try
                {
                    succeeded = instance.RunInteraction();
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                return succeeded;
            }

            Target.AddExitReason(ExitReason.FailedToStart);
            return false;
        }

        public override bool Run()
        {
            try
            {
                if (!Actor.Posture.Satisfies(CommodityKind.Relaxing, null) && (Actor.Motives.HasMotive(CommodityKind.BeSuspicious) || !RandomUtil.RandomChance(Sim.ChanceOfReadingBookRatherThanNewsaperWhenReadingOutdoors)))
                {
                    return DoReadNewspaper();
                }
                return DoReadBook();
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

        public new class Definition : Sim.ReadSomethingInInventory.Definition
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new ReadSomethingInInventoryEx();
                na.Init(ref parameters);
                return na;
            }
        }

        public class CustomInjector : Common.InteractionInjector<Sim>
        {
            public CustomInjector()
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                Sim sim = obj as Sim;
                if (sim != null)
                {
                    sim.AddSoloInteraction(Singleton);
                    return true;
                }

                return false;
            }
        }
    }
}


