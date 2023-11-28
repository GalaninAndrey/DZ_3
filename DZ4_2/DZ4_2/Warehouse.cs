using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza
{
    public class Warehouse
    {
        public int maxСapacity { get; }
        public int currentCapacity { get; private set; }

        public List<int> orders { get; }

        //public Warehouse()
        //{s
        //    maxСapacity = 0;
        //    currentCapacity = 0;
        //    orders = new List<int>();
        //}

        public Warehouse(int _maxCapacity)
        {
            maxСapacity = _maxCapacity;
            currentCapacity = 0;
            orders = new List<int>();
        }

        public void SetOrder(Order order)
        {
            orders.Add(order.index);
            currentCapacity += order.size;
        }

        public void RemoveOrders() // чтобы завершить смену
        {
            currentCapacity = 0;
            orders.Clear();
        }

        public void RemoveOrders(List<Order> ordersRemoveList)
        {
            foreach (Order order in ordersRemoveList)
            {
                currentCapacity += order.size;
            }
            orders.RemoveAll(r => ordersRemoveList.Any(a => a.index == r));
        }

        public bool IsFull() { return currentCapacity == maxСapacity; }
    }
}
