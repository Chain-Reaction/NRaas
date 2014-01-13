using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Interactions
{
    public class ReadToddlerBookEx : Common.IPreLoad, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<BookToddler, BookToddler_ReadWithMenu.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<BookToddler, BookToddler_ReadWithMenu.Definition, Definition>(false);

            BookToddler_ReadWithMenu.Singleton = Singleton;
        }

        public class Definition : BookToddler_ReadWithMenu.Definition
        {
            public override void AddInteractions(InteractionObjectPair iop, Sim actor, BookToddler target, List<InteractionObjectPair> results)
            {
                if (Test(actor))
                {
                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (!sim.SimDescription.Toddler) continue;

                        if (ReadBookData.HasSimFinishedBook(sim, target.Data.ID)) continue;

                        results.Add(new InteractionObjectPair(new BookToddler_ReadWithMenu.SubDefinition(sim), target));
                    }
                }
            }
        }
    }
}


