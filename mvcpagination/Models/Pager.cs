using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace mvcpagination.Models
{
    public class Pager
    {
        public int totalItems { get; set; }
        public int? currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public int startPage { get; set; }
        public int endPage { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public int pages { get; set; }
        static int[] pageSizeOptions = { 5, 10, 25, 50, 100 };

        #region GetPager
        public Pager GetPager(int totalItems, int? currentPage, int? pageSize)
        {
            // default to first page
            currentPage = (currentPage != null) ? currentPage : 1;

            // default page size is 10
            pageSize = (pageSize != null) ? pageSize : 10;

            // calculate total pages
            int totalPages = (int)Math.Ceiling(((double)totalItems / (double)pageSize));

            int startPage, endPage;
            if (totalPages <= 10)
            {
                // less than 10 total pages so show all
                startPage = 1;
                endPage = totalPages;
            }
            else
            {
                // more than 10 total pages so calculate start and end pages
                if (currentPage <= 6)
                {
                    startPage = 1;
                    endPage = 10;
                }
                else if (currentPage + 4 >= totalPages)
                {
                    startPage = totalPages - 9;
                    endPage = totalPages;
                }
                else
                {
                    startPage = (int)currentPage - 5;
                    endPage = (int)currentPage + 4;
                }
            }

            // calculate start and end item indexes
            int startIndex = (int)((currentPage - 1) * pageSize);
            int endIndex = Math.Min(startIndex + (int)pageSize - 1, totalItems - 1);

            this.totalItems = totalItems;
            this.currentPage = currentPage;
            this.pageSize = (int)pageSize;
            this.totalPages = totalPages;
            this.startPage = startPage;
            this.endPage = endPage;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.pages = pages;

            return this;
        }
        #endregion

        #region CreateHtmlFilterSearchBlock
        public static StringBuilder CreateHtmlFilterSearchBlock(StringBuilder htmlBuilder, string SearchBy, int pageSize,bool showPageLengthBlock=true,bool showSearchBlock=false)
        {

            if (showPageLengthBlock || showSearchBlock)
            {
                htmlBuilder.Append("<div class='row bg-success'>");

                if (showPageLengthBlock)
                {
                    htmlBuilder.Append("<div class='col-md-6'>");
                    htmlBuilder.Append("<div class='form-group'>");
                    htmlBuilder.Append("<label class='control-label'> Show : </label>");
                    htmlBuilder.Append("<select id='SelectPageSize' class='form-control'>");

                    foreach (var currentPageSize in pageSizeOptions)
                    {
                        if (currentPageSize == pageSize)
                        {
                            htmlBuilder.Append("<option value=" + currentPageSize + " selected>" + currentPageSize + "</option>");
                        }
                        else
                        {
                            htmlBuilder.Append("<option value=" + currentPageSize + ">" + currentPageSize + "</option>");
                        }
                    }

                    htmlBuilder.Append("</select>");
                    htmlBuilder.Append("</div>");
                    htmlBuilder.Append("</div>");
                }

                if (showSearchBlock)
                {

                    htmlBuilder.Append("<div class='col-md-6 pull-right'>");
                    htmlBuilder.Append("<div class='form-group pull-right'>");
                    htmlBuilder.Append("<label class='control-label'>Search : </label>");
                    htmlBuilder.Append("<input type='text' id='filter' value='" + SearchBy + "' class='form-control' />");
                    htmlBuilder.Append("</div>");
                    htmlBuilder.Append("</div>");
                }

                htmlBuilder.Append("</div>");

            }
            return htmlBuilder;
        }
        #endregion

        #region CreateHtmlTableStartBlock
        public static StringBuilder CreateHtmlTableStartBlock(StringBuilder htmlBuilder)
        {
            htmlBuilder.Append("<div class='table-responsive'>");
            htmlBuilder.Append("<table class='table table-bordered table-striped'>");

            return htmlBuilder;
        }
        #endregion

        #region CreateHtmlTableHeaderBlock
        public static StringBuilder CreateHtmlTableHeaderBlock(StringBuilder htmlBuilder, List<HtmlTableSettings> headerTextAndIDs, string OrderBy, bool isEditDeleteSuppported = true)
        {
            htmlBuilder.Append("<thead>");
            htmlBuilder.Append("<tr>");

            foreach (var item in headerTextAndIDs)
            {
                if (string.IsNullOrEmpty(OrderBy))
                {
                    if (item.isColumnSupportSorting)
                    {
                        htmlBuilder.Append("<th class='header-column' id=" + item.columnName + ">" + item.columnText + "<i class='fa fa-sort pull-right'></i></th>");
                    }
                    else
                    {
                        htmlBuilder.Append("<th class='header-column' id=" + item.columnName + ">" + item.columnText + "<i class=''></i></th>");
                    }
                }
                else
                {
                    if (OrderBy.Split('_').Count() == 2)
                    {
                        if (item.columnName.Equals(OrderBy.Split('_')[0]))
                        {
                            if (OrderBy.Split('_')[1].Equals("Asc"))
                            {
                                htmlBuilder.Append("<th class='header-column' id=" + item.columnName + ">" + item.columnText + "<i class='fa fa-sort-asc pull-right'></i></th>");
                            }
                            else if (OrderBy.Split('_')[1].Equals("Desc"))
                            {
                                htmlBuilder.Append("<th class='header-column' id=" + item.columnName + ">" + item.columnText + "<i class='fa fa-sort-desc pull-right'></i></th>");
                            }
                        }
                        else
                        {
                            if (item.isColumnSupportSorting)
                            {
                                htmlBuilder.Append("<th class='header-column' id=" + item.columnName + ">" + item.columnText + "<i class='fa fa-sort pull-right'></i></th>");
                            }
                            else
                            {
                                htmlBuilder.Append("<th class='header-column' id=" + item.columnName + ">" + item.columnText + "<i class=''></i></th>");
                            }
                        }
                    }
                }
            }

            if (isEditDeleteSuppported)
            {
                htmlBuilder.Append("<th>Edit</th>");
                htmlBuilder.Append("<th>Delete</th>");
            }

            htmlBuilder.Append("</tr>");
            htmlBuilder.Append("</thead>");

            return htmlBuilder;
        }
        #endregion

        #region CreateHtmlTableBodyFromList
        public static StringBuilder CreateHtmlTableBodyFromList(StringBuilder htmlBuilder, List<object> listOfObjects, List<HtmlTableSettings> headerTextAndIDs, bool isEditDeleteSuppported = true)
        {
            htmlBuilder.Append("<tbody>");
            foreach (var item in listOfObjects)
            {
                Dictionary<string, object> currentItem = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                               .ToDictionary(prop => prop.Name, prop => prop.GetValue(item, null));
                htmlBuilder.Append("<tr>");
                foreach (var currentColumn in headerTextAndIDs)
                {
                    if (currentItem.ContainsKey(currentColumn.columnName))
                    {
                        htmlBuilder.Append("<td>" + currentItem[currentColumn.columnName] + "</td>");
                    }
                }
                if (isEditDeleteSuppported)
                {
                    htmlBuilder.Append("<td><p data-placement='top' data-toggle='tooltip' title='Edit'><button class='btn btn-primary btn-xs' data-title='Edit' data-toggle='modal' data-target='#edit' ><span class='glyphicon glyphicon-pencil'></span></button></p></td>");
                    htmlBuilder.Append("<td><p data-placement='top' data-toggle='tooltip' title='Delete'><button class='btn btn-danger btn-xs' data-title='Delete' data-toggle='modal' data-target='#delete' ><span class='glyphicon glyphicon-trash'></span></button></p></td>");
                }
                htmlBuilder.Append("</tr>");
            }
            htmlBuilder.Append("</tbody>");
            return htmlBuilder;
        }
        #endregion

        #region CreateHtmlTableEndBlock
        public static StringBuilder CreateHtmlTableEndBlock(StringBuilder htmlBuilder)
        {
            htmlBuilder.Append("</table>");
            htmlBuilder.Append("</div>");

            return htmlBuilder;
        }
        #endregion

        #region CreateHtmlPagerLinksBlock
        public static StringBuilder CreateHtmlPagerLinksBlock(StringBuilder htmlBuilder, Pager pagerSettings)
        {
            htmlBuilder.Append("<div class='row bg-success'>");

            htmlBuilder.Append("<div class='col-md-2'>");
            htmlBuilder.Append("<ul class='text-secondary bg-light text-dark text-center pagination'>");
            if (pagerSettings.totalItems > 0)
            {
                htmlBuilder.Append("<li>Showing " + ++pagerSettings.startIndex + " To " + ++pagerSettings.endIndex + " Of " + pagerSettings.totalItems + "</li>");
            }
            else
            {
                htmlBuilder.Append("<li>Sorry No Records Found</li>");
            }
            htmlBuilder.Append("</ul>");
            htmlBuilder.Append("</div>");

            htmlBuilder.Append("<div class='col-md-10'>");
            htmlBuilder.Append("<ul class='pagination pull-right'>");
            if (pagerSettings.currentPage == 1)
            {
                htmlBuilder.Append("<li class='disabled currentPage' pagenumber='" + 1 + "'><a>First</a></li>");
                htmlBuilder.Append("<li class='disabled currentPage' pagenumber='" + (pagerSettings.currentPage - 1) + "'><a >Previous</a></li>");
            }
            else
            {
                htmlBuilder.Append("<li class='currentPage' pagenumber='" + 1 + "'><a>First</a></li>");
                htmlBuilder.Append("<li class='currentPage' pagenumber='" + (pagerSettings.currentPage - 1) + "'><a>Previous</a></li>");
            }


            for (int i = pagerSettings.startPage; i <= pagerSettings.endPage; i++)
            {
                int pagenumber = pagerSettings.startPage;
                if (pagenumber == pagerSettings.currentPage)
                {
                    htmlBuilder.Append("<li class='active currentPage'><a pagenumber='" + pagenumber + "'>" + pagenumber + "</a></li>");
                }
                else
                {
                    htmlBuilder.Append("<li class='currentPage' pagenumber='" + pagenumber + "'><a>" + pagenumber + "</a></li>");
                }
                ++pagerSettings.startPage;
            }


            if (pagerSettings.totalPages == pagerSettings.endPage)
            {
                htmlBuilder.Append("<li class='disabled currentPage'  pagenumber='" + (pagerSettings.currentPage + 1) + "'><a>Next</a></li>");
                htmlBuilder.Append("<li class='disabled currentPage'  pagenumber='" + (pagerSettings.totalPages) + "'><a>Last</a></li>");
            }
            else
            {
                htmlBuilder.Append("<li class='currentPage'  pagenumber='" + (pagerSettings.currentPage + 1) + "'><a>Next</a></li>");
                htmlBuilder.Append("<li class='currentPage'  pagenumber='" + (pagerSettings.totalPages) + "'><a>Last</a></li>");
            }
            htmlBuilder.Append("</ul>");
            htmlBuilder.Append("</div>");
            htmlBuilder.Append("</div>");

            return htmlBuilder;
        }
        #endregion

        #region CreateHtmlTableWithPagination
        public static StringBuilder CreateHtmlTableWithPagination(List<HtmlTableSettings> headerTextAndIDs, List<object> listOfObjects,string OrderBy,string SearchBy,bool isEditDeleteSuppported,int? currentPage,int? pageSize)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            Pager pagerSettings = new Pager().GetPager(listOfObjects.Count(), currentPage, pageSize);
            listOfObjects = listOfObjects.Skip(pagerSettings.startIndex).Take(pagerSettings.pageSize).ToList();

            htmlBuilder = Pager.CreateHtmlFilterSearchBlock(htmlBuilder, SearchBy, pagerSettings.pageSize);

            htmlBuilder = Pager.CreateHtmlTableStartBlock(htmlBuilder);

            htmlBuilder = Pager.CreateHtmlTableHeaderBlock(htmlBuilder, headerTextAndIDs, OrderBy);

            htmlBuilder = Pager.CreateHtmlTableBodyFromList(htmlBuilder, listOfObjects, headerTextAndIDs, true);

            htmlBuilder = Pager.CreateHtmlTableEndBlock(htmlBuilder);

            htmlBuilder = Pager.CreateHtmlPagerLinksBlock(htmlBuilder, pagerSettings);


            return htmlBuilder;
        }
        #endregion

    }

    public class HtmlTableSettings
    {
        public string columnName { get; set; }
        public string columnText { get; set; }
        public bool isColumnSupportSorting { get; set; }
    }
}