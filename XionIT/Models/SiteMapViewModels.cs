using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace XionIT.Models
{

	public class MapItem
	{
		public string Name { get; set; }

		public string Action { get; set; }
		public string Controller { get; set; }
		public ICollection<MapItem> Children { get; set; }
		
		public bool RequiresAuth { get; set; }

		public bool HasParams { get; set; }

		public MapItem()
		{
			Children = new HashSet<MapItem>();
		}
		public MapItem(string action, string controller, string name = null, bool requiresAuth = false)
			: this()
		{
			Action = action;
			Controller = controller;
			RequiresAuth = requiresAuth;
			Name = name ?? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Action);
		}

		public static MapItem Create(string action, string controller, string name = null, bool requiresAuth = false)
		{
			return new MapItem(action, controller, name, requiresAuth);
		}

	}

	public static class MapItemExt
	{
		public static MapItem AddChild(this MapItem map, string action, string controller = null, bool hasParams = false, string name = null, bool? requiresAuth = null)
		{
			bool rAuth = (requiresAuth.HasValue) ? requiresAuth.Value : map.RequiresAuth;

			map.Children.Add(new MapItem(action, controller ?? map.Controller, name, rAuth) { HasParams = hasParams });
			return map;
		}
	}

	public interface ISitemapGenerator
	{
		XDocument GenerateSiteMap(IEnumerable<ISitemapItem> items);
	}

	public interface ISitemapItem
	{
		/// <summary>
		/// URL of the page.
		/// </summary>
		string Url { get; }

		/// <summary>
		/// The date of last modification of the file.
		/// </summary>
		DateTime? LastModified { get; }

		/// <summary>
		/// How frequently the page is likely to change.
		/// </summary>
		SitemapChangeFrequency? ChangeFrequency { get; }

		/// <summary>
		/// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0.
		/// </summary>
		double? Priority { get; }
	}

	public enum SitemapChangeFrequency
	{
		Always,
		Hourly,
		Daily,
		Weekly,
		Monthly,
		Yearly,
		Never
	}

	public class SitemapGenerator : ISitemapGenerator
	{
		private static readonly XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
		private static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

		public virtual XDocument GenerateSiteMap(IEnumerable<ISitemapItem> items)
		{
			Ensure.Argument.NotNull(items, "items");

			var sitemap = new XDocument(
				new XDeclaration("1.0", "utf-8", "yes"),
					new XElement(xmlns + "urlset",
					  new XAttribute("xmlns", xmlns),
					  new XAttribute(XNamespace.Xmlns + "xsi", xsi),
					  new XAttribute(xsi + "schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"),
					  from item in items
					  select CreateItemElement(item)
					  )
				 );

			return sitemap;
		}

		private XElement CreateItemElement(ISitemapItem item)
		{
			var itemElement = new XElement(xmlns + "url", new XElement(xmlns + "loc", item.Url.ToLowerInvariant()));

			// all other elements are optional

			if (item.LastModified.HasValue)
				itemElement.Add(new XElement(xmlns + "lastmod", item.LastModified.Value.ToString("yyyy-MM-dd")));

			if (item.ChangeFrequency.HasValue)
				itemElement.Add(new XElement(xmlns + "changefreq", item.ChangeFrequency.Value.ToString().ToLower()));

			if (item.Priority.HasValue)
				itemElement.Add(new XElement(xmlns + "priority", item.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));

			return itemElement;
		}
	}

	public class SitemapItem : ISitemapItem
	{
		public SitemapItem(string url, DateTime? lastModified = null, SitemapChangeFrequency? changeFrequency = null, double? priority = null)
		{
			Ensure.Argument.NotNullOrEmpty(url, "url");

			Url = url;
			LastModified = lastModified;
			ChangeFrequency = changeFrequency;
			Priority = priority;
		}

		/// <summary>
		/// URL of the page.
		/// </summary>
		public string Url { get; protected set; }

		/// <summary>
		/// The date of last modification of the file.
		/// </summary>
		public DateTime? LastModified { get; protected set; }

		/// <summary>
		/// How frequently the page is likely to change.
		/// </summary>
		public SitemapChangeFrequency? ChangeFrequency { get; protected set; }

		/// <summary>
		/// The priority of this URL relative to other URLs on your site. Valid values range from 0.0 to 1.0.
		/// </summary>
		public double? Priority { get; protected set; }
	}

	public class SitemapResult : ActionResult
	{
		private readonly IEnumerable<ISitemapItem> items;
		private readonly ISitemapGenerator generator;

		public SitemapResult(IEnumerable<ISitemapItem> items) : this(items, new SitemapGenerator())
		{
		}

		public SitemapResult(IEnumerable<ISitemapItem> items, ISitemapGenerator generator)
		{
			Ensure.Argument.NotNull(items, "items");
			Ensure.Argument.NotNull(generator, "generator");

			this.items = items;
			this.generator = generator;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			var response = context.HttpContext.Response;

			response.ContentType = "text/xml";
			response.ContentEncoding = Encoding.UTF8;

			using (var writer = new XmlTextWriter(response.Output))
			{
				writer.Formatting = Formatting.Indented;
				var sitemap = generator.GenerateSiteMap(items);

				sitemap.WriteTo(writer);
			}
		}
	}

}