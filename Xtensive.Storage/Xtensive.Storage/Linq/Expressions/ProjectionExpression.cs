// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.12.11

using System;
using System.Collections.Generic;
using Xtensive.Core.Parameters;
using Xtensive.Core.Tuples;

namespace Xtensive.Storage.Linq.Expressions
{
  internal class ProjectionExpression : ExtendedExpression
  {
    public ItemProjectorExpression ItemProjector { get; private set;}
    public ResultType ResultType { get; private set; }
    public Dictionary<Parameter<Tuple>, Tuple> TupleParameterBindings { get; private set; }

    public bool IsScalar
    {
      get {  return ResultType != ResultType.All; }
    }

    public override string ToString()
    {
      return string.Format("Projection:  {0}, IsScalar = {1}", ItemProjector, IsScalar);
    }

    // Constructors

    public ProjectionExpression(
      Type type,
      ItemProjectorExpression itemProjectorExpression,
      Dictionary<Parameter<Tuple>, Tuple> tupleParameterBindings)
      : this(type, itemProjectorExpression, ResultType.All, tupleParameterBindings)
    {}

    public ProjectionExpression(
      Type type, 
      ItemProjectorExpression itemProjectorExpression, 
      ResultType resultType, 
      Dictionary<Parameter<Tuple>, Tuple> tupleParameterBindings)
      : base(ExtendedExpressionType.Projection, type)
    {
      ItemProjector = itemProjectorExpression;
      ResultType = resultType;
      TupleParameterBindings = new Dictionary<Parameter<Tuple>, Tuple>(tupleParameterBindings); 
    }
  }
}