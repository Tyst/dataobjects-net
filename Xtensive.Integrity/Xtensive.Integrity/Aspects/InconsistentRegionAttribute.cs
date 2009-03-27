// Copyright (C) 2007 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexander Ilyin
// Created:    2007.07.16

using System;
using System.Diagnostics;
using System.Reflection;
using PostSharp.Extensibility;
using PostSharp.Laos;
using Xtensive.Core.Aspects;
using Xtensive.Core.Aspects.Helpers;
using Xtensive.Core.Disposing;
using Xtensive.Core.Reflection;
using Xtensive.Integrity.Validation;
using Xtensive.Integrity.Validation.Interfaces;

namespace Xtensive.Integrity.Aspects
{
  /// <summary>
  /// Wraps a method of property body into so-called "inconsistent region"
  /// using <see cref="ValidationContextBase.OpenInconsistentRegion"/> method.
  /// </summary>
  // [MulticastAttributeUsage(MulticastTargets.Property | MulticastTargets.Method)]
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  [Serializable]
  public sealed class InconsistentRegionAttribute : OnMethodBoundaryAspect,
    ILaosWeavableAspect
  {
    int ILaosWeavableAspect.AspectPriority
    {
      get {
        return (int)IntegrityAspectPriority.InconsistentRegion;
      }
    }

    /// <inheritdoc/>
    public override bool CompileTimeValidate(MethodBase method)
    {
      if (!AspectHelper.ValidateBaseType(this, SeverityType.Error, method.DeclaringType, true, typeof(IValidationAware)))
        return false;

      var methodInfo = method as MethodInfo;      

      if (methodInfo.IsGetter()) {
        // This is getter; let's check if it is explicitely marked as [InconsistentRegion]
        var propertyInfo = methodInfo.GetProperty();
        if (propertyInfo!=null && propertyInfo.GetAttribute<AtomicAttribute>(
          AttributeSearchOptions.Default)!=null)
          // Property itself is marked as [InconsistentRegion]
          return false;
        
        // Property getter is marked as [InconsistentRegion]
        ErrorLog.Write(SeverityType.Warning, AspectMessageType.AspectPossiblyMissapplied,
          AspectHelper.FormatType(GetType()),
          AspectHelper.FormatMember(methodInfo.DeclaringType, methodInfo));
      }

      return true;
    }

    /// <inheritdoc/>
    [DebuggerStepThrough]
    public override void OnEntry(MethodExecutionEventArgs eventArgs)
    {
      var validatable = (IValidationAware)eventArgs.Instance;
      eventArgs.MethodExecutionTag = validatable.Context.OpenInconsistentRegion();
    }

    /// <inheritdoc/>
    [DebuggerStepThrough]
    public override void OnExit(MethodExecutionEventArgs eventArgs)
    {
      ((IDisposable)eventArgs.MethodExecutionTag).DisposeSafely();      
    }
  }
}