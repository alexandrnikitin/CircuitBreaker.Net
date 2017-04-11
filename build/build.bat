@echo off
cls
set encoding=utf-8
..\.nuget\NuGet.exe restore ..\.nuget\packages.config -PackagesDirectory ..\packages

SET TARGET="Default"
IF NOT [%1]==[] (set TARGET="%1")

SET BUILDMODE="Release"
IF NOT [%2]==[] (set BUILDMODE="%2")

"..\packages\FAKE.4.58.5\tools\Fake.exe" build.fsx "target=%TARGET%" "buildMode=%BUILDMODE%"
pause