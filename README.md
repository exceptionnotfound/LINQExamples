# LINQ for Beginners

_Just want the code? Download the sample project from [GitHub](https://github.com/exceptionnotfound/LINQExamples)!_

## What is LINQ?

LINQ stands for Language INtegrated Query, a feature of .NET that was released as part of version 3.5 way back in 2007. It greatly improved the ability of C# and VB programmers to handle and parse data in business-level code.

What LINQ does is **provide a syntax that allows business-level programmers to query sets of data** without needing to know any SQL.

Let's demo some really basic queries, and then we'll start seeing examples of more complex (but exciting!) things you can do with LINQ.

We'll cover the following items in this tutorial (click on the links to jump to those sections):

* [Structure of a LINQ Query](#structure-of-a-linq-query)
* [Filtering](#filtering)
* [Ordering](#ordering)
* [Projections](#projections)
* [LINQ-to-Entities](#linq-to-entities)
* [Deferred Execution](#deferred-execution)
* [Aggregates](#aggregates)
* [Query and Method Syntaxes](#query-syntax-vs-method-syntax)
* [Joins](#joins)
* [Grouping](#grouping)
* [Skip and Take](#skip-and-take)
* [Working with Collections](#working-with-collections)
* [Set Operations](#set-operations)

### Structure of a LINQ Query

Basic LINQ queries work by specifying three things:

1. Where's the source data? (FROM)
2. Of that source data, do I want any specific data? (WHERE)
3. Which aspects of the data do I want returned? (SELECT)

In order to demonstrate this structure, we'll need a class and some test data.

```c#
public class StoreEmployee  
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JobTitle { get; set; }
    public DateTime BirthDate { get; set; }
    public int ID { get; set; }
}

List<StoreEmployee> members = new List<StoreEmployee>
{  
    new StoreEmployee { FirstName = "Tony", LastName = "Jefferson", BirthDate = new DateTime(1955,9,25), JobTitle = "Store Manager", ID = 1 },
    new StoreEmployee { FirstName = "Marcia", LastName = "Levinson", BirthDate = new DateTime(1992,3,1), JobTitle = "Produce Manager", ID = 2 },
    new StoreEmployee { FirstName = "Alex", LastName = "Gonzalez", BirthDate = new DateTime(1989,1,15), JobTitle = "Cashier", ID = 3 },
    new StoreEmployee { FirstName = "Mikhail", LastName = "Severin", BirthDate = new DateTime(1977,4,28), JobTitle = "Deli Manager", ID = 4 },
    new StoreEmployee { FirstName = "Travis", LastName = "Ishikawa", BirthDate = new DateTime(1983,10,1), JobTitle = "Public Relations Specialist", ID = 5 },
    new StoreEmployee { FirstName = "Grace", LastName = "Jones", BirthDate = new DateTime(1960,11,1), JobTitle = "Quality Control Specialist", ID = 6 },
    new StoreEmployee { FirstName = "Leah", LastName = "Goldman", BirthDate = new DateTime(1997,1,1), JobTitle = "Cashier", ID = 7 },
    new StoreEmployee { FirstName = "Esmail", LastName = "Salas", BirthDate = new DateTime(1997,5,31), JobTitle = "Lead Cashier", ID = 8 }
};
```

### Filtering

Using this collection of Store Employees, we can start writing some simple LINQ queries.

First, let's get the Store Employees who were born after 1980:

```c#
var younguns = from m in members
               where m.BirthDate > new DateTime(1980, 1, 1)
               select m;
```

What if we want people who were born after 1980 and have the word "Manager" in their title?

```c#
var youngManagers = from m in members  
                    where m.BirthDate > new DateTime(1980, 1, 1) 
                        && m.JobTitle.Contains("Manager")
                    select m;
```

As you can see, the structure of a basic LINQ query is pretty simple. What if we make it more complex?

### Ordering

You can order the results using the `orderby` keyword.

```c#
var orderedYoungManagers = from m in members  
                           where m.BirthDate < new DateTime(2010, 1, 1) 
                              && m.JobTitle.Contains("Manager")
                           orderby m.BirthDate
                           select m;
```

You can also specify a descending order using the `descending` keyword:

```c#
var descendingYoungManagers = from m in members  
                              where m.BirthDate < new DateTime(2010, 1, 1) 
                                && m.JobTitle.Contains("Manager")
                              orderby m.BirthDate descending
                              select m;
```

You can also order by multiple fields, and `descending` can be applied to any of them:

```c#
var complexOrderedManagers = from m in members  
                             where m.BirthDate < new DateTime(2010, 1, 1) 
                                && m.JobTitle.Contains("Manager")
                             orderby m.BirthDate descending, m.LastName
                             select m;
```

### Projections

We can also just get the properties of the object that we want, rather than the entire object. One way of doing this is by using an [anonymous type](https://msdn.microsoft.com/en-us/library/bb397696.aspx).

```c#
var namesOnlyYounguns = from m in members  
                        where m.BirthDate > new DateTime(1980, 1, 1)
                        select new { m.FirstName, m.LastName };
```

The nice thing about anonymous types is that we can still iterate over them.

```c#
foreach(var name in namesOnlyYounguns)  
{
  Console.WriteLine("Name: " + name.FirstName + " " + name.LastName);
}
```

Notice, however, that anonymous types cannot be passed outside of a method.

## LINQ-to-Entities

The first examples we have seen use [LINQ-to-Objects](https://msdn.microsoft.com/en-us/library/bb397919.aspx), which is LINQ executed against in-memory collections. For the rest of this tutorial, we'll be using LINQ-to-Entities, which is LINQ executed against Entity Framework contexts and data in a database.

### Deferred Execution

LINQ-to-Entities works a bit differently from LINQ-to-Objects. First, let's demo a simple query in L2E:

```c#
using (NorthwindEntities context = new NorthwindEntities())  
{
  var customers = from c in context.Customers
                  where c.ContactName.Contains("Mar")
                  orderby c.City, c.Country
                  select c;
}
```

After this query has executed, what is `customers`?

In LINQ-to-Objects, it would have been a collection of objects. However, in LINQ-to-Entities, `customers` actually represents a query, rather than data.

This is due to an idea called **[deferred execution](https://msdn.microsoft.com/en-us/library/vstudio/bb738633%28v=vs.100%29.aspx)**. Deferred execution basically means thtam Entity Framework will not execute the query until the data is actually needed.

One of the cool things about this idea is that we can actually modify the query as an object:

```c#
var customers = from c in context.Customers  
                where c.ContactName.Contains("Mar")
                orderby c.City, c.Country
                select c;
                    
customers = customers.Where(x => x.Country == "USA");   
```

What we've done is simply added another `WHERE` constraint to the query, but because the query has not been executed yet no data has been retrieved, so the performance cost of making this change is minimal.

You can get the actual data by enumerating over the collection, using methods such as `ToList()` or a `foreach` loop:

```c#
var customersList = customers.ToList();  

foreach (var customer in customers)  
{
  Console.WriteLine("Customer: " + customer.ContactName);
}
```

There are also several other "conversion" methods such as `ToArray()`.

### Aggregates

Let's say we have this query:

```c#
using (NorthwindEntities context = new NorthwindEntities())  
{
	var products = from p in context.Products
                   where p.UnitPrice < price
                   select p;
}
```

How can we know if we got any products back? We can use a method called `Any()`:

```c#
var hasProducts = products.Any(); 
```

`Any()` returns a boolean that represents whether the collection has any elements. It's much quicker than doing `Count() == 0` because `Count()` has to iterate over the entire collection, while `Any()` just checks for the first object in the collection.

There's also several other aggregates we can use:

```c#
// Sum the UnitPrice
var totalPrice = products.Sum(x => x.UnitPrice);  
Console.WriteLine("Total Price: $" + totalPrice.ToString());

//Total number of products
var totalProducts = products.Count();
Console.WriteLine("# of Products: " + totalProducts.ToString());

// Total number of products where the unit price is greater than some comparison price
var totalProductsWhere = products.Count(x => x.UnitPrice < price);
Console.WriteLine("# of Products (Price < $" + price.ToString() + "): " + totalProductsWhere.ToString());

// The maximum unit price in the set
var maxPrice = products.Max(x => x.UnitPrice);  
Console.WriteLine("Maximum Price: $" + maxPrice.ToString());  
```

### Query Syntax vs Method Syntax

Notice that the Aggregate examples use methods `Any()`, `Count()`, and `Max()` rather then using the query structure (`from x in y where z select x`) we saw in the previous examples. In LINQ, there are two different syntaxes you can use to query for data: [query syntax and method syntax](https://msdn.microsoft.com/en-us/library/bb397947.aspx).

It is possible to do most things in either syntax, but certain things are much easier in one syntax or the other. For example, SQL-type operations such as joins or group by are much easier to write (and read) in query syntax than in method syntax. Be aware that **Query Syntax LINQ queries will be compiled down into method syntax.**

### Joins

Let's get some sample data for our Joins examples:

```c#
using (NorthwindEntities context = new NorthwindEntities())  
{
	string[] categoryNames = {  
  	"Beverages",   
    "Condiments",   
    "Vegetables",   
    "Dairy Products",   
    "Seafood"
	};
}
```

Just like in SQL queries, we can do several types of joins:

```c#
var products = from c in categoryNames  
               join p in context.Products on c equals p.Category.CategoryName
               select new { Category = c, p.ProductName }; // Cross Join
```

A [cross join](https://technet.microsoft.com/en-us/library/ms190690%28v=sql.105%29.aspx) is the result of combining every item from Set A (in our case, categoryNames) with every item from Set B (Products); the result is called a Cartesian product.

However, cross joins are not often useful in real-world scenarios. The more useful kind of join is called a group join:

```c#
var categories = from c in categoryNames  
                 join p in context.Products on c equals p.Category.CategoryName into ps
                 select new { Category = c, Products = ps }; // Group Join
```

Notice the `into` keyword. That keyword takes the joined data and inserts it into a a new collection, in our case called ps.

By the way, that same group join looks like this in method syntax:

```c#
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
```

We may also want a join called a left-outer join. A left outer join takes all elements from Set A and returns them, also returning elements from Set B if they match an element from Set A. Such a join looks like this:

```c#
var leftOuterJoin = from c in context.Categories  
                    join p in context.Products on c equals p.Category into ps
                    from p in ps.DefaultIfEmpty()
                    select new { Category = c, ProductName = p == null ? "(No products)" : p.ProductName };
```

### Grouping

Let's say that now, I want each categories, and I also want the products in each category. I'd accomplish this by performing a grouping, which looks like this:

```c#
var groupedProducts = from p in context.Products  
                      group p by p.Category.CategoryName into g
                      select new { Category = g.Key, Products = g };
```

Notice that we're using the `into` keyword again. The result of this query is a list of Categories, each of which have a collection of Products associated to them. We could iterate over this (and print each category/product combo) like this:

```c#
foreach (var category in groupedProducts)  
{
  foreach (var product in category.Products)
	{
    Console.WriteLine(category.Category + ": " + product.ProductName);
  }
}
```

### Skip and Take

That last query returns a lot of products. What if I only want the first 50?

```c#
var first50 = groupedProducts.Take(50);   
```

I can also get items 51-100:

```c#
var next50 = groupedProducts.Skip(50).Take(50);   
```
Notice the chaining aspect of this syntax here: method syntax often ends up being more concise than query syntax.

### Working with Collections

There are several methods we can use to manipulate or query collections:

```c#
var categories = from c in context.Categories  
                 where c.CategoryID > 3
                 select c;

var firstCategory = categories.First();
 
var firstCategoryMatched = categories.First(x => x.CategoryName == "Produce");

var firstCategoryDefault = categories.FirstOrDefault(x => x.CategoryName == "Nuts"); //returns null

var singleCategoryMatched = categories.Single(x => x.CategoryName == "Produce");
```

*   `First()` returns the first item in the collection (that matches the optional predicate) and throws an exception if it doesn't find one.
*   `FirstOrDefault()` returns the first item that matches the predicate and returns `null` if no item is found.
*   `Single()` returns the item in the collection that matches the predicate if there is one and only one item that matches; otherwise it throws an exception.

### Set Operations

We need some sample data for this next operation. Let's grab the first letters of all the Products, and the first letters of all the Customers.

```c#
var productsFirstLetters = (from p in context.Products  
                            select p.ProductName).ToList().Select(x => x[0]);

var customersFirstLetters = (from c in context.Customers  
                             select c.CompanyName).ToList().Select(x => x[0]);
```

There are three major set operations we can perform with these two sets of data: `UNION`, `INTERSECT`, and `EXCEPT`.

```c#
var unionLetters = productsFirstLetters.Union(customersFirstLetters).OrderBy(x => x);

var intersectLetters = productsFirstLetters.Intersect(customersFirstLetters).OrderBy(x => x);

var exceptLetters = productsFirstLetters.Except(customersFirstLetters).OrderBy(x => x);    
```

* [UNION](https://msdn.microsoft.com/en-us/library/bb386993%28v=vs.110%29.aspx) is used to return the items that exist in **either** set.
* [INTERSECT](https://msdn.microsoft.com/en-us/library/bb399392%28v=vs.110%29.aspx) is used to return the items that exist in **both** sets.
*   [EXCEPT](https://msdn.microsoft.com/en-us/library/vstudio/bb300779%28v=vs.100%29.aspx) is used to return the items in Set A that are **not also in** Set B.

## Grab the Sample Project

All of these are just the tip of the iceberg as far as learning LINQ. If you want to see these examples in an executable environment, download the [sample project from GitHub](https://github.com/exceptionnotfound/LINQExamples).

That sample project includes a fully-functional command-line application which you can use to call many different LINQ examples. It also includes a copy of the Northwind database, so that you can see how LINQ-to-Entities works against real data.

Happy Coding!

## About

[Original article](http://www.exceptionnotfound.net/linq-for-beginners) wrote by @exceptionnotfound.

Markdown port by @aloidg.
