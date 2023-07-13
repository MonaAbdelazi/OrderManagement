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
using System.Data.Entity.Validation;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using RestSharp;
using System.Threading.Tasks;




namespace NMS.Controllers
{
    public class OpenOrderController : BaseController
    {
        private kafouriEntities db = new kafouriEntities();
        private readonly string sessionName = "items";

        private readonly string addedModelsSessionName = "addedItems";
        private readonly string removedModelsSessionName = "removedItems";


        // GET: /Supp/
        public ActionResult Index()
        {
            try
            {
                var orders = db.Orders;
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
        // GET: /Supp/Create
        public ActionResult Create()
        {
            try
            {
                Session["Supplier"] = null;
                ViewBag.VendorId = new SelectList(db.Vendors, "ID", "Name");
                ViewBag.ItemId = new SelectList(db.VendorItems, "ID", "Name");
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
        static void Main(string[] args)
        {
            // Find your Account Sid and Token at twilio.com/console
            

           // Console.WriteLine("Message SID: " + message.Sid);
        }
        // POST: /Supp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Order supplier)
        {
            try
            {
                if (Session["Supplier"] != null)
                {
                    List<OrderItem> Supplierlist = (List<OrderItem>)Session["Supplier"];
                    long id = 0;
                    if (db.Orders.Any())
                        id = db.Orders.Max(i => i.ID);
                    long itemid = 0;
                    if (db.OrderItems.Any())
                        itemid = db.OrderItems.Max(i => i.ID);
                    id += 1;
                    supplier.ID = id;
                    supplier.Status = "Openned";
                    List<OrderItem> itemList = new List<OrderItem>();
                    foreach (var item in Supplierlist)
                    {
                        var supObj = new OrderItem();
                        itemid += 1;
                        supObj.ID = itemid;
                        supObj.vendorItemId = item.vendorItemId;
                        supObj.Price = item.Price;
                        supObj.ItemPrice = item.ItemPrice;
                        itemList.Add(supObj);
                    }
                    supplier.OrderItems = itemList;
                    db.Orders.Add(supplier);
                    db.SaveChanges();
                    Session["Supplier"] = null;

                    //

                    //string instanceId = "ACf634c7c2ea1116932b6bcfe12b5f7058"; // your instanceId
                    //string token = "9cf540eba75a38bc8794adeb11ecb488";         //instance Token
                    //string mobile = "+249912163821";
                    //string message = "WhatsApp API on UltraMsg.com works good";
                    //var url = "https://api.ultramsg.com/" + instanceId + "/messages/chat";
                    //var client = new RestClient(url);
                    //var request = new RestRequest(url, Method.Post);
                    //request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    //request.AddParameter("token", token);
                    //request.AddParameter("to", mobile);
                    //request.AddParameter("body", message);


                    //RestResponse response = await client.ExecuteAsync(request);
                    //var output = response.Content;

                    //


                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                // auth = 9cf540eba75a38bc8794adeb11ecb488
                // sid = ACf634c7c2ea1116932b6bcfe12b5f7058
                //TwilioClient.Init("ACf634c7c2ea1116932b6bcfe12b5f7058", "9cf540eba75a38bc8794adeb11ecb488");

                //var message = MessageResource.Create(

                //   //whatsapp message 
                //   to: new PhoneNumber("+249906662162"), from: new PhoneNumber("+24912163821"),
                //   body: "Ahoy from Twilio!!!!!"
                // );

                //string from = "9199********";
                //string to = "+249912163821";//Sender Mobile
                //string msg = supplier.Vendor.Name+"  "+"account No "+supplier.AccountNo+" Acc Name "+supplier.AccountName;

                //WhatsApp wa = new WhatsApp(from, "BnXk*******B0=", "NickName", true, true);

                //wa.OnConnectSuccess += () =>
                //{
                //    MessageBox.Show("Connected to whatsapp...");

                //    wa.OnLoginSuccess += (phoneNumber, data) =>
                //    {
                //        wa.SendMessage(to, msg);
                //        MessageBox.Show("Message Sent...");
                //    };

                //    wa.OnLoginFailed += (data) =>
                //    {
                //        MessageBox.Show("Login Failed : {0}", data);
                //    };

                //    wa.Login();
                //};

                //wa.OnConnectFailed += (ex) =>
                //{
                //    MessageBox.Show("Connection Failed...");
                //};

                //wa.Connect();

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

        // GET: /Supp/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                Order supplier = db.Orders.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            List<OrderItem> vendorItems = new List<OrderItem>();
            foreach (var item in supplier.OrderItems.ToList())
            {
                vendorItems.Add(item);
            }
            ViewBag.EditItems = vendorItems;
            Session[sessionName] = vendorItems;
            Session[addedModelsSessionName] = new List<OrderItem>();
            Session[removedModelsSessionName] = new List<OrderItem>();
            ViewBag.VendorId = new SelectList(db.Vendors, "ID", "Name",supplier.VendorId);
            ViewBag.ItemId = new SelectList(db.VendorItems, "ID", "Name");

            return View(supplier);
        }

        // POST: /Supp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<OrderItem> addedSubModels = (List<OrderItem>)Session[addedModelsSessionName];
                    List<OrderItem> removedSubModels = (List<OrderItem>)Session[removedModelsSessionName];

                    Order freshObject = db.Orders.Find(model.ID);
                    //freshObject.Name = model.Name;
                    //model = freshObject;
                    db.Entry(model).State = EntityState.Modified;

                    long idChild = 0;
                    if (db.OrderItems.Any())
                        idChild = db.OrderItems.Max(i => i.ID);

                    foreach (var item in addedSubModels)
                    {
                        var curObj = new OrderItem();
                        idChild += 1;
                        curObj.ID = idChild;
                        curObj.OrderId = model.ID;
                        curObj.vendorItemId = long.Parse(item.vendorItemId.ToString());
                        curObj.Price = (item.Price);
                        curObj.VendorItem = new VendorItem();
                        curObj.VendorItem = db.VendorItems.Where(i => i.ID == item.vendorItemId).SingleOrDefault();
                        curObj.ItemPrice = item.ItemPrice;
                        db.OrderItems.Add(curObj);
                    }

                    foreach (var item in removedSubModels)
                    {

                        try
                        {
                            OrderItem DelObject = db.OrderItems.Find(item.ID);

                            db.OrderItems.Remove(DelObject);

                        }
                        catch (Exception ex)
                        {

                            //  throw;
                        }
                    }





                    db.SaveChanges();
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (DbEntityValidationException ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(model);
            }
        }



        // GET: /Supp/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order supplier = db.Orders.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }
        public JsonResult EditAdd(string ItemId, string Price, string ItemPrice)
        {
            var addedSubModels = (List<OrderItem>)Session[addedModelsSessionName];
            var mainList = (List<OrderItem>)Session[sessionName];

            var minIndex = 0L;
            if (mainList != null && mainList.Count > 0)
            {
                minIndex = mainList.Min(a => a.ID);
            }

            var subModel = new OrderItem();
            subModel.vendorItemId = long.Parse(ItemId);
            subModel.Price = (Price);
            subModel.VendorItem = new VendorItem();
            subModel.VendorItem = db.VendorItems.Where(i => i.ID == subModel.vendorItemId).SingleOrDefault();
            subModel.ItemPrice = ItemPrice;
            addedSubModels.Add(subModel);
            mainList.Add(subModel);

            Session[addedModelsSessionName] = addedSubModels;
            ViewBag.EditItems = mainList;
            Session[sessionName] = mainList;

            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/OpenOrder/Partial/BranchesEditGridPartial.cshtml", mainList);
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditDelete(int? id)
        {
            List<OrderItem> mainList = (List<OrderItem>)Session[sessionName];
            List<OrderItem> removedList = (List<OrderItem>)Session[removedModelsSessionName];
            List<OrderItem> addedList = (List<OrderItem>)Session[addedModelsSessionName];
            OrderItem subModel = null;
            for (int i = 0; i < mainList.Count; i++)
            {
                if (mainList[i].ID == id)
                {
                    subModel = mainList[i];
                    mainList.RemoveAt(i);
                    removedList.Add(subModel);
                    if (id < 0)
                    {
                        for (int j = 0; j < addedList.Count; j++)
                        {
                            if (addedList[j].ID == id)
                            {
                                addedList.RemoveAt(j);
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            ViewBag.EditItems = mainList;
            Session[sessionName] = mainList;
            Session[removedModelsSessionName] = removedList;
            Session[addedModelsSessionName] = addedList;

            Order model = db.Orders.Find(subModel.OrderId);

            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/OpenOrder/Partial/BranchesEditGridPartial.cshtml", mainList);
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);

        }

        // POST: /Supp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Order supplier = db.Orders.Find(id);
                supplier.Vendor = null;

                //supplier.Status = "Deleted";
                //  db.Entry(supplier).State = EntityState.Modified;
                List<OrderItem> items = supplier.OrderItems.ToList();
                foreach (var item in items)
                {
                    db.OrderItems.Remove(item);
                }
                db.Orders.Remove(supplier);
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
