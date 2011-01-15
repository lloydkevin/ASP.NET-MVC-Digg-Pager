using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcContrib.UI.Grid;
using MvcContrib.Sorting;
using MvcContrib.Pagination;

namespace PagerTest.Controllers
{
	public class Customer
	{
		public string Name {get; set;}
	}
	
	public class HomeController : Controller
	{
		public ActionResult Index(GridSortOptions sort, int? page)
		{
			List<Customer> list = new List<Customer>();
			for (int i = 0; i < 2500; i++)
			{
				list.Add(new Customer() { Name = "Name " + i.ToString() });
			}

			IEnumerable<Customer> ret = list.AsEnumerable();
			if (sort.Column != null)
			{
				ret = ret.OrderBy(sort.Column, sort.Direction);
			}

			ret = ret.AsPagination(page ?? 1, 10);
			
			return View(ret);
		}

		public ActionResult About()
		{
			return View();
		}
	}
}
