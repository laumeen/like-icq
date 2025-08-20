# 1. Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyasını kopyala
COPY like-icq.csproj ./

# Gerekli paketleri indir
RUN dotnet restore ./like-icq.csproj

# Tüm dosyaları kopyala
COPY . ./

# Publish et (dikkat: .csproj kullanıyoruz!)
RUN dotnet publish ./like-icq.csproj -c Release -o /app/out

# 2. Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Render için PORT değişkenini kullan
ENV ASPNETCORE_URLS=http://+:$PORT
ENTRYPOINT ["dotnet", "like-icq.dll"]
