using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;

namespace NRaas.CommonSpace.Helpers
{
    public class InsectTerrarium
    {
        public static Terrarium Create(InsectJig ths, Sim actor)
        {
            Terrarium terrarium = Terrarium.Create(ths.InsectType);
            if (Inventories.TryToMove(terrarium, actor))
            {
                InsectData data;
                if (InsectData.sData.TryGetValue(ths.InsectType, out data))
                {
                    bool flag;
                    bool flag2;
                    (actor.SkillManager.AddElement(SkillNames.Collecting) as Collecting).Collected(ths.InsectType, actor, terrarium, out flag, out flag2);

                    /*
                    string str = CatchInsect.GetLocalizedCatchSuccessTNS(this.InsectType, data.Rarity, terrarium.Value, actor);
                    string message = str;
                    if (flag)
                    {
                        string str3 = CatchInsect.LocalizeString(actor.IsFemale, "TNSScienceLab", new object[0]);
                        message = CatchInsect.LocalizeString(actor.IsFemale, "TNSFullPlusScienceLab", new object[] { str, str3 });
                    }
                    actor.ShowTNSIfSelectable(message, StyledNotification.NotificationStyle.kGameMessagePositive, ObjectGuid.InvalidObjectGuid);
                    if (!flag2)
                    {
                        actor.Play3dStingIfSelectable("sting_collect_collection");
                    }
                    */

                    EventTracker.SendEvent(EventTypeId.kCaughtBug, actor, terrarium);
                    ActiveTopic.AddToSim(actor, "Recently Collected Bugs");
                }

                return terrarium;
            }
            else
            {
                terrarium.Destroy();
                return null;
            }
        }
    }
}

