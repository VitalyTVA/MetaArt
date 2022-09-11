@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet restore
dotnet build --configuration Debug

echo.
echo METAART
cd ..\..\MetaArt\MetaArt
dotnet restore MetaArt.sln
dotnet build --configuration Debug MetaArt.sln

echo.
echo METABUTTON
cd ..\..\MetaButton\src
dotnet restore
dotnet build --configuration Debug

cd ..\..\MetaArt
