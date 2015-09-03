#r @"..\packages\FAKE.4.3.4/tools/FakeLib.dll"
open Fake

RestorePackages()

let product = "CircuitBreaker.Net"
let description = "Implementation of CircuitBreaker pattern on .NET"
let copyright = "Copyright © 2015"
let authors = [ "Alexandr Nikitin" ]
let company = "Alexandr Nikitin"
let tags = ["CircuitBreaker"]
let version = "0.0.1-beta"

let buildDir = "output"
let packagingRoot = "./packaging/"
let packagingDir = packagingRoot @@  product
let packagingSourceDir = packagingRoot @@  product + ".Source"
let nugetPath = "../.nuget/nuget.exe"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; packagingRoot]
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

Target "Build" (fun _ ->
    !! "../src/CircuitBreaker.Net/**/*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "Build-Output: "
)

open Fake.AssemblyInfoFile

Target "AssemblyInfo" (fun _ ->
    let assemblyInfoVersion = version + ".0"
    CreateCSharpAssemblyInfo "../src/CircuitBreaker.Net/Properties/AssemblyInfo.cs"
        [Attribute.Title product
         Attribute.Description description
         Attribute.Copyright copyright
         Attribute.Guid "dc40055a-b114-422e-b9bf-555fe23d9305"
         Attribute.Product product
         Attribute.Version assemblyInfoVersion
         Attribute.FileVersion assemblyInfoVersion]
)

Target "NuGet" (fun _ ->
    let net45Dir = packagingDir @@ "lib/net45/"
    ensureDirectory net45Dir

    CopyFile net45Dir (buildDir @@ "CircuitBreaker.Net.dll")
    CopyFile net45Dir (buildDir @@ "CircuitBreaker.Net.pdb")

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = product
            Description = description
            Summary = description
            Tags = tags |> String.concat " "
            Version = version
            Copyright = copyright

            OutputPath = packagingRoot
            WorkingDir = packagingDir
            ToolPath = nugetPath

            Publish = true
            }) 
            "CircuitBreaker.Net.nuspec"
)

Target "NuGetSource" (fun _ ->
    
    let contentDir = packagingSourceDir @@ "content"
    ensureDirectory contentDir

    CopyFile contentDir ("../src/CircuitBreaker.Net/CircuitBreaker.cs")

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = product + ".Source"
            Description = description
            Summary = description
            Tags = tags |> String.concat " "
            Version = version

            OutputPath = packagingRoot
            WorkingDir = packagingSourceDir
            ToolPath = nugetPath

            Publish = true 
            }) 
            "CircuitBreaker.Net.Source.nuspec"
)

"Clean"
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "NuGet"
  ==> "NuGetSource"
  ==> "Default"

RunTargetOrDefault "Default"