using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Internal;
using Amazon.DynamoDBv2.Model;
using CsvHelper;

namespace Dynamo
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new AmazonDynamoDBConfig();
            config.ServiceURL = "http://localhost:8000";
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(config);
            string tableName = "MonthlyTotals";
            bool tableExists = false;
            string lastEvaluatedTableName = null;
            do
            {
                // Create a request object to specify optional parameters.
                var req = new ListTablesRequest
                {
                    Limit = 10, // Page size.
                    ExclusiveStartTableName = lastEvaluatedTableName
                };

                var tblres = client.ListTables(req);
                foreach (string name in tblres.TableNames) {
                    if (name.Equals(tableName))
                    {
                        tableExists = true;
                        break;
                    }
                }
                if (tableExists)
                {
                    break;
                }
                lastEvaluatedTableName = tblres.LastEvaluatedTableName;

            } while (lastEvaluatedTableName != null);

            if (!tableExists)
            {
                //Table doesnt exist, lets create it
                var request = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = new List<AttributeDefinition>()
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "SKUId",
                            AttributeType = "N"
                        },
                        new AttributeDefinition
                        {
                        AttributeName = "Month",
                        AttributeType = "S"
                        }
                    },
                        KeySchema = new List<KeySchemaElement>()
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "SKUId",
                            KeyType = "HASH"  //Partition key
                        },
                        new KeySchemaElement
                        {
                            AttributeName = "Month",
                            KeyType = "RANGE"
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
            }

            var res = client.DescribeTable(new DescribeTableRequest { TableName = tableName });
            Console.WriteLine(tableName + " - " + res.Table.TableStatus);
            Console.WriteLine();
            Console.WriteLine("Test Crud?");
            Console.ReadLine();

            DynamoDBContext context = new DynamoDBContext(client);
            TestCRUDOperations(context);
            Console.ReadLine();
        }

        private static void TestCRUDOperations(DynamoDBContext context)
        {
            List<MonthlyTotal> Import = new List<MonthlyTotal>();
            string oneTimeFile = @"C: \Users\wildbillcat\Downloads\unknown_20160206082017.csv";
            int i = 0;
            using (CsvReader csv = new CsvReader(System.IO.File.OpenText(oneTimeFile)))
            {
                CsvReader Header = new CsvReader(System.IO.File.OpenText(oneTimeFile));
                Header.Read();
                string parsedate = Header.FieldHeaders[2].Substring(21, 10);
                Header.Dispose();
                DateTime ImportDate = Convert.ToDateTime(parsedate);
                while (csv.Read())
                {
                    //MonthlyTotal SKUDocument = context.Load<MonthlyTotal>(SKUNumber, ImportDate, new DynamoDBOperationConfig { ConsistentRead = true });
                    //if(SKUDocument == null)
                    //{
                    //    //SKU isnt in Datebase, will have to create it.
                    //    SKUDocument = new MonthlyTotal()
                    //    {
                    //        SKUId = SKUNumber,
                    //        Month = ImportDate,
                    //        Total = csv.GetField<double>(2)
                    //    };
                    //}
                    MonthlyTotal SKUDocument = new MonthlyTotal()
                    {
                        SKUId = csv.GetField<int>(0),
                        Month = ImportDate,
                        Total = csv.GetField<double>(2)
                    };
                    //context.Save(SKUDocument);
                    Import.Add(SKUDocument);
                    i++;
                    if(i > 1000)
                    {
                        i = 0;
                        Console.Write("SKU :");
                        Console.WriteLine(SKUDocument.SKUId);
                    }
                }
            }
            var importBatch = context.CreateBatchWrite<MonthlyTotal>();
            importBatch.AddPutItems(Import);
            
            importBatch.Execute();
            Console.WriteLine("Wrote to NoSQL DB");
            Console.WriteLine();
        }
    }
}
