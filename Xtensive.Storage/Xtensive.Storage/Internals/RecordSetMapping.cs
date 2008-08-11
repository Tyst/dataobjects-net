// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.07.31

using System.Collections.Generic;
using Xtensive.Core.Collections;
using Xtensive.Storage.Rse;

namespace Xtensive.Storage.Internals
{
  internal class RecordSetMapping
  {
    public RecordSetHeader Header { get; private set;}

    public IList<ColumnGroupMapping> ColumnGroupMappings { get; private set; }


    // Constructors

    public RecordSetMapping(RecordSetHeader header, IList<ColumnGroupMapping> hierarchyMappings)
    {
      Header = header;
      ColumnGroupMappings = new ReadOnlyList<ColumnGroupMapping>(hierarchyMappings);
    }
  }
}