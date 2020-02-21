using MusicShop.DAL;
using MusicShop.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop.Infrastructure
{
    public class ProductListDynamicNodeProvider : DynamicNodeProviderBase
    {
        private StoreContext db = new StoreContext();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            var returnValue = new List<DynamicNode>();

            foreach (Genre g in db.Genres)
            {
                DynamicNode n = new DynamicNode();
                n.Title = g.Name;
                n.Key = "Genre_" + g.GenreId;
                n.RouteValues.Add("genrename", g.Name.ToLower());
                returnValue.Add(n);
            }

            return returnValue;
        }
    }
}