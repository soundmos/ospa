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

module ProgDev.BusinessLogic.InputValidator
open System.Text.RegularExpressions

let private IdentifierRegex = Regex @"^[A-Za-z_][A-Za-z_0-9]*$"
let private DottedIdentifierListRegex = Regex @"^[A-Za-z_][A-Za-z_0-9]*(\.[A-Za-z_][A-Za-z_0-9]*)*$"
let private FolderRegex = Regex @"^[A-Za-z_0-9,\.\-()][A-Za-z_0-9,\.\-() ]*$"

let (| Identifier | _ |) (x : string) : string option =
   if x = null then None
   else
      let m = IdentifierRegex.Match x
      if m.Success then Some x else None

let (| DottedIdentifierList | _ |) (x : string) : string option =
   if x = null then None
   else
      let m = DottedIdentifierListRegex.Match x
      if m.Success then Some x else None

let (| Folder | _ |) (x : string) : string option =
   if x = null then None
   else
      let m = FolderRegex.Match x
      if m.Success then Some x else None

let IsIdentifier (x : string) : bool =
   match x with
   | Identifier y -> true
   | _ -> false

let IsNamespace (x : string) : bool =
   match x with
   | DottedIdentifierList y -> true
   | _ -> false

let IsFolder (x: string) : bool =
   match x with
   | Folder y -> true
   | _ -> false
