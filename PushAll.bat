@echo off

echo.
echo METAART
git push

echo.
echo METACORE
cd ../metacore
git push

echo.
echo METABUTTON
cd ../metabutton
git push
cd ../metaart