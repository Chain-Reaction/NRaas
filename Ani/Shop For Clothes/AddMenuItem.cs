using Sims3.SimIFace;
using System;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.ShelvesStorage;
using Sims3.Gameplay.Objects.Decorations.Mimics;
using Sims3.Gameplay.EventSystem;
using System.Collections.Generic;
using Sims3.Gameplay.Autonomy;

namespace ani_ShopForClothes
{
   public class AddMenuItem
{
    // Fields
    [Tunable]
    protected static bool Nuuttis;

    // Methods
    static AddMenuItem()
    {
        World.sOnWorldLoadFinishedEventHandler += new EventHandler(AddMenuItem.OnWorldLoadFinishedHandler);
        World.sOnLotAddedEventHandler += new EventHandler(AddMenuItem.OnWorldLoadFinishedHandler);
    }

    public static void AddInteractions(GameObject obj)
    {
        obj.RemoveAllInteractions();
        if ((obj != null) && (obj.Interactions != null))
        {
            obj.AddInteraction(ShoppingInteraction.ShopForClothes.Singleton);
            obj.AddInteraction(ShoppingInteraction.SuggestOutfit.Singleton);
        }
    }

    protected static ListenerAction OnNewObject(Event e)
    {
        Dresser targetObject = e.TargetObject as Dresser;
        if (targetObject != null)
        {
            RemoveInteraction(targetObject);
        }
        else
        {
            SculptureFloorClothingRack2x1 rackx = e.TargetObject as SculptureFloorClothingRack2x1;
            if (rackx != null)
            {
                AddInteractions(rackx);
            }
        }
        return ListenerAction.Keep;
    }

    public static void OnWorldLoadFinishedHandler(object sender, EventArgs e)
    {
        List<Dresser> list = new List<Dresser>(Sims3.Gameplay.Queries.GetObjects<Dresser>());
        foreach (Dresser dresser in list)
        {
            RemoveInteraction(dresser);
        }
        List<SculptureFloorClothingRack2x1> list2 = new List<SculptureFloorClothingRack2x1>(Sims3.Gameplay.Queries.GetObjects<SculptureFloorClothingRack2x1>());
        foreach (SculptureFloorClothingRack2x1 rackx in list2)
        {
            AddInteractions(rackx);
        }
        EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(AddMenuItem.OnNewObject));
    }

    public static void RemoveInteraction(GameObject obj)
    {
        if ((obj != null) && (obj.Interactions != null))
        {
            foreach (InteractionObjectPair pair in obj.Interactions)
            {
                if (pair.ToString().Contains("CreateAnOutfit"))
                {
                    obj.Interactions.Remove(pair);
                    break;
                }
            }
        }
    }
}


}
