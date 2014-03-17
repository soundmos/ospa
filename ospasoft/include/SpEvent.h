// OSPASOFT
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

#pragma once

#include <functional>

/// Empty event arguments, as a convenience for events that do not require additional information.
class SpEventArgs
{
};

/// An event source to which a single handler can be attached.
template<typename EventArgsType>
class SpEvent
{
public:
   /// Signature for the event handler method.
   typedef std::function<void(EventArgsType&)> Func;

   /// Constructor, using a no-op handler by default.
   SpEvent()
      : _Handler(SpEvent::DefaultHandler)
   {
   }

   /// Sets the event handler, replacing the existing handler.
   void SetHandler(Func handler)
   {
      _Handler = handler;
   }

   /// Removes the event handler, resetting it to the no-op handler.
   void RemoveHandler()
   {
      _Handler = SpEvent::DefaultHandler;
   }

   /// Fires the event handler by copying the event arguments structure.
   /// \param e Event arguments object.
   void Fire(EventArgsType e)
   {
      _Handler(e);
   }

   /// Fires the event handler.
   /// \param e Event arguments object reference.
   void Fire(EventArgsType& e)
   {
      _Handler(e);
   }

private:
   static void DefaultHandler(EventArgsType&)
   {
   }

   Func _Handler;
};
