using Sims3.Store.Objects;
using ani_StoreSetBase;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.SimIFace;
using System;
using Sims3.Gameplay.Abstracts;
using System.Collections.Generic;
using Sims3.Gameplay.Objects.Vehicles;
using ani_StoreSetRegister;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.TombObjects.ani_StoreRestockItem;
using ani_StoreRestockItem;
using Sims3.Gameplay.Utilities;

namespace Sims3.Gameplay.Objects.TombObjects.ani_StoreSetBase
{
    //   [RuntimeExport]
    public class ani_StoreRug : StoreSetBase
    {
        public override void OnCreation()
        {
            base.OnCreation();
        }

        private List<ObjectGuid> mObjectsWithMyInteraction = new List<ObjectGuid>();

        public override void OnStartup()
        {
            base.OnStartup();
            World.OnObjectPlacedInLotEventHandler -= new EventHandler(this.AddInteractionsToOverlappingObjects);
            World.OnObjectPlacedInLotEventHandler += new EventHandler(this.AddInteractionsToOverlappingObjects);

           // this.AddInteractionsToChildObjects();
            base.AddAlarm(0f, TimeUnit.Seconds, new AlarmTimerCallback(this.AddInteractionsToChildObjects), "Timer to add interactions to rug as soon as the world starts up", AlarmType.NeverPersisted);

        }

        #region Overrides
        public void Cleanup()
        {
            World.OnObjectPlacedInLotEventHandler -= new EventHandler(this.AddInteractionsToOverlappingObjects);
            for (int i = 0; i < this.mObjectsWithMyInteraction.Count; i++)
            {
                GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mObjectsWithMyInteraction[i]);
                gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectPurchaseStub.Singleton);
                gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectBrowseStub.Singleton);
            }
            this.mObjectsWithMyInteraction.Clear();
        }
        public override void Destroy()
        {
            this.Cleanup();
            base.Destroy();
        }
        public override void OnHandToolDeleted()
        {
            this.Cleanup();
            base.OnHandToolDeleted();
        }
        public override void OnHandToolPickup(ePickupTypeHandTool pickupType)
        {
            this.RemoveInteractionsFromChildren();
            base.OnHandToolPickup(pickupType);
        }
        public override void OnHandToolPlacement()
        {
            this.AddInteractionsToChildren();
            base.OnHandToolPlacement();
        }
        public void RemoveInteractionsFromChildren()
        {
            for (int i = 0; i < this.mObjectsWithMyInteraction.Count; i++)
            {
                GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(this.mObjectsWithMyInteraction[i]);
                if (gameObject != null)
                {
                    gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectPurchaseStub.Singleton);
                    gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectBrowseStub.Singleton);
                }
            }
            this.mObjectsWithMyInteraction.Clear();
        }
        public void AddInteractionsToChildren()
        {
            this.RemoveInteractionsFromChildren();
            List<ObjectGuid> objectsICanBuyInDisplay = this.GetObjectIDsICanBuyInDisplay(null, false);
            for (int i = 0; i < objectsICanBuyInDisplay.Count; i++)
            {
                GameObject gameObject = GlobalFunctions.ConvertGuidToObject<GameObject>(objectsICanBuyInDisplay[i]);
                if (gameObject != null)
                {
                    if (StoreHelperClass.AddPurchaseInteraction(null, gameObject, null, true))
                    {
                        gameObject.RemoveAllInteractions();
                        this.mObjectsWithMyInteraction.Add(objectsICanBuyInDisplay[i]);
                        gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectPurchaseStub.Singleton);
                        gameObject.RemoveInteractionByType(StoreSetBase.ChildObjectBrowseStub.Singleton);
                        gameObject.AddInteraction(new StoreSetBase.ChildObjectPurchaseStub.Definition(base.ObjectId));
                        gameObject.AddInteraction(new StoreSetBase.ChildObjectBrowseStub.Definition(base.ObjectId));
                    }
                }
            }
        }
        public override void AddInteractionsToChildObjects()
        {
            this.AddInteractionsToChildren();
        }
        public void AddInteractionsToOverlappingObjects(object sender, EventArgs e)
        {
            World.OnObjectPlacedInLotEventArgs onObjectPlacedInLotEventArgs = e as World.OnObjectPlacedInLotEventArgs;
            if (onObjectPlacedInLotEventArgs == null || onObjectPlacedInLotEventArgs.LotId != base.LotCurrent.LotId)
            {
                return;
            }
            this.AddInteractionsToChildren();
        }
        public override string GetSwipeAnimationName(GameObject target)
        {
            string result = "a2o_object_genericSwipe_x";
            if (target.HeightAboveFloor < 0.1f)
            {
                result = "a_genericSwipe_pickUp_floor_x";
            }
            else
            {
                if (target.HeightAboveFloor < 0.5f)
                {
                    result = "a_genericSwipe_pickUp_coffeeTable_x";
                }
                else
                {
                    if (target.HeightAboveFloor < 1f)
                    {
                        result = "a_genericSwipe_pickUp_table_x";
                    }
                    else
                    {
                        if (target.HeightAboveFloor < 1.5f)
                        {
                            result = "a2o_object_genericSwipe_x";
                        }
                        else
                        {
                            result = "a_genericSwipe_pickUp_counter_x";
                        }
                    }
                }
            }
            return result;
        }
        public override List<ObjectGuid> GetObjectIDsICanBuyInDisplay(Sim actor, bool isAutonomous)
        {
            List<ObjectGuid> list = new List<ObjectGuid>();
            if (base.Charred)
            {
                return list;
            }
            GameObject[] objectsIntersectingObject = Sims3.Gameplay.Queries.GetObjectsIntersectingObject(base.ObjectId);
            if (objectsIntersectingObject != null && objectsIntersectingObject.Length > 0)
            {
                //bool isRug = true;
                //StoreSetBase sBase = RestockItemHelperClass.ReturnStoreSetBase(objectsIntersectingObject[0], out isRug);
                for (int i = 0; i < objectsIntersectingObject.Length; i++)
                {
                    if (StoreHelperClass.AddPurchaseInteraction(actor, objectsIntersectingObject[i], this, isAutonomous))
                    {
                        objectsIntersectingObject[i].RemoveInteractionByType(StoreSetBase.ChildObjectPurchaseStub.Singleton);
                        objectsIntersectingObject[i].RemoveInteractionByType(StoreSetBase.ChildObjectBrowseStub.Singleton);
                        objectsIntersectingObject[i].AddInteraction(new StoreSetBase.ChildObjectPurchaseStub.Definition(base.ObjectId));
                        objectsIntersectingObject[i].AddInteraction(new StoreSetBase.ChildObjectBrowseStub.Definition(base.ObjectId));
                        list.Add(objectsIntersectingObject[i].ObjectId);
                    }

                    BroomStand broomStand = objectsIntersectingObject[i] as BroomStand;
                    if (broomStand != null)
                    {
                        MagicBroom magicBroom = broomStand.GetContainedObject((Slot)2820733094u) as MagicBroom;
                        if (magicBroom != null)// && base.TestIfObjectCanBeBoughtByActor(magicBroom, actor))
                        {
                            list.Add(magicBroom.ObjectId);
                        }
                    }
                }
            }
            return list;
        }
        #endregion Overrides

    }
}
