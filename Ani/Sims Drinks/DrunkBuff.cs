using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.ActorSystems;
using Sims3.SimIFace;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.UI;
using Sims3.UI;

namespace Alcohol
{
    public class DrunkBuff : Buff
    {
        public static ulong StaticGuid
        {
            get { return 5531456504025179778uL; }
        }

        public class BuffInstanceDrunk : BuffInstance
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
            private BuffInstanceDrunk()
            {
            }
            public BuffInstanceDrunk(Buff buff, BuffNames buffGuid, int effectValue, float timeoutCount)
                : base(buff, buffGuid, effectValue, timeoutCount)
            {
            }
            public override BuffInstance Clone()
            {
                return new DrunkBuff.BuffInstanceDrunk(this.mBuff, this.mBuffGuid, this.mEffectValue, this.mTimeoutCount);
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
        public DrunkBuff(Buff.BuffData info)
            : base(info)
        {
        }
        public override BuffInstance CreateBuffInstance()
        {
            return new DrunkBuff.BuffInstanceDrunk(this, base.BuffGuid, base.EffectValue, base.TimeoutSimMinutes);
        }

        public override void OnTimeout(BuffManager bm, BuffInstance bi, Buff.OnTimeoutReasons reason)
        {
            bm.AddElement(DrinkingBuffs.sHangover, Origin.FromJuice);
        }

    }

    public class DrunkCounter : Buff
    {
        public static ulong StaticGuid
        {
            get { return 9223372036854775808uL; }
        }

        public class BuffInstanceDrunk : BuffInstance
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
            private BuffInstanceDrunk()
            {
            }
            public BuffInstanceDrunk(Buff buff, BuffNames buffGuid, int effectValue, float timeoutCount)
                : base(buff, buffGuid, effectValue, timeoutCount)
            {
            }
            public override BuffInstance Clone()
            {
                return new DrunkCounter.BuffInstanceDrunk(this.mBuff, this.mBuffGuid, this.mEffectValue, this.mTimeoutCount);
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
        public DrunkCounter(Buff.BuffData info)
            : base(info)
        {
        }
        public override BuffInstance CreateBuffInstance()
        {
            return new DrunkCounter.BuffInstanceDrunk(this, base.BuffGuid, base.EffectValue, base.TimeoutSimMinutes);
        }

        public override void OnTimeout(BuffManager bm, BuffInstance bi, Buff.OnTimeoutReasons reason)
        {
            if (bm.Actor != null)
                DrunkInteractions.DoDrunkInteraction(bm.Actor);
        }

    }

}
