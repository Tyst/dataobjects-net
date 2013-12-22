// Copyright (C) 2013 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2013.08.20

using System;
using System.Linq;
using Mono.Cecil;

namespace Xtensive.Orm.Weaver.Stages
{
  internal sealed class ImportReferencesStage : ProcessorStage
  {
    public override ActionResult Execute(ProcessorContext context)
    {
      var registry = context.References;
      var mscorlibAssembly = context.TargetModule.TypeSystem.Corlib;

      var ormAssembly = FindReference(context, WellKnown.OrmAssemblyFullName);
      if (ormAssembly==null) {
        context.SkipProcessing = true;
        return ActionResult.Success;
      }
      registry.OrmAssembly = ormAssembly;

      var stringType = context.TargetModule.TypeSystem.String;
      var voidType = context.TargetModule.TypeSystem.Void;

      // mscorlib
      var typeType = registry.Type = ImportType(context, mscorlibAssembly, "System.Type");
      var exceptionType = registry.Exception = ImportType(context, mscorlibAssembly, "System.Exception");
      var runtimeTypeHandleType = registry.RuntimeTypeHandle = ImportType(context, mscorlibAssembly, "System.RuntimeTypeHandle", true);
      registry.SerializationInfo = ImportType(context, mscorlibAssembly, "System.Runtime.Serialization.SerializationInfo");
      registry.StreamingContext = ImportType(context, mscorlibAssembly, "System.Runtime.Serialization.StreamingContext", true);
      registry.TypeGetTypeFromHandle = ImportMethod(context, typeType, "GetTypeFromHandle", false, typeType, runtimeTypeHandleType);
      registry.CompilerGeneratedAttributeConstructor = ImportConstructor(context, mscorlibAssembly, WellKnown.CompilerGeneratedAttribute);

      // Xtensive.Core
      registry.Tuple = ImportType(context, ormAssembly, "Xtensive.Tuples.Tuple");

      // Xtensive.Orm
      registry.Session = ImportType(context, ormAssembly, "Xtensive.Orm.Session");
      registry.Entity = ImportType(context, ormAssembly, WellKnown.EntityType);
      registry.EntityInterface = ImportType(context, ormAssembly, WellKnown.EntityInterfaceType);
      registry.EntityState = ImportType(context, ormAssembly, "Xtensive.Orm.EntityState");
      registry.FieldInfo = ImportType(context, ormAssembly, "Xtensive.Orm.Model.FieldInfo");
      registry.EntitySetItem = ImportType(context, ormAssembly, WellKnown.EntitySetItemType);
      var persistentType = registry.Persistent = ImportType(context, ormAssembly, "Xtensive.Orm.Persistent");

      registry.PersistenceImplementation = ImportType(context, ormAssembly, "Xtensive.Orm.Weaving.PersistenceImplementation");
      registry.PersistenceImplementationHandleKeySet = ImportMethod(context, registry.PersistenceImplementation, "HandleKeySet", false, voidType, stringType, stringType);

      registry.PersistentInitialize = ImportMethod(context, persistentType, "Initialize", true, voidType, typeType);
      registry.PersistentInitializationError = ImportMethod(context, persistentType, "InitializationError", true, voidType, typeType, exceptionType);

      var getterType = new GenericParameter(0, GenericParameterType.Method, context.TargetModule);
      var persistentGetter = new MethodReference("GetFieldValue", getterType, persistentType) {HasThis = true};
      persistentGetter.Parameters.Add(new ParameterDefinition(stringType));
      persistentGetter.GenericParameters.Add(getterType);
      registry.PersistentGetterDefinition = context.TargetModule.Import(persistentGetter);

      var setterType = new GenericParameter(0, GenericParameterType.Method, context.TargetModule);
      var persistentSetter = new MethodReference("SetFieldValue", voidType, persistentType) {HasThis = true};
      persistentSetter.Parameters.Add(new ParameterDefinition(stringType));
      persistentSetter.Parameters.Add(new ParameterDefinition(setterType));
      persistentSetter.GenericParameters.Add(setterType);
      registry.PersistentSetterDefinition = context.TargetModule.Import(persistentSetter);

      registry.ProcessedByWeaverAttributeConstructor = ImportConstructor(context, ormAssembly, WellKnown.ProcessedByWeaverAttribute);
      registry.EntityTypeAttributeConstructor = ImportConstructor(context, ormAssembly, WellKnown.EntityTypeAttribute);
      registry.EntitySetTypeAttributeConstructor = ImportConstructor(context, ormAssembly, WellKnown.EntitySetTypeAttribute);
      registry.EntityInterfaceAttributeConstructor = ImportConstructor(context, ormAssembly, WellKnown.EntityInterfaceAttribute);
      registry.StructureTypeAttributeConstructor = ImportConstructor(context, ormAssembly, WellKnown.StructureTypeAttribute);
      registry.OverrideFieldNameAttributeConstructor = ImportConstructor(context, ormAssembly, WellKnown.OverrideFieldNameAttribute, stringType);

      return ActionResult.Success;
    }

    private TypeReference ImportType(ProcessorContext context, IMetadataScope assembly, string fullName, bool isValueType = false)
    {
      var splitName = SplitTypeName(fullName);
      var targetModule = context.TargetModule;
      var reference = new TypeReference(splitName.Item1, splitName.Item2, targetModule, assembly, isValueType);
      return targetModule.Import(reference);
    }

    private MethodReference ImportConstructor(ProcessorContext context, IMetadataScope assembly, string fullName, params TypeReference[] parameterTypes)
    {
      var splitName = SplitTypeName(fullName);
      var targetModule = context.TargetModule;
      var typeReference = new TypeReference(splitName.Item1, splitName.Item2, targetModule, assembly);
      var constructorReference = new MethodReference(WellKnown.Constructor, targetModule.TypeSystem.Void, typeReference) {HasThis = true};
      foreach (var type in parameterTypes)
        constructorReference.Parameters.Add(new ParameterDefinition(type));
      return targetModule.Import(constructorReference);
    }

    private MethodReference ImportMethod(ProcessorContext context, TypeReference declaringType, string methodName,
      bool hasThis, TypeReference returnType, params TypeReference[] parameterTypes)
    {
      var targetModule = context.TargetModule;
      var methodReference = new MethodReference(methodName,returnType, declaringType) {HasThis = hasThis};
      foreach (var parameterType in parameterTypes)
        methodReference.Parameters.Add(new ParameterDefinition(parameterType));
      return targetModule.Import(methodReference);
    }

    private AssemblyNameReference FindReference(ProcessorContext context, string assemblyName)
    {
      var comparer = WeavingHelper.AssemblyNameComparer;
      var reference = context.TargetModule.AssemblyReferences
        .FirstOrDefault(r => comparer.Equals(r.FullName, assemblyName));
      return reference;
    }

    private static Tuple<string, string> SplitTypeName(string fullName)
    {
      var index = fullName.LastIndexOf(".", StringComparison.InvariantCulture);
      if (index < 0)
        return Tuple.Create(String.Empty, fullName);
      return Tuple.Create(
        fullName.Substring(0, index),
        fullName.Substring(index + 1));
    }
  }
}