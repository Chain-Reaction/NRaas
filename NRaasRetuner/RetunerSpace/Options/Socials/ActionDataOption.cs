using NRaas.CommonSpace.Options;
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

namespace NRaas.RetunerSpace.Options.Socials
{
    public class ActionDataOption : InteractionOptionList<ISocialOption, GameObject>
    {
        ActionData mData;

        bool mAppendKey;

        public ActionDataOption(ActionData data)
            : base(string.IsNullOrEmpty(data.ActionText) ? data.Key : Common.LocalizeEAString(data.ActionText))
        {
            mData = data;
        }

        public void AppendKey()
        {
            mAppendKey = true;
        }

        public override string GetTitlePrefix()
        {
            return null;
        }

        public override string Name
        {
            get
            {
                string name = base.Name;

                if (mAppendKey)
                {
                    name += " (" + mData.Key + ")";
                }

                return name;
            }
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<ISocialOption> GetOptions()
        {
            List<ISocialOption> results = new List<ISocialOption>();

            results.Add(new AllowAutonomous(mData));
            results.Add(new UserDirectedOnly(mData));
            results.Add(new AllowPregnant(mData));
            results.Add(new ActorAgeSpecies(mData));
            results.Add(new TargetAgeSpecies(mData));

            return results;
        }
    }
}
