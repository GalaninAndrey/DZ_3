using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza
{
    public class Baker
    {
        private static int currentId = 1;

        public int index { get; }
        public int workExperience { get; }

        public int order { get; set; }

        public enum BakerStatus
        {
            Ready,
            Waiting,
            Cooking
        }

        public BakerStatus status { get; private set; }

        public Baker(int workExperience)
        {
            index   = currentId++;
            status  = BakerStatus.Ready;
            order   = 0;
            this.workExperience = workExperience;
        }

        public bool AssignOrder(int orderIndex)
        {
            if (IsCooking()) return false;
            order = orderIndex;
            return true;
        }

        public void CompleteOrder()
        {
            order = 0;
            UpdateStatus(BakerStatus.Ready);
        }

        public void UpdateStatus(BakerStatus newStatus)
        {
            status = newStatus;
            if (status == BakerStatus.Ready) order = 0;
        }

        public bool IsCooking()
        {
            return status == BakerStatus.Cooking;
        } 
    }
}
