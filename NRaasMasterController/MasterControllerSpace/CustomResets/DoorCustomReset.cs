using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ObjectComponents;

namespace NRaas.MasterControllerSpace.CustomResets
{
    public class DoorCustomReset : GenericCustomReset<Door>
    {
        protected override bool PrivatePerform(Door obj)
        {
            PortalComponent portalComponent = obj.PortalComponent;
            if (portalComponent != null)
            {
                portalComponent.FreeAllRoutingLanes();
            }

            obj.SetObjectToReset();

            return true;
        }
    }
}
