using Pizza;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Pizza.Baker;

namespace Pizza
{
    public class Pizzeria
    {
        public Dictionary<int, Order> orders { get; set; }
        public Dictionary<int, Baker> bakerDictionary { get; set; }
        public Dictionary<int, Courier> courierDictionary { get; set; }
        public Warehouse warehouse { get; set; }
        public AutomationSystem automationSystem { get; set; }

        public Pizzeria(List<Baker> bakerList, List<Courier> courierList, int warehouseCapacity)
        {
            orders = new Dictionary<int, Order>();
            automationSystem = new AutomationSystem();
            bakerDictionary = new Dictionary<int, Baker>();
            foreach (Baker bakerToAdd in bakerList)
            {
                bakerDictionary.Add(bakerToAdd.index, bakerToAdd);
            }

            courierDictionary = new Dictionary<int, Courier>();
            foreach (Courier courierToAdd in courierList)
            {
                courierDictionary.Add(courierToAdd.index, courierToAdd);
            }

            warehouse = new Warehouse(warehouseCapacity);
        }

        public List<int> GetBakersCooking() // те, что готовят
        {
            List<int> bakers = new List<int>();

            foreach (Baker curBaker in bakerDictionary.Values)
            {
                if (curBaker.status == Baker.BakerStatus.Cooking)
                {
                    bakers.Add(curBaker.index);
                }
            }

            return bakers;
        }

        public List<int> GetCouriersDelivers() // те, что доставляют удовольствие
        {
            List<int> couriers = new List<int>();

            foreach (Courier curCourier in courierDictionary.Values)
            {
                if (curCourier.status == Courier.CourierStatus.Delivers)
                {
                    couriers.Add(curCourier.index);
                }
            }

            return couriers;
        }

        public int? FindOrderInWarehouse(int maxSize) // ищем заказ, который влезет в сумку курьера
        {
            int order = warehouse.orders.FirstOrDefault(x => orders[x].size <= maxSize);
            if (order == 0) return null;
            return order;
        }

        public int? FindWaitingBaker() // бейкер который ожидает место на складе
        {
            Baker baker = bakerDictionary.Values.FirstOrDefault(x => x.status == Baker.BakerStatus.Waiting);
            if (baker == null) return null;
            return baker.index;
        }

        public int? FindFreeCourier() // свободный курьер
        {
            Courier courier = courierDictionary.Values.FirstOrDefault(x => x.status == Courier.CourierStatus.Ready);
            if (courier == null) return null;
            return courier.index;
        }

        public int? FindOrderWaitingBaker() // ордер, который ждет бейкера
        {
            Order order = orders.Values.FirstOrDefault(x => x.status == Order.OrderStatus.WaitingBaker);
            if (order == null) return null;
            return order.index;
        }

        public int? FindFreeBaker() // бейкер, который свободен
        {
            Baker baker = bakerDictionary.Values.FirstOrDefault(x => x.status == Baker.BakerStatus.Ready);
            if (baker == null) return null;
            return baker.index;
        }

        public List<int> CreateOrdersQueueToAssignCourier(int trunkSize) // ищем заказы по максимуму, которые влезут курьеру
        {
            List<int> ordersToCourier = new List<int>();
            int? orderToAssign = FindOrderInWarehouse(trunkSize);
            while (orderToAssign.HasValue)
            {
                ordersToCourier.Add(orderToAssign.Value);
                trunkSize -= orders[orderToAssign.Value].size;
                orderToAssign = FindOrderInWarehouse(trunkSize);
            }
            return ordersToCourier;
        }

        public List<Order> AssignOrdersToCourierFromQueue(int indexCourier)
        {
            List<int> ordersToCourier = CreateOrdersQueueToAssignCourier(courierDictionary[indexCourier].trunkSize);
            return AssignOrdersToCourier(indexCourier, ordersToCourier);
        }

