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

module ProgDev.Services.Bundler
open ProgDev.Domain
open ProgDev.Resources
open System
open System.IO
open System.Text
open MimeKit

let private ToTextPart (file : BundleFile) =
   let textPart = new TextPart()
   textPart.FileName <- file.Folder + "/" + file.Filename
   textPart.Text <- file.Content
   textPart

let private ToBundleFile (file : TextPart) =
   if not (file.FileName.Contains("/")) then failwith Strings.ErrorFileAtRoot
   else
      let filenameParts = file.FileName.Split '/'
      { Folder = filenameParts.[0]; Filename = filenameParts.[1]; Content = file.Text } : BundleFile

let Save (bundle : Bundle) (filePath : string) =
   let textParts = bundle.Files |> List.map ToTextPart |> List.toArray
   let multipart = new Multipart()
   multipart.Boundary <- "=-9t4JuySFJzSo9CdzTIgoQA=="
      // Use a fixed boundary to ensure stability from one save to the next.
   for textPart in textParts do
      multipart.Add textPart
   use stream = File.Create(filePath)
   multipart.WriteTo stream

let Load (filePath : string) =
   use stream = File.OpenRead(filePath)
   let parser = new MimeParser(stream)
   let multipart = downcast parser.ParseEntity() : Multipart
   { Files = multipart |> Seq.map (fun x -> ToBundleFile (downcast x : TextPart)) |> Seq.toList }

let Bundle (sourcePath : string) (targetFilePath : string) =
   let GetAllFolderPaths (path : string) : string seq =
      Directory.GetDirectories path
      |> Array.toSeq
   let GetAllFilePaths (folders : string seq) : string seq =
      folders
      |> Seq.map Directory.GetFiles
      |> Seq.map Array.toSeq
      |> Seq.fold Seq.append Seq.empty<string>
   let CreateBundleFile (filePath : string) : BundleFile =
      {
         Folder = Path.GetFileName(Path.GetDirectoryName(filePath))
         Filename = Path.GetFileName(filePath)
         Content = File.ReadAllText(filePath)
      }
   let CreateBundle (sourcePath : string) : Bundle =
      {
         Files = 
            GetAllFolderPaths sourcePath
            |> GetAllFilePaths
            |> Seq.map CreateBundleFile 
            |> Seq.toList
      }
   Save (CreateBundle sourcePath) targetFilePath

let Unbundle (sourceFilePath : string) (targetPath : string) =
   let CreateFile (absoluteRoot : string) (file : BundleFile) =
      let absolutePath = Path.Combine(absoluteRoot, file.Folder, file.Filename)
      let folderPath = Path.Combine(absoluteRoot, file.Folder)
      if not (Directory.Exists folderPath) then ignore (Directory.CreateDirectory folderPath)
      File.WriteAllText(absolutePath, file.Content)
   let CreateFiles (absoluteRoot : string) (files : BundleFile list) =
      ignore (List.map (CreateFile absoluteRoot) files)
   CreateFiles targetPath (Load sourceFilePath).Files
