@echo off
setlocal

set "version_number=11"

echo "Building Xml.Revit.Toolkit"
set project=".\Xml.Revit.Toolkit\Xml.Revit.Toolkit.csproj"
echo "Building all releases..."
for /l %%i in (17,1,27) do (
    echo "Building Release R%%i"
    dotnet build -c "Release R%%i" %project%
)

set /p v=input end version(version.%version_number%.*):

echo "Pushing NuGet packages..."
for /l %%i in (17,1,27) do (
    echo "Pushing Release R%%i"
    dotnet nuget push ".\xml.Revit.Toolkit\bin\Release R%%i\xml.Revit.Toolkit.20%%i.%version_number%.%v%.nupkg" -k %NUGET_API_KEY% ^ -s https://api.nuget.org/v3/index.json --skip-duplicate
)

echo "Build and publish complete."
