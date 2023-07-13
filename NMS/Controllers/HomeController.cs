using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NMS;
using KOrders.Data;
using NMS.Models;

namespace VMS.Controllers
{
    public class HomeController : BaseController
    {
        kafouriEntities db = new kafouriEntities();
        ApplicationDbContext dba=new ApplicationDbContext();
        public JsonResult GetChartData()
        {
            var data =
            (from a in db.Orders
             group a by new
             {
                // Year = a.LastUpdate.Value.Year,

             } into g
             select new
             {
                 //  Year = g.Key.Year,

                 Openned = g.Count(o => o.Status == "Openned"),
                 Closed = g.Count(o => o.Status == "Closed")
             }).AsEnumerable().Select(g => new
             {
                 // Period = g.Year,
                 Openned = g.Openned,
                 Closed = g.Closed
             }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDonutChartData()
        {
            var data =
             (from a in dba.Users
              group a by new
              {
                  status = a.UserName
              }
                            into g
              select new
              {
                  label = g.Key.status,
                  value = g.Count()
              }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDriverDonutChartData()
        {
            var data = (from a in db.Orders
                        group a by new
                        {
                            status = a.Vendor.Name
                        }
                            into g
                        select new
                        {
                            label = g.Key.status,
                            value = g.Count()
                        }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSpareDonutChartData()
        {
            var data = (from a in db.Orders
                        group a by new
                        {
                            status = a.Vendor.Name
                        }
                            into g
                        select new
                        {
                            label = g.Key.status,
                            value = g.Count()
                        }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}