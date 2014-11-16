using Sims3.SimIFace;
using ani_StoreSetRegister;

namespace ani_StoreSetBase.Shopping
{
    [Persistable]
    public class ShoppingProgress
    {
        public int ProgressLevel = 0;
        public int MaxProgressLevel = 500;
        public float ProgressPoints = 0;
        public int PointsForNextLevel = 500;

        public float kMinutesPerPlusPlusEffect;

        public ShoppingProgress(float minutes)
        {
            kMinutesPerPlusPlusEffect = minutes;
        }
       

        public float Progress
        {
            get
            {
                return this.ProgressPoints;
                //if (this.ProgressLevel != this.MaxProgressLevel)
                //{
                //    int num = MaxProgressLevel;// this.mNonPersistableData.PointsForNextLevel[this.ProgressLevel];

                //    if (this.ProgressLevel > 0)
                //    {                        
                //        int num2 = this.PointsForNextLevel;// this.mNonPersistableData.PointsForNextLevel[this.ProgressLevel - 1];
                //        return (this.ProgressPoints - (float)num2) / (float)(num - num2);
                //    }                  

                //    if (num > 0)
                //    {
                //        return this.ProgressPoints / (float)num;
                //    }
                //}
                //return 0f;
            }
        }
               
    }
}
