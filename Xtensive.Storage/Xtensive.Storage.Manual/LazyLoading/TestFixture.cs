// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexis Kochetov
// Created:    2009.11.02

using System;
using NUnit.Framework;
using System.Diagnostics;
using Xtensive.Storage.Configuration;

namespace Xtensive.Storage.Manual.LazyLoading
{
  [HierarchyRoot]
  public class Person : Entity
  {
    [Key, Field]
    public int Id { get; private set; }

    [Field(Length = 200)]
    public string Name { get; set; }

    [Field]
    public DateTime BirthDay { get; set; }

    [Field(LazyLoad = true, Length = 65536)]
    public byte[] Photo { get; set; }

    [Field(LazyLoad = true, Length = 8192)]
    public byte[] Avatar { get; set; }

    [Field(LazyLoad = true)]
    public Address Address { get; set; }

    [Field]
    public Person Manager { get; private set; }

    [Field]
    [Association(PairTo = "Manager")]
    public EntitySet<Person> Employees { get; private set; }
  }

  public class Address : Structure
  {
    [Field(Length = 60)]
    public string Street { get; set; }

    [Field(Length = 15)]
    public string City { get; set; }

    [Field(Length = 15)]
    public string Region { get; set; }

    [Field(Length = 10)]
    public string PostalCode { get; set; }

    [Field(Length = 15)]
    public string Country { get; set; }
  }

  [TestFixture]
  public class TestFixture
  {
    [Test]
    public void MainTest()
    {
      var config = new DomainConfiguration("memory://localhost/DO40-Tests");
      config.UpgradeMode = DomainUpgradeMode.Recreate;
      config.Types.Register(typeof(Person));
      Domain.Build(config);
    }
  }
}