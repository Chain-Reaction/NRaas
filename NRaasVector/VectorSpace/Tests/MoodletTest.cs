using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Tests
{
    public class MoodletTest : VectorBooter.Test
    {
        List<BuffNames> mBuffs = new List<BuffNames>();

        List<Origin> mOrigins = new List<Origin>();

        public MoodletTest(XmlDbRow row)
        {
            string name = row.GetString("GUID");

            if (!row.Exists("BuffName"))
            {
                BooterLogger.AddError(name + " BuffName Missing");
            }
            else
            {
                foreach (string buffStr in row.GetStringList("BuffName", ','))
                {
                    BuffNames buff;
                    if (ParserFunctionsEx.Parse(buffStr, out buff))
                    {
                        mBuffs.Add(buff);
                    }
                    else
                    {
                        BooterLogger.AddError(name + " Unknown BuffName: " + buffStr);
                    }
                }
            }

            if (row.Exists("Origin"))
            {
                foreach (string originStr in row.GetStringList("Origin", ','))
                {
                    Origin origin;
                    if (ParserFunctionsEx.Parse(originStr, out origin))
                    {
                        mOrigins.Add(origin);
                    }
                    else
                    {
                        BooterLogger.AddError(name + " Unknown Origin: " + origin);
                    }

                    mOrigins.Add(origin);
                }
            }
        }

        public override void GetEvents(Dictionary<EventTypeId, bool> events)
        {
            if (!events.ContainsKey(EventTypeId.kGotBuff))
            {
                events.Add(EventTypeId.kGotBuff, true);
            }
        }

        public override bool IsSuccess(Event e)
        {
            if (e.Id != EventTypeId.kGotBuff) return false;

            HasGuidEvent<BuffNames> buffEvent = e as HasGuidEvent<BuffNames>;
            if (buffEvent == null) return false;

            if (!mBuffs.Contains(buffEvent.Guid)) return false;

            if (mOrigins.Count > 0)
            {
                BuffInstance buff = e.Actor.BuffManager.GetElement(buffEvent.Guid);
                if (buff != null)
                {
                    if (!mOrigins.Contains(buff.BuffOrigin)) return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            string result = base.ToString();

            foreach (BuffNames buff in mBuffs)
            {
                result += Common.NewLine + " BuffName: " + buff;
            }

            foreach (Origin origin in mOrigins)
            {
                result += Common.NewLine + " Origin: " + origin;
            }

            return result;
        }
    }
}
