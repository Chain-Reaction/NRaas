using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NRaas.DresserSpace.Options.Sims
{
    public class AddOutfit : OperationSettingOption<Sim>, ICommonOptionListProxy<AddOutfit>
    {
        public OutfitCategories mCategory;

        public AddOutfit()
        { }
        protected AddOutfit(OutfitCategories category)
        {
            mCategory = category;
        }

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!parameters.mTarget.IsHuman) return false;

            return base.Allow(parameters);
        }

        protected static OptionResult ApplyOutfit(SimDescription sim, OutfitCategories category)
        {
            ResourceKey newOutfitKey = ResourceKey.kInvalidResourceKey;
            if (sim.IsMummy)
            {
                newOutfitKey = OccultMummy.CreateMummyUniform(sim.Age, sim.Gender).Key;
            }
            else if (sim.IsFrankenstein)
            {
                newOutfitKey = OccultBaseClass.CreateUniform(OccultFrankenstein.CreateUniformName(sim, sim.OccultManager.mIsLifetimeReward), sim.Age, ProductVersion.EP2, CASSkinTones.NoSkinTone, 0f).Key;
            }
            else if (sim.IsImaginaryFriend)
            {
                OccultImaginaryFriend friend = sim.OccultManager.GetOccultType(OccultTypes.ImaginaryFriend) as OccultImaginaryFriend;

                string name;
                newOutfitKey = OccultImaginaryFriend.CreateImaginaryFriendUniform(sim.Age, friend.Pattern, out name).Key;
            }
            else
            {
                SimOutfit source = null;
                if (sim.GetOutfitCount(OutfitCategories.Naked) > 0)
                {
                    source = sim.GetOutfit(OutfitCategories.Naked, 0);
                }
                else if (sim.GetOutfitCount(category) > 0)
                {
                    source = sim.GetOutfit(category, 0);
                }

                if (source != null)
                {
                    using (SimBuilder builder = new SimBuilder())
                    {
                        OutfitUtils.SetOutfit(builder, source, sim);

                        OutfitUtils.MakeCategoryAppropriate(builder, category, sim);

                        newOutfitKey = builder.CacheOutfit(CASParts.GetOutfitName(sim, category, sim.IsUsingMaternityOutfits));
                    }
                }
            }

            if (newOutfitKey == ResourceKey.kInvalidResourceKey) return OptionResult.Failure;

            sim.AddOutfit(new SimOutfit(newOutfitKey), category, false);

            int index = sim.GetOutfitCount(category) - 1;

            ArrayList list = sim.GetCurrentOutfits()[category] as ArrayList;

            object a = list[0];
            object b = list[index];

            list[0] = b;
            list[index] = a;

            return OptionResult.SuccessClose;
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            OptionResult result = OptionResult.Failure;

            if (mCategory == OutfitCategories.PrimaryCategories)
            {
                List<AddOutfit> outfits = new List<AddOutfit> ();
                GetOptions(outfits);

                foreach (AddOutfit outfit in outfits)
                {
                    if (outfit.mCategory == OutfitCategories.PrimaryCategories) continue;

                    ApplyOutfit(parameters.mTarget.SimDescription, outfit.mCategory);
                }

                result = OptionResult.SuccessClose;
            }
            else
            {
                result = ApplyOutfit(parameters.mTarget.SimDescription, mCategory);

                if (result != OptionResult.Failure)
                {
                    CommonSpace.Helpers.SwitchOutfits.SwitchNoSpin(parameters.mTarget, new CASParts.Key(mCategory, 0));
                }
            }

            return result;
        }

        public void GetOptions(List<AddOutfit> items)
        {
            items.Add(new AddOutfit(OutfitCategories.Everyday));
            items.Add(new AddOutfit(OutfitCategories.Formalwear));
            items.Add(new AddOutfit(OutfitCategories.Sleepwear));
            items.Add(new AddOutfit(OutfitCategories.Swimwear));
            items.Add(new AddOutfit(OutfitCategories.Athletic));
            items.Add(new AddOutfit(OutfitCategories.PrimaryCategories));

            if (GameUtils.IsInstalled(ProductVersion.EP8))
            {
                items.Add(new AddOutfit(OutfitCategories.Outerwear));
            }
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string Name
        {
            get
            {
                if (mCategory == OutfitCategories.PrimaryCategories)
                {
                    return Common.LocalizeEAString("Ui/Caption/ObjectPicker:All");
                }
                else
                {
                    return Common.LocalizeEAString("Ui/Caption/ObjectPicker:" + mCategory);
                }
            }
        }

        public class ListingOption : InteractionOptionList<AddOutfit, Sim>, ISimOption
        {
            protected override int NumSelectable
            {
                get
                {
                    return 0;
                }
            }

            public override string GetTitlePrefix()
            {
                return "AddOutfit";
            }

            public override ITitlePrefixOption ParentListingOption
            {
                get { return null; }
            }
        }
    }
}