        public void CompleteCourierOrders(int indexCourier)
        {
            foreach (int orderToComplete in courierDictionary[indexCourier].orders)
            {
                orders.Remove(orderToComplete);
            }
            courierDictionary[indexCourier].CompleteOrders();

            List<Order> ordersToRemoveFromWarehouse = AssignOrdersToCourierFromQueue(indexCourier);
            if (ordersToRemoveFromWarehouse.Count > 0)
            {
                warehouse.RemoveOrders(ordersToRemoveFromWarehouse);
                SendOrdersToWarehouseFromQueue();
            }
        }
        public void AssignOrdersToCourier(int indexCourier, int orderToAssign) // курьер берет один 
        {
            courierDictionary[indexCourier].AssignOrder(orderToAssign, orders[orderToAssign].size);
            courierDictionary[indexCourier].UpdateStatus(Courier.CourierStatus.Delivers);   
            orders[orderToAssign].UpdateStatus(Order.OrderStatus.Courier);
        }
        public List<Order> AssignOrdersToCourier(int indexCourier, List<int> ordersToAssign) // берем как можно больше
        {
            List<Order> assignedOrders = new List<Order>();
            foreach (int orderToAdd in ordersToAssign)
            {
                if (courierDictionary[indexCourier].AssignOrder(orderToAdd, orders[orderToAdd].size))
                {
                    orders[orderToAdd].UpdateStatus(Order.OrderStatus.Courier);
                    assignedOrders.Add(orders[orderToAdd]);
                }
            }
            if (assignedOrders.Count != 0) courierDictionary[indexCourier].UpdateStatus(Courier.CourierStatus.Delivers);
            return assignedOrders;
        }

        public void SendOrdersToWarehouseFromQueue()
        {
            int? indexWaitingBaker = FindWaitingBaker();
            if (indexWaitingBaker != null) CompleteBakerOrder(indexWaitingBaker.Value);
        }

        public bool SendOrderToWarehouse(int indexBaker)
        {
            int orderToAssign = bakerDictionary[indexBaker].order;
            if (warehouse.currentCapacity + orders[orderToAssign].size > warehouse.maxСapacity) return false;

            warehouse.SetOrder(orders[orderToAssign]);
            orders[orderToAssign].UpdateStatus(Order.OrderStatus.Warehouse);

            bakerDictionary[indexBaker].CompleteOrder();

            int? indexFreeCourier = FindFreeCourier();
            if (indexFreeCourier.HasValue)
            {
                AssignOrdersToCourier(indexFreeCourier.Value,  orderToAssign );

                warehouse.RemoveOrders(new List<Order> { orders[orderToAssign] });
                SendOrdersToWarehouseFromQueue();
            }
            else automationSystem.SendReportCouriers();

            return true;
        }

        public void AssignOrdersToBakerFromQueue(int indexBaker)
        {
            int? orderToBaker = FindOrderWaitingBaker(); // ищем заказ, который в ожидании
            if (orderToBaker.HasValue) AssignOrderToBaker(indexBaker, orderToBaker.Value);
        }

        public void CompleteBakerOrder(int indexBaker) // завершить заказ пекаря
        {
            bakerDictionary[indexBaker].UpdateStatus(Baker.BakerStatus.Waiting);
            if (SendOrderToWarehouse(indexBaker)) AssignOrdersToBakerFromQueue(indexBaker);
            else automationSystem.SendReportWarehouse();
        }

        public void AssignOrderToBaker(int indexBaker, int orderToAssign) // бейкер берет заказ
        {
            bakerDictionary[indexBaker].AssignOrder(orderToAssign);
            bakerDictionary[indexBaker].UpdateStatus(Baker.BakerStatus.Cooking);
            orders[orderToAssign].UpdateStatus(Order.OrderStatus.Baker);
        }

        public void CreateOrder(int sizeOrder = 1)
        {
            Order newOrder = new Order(sizeOrder);
            orders.Add(newOrder.index, newOrder);

            int? indexFreeBaker = FindFreeBaker();
            if (indexFreeBaker.HasValue) AssignOrderToBaker(indexFreeBaker.Value, newOrder.index);
            else if (!warehouse.IsFull()) automationSystem.SendReportBakers();
        }

        public void FinishWork()
        {
            foreach (Baker baker in bakerDictionary.Values) baker.CompleteOrder();
            foreach (Courier courier in courierDictionary.Values) courier.CompleteOrders();
            orders.Clear();
            warehouse.RemoveOrders();

            automationSystem.MakeRecomendation();
        }
    }
}
