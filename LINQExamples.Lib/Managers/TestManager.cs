using LINQExamples.Lib.Classes;
using LINQExamples.Lib.Model;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQExamples.Lib.Managers
{
    public static class TestManager
    {
        public static void BasicsTest()
        {
            List<StoreEmployee> members = new List<StoreEmployee>() {
                new StoreEmployee() {FirstName = "Tony", LastName = "Jefferson", BirthDate = new DateTime(1955,9,25), JobTitle = "Store Manager", ID = 1},
                new StoreEmployee() {FirstName = "Marcia", LastName = "Levinson", BirthDate = new DateTime(1998,3,1), JobTitle = "Produce Manager", ID = 2},
                new StoreEmployee() {FirstName = "Alex", LastName = "Gonzalez", BirthDate = new DateTime(1989,1,15), JobTitle = "Cashier", ID = 3},
                new StoreEmployee() {FirstName = "Mikhail", LastName = "Severin", BirthDate = new DateTime(1977,4,28), JobTitle = "Stocking Manager", ID = 4},
                new StoreEmployee() {FirstName = "Travis", LastName = "Ishikawa", BirthDate = new DateTime(1983,10,1), JobTitle = "Public Relations Specialist", ID = 5},
                new StoreEmployee() {FirstName = "Grace", LastName = "Jones", BirthDate = new DateTime(1960,11,1), JobTitle = "Quality Control Specialist", ID = 6},
                new StoreEmployee() {FirstName = "Leah", LastName = "Goldman", BirthDate = new DateTime(1997,1,1), JobTitle = "Cashier", ID = 7},
                new StoreEmployee() {FirstName = "Esmail", LastName = "Salas", BirthDate = new DateTime(1997,5,31), JobTitle = "Lead Cashier", ID = 8}
            };

            var results = from m in members
                          where m.BirthDate < new DateTime(2010, 1, 1)
                          select m;

            

            var complexWhereResults = from m in members
                                      where m.BirthDate < new DateTime(2010, 1, 1) && m.JobTitle.Contains("Manager")
                                      select m;

            var orderByResults = from m in members
                                 where m.BirthDate < new DateTime(2010, 1, 1) && m.JobTitle.Contains("Manager")
                                 orderby m.BirthDate
                                 select m;

            var orderByDescResults = from m in members
                                     where m.BirthDate < new DateTime(2010, 1, 1) && m.JobTitle.Contains("Manager")
                                     orderby m.BirthDate descending
                                     select m;

            var orderByComplexResults = from m in members
                                        where m.BirthDate < new DateTime(2010, 1, 1) && m.JobTitle.Contains("Manager")
                                        orderby m.BirthDate descending, m.LastName
                                        select m;

            Console.WriteLine("Managers: ");
            foreach(var manager in orderByComplexResults)
            {
                Console.WriteLine(manager.FirstName + " " + manager.LastName);
            }
            Console.WriteLine(Environment.NewLine);


            var namesOnlyResults = from m in members
                                   where m.BirthDate < new DateTime(2010, 1, 1)
                                   select new { m.FirstName, m.LastName };

            Console.WriteLine("Seperate Names:");
            foreach(var name in namesOnlyResults)
            {
                Console.WriteLine("Name: " + name.FirstName + " " + name.LastName);
            }
            Console.WriteLine(Environment.NewLine);

            var renamedPropertiesResults = from m in members
                                           where m.BirthDate < new DateTime(2010, 1, 1)
                                           select new { m.FirstName, m.LastName, FullName = m.FirstName + " " + m.LastName, ID = m.ID };

            Console.WriteLine("Combined Names:");
            foreach (var name in renamedPropertiesResults)
            {
                Console.WriteLine("Name: " + name.FullName);
            }
            Console.WriteLine(Environment.NewLine);
        }


        public static void EntitiesBasicsTest()
        {
            using (NorthwindEntities context = new NorthwindEntities())
            {
                var customers = from c in context.Customers
                                where c.ContactName.Contains("Mar")
                                orderby c.City, c.Country
                                select c;

                var customersList = customers.ToList();

                foreach (var customer in customersList)
                {
                    Console.WriteLine("Customer: " + customer.ContactName);
                }
            }
        }


        


        public static void AggregatesTest(int price)
        {
            using(NorthwindEntities context = new NorthwindEntities())
            {
                var products = from p in context.Products
                               where p.UnitPrice < price
                               select p;

                var hasProducts = products.Any();

                var hasProductsLambda = products.Any(x => x.UnitPrice < price);

                var allProducts = products.All(x => x.UnitPrice < price);

                var totalPrice = products.Sum(x => x.UnitPrice);
                Console.WriteLine("Total Price: $" + totalPrice.ToString());

                var totalProducts = products.Count();
                Console.WriteLine("# of Products: " + totalProducts.ToString());

                var totalProductsWhere = products.Count(x => x.UnitPrice < price);
                Console.WriteLine("# of Products (Price < $" + price.ToString() + "): " + totalProductsWhere.ToString());

                var maxPrice = products.Max(x => x.UnitPrice);
                Console.WriteLine("Maximum Price: $" + maxPrice.ToString());
            }
        }


        public static int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
        public static int[] numbersB = { 1, 3, 5, 7, 8 };
        public static void SelectManyTest()
        {
            var results = from a in numbersA
                          from b in numbersB
                          where a < b
                          select new { a, b };

            foreach (var res in results)
            {
                Console.WriteLine("{" + res.a.ToString() + ", " + res.b.ToString() + "}");
            }

            using (NorthwindEntities context = new NorthwindEntities())
            {
                var orders = from o in context.Orders
                             from c in context.Customers
                             where o.OrderDate < new DateTime(1998, 1, 1)
                             select new { o.OrderID, o.OrderDate, c.ContactName };

                foreach (var order in orders)
                {
                    Console.WriteLine("ID: " + order.OrderID + ", Date: " + order.OrderDate.Value.ToShortDateString() + ", Contact: " + order.ContactName);
                }
            }
        }

        public static void JoinTest()
        {
            using(NorthwindEntities context = new NorthwindEntities())
            {
                string[] categoryNames = new string[]{  
                    "Beverages",   
                    "Condiments",   
                    "Vegetables",   
                    "Dairy Products",   
                    "Seafood" };

                //Cross join (Every result is one category name and one product name, AKA a flat list)
                var products = from c in categoryNames
                               join p in context.Products on c equals p.Category.CategoryName
                               select new { Category = c, p.ProductName };

                foreach(var product in products)
                {
                    Console.WriteLine(product.Category + ": " + product.ProductName);
                }

                //Group join (each result is one category name and a list of product names, a multidimensional list)
                var categories = from c in categoryNames
                                 join p in context.Products on c equals p.Category.CategoryName into ps
                                 select new { Category = c, Products = ps };
                foreach (var category in categories)
                {
                    foreach (var product in category.Products)
                    {
                        Console.WriteLine(category.Category + ": " + product.ProductName);
                    }
                }

                //Group join method syntax 
                var categoriesMethod = categoryNames
                                               .GroupJoin(
                                                  context.Products,
                                                  c => c,
                                                  p => p.Category.CategoryName,
                                                  (c, ps) =>
                                                     new
                                                     {
                                                         Category = c,
                                                         Products = ps
                                                     }
                                               );


                //Left outer join (All items from set A [Categories], each related to a Product Name, where the Product Name can be empty if no products are related to the Category)
                var leftOuterJoin = from c in context.Categories
                                    join p in context.Products on c equals p.Category into ps
                                    from p in ps.DefaultIfEmpty()
                                    select new { Category = c, ProductName = p == null ? "(No products)" : p.ProductName };

                foreach (var leftGroup in leftOuterJoin)
                {
                    Console.WriteLine(leftGroup.Category.CategoryName + ": " + leftGroup.ProductName);
                }
            }
        }


        public static void GroupingTest()
        {
            using(NorthwindEntities context = new NorthwindEntities())
            {
                //Get each category, and a set of products for that category
                var groupedProducts = from p in context.Products
                                      group p by p.Category.CategoryName into g
                                      select new { Category = g.Key, Products = g };

                foreach (var category in groupedProducts)
                {
                    foreach (var product in category.Products)
                    {
                        Console.WriteLine(category.Category + ": " + product.ProductName);
                    }
                }

                //Get all orders, split by month and year
                var customerOrderGroups = from c in context.Customers
                                          select new
                                          {
                                              c.CompanyName,
                                              YearGroups = from o in c.Orders
                                                           where o.OrderDate.HasValue
                                                           group o by o.OrderDate.Value.Year into yg
                                                           select new
                                                           {
                                                               Year = yg.Key,
                                                               MonthGroups = from o in yg
                                                                             group o by o.OrderDate.Value.Month into mg
                                                                             select new { Month = mg.Key, Orders = mg }
                                                           }

                                          };

                foreach (var orderGroup in customerOrderGroups)
                {
                    foreach (var year in orderGroup.YearGroups)
                    {
                        foreach (var month in year.MonthGroups)
                        {
                            Console.WriteLine(orderGroup.CompanyName + " in " + month.Month + "/" + year.Year + ": " + month.Orders.Count().ToString());
                        }
                    }
                }

                var customersOrderGroupsMethod = context.Customers
                                                           .OrderBy(c => c.CompanyName)
                                                           .Select(
                                                              c =>
                                                                 new
                                                                 {
                                                                     CompanyName = c.CompanyName,
                                                                     YearGroups = c.Orders.Where(o => o.OrderDate.HasValue).GroupBy(o => o.OrderDate.Value.Year)
                                                                        .Select(
                                                                           yg =>
                                                                              new
                                                                              {
                                                                                  Year = yg.Key,
                                                                                  MonthGroups = yg.GroupBy(o => o.OrderDate.Value.Month)
                                                                                     .Select(
                                                                                        mg =>
                                                                                           new
                                                                                           {
                                                                                               Month = mg.Key,
                                                                                               Orders = mg
                                                                                           }
                                                                                     )
                                                                              }
                                                                        )
                                                                 }
                                                           );
            }
        }


        public static void CollectionsTest()
        {
            using(NorthwindEntities context = new NorthwindEntities())
            {
                var categories = from c in context.Categories
                                 where c.CategoryID > 3
                                 select c;

                var firstCategory = categories.First();
                Console.WriteLine("First category: " + firstCategory.CategoryName);

                var firstCategoryMatched = categories.First(x => x.CategoryName == "Produce");

                var firstCategoryDefault = categories.FirstOrDefault(x => x.CategoryName == "Nuts"); //returns null

                //var singleCategory = categories.Single(); //throws exception

                var singleCategoryMatched = categories.Single(x => x.CategoryName == "Produce");
            }
        }


        public static void SetTest()
        {
            using(NorthwindEntities context = new NorthwindEntities())
            {
                var categories = (from c in context.Categories
                                  select c.CategoryName).Distinct();

                var productsFirstLetters = (from p in context.Products
                                            select p.ProductName).ToList().Select(x => x[0]);

                var customersFirstLetters = (from c in context.Customers
                                             select c.CompanyName).ToList().Select(x => x[0]);

                var unionLetters = productsFirstLetters.Union(customersFirstLetters).OrderBy(x => x);

                var intersectLetters = productsFirstLetters.Intersect(customersFirstLetters).OrderBy(x => x);

                var exceptLetters = productsFirstLetters.Except(customersFirstLetters).OrderBy(x => x);


                string unionOutput = "UNION Letters: ", intersectOutput = "INTERSECT Letters: ", exceptOutput = "EXCEPT Letters: ";

                foreach(var unionLetter in unionLetters)
                {
                    unionOutput += unionLetter + ", ";
                }
                unionOutput = unionOutput.Remove(unionOutput.LastIndexOf(","));


                foreach (var intersectLetter in intersectLetters)
                {
                    intersectOutput += intersectLetter + ", ";
                }
                intersectOutput = intersectOutput.Remove(intersectOutput.LastIndexOf(","));


                foreach (var exceptLetter in exceptLetters)
                {
                    exceptOutput += exceptLetter + ", ";
                }
                exceptOutput = exceptOutput.Remove(exceptOutput.LastIndexOf(","));

                Console.WriteLine(unionOutput);
                Console.WriteLine(intersectOutput);
                Console.WriteLine(exceptOutput);
            }
        }

        public static void ErrorTest()
        {
            using (NorthwindEntities context = new NorthwindEntities())
            {
                var customers = context.Customers.ToList();

                var errorTime = DateTime.Now;
                foreach (var customer in customers)
                {
                    var orders = customer.Orders;
                    foreach(var order in orders)
                    {
                        Console.WriteLine("Order #" + order.OrderID.ToString() + ", placed on " + order.OrderDate.Value.ToShortDateString());
                    }
                }

                var errorNewTime = DateTime.Now;
                var errorDifference = errorNewTime.Subtract(errorTime).Milliseconds;

                var time = DateTime.Now;
                var includedCustomers = context.Customers.Include(x => x.Orders).ToList();
                foreach(var customer in includedCustomers)
                {
                    var orders = customer.Orders;
                    foreach (var order in orders)
                    {
                        Console.WriteLine("Order #" + order.OrderID.ToString() + ", placed on " + order.OrderDate.Value.ToShortDateString());
                    }
                }

                var newTime = DateTime.Now;
                var difference = newTime.Subtract(time).Milliseconds;

                Console.WriteLine("Milliseconds with error: " + errorDifference.ToString());
                Console.WriteLine("Milliseconds without error: " + difference.ToString());
            }
        }
    }
}
