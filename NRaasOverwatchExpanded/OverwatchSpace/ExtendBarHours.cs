using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Appliances;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Preload
{
    public class ExtendBarHours : PreloadOption
    {
        static List<Lot.MetaAutonomyType> sTypes = new List<Lot.MetaAutonomyType>();

        static ExtendBarHours()
        {
            sTypes.Add(Lot.MetaAutonomyType.CocktailLoungeAsian);
            sTypes.Add(Lot.MetaAutonomyType.CocktailLoungeCelebrity);
            sTypes.Add(Lot.MetaAutonomyType.CocktailLoungeVampire);
            sTypes.Add(Lot.MetaAutonomyType.DiveBarCriminal);
            sTypes.Add(Lot.MetaAutonomyType.DiveBarIrish);
            sTypes.Add(Lot.MetaAutonomyType.DiveBarSports);
            sTypes.Add(Lot.MetaAutonomyType.DanceClubLiveMusic);
            sTypes.Add(Lot.MetaAutonomyType.DanceClubPool);
            sTypes.Add(Lot.MetaAutonomyType.DanceClubRave);
        }
        public ExtendBarHours()
        { }

        public override string GetTitlePrefix()
        {
            return "ExtendBarHours";
        }

        public override void OnPreLoad()
        {
            try
            {
                Overwatch.Log("Try Extend Hours");

                foreach(Lot.MetaAutonomyType type in sTypes)
                {
                    Bartending.BarData data;
                    if (!Bartending.sData.TryGetValue(type, out data)) continue;

                    data.mHourClose += 2;

                    Overwatch.Log("Hours Extended " + type);
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }
    }
}
