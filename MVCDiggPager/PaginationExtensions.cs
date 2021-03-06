﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MvcContrib.Pagination;

namespace MVCDiggPager
{
	public static class PaginationExtensions
	{
		/// <summary>
		/// Creates a pager component using the item from the viewdata with the specified key as the datasource.
		/// </summary>
		/// <param name="helper">The HTML Helper</param>
		/// <param name="viewDataKey">The viewdata key</param>
		/// <returns>A Pager component</returns>
		public static DiggPager CreatePager(this HtmlHelper helper, string viewDataKey)
		{
			var dataSource = helper.ViewContext.ViewData.Eval(viewDataKey) as IPagination;

			if(dataSource == null)
			{
				throw new InvalidOperationException(string.Format("Item in ViewData with key '{0}' is not an IPagination.",
				                                                  viewDataKey));
			}

			return helper.CreatePager(dataSource);
		}

		/// <summary>
		/// Creates a pager component using the specified IPagination as the datasource.
		/// </summary>
		/// <param name="helper">The HTML Helper</param>
		/// <param name="pagination">The datasource</param>
		/// <returns>A Pager component</returns>
		public static DiggPager CreatePager(this HtmlHelper helper, IPagination pagination)
		{
			return new DiggPager(pagination, helper.ViewContext);
		}

		public static MvcHtmlString DiggPager(this HtmlHelper helper, string viewDataKey)
		{
			return MvcHtmlString.Create(helper.CreatePager(viewDataKey).ToString());
		}

		public static MvcHtmlString DiggPager(this HtmlHelper helper, IPagination pagination)
		{
			return MvcHtmlString.Create(helper.CreatePager(pagination).ToString());
		}
	}
}
