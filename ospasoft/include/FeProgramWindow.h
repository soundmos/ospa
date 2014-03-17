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

#include "SpEventArgs.h"
#include "FeWindow.h"
#include "FeTaskPropertiesWindow.h"
#include "FeAboutWindow.h"

/// Driver for the main Program window.
class FeProgramWindow : public FeWindow<class FeProgramWindowView>
{
public:
   /// Constructor.
   FeProgramWindow();

private:
   void OnAboutClicked(SpEventArgs& e);
   void OnNewTaskClicked(SpEventArgs& e);

   std::shared_ptr<FeTaskPropertiesWindow> _TaskPropertiesWindow;
   std::shared_ptr<FeAboutWindow> _AboutWindow;
};
