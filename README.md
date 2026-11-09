# RaceDay – Event Management System

## Project Overview

RaceDay is an event management system developed using ASP.NET Core Web API, ASP.NET Core MVC and Microsoft SQL Server.

The system allows event organisers to create and manage sporting events, while participants can browse events, enrol in event categories and view their results.

## Technologies Used

- C#
- ASP.NET Core Web API (.NET 8)
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- Microsoft SQL Server
- HTML, CSS and Bootstrap
- JavaScript
- Azure Blob Storage
- xUnit
- GitHub Actions
- Docker

## System Features

### Organiser
- Organiser registration and login.
- Organiser dashboard.
- Create, view, update and delete events.
- Upload event banner images.
- Create event categories.
- View participant enrolments.
- Record and publish participant results.

### Participant
- Participant registration and login.
- Browse and search sporting events.
- Filter events.
- View event details and categories.
- Enrol in event categories.
- View personal enrolments.
- View personal results.
- Update profile information.
- Upload a profile picture.

## System Architecture

The project uses a three-part architecture:

1. **RaceDay.API:** Handles application data, authentication, database operations and business logic.
2. **RaceDay.MVC:** Provides the user interface and communicates with the API using HTTP requests.
3. **SQL Server:** Stores users, organisers, participants, events, categories, enrolments and results.

Azure Blob Storage is used for image storage.

## Database Setup

1. Install Microsoft SQL Server and SQL Server Management Studio.
2. Create the RaceDay database using the SQL script in the `docs` folder.
3. Configure the API database connection string in the appropriate configuration.
4. Ensure that the database server is running before starting the application.

## Running the Application

### Requirements
- Visual Studio 2022
- .NET 8 SDK
- Microsoft SQL Server
- SQL Server Management Studio

### Steps

1. Clone or download this repository.
2. Open `RaceDay.sln` in Visual Studio 2022.
3. Configure the SQL Server connection string.
4. Configure required JWT and Azure Blob Storage settings securely.
5. Start the RaceDay.API project.
6. Start the RaceDay.MVC project.
7. Open the MVC application in your browser.

The API and MVC application must both be running for the website to communicate with the backend.

## Testing

The API includes an xUnit test project.

The solution was built successfully in Visual Studio 2022, and all five available unit tests passed during the latest local verification.

## Docker

Dockerfiles have been prepared for the API and MVC projects.

Docker Desktop installation and container deployment have not yet been completed or verified.

## Repository Structure

- `.github/workflows` – GitHub Actions workflow.
- `docs` – Project planning and database documentation.
- `RaceDay.API` – Backend Web API.
- `RaceDay.API.Tests` – Automated tests.
- `Controllers` – MVC controllers.
- `Models` – MVC models.
- `Services` – API communication services.
- `Views` – MVC user interface.
- `wwwroot` – Static website files.
- `Program.cs` – MVC application configuration.
- `RaceDay.MVC.csproj` – MVC project file.
- `Dockerfile` – MVC Docker configuration.

## GitHub Repository

[https://github.com/Njabuo/Raceday](https://github.com/Njabuo/Raceday)

# Raceday
