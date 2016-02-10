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
            //string tableName = "ProductCatalog";

            //var request = new CreateTableRequest
            //{
            //    TableName = tableName,
            //    AttributeDefinitions = new List<AttributeDefinition>()
            //    {
            //        new AttributeDefinition
            //        {
            //            AttributeName = "Id",
            //            AttributeType = "N"
            //        }
            //    },
            //            KeySchema = new List<KeySchemaElement>()
            //    {
            //        new KeySchemaElement
            //        {
            //            AttributeName = "Id",
            //            KeyType = "HASH"  //Partition key
            //        }
            //        },
            //    ProvisionedThroughput = new ProvisionedThroughput
            //    {
            //        ReadCapacityUnits = 10,
            //        WriteCapacityUnits = 5
            //    }
            //};

            //CreateTableResponse response = client.CreateTable(request);

            //var tableDescription = response.TableDescription;
            //Console.WriteLine("{1}: {0} \t ReadCapacityUnits: {2} \t WriteCapacityUnits: {3}",
            //                tableDescription.TableStatus,
            //                tableDescription.TableName,
            //                tableDescription.ProvisionedThroughput.ReadCapacityUnits,
            //                tableDescription.ProvisionedThroughput.WriteCapacityUnits);

            //string status = tableDescription.TableStatus;
            //Console.WriteLine(tableName + " - " + status);

            //var res = client.DescribeTable(new DescribeTableRequest { TableName = "ProductCatalog" });
            Console.WriteLine("Test Crud?");
            Console.ReadLine();

            DynamoDBContext context = new DynamoDBContext(client);
            TestCRUDOperations(context);
            Console.ReadLine();
        }

        private static void TestCRUDOperations(DynamoDBContext context)
        {
            int bookID = 1001; // Some unique value.
            Book myBook = new Book
            {
                Id = bookID,
                Title = "object persistence-AWS SDK for.NET SDK-Book 1001",
                ISBN = "111-1111111001",
                BookAuthors = new List<string> { "Author 1", "Author 2" },
            };

            // Save the book.
            context.Save(myBook);
            // Retrieve the book. 
            Book bookRetrieved = context.Load<Book>(bookID);
            Console.WriteLine(string.Concat("Title: ", bookRetrieved.Title));
            Console.WriteLine(string.Concat("ISBN: ", bookRetrieved.ISBN));
            foreach (string author in bookRetrieved.BookAuthors) {
                Console.WriteLine(string.Concat("BookAuthor: ", author));
            }
            Console.WriteLine();

            // Update few properties.
            bookRetrieved.ISBN = "222-2222221001";
            bookRetrieved.BookAuthors = new List<string> { " Author 1", "Author x" }; // Replace existing authors list with this.
            context.Save(bookRetrieved);

            // Retrieve the updated book. This time add the optional ConsistentRead parameter using DynamoDBContextConfig object.
            Book updatedBook = context.Load<Book>(bookID, new DynamoDBContextConfig { ConsistentRead = true });
            Console.WriteLine(string.Concat("Updated Title: ", updatedBook.Title));
            Console.WriteLine(string.Concat("Updated ISBN: ", updatedBook.ISBN));
            foreach (string author in updatedBook.BookAuthors)
            {
                Console.WriteLine(string.Concat("BookAuthor: ", author));
            }
            Console.WriteLine();

            // Delete the book.
            context.Delete<Book>(bookID);
            // Try to retrieve deleted book. It should return null.
            Book deletedBook = context.Load<Book>(bookID, new DynamoDBContextConfig { ConsistentRead = true });
            if (deletedBook == null)
                Console.WriteLine("Book is deleted");

        }
    }
}
