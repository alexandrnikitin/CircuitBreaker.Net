@echo off
cls
set encoding=utf-8
..\.nuget\NuGet.exe restore ..\.nuget\packages.config -PackagesDirectory ..\packages
"..\packages\FAKE.4.3.4\tools\Fake.exe" build.fsx
pause