// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.12.17

using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Xtensive.Storage.Configuration;
using Xtensive.Storage.Tests.ObjectModel.NorthwindDO;

namespace Xtensive.Storage.Tests.Linq
{
  [TestFixture]
  public class JoinTest : NorthwindDOModelTest
  {
    [Test]
    public void SingleTest()
    {
      using (Domain.OpenSession()) {
        using (var t = Transaction.Open()) {
          var products = Query<Product>.All;
          var productsCount = products.Count();
          var suppliers = Query<Supplier>.All;
          var result = from p in products
                       join s in suppliers on p.Supplier.Id equals s.Id
                       select new {p.ProductName, s.ContactName, s.Phone};
          var list = result.ToList();
          Assert.AreEqual(productsCount, list.Count);
          t.Complete();
        }
      }
    }

    [Test]
    public void SeveralTest()
    {
      using (Domain.OpenSession()) {
        using (var t = Transaction.Open()) {
          var products = Query<Product>.All;
          var productsCount = products.Count();
          var suppliers = Query<Supplier>.All;
          var categories = Query<Category>.All;
          var result = from p in products
                       join s in suppliers on p.Supplier.Id equals s.Id
                       join c in categories on p.Category.Id equals c.Id
                       select new { p, s, c.CategoryName };
          var list = result.ToList();
          Assert.AreEqual(productsCount, list.Count);
          t.Complete();
        }
      }
    }

    [Test]
    public void OneToManyTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var products = Query<Product>.All;
        var productsCount = products.Count();
        var suppliers = Query<Supplier>.All;
        var result =  from s in suppliers
                      join p in products on s.Id equals p.Supplier.Id
                      select new { p.ProductName, s.ContactName };
        var list = result.ToList();
        Assert.AreEqual(productsCount, list.Count);
        t.Complete();
      }
    }

    [Test]
    public void GroupJoinTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var categories = Query<Category>.All;
        var products = Query<Product>.All;
        var categoryCount = categories.Count();
        var result = from c in categories
                     join p in products on c equals p.Category into pGroup
                     select pGroup;
        var list = result.ToList();
        Assert.AreEqual(categoryCount, list.Count);
        t.Complete();
      }
    }

    [Test]
    public void GroupJoinNestedTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var categories = Query<Category>.All;
        var products = Query<Product>.All;
        var categoryCount = categories.Count();
        var result = from c in categories
                     orderby c.CategoryName
                     join p in products on c equals p.Category into pGroup
                     select new
                     {
                       Category = c.CategoryName,
                       Products = from ip in pGroup
                                  orderby ip.ProductName
                                  select ip
                     };
        var list = result.ToList();
        Assert.AreEqual(categoryCount, list.Count);
        t.Complete();
      }
    }

    [Test]
    public void GroupJoinSelectManyTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var categories = Query<Category>.All;
        var products = Query<Product>.All;
        var productsCount = products.Count();
        var result =  from c in categories
                      orderby c.CategoryName
                      join p in products on c equals p.Category into pGroup
                      from gp in pGroup
                      orderby gp.ProductName
                      select new {Category = c.CategoryName, gp.ProductName};
        var list = result.ToList();
        Assert.AreEqual(productsCount, list.Count);
        t.Complete();
      }
    }

    [Test]
    public void LeftOuterTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var categories = Query<Category>.All;
        var products = Query<Product>.All;
        var categoryCount = categories.Count();
        var result = from c in categories
                     join p in products on c equals p.Category into pGroup
                     select pGroup.DefaultIfEmpty(new Product(){ProductName = "Nothing!", Category = c});
        var list = result.ToList();
        Assert.AreEqual(categoryCount, list.Count);
        t.Complete();
      }
    }

    [Test]
    public void LeftOuterNestedTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var categories = Query<Category>.All;
        var products = Query<Product>.All;
        var productsCount = products.Count();
        var result = from c in categories
                     join p in products on c equals p.Category into pGroup
                     from pg in pGroup.DefaultIfEmpty()
                     select new { Name = pg == null ? "Nothing!" : pg.ProductName, CategoryID = c.Id };
        var list = result.ToList();
        Assert.AreEqual(productsCount, list.Count);
        t.Complete();
      }
    }

    [Test]
    public void SelectManyTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var products = Query<Product>.All;
        var productsCount = products.Count();
        var suppliers = Query<Supplier>.All;
        var categories = Query<Category>.All;
        var result =  from p in products
                      from s in suppliers
                      from c in categories
                      where p.Supplier==s && p.Category==c
                      select new {p, s, c.CategoryName};
        var list = result.ToList();
        Assert.AreEqual(productsCount, list.Count);
        t.Complete();
      }
    }

    [Test]
    public void SelectManyJoinedTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var result = from c in Query<Customer>.All
                     from o in Query<Order>.All.Where(o => o.Customer == c)
                     select new { c.ContactName, o.OrderDate };
        var list = result.ToList();
        t.Complete();
      }
    }

    [Test]
    public void SelectManyJoinedDefaultIfEmptyTest()
    {
      using (Domain.OpenSession())
      using (var t = Transaction.Open()) {
        var customersCount = Query<Customer>.All.Count();
        var result = from c in Query<Customer>.All
                     from o in Query<Order>.All.Where(o => o.Customer == c).DefaultIfEmpty()
                     select new { c.ContactName, o.OrderDate };
        var list = result.ToList();
        Assert.AreEqual(customersCount, list.Count);
        t.Complete();
      }
    }
  }
}