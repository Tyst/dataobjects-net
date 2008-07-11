// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.07.08

namespace Xtensive.Storage.Providers.Index.Compilers
{
  public sealed class CompilerResolver : Rse.Compilation.CompilerResolver
  {
    public ExecutionContext ExecutionContext { get; private set; }


    // Constructor

    public CompilerResolver(ExecutionContext context)
    {
      ExecutionContext = context;
    }
  }
}