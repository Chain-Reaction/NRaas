using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class RemoveMemories : SimFromList, IIntermediateOption
    {
        Item mItem = null;

        public override string GetTitlePrefix()
        {
            return "RemoveMemories";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.LifeEventManager == null) return false;

            if (me.LifeEventManager.mCurrentNumberOfVisibleLifeEvents <= 0) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                Item choice = new CommonSelection<Item>(Name, me.FullName, Common.DerivativeSearch.Find<Item>()).SelectSingle();
                if (choice == null) return false;

                if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { choice.Name })))
                {
                    return false;
                }

                mItem = choice;
            }

            return mItem.Perform(me.LifeEventManager);
        }

        public static bool HasSnapshot(LifeEventManager.LifeEvent e)
        {
            if (!e.HasSnapshot) return false;

            UIImage image = UIManager.LoadUIImage(e.Snapshot);
            return (image != null);
        }

        public abstract class Item : SelectionOptionBase
        {
            public Item()
            { }
            public Item(string name)
                : base(name, -1)
            { }

            protected override bool Allow(SimDescription me, IMiniSimDescription actor)
            {
                if (me.LifeEventManager == null) return false;

                return Allow(me.LifeEventManager);
            }

            protected abstract bool Allow(LifeEventManager manager);

            public abstract bool Perform(LifeEventManager manager);
        }

        public class AllItem : Item
        {
            public AllItem()
                : base(Common.LocalizeEAString("Ui/Caption/ObjectPicker:All"))
            { }

            public override string GetTitlePrefix()
            {
                return null;
            }

            protected override bool Allow(LifeEventManager manager)
            {
                return (manager.mCurrentNumberOfVisibleLifeEvents > 0);
            }

            public override bool Perform(LifeEventManager manager)
            {
                foreach (LifeEventManager.LifeEvent e in manager.GetVisibleLifeEvents())
                {
                    e.Forget(manager.mOwnerDescription);
                }
                return true;
            }
        }

        public class NoPhotosItem : Item
        {
            public NoPhotosItem()
                : base(Common.Localize("Memories:NoPhotos"))
            { }

            public override string GetTitlePrefix()
            {
                return null;
            }

            protected override bool Allow(LifeEventManager manager)
            {
                foreach (LifeEventManager.LifeEvent e in manager.GetVisibleLifeEvents())
                {
                    if (e.HasSnapshot) return true;
                }

                return false;
            }

            public override bool Perform(LifeEventManager manager)
            {
                foreach (LifeEventManager.LifeEvent e in manager.GetVisibleLifeEvents())
                {
                    if (HasSnapshot(e)) continue;

                    e.Forget(manager.mOwnerDescription);
                }
                return true;
            }
        }

        public class DuplicatesItem : Item
        {
            public DuplicatesItem()
               : base(Common.Localize("Memories:Duplicates"))
            { }

            public override string GetTitlePrefix()
            {
                return null;
            }

            protected override bool Allow(LifeEventManager manager)
            {
                return (manager.mCurrentNumberOfVisibleLifeEvents > 0);
            }

            public static int OnSort(LifeEventManager.LifeEvent left, LifeEventManager.LifeEvent right)
            {
                try
                {
                    return left.Timestamp.CompareTo(right.Timestamp);
                }
                catch (Exception e)
                {
                    Common.Exception("OnSort", e);
                    return 0;
                }
            }

            public override bool Perform(LifeEventManager manager)
            {
                Dictionary<string, List<LifeEventManager.LifeEvent>> lookup = new Dictionary<string, List<LifeEventManager.LifeEvent>>();

                foreach (LifeEventManager.LifeEvent e in manager.GetVisibleLifeEvents())
                {
                    string name = null;

                    try
                    {
                        name = e.GetLocalizedName(manager.mOwnerDescription);
                    }
                    catch (Exception exception)
                    {
                        Common.Exception(manager.mOwnerDescription, exception);
                    }
                    if (string.IsNullOrEmpty(name)) continue;

                    List<LifeEventManager.LifeEvent> list;
                    if (!lookup.TryGetValue(name, out list))
                    {
                        list = new List<LifeEventManager.LifeEvent>();
                        lookup.Add(name, list);
                    }

                    list.Add(e);
                }

                foreach (List<LifeEventManager.LifeEvent> list in lookup.Values)
                {
                    list.Sort(OnSort);

                    LifeEventManager.LifeEvent source = null;

                    foreach (LifeEventManager.LifeEvent e in list)
                    {
                        if ((source == null) || (HasSnapshot(e)))
                        {
                            source = e;
                        }
                        else if (source.Subject == e.Subject)
                        {
                            e.Forget(manager.mOwnerDescription);
                        }
                    }
                }

                return true;
            }
        }
    }
}
