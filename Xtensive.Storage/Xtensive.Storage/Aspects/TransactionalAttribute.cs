// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Kochetov
// Created:    2008.08.21

using System;
using System.Diagnostics;
using System.Reflection;
using PostSharp.Extensibility;
using PostSharp.Laos;
using Xtensive.Core.Aspects.Helpers;
using Xtensive.Core.Disposing;

namespace Xtensive.Storage.Aspects
{
  /// <summary>
  /// Wraps method body into transaction, if necessary.
  /// </summary>
  [MulticastAttributeUsage(MulticastTargets.Method)]
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)] 
  [Serializable]
  public sealed class TransactionalAttribute : ReprocessMethodBoundaryAspect,
    ILaosWeavableAspect
  {   
    int ILaosWeavableAspect.AspectPriority {
      get {
        return (int) StorageAspectPriority.Transactional;
      }
    }

    /// <inheritdoc/>
    public override bool CompileTimeValidate(MethodBase method)
    {
      if (!AspectHelper.ValidateContextBoundMethod<Session>(this, method))
        return false;

      if (!AspectHelper.ValidateNotInfrastructure(this, method))
        return false;

      return true;
    }

    /// <inheritdoc/>
    [DebuggerStepThrough]
    public override object OnEntry(object instance)
    {
      var transactionScope = Transaction.Open();
      return transactionScope;
    }

    /// <inheritdoc/>
    [DebuggerStepThrough]
    public override void OnExit(object instance, object onEntryResult)
    {
      var transactionScope = (TransactionScope)onEntryResult;
      transactionScope.DisposeSafely();
    }

    /// <inheritdoc/>
    [DebuggerStepThrough]
    public override void OnSuccess(object instance, object onEntryResult)
    {
      var transactionScope = (TransactionScope)onEntryResult;
      transactionScope.Complete();
    }
  }
}