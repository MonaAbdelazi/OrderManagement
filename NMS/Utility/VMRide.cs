using KOrders.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMS.Utility
{
    public class VMOrder
    {


        public RequestOrder Ride_Id { get; set; }
        public string totalPrice { get; set; }
        public List<VMOrderItem> items { get; set; }


    }
    public class VMOrderName
    {


        public RequestOrder Ride_Id { get; set; }
        public string totalPrice { get; set; }
        public List<VMOrderItem> items { get; set; }


    }

    public class VMOrderItem
    {


        public RequestOrder order { get; set; }
        public string ItemId { get; set; }
        public int OrderId { get; set; }


        public string ItemName { get; set; }
        public string ItemPrice { get; set; }

        public string OrderedQuantity { get; set; }

        public string OrderedPackage { get; set; }

        public string remaniedQuantitytoPackage { get; set; }
        public string remaniedPriceToClose { get; set; }

        public string totaprice { get; set; }

        public string package { get; set; }
        public string packagePrice { get; set; }

        public string packageNos { get; set; }

        public Boolean close { get; set; }








    }
    public class Requests
    {


        public string requestername { get; set; }
        public long requestId { get; set; }

        public string blocknum { get; set; }
        public string totalPrice { get; set; }
        public string totalNewPrice { get; set; }
        public string Difference { get; set; }

        public string ItemName1 { get; set; }
        public string itemQuauantity1 { get; set; }

        public string ItemName2 { get; set; }

        public string itemQuauantity2 { get; set; }

        public string ItemName3 { get; set; }
        public string itemQuauantity3 { get; set; }

        public string ItemName4 { get; set; }
        public string itemQuauantity4 { get; set; }

        public string ItemName5 { get; set; }

        public string itemQuauantity5 { get; set; }

        public string ItemName6 { get; set; }
        public string itemQuauantity6 { get; set; }

        public string ItemName7 { get; set; }
        public string itemQuauantity7 { get; set; }

        public string ItemName8 { get; set; }

        public string itemQuauantity8 { get; set; }

        public string ItemName9 { get; set; }
        public string itemQuauantity9 { get; set; }

        public string ItemNam10 { get; set; }
        public string itemQuauantity10 { get; set; }

        public string ItemName11 { get; set; }

        public string itemQuauantity11 { get; set; }

        public string ItemName12 { get; set; }
        public string itemQuauantity12 { get; set; }
        public string ItemName13 { get; set; }
        public string itemQuauantity13 { get; set; }

        public string ItemName14 { get; set; }

        public string itemQuauantity14 { get; set; }

        public string ItemName15 { get; set; }
        public string itemQuauantity15 { get; set; }

        public string ItemName16 { get; set; }
        public string itemQuauantity16 { get; set; }

        public string ItemName17 { get; set; }

        public string itemQuauantity17 { get; set; }

        public string ItemName18 { get; set; }
        public string itemQuauantity18 { get; set; }

        public string ItemName19 { get; set; }
        public string itemQuauantity19 { get; set; }

        public string ItemName20 { get; set; }

        public string itemQuauantity20 { get; set; }

        public string ItemName21 { get; set; }
        public string itemQuauantity21{ get; set; }

        public string ItemNam22 { get; set; }
        public string itemQuauantity22 { get; set; }

        public string ItemName23 { get; set; }

        public string itemQuauantity23 { get; set; }

        public string ItemName24 { get; set; }
        public string itemQuauantity24 { get; set; }

    }

}