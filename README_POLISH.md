# Airly.Net
Nieoficjalny mocny i szybki .NET api wrapper napisany w C# dla [Airly](https://developer.airly.org/pl/docs)<br>
Jeśli chcesz zintegrować swoja aplikacje z API od airly to jest to czego szukasz

- Bazowany asynchronicznie
- 100% pokryty z Airly API
- W pełni bezpieczny i szybki
- Nie więcej niz 1 zapytanie na wywołaną metode*

## Dokumentacja
Dostęp do dokumentacji mozesz uzyskać [**tutaj**](https://mondonno.github.io/airly.net)<br>
Lub mozesz odwiedzic Wiki naszego projektu

## Instalacja
Stabilne kompilacje **Airly.Net** są dostępne za pośrednictwem platformy Nuget.org

- [**Airly.Net**](https://www.nuget.org/packages/AirlyNet/)

## Kompilowanie
Skompilować `Airly.Net` mozesz razem z tymi programami

- `Visual Studio 8.8+`
- `.NET Core SDK 5.0+`

Lub za pomocą `.NET Command CLI`

- `.NET Command CLI 5.0`

Wymagany `C# 9.0` dla wszystkich kompilacji

## Przykład uzycia
```csharp
string myApiKey = "mójStuprocentowoDobryApiKey"; // Zdefinuj swój klucz api
Airly client = new Airly(myApiKey); // Stwórz nową instancję klienta Airly API

Location location = new Location(0, 0) // Zdefiniuj koordynaty (lat, lng)
AirMeasurment measurment = await client.Measurments.Nearest(location) // Podaj je

DateTime fromMeasurmentDateTime = measurment.Current.FromDateTime; // Zdectruktuj datę aktualnego pomairy zanieczyszczeń
Console.WriteLine(fromMeasurmentDateTime.ToString()); // Pokaz je światu
```
*Zawsze pamiętaj aby po instalacji pakietu dodać na początku pliku `using AirlyNet`*<br>
**Notka**: musisz dodać `using AirlyNet.Models` jeśli chcesz uzyć `Location`, `Address` itp. itd.

#### [Wersja w języku Angielskim](https://github.com/Mondonno/Airly.Net)
