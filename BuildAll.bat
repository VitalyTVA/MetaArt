@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet restore
dotnet build --configuration Debug

echo.
echo METAART
cd ..\MetaArt\MetaArt
dotnet restore
dotnet build --configuration Debug

echo.
echo METABUTTON
cd ..\MetaButton\src
dotnet restore
dotnet build --configuration Debug

cd ..\..\MetaArt
