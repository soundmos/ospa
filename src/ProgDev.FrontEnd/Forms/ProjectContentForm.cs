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

using ProgDev.FrontEnd.Common.FlexForms;
using ProgDev.FrontEnd.Common.Toolkit;
using WeifenLuo.WinFormsUI.Docking;

namespace ProgDev.FrontEnd.Forms
{
   public partial class ProjectContentForm : DockContent
   {
      public ProjectContentForm(ProjectContentFormViewModel viewModel)
      {
         InitializeComponent();

         _ListView.AutoSizeColumnWidths(_NameCol);
         _ListView.BindItems(viewModel.List);
         _ListView.BindSelectedItems(viewModel.SelectedList);
         _ListView.BindItemActivate(viewModel.ItemActivate);
         _ContextMnu.BindEnabled(viewModel.ContextMenuEnabled);
         _OpenMnu.BindClick(viewModel.ItemActivate);
         _RenameMnu.BindEnabled(viewModel.RenameEnabled);
         _RenameMnu.BindClick(viewModel.RenameClick);
         _MoveMnu.BindClick(viewModel.MoveClick);
         _DeleteMnu.BindClick(viewModel.DeleteClick);
         _DuplicateMnu.BindClick(viewModel.DuplicateClick);
         viewModel.Start(this);
      }
   }
}
