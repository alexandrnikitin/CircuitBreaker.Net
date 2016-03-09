#r @"../packages/FAKE.4.3.4/tools/FakeLib.dll"
open Fake

RestorePackages()

let product = "CircuitBreaker.Net"
let description = "Implementation of CircuitBreaker pattern on .NET"
let copyright = @"Copyright © 2015"
let authors = [ "Alexandr Nikitin" ]
let company = "Alexandr Nikitin"
let tags = ["CircuitBreaker"]
let version = "0.2.0-alpha"

let buildDir = "output"
let buildTestsDir = "output-tests"
let packagingRoot = "./packaging/"
let packagingDir = packagingRoot @@  product
let packagingSourceDir = packagingRoot @@  product + ".Source"
let nugetPath = "../.nuget/nuget.exe"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; buildTestsDir; packagingRoot]
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

Target "Publish" (fun _ ->
    trace "Publish nuget packages"
)

Target "Build" (fun _ ->
    !! "../src/**/*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "Build-Output: "
)

Target "BuildTests" (fun _ ->
    !! "../tests/**/*.csproj"
        |> MSBuildRelease buildTestsDir "Build"
        |> Log "BuildTests-Output: "
)

open Fake.Testing
let testDlls = !! (buildTestsDir + "/*.Tests.dll")

Target "Test" (fun _ ->
    testDlls
        |> xUnit2 (fun p -> 
            {p with 
                ToolPath = "../packages/xunit.runner.console.2.1.0-beta4-build3109/tools/xunit.console.exe"
                ShadowCopy = false
                HtmlOutputPath = Some (buildTestsDir @@ "xunit-html-output.html")})
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

            Publish = false
            }) 
            "CircuitBreaker.Net.nuspec"
)

Target "NuGetSource" (fun _ ->
    
    let contentDir = packagingSourceDir @@ "content"
    ensureDirectory contentDir
    let contentCircuitBreakerDir = contentDir @@ "CircuitBreaker"
    ensureDirectory contentCircuitBreakerDir
    let contentCircuitBreakerStatesDir = contentCircuitBreakerDir @@ "States"
    ensureDirectory contentCircuitBreakerStatesDir
    let contentCircuitBreakerExceptionsDir = contentCircuitBreakerDir @@ "Exceptions"
    ensureDirectory contentCircuitBreakerExceptionsDir

    CopyFile contentCircuitBreakerDir ("../src/CircuitBreaker.Net/CircuitBreaker.cs")
    CopyFile contentCircuitBreakerDir ("../src/CircuitBreaker.Net/CircuitBreakerInvoker.cs")
    CopyFile contentCircuitBreakerDir ("../src/CircuitBreaker.Net/ICircuitBreaker.cs")
    CopyFile contentCircuitBreakerDir ("../src/CircuitBreaker.Net/ICircuitBreakerInvoker.cs")
    CopyFile contentCircuitBreakerDir ("../src/CircuitBreaker.Net/ICircuitBreakerSwitch.cs")

    CopyFile contentCircuitBreakerStatesDir ("../src/CircuitBreaker.Net/States/ClosedCircuitBreakerState.cs")
    CopyFile contentCircuitBreakerStatesDir ("../src/CircuitBreaker.Net/States/HalfOpenCircuitBreakerState.cs")
    CopyFile contentCircuitBreakerStatesDir ("../src/CircuitBreaker.Net/States/ICircuitBreakerState.cs")
    CopyFile contentCircuitBreakerStatesDir ("../src/CircuitBreaker.Net/States/OpenCircuitBreakerState.cs")

    CopyFile contentCircuitBreakerExceptionsDir ("../src/CircuitBreaker.Net/Exceptions/CircuitBreakerExecutionException.cs")
    CopyFile contentCircuitBreakerExceptionsDir ("../src/CircuitBreaker.Net/Exceptions/CircuitBreakerOpenException.cs")
    CopyFile contentCircuitBreakerExceptionsDir ("../src/CircuitBreaker.Net/Exceptions/CircuitBreakerTimeoutException.cs")

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

            Publish = false
            }) 
            "CircuitBreaker.Net.Source.nuspec"
)

"Clean"
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "BuildTests"
  ==> "Test"
  ==> "Default"
  ==> "NuGet"
  ==> "NuGetSource"
  ==> "Publish"


RunTargetOrDefault "Default"