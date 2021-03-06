﻿// OSPA ProgDev
// Copyright (C) 2014 Brian Luft
// 
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public 
// License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later
// version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied 
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more 
// details.
// 
// You should have received a copy of the GNU General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;

namespace ProgDev.FrontEnd.Common.FlexForms
{
   public sealed class ComputedField<T> : Field<T>, IComputedField
   {
      Func<object> IComputedField.Evaluator { get; set; }

      IEnumerable<Field> IComputedField.Dependencies
      {
         set
         {
            foreach (var dependency in value)
               dependency.Changed += (sender, e) => Recompute();
         }
      }

      public void Poll()
      {
         Recompute();
      }

      private void Recompute()
      {
         _Value = (T)((IComputedField)this).Evaluator();
         Notify();
      }

      public override T Value
      {
         get
         {
            return _Value;
         }
         set
         {
            // Ignore it.  This field is computed dynamically.  But notify normally.
            Notify();
         }
      }
   }
}
