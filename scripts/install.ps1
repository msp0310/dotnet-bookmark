pushd ../src
dotnet pack 
dotnet tool install --global --add-source ./nupkg dotnet-bookmark
popd