# MyBook: Simple Book Management Web API
<img src="https://github.com/emaadgh/mybook/assets/10380342/8c04de39-c177-4dd8-866e-8fa6efd7e98d" width="150" height="150">

**MyBook** is a well-structured and versatile **ASP.NET Core Web API** designed to streamline book-related data management. MyBook offers the essential endpoints and features to effectively handle book CRUD (Create, Read, Update, Delete) operations and enhancements for filtering, searching, ordering, pagination, and data shaping. Additionally, MyBook supports HATEOAS links, allowing clients to navigate the API dynamically by providing relevant links in the responses.

## Getting Started

1. **Clone the Repository**:
    ```bash
    git clone https://github.com/emaadgh/mybook.git
    ```

2. **Install Dependencies**:
    - Ensure you have **.NET SDK 8** installed.
    - Open a terminal and navigate to the project folder:
        ```bash
        cd mybook
        ```
    - Restore dependencies:
        ```bash
        dotnet restore
        ```

    **Dependencies** (for MyBook API project):
    - AutoMapper
    - Microsoft.AspNetCore.JsonPatch
    - Microsoft.AspNetCore.Mvc.NewtonsoftJson
    - Microsoft.EntityFrameworkCore.SqlServer
    - Microsoft.EntityFrameworkCore.Tools
    - Microsoft.VisualStudio.Azure.Containers.Tools.Targets
    - Swashbuckle.AspNetCore
    - System.Linq.Dynamic.Core

    **Dependencies** (for MyBook Test project):
    - Microsoft.NET.Test.Sdk
    - Moq
    - xunit
    - xunit.runner.visualstudio
    - coverlet.collector
    - FluentAssertions

3. **Run the Application**:
    - Start the API by running the following command in your terminal:
        ```bash
        dotnet run
        ```
4. **Database Migration**:
    - After building the project, run the following command to create or update the database based on migration files:
        ```bash
        dotnet ef database update
        ```
        If you don't have the Entity Framework Core tools (`dotnet ef`) installed globally, you can install them by running the following command:
        ```bash
        dotnet tool install --global dotnet-ef
        ```

5. **Access the API**:
    - By default, the API will be hosted on `localhost` with a randomly assigned port. You can access the API using the following URL format:
        ```
        https://localhost:<PORT>/api
        ```
    - Replace `<PORT>` with the port number assigned to the API during startup.

6. **Explore the API**:
    - Once the API is running, you can use tools like **Swagger** or **Postman** to interact with the endpoints.
    - Visit the Swagger UI at `https://localhost:<PORT>/swagger/index.html` to explore the API documentation interactively.
  
## Exploring the API with Postman

1. **Import Postman Collection**:
    - Locate the `MyBook.postman_collection.json` file in the project repository.
    - Import this collection into Postman using the following steps:
        - Open Postman.
        - Click on the "Import" button in the top left corner.
        - Select the option to "Import From File" and choose the `MyBook.postman_collection.json` file.
        - Click "Import" to add the collection to your Postman workspace.

2. **Set Base URL**:
    - Once the collection is imported, navigate to the collection settings in Postman.
    - In the "Variables" section, locate the variable named `base_url`.
    - Set the value of this variable to the base URL of your API. You can specify the URL and port here, e.g., `https://localhost:5001`.
    - This base URL will be automatically used for all requests within the collection.

3. **Explore Endpoints**:
    - Now you can explore and interact with the API endpoints conveniently using the imported Postman collection.
    - Each request in the collection will utilize the base URL configured in the variables, making it easy to test different endpoints with your local setup
      
4. **Written Tests**:
    - The Postman collection includes written tests for various requests to validate their responses.
    - These tests ensure that the responses meet expected criteria, such as status codes, data types, and content format.
    - You can run these tests within Postman to verify the correctness of API responses.
  
## AppSettings.json Configuration

Please note that `appsettings.json` files are not included in the Git repository. These files typically contain sensitive information such as database connection strings, API keys, and other configuration settings specific to the environment.

For local testing, you can create your own `appsettings.json` file in the root directory of the MyBook project and add the necessary configurations. Make sure to include the `MyBookDbContextConnection` connection string for the SQL Server database if you're using one locally.

If you deploy the MyBook API to Azure App Service, you can add the connection string in the Connection Strings section of the Configuration settings in the Azure portal. Azure App Service securely encrypts connection strings at rest and transmits them over an encrypted channel, ensuring the security of your sensitive information.

## CI/CD Pipeline

This repository includes a CI/CD pipeline that automates the build, test, and deployment processes for the MyBook project.

### Pipeline Configuration

The repository contains a configuration file named `azure-pipeline.yml` which defines the CI/CD pipeline. This file can be used with Microsoft Azure DevOps's Pipeline service.

The pipeline configuration orchestrates the following tasks:
- Dependency restoration
- Building the project
- Running tests
- Publishing artifacts for deployment

### Usage

The CI/CD pipeline ensures that changes to the project are automatically validated, built, and prepared for deployment, streamlining the development process and maintaining code quality.

In addition, we utilize Azure DevOps Pipeline Releases to continuously deploy artifacts from the CI pipeline to an Azure app service in the cloud. This allows for seamless and automated deployment of updates to the application environment.

<img src="https://github.com/emaadgh/mybook/assets/10380342/bd11d934-e762-4ceb-88cc-0013123dba03" width="589.5" height="314">

Users can leverage this pipeline configuration in Microsoft Azure DevOps's Pipeline service to automate the software delivery process.

## Dockerizing the API with Dockerfile and Docker Compose

The MyBook API can be easily containerized using DockerFile along with Docker Compose to orchestrate multiple containers. Docker provides a consistent environment across different systems, making it easier to deploy and manage applications.

To Dockerize the MyBook API and run it with Docker Compose, follow these steps:

1. Ensure you have Docker installed on your system.
2. Navigate to the root directory of the MyBook project.
3. Create a `.env` file beside the `docker-compose.yml` file.
4. Add the following environment variable to the `.env` file:
```
SA_PASSWORD=preferedpassword
```
Replace `preferedpassword` with your preferred SQL Server SA password.

5. Run the following command in the terminal to start the containers:
```
docker-compose up
```
This will build the MyBook API Docker image and start the containers for the API and the SQL Server database.

With Docker and Docker Compose, you can easily deploy and manage the MyBook API in a containerized environment, ensuring consistency and scalability across different systems.

## MyBook Test Project

The `MyBook.UnitTests` project contains unit tests that validate the behavior of various components within the MyBook API. These unit tests cover Controllers, Helper classes, AutoMapper Profiles, ResourceParameters, and Services used in the project.

These tests are crucial for ensuring the correctness and reliability of the MyBook API. They are automatically executed as part of the CI/CD pipeline to maintain code quality and identify any regressions that may occur during development.

## Database Reset for Testing Purposes

For testing purposes, you can add the following code snippet to your `Program.cs` file. This code will reset the database and its data every time the application is run:

```
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetService<MyBookDbContext>();
        if (context != null)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        }
    }
    catch (Exception ex)
    {
        // Handle any exceptions if necessary
    }
}
```
Make sure to replace `MyBookDbContext` with your actual DbContext class if it's named differently.

## Creator

- [Emaad Ghorbani](https://github.com/emaadgh)

## Contributing

Contributions are welcome! If you'd like to enhance MyBook, feel free to submit pull requests or open issues.

## License

This project is licensed under the MIT License.
