// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.12.02

using System;
using Xtensive.Storage.Model;

namespace Xtensive.Storage.Linq.Expressions
{
  [Serializable]
  public sealed class IndexAccessExpression : ExtendedExpression
  {
    public IndexInfo Index { get; private set; }

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format("Index[{0}]", Index.Name);
    }


    // Constructors

    public IndexAccessExpression(Type type, IndexInfo index)
      : base(ExtendedExpressionType.IndexAccess, type)
    {
      Index = index;
    }
  }
}