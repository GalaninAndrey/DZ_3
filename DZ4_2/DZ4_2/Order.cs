using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza
{
    public class Order
    {
        private static int currentId = 1;

        public int index { get; }
        public int size { get; }

        public enum OrderStatus
        {
            WaitingBaker,
            Baker,
            Warehouse,
            Courier
        }

        public OrderStatus status { get; private set; }

        public Order(int _size)
        {
            index = currentId++;
            size = _size;
            status = OrderStatus.WaitingBaker;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            status = newStatus;
        }
    }
}
