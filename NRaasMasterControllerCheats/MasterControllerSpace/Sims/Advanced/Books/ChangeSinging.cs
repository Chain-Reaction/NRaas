using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Books
{
    public class ChangeSinging : SimFromList, IBooksOption
    {
        CommonSelection<Item>.Results mSelection = null;

        public override string GetTitlePrefix()
        {
            return "ChangeSongs";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.Singing == null) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<Item> allOptions = new List<Item>();

                allOptions.Add(null);

                foreach (SingingInfo.SingingComposition song in SingingInfo.SingingCompositions)
                {
                    allOptions.Add(new Item(song, me.Singing.mKnownCompositions.Contains(song) ? 1 : 0));
                }

                foreach (SingingInfo.SingingComposition song in SingingInfo.SingingRomanticCompositions)
                {
                    allOptions.Add(new Item(song, me.Singing.mKnownRomanticCompositions.Contains(song) ? 1 : 0));
                }

                mSelection = new CommonSelection<Item>(Name, me.FullName, allOptions).SelectMultiple();
                if ((mSelection == null) || (mSelection.Count == 0)) return false;

                CommonSelection<Item>.HandleAllOrNothing(mSelection);
            }

            foreach (Item item in mSelection)
            {
                if (item == null) continue;

                if (item.Value.IsRomanticTrack)
                {
                    if (me.Singing.mKnownRomanticCompositions.Contains(item.Value))
                    {
                        if (!item.IsSet) continue;

                        me.Singing.mKnownRomanticCompositions.Remove(item.Value);
                    }
                    else
                    {
                        if (item.IsSet) continue;

                        me.Singing.mKnownRomanticCompositions.Add(item.Value);
                    }
                }
                else
                {
                    if (me.Singing.mKnownCompositions.Contains(item.Value))
                    {
                        if (!item.IsSet) continue;

                        me.Singing.mKnownCompositions.Remove(item.Value);
                    }
                    else
                    {
                        if (item.IsSet) continue;

                        me.Singing.mKnownCompositions.Add(item.Value);
                    }
                }
            }

            return true;
        }

        protected class Item : ValueSettingOption<SingingInfo.SingingComposition>
        {
            public Item(SingingInfo.SingingComposition song, int count)
                : base(song, song.Name, count)
            { }

            public override string DisplayKey
            {
                get { return "Knows"; }
            }
        }
    }
}
