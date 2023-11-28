using System.Data;

namespace Pizza
{
    class Program
    {
        static void Main()
        {
            int warehouseCapacity = 5;

            Pizzeria myPizzeria = new Pizzeria(new List<Baker> { new Baker(2), new Baker(5)}, new List<Courier> { new Courier(2), new Courier(1) }, warehouseCapacity);

            bool finishWork = false;
            while (!finishWork)
            {
                Console.WriteLine("Выберите операцию\n" +
                    "1) Создать новый заказ\n" +
                    "2) Завершить заказ пекаря i\n" +
                    "3) Завершить заказ курьера i\n" +
                    "4) Завершить смену");

                string method = Console.ReadLine();
                switch (method)
                {
                    case "1":
                        myPizzeria.CreateOrder();
                        foreach (Order order in myPizzeria.orders.Values)
                        {
                            Console.WriteLine("Заказ    - {0}", order.index);
                            Console.WriteLine("Статус   - {0}", order.status);
                        }
                        break;
                    case "2":
                        List<int> bakersCooking = myPizzeria.GetBakersCooking(); // только тех, что могут закончить
                        if (bakersCooking.Count != 0)
                        {
                            Console.WriteLine("Выберите пекаря");
                            foreach (int baker in bakersCooking)
                            {
                                Console.WriteLine("Пекарь - {0}", baker);
                            }

                            int indexBaker = int.Parse(Console.ReadLine());
                            while (!bakersCooking.Contains(indexBaker))
                            {
                                Console.WriteLine("Выберите пекаря из предложенных");
                                indexBaker = int.Parse(Console.ReadLine());
                            }

                            myPizzeria.CompleteBakerOrder(indexBaker);
                            foreach(Order order in myPizzeria.orders.Values)
                            {
                                Console.WriteLine("Заказ    - {0}", order.index);
                                Console.WriteLine("Статус   - {0}", order.status);
                            }
                            break;
                        }
                        Console.WriteLine("Нет пекарей в работе");
                        break;
                    case "3":
                        List<int> couriersDelivers = myPizzeria.GetCouriersDelivers();
                        if (couriersDelivers.Count != 0)
                        {
                            Console.WriteLine("Выберите курьера");
                            foreach (int courier in couriersDelivers)
                            {
                                Console.WriteLine("Курьер - {0}", courier);
                            }

                            int indexCourier = int.Parse(Console.ReadLine());
                            while (!couriersDelivers.Contains(indexCourier))
                            {
                                Console.WriteLine("Выберите курьера из предложенных");
                                indexCourier = int.Parse(Console.ReadLine());
                            }

                            myPizzeria.CompleteCourierOrders(indexCourier);
                            break;
                            foreach (Order order in myPizzeria.orders.Values)
                            {
                                Console.WriteLine("Заказ    - {0}", order.index);
                                Console.WriteLine("Статус   - {0}", order.status);
                            }
                        }
                        Console.WriteLine("Нет курьеров в работе");
                        break;
                    case "4":
                        myPizzeria.FinishWork();
                        finishWork = true;
                        break;
                }
            }
        }
    }
}
