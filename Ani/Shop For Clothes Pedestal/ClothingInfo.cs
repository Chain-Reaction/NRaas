using Sims3.Gameplay.CAS;
using System.Collections.Generic;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace;

namespace ani_ClothingPedestal
{
    [Persistable]
    public class ClothingInfo
    {
        public ulong OwnerId;
        public string OwnerName;
        
        public int Price;
        public int SelectedPose;
        public string Name;

        public bool IsVisible;
        public bool PoseRotation;
        //public bool AllowAutonomous;

        public CASAgeGenderFlags Age;
        public CASAgeGenderFlags Gender;

        public OutfitCategories OutfitCategory;        

        public ClothingInfo()
        {
            Price = 250;
            Name = "Pedestal#";
            SelectedPose = 1;
            IsVisible = true;
            PoseRotation = false;
            //AllowAutonomous = false;
            Gender = CASAgeGenderFlags.Female;
            Age = CASAgeGenderFlags.YoungAdult;
            OutfitCategory = OutfitCategories.Everyday;

        }
    }
}
