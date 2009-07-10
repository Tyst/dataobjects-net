// Copyright (C) 2009 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexander Nikolaev
// Created:    2009.05.27

using NUnit.Framework;
using Xtensive.Sql;
using Xtensive.Storage.Indexing.Model;
using Xtensive.Storage.Providers;

namespace Xtensive.Storage.Tests.Upgrade
{
  [Explicit("Requires PostgreSQL")]
  public sealed class PgSqlActionTranslatorTest : SqlActionTranslatorTest
  {
    protected override string GetConnectionUrl()
    {
      return "postgresql://do4test:do4testpwd@localhost:8332/do40test?Encoding=ASCII";
    }

    protected override bool IsIncludedColumnsSupported { get { return false; } }

//    protected override TypeInfo ConvertType(SqlValueType valueType)
//    {
//      var driver = SqlDriver.Create(Url);
//      using (var connection = driver.CreateConnection(Url)) {
//        connection.Open();
//        var dataTypes = connection.Driver.ServerInfo.DataTypes;
//        var nativeType = connection.Driver.Translator.Translate(valueType);
//
//        var dataType = dataTypes[nativeType] ?? dataTypes[valueType.DataType];
//
//        int? length = 0;
//        var streamType = dataType as StreamDataTypeInfo;
//        if (streamType!=null
//          && (streamType.SqlType==SqlDataType.VarBinaryMax
//            || streamType.SqlType==SqlDataType.VarCharMax
//              || streamType.SqlType==SqlDataType.AnsiVarCharMax))
//          length = null;
//        else
//          length = valueType.Size;
//
//        return new TypeInfo(dataType.Type, false, length);
//      }
//    }
    
    protected override ProviderInfo CreateProviderInfo()
    {
      var providerInfo = new ProviderInfo();
      providerInfo.SupportsRealTimeSpan = true;
      providerInfo.SupportSequences = true;
      providerInfo.SupportKeyColumnSortOrder = false;
      providerInfo.SupportsIncludedColumns = false;
      providerInfo.SupportsForeignKeyConstraints = true;
      return providerInfo;
    }
  }
}
