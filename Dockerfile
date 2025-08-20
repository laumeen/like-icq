# 1. Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala
COPY ["like-icq.csproj", "./"]
RUN dotnet restore "./like-icq.csproj"

# Geri kalan tüm dosyaları kopyala
COPY . .

# Build ve publish
RUN dotnet publish "./like-icq.csproj" -c Release -o /app/publish

# 2. Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "like-icq.dll"]
