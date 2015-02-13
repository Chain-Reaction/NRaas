using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.ActorSystems;
using Sims3.SimIFace;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Autonomy;

namespace Alcohol
{
        public class HangoverBuff : Buff
        {
            public class BuffInstanceHangover : BuffInstance
            {
                private List<TraitNames> mTraitsAdded = new List<TraitNames>();
                private List<TraitNames> mTraitsRemoved = new List<TraitNames>();
                public List<TraitNames> TraitsAdded
                {
                    get
                    {
                        return this.mTraitsAdded;
                    }
                }
                public List<TraitNames> TraitsRemoved
                {
                    get
                    {
                        return this.mTraitsRemoved;
                    }
                }
                private BuffInstanceHangover()
                {
                }
                public BuffInstanceHangover(Buff buff, BuffNames buffGuid, int effectValue, float timeoutCount)
                    : base(buff, buffGuid, effectValue, timeoutCount)
                {
                }
                public override BuffInstance Clone()
                {
                    return new HangoverBuff.BuffInstanceHangover(this.mBuff, this.mBuffGuid, this.mEffectValue, this.mTimeoutCount);
                }
            }          

            [TunableComment("Range:  0+  Description:  The fun motive delta, per Sim hour"), Tunable]
            public static float kFunMultiplier = 35f;

            [Tunable, TunableComment("Range:  TraitNames[]  Description:  The traits the Sim temporarily receives when this buff is added")]
            private static TraitNames[] kReceivedTraits = new TraitNames[]
		{
			
		};
            [TunableComment("Range:  TraitNames[]  Description:  The traits that the Sim temporarily loses when this buff is added"), Tunable]
            private static TraitNames[] kRemovedTraits = new TraitNames[]
		{
			
		};
            public HangoverBuff(Buff.BuffData info)
                : base(info)
            {
            }
            public override BuffInstance CreateBuffInstance()
            {
                return new HangoverBuff.BuffInstanceHangover(this, base.BuffGuid, base.EffectValue, base.TimeoutSimMinutes);
            }

            public override void OnTimeout(BuffManager bm, BuffInstance bi, Buff.OnTimeoutReasons reason)
            {
                if (bm.Actor.RabbitHoleCurrent != null)
                {
                    return;
                }
                if (bm.Actor.Occupation != null && bm.Actor.Occupation.IsAtWork)
                {
                    return;
                }
                InteractionInstance interactionInstance = bm.Actor.Autonomy.FindBestActionForCommodityOnLot(CommodityKind.RelieveNausea, bm.Actor.LotCurrent, AutonomySearchType.BuffAutoSolve);
                if (interactionInstance != null)
                {
                    interactionInstance.CancellableByPlayer = false;
                    interactionInstance.SetPriority(InteractionPriorityLevel.High);
                    bm.Actor.InteractionQueue.AddNext(interactionInstance);
                    return;
                }
                bm.Actor.InteractionQueue.AddNext(BuffNauseous.ThrowUpOutside.Singleton.CreateInstance(bm.Actor, bm.Actor, new InteractionPriority(InteractionPriorityLevel.High), false, false));
		
            }

        }
    
}
