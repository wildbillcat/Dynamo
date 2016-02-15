using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace Dynamo
{
    [DynamoDBTable("MonthlyTotals")]
    class MonthlyTotal
    {
        [DynamoDBHashKey]
        public int SKUId { get; set; }

        [DynamoDBRangeKey]
        public DateTime Month { get; set; }

        public double Total { get; set; }
        
    }
}
