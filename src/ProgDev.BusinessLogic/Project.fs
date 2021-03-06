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

module ProgDev.BusinessLogic.Project
open ProgDev.Domain
open ProgDev.Services
open ProgDev.Services.Utility
open ProgDev.Resources
open System

(*********************************************************************************************************************)
let mutable private _FilePath : string option = None
let private _ChangedEvent = new DelegateEvent<System.Action>()
let private Notify () = _ChangedEvent.Trigger([| |])

let private ToNamePart (filename : string) : string = filename.Split('.').[0]

(*********************************************************************************************************************)
module private BundleManager =
   let mutable private _Bundle : Bundle = { Files = [] }
   let mutable private _Dirty : bool = false
   let mutable private _UndoStack = [] : Bundle list
   let mutable private _RedoStack = [] : Bundle list
  
   // Read-only access
   let Bundle () = _Bundle
   let IsDirty () = _Dirty

   let CanUndo () = not _UndoStack.IsEmpty
   let CanRedo () = not _RedoStack.IsEmpty

   let GetContent (folder : string) (name : string) =
      (_Bundle.Files |> List.find (fun x -> x.Folder =? folder && (ToNamePart x.Filename) =? name)).Content

   let Do (action : Bundle -> Bundle) =
      _UndoStack <- List.Cons(_Bundle, _UndoStack)
      if _UndoStack.Length > 100 then
         _UndoStack <- _UndoStack |> List.toSeq |> Seq.take 100 |> Seq.toList
      _RedoStack <- []
      _Bundle <- action _Bundle
      _Dirty <- true
      Notify()

   let Undo () =
      if _UndoStack.IsEmpty then ()
      else
         _RedoStack <- List.Cons(_Bundle, _RedoStack)
         _Bundle <- _UndoStack.Head
         _UndoStack <- _UndoStack.Tail
         _Dirty <- true
         Notify()
   
   let Redo () =
      if _RedoStack.IsEmpty then ()
      else
         _UndoStack <- List.Cons(_Bundle, _UndoStack)
         _Bundle <- _RedoStack.Head
         _RedoStack <- _RedoStack.Tail
         _Dirty <- true
         Notify()

   let New () =
      _FilePath <- None
      _Bundle <- { Files = [] }
      _UndoStack <- []
      _RedoStack <- []
      _Dirty <- false
      Notify()

   let Load (filePath : string) =
      _Bundle <- Bundler.Load filePath
      _FilePath <- Some filePath
      _UndoStack <- []
      _RedoStack <- []
      _Dirty <- false
      Notify()

   let Save (filePath : string) =
      Bundler.Save _Bundle filePath
      _FilePath <- Some filePath
      _Dirty <- false
      Notify()

let private MaybeGetBundleFile (folder : string) (name : string) (bundle : Bundle) =
   bundle.Files 
   |> List.toSeq 
   |> Seq.tryFind (fun x -> (ToNamePart x.Filename) =? name && x.Folder =? folder) 

let private GetBundleFile (folder : string) (name : string) (bundle : Bundle) =
   match (MaybeGetBundleFile folder name bundle) with
   | Some x -> x
   | None -> failwith Strings.ErrorFileNotFound

(*********************************************************************************************************************)
type File (folder, name, pouType, pouLanguage) =
   member this.Folder : string = folder
   member this.Name : string = name // No file extension, just the name portion
   member this.Type : PouType = pouType
   member this.Language : PouLanguage = pouLanguage
   member this.Content with get () = BundleManager.GetContent this.Folder this.Name
   member this.Exists with get () = (MaybeGetBundleFile this.Folder this.Name (BundleManager.Bundle())) <> None

let private ToNewFilename (file : File) (name : string) : string =
   let languageExt = FileExtensions.GetLanguageExtension file.Language
   let typeExt = FileExtensions.GetTypeExtension file.Type
   name + "." + languageExt + "." + typeExt

let private ToFilename (file : File) : string =
   ToNewFilename file file.Name

let private ToFile (x : BundleFile) : File =
   let dottedParts = x.Filename.Split('.')
   if dottedParts.Length <> 3 then failwith Strings.ErrorMalformedFilename
   let name = dottedParts.[0]
   let pouLanguage = FileExtensions.ParseLanguageExtension dottedParts.[1]
   let pouType = FileExtensions.ParseTypeExtension dottedParts.[2]
   new File(x.Folder, name, pouType, pouLanguage)

(*********************************************************************************************************************)
type ProjectEvents () =
   [<CLIEvent>]
   member this.Changed = _ChangedEvent.Publish

