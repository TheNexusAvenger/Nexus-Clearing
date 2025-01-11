FROM mcr.microsoft.com/dotnet/sdk:9.0 as build
ARG CONFIGURATION_FILE_PATH

# Copy the application files and build them.
WORKDIR /build
COPY . .
RUN dotnet build Nexus.Clearing.Server -c release -r linux-musl-x64 --self-contained -o /publish

# Switch to a container for runtime.
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine as runtime

# Prepare the runtime.
WORKDIR /app
COPY --from=build /publish .
RUN apk add wget
RUN ln -s Nexus.Clearing.Server.dll app.dll
EXPOSE 8000
ENTRYPOINT ["dotnet", "/app/app.dll"]