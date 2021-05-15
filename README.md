# Airly.Net

Unoffical powerfull and fast .NET API wrapper for [**Airly**](https://developer.airly.org/docs)<br>
If you want to integrate your application with the API from Airly this is what you're looking for

- Async based 
- 100% converage with the Airly API
- Fully safe and easy to use
- No more than one request per invoked method*

## Documentation

You can acces the documentation [**here**](https://mondonno.github.io/airly.net)<br>
Or visit project Wiki

## Installation

Airly.Net stable builds are avaible on NugGet.org through the Airly.Net meta package
- [**Airly.Net**](https://www.nuget.org/packages/AirlyNet/)

## Compiling
You can compile our library with the following dependencies
- `Visual Studio 8.8+`
- `.NET Core SDK 5.0+`

Or via `.NET Command line CLI`
- `.NET Core SDK 5.0+`

C# `9.0+` for all combinations

## Example of usage
```csharp
string myApiKey = "myFullyGoodAirlyApiKey"; // Define your apikey
Airly client = new Airly(myApiKey); // Create new instance of the Api Client

Location location = new Location(0, 0) // Provide coordinates (lat, lng)
Measurment measurment = await client.Measurments.Nearest(location) // Pass it

DateTime fromMeasurmentDateTime = measurment.Current.FromDateTime; // Destruct actual measurments from date time
Console.WriteLine(fromMeasurmentDateTime.ToString()); // Show it over the world
```
*Always remember to add after packet installation `using AirlyNet`*

#### [Polish Version](./README_POLISH.md)