FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/LiquidVictor.Output.RevealJs.Layout.Service/LiquidVictor.Output.RevealJs.Layout.Service.csproj", "src/LiquidVictor.Output.RevealJs.Layout.Service/"]
RUN dotnet restore "src/LiquidVictor.Output.RevealJs.Layout.Service/LiquidVictor.Output.RevealJs.Layout.Service.csproj"
COPY . .
WORKDIR "/src/src/LiquidVictor.Output.RevealJs.Layout.Service"
RUN dotnet build "LiquidVictor.Output.RevealJs.Layout.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "LiquidVictor.Output.RevealJs.Layout.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LiquidVictor.Output.RevealJs.Layout.Service.dll"]