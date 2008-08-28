// Copyright (C) 2008 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alexey Gamzov
// Created:    2008.02.12

using System;
using System.IO;
using System.Runtime.Serialization;
using Xtensive.Core.Internals.DocTemplates;

namespace Xtensive.Core.Serialization
{
  /// <summary>
  /// Base class for any <see cref="IValueSerializer{T}"/>.
  /// </summary>
  /// <typeparam name="T">Type of value to serialize or deserialize.</typeparam>
  [Serializable]
  public abstract class ValueSerializerBase<T> : IValueSerializer<T>,
    IDeserializationCallback
  {
    /// <summary>
    /// A thread-static buffer, that can be used during the serialization.
    /// </summary>
    [ThreadStatic]
    protected static byte[] ThreadBuffer;

    /// <inheritdoc/>
    public IValueSerializerProvider Provider { get; protected set; }

    /// <summary>
    /// Gets the length of the output produced by this serializer.
    /// <see langword="-1" /> means it may vary.
    /// </summary>
    public int OutputLength { get; protected set; }

    /// <inheritdoc/>
    public abstract T Deserialize(Stream stream);

    /// <inheritdoc/>
    public abstract void Serialize(Stream stream, T value);

    #region IValueSerializer methods

    void IValueSerializer.Serialize(Stream stream, object value) 
    {
      Serialize(stream, (T) value);
    }

    object IValueSerializer.Deserialize(Stream stream) 
    {
      return Deserialize(stream);
    }

    #endregion

    /// Ensures the <see cref="ThreadBuffer"/> is initialized.
    /// </summary>
    /// <param name="size">The required buffer size.</param>
    protected static void EnsureThreadBufferIsInitialized(int size)
    {
      if (ThreadBuffer==null)
        ThreadBuffer = new byte[size];
    }


    // Constructors

    /// <summary>
    /// <see cref="ClassDocTemplate.Ctor" copy="true" />
    /// </summary>
    /// <param name="provider">Serializer provider this serializer is bound to.</param>
    protected ValueSerializerBase(IValueSerializerProvider provider) 
    {
      ArgumentValidator.EnsureArgumentNotNull(provider, "provider");
      Provider = provider;
    }

    // IDeserializationCallback methods

    /// <see cref="SerializableDocTemplate.OnDeserialization" copy="true" />
    public virtual void OnDeserialization(object sender)
    {
      if (Provider==null || Provider.GetType()==typeof (ValueSerializerProvider))
        Provider = ValueSerializerProvider.Default;
    }
  }
}