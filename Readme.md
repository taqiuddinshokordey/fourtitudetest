# FourtitudeTest Application

## How to Run the .NET Application

1. **Install .NET SDK:**
    Ensure you have the .NET SDK installed. You can download it from [here](https://dotnet.microsoft.com/download).

2. **Clone the Repository:**
    ```sh
    git clone https://github.com/your-repo/fourtitudeTest.git
    cd fourtitudeTest/fourtitudetest
    ```

3. **Restore Dependencies:**
    ```sh
    dotnet restore
    ```

4. **Build the Application:**
    ```sh
    dotnet build
    ```

5. **Run the Application:**
    ```sh
    dotnet run
    ```

## How to Deploy in Docker

1. **Install Docker:**
    Ensure you have Docker installed. You can download it from [here](https://www.docker.com/products/docker-desktop).

2. **Create a Dockerfile:**
    Create a `Dockerfile` in the root of your project with the following content:
    ```dockerfile
    FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
    WORKDIR /app
    EXPOSE 80

    FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
    WORKDIR /src
    COPY ["fourtitudetest/fourtitudetest.csproj", "fourtitudetest/"]
    RUN dotnet restore "fourtitudetest/fourtitudetest.csproj"
    COPY . .
    WORKDIR "/src/fourtitudetest"
    RUN dotnet build "fourtitudetest.csproj" -c Release -o /app/build

    FROM build AS publish
    RUN dotnet publish "fourtitudetest.csproj" -c Release -o /app/publish

    FROM base AS final
    WORKDIR /app
    COPY --from=publish /app/publish .
    ENTRYPOINT ["dotnet", "fourtitudetest.dll"]
    ```

3. **Build the Docker Image:**
    ```sh
    docker build -t fourtitudetest:latest .
    ```

4. **Run the Docker Container:**
    ```sh
    docker run -d -p 8080:80 --name fourtitudetest_container fourtitudetest:latest
    ```

5. **Access the Application:**
    Open your browser and navigate to `http://localhost:8080`.

That's it! You have successfully run and deployed your .NET application using Docker.