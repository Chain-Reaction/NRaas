using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class LongTermRelationshipEx : SelectionTestableOptionList<LongTermRelationshipEx.Item, string, LongTermRelationshipTypes>
    {
        public override string GetTitlePrefix()
        {
            return "Criteria.LongTermRelationship";
        }

        public class Item : TestableOption<string, LongTermRelationshipTypes>
        {
            public override bool Get(SimDescription me, IMiniSimDescription actor, Dictionary<LongTermRelationshipTypes, string> results)
            {
                SimDescription actorSim = actor as SimDescription;
                if (actorSim == null) return false;

                Relationship relation = Relationship.Get(me, actorSim, false);
                if (relation == null) return false;

                LTRData data = LTRData.Get(relation.CurrentLTR);
                if (data != null)
                {
                    results[relation.CurrentLTR] = data.GetName(me, actor);
                }

                return true;
            }

            public override bool Get(MiniSimDescription me, IMiniSimDescription actor, Dictionary<LongTermRelationshipTypes, string> results)
            {
                IMiniRelationship relation = me.GetMiniRelationship(actor);
                if (relation == null) return false;

                LTRData data = LTRData.Get(relation.CurrentLTR);
                if (data != null)
                {
                    results[relation.CurrentLTR] = data.GetName(me, actor);
                }

                return true;
            }

            public override void SetValue(string dataType, LongTermRelationshipTypes storeType)
            {
                mValue = storeType;

                mName = dataType;
            }
        }
    }
}
