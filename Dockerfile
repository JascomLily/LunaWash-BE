# Stage 1: Build ứng dụng sử dụng .NET 10 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy các file .csproj vào đúng cấu trúc để chạy dotnet restore trước (tối ưu cache Docker)
COPY ["LunaWash.API/LunaWash.API.csproj", "LunaWash.API/"]
COPY ["LunaWash.BLL/LunaWash.BLL.csproj", "LunaWash.BLL/"]
COPY ["LunaWash.DAL/LunaWash.DAL.csproj", "LunaWash.DAL/"]

# Thực hiện restore các dependencies cho project chính
RUN dotnet restore "LunaWash.API/LunaWash.API.csproj"

# Copy toàn bộ phần code còn lại vào container
COPY . .

# Biên dịch và publish dự án ra thư mục /app/publish
WORKDIR "/src/LunaWash.API"
RUN dotnet publish "LunaWash.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Chạy ứng dụng bằng .NET 10 Runtime nhỏ gọn
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render yêu cầu container lắng nghe một cổng, mặc định ta chọn 10000
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Lệnh khởi chạy Web API bằng file dll chính
ENTRYPOINT ["dotnet", "LunaWash.API.dll"]