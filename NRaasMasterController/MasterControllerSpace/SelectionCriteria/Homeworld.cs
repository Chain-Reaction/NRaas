using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class Homeworld : SelectionTestableOptionList<Homeworld.Item, WorldName, WorldName>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.Homeworld";
        }

        public override bool IsSpecial
        {
            get { return true; }
        }

        public class Item : TestableOption<WorldName, WorldName>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<WorldName, WorldName> results)
            {
                results[me.HomeWorld] = me.HomeWorld;
                return true;
            }
            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<WorldName, WorldName> results)
            {
                results[me.HomeWorld] = me.HomeWorld;
                return true;
            }

            public override void SetValue(WorldName value, WorldName storeType)
            {
                mValue = value;

                mName = GetName(value);
            }

            public static string GetName(WorldName world)
            {
                string homeWorld = Common.LocalizeEAString("Ui/Caption/Global/WorldName/EP01:" + world.ToString());
                if (!homeWorld.Contains("****"))
                {
                    return homeWorld;
                }
                else if (Common.kDebugging) 
                {
                    return world.ToString();
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
