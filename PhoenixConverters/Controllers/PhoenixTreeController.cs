using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using Umbraco.Web.Trees;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Constants = Umbraco.Core.Constants;
using Umbraco.Core;
using PhoenixConverters.Core;

namespace PhoenixConverters
{
    [Tree("developer", "phoenixTree", "Phoenix Converters")]
    [PluginController("Phoenix")]
    public class PhoenixTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            if (id == Constants.System.Root.ToInvariantString())
            {
                var tree = new TreeNodeCollection();
                foreach(var converter in PhoenixCore.GetAllConverters())
                {
                    tree.Add(CreateTreeNode(converter.Alias, id, queryStrings, converter.Name));
                }

                return tree;
            }
            else
            {
                return new TreeNodeCollection();
            }
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }
    }
}
