// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.11.02

using System;
using Xtensive.Core;
using Xtensive.Core.Aspects;
using Xtensive.Core.Tuples;
using Xtensive.Storage.Model;
using Activator=Xtensive.Storage.Internals.Activator;

namespace Xtensive.Storage
{
  public sealed class PersistentAccessor : SessionBound
  {
    #region Entity/Structure-related methods

    [Infrastructure]
    public Persistent CreateInstance(Type type)
    {
      ArgumentValidator.EnsureArgumentNotNull(type, "type");

      TypeInfo typeInfo = Session.Domain.Model.Types[type];
      if (typeInfo.IsEntity) {
        var key = Key.Create(type);
        var state = Session.CreateNewEntityState(key);
        var result = Activator.CreateEntity(type, state, false);
        return result;
      }
      throw new NotImplementedException();
    }

    [Infrastructure]
    public Persistent CreateInstance(Type type, Tuple tuple)
    {
      ArgumentValidator.EnsureArgumentNotNull(type, "type");
      ArgumentValidator.EnsureArgumentNotNull(tuple, "tuple");

      TypeInfo typeInfo = Session.Domain.Model.Types[type];
      if (typeInfo.IsEntity) {
        var key = Key.Create(type, tuple, true);
        var state = Session.CreateNewEntityState(key);
        var result = Activator.CreateEntity(type, state, false);
        return result;
      }
      throw new NotImplementedException();
    }

    [Infrastructure]
    public T GetField<T>(Persistent target, FieldInfo field)
    {
      ValidateArguments(target, field);
      return target.GetField<T>(field, false);
    }

    [Infrastructure]
    public void SetField<T>(Persistent target, FieldInfo field, T value)
    {
      ValidateArguments(target, field);
      target.SetField(field, value, false);
    }

    [Infrastructure]
    public Key GetKey(Persistent target, FieldInfo field)
    {
      ValidateArguments(target, field);
      return target.GetKey(field);
    }

    [Infrastructure]
    public void Remove(Entity target)
    {
      ArgumentValidator.EnsureArgumentNotNull(target, "target");
      target.Remove(false);
    }

    #endregion

    #region Private members

    private static void ValidateArguments(Persistent target, FieldInfo field)
    {
      ArgumentValidator.EnsureArgumentNotNull(target, "target");
      ArgumentValidator.EnsureArgumentNotNull(field, "field");
      if (!target.Type.Fields.Contains(field))
        throw new InvalidOperationException(string.Format("Type '{0}' doesn't contain '{1}' field.", target.Type.Name, field.Name));
    }

    #endregion


    // Constructor

    internal PersistentAccessor(Session session)
      : base(session)
    {
    }
  }
}