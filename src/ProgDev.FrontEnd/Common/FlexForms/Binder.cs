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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ProgDev.FrontEnd.Common.FlexForms
{
   public static class Binder
   {
      public static EventHandler Bind<T>(Field<T> field, Func<T> accessor, Action<T> mutator)
      {
         bool ignoreChangedEvent = false;

         // Initialize the property by reading from the form.  The view model's Initialize() method may override this 
         // later, or it may choose to accept the value set by the form.
         field.Value = accessor();

         field.Changed += (sender, e) =>
         {
            if (!ignoreChangedEvent)
               mutator(field.Value);
         };

         return (sender, e) =>
         {
            ignoreChangedEvent = true;
            try
            {
               field.Value = accessor();
            }
            finally
            {
               ignoreChangedEvent = false;
            }
         };
      }

      public static void BindText(this Control control, Field<string> field)
      {
         control.TextChanged += Bind(field, () => control.Text, x => control.Text = x);
      }

      public static void BindText(this ToolStripControlHost control, Field<string> field)
      {
         control.TextChanged += Bind(field, () => control.Text, x => control.Text = x);
      }

      public static void BindForeColor(this ToolStripControlHost control, Field<Color> field)
      {
         control.ForeColorChanged += Bind(field, () => control.ForeColor, x => control.ForeColor = x);
      }
      
      public static void BindVisible(this Control control, Field<bool> field)
      {
         control.VisibleChanged += Bind(field, () => control.Visible, x => control.Visible = x);
      }

      public static void BindEnabled(this Control control, Field<bool> field)
      {
         control.EnabledChanged += Bind(field, () => control.Enabled, x => control.Enabled = x);
      }

      public static void BindEnabled(this ToolStripItem control, Field<bool> field)
      {
         control.EnabledChanged += Bind(field, () => control.Enabled, x => control.Enabled = x);
      }

      public static void BindChecked(this CheckBox control, Field<bool> field)
      {
         control.CheckedChanged += Bind(field, () => control.Checked, x => control.Checked = x);
      }

      public static void BindChecked(this RadioButton control, Field<bool> field)
      {
         control.CheckedChanged += Bind(field, () => control.Checked, x => control.Checked = x);
      }

      public static void BindLocation(this Control control, Field<Point> field)
      {
         control.LocationChanged += Bind(field, () => control.Location, x => control.Location = x);
      }

      public static void BindSize(this Control control, Field<Size> field)
      {
         control.SizeChanged += Bind(field, () => control.Size, x => control.Size = x);
      }

      public static void BindError(this ErrorProvider control, Control target, Field<string> field)
      {
         // This is a read-only control, so there's no event.
         Bind(field, () => control.GetError(target), x => control.SetError(target, x));
      }

      public static void BindError(this Control target, ErrorProvider control, Field<string> field)
      {
         // This is a read-only control, so there's no event.
         Bind(field, () => control.GetError(target), x => control.SetError(target, x));
      }

      public static void BindMinimumSize(this Control control, Field<Size> field)
      {
         // There is no MinimumSizeChanged event.
         Bind(field, () => control.MinimumSize, x => control.MinimumSize = x);
      }

      public static void BindMaximumSize(this Control control, Field<Size> field)
      {
         // There is no MaximumSizeChanged event.
         Bind(field, () => control.MaximumSize, x => control.MaximumSize = x);
      }

      public static void BindClick(this Control control, Signal signal)
      {
         control.Click += signal;
      }

      public static void BindClick(this ToolStripButton control, Signal signal)
      {
         control.Click += signal;
      }

      public static void BindClick(this ToolStripMenuItem control, Signal signal)
      {
         control.Click += signal;
      }

      public static void BindClosing(this Form control, Signal signal)
      {
         control.FormClosing += (sender, e) => signal.Handler(sender, e);
      }

      public static void BindClosing(this Form control, Signal promptClose, Field<bool> canClose)
      {
         control.FormClosing += (sender, e) =>
         {
            promptClose.Handler(sender, e);
            if (!canClose.Value)
               e.Cancel = true;
         };
      }

      public static void BindWindowState(this Form control, Field<FormWindowState> field)
      {
         var eventHandler = Bind(field, () => control.WindowState, x => control.WindowState = x);

         var lastWindowState = control.WindowState;
         control.Resize += (sender, e) =>
         {
            if (control.WindowState != lastWindowState)
            {
               lastWindowState = control.WindowState;
               eventHandler(control, EventArgs.Empty);
            }
         };
      }

      public static void BindItems(this ComboBox control, ListField<string> field)
      {
         control.Items.AddRange(field.ToArray());

         field.Changed += (sender, e) =>
         {
            int selectedIndex = control.SelectedIndex;
            control.BeginUpdate();
            try
            {
               control.Items.Clear();
               control.Items.AddRange(field.ToArray());
               if (control.Items.Count > 0)
                  control.SelectedIndex = Math.Max(0, Math.Min(control.Items.Count - 1, selectedIndex));
            }
            finally
            {
               control.EndUpdate();
            }
         };
      }

      public static void BindSelectedIndex(this ComboBox control, Field<int> field)
      {
         control.SelectedIndexChanged += Bind(field, () => control.SelectedIndex, x => control.SelectedIndex = x);
      }

      public static void BindItems(this ListView control, ListField<ListViewRow> field)
      {
         PopulateListView(control, field);

         field.Changed += (sender, e) => PopulateListView(control, field);
      }

      public static void BindSelectedItems(this ListView control, ListField<ListViewRow> field)
      {
         bool ignore = false;

         control.SelectedIndexChanged += (sender, e) =>
         {
            ignore = true;
            try
            {
               field.Set(control.SelectedItems.Cast<ListViewItem>().Select(x => x.Tag).Cast<ListViewRow>());
            }
            finally
            {
               ignore = false;
            }
         };

         field.Changed += (sender, e) =>
         {
            // ignore is true when the ListField has changed as the result of the user selecting an item.  Therefore,
            // we don't want to redo that same change on the control.
            if (ignore)
               return;

            control.BeginUpdate();
            try
            {
               control.SelectedItems.Clear();
               foreach (var row in field)
               {
                  foreach (var item in control.Items.Cast<ListViewItem>())
                  {
                     if (item.Tag == row)
                     {
                        item.Selected = true;
                        break;
                     }
                  }
               }
            }
            finally
            {
               control.EndUpdate();
            }
         };
      }

      private static void PopulateListView(ListView control, ListField<ListViewRow> field)
      {
         var groups =
            field
            .Select(x => x.GroupName)
            .Where(x => x != null)
            .Distinct()
            .OrderBy(x => x)
            .ToDictionary(
               x => x,
               x => new ListViewGroup(x, x));
         var images =
            field
            .Select(x => x.Icon)
            .Where(x => x != null)
            .Distinct()
            .Zip(Enumerable.Range(0, int.MaxValue), (image, index) => new { image, index })
            .ToDictionary(x => x.image, x => x.index);

         if (images.Any())
         {
            var imageList = new ImageList();
            foreach (var pair in images)
               imageList.Images.Add(pair.Key);
            if (control.SmallImageList != null)
               control.SmallImageList.Dispose();
            control.SmallImageList = imageList;
         }

         var boldFont = new Font(control.Font, FontStyle.Bold);

         control.BeginUpdate();
         try
         {
            var selectedIndices = control.SelectedIndices.Cast<int>().ToList();

            control.Groups.Clear();
            control.Items.Clear();

            control.Groups.AddRange(groups.Values.ToArray());

            foreach (var row in field)
            {
               var lvi =
                  row.GroupName == null
                  ? new ListViewItem()
                  : new ListViewItem(groups[row.GroupName]);
               
               lvi.Tag = row;
               lvi.Font = boldFont;
               lvi.UseItemStyleForSubItems = false;

               bool first = true;
               foreach (string cell in row.Cells)
               {
                  if (first)
                  {
                     lvi.Text = cell;
                  }
                  else
                  {
                     lvi.SubItems.Add(cell);
                     lvi.SubItems.Cast<ListViewItem.ListViewSubItem>().Last().Font = control.Font;
                  }
                  first = false;
               }

               if (row.Icon != null)
                  lvi.ImageIndex = images[row.Icon];

               control.Items.Add(lvi);
            }

            control.SelectedIndices.Clear();
            foreach (int index in selectedIndices.Where(x => x < control.Items.Count))
               control.SelectedIndices.Add(index);
         }
         finally
         {
            control.EndUpdate();
         }
      }

      public static void BindDockState(this DockContent control, Field<DockState> field)
      {
         control.DockStateChanged += Bind(field, () => control.DockState, x => control.DockState = x);
      }

      public static void BindDockLeftPortion(this DockPanel control, Field<double> field)
      {
         control.DockLeftPortionChanged += 
            Bind(field, () => control.DockLeftPortion, x => control.DockLeftPortion = x);
      }

      public static void BindDockRightPortion(this DockPanel control, Field<double> field)
      {
         control.DockRightPortionChanged +=
            Bind(field, () => control.DockRightPortion, x => control.DockRightPortion = x);
      }

      public static void BindDockTopPortion(this DockPanel control, Field<double> field)
      {
         control.DockTopPortionChanged +=
            Bind(field, () => control.DockTopPortion, x => control.DockTopPortion = x);
      }

      public static void BindDockBottomPortion(this DockPanel control, Field<double> field)
      {
         control.DockBottomPortionChanged +=
            Bind(field, () => control.DockBottomPortion, x => control.DockBottomPortion = x);
      }

      public static void BindItemActivate(this ListView control, Signal signal)
      {
         control.ItemActivate += signal;
      }

      public static void BindFocused(this Control control, Field<bool> field)
      {
         bool isFocused = control.Focused;
         var handler = Bind(field, () => isFocused, x => 
         {
            if (x)
               control.Focus();
            else
               control.SelectNextControl(control, true, true, true, true);
         });
         control.GotFocus += (sender, e) => { isFocused = true; handler(sender, e); };
         control.LostFocus += (sender, e) => { isFocused = false; handler(sender, e); };
      }

      public static void BindFocused(this ToolStripControlHost control, Field<bool> field)
      {
         bool isFocused = control.Focused;
         var handler = Bind(field, () => isFocused, x => { if (x) control.Focus(); });
         control.GotFocus += (sender, e) => { isFocused = true; handler(sender, e); };
         control.LostFocus += (sender, e) => { isFocused = false; handler(sender, e); };
      }

      public static void BindKeyPress(this ToolStripControlHost control, Signal signal, params char[] keyChars)
      {
         control.KeyPress += (sender, e) =>
         {
            if (keyChars.Contains(e.KeyChar))
               signal.Handler(control, e);
         };
      }
   }
}
