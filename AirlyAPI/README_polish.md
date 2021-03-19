# Airly.Net
Nieoficjalny mocny i szybki .NET api wrapper napisany w C# dla [Airly](https://developer.airly.org/docs)

- Bazowany asynchronicznie
- 100% pokryty z Airly API
- W pełni bezpieczny i szybki
- Nie więcej niz 1 zapytanie na wywołaną metode*

# Dokumentacja
Dostęp do dokumentacji mozesz uzyskać [tutaj](https://mondonno.github.io/airly.net)
Lub mozesz odwiedzic Wiki naszego projektu

# Instalacja
Stabilne kompilacje Airly.Net są dostępne za pośrednictwem platformy Nuget.org
    - [Airly.Net]()

# Kompilowanie
Skompilować Airly.Net mozesz razem z tymi programami
    - Visual Studio 8.8+
    - .NET Core SDK 5.0+
Lub za pomocą .NET Command CLI
    - .NET Command CLI 5.0
Wymagany C# 9.0 dla wszystkich kompilacji

# Przykład uzycia
```csharp
string myApiKey = "mójStuprocentowoDobryApiKey"; // Zdefinuj swój klucz api
Airly client = new Airly(myApiKey); // Stwórz nową instancję klienta Airly API

Location location = new Location(0, 0) // Zdefiniuj koordynaty (lat, lng)
Measurment measurment = await client.Measurments.Nearest(location) // Podaj je

DateTime fromMeasurmentDateTime = measurment.Current.FromDateTime; // Zdectruktuj datę aktualnego pomairy zanieczyszczeń
Console.WriteLine(fromMeasurmentDateTime.ToString()); // Pokaz je światu
```