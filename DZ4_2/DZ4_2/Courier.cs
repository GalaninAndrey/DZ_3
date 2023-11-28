using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pizza.Baker;

namespace Pizza
{
    public class Courier
    {
        private static int currentId = 1;

        public int index { get; }
        public int trunkSize { get; }
        public int availableTrunkSize { get; set; }

        public List<int> orders { get; }

        public enum CourierStatus
        {
            Ready,
            Delivers
        }

        public CourierStatus status { get; private set; }

        public Courier(int trunkSize)
        {
            index = currentId++;
            availableTrunkSize = this.trunkSize = trunkSize;
            status = CourierStatus.Ready;
            orders = new List<int>();
        }

        public bool AssignOrder(int indexOrder, int sizeOrder)
        {
            if (availableTrunkSize - sizeOrder >= 0)
            {
                orders.Add(indexOrder);
                availableTrunkSize -= sizeOrder;
                return true;
            }
            return false;
        }

        public void CompleteOrder(int orderIndex, int sizeOrder)
        {
            orders.Remove(orderIndex);
            availableTrunkSize += sizeOrder;
            if (orders.Count == 0) UpdateStatus(CourierStatus.Ready);
        }

        public void CompleteOrders()
        {
            orders.Clear();
            availableTrunkSize = trunkSize;
            UpdateStatus(CourierStatus.Ready);
        }

        public void UpdateStatus(CourierStatus newStatus)
        {
            status = newStatus;
            if (status == CourierStatus.Ready) orders.Clear();
        }
    }
}
