using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NMS.Tools;
using KOrders.Data;
using NMS.Utility;
using KOrders.Core.Resources;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace NMS.Controllers
{
    public class RequestOrderController : BaseController
    {
        private kafouriEntities db = new kafouriEntities();

        // GET: /Supp/
        public ActionResult Index()
        {
            try
            {
                var orders = db.RequestOrders.Where(i => i.Order.Status == "Openned" || i.Order.Status=="ToReview").ToList();
                return View(orders.ToList());

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // GET: /Supp/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor supplier = db.Vendors.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }
        public JsonResult ItemSelected(string ItemId, string Price)
        {
            if (!string.IsNullOrWhiteSpace(ItemId) && !string.IsNullOrWhiteSpace(Price))
            {
                int id = int.Parse(ItemId);
                VendorItem item = db.VendorItems.Where(i => i.ID == id).SingleOrDefault();
                int noOfItem = item.ItemsPerPackage;
                decimal itemPrice = decimal.Parse(Price) / noOfItem;
                return Json(itemPrice.ToString(), JsonRequestBehavior.AllowGet);

            }
            else return Json("", JsonRequestBehavior.AllowGet);

        }

        public JsonResult getOrderData(string ItemId)
        {

            long id = long.Parse(ItemId);
           Order Supplier = db.Orders.Where(i => i.ID == id).SingleOrDefault();

            //@ViewBag.closeDate = Supplier.Close_Date.ToString();
            //@ViewBag.CloseTime = Supplier.CloseTime.ToString();
            //@ViewBag.AccountNo = Supplier.AccountNo.ToString();

            //@ViewBag.AccountName = Supplier.AccountName.ToString();
            //@ViewBag.RecievePlace = Supplier.RecievePlace.ToString();
            return Json(Supplier, JsonRequestBehavior.AllowGet);
        }
        
              public JsonResult getOrderInfo(string ItemId)
        {

            var Supplier = new List<RequestOrderItem>();
            VMOrder vmorder = new VMOrder();
            //  vmorder.totalPrice
            decimal totalprice = 0;
            vmorder.items = new List<VMOrderItem>();
           
            long id = long.Parse(ItemId);
            Supplier = db.RequestOrderItems.Where(i => i.RequestOrder.OrderId == id).ToList();
            List<RequestOrder> listOfRequest = db.RequestOrders.Where(i => i.OrderId == id).ToList();
            totalprice = listOfRequest.Sum(i=>i.TotalPrice);
            Order order = db.Orders.Where(i => i.ID == id).SingleOrDefault();
            foreach (var item in order.OrderItems)
            {
                VMOrderItem it = new VMOrderItem();
                it.ItemName = item.VendorItem.Name;
                it.OrderedQuantity = db.RequestOrderItems.Where(i=>i.ItemId==item.ID).ToList().Sum(i=>i.Quantity).ToString();
                Decimal OrderedPackage = 0;
                if (decimal.Parse(it.OrderedQuantity) > 0 && item.VendorItem.ItemsPerPackage>0)
                 OrderedPackage = decimal.Parse(it.OrderedQuantity) / decimal.Parse(item.VendorItem.ItemsPerPackage.ToString());
                int rem = 0;
                int div = 0;
                if(int.Parse(it.OrderedQuantity)>0 && int.Parse(item.VendorItem.ItemsPerPackage.ToString())>0)
                  div = Math.DivRem(int.Parse(it.OrderedQuantity), int.Parse(item.VendorItem.ItemsPerPackage.ToString()),out rem);
                it.OrderedPackage = div.ToString() ;//    ToString();
                it.remaniedQuantitytoPackage =( decimal.Parse(item.VendorItem.ItemsPerPackage.ToString()) - rem).ToString();
                it.remaniedPriceToClose =(decimal.Parse(it.remaniedQuantitytoPackage) *decimal.Parse( item.ItemPrice)).ToString();
                it.totaprice = totalprice.ToString();
                it.ItemId = item.ID.ToString();
                it.OrderId =int.Parse(ItemId);
                vmorder.items.Add(it);

            }



            Session["Supplier"] = vmorder.items;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/RequestOrder/Partial/_ItemsInfoartial.cshtml", vmorder.items.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getPartial(string ItemId)
        {
            //86-90
            var Supplier = new List<OrderItem>();
            long id = long.Parse(ItemId);
            Supplier = db.OrderItems.Where(i => i.OrderId == id && i.Status!="Closed").ToList();
            Order order = db.Orders.Where(i => i.ID == id).SingleOrDefault();
            TempData["Close_Date"] = order.Close_Date.ToString();
            TempData["CloseTime"] = order.CloseTime.ToString();
            TempData["AccountNo"] = order.AccountNo.ToString();
            TempData["AccountName"] = order.AccountName.ToString();
            TempData["RecievePlace"] = order.RecievePlace.ToString();
            Session["Close_Date"] = order.Close_Date; ;
            Session["CloseTime"] = order.CloseTime;
            Session["AccountNo"] = order.AccountNo;
            Session["AccountName"] = order.AccountName;
            Session["RecievePlace"] = order.RecievePlace;

            var items = new List<RequestOrderItem>();
            foreach (var item in Supplier)
            {
                RequestOrderItem it = new RequestOrderItem();
                it.OrderItem = item;
               // it.Quantity = 1;
                long count = 0;
                //if (id == 8)
                //{
                //    if (db.RequestOrderItems.Where(i => i.ItemId == item.ID).Any())
                //    {
                //        count = db.RequestOrderItems.Where(i => i.ItemId == item.ID).Sum(i => i.Quantity);

                //    }
                //}
                //if (count <= 9)
                
                   if(item.ID!=247)
                     items.Add(it);
               

            }

            Session["Supplier"] = items;
            //if (id == 8)
            //{
            //    string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/RequestOrder/Partial/_ItemsAddPartialANN.cshtml", items.ToList());
            //    return Json(resultsHtml, JsonRequestBehavior.AllowGet);
            //}
            //else

            //{ 
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/RequestOrder/Partial/_ItemsAddPartial.cshtml", items.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
       // }
           
        }

        public JsonResult AddInvoice(string fileName, HttpPostedFileBase file , string id)
        {
            HttpPostedFileBase files = file;
            var invoices = new List<RequestedOrderInvoice>();
            if (Session["invoices"] != null)
                invoices = (List<RequestedOrderInvoice>)Session["invoices"];
            var invoice = new RequestedOrderInvoice();
            string FileName = files.FileName;
            var pdf = new byte[files.ContentLength];
            files.InputStream.Read(pdf, 0, files.ContentLength);
            byte[] uploadedFile = new byte[files.InputStream.Length];
            //
            string pic = System.IO.Path.GetFileName(files.FileName);
            string path = System.IO.Path.Combine(
                                   Server.MapPath("~/images/profile"), pic);
            // file is uploaded
            files.SaveAs(path);
            invoice.fileName = FileName;

            using (MemoryStream ms = new MemoryStream())
            {
                files.InputStream.CopyTo(ms);
                byte[] array = ms.GetBuffer();
                invoice.ScanOfInvoice = array;

            }

            invoice.ScanOfInvoicePath = path;


            invoices.Add(invoice);
            Session["invoices"] = invoices;
            int intid = int.Parse(Session["id"].ToString());
            RequestOrder supplier = db.RequestOrders.Where(i => i.ID == intid).SingleOrDefault();
            var itemList = from b in db.Orders
                           where b.Status == "Openned"
                           select new { desc = b.Vendor.Name, code = b.ID };
            ViewBag.OrderId = new SelectList(itemList, "code", "desc", supplier.OrderId);
            supplier.RequestedOrderInvoices.Add(invoice);

            Session["Supplier"] = supplier.RequestOrderItems.ToList();
            supplier.RequestedOrderInvoices=invoices;
            //return RedirectToAction("Edit/"+intid.ToString()+"& invoices/"+invoices);
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/RequestOrder/Partial/_ItemsInvoicesPartial.cshtml", invoices.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        //
        public JsonResult Add(string ItemId, string Price, string ItemPrice)
        {

            var Supplier = new List<OrderItem>();
            if (Session["Supplier"] != null)
                Supplier = (List<OrderItem>)Session["Supplier"];
            var SupplierObj = new OrderItem();
            SupplierObj.vendorItemId =long.Parse(ItemId);
            SupplierObj.Price =(Price);
            SupplierObj.VendorItem = new VendorItem();
            SupplierObj.VendorItem = db.VendorItems.Where(i=>i.ID==SupplierObj.vendorItemId).SingleOrDefault();
            SupplierObj.ItemPrice = ItemPrice;
            Supplier.Add(SupplierObj);
            Session["Supplier"] = Supplier;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/OpenOrder/Partial/_ItemsAddPartial.cshtml", Supplier.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ToReview(string OrderId)
        {

            long id = long.Parse(OrderId);
           
            Order ord = db.Orders.Where(i => i.ID == id).SingleOrDefault();
            ord.Status = "ToReview";
            db.Entry(ord).State = EntityState.Modified;
            db.SaveChanges();
            Session["Supplier"] = null;
            CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
            // RedirectToAction("Follow");
            return Json("OK", JsonRequestBehavior.AllowGet);

        }
        public JsonResult ReOpen(string OrderId)
        {

            long id = long.Parse(OrderId);

            Order ord = db.Orders.Where(i => i.ID == id).SingleOrDefault();
            ord.Status = "Openned";
            db.Entry(ord).State = EntityState.Modified;
            db.SaveChanges();
            Session["Supplier"] = null;
            CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
            // RedirectToAction("Follow");
            return Json("OK", JsonRequestBehavior.AllowGet);

        }
        // GET: /Supp/Create
        public ActionResult Create()
        {
            try
            {
                Session["Supplier"] = null;
                var itemList = from b in db.Orders
                               where b.Status == "Openned" 
                               select new { desc = b.Vendor.Name, code = b.ID };
                ViewBag.OrderId = new SelectList(itemList, "code", "desc");
                ViewBag.ItemId = new SelectList(db.OrderItems, "ID", "Name");
                TempData["Close_Date"] = "";
                TempData["CloseTime"] = "";
                TempData["AccountNo"] = "";
                TempData["AccountName"] = "";
                TempData["RecievePlace"] = "";
                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public ActionResult Follow()
        {
            try
            {

                Session["Supplier"] = null;
                var itemList = from b in db.Orders
                               where b.Status == "Openned" || b.Status== "ToReview"
                               select new { desc = b.Vendor.Name, code = b.ID };
                ViewBag.OrderId = new SelectList(itemList, "code", "desc");
                ViewBag.ItemId = new SelectList(db.OrderItems, "ID", "Name");
                TempData["Close_Date"] = "";
                TempData["CloseTime"] = "";
                TempData["AccountNo"] = "";
                TempData["AccountName"] = "";
                TempData["RecievePlace"] = "";
                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public JsonResult VendorSelected(string VendorId)
        {

            try

            {
                int group = int.Parse(VendorId);
                var lms = db.VendorItems.Where(o => o.VendorId == group).ToList();

                var result3 = new SelectList(lms, "ID", "Name");

                return Json(result3, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        
               public JsonResult GenerateDetailsReport(string OrderId)
        {

            var Supplier = new List<OrderItem>();
            long id = long.Parse(OrderId);
            Supplier = db.OrderItems.Where(i => i.OrderId == id).ToList();
            Order order = db.Orders.Where(i => i.ID == id).SingleOrDefault();
            VMOrder vmorder = new VMOrder();
            //  vmorder.totalPrice
            decimal totalprice = 0;
            vmorder.items = new List<VMOrderItem>();

            foreach (var item in order.OrderItems)
            {
                VMOrderItem it = new VMOrderItem();
                it.ItemName = item.VendorItem.Name;
                it.package = item.VendorItem.Package;
                it.packageNos = item.VendorItem.ItemsPerPackage.ToString();//    ToString();
                it.packagePrice = item.Price;
                it.ItemId = item.ID.ToString();
                vmorder.items.Add(it);

            }
            if (vmorder.items != null && vmorder.items.ToList().Count > 0)
            {
                Session["ReportData"] = vmorder.items;
                Session["ReportName"] = "rptDetdata";

                return Json(true, JsonRequestBehavior.AllowGet);


            }
            else
            {
                // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult generateData(string OrderId)
        {

            var Supplier = new List<RequestOrderItem>();
            VMOrder vmorder = new VMOrder();
            //  vmorder.totalPrice
            decimal totalprice = 0;
            vmorder.items = new List<VMOrderItem>();

            long id = long.Parse(OrderId);
            Supplier = db.RequestOrderItems.Where(i => i.RequestOrder.OrderId == id).ToList();
            List<RequestOrder> listOfRequest = db.RequestOrders.Where(i => i.OrderId == id).ToList();
            totalprice = listOfRequest.Sum(i => i.TotalPrice);
            Order order = db.Orders.Where(i => i.ID == id).SingleOrDefault();
            foreach (var item in order.OrderItems)
            {

                VMOrderItem it = new VMOrderItem();
                it.ItemName = item.VendorItem.Name;
                it.OrderedQuantity = db.RequestOrderItems.Where(i => i.ItemId == item.ID).ToList().Sum(i => i.Quantity).ToString();
                if (int.Parse(it.OrderedQuantity) > 0)
                {
                    Decimal OrderedPackage = decimal.Parse(it.OrderedQuantity) / decimal.Parse(item.VendorItem.ItemsPerPackage.ToString());
                    int rem = 0;
                    int div = Math.DivRem(int.Parse(it.OrderedQuantity), int.Parse(item.VendorItem.ItemsPerPackage.ToString()), out rem);
                    it.OrderedPackage = div.ToString();//    ToString();
                    it.remaniedQuantitytoPackage = (decimal.Parse(item.VendorItem.ItemsPerPackage.ToString()) - rem).ToString();
                    it.remaniedPriceToClose = (decimal.Parse(it.remaniedQuantitytoPackage) * decimal.Parse(item.ItemPrice)).ToString();
                    it.totaprice = totalprice.ToString();
                    it.ItemId = item.ID.ToString();
                    vmorder.items.Add(it);
                }

            }
            if (vmorder.items != null && vmorder.items.ToList().Count > 0)
            {
                Session["ReportData"] = vmorder.items;
                Session["ReportName"] = "rptdata";

                return Json(true, JsonRequestBehavior.AllowGet);


            }
            else
            {
                // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }
        
             public JsonResult GenerateAllEditedReport(string OrderId)
        {

            var Supplier = new List<RequestOrderItem>();
            VMOrder vmorder = new VMOrder();
            //  vmorder.totalPrice
            decimal totalprice = 0;
            vmorder.items = new List<VMOrderItem>();

            long id = long.Parse(OrderId);
            Supplier = db.RequestOrderItems.Where(i => i.RequestOrder.OrderId == id).ToList();
            List<Requests> requests = new List<Requests>();
            Decimal totalOldPrice = 0;
            foreach (var item in Supplier)
            {
                Requests req = new Requests();
                if (item.Quantity > 0)
                {
                    req.requestername = item.RequestOrder.RequesterName;
                    req.blocknum = item.RequestOrder.RequesterBlockNo;
                    req.ItemName1 = item.OrderItem.VendorItem.Name;
                    req.itemQuauantity1 = item.Quantity.ToString();
                    req.requestId = item.RequestId;
                    if (item.NewPrice > 0)
                    {
                        req.totalPrice = item.RequestOrder.TotalPrice.ToString();
                        req.totalNewPrice = (item.Quantity * item.NewPrice).ToString();
                        req.Difference = (Decimal.Parse(req.totalPrice) - Decimal.Parse(req.totalNewPrice)).ToString();
                        requests.Add(req);

                    }


                }
            }
            if (requests != null && requests.ToList().Count > 0)
            {
                Session["ReportData"] = requests;
                Session["ReportName"] = "rptAllEditeddata";

                return Json(true, JsonRequestBehavior.AllowGet);


            }
            else
            {
                // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult generateAllData(string OrderId)
        {

            var Supplier = new List<RequestOrderItem>();
            VMOrder vmorder = new VMOrder();
            //  vmorder.totalPrice
            decimal totalprice = 0;
            vmorder.items = new List<VMOrderItem>();

            long id = long.Parse(OrderId);
            Supplier = db.RequestOrderItems.Where(i => i.RequestOrder.OrderId == id).ToList();
            List<Requests> requests = new List<Requests>();

            foreach (var item in Supplier)
            {
                Requests req = new Requests();
                if (item.Quantity > 0)
                {
                    req.requestername = item.RequestOrder.RequesterName;
                    req.blocknum = item.RequestOrder.RequesterBlockNo;
                    req.ItemName1 = item.OrderItem.VendorItem.Name;
                    req.itemQuauantity1 = item.Quantity.ToString();
                    req.requestId = item.RequestId;
                    requests.Add(req);
                }
            }
            if (requests != null && requests.ToList().Count > 0)
            {
                Session["ReportData"] = requests;
                Session["ReportName"] = "rptAlldata";

                return Json(true, JsonRequestBehavior.AllowGet);


            }
            else
            {
                // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }


        // POST: /Supp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( RequestOrder supplier, HttpPostedFileBase file, List<RequestOrderItem> details)
        {
            try
            {
               // if (Session["Supplier"] != null)
                {
                    //long sum = 0;
                    //sum = details.Sum(i => i.Quantity);
                    //if(sum>1)
                    //{
                    //    CommonUtils.SetFeedback("الرجاء اختار قطعة واحدة فقط", Feedback.Feedback_Error);
                    //    Session["Supplier"] = null;
                    //    var itemLists= from b in db.Orders
                    //                   where b.Status == "Openned"
                    //                   select new { desc = b.Vendor.Name, code = b.ID };
                    //    ViewBag.OrderId = new SelectList(itemLists, "code", "desc");
                    //    ViewBag.ItemId = new SelectList(db.OrderItems, "ID", "Name");
                    //    TempData["Close_Date"] = "";
                    //    TempData["CloseTime"] = "";
                    //    TempData["AccountNo"] = "";
                    //    TempData["AccountName"] = "";
                    //    TempData["RecievePlace"] = "";
                    //    return View();
                    //}
                    long id = 0;
                    if (db.RequestOrders.Any())
                        id = db.RequestOrders.Max(i => i.ID);
                    long itemid = 0;
                    if (db.RequestOrderItems.Any())
                        itemid = db.RequestOrderItems.Max(i => i.ID);
                    long invoiceId = 0;
                    if (db.RequestedOrderInvoices.Any())
                        invoiceId = db.RequestedOrderInvoices.Max(i => i.Id);
                    invoiceId += 1;
                    RequestedOrderInvoice invoice = new RequestedOrderInvoice();
                    string FileName = file.FileName;
                    var pdf = new byte[file.ContentLength];
                    file.InputStream.Read(pdf, 0, file.ContentLength);
                    byte[] uploadedFile = new byte[file.InputStream.Length];
                    //
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/images/profile"), pic);
                    // file is uploaded
                    file.SaveAs(path);
                    invoice.fileName = FileName;
                    invoice.Id = invoiceId;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                        invoice.ScanOfInvoice = array;
                        supplier.ScanOfInvoice = array;
                    }

                    invoice.ScanOfInvoicePath = path;


                    //
                    id += 1;
                    supplier.RequestedOrderInvoices = new List<RequestedOrderInvoice>();
                    // List<RequestedOrderInvoice> invoices = (List<RequestedOrderInvoice>)Session["invoices"];
                    // foreach (var inv in invoices)
                    // {
                    invoice.requestId = id;
                        supplier.RequestedOrderInvoices.Add(invoice);
                  //  }
                    supplier.ID = id;
                    supplier.Status = "Openned";

                    supplier.EnteredBy =(CurrentUserID);
                    List<RequestOrderItem> itemList = new List<RequestOrderItem>();
                    foreach (var item in details)
                    {
                        var supObj = new RequestOrderItem();
                        itemid += 1;
                        supObj.ID = itemid;
                        supObj.RequestId = id;
                        supObj.ItemId = item.ItemId;
                        supObj.Price = item.Price;
                        supObj.Quantity = item.Quantity;
                        itemList.Add(supObj);
                    }
                    supplier.RequestOrderItems = itemList;
                    db.RequestOrders.Add(supplier);
                    db.SaveChanges();
                    Session["Supplier"] = null;
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View();
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                throw;            }
         //   if (ModelState.IsValid)
         //   {
         //       db.Suppliers.Add(supplier);
         // supplier.Status="Active";
         // supplier.LastUpdate=DateTime.Now;
         //supplier.Entered_By = CurrentUserName;
         //       db.SaveChanges();
         //       return RedirectToAction("Index");
         //   }

          return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Follow(RequestOrder supplier, List<VMOrderItem> details)
        {
            try
            {
                // if (Session["Supplier"] != null)
                {
                    long id = 0;
                    if (db.ClosedOrders.Any())
                        id = db.ClosedOrders.Max(i => i.ID);
                    long itemid = 0;
                    if (db.ClosedItems.Any())
                        itemid = db.ClosedItems.Max(i => i.ID);
                    ClosedOrder closedOrder = new ClosedOrder();
                     id += 1;
                    closedOrder.ID = id;
                    closedOrder.OrderId = supplier.OrderId;
                    closedOrder.ClosedItems = new List<ClosedItem>();

                    List<ClosedItem> itemList = new List<ClosedItem>();
                    Boolean closeAll = true;
                    foreach (var item in details)
                    {
                        if (!item.close)
                        {
                            closeAll = false;
                            var supObj = new ClosedItem();
                            itemid += 1;
                            supObj.ID = itemid;
                            supObj.OrderItemId = long.Parse(item.ItemId.ToString());
                            supObj.ItemPrice = item.ItemPrice;
                            supObj.OrderedPackages = item.OrderedPackage;
                            supObj.OrderedQuantityItems = item.OrderedQuantity;
                            supObj.RemaindedQuantityToCl = item.remaniedQuantitytoPackage;
                            supObj.RemianedPriceToClose = item.remaniedPriceToClose;
                            supObj.TotalPrice = item.totaprice;
                            closedOrder.totalPrice = item.totaprice.ToString();
                            closedOrder.ClosedItems.Add(supObj);
                        }
                        else
                        {
                            OrderItem orderItem = db.OrderItems.Where(i => i.ID == long.Parse(item.ItemId.ToString())).SingleOrDefault();
                            orderItem.Status = "Closed";
                            db.Entry(orderItem).State = EntityState.Modified;
                        }
                    }
                    if (closeAll)
                    {
                        Order ord = db.Orders.Where(i => i.ID == closedOrder.OrderId).SingleOrDefault();
                        ord.Status = "Closed";
                        db.Entry(ord).State = EntityState.Modified;
                        db.ClosedOrders.Add(closedOrder);
                    }
                    else
                    {

                    }
                    db.SaveChanges();
                    Session["Supplier"] = null;
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Follow");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View();
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(supplier);
            }
            //   if (ModelState.IsValid)
            //   {
            //       db.Suppliers.Add(supplier);
            // supplier.Status="Active";
            // supplier.LastUpdate=DateTime.Now;
            //supplier.Entered_By = CurrentUserName;
            //       db.SaveChanges();
            //       return RedirectToAction("Index");
            //   }

            return View(supplier);
        }

        public FileContentResult RetrieveImage(int id)
        {

            byte[] byteArray = db.RequestOrders.Find(id).ScanOfInvoice;
            return byteArray != null
                ? new FileContentResult(byteArray, "image/jpeg")
                : null;
        }
        public byte[] GetImageFromDataBase(int Id)
        {
            var q = from temp in db.RequestOrders where temp.ID == Id select temp.ScanOfInvoice;
            byte[] cover = q.First();
            return cover;
        }
        // GET: /Supp/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                RequestOrder supplier = db.RequestOrders.Find(id);
            bool dontOpen = false;
            if (supplier != null && supplier.EnteredBy!=null && supplier.EnteredBy.ToString() != CurrentUserID )
                dontOpen = true;
            
            if(supplier.Order.Status== "ToReview")
                dontOpen = true;
            if (Session["Role"].ToString() == "Admins")
                dontOpen = false;
            if (supplier == null || dontOpen)
            {
                return HttpNotFound();
            }
            var itemList = from b in db.Orders
                           where b.Status == "Openned"
                           select new { desc = b.Vendor.Name, code = b.ID };
            ViewBag.OrderId = new SelectList(itemList, "code", "desc",supplier.OrderId);

            Session["Supplier"] = supplier.RequestOrderItems.ToList();
            if (Session["invoices"] == null || Session["Operation"]==null)
            Session["invoices"] = supplier.RequestedOrderInvoices.ToList();
            List<RequestedOrderInvoice> list = (List<RequestedOrderInvoice>)Session["invoices"];
            if (list == null)
                list = new List<RequestedOrderInvoice>();
            int count = list.Count;
            for (int i = count+1; i <= 3; i++)
            {
                RequestedOrderInvoice inv = new RequestedOrderInvoice();
                list.Add(inv);


            }
            Session["id"] = id;
            Session["invoices"] = list;

            return View(supplier);
        }
     
        // POST: /Supp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RequestOrder supplier, List<HttpPostedFileBase> file, List<RequestOrderItem> details)
        {
            try {
                RequestOrder request = db.RequestOrders.Where(i => i.ID == supplier.ID).SingleOrDefault();
                long itemid = 0;
                if (db.RequestedOrderInvoices.Any())
                    itemid = db.RequestedOrderInvoices.Max(i => i.Id);
                List<RequestedOrderInvoice> listofInvoices = new List<RequestedOrderInvoice>();
               if(file!=null)
                foreach (var itemFile in file)
                {
                    if (itemFile != null)
                    {
                        itemid += 1;
                        RequestedOrderInvoice invoice = new RequestedOrderInvoice();
                        string FileName = itemFile.FileName;
                        var pdf = new byte[itemFile.ContentLength];
                        itemFile.InputStream.Read(pdf, 0, itemFile.ContentLength);
                        byte[] uploadedFile = new byte[itemFile.InputStream.Length];
                        //
                        string pic = System.IO.Path.GetFileName(itemFile.FileName);
                        string path = System.IO.Path.Combine(
                                               Server.MapPath("~/images/profile"), pic);
                        // file is uploaded
                        itemFile.SaveAs(path);
                        invoice.fileName = FileName;
                        invoice.Id = itemid;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            itemFile.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                            invoice.ScanOfInvoice = array;
                            request.ScanOfInvoice = array;
                        }

                        invoice.ScanOfInvoicePath = path;
                        invoice.requestId = request.ID;
                        db.RequestedOrderInvoices.Add(invoice);
                        request.RequestedOrderInvoices.Add(invoice);
                    }
                }



                //
                //id += 1;
                // supplier.RequestedOrderInvoices = listofInvoices;
                // List<RequestedOrderInvoice> invoices = (List<RequestedOrderInvoice>)Session["invoices"];
                // foreach (var inv in invoices)
                // {
                //supplier.RequestedOrderInvoices.Add(invoice);
                //  }
                //  supplier.ID = id;
                //  supplier.Status = "Openned";

                request.EnteredBy = (CurrentUserID);
                List<RequestOrderItem> itemList = new List<RequestOrderItem>();
                foreach (var item in details)
                {
                    var supObj = new RequestOrderItem();
                    //itemid += 1;
                    supObj.ID = item.ID;
                    supObj.RequestId = item.RequestId;
                    supObj.ItemId = item.ItemId;
                    supObj.Price = item.Price;
                    supObj.Quantity = item.Quantity;
                    db.Entry(supObj).State = EntityState.Modified;

                    itemList.Add(supObj);
                }
                request.RequestOrderItems = itemList;
                request.TotalPrice = supplier.TotalPrice;
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                Session["Supplier"] = null;
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                return RedirectToAction("Index");
            } catch(Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(Request);
            }
        }
        
        //    return View(supplier);
       // }

        // GET: /Supp/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            RequestOrder supplier = db.RequestOrders.Find(id);
            bool dontOpen = false;
            if (supplier != null && supplier.EnteredBy != null && supplier.EnteredBy.ToString() != CurrentUserID)
                dontOpen = true;
            if (Session["Role"].ToString() == "Admins")
                dontOpen = false;
            if (supplier == null || dontOpen)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: /Supp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                RequestOrder supplier = db.RequestOrders.Find(id);
                List<RequestOrderItem> reqItems =(List<RequestOrderItem>) supplier.RequestOrderItems.ToList();
                foreach (var item in reqItems)
                {
                    db.RequestOrderItems.Remove(item);
                }
                List<RequestedOrderInvoice> reqInv = (List<RequestedOrderInvoice>)supplier.RequestedOrderInvoices.ToList();

                foreach (var item in reqInv)
                {
                    db.RequestedOrderInvoices.Remove(item);
                }
                db.RequestOrders.Remove(supplier);
                // db.Suppliers.Remove(supplier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
