using MusicShop.DAL;
using MusicShop.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.Infrastructure
{
    public class ProductDetailsDynamicNodeProvider : DynamicNodeProviderBase
    {
        private StoreContext db = new StoreContext();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            var returnValue = new List<DynamicNode>();

            foreach (Album a in db.Albums)
            {
                DynamicNode n = new DynamicNode();
                n.Title = a.AlbumTitle;
                n.Key = "Album_" + a.AlbumId;
                n.ParentKey = "Genre_" + a.GenreId;
                n.RouteValues.Add("id", a.AlbumId);
                returnValue.Add(n);
            }

            return returnValue;
        }
    }
}