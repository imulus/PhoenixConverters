using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixConverters.Models
{
    public class PropertyResult
    {
        public int ContentId { get; set; }
        public string ContentName { get; set; }
        public string PropertyAlias { get; set; }
        public string PropertyValue { get; set; }
        public string NewValue { get; set; }
        public bool IsCompatible { get; set; }
    }
}
