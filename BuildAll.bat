@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet restore
dotnet build --configuration Debug

echo.
echo METAART
cd ..\..\MetaArt\MetaArt
dotnet restore
dotnet build --configuration Debug

echo.
echo METABUTTON
cd ..\..\MetaButton\src
dotnet restore
dotnet build --configuration Debug

echo.
echo METAART_VIEWERS
cd ..\..\MetaArt\MetaArt.Viewers
dotnet restore MetaArt.Viewers.sln
dotnet build --configuration Debug MetaArt.Viewers.sln

cd ..\..\MetaArt
