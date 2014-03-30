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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ProgDev.IDE.Common.FlexForms
{
   public abstract class ViewModel<InterfaceType>
   {
      public Action<Action<InterfaceType>> Invoke = f => { };

      public ViewModel()
      {
         InitializeFieldsOfType<Field>();
         InitializeFieldsOfType<Signal>();
         AttachHandlers();
      }

      public void Start(InterfaceType form)
      {
         Invoke = action => action(form);

         Initialize();
      }

      protected abstract void Initialize();

      private void AttachHandlers()
      {
         var methods = GetType()
            .GetMethods()
            .Where(x => x.GetCustomAttributes(true).OfType<OnSignalAttribute>().Any());

         foreach (var methodInfo in methods)
         {
            var attributes = methodInfo.GetCustomAttributes(true);

            var handler = attributes.OfType<OnSignalAttribute>().SingleOrDefault();
            if (handler != null)
            {
               var callback = CreateCallback<Signal>(methodInfo, handler.FieldName, typeof(void));
               callback.Item1.Handle += (sender, e) => callback.Item2();
            }

            var onChange = attributes.OfType<OnChangeAttribute>().SingleOrDefault();
            if (onChange != null)
            {
               var callback = CreateCallback<Field>(methodInfo, handler.FieldName, typeof(void));
               callback.Item1.Changed += (sender, e) => callback.Item2();
            }
         }
      }

      private Tuple<T, Func<object>> CreateCallback<T>(MethodInfo methodInfo, string fieldName, Type returnType)
      {
         if (methodInfo.ReturnType != returnType)
            throw new FlexException("Method must return: " + returnType.Name);
         if (methodInfo.GetParameters().Any())
            throw new FlexException("Method must not have any parameters.");

         var fieldInfo = GetType().GetFields().SingleOrDefault(x => x.Name == fieldName);
         if (fieldInfo == null)
            throw new FlexException("Cannot find a field named: " + fieldName);

         return Tuple.Create<T, Func<object>>(
            (T)fieldInfo.GetValue(this),
            () => methodInfo.Invoke(this, new object[0])
         );
      }

      private void InitializeFieldsOfType<T>()
      {
         var type = typeof(T);

         foreach (var fieldInfo in GetType().GetFields())
         {
            if (fieldInfo.FieldType == type || fieldInfo.FieldType.IsSubclassOf(type))
               fieldInfo.SetValue(this, Activator.CreateInstance(fieldInfo.FieldType));
         }
      }
   }
}