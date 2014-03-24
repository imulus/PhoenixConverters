using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace PhoenixConverters.Models
{
    public class AffectedContent
    {
        public IContent Content { get; set; }
        public PropertyType PropertyType { get; set; }
    }
}
