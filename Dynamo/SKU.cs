using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace Dynamo
{
    [DynamoDBTable("SKUs")]
    class SKU
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public Dictionary<DateTime, int> MonthlyTotals { get; set; }        
    }
}
