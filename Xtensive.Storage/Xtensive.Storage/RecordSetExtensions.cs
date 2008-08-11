// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.06.09

using System;
using System.Collections.Generic;
using Xtensive.Core.Tuples;
using Xtensive.Core.Tuples.Transform;
using Xtensive.Storage.Internals;
using Xtensive.Storage.Model;
using Xtensive.Storage.Providers;
using Xtensive.Storage.Rse;
using ColumnGroup=Xtensive.Storage.Rse.ColumnGroup;

namespace Xtensive.Storage
{
  public static class RecordSetExtensions
  {
    public static IEnumerable<T> AsEntities<T>(this RecordSet source) 
      where T : class, IEntity
    {
      foreach (var entity in AsEntities(source, typeof (T)))
        yield return entity as T;
    }

    public static IEnumerable<Entity> AsEntities(this RecordSet source, Type type)
    {
      RecordSetHeaderParsingContext context = new RecordSetHeaderParsingContext(Session.Current, source.Header);
      RecordSetMapping mapping = GetRecordSetMapping(context);

      foreach (Tuple tuple in source) {
        Entity entity = null;
        foreach (ColumnGroupMapping columnGroupMapping in mapping.ColumnGroupMappings) {
          TypeMapping typeMapping = GetTypeMapping(context, columnGroupMapping, tuple);
          Tuple result = typeMapping.Transform.Apply(TupleTransformType.TransformedTuple, tuple);
          Key key = context.Domain.KeyManager.Get(typeMapping.Type, result);
          context.Session.DataCache.Update(key, result);
          if (entity == null && type.IsAssignableFrom(key.Type.UnderlyingType)) {
            entity = key.Resolve();
            yield return entity;
          }
        }
      }
    }

    private static RecordSetMapping GetRecordSetMapping(RecordSetHeaderParsingContext context)
    {
      List<ColumnGroupMapping> mappings = new List<ColumnGroupMapping>();
      foreach (ColumnGroup group in context.Header.ColumnGroups) {
        ColumnGroupMapping mapping = GetColumnGroupMapping(context, group);
        if (mapping != null)
          mappings.Add(mapping);
      }
      return new RecordSetMapping(context.Header, mappings);
    }

    private static ColumnGroupMapping GetColumnGroupMapping(RecordSetHeaderParsingContext context, ColumnGroup group)
    {
      int typeIdIndex = -1;
      Dictionary<ColumnInfo, Column> columnMapping = new Dictionary<ColumnInfo, Column>(group.Columns.Count);

      foreach (int columnIndex in group.Columns) {
        Column column = context.Header.Columns[columnIndex];
        ColumnInfo columnInfo = column.ColumnInfoRef.Resolve(context.Domain.Model);
        columnMapping[columnInfo] = column;
        if (columnInfo.Name==NameBuilder.TypeIdFieldName)
          typeIdIndex = column.Index;
      }

      if (typeIdIndex == -1)
        return null;

      return new ColumnGroupMapping(typeIdIndex, columnMapping);
    }

    private static TypeMapping GetTypeMapping(RecordSetHeaderParsingContext context, ColumnGroupMapping columnGroupMapping, Tuple tuple)
    {
      int typeId = tuple.GetValue<int>(columnGroupMapping.TypeIdIndex);
      TypeInfo type = context.Domain.Model.Types[typeId];
      TypeMapping typeMapping = columnGroupMapping.TypeMappings.GetValue(type);
      if (typeMapping==null) {
        typeMapping = BuildTypeMapping(columnGroupMapping, type);
        columnGroupMapping.TypeMappings.SetValue(type, typeMapping);
      }
      return typeMapping;
    }

    private static TypeMapping BuildTypeMapping(ColumnGroupMapping columnGroupMapping, TypeInfo type)
    {
      List<int> map = new List<int>(type.Columns.Count);
      foreach (ColumnInfo columnInfo in type.Columns) {
        Column column;
        if (columnGroupMapping.ColumnInfoMapping.TryGetValue(columnInfo, out column))
          map.Add(column.Index);
        else
          map.Add(MapTransform.NoMapping);
      }
      return new TypeMapping(type, new MapTransform(true, type.TupleDescriptor, map.ToArray()));
    }
  }
}