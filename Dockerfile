# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY PFE.ExpenseTracker.sln ./
COPY src/PFE.ExpenseTracker.API/PFE.ExpenseTracker.API.csproj ./src/PFE.ExpenseTracker.API/
COPY src/PFE.ExpenseTracker.Application/PFE.ExpenseTracker.Application.csproj ./src/PFE.ExpenseTracker.Application/
COPY src/PFE.ExpenseTracker.Domain/PFE.ExpenseTracker.Domain.csproj ./src/PFE.ExpenseTracker.Domain/
COPY src/PFE.ExpenseTracker.Infrastructure/PFE.ExpenseTracker.Infrastructure.csproj ./src/PFE.ExpenseTracker.Infrastructure/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /src/src/PFE.ExpenseTracker.API
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PFE.ExpenseTracker.API.dll"]
