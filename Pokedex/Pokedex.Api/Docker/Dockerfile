FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Pokedex.Api/Pokedex.Api.csproj", "Pokedex.Api/"]
RUN dotnet restore "Pokedex.Api/Pokedex.Api.csproj"
COPY . .

#Bdd tests use endpoind(https://api.funtranslations.com/) which has restriction(5 requests per hour) to get and translate pokemons. 
#RUN dotnet test

WORKDIR "/src/Pokedex.Api"
RUN dotnet build "Pokedex.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pokedex.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pokedex.Api.dll"]