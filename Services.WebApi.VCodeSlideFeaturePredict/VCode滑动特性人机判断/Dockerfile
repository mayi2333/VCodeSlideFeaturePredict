#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-nanoserver-1803 AS build
WORKDIR /src
COPY ["VCode滑动特性人机判断/VCode滑动特性人机判断.csproj", "VCode滑动特性人机判断/"]
RUN dotnet restore "VCode滑动特性人机判断/VCode滑动特性人机判断.csproj"
COPY . .
WORKDIR "/src/VCode滑动特性人机判断"
RUN dotnet build "VCode滑动特性人机判断.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "VCode滑动特性人机判断.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "VCode滑动特性人机判断.dll"]