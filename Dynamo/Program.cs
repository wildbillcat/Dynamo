using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Internal;
using Amazon.DynamoDBv2.Model;

namespace Dynamo
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new AmazonDynamoDBConfig();
            config.ServiceURL = "http://localhost:8000";
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(config);
            string tableName = "ProductCatalog";

            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "N"
                    }
                },
                        KeySchema = new List<KeySchemaElement>()
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH"  //Partition key
                    }
                    },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                }
            };

            CreateTableResponse response = client.CreateTable(request);
            
            var tableDescription = response.TableDescription;
            Console.WriteLine("{1}: {0} \t ReadCapacityUnits: {2} \t WriteCapacityUnits: {3}",
                            tableDescription.TableStatus,
                            tableDescription.TableName,
                            tableDescription.ProvisionedThroughput.ReadCapacityUnits,
                            tableDescription.ProvisionedThroughput.WriteCapacityUnits);

            string status = tableDescription.TableStatus;
            Console.WriteLine(tableName + " - " + status);

            var res = client.DescribeTable(new DescribeTableRequest { TableName = "ProductCatalog" });

            Console.ReadLine();

        }
    }
}
