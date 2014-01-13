using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.RelativitySpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Options.RelativeMotives
{
    public class RelativeMotive : BooleanSettingOption<GameObject>, IRelativeMotivesOption
    {
        CommodityKind mMotive;

        public RelativeMotive(CommodityKind motive)
        {
            mMotive = motive;
        }

        public override string GetTitlePrefix()
        {
            return "RelativeMotives";
        }

        public override string Name
        {
            get { return CommoditiesEx.GetMotiveLocalizedName(mMotive); }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new ListingOption(); }
        }

        protected override bool Value
        {
            get
            {
                return Relativity.Settings.mRelativeMotives.ContainsKey(mMotive);
            }
            set
            {
                Relativity.Settings.mRelativeMotives.Remove(mMotive);

                if (value)
                {
                    Relativity.Settings.mRelativeMotives.Add(mMotive, true);
                }
            }
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            OptionResult result = base.Run(parameters);
            if (result != OptionResult.Failure)
            {
                PriorValues.sFactorChanged = true;
            }

            return result;
        }
    }
}
