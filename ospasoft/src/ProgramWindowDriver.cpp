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

#include "UserInterface.h"
#include "AboutWindowDriver.h"
#include "ProgramWindowDriver.h"

ProgramWindowDriver::ProgramWindowDriver()
{
   _AboutWindowDriver = std::make_shared<AboutWindowDriver>();

   Register(_Window.NewProgramMnu);
   Register(_Window.AboutMnu);
}

void ProgramWindowDriver::OnCallback(void* widget)
{
   WD_CALLBACK(NewProgramMnu);
   WD_CALLBACK(AboutMnu);
}

void ProgramWindowDriver::OnNewProgramMnu()
{
}

void ProgramWindowDriver::OnAboutMnu()
{
   _AboutWindowDriver->Show();
}
