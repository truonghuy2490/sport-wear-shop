dotnet add tests/SportWearShop.UnitTests package Moq
dotnet add tests/SportWearShop.UnitTests package coverlet.collector
dotnet add package MockQueryable.Moq
dotnet add package Moq

reference businesslogics 

 dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults --settings ./coverlet.runsettings  
 reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:Html