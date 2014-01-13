using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
    public class AntiMagicScroll : Common, Common.IWorldLoadFinished
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

        static AntiMagicScroll()
        {
            Bootstrap();
        }

        public void OnWorldLoadFinished()
        {
            new Common.DelayedEventListener(EventTypeId.kLearnedRecipe, OnLearnRecipe);
            new Common.DelayedEventListener(EventTypeId.kLearnedComposition, OnLearnComposition);
            new Common.DelayedEventListener(EventTypeId.kLearnAlchemyObject, OnLearnAlchemyObject);
        }

        protected static void OnLearnAlchemyObject(Event paramE)
        {
            AlchemyObjectEvent e = paramE as AlchemyObjectEvent;
            if (e != null)
            {
                Sim sim = e.Actor as Sim;
                if (sim != null)
                {
                    foreach (BookAlchemyRecipeData data in BookData.BookAlchemyRecipeDataList.Values)
                    {
                        if (data.AlchemyRecipeKey == e.AlchemyRecipeKey)
                        {
                            Inventories.TryToMove(BookAlchemyRecipe.CreateOutOfWorld(data), sim);
                        }
                    }
                }
            }
        }

        protected static void OnLearnRecipe(Event paramE)
        {
            LearnRecipeEvent e = paramE as LearnRecipeEvent;
            if (e != null)
            {
                Sim sim = e.Actor as Sim;
                if ((sim != null) && (!sim.IsEP11Bot))
                {
                    foreach (BookRecipeData data in BookData.BookRecipeDataList.Values)
                    {
                        if (data.Recipe == e.Recipe.Key)
                        {
                            Inventories.TryToMove(BookRecipe.CreateOutOfWorld(data), sim);
                        }
                    }
                }
            }
        }

        protected static void OnLearnComposition(Event e)
        {
            Sim sim = e.Actor as Sim;
            if (sim != null)
            {
                SheetMusic music = e.TargetObject as SheetMusic;
                if (music != null)
                {
                    SheetMusic newMusic = music.Copy(false) as SheetMusic;
                    if (newMusic != null)
                    {
                        Inventories.TryToMove(newMusic, sim);
                    }
                }
            }
        }
    }
}
