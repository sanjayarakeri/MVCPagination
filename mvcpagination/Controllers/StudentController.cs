using mvcpagination.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace mvcpagination.Controllers
{

    public class StudentController : Controller
    {
        public JsonResult getTable(string OrderBy, string SearchBy, int? currentPage, int? pageSize)
        {
            List<HtmlTableSettings> headerTextAndIDs = new List<HtmlTableSettings>();
            headerTextAndIDs.Add(new HtmlTableSettings() { columnName = "CustomerID", columnText = "Customer ID", isColumnSupportSorting = true });
            headerTextAndIDs.Add(new HtmlTableSettings() { columnName = "EmployeeID", columnText = "Employee ID", isColumnSupportSorting = false });
            headerTextAndIDs.Add(new HtmlTableSettings() { columnName = "ShipName", columnText = "Ship Name", isColumnSupportSorting = true });
            headerTextAndIDs.Add(new HtmlTableSettings() { columnName = "ShipAddress", columnText = "Ship Address", isColumnSupportSorting = true });

            NORTHWNDEntities context = new NORTHWNDEntities();

            var customerList = (from customer in context.Orders
                                select customer).ToList();

            if (!string.IsNullOrEmpty(SearchBy))
            {
                customerList = customerList.Where(Cust => Cust.CustomerID.Contains(SearchBy) || Cust.ShipName.Contains(SearchBy) || Cust.ShipAddress.Contains(SearchBy)).ToList();
            }


            if (!string.IsNullOrEmpty(OrderBy))
            {
                switch (OrderBy)
                {
                    case "CustomerID_Asc":
                        customerList = customerList.OrderBy(O => O.CustomerID).ToList();
                        break;

                    case "CustomerID_Desc":
                        customerList = customerList.OrderByDescending(O => O.CustomerID).ToList();
                        break;

                    case "EmployeeID_Asc":
                        customerList = customerList.OrderBy(O => O.EmployeeID).ToList();
                        break;

                    case "EmployeeID_Desc":
                        customerList = customerList.OrderByDescending(O => O.EmployeeID).ToList();
                        break;

                    case "ShipName_Asc":
                        customerList = customerList.OrderBy(O => O.ShipName).ToList();
                        break;

                    case "ShipName_Desc":
                        customerList = customerList.OrderByDescending(O => O.ShipName).ToList();
                        break;

                    case "ShipAddress_Asc":
                        customerList = customerList.OrderBy(O => O.ShipAddress).ToList();
                        break;

                    case "ShipAddress_Desc":
                        customerList = customerList.OrderByDescending(O => O.ShipAddress).ToList();
                        break;
                }
            }

            StringBuilder htmlBuilder = Pager.CreateHtmlTableWithPagination(headerTextAndIDs, customerList.ToList<object>(), OrderBy, SearchBy, true, currentPage, pageSize);

            return Json(new { data = htmlBuilder.ToString() }, JsonRequestBehavior.AllowGet);
        }

        // GET: Student
        [HttpGet]
        public ActionResult Index(string OrderBy, string SearchBy, int? currentPage, int? pageSize)
        {
            List<HtmlTableSettings> headerTextAndIDs = new List<HtmlTableSettings>();
            headerTextAndIDs.Add(new HtmlTableSettings() { columnName = "CustomerID", columnText = "Customer ID", isColumnSupportSorting = true });
            headerTextAndIDs.Add(new HtmlTableSettings() { columnName = "EmployeeID", columnText = "Employee ID", isColumnSupportSorting = false });
            headerTextAndIDs.Add(new HtmlTableSettings() { columnName = "ShipName", columnText = "Ship Name", isColumnSupportSorting = true });
            headerTextAndIDs.Add(new HtmlTableSettings() { columnName = "ShipAddress", columnText = "Ship Address", isColumnSupportSorting = true });

            NORTHWNDEntities context = new NORTHWNDEntities();
            List<Order> customerList = null;

            customerList = context.Orders.ToList();

            StringBuilder htmlBuilder = Pager.CreateHtmlTableWithPagination(headerTextAndIDs, customerList.ToList<object>(), OrderBy, SearchBy, true, currentPage, pageSize);

            ViewBag.HtmlStr = htmlBuilder.ToString();

            return View();
        }
    }
}