@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet build --configuration Debug

echo.
echo METAART
cd ..\..\MetaArt\MetaArt
dotnet build --configuration Debug

echo.
echo METABUTTON
cd ..\..\MetaButton\src
dotnet build --configuration Debug

echo.
echo METAART_VIEWERS
cd ..\..\MetaArt\MetaArt.Viewers
dotnet build --configuration Debug MetaArt.Viewers.sln

cd ..\..\MetaArt
