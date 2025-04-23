# my-dotnet-webapi-app

## Overview
This project is a .NET Core Web API that fetches the top 200 stories from the Hacker News API. It includes features such as caching, exception handling, and XML documentation.

## Features
- Fetches top 200 stories with Title and URL properties from the Hacker News API.
- Implements caching to improve performance.
- Custom exception handling middleware for standardized error responses.
- Dependency injection for better modularity and testability.

## Project Structure
```
my-dotnet-webapi-app
├── src
│   ├── Controllers
│   │   └── StoriesController.cs
│   ├── Middleware
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Models
│   │   └── Story.cs
│   ├── Services
│   │   ├── IHackerNewsService.cs
│   │   └── HackerNewsService.cs
│   ├── Caching
│   │   └── CacheProvider.cs
│   ├── Program.cs
│   └── my-dotnet-webapi-app.csproj
├── tests
│   ├── UnitTests
│   │   ├── CachingTests.cs
│   │   ├── ControllerTests.cs
│   │   └── ServiceTests.cs
│   └── my-dotnet-webapi-app.Tests.csproj
└── README.md
```

## Setup Instructions
1. Clone the repository:
   ```
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```
   cd my-dotnet-webapi-app
   ```
3. Restore the dependencies:
   ```
   dotnet restore
   ```
4. Run the application:
   ```
   dotnet run --project src/my-dotnet-webapi-app.csproj
   ```

## Usage
- The API endpoint to fetch the top 200 stories is `/api/stories`.
- You can test the API using tools like Postman or curl.

## Testing
To run the unit tests, navigate to the tests directory and execute:
```
dotnet test
```

## License
This project is licensed under the MIT License.