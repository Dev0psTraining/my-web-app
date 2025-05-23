# DevOps Pipeline Demo

This project demonstrates a simplified DevOps pipeline implemented in a local environment for an ASP.NET Core web application.

## Project Overview

This DevOps pipeline demonstration includes:

- A simple ASP.NET Core web application with form submission functionality
- Git version control with main and dev branches
- Continuous Integration using GitHub Actions
- Infrastructure as Code using Ansible
- Continuous Deployment with Blue-Green deployment strategy
- Health monitoring and automatic rollback

## Technologies Used

- **Web Application**: ASP.NET Core 7.0
- **Version Control**: Git and GitHub
- **CI/CD**: GitHub Actions
- **Infrastructure as Code**: Ansible
- **Testing Framework**: xUnit
- **Deployment Strategy**: Blue-Green Deployment

## CI/CD Pipeline Explanation

This project implements a complete CI/CD pipeline:

1. **Continuous Integration:**
   - Automated builds trigger on pushes to main and dev branches
   - Runs tests automatically
   - Performs code quality checks (dotnet-format)

2. **Infrastructure as Code:**
   - Ansible playbook configures the local environment
   - Installs necessary dependencies (dotnet SDK)
   - Creates required directories for blue-green deployment

3. **Continuous Deployment:**
   - Blue-Green deployment strategy for zero-downtime deployments
   - Health checks to verify successful deployment
   - Automatic rollback if deployment fails

## Folder Structure

```
my-web-app/
├── .github/
│   └── workflows/
│       └── ci-cd.yml          # GitHub Actions workflow
├── ansible/
│   └── setup.yml              # Ansible playbook
├── scripts/
│   ├── deploy.sh              # Deployment script
│   └── health_check.sh        # Health monitoring script
├── src/
│   └── my-web-app/
│       ├── Controllers/
│       │   └── HomeController.cs
│       ├── Models/
│       │   ├── ErrorViewModel.cs
│       │   └── FeedbackModel.cs
│       ├── Services/
│       │   └── MessageService.cs
│       ├── Views/
│       │   ├── Home/
│       │   │   ├── Index.cshtml
│       │   │   ├── Privacy.cshtml
│       │   │   ├── Feedback.cshtml
│       │   │   └── Confirmation.cshtml
│       │   └── Shared/
│       │       ├── _Layout.cshtml
│       │       └── Error.cshtml
│       ├── wwwroot/
│       │   ├── css/
│       │   ├── js/
│       │   └── lib/
│       ├── Program.cs
│       └── my-web-app.csproj
├── tests/
│   └── my-web-app.Tests/
│       ├── HomeControllerTests.cs
│       ├── MessageServiceTests.cs
│       └── my-web-app.Tests.csproj
└── my-web-app.sln
```

## Setup and Deployment

### Local Environment Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/my-web-app.git
   cd my-web-app
   ```

2. Set up the local environment using Ansible:
   ```bash
   cd ansible
   ansible-playbook setup.yml
   ```

3. Build and test the application:
   ```bash
   cd ../src/my-web-app
   dotnet build
   dotnet test ../../tests/my-web-app.Tests
   ```

### Deployment Process

1. Push changes to the dev branch:
   ```bash
   git checkout dev
   # Make your changes
   git add .
   git commit -m "Meaningful commit message"
   git push origin dev
   ```

2. Create a pull request from dev to main.

3. After the PR is merged, the GitHub Actions workflow will automatically:
   - Build the application
   - Run tests
   - Publish the application (if on main branch)

4. Deploy the application using the deployment script:
   ```bash
   cd ../../scripts
   chmod +x deploy.sh
   ./deploy.sh
   ```

5. Start the health monitoring script:
   ```bash
   chmod +x health_check.sh
   ./health_check.sh &
   ```

## Blue-Green Deployment Strategy

This project uses a Blue-Green deployment strategy for zero-downtime deployments:

1. Two identical environments (blue and green) are maintained.
2. Only one environment is active at a time, serving production traffic.
3. Deployments are made to the inactive environment.
4. After successful deployment and health checks, traffic is switched to the newly deployed environment.
5. If deployment fails, no switching occurs, ensuring zero downtime.

## Monitoring and Health Checks

The health check script continuously monitors the application and logs its status. If the application becomes unhealthy, the script attempts to restart it automatically.

## Rollback Mechanism

If a deployment fails health checks, the deployment script will not switch to the new environment, effectively maintaining the previous working version. For manual rollback, you can use:

```bash
cd scripts
./deploy.sh rollback
```

## Project Workflow Diagram

![Screenshot 2025-04-21 024110](https://github.com/user-attachments/assets/f2bf4cb1-3ab1-4963-8510-6df2d7047157)


## Evaluation Criteria:


- The DevOps principles applied in the project
- The CI/CD pipeline workflow
- The Blue-Green deployment strategy
- Infrastructure as Code implementation
- Advantages of the implemented approach


