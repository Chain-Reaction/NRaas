using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;

namespace NRaas.CareerSpace.Helpers
{
    public class LearnToDriveHelper : Common.IAddInteraction
    {
        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddNoDupTest<SchoolRabbitHole>(new RabbitHole.AttendClassInRabbitHole.Definition(SkillNames.LearnToDrive, SchoolRabbitHole.kClassPaintingCost, SchoolRabbitHole.kClassPaintingDuration));
        }
    }
}
