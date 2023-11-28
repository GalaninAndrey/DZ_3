using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza
{
    public class AutomationSystem
    {
        const int maxNumComplaintsBakers = 2; // допустимое количестов репортов
        const int maxNumComplaintsCouriers = 2;
        const int maxNumComplaintsWarehouse = 2;

        int ReportWarehouse;
        int ReportBakers;
        int ReportCouriers;

        public AutomationSystem()
        {
            ReportWarehouse = 0;
            ReportBakers = 0;
            ReportCouriers = 0;
        }

        public void SendReportWarehouse() { ReportWarehouse++; }
        public void SendReportBakers() { ReportBakers++; }
        public void SendReportCouriers() { ReportCouriers++; }

        public void MakeRecomendation()
        {
            bool takeMoreOrders = true;

            if (ReportWarehouse > maxNumComplaintsWarehouse)
            {
                Console.WriteLine("Необходимо увеличить склад");
                takeMoreOrders = false;
            }

            if (ReportBakers > maxNumComplaintsBakers)
            {
                Console.WriteLine("Необходимо пополнить штат пекарей");
                takeMoreOrders = false;
            }

            if (ReportCouriers > maxNumComplaintsCouriers)
            {
                Console.WriteLine("Необходимо пополнить штат курьеров");
                takeMoreOrders = false;
            }

            if (takeMoreOrders) Console.WriteLine("Можно увеличить количество заказов");
        }
    }
}
