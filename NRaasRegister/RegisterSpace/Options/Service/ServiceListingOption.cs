using NRaas.CommonSpace.Options;
using NRaas.RegisterSpace.Options.Service;
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
using Sims3.Gameplay.Services;
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
    public class ServiceListingOption : InteractionOptionList<IServiceOption, GameObject>
    {
        Service mData;        

        public ServiceListingOption(Service data)
            : base(Common.LocalizeEAString("Ui/Caption/Services/Service:" + data.ServiceType.ToString()))
        {
            mData = data;
        }        

        public override string GetTitlePrefix()
        {
            return null;
        }        

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        public override List<IServiceOption> GetOptions()
        {
            List<IServiceOption> results = new List<IServiceOption>();

            results.Add(new AgeSetting(mData));
            results.Add(new PoolSetting(mData));
            results.Add(new CostSetting(mData));
            results.Add(new ReoccurrenceSetting(mData));
            if (GameUtils.IsFutureWorld())
            {
                results.Add(new UseBotsSetting(mData));
            }

            return results;
        }
    }
}