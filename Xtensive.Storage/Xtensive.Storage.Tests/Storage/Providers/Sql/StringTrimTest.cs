// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2009.06.15

using System;
using System.Linq;
using NUnit.Framework;
using Xtensive.Core.Disposing;
using Xtensive.Storage.Configuration;
using Xtensive.Storage.Tests.Storage.DbTypeSupportModel;

namespace Xtensive.Storage.Tests.Storage.Providers.Sql
{
  [TestFixture]
  public class StringTrimTest : AutoBuildTest
  {
    private DisposableSet disposableSet;

    protected override void CheckRequirements()
    {
      EnsureIs(StorageProtocols.Sql);
    }

    public override void TestFixtureSetUp()
    {
      base.TestFixtureSetUp();
      disposableSet = new DisposableSet();
      disposableSet.Add(Domain.OpenSession());
      disposableSet.Add(Transaction.Open());
      var testValues = new[]
        {
          " :-P  ",
          ";-)",
          " :-)",
          ":-( ",
          "0_o",
          "o_0",
          "0_0",
          "o_o",
          "1111oneoneoneo",
          ")))))",
          "?????? ",
          " PROFIT",
        };
      foreach (var value in testValues)
        new X {FString = value};
    }

    public override void TestFixtureTearDown()
    {
      disposableSet.DisposeSafely();
      base.TestFixtureTearDown();
    }

    protected override DomainConfiguration BuildConfiguration()
    {
      var configuration = base.BuildConfiguration();
      configuration.Types.Register(typeof (X).Assembly, typeof (X).Namespace);
      return configuration;
    }

    [Test]
    public void BasicTest()
    {
      var results = Query<X>.All
        .Select(x => new
          {
            String = x.FString,
            StringTrim = x.FString.Trim(),
            StringTrimLeading = x.FString.TrimStart(),
            StringTrimTrailing = x.FString.TrimEnd(),
            StringTrimNull = x.FString.Trim(null),
            StringTrimLeadingNull = x.FString.TrimStart(null),
            StringTrimTrailingNull = x.FString.TrimEnd(null),
            StringTrimSpace = x.FString.Trim(' '),
            StringTrimLeadingSpace = x.FString.TrimStart(' '),
            StringTrimTrailingSpace = x.FString.TrimEnd(' '),
          });
      foreach (var x in results) {
        Assert.AreEqual(x.String.Trim(), x.StringTrim);
        Assert.AreEqual(x.String.TrimStart(), x.StringTrimLeading);
        Assert.AreEqual(x.String.TrimEnd(), x.StringTrimTrailing);
        Assert.AreEqual(x.String.Trim(null), x.StringTrimNull);
        Assert.AreEqual(x.String.TrimStart(null), x.StringTrimLeadingNull);
        Assert.AreEqual(x.String.TrimEnd(null), x.StringTrimTrailingNull);
        Assert.AreEqual(x.String.Trim(' '), x.StringTrimSpace);
        Assert.AreEqual(x.String.TrimStart(' '), x.StringTrimLeadingSpace);
        Assert.AreEqual(x.String.TrimEnd(' '), x.StringTrimTrailingSpace);
      }
    }

    [Test]
    public void ExtendedTest()
    {
      EnsureIs(~StorageProtocols.SqlServer);
      var results = Query<X>.All
        .Select(x => new
          {
            String = x.FString,
            StringTrimLeadingZeroAndOne = x.FString.TrimStart('0', '1'),
            StringTrimTrailingClosingBracket = x.FString.TrimEnd(')'),
            StringTrimSmallOLetter = x.FString.Trim('o'),
          });
      foreach (var x in results) {
        Assert.AreEqual(x.String.TrimStart('0', '1'), x.StringTrimLeadingZeroAndOne);
        Assert.AreEqual(x.String.TrimEnd(')'), x.StringTrimTrailingClosingBracket);
        Assert.AreEqual(x.String.Trim('o'), x.StringTrimSmallOLetter);
      }
    }
  }
}