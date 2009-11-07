// Copyright (C) 2007 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.

using System;

namespace Xtensive.Sql.Compiler
{
  public class SqlCompilerOutputScope : IDisposable
  {
    private readonly SqlCompilerContext context;

    internal ContextType Type { get; private set; }

    internal ContainerNode ParentContainer { get; private set; }

    /// <inheritdoc/>
    public void Dispose()
    {
      context.CloseScope(this);
    }

    internal SqlCompilerOutputScope(SqlCompilerContext context, ContainerNode parentContainer, ContextType type)
    {
      this.context = context;
      Type = type;
      ParentContainer = parentContainer;
    }
  }
}