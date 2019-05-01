# this script is for generating coverage reports to the path c:\coverlet - the path is hardcoded for supporting PrestoCoverage extension
# requires installing coverlet, and may require .net core 2.2+ - `dotnet tool install --global coverlet.console`
dotnet test
coverlet.exe .\TypeSupport.Tests\bin\Debug\netcoreapp2.1\TypeSupport.Tests.dll --target "dotnet" --targetargs "test .\TypeSupport.Tests\TypeSupport.Tests.csproj --no-build" --output "C:\coverlet\coverage" --format json