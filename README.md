# AspireStack

A .NET Aspire Startup Template with Angular Frontend

## Overview

AspireStack is a starter project template designed to accelerate the development of modern, cloud-native applications using .NET Aspire. It provides a pre-configured environment with an Angular frontend, PostgreSQL database, and Redis cache, enabling you to quickly build and deploy scalable and resilient applications.

## Features

-   **.NET Aspire Integration:** Leverages the power of .NET Aspire for simplified service discovery, configuration, and observability.
-   **Angular Frontend:**  Includes a fully functional Angular UI to get you started on the user interface components of your application.
-   **PostgreSQL Database:**  Pre-configured to use PostgreSQL as a relational database for storing application data.
-   **Redis Cache:** Utilizes Redis for caching frequently accessed data, improving application performance.
-   **Authentication and Authorization:** Already configured to get you started quickly.
-   **Modular Structure:**  Follows a modular structure in both the backend and frontend, promoting maintainability and scalability.
-   **Easy Setup:**  Designed for quick setup and deployment with minimal configuration required.

## Project Structure

-   **AspireStack.Application:** Contains application logic, services, and business rules.
    -   **AppService:** Contains app services and contracts
    -   **Localization:** Contains services related to localization.
    -   **RoleManagement:** Contains services related to Role management.
    -   **Security:** Contains security related classes.
    -   **UserManagement:** Contains services related to user management.
    -   **AspireStack.Application.csproj:** .NET Application Project file.
-   **AspireStack.DbInitializer:** DbInitializer class
-   **AspireStack.Angular.ServiceGenerator:** Angular service generator class
-   **AspireStack.Angular:** Contains the Angular frontend application.
    -   **src/app:** Contains the main application source code.
        -   **modules:**  Organized Angular modules for different features (e.g., dashboard, user-management, role-management).
        -   **pages:** Angular page compoennts
        -   **shared:**  Shared components and services.
        -   **services:**  Angular Services
    -   **src/assets:** Static assets (images, fonts, etc.).
    -   **src/scss:** SCSS stylesheets.
    -   **src/types:**  TypeScript type definitions.
    -   **AspireStack.Angular.njsproj:** .NET Angular Project file.
-   **.dockerignore:** Specifies intentionally untracked files that Docker should ignore.
-   **.gitignore:** Specifies intentionally untracked files that Git should ignore.
-   **LICENSE:** License file for the project.
-   **README.md:** Project documentation (this file).
-   **RenameAspireProject.ps1:** Powershell script to rename aspire project name.

## Getting Started

1.  **Prerequisites:**
    -   .NET 9 SDK or later
    -   Node.js and npm (for the Angular frontend)
    -   Docker Desktop (for containerized deployment)
2.  **Clone the repository:**

    ```bash
    git clone https://github.com/fermanquliyev/AspireStack.git
    cd AspireStack
    ```

3.  **Install .NET dependencies:**

    ```bash
    dotnet restore
    ```

4.  **Install Angular dependencies:**

    ```bash
    cd AspireStack.Angular
    npm install
    cd ..
    ```

5.  **Configure your environment:**
    -   Make sure Docker desktop is running
    -   Set AspireStack.AppHost as a startup project

6.  **Run the application:**

    ```bash
    dotnet run --project AspireStack
    ```

7.  **Access the application:**

    -   Browser will be open with Aspire dashboard

## Deployment

AspireStack is designed for containerized deployment. You can use Docker to build and deploy the application to various cloud platforms.

1.  **Build the Docker image:**

    ```bash
    docker build -t aspirestack .
    ```

2.  **Run the Docker container:**

    ```bash
    docker run -p 80:80 aspirestack
    ```

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please submit a pull request.
