@echo off

echo.
echo METACORE
cd ..\MetaCore\src
dotnet test -clp:ErrorsOnly

echo.
echo METAART
cd ..\..\MetaArt\MetaArt
dotnet test -clp:ErrorsOnly

echo.
echo META3D
cd ..\Meta3D
dotnet test -clp:ErrorsOnly

echo.
echo METABUTTON
cd ..\..\MetaButton\src
dotnet test -clp:ErrorsOnly

echo.
echo METAART_VIEWERS
cd ..\..\MetaArt\MetaArt.Viewers
dotnet test -clp:ErrorsOnly MetaArt.Viewers.sln

cd ..\..\MetaArt
