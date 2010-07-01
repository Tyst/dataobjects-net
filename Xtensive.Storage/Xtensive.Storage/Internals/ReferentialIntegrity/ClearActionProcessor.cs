// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.07.01

using System;
using Xtensive.Storage.Internals;
using Xtensive.Storage.Model;

namespace Xtensive.Storage.ReferentialIntegrity
{
  internal class ClearActionProcessor : ActionProcessor
  {
    public override void Process(RemovalContext context, AssociationInfo association, Entity removingObject, Entity target, Entity referencingObject, Entity referencedObject)
    {
      switch (association.Multiplicity) {
        case Multiplicity.ZeroToOne:
        case Multiplicity.OneToOne:
          ReferentialActions.ClearReference(association, referencingObject, null, null);
          break;
        case Multiplicity.ManyToOne:
          ReferentialActions.RemoveReference(association.Reversed, referencedObject, referencingObject, null);
          break;
        case Multiplicity.ZeroToMany:
        case Multiplicity.OneToMany:
        case Multiplicity.ManyToMany:
          ReferentialActions.RemoveReference(association, referencingObject, referencedObject, null);
          break;
      }
    }
  }
}