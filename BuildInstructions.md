# How to build Kongo

Download and install [Dotnet Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0)

Change directory to location of Kongo.csproj  (https://github.com/stakeyourada/Kongo/blob/master/Kongo/Kongo.csproj)

Open command prompt or terminal 

- To Build

```code
   dotnet build --configuration Release
```
    
- To run Tests

```code
   dotnet test --configuration Release
```

- To Publish
```code
   dotnet publish --configuration Release
```
