using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace Dynamo
{
    [DynamoDBTable("ProductCatalog")]
    class Book
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        public string Title { get; set; }
        public string ISBN { get; set; }

        [DynamoDBProperty("Authors")]
        public List<string> BookAuthors { get; set; }

        [DynamoDBIgnore]
        public string CoverPage { get; set; }
    }
}
