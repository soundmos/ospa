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

using ProgDev.BusinessLogic;
using ProgDev.Domain;
using ProgDev.FrontEnd.Common;
using ProgDev.FrontEnd.Common.FlexForms;
using ProgDev.Resources;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ProgDev.FrontEnd.Forms
{
   public sealed class AppFormViewModel : FormViewModel
   {
      private readonly string _InitialFilePath;

      // Window frame
      public Field<Point> Location;
      public Field<Size> Size;
      public Field<Size> MinimumSize;
      public Field<FormWindowState> WindowState;
      public Field<DockState> ProjectContentFormDockState;
      public Field<double> DockLeftPortion;
      public Field<double> DockRightPortion;
      public Field<double> DockTopPortion;
      public Field<double> DockBottomPortion;
      // Titlebar
      public ComputedField<string> Title;
      public Signal PromptClose;
      public Field<bool> CanClose;
      // File menu
      public Signal NewClick;
      public Signal OpenClick;
      public ComputedField<bool> SaveEnabled;
      public Signal SaveClick;
      public Signal SaveAsClick;
      // Edit menu
      public ComputedField<bool> UndoEnabled;
      public Signal UndoClick;
      public ComputedField<bool> RedoEnabled;
      public Signal RedoClick;
      // Program menu
      public Signal NewFileClick;
      public Signal BuildClick;
      public Signal DeployClick;
      public Signal DebugClick;
      // Help menu
      public Signal AboutClick;
      // Search box
      public Field<string> SearchText;
      public Field<Color> SearchForeColor;
      public Field<bool> SearchFocused;
      public Signal SearchEnterKeyPress;
      // Internals, set from the project in code rather than bound to the window
      public Field<string> Project_Name;
      public Field<bool> Project_IsDirty;
      public Field<bool> Project_CanUndo;
      public Field<bool> Project_CanRedo;

      public AppFormViewModel(string initialFilePath)
      {
         _InitialFilePath = initialFilePath;
      }

      protected override void Initialize()
      {
         try
         {
            LoadWindowPosition();
         }
         catch (Exception)
         {
            // Ignore.  This protects us against bogus setting values.
         }

         OnProjectChanged();
         Project.Events.Changed += OnProjectChanged;

         if (_InitialFilePath != null)
            Project.Load(_InitialFilePath);
      }

      private void LoadWindowPosition()
      {
         var s = Settings.Default;
         if (s.AppForm_SavedPositionExists)
         {
            var screenArea = Screen.PrimaryScreen.WorkingArea;

            int width = Math.Max(MinimumSize.Value.Width, s.AppForm_Size.Width);
            int height = Math.Max(MinimumSize.Value.Height, s.AppForm_Size.Height);
            int left = Math.Min(screenArea.Width - width, Math.Max(0, s.AppForm_Location.X));
            int top = Math.Min(screenArea.Height - height, Math.Max(0, s.AppForm_Location.Y));
            Location.Value = new Point(left, top);
            Size.Value = new Size(width, height);
            WindowState.Value = s.AppForm_WindowState;
            ProjectContentFormDockState.Value = s.ProjectContentForm_DockState;
            DockLeftPortion.Value = s.DockPanel_DockLeftPortion;
            DockRightPortion.Value = s.DockPanel_DockRightPortion;
            DockTopPortion.Value = s.DockPanel_DockTopPortion;
            DockBottomPortion.Value = s.DockPanel_DockBottomPortion;
         }
      }

      private void SaveWindowPosition()
      {
         var s = Settings.Default;
         s.AppForm_SavedPositionExists = true;
         s.AppForm_WindowState = WindowState.Value;
         if (WindowState.Value == FormWindowState.Normal)
         {
            s.AppForm_Location = Location.Value;
            s.AppForm_Size = Size.Value;
         }
         s.ProjectContentForm_DockState = ProjectContentFormDockState.Value;
         s.DockPanel_DockLeftPortion = DockLeftPortion.Value;
         s.DockPanel_DockRightPortion = DockRightPortion.Value;
         s.DockPanel_DockTopPortion = DockTopPortion.Value;
         s.DockPanel_DockBottomPortion = DockBottomPortion.Value;
         s.Save();
      }

      private void OnProjectChanged()
      {
         Project_Name.Value = Project.Contents.ProjectName;
         Project_IsDirty.Value = Project.Contents.IsDirty;
         Project_CanUndo.Value = Project.Commands.CanUndo;
         Project_CanRedo.Value = Project.Commands.CanRedo;
      }

      private bool DoSave()
      {
         if (Project.Contents.FilePath == null)
         {
            return DoSaveAs();
         }
         else
         {
            Project.Save(Project.Contents.FilePath);
            return true;
         }
      }

      private bool DoSaveAs()
      {
         var dlg = new SaveFileDialog
         {
            AddExtension = true,
            AutoUpgradeEnabled = true,
            CheckPathExists = true,
            DefaultExt = "." + FileExtensions.ProjectExtension,
            Filter = string.Format(Strings.OpenProjectFilter, FileExtensions.ProjectExtension),
            OverwritePrompt = true,
            Title = Strings.SaveProjectTitle
         };
         var result = ShowChildDialog(dlg);
         if (result == DialogResult.OK)
         {
            Project.Save(dlg.FileName);
            return true;
         }
         else
         {
            return false;
         }
      }

      [Compute("Title"), Depends("Project_Name", "Project_IsDirty")]
      private string ComputeTitle()
      {
         string filename = Project_Name.Value;
         if (Project_IsDirty.Value)
            filename += "*";
         return string.Format(Strings.AppFormTitle, filename);
      }

      [Compute("SaveEnabled"), Depends("Project_IsDirty")]
      private bool ComputeSaveEnabled()
      {
         return Project_IsDirty.Value;
      }

      [OnSignal("NewClick")]
      private void OnNewClick()
      {
         if (!PromptAndSave())
            return;

         Project.New();
      }

      private bool PromptAndSave() // true = ok, false = cancel
      {
         if (Project.Contents.IsDirty)
         {
            string projectName =
               Project.Contents.FilePath == null
               ? Strings.Untitled
               : Path.GetFileNameWithoutExtension(Project.Contents.FilePath);
            string message = string.Format(Strings.SaveChangedPrompt, projectName);

            var dlg = new MessageForm(message, Strings.OpenProjectTitle,
               new[] { Strings.ButtonSave, Strings.ButtonDontSave, Strings.ButtonCancel }, Images.Page32);
            if (ShowChildDialog(dlg) == DialogResult.Cancel)
               return false;
            else if (dlg.Result == Strings.ButtonSave)
               return DoSave();
         }

         return true;
      }

      [OnSignal("OpenClick")]
      private void OnOpenClick()
      {
         if (!PromptAndSave())
            return;

         var openDlg = new OpenFileDialog
         {
            AutoUpgradeEnabled = true,
            CheckFileExists = true,
            CheckPathExists = true,
            DefaultExt = "." + FileExtensions.ProjectExtension, 
            Filter = string.Format(Strings.OpenProjectFilter, FileExtensions.ProjectExtension),
            Multiselect = false, 
            Title = Strings.OpenProjectTitle,
            ValidateNames = true
         };
         if (ShowChildDialog(openDlg) == DialogResult.OK)
         {
            try
            {
               Project.Load(openDlg.FileName);
            }
            catch (Exception ex)
            {
               ShowChildDialog(new MessageForm(
                  Strings.ErrorOpenProject, Strings.OpenProjectTitle, detail: ex.ToDetailString()));
               return;
            }
         }
      }

      [OnSignal("SaveClick")]
      private void OnSaveClick()
      {
         DoSave();
      }

      [OnSignal("SaveAsClick")]
      private void OnSaveAsClick()
      {
         DoSaveAs();
      }

      [Compute("UndoEnabled"), Depends("Project_CanUndo")]
      private bool ComputeUndoEnabled()
      {
         return Project_CanUndo.Value;
      }

      [OnSignal("UndoClick")]
      private void OnUndoClick()
      {
         Project.Commands.Undo();
      }

      [Compute("RedoEnabled"), Depends("Project_CanRedo")]
      private bool ComputeRedoEnabled()
      {
         return Project_CanRedo.Value;
      }

      [OnSignal("RedoClick")]
      private void OnRedoClick()
      {
         Project.Commands.Redo();
      }

      [OnSignal("NewFileClick")]
      private void OnNewFileClick()
      {
         var form = FormsFactory.NewNewFileForm(Strings.Untitled);
         ShowChildDialog(form);
      }

      [OnSignal("PromptClose")]
      private void OnPromptClose()
      {
         CanClose.Value = PromptAndSave();
         SaveWindowPosition();
      }

      [OnSignal("AboutClick")]
      private void OnAboutClick()
      {
         ShowChildDialog(FormsFactory.NewAboutForm());
      }

      [OnChange("SearchFocused")]
      private void OnSearchFocusedChanged()
      {
         if (SearchFocused.Value && SearchText.Value == "Search")
         {
            SearchText.Value = "";
            SearchForeColor.Value = SystemColors.WindowText;
         }
         else if (!SearchFocused.Value && SearchText.Value == "")
         {
            SearchText.Value = "Search";
            SearchForeColor.Value = SystemColors.ControlDark;
         }
      }

      [OnSignal("SearchEnterKeyPress")]
      private void OnSearchEnterKeyPress()
      {

      }
   }
}
