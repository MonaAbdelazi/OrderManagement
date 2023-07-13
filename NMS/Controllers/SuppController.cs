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

namespace NMS.Controllers
{
    public class SuppController : BaseController
    {
        private kafouriEntities db = new kafouriEntities();
        private readonly string sessionName = "items";

        private readonly string addedModelsSessionName = "addedItems";
        private readonly string removedModelsSessionName = "removedItems";
        private readonly string editedModelsSessionName = "editedItems";


        // GET: /Supp/
        public ActionResult Index()
        {
            try
            {
                var suppliers = db.Vendors;
                return View(suppliers.ToList());

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
        //
        public JsonResult Add(string NoPerPackage, string Packaging, string Name)
        {

            var Supplier = new List<VendorItem>();
            if (Session["Supplier"] != null)
                Supplier = (List<VendorItem>)Session["Supplier"];
            var SupplierObj = new VendorItem();
            SupplierObj.Package = Packaging;
            SupplierObj.ItemsPerPackage = int.Parse(NoPerPackage);
            SupplierObj.Name = Name;
            Supplier.Add(SupplierObj);
            Session["Supplier"] = Supplier;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Supp/Partial/_SupplierAddPartial.cshtml", Supplier.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditAdd(string NoPerPackage, string Packaging, string Name)
        {
            var addedSubModels = (List<VendorItem>)Session[addedModelsSessionName];
            var mainList = (List<VendorItem>)Session[sessionName];

            var minIndex = 0L;
            if (mainList != null && mainList.Count > 0)
            {
                minIndex = mainList.Min(a => a.ID);
            }

            var subModel = new VendorItem();
            subModel.Name = Name;
            subModel.Package = Packaging;
            subModel.ItemsPerPackage = int.Parse(NoPerPackage); subModel.ID = (minIndex > 0) ? -1 : minIndex - 1;
            addedSubModels.Add(subModel);
            mainList.Add(subModel);

            Session[addedModelsSessionName] = addedSubModels;
            ViewBag.EditItems = mainList;
            Session[sessionName] = mainList;

            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Supp/Partial/BranchesEditGridPartial.cshtml", mainList);
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // GET: /Supp/Create
        public ActionResult Create()
        {
            Session["Supplier"] = null;
            return View();
        }

        // POST: /Supp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Vendor supplier)
        {
            try
            {
                if (Session["Supplier"] != null)
                {
                    List<VendorItem> Supplierlist = (List<VendorItem>)Session["Supplier"];
                    long id = 0;
                    if (db.Vendors.Any())
                        id = db.Vendors.Max(i => i.ID);
                    id += 1;
                    long itemid = 0;
                    if (db.VendorItems.Any())
                        itemid = db.VendorItems.Max(i => i.ID);
                    supplier.ID = id;
                    List<VendorItem> itemList = new List<VendorItem>();
                    foreach (var item in Supplierlist)
                    {
                        var supObj = new VendorItem();
                        itemid += 1;
                        supObj.ID = itemid;
                        supObj.Name = item.Name;
                        supObj.ItemsPerPackage = item.ItemsPerPackage;
                        supObj.Package = item.Package;
                        supObj.VendorId = id;
                        itemList.Add(supObj);
                    }
                    supplier.VendorItems = itemList;
                    db.Vendors.Add(supplier);
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
            Vendor supplier = db.Vendors.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }

            List<VendorItem> vendorItems = new List<VendorItem>();
            foreach (var item in supplier.VendorItems.ToList())
            {
                vendorItems.Add(item);
            }
            ViewBag.EditItems = vendorItems;
            Session[sessionName] = vendorItems;
            Session[addedModelsSessionName] = new List<VendorItem>();
            Session[removedModelsSessionName] = new List<VendorItem>();

            return View(supplier);
        }
        public JsonResult EditDelete(int? id)
        {
            List<VendorItem> mainList = (List<VendorItem>)Session[sessionName];
            List<VendorItem> removedList = (List<VendorItem>)Session[removedModelsSessionName];
            List<VendorItem> addedList = (List<VendorItem>)Session[addedModelsSessionName];
            VendorItem subModel = null;
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

            Vendor model = db.Vendors.Find(subModel.VendorId);

            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Supp/Partial/BranchesEditGridPartial.cshtml", mainList);
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);

        }

        // POST: /Supp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vendor model, List<VendorItem> details, List<VendorItem> EditItems)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<VendorItem> addedSubModels = (List<VendorItem>)Session[addedModelsSessionName];
                    List<VendorItem> removedSubModels = (List<VendorItem>)Session[removedModelsSessionName];
                    List<VendorItem> editedSubModels = (List<VendorItem>)Session[editedModelsSessionName];
                    List<VendorItem>  llist = (List<VendorItem>) ViewBag.EditItems;
                    Vendor freshObject = db.Vendors.Find(model.ID);
                    freshObject.Name = model.Name;
                    model = freshObject;

                    long idChild = 0;
                    if (db.VendorItems.Any())
                        idChild = db.VendorItems.Max(i => i.ID);
                    db.Entry(model).State = EntityState.Modified;

                    foreach (var item in addedSubModels)
                    {
                        var curObj = new VendorItem();
                        idChild += 1;
                        curObj.ID = idChild;
                        curObj.VendorId = model.ID;
                        curObj.Name = item.Name;
                        curObj.Package = item.Package;
                        curObj.ItemsPerPackage = item.ItemsPerPackage;
                        db.VendorItems.Add(curObj);
                    }

                    foreach (var item in removedSubModels)
                    {

                        try
                        {
                            VendorItem DelObject = db.VendorItems.Find(item.ID);

                            db.VendorItems.Remove(DelObject);

                        }
                        catch (Exception ex)
                        {

                          //  throw;
                        }
                    }

                    //List<VendorItem> itemList = new List<VendorItem>();
                    //foreach (var item in details)
                    //{
                    //    var supObj = new VendorItem();
                    //    //itemid += 1;
                    //    supObj.ID = item.ID;
                    //    supObj.VendorId = item.VendorId;
                    //    supObj.Name = item.Name;
                    //    supObj.ItemsPerPackage = item.ItemsPerPackage;
                    //    supObj.Package = item.Package;
                    //    db.Entry(supObj).State = EntityState.Modified;

                    //    itemList.Add(supObj);
                    //}
                    //model.VendorItems = itemList;



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
            Vendor supplier = db.Vendors.Find(id);
            if (supplier == null)
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
                Vendor supplier = db.Vendors.Find(id);
                
                db.Vendors.Remove(supplier);
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
