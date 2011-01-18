using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MvcContrib.Pagination;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCDiggPager
{
	/// <summary>
	/// Renders a pager component from an IPagination datasource.
	/// </summary>
	public class DiggPager
	{
		private readonly IPagination _pagination;
		private readonly ViewContext _viewContext;

		//private string _paginationFormat = "Showing {0} - {1} of {2} ";
		//private string _paginationSingleFormat = "Showing {0} of {1} ";
		//private string _paginationFirst = "first";
		private string _paginationPrev = "<";
		private string _paginationNext = ">";
		//private string _paginationLast = "last";
		private string _pageQueryName = "page";
		private Func<int, string> _urlBuilder;

		/// <summary>
		/// Creates a new instance of the Pager class.
		/// </summary>
		/// <param name="pagination">The IPagination datasource</param>
		/// <param name="context">The view context</param>
		public DiggPager(IPagination pagination, ViewContext context)
		{
			_pagination = pagination;
			_viewContext = context;

			_urlBuilder = CreateDefaultUrl;
		}

		protected ViewContext ViewContext
		{
			get { return _viewContext; }
		}


		/// <summary>
		/// Specifies the query string parameter to use when generating pager links. The default is 'page'
		/// </summary>
		public DiggPager QueryParam(string queryStringParam)
		{
			_pageQueryName = queryStringParam;
			return this;
		}
	
		/// <summary>
		/// Text for the 'prev' link
		/// </summary>
		public DiggPager Previous(string previous)
		{
			_paginationPrev = previous;
			return this;
		}

		/// <summary>
		/// Text for the 'next' link
		/// </summary>
		public DiggPager Next(string next)
		{
			_paginationNext = next;
			return this;
		}

		/// <summary>
		/// Uses a lambda expression to generate the URL for the page links.
		/// </summary>
		/// <param name="urlBuilder">Lambda expression for generating the URL used in the page links</param>
		public DiggPager Link(Func<int, string> urlBuilder)
		{
			_urlBuilder = urlBuilder;
			return this;
		}

		// For backwards compatibility with WebFormViewEngine
		public override string ToString()
		{
			return ToHtmlString();
		}

		public string ToHtmlString()
		{
			string ret = RenderHtml();
			return ret;
		}

		
		private string CreateDefaultUrl(int pageNumber)
		{
			var routeValues = new RouteValueDictionary();

			foreach (var key in _viewContext.RequestContext.HttpContext.Request.QueryString.AllKeys.Where(key => key != null))
			{
				routeValues[key] = _viewContext.RequestContext.HttpContext.Request.QueryString[key];
			}

			routeValues[_pageQueryName] = pageNumber;

			var url = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, _viewContext.RequestContext, true);
			return url;
		}

		private string CreatePageLink(string text, int pageNumber)
		{
			var builder = new TagBuilder("a");
			builder.SetInnerText(text);
			builder.MergeAttribute("href", _urlBuilder(pageNumber));
			return builder.ToString(TagRenderMode.Normal);
		}

		private string CreateDisabledItem(string text)
		{
			var builder = new TagBuilder("span");
			builder.SetInnerText(text);
			builder.AddCssClass("disabled");
			return builder.ToString(TagRenderMode.Normal);
		}

		private string CreateCurrentItem(string text)
		{
			var builder = new TagBuilder("span");
			builder.SetInnerText(text);
			builder.AddCssClass("current");
			return builder.ToString(TagRenderMode.Normal);
		}

		public string RenderHtml()
		{
			int pageCount = (int)Math.Ceiling(this._pagination.TotalItems / (double)this._pagination.PageSize);
			int nrOfPagesToDisplay = 10;

			var sb = new StringBuilder();

			// Previous
			if (this._pagination.PageNumber > 1)
			{
				sb.Append(CreatePageLink(_paginationPrev, this._pagination.PageNumber - 1));
			}
			else
			{
				sb.Append(CreateDisabledItem(_paginationPrev));
				//sb.AppendFormat("<span class=\"disabled\">{0}</span>", _paginationPrev);
			}

			int start = 1;
			int end = pageCount;

			if (pageCount > nrOfPagesToDisplay)
			{
				int middle = (int)Math.Ceiling(nrOfPagesToDisplay / 2d) - 1;
				int below = (this._pagination.PageNumber - middle);
				int above = (this._pagination.PageNumber + middle);

				if (below < 4)
				{
					above = nrOfPagesToDisplay;
					below = 1;
				}
				else if (above > (pageCount - 4))
				{
					above = pageCount;
					below = (pageCount - nrOfPagesToDisplay);
				}

				start = below;
				end = above;
			}

			if (start > 3)
			{
				sb.Append(CreatePageLink("1", 1));
				sb.Append(CreatePageLink("2", 2));
				sb.Append("...");
			}
			for (int i = start; i <= end; i++)
			{
				if (i == this._pagination.PageNumber)
				{
					sb.Append(CreateCurrentItem(i.ToString()));
					//sb.AppendFormat("<span class=\"current\">{0}</span>", i);
				}
				else
				{
					sb.Append(CreatePageLink(i.ToString(), i));
				}
			}
			if (end < (pageCount - 3))
			{
				sb.Append("...");
				sb.Append(CreatePageLink((pageCount - 1).ToString(), pageCount - 1));
				sb.Append(CreatePageLink(pageCount.ToString(), pageCount));
			}

			// Next
			if (this._pagination.PageNumber < pageCount)
			{
				sb.Append(CreatePageLink(_paginationNext, (this._pagination.PageNumber + 1)));
			}
			else
			{
				sb.Append(CreateDisabledItem(_paginationNext));
			}
			return sb.ToString();
		}

		//private string _GeneratePageLink(string linkText, int pageNumber)
		//{
		//    string linkFormat = "<a href=\"{0}\">{1}</a>";
		//    return CreatePageLink(pageNumber, linkText);
		//    return String.Format(linkFormat, _urlBuilder(pageNumber), linkText);
		//}
	}
}
