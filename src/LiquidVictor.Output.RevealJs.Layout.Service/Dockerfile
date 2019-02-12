FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY ./bin/Release/netcoreapp2.2/publish .
ENTRYPOINT ["dotnet", "LiquidVictor.Output.RevealJs.Layout.Service.dll"]