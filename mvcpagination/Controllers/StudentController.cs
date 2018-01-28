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
        public JsonResult getTable(string OrderBy, string SearchBy, int? currentPage,int? pageSize)
        {
            Dictionary<string, string> headerTextAndIDs = new Dictionary<string, string>();
            headerTextAndIDs.Add("CustomerID", "Customer ID");
            headerTextAndIDs.Add("EmployeeID", "Employee ID");
            headerTextAndIDs.Add("ShipName", "Ship Name");
            headerTextAndIDs.Add("ShipAddress", "Ship Address");

            NORTHWNDEntities context = new NORTHWNDEntities();
            int totalRecords = 0;
            Pager pagerSettings = null;
            bool IsOrderByAppliedOnAnyColumn = false;
            

            var customerList = (from customer in context.Orders
                               select customer).ToList();

            if (!string.IsNullOrEmpty(SearchBy))
            {
                customerList = customerList.Where(Cust => Cust.CustomerID.Contains(SearchBy) || Cust.ShipName.Contains(SearchBy) || Cust.ShipAddress.Contains(SearchBy)).ToList();
            }


            if (!string.IsNullOrEmpty(OrderBy))
            {
                IsOrderByAppliedOnAnyColumn = true;

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


            totalRecords = customerList.Count();
            pagerSettings = new Pager().GetPager(totalRecords, currentPage,pageSize);
            if (IsOrderByAppliedOnAnyColumn == false)
            {
                customerList = customerList.Skip(pagerSettings.startIndex).Take(pagerSettings.pageSize).ToList();
            }
            else
            {
                customerList = customerList.Skip(pagerSettings.startIndex).Take(pagerSettings.pageSize).ToList();
            }

            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder = Pager.CreateHtmlFilterSearchBlock(htmlBuilder, SearchBy,pagerSettings.pageSize);

            htmlBuilder = Pager.CreateHtmlTableStartBlock(htmlBuilder);

            htmlBuilder = Pager.CreateHtmlTableHeaderBlock(htmlBuilder, headerTextAndIDs,OrderBy);

            htmlBuilder = Pager.CreateHtmlTableBodyFromList(htmlBuilder, customerList.ToList<object>(), headerTextAndIDs,true);

            htmlBuilder = Pager.CreateHtmlTableEndBlock(htmlBuilder);

            htmlBuilder = Pager.CreateHtmlPagerLinksBlock(htmlBuilder, pagerSettings);


            return Json(new { data = htmlBuilder.ToString()} ,JsonRequestBehavior.AllowGet);
        }

        // GET: Student
        [HttpGet]
        public ActionResult Index(string OrderBy, string SearchBy, int? currentPage, int? pageSize)
        {
            Dictionary<string, string> headerTextAndIDs = new Dictionary<string, string>();
            headerTextAndIDs.Add("CustomerID", "Customer ID");
            headerTextAndIDs.Add("EmployeeID", "Employee ID");
            headerTextAndIDs.Add("ShipName", "Ship Name");
            headerTextAndIDs.Add("ShipAddress", "Ship Address");

            NORTHWNDEntities context = new NORTHWNDEntities();
            List<Order> customerList = null;
            int totalRecords = 0;
            Pager pagerSettings = null;

            customerList = context.Orders.ToList();
            totalRecords = customerList.Count();
            pagerSettings = new Pager().GetPager(totalRecords, currentPage,pageSize);
            customerList = customerList.Skip(pagerSettings.startIndex).Take(pagerSettings.pageSize).ToList();

            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder = Pager.CreateHtmlFilterSearchBlock(htmlBuilder, SearchBy,pagerSettings.pageSize);

            htmlBuilder = Pager.CreateHtmlTableStartBlock(htmlBuilder);

            htmlBuilder = Pager.CreateHtmlTableHeaderBlock(htmlBuilder, headerTextAndIDs,OrderBy);

            htmlBuilder = Pager.CreateHtmlTableBodyFromList(htmlBuilder, customerList.ToList<object>(), headerTextAndIDs,true);

            htmlBuilder = Pager.CreateHtmlTableEndBlock(htmlBuilder);

            htmlBuilder = Pager.CreateHtmlPagerLinksBlock(htmlBuilder, pagerSettings);

            ViewBag.HtmlStr = htmlBuilder.ToString();

            return View();
        }
    }
}