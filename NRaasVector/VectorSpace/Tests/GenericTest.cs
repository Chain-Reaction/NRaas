using NRaas.CommonSpace.Booters;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Utilities;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Tests
{
    public class GenericTest : VectorBooter.Test
    {
        List<EventTypeId> mEvents = new List<EventTypeId>();

        public GenericTest(XmlDbRow row)
        {
            string name = row.GetString("GUID");

            foreach (string e in row.GetStringList("Event", ','))
            {
                EventTypeId id;
                if (!ParserFunctions.TryParseEnum<EventTypeId>(e, out id, EventTypeId.kEventNone))
                {
                    BooterLogger.AddError(name + " Unknown Event: " + e);
                }

                mEvents.Add(id);
            }
        }

        public override void GetEvents(Dictionary<EventTypeId, bool> events)
        {
            foreach (EventTypeId e in mEvents)
            {
                if (events.ContainsKey(e)) continue;

                events.Add(e, true);
            }
        }

        public override bool IsSuccess(Event e)
        {
            return mEvents.Contains(e.Id);
        }

        public override string ToString()
        {
            string result = GetType().Name;

            foreach (EventTypeId id in mEvents)
            {
                result += Common.NewLine + " Event: " + id;
            }

            return result;
        }
    }
}