type ProjectContents () =
   member this.Folders
      with get () = 
         BundleManager.Bundle().Files
         |> List.toSeq 
         |> Seq.map (fun x -> x.Folder)
         |> Seq.distinct
         |> Seq.sort

   member this.Files
      with get () = 
         BundleManager.Bundle().Files 
         |> List.toSeq 
         |> Seq.map ToFile
         |> Seq.sortBy (fun x -> x.Folder)

   member this.FilePath
      with get () =
         match _FilePath with
         | Some x -> x
         | None -> null

   member this.ProjectName
      with get () =
         match _FilePath with
         | Some x -> System.IO.Path.GetFileNameWithoutExtension x
         | None -> Strings.Untitled

   member this.IsDirty
      with get () = BundleManager.IsDirty()

   member this.GetFile folder name = 
      BundleManager.Bundle().Files 
      |> List.find (fun x -> x.Folder =? folder && (ToNamePart x.Filename) =? name) 
      |> ToFile

let Events = new ProjectEvents()

let Contents = new ProjectContents()

(*********************************************************************************************************************)
let New () = BundleManager.New()
let Load (filePath : string) = BundleManager.Load filePath
let Save (filePath : string) = BundleManager.Save filePath

(*********************************************************************************************************************)
module private FileOperations =
   let private CheckFileDoesNotExist folder name bundle =
      if bundle.Files |> List.exists (fun x -> (ToNamePart x.Filename) =? name && x.Folder =? folder) 
         then failwith (String.Format(Strings.ErrorFileExists, name, folder))

   let private GetTemplate (pouType : PouType, pouLanguage : PouLanguage) =
      match (pouLanguage, pouType) with
      | PouLanguage.FunctionBlockDiagram, PouType.Class -> FileTemplates.FbdClass
      | PouLanguage.FunctionBlockDiagram, PouType.FunctionBlock -> FileTemplates.FbdBlock
      | PouLanguage.FunctionBlockDiagram, PouType.Function -> FileTemplates.FbdFunction
      | PouLanguage.FunctionBlockDiagram, PouType.Program -> FileTemplates.FbdProgram
      | PouLanguage.InstructionList, PouType.Class -> FileTemplates.IlClass
      | PouLanguage.InstructionList, PouType.FunctionBlock -> FileTemplates.IlBlock
      | PouLanguage.InstructionList, PouType.Function -> FileTemplates.IlFunction
      | PouLanguage.InstructionList, PouType.Program -> FileTemplates.IlProgram
      | PouLanguage.LadderDiagram, PouType.Class -> FileTemplates.LdClass
      | PouLanguage.LadderDiagram, PouType.FunctionBlock -> FileTemplates.LdBlock
      | PouLanguage.LadderDiagram, PouType.Function -> FileTemplates.LdFunction
      | PouLanguage.LadderDiagram, PouType.Program -> FileTemplates.LdProgram
      | PouLanguage.SequentialFunctionChart, PouType.FunctionBlock -> FileTemplates.SfcBlock
      | PouLanguage.SequentialFunctionChart, PouType.Program -> FileTemplates.SfcProgram
      | PouLanguage.StructuredText, PouType.Class -> FileTemplates.StClass
      | PouLanguage.StructuredText, PouType.FunctionBlock -> FileTemplates.StBlock
      | PouLanguage.StructuredText, PouType.Function -> FileTemplates.StFunction
      | PouLanguage.StructuredText, PouType.GlobalVars -> FileTemplates.StVariables
      | PouLanguage.StructuredText, PouType.Interface -> FileTemplates.StInterface
      | PouLanguage.StructuredText, PouType.Program -> FileTemplates.StProgram
      | PouLanguage.StructuredText, PouType.DataType -> FileTemplates.StDataType
      | _ -> failwith Strings.ErrorInvalidTypeLanguageCombo

   let private GetTemplateString (pouType : PouType, pouLanguage : PouLanguage) =
      GetTemplate(pouType, pouLanguage) |> System.Text.Encoding.UTF8.GetString

   let NewFile (folder : string) (name : string) (pouType : PouType) (pouLanguage : PouLanguage) (bundle : Bundle) =
      CheckFileDoesNotExist folder name bundle
      let parts = [name; FileExtensions.GetLanguageExtension pouLanguage; FileExtensions.GetTypeExtension pouType]
      let filename = String.Join(".", parts)
      let content = GetTemplateString(pouType, pouLanguage)
      let newFile : BundleFile = { Folder = folder; Filename = filename; Content = content; }
      let files = List.Cons(newFile, bundle.Files)
      { Files = files }

   let RenameFile (file : File) (newName : string) (bundle : Bundle) =
      CheckFileDoesNotExist file.Folder newName bundle
      let oldFile = GetBundleFile file.Folder file.Name bundle
      let newFilename = ToNewFilename file newName
      let newFile = { Folder = oldFile.Folder; Filename = newFilename; Content = oldFile.Content } : BundleFile
      let files = bundle.Files |> List.map (fun x -> if x = oldFile then newFile else x) 
      { Files = files }

   // namePaths = list of (folder, name without extension)
   let MoveFiles (filesToMove : File list) (newFolder : string) (bundle : Bundle) =
      // Make sure there aren't two files with the same name in the group being moved.
      let names =
         filesToMove
         |> List.map (fun x -> x.Name)
         |> List.sort
         |> List.toSeq
      let firstDuplicate =
         names 
         |> Seq.zip (names |> Seq.skip 1)
         |> Seq.tryFind (fun (a, b) -> a =? b)
      match firstDuplicate with
      | Some (a, b) -> failwith (String.Format(Strings.ErrorDuplicateNameInMove, a))
      | _ -> ()
      // Perform the move.
      let bundleFiles = bundle.Files |> List.map (fun bundleFile ->
         if filesToMove |> List.exists (fun projectFile -> projectFile.Folder =? bundleFile.Folder && 
                                                           projectFile.Name =? (ToNamePart bundleFile.Filename)) then
            // This is one of the files being moved.
            CheckFileDoesNotExist newFolder (ToNamePart bundleFile.Filename) bundle
            { Folder = newFolder; Filename = bundleFile.Filename; Content = bundleFile.Filename } : BundleFile
         else bundleFile)
      { Files = bundleFiles }

   let DeleteFiles (filesToDelete : File list) (bundle : Bundle) =
      let namePathsToDelete = 
         filesToDelete
         |> List.map (fun x -> (x.Folder, x.Name))
      let bundleFiles = 
         bundle.Files
         |> List.filter (fun bundleFile -> 
            let namePath = (bundleFile.Folder, ToNamePart bundleFile.Filename)
            not (namePathsToDelete |> List.exists ((=) namePath)))
      { Files = bundleFiles }

   let DuplicateFile (fileToClone : File) (bundle : Bundle) =
      let HasFile folder name = 
         bundle.Files 
         |> List.exists (fun x -> x.Folder =? folder && (ToNamePart x.Filename) =? name)
      let bundleFile = GetBundleFile fileToClone.Folder fileToClone.Name bundle
      // Find a unique filename by appending a number starting at 2.
      let cloneFilename = 
         Seq.initInfinite (fun x -> x + 2)
         |> Seq.map (fun number -> String.Format("{0}{1}", fileToClone.Name, number))
         |> Seq.find (fun name -> not (HasFile fileToClone.Folder name))
         |> ToNewFilename fileToClone
      let newFile = { Folder = fileToClone.Folder; Filename = cloneFilename; Content = bundleFile.Content }
      { Files = List.Cons(newFile, bundle.Files) }

   let rec DuplicateFiles (filesToClone : File list) (bundle : Bundle) =
      match filesToClone with
      | x :: xs -> (DuplicateFile x bundle) |> (DuplicateFiles xs)
      | [] -> bundle

   let ModifyFile (file : File) (newContent : string) (bundle : Bundle) =
      let oldFile = GetBundleFile file.Folder file.Name bundle
      let newFile = { Folder = file.Folder; Filename = oldFile.Filename; Content = newContent }
      { Files = bundle.Files |> List.map (fun x -> if x = oldFile then newFile else x) }

(*********************************************************************************************************************)
type ProjectCommands () =
   member this.CanUndo with get () = BundleManager.CanUndo()
   member this.CanRedo with get () = BundleManager.CanRedo()
   member this.Undo () = BundleManager.Undo()
   member this.Redo () = BundleManager.Redo()
   
   member this.NewFile folder name pouType pouLanguage =
      BundleManager.Do (FileOperations.NewFile folder name pouType pouLanguage)
      
   member this.RenameFile (file : File) newName =
      if file.Name <> newName then
         BundleManager.Do (FileOperations.RenameFile file newName)

   member this.MoveFiles (files : File seq) newFolder =
      BundleManager.Do (FileOperations.MoveFiles (files |> Seq.toList) newFolder)

   member this.DeleteFiles (files : File seq) =
      BundleManager.Do (FileOperations.DeleteFiles (files |> Seq.toList))

   member this.DuplicateFiles (files : File seq) =
      BundleManager.Do (FileOperations.DuplicateFiles (files |> Seq.toList))

   member this.ModifyFile (file : File) newContent =
      BundleManager.Do (FileOperations.ModifyFile file newContent)

let Commands = new ProjectCommands()
