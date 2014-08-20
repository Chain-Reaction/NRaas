using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.CAS;
using NRaas.MasterControllerSpace.Sims.Basic;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class CopyClothing : DualSimFromList, IIntermediateOption
    {
        static List<BodyTypes> sTypes = new List<BodyTypes>();

        Dictionary<OutfitCategories, Dictionary<int, bool>> mTransfers = new Dictionary<OutfitCategories, Dictionary<int, bool>>();

        bool mAdd = false;

        static CopyClothing()
        {
            sTypes.Add(BodyTypes.Accessories);
            sTypes.Add(BodyTypes.Armband);
            sTypes.Add(BodyTypes.Bracelet);
            sTypes.Add(BodyTypes.Earrings);
            sTypes.Add(BodyTypes.FullBody);
            sTypes.Add(BodyTypes.Glasses);
            sTypes.Add(BodyTypes.Gloves);
            sTypes.Add(BodyTypes.LeftEarring);
            sTypes.Add(BodyTypes.LeftGarter);
            sTypes.Add(BodyTypes.LowerBody);
            sTypes.Add(BodyTypes.Necklace);
            sTypes.Add(BodyTypes.NoseRing);
            sTypes.Add(BodyTypes.RightEarring);
            sTypes.Add(BodyTypes.RightGarter);
            sTypes.Add(BodyTypes.Ring);
            sTypes.Add(BodyTypes.Shoes);
            sTypes.Add(BodyTypes.Socks);
            sTypes.Add(BodyTypes.UpperBody);
        }

        public override string GetTitlePrefix()
        {
            return "CopyClothing";
        }

        protected override int GetMaxSelectionB(IMiniSimDescription sim)
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override string GetTitleA()
        {
            return Common.Localize("Copy:Source");
        }

        protected override string GetTitleB()
        {
            return Common.Localize("Copy:Destination");
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.GetOutfit(OutfitCategories.Everyday, 0) == null) return false;

            if (me.IsUsingMaternityOutfits) return false;

            if (SimTypes.IsSkinJob(me)) return false;

            if (me.Household == null) return false;

            if (me.Household == Household.ActiveHousehold) return true;

            return (!SimTypes.IsTourist(me));
        }

        protected override bool PrivateAllow(SimDescription a, SimDescription b)
        {
            if (!base.PrivateAllow(a, b)) return false;

            if (a.Gender != b.Gender) return false;

            switch (a.Age)
            {
                case CASAgeGenderFlags.YoungAdult:
                case CASAgeGenderFlags.Adult:
                    switch (b.Age)
                    {
                        case CASAgeGenderFlags.YoungAdult:
                        case CASAgeGenderFlags.Adult:
                            return true;
                    }
                    break;
            }

            return (a.Age == b.Age);
        }

        protected override bool Run(SimDescription a, SimDescription b)
        {
            SavedOutfit.Cache cache = new SavedOutfit.Cache(a);

            if (!ApplyAll)
            {
                mAdd = false;

                List<ChangeOutfit.Item> allOptions = new List<ChangeOutfit.Item>();
                allOptions.Add(new ChangeOutfit.Item(new CASParts.Key(OutfitCategories.None, 0)));

                foreach (SavedOutfit.Cache.Key outfit in cache.Outfits)
                {
                    switch (outfit.Category)
                    {
                        case OutfitCategories.Everyday:
                        case OutfitCategories.Formalwear:
                        case OutfitCategories.Sleepwear:
                        case OutfitCategories.Swimwear:
                        case OutfitCategories.Athletic:
                        case OutfitCategories.Career:
                        case OutfitCategories.Outerwear:
                        case OutfitCategories.MartialArts:
                            allOptions.Add(new ChangeOutfit.Item(outfit.mKey, a));
                            break;
                    }
                }

                CommonSelection<ChangeOutfit.Item>.Results choices = new CommonSelection<ChangeOutfit.Item>(Name, allOptions).SelectMultiple();
                if ((choices == null) || (choices.Count == 0)) return false;

                mTransfers.Clear();
                foreach (ChangeOutfit.Item choice in choices)
                {
                    if (choice.Category == OutfitCategories.None)
                    {
                        mTransfers.Clear();
                        break;
                    }

                    Dictionary<int, bool> indices;
                    if (!mTransfers.TryGetValue(choice.Category, out indices))
                    {
                        indices = new Dictionary<int, bool>();
                        mTransfers.Add(choice.Category, indices);
                    }

                    indices.Add(choice.Index, true);
                }

                if (TwoButtonDialog.Show(
                    Common.Localize(GetTitlePrefix() + ":Prompt", a.IsFemale, b.IsFemale, new object[] { a, b }),
                    Common.Localize(GetTitlePrefix() + ":Add"),
                    Common.Localize(GetTitlePrefix() + ":Replace")
                ))
                {
                    mAdd = true;
                }
            }

            SimOutfit geneOutfit = CASParts.GetOutfit(b, CASParts.sPrimary, false);

            foreach (SavedOutfit.Cache.Key outfit in cache.Outfits)
            {
                bool transfer = false;
                if (mTransfers.Count == 0)
                {
                    transfer = true;
                }
                else
                {
                    Dictionary<int, bool> indices;
                    if (mTransfers.TryGetValue(outfit.Category, out indices))
                    {
                        if (indices.ContainsKey(outfit.Index))
                        {
                            transfer = true;
                        }
                    }
                }

                if (!transfer) continue;

                int newIndex = -1;
                if (!mAdd)
                {
                    newIndex = outfit.Index;
                }

                using (CASParts.OutfitBuilder builder = new CASParts.OutfitBuilder(b, new CASParts.Key(outfit.Category, newIndex), geneOutfit))
                {
                    outfit.Apply(builder, false, sTypes, null);
                }
            }

            SpeedTrap.Sleep();

            if (b.CreatedSim != null)
            {
                b.CreatedSim.RefreshCurrentOutfit(false);
            }

            return true;
        }
    }
}
