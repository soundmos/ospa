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
using ProgDev.Resources;
using System.Diagnostics;
using System.Windows.Forms;

namespace ProgDev.FrontEnd.Forms
{
   public sealed class AboutFormViewModel : FormViewModel
   {
      public ComputedField<string> VersionText;
      public Signal GitHubClick;
      public Signal FarmFreshClick;
      public Signal FatCowClick;
      public Signal CreativeCommonsClick;
      public Signal DockPanelClick;
      public Signal AntlrClick;
      public Signal MimeKitClick;
      public Signal TextEditorClick;
      public Signal CloseClick;

      [OnSignal("CloseClick")]
      private void OnCloseClick()
      {
         Close();
      }

      [OnSignal("GitHubClick")]
      private void OnGitHubClick()
      {
         Process.Start("https://github.com/electroly/ospa");
      }

      [OnSignal("FarmFreshClick")]
      private void OnFarmFreshClick()
      {
         Process.Start("http://www.fatcow.com/free-icons");
      }

      [OnSignal("FatCowClick")]
      private void OnFatCowClick()
      {
         Process.Start("http://www.fatcow.com/");
      }

      [OnSignal("CreativeCommonsClick")]
      private void OnCreativeCommonsClick()
      {
         Process.Start("http://creativecommons.org/licenses/by/3.0/us/");
      }

      [OnSignal("DockPanelClick")]
      private void OnDockPanelClick()
      {
         Process.Start("http://dockpanelsuite.com/");
      }

      [OnSignal("AntlrClick")]
      private void OnAntlrClick()
      {
         Process.Start("http://www.antlr.org/");
      }

      [OnSignal("MimeKitClick")]
      private void OnMimeKitClick()
      {
         Process.Start("http://jstedfast.github.io/MimeKit/");
      }

      [OnSignal("TextEditorClick")]
      private void OnTextEditorClick()
      {
         Process.Start("https://github.com/icsharpcode/SharpDevelop");
      }

      [Compute("VersionText")]
      private string ComputeVersionText()
      {
         return string.Format(Strings.AboutVersionFormat, Application.ProductVersion);
      }
   }
}
