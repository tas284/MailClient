# MailClient

## Email Consumer and API - ASP.NET Core Project

Project to read emails from a specified inbox using an ASP.NET Core API and a console application (consumer). The API authenticates with an email server to retrieve emails for a specified period and sends them to a RabbitMQ queue. The consumer then processes these messages and saves them to a MongoDB database.

### Requirements
1. NET SDK (version 8.0 or higher)
2. MongoDB
3. RabbitMQ

### Execution Options
1. ASP.NET Core Web Application
The API serves as the entry point for email retrieval.

1. Console Application (Consumer)
Processes messages from the RabbitMQ queue and saves them to the database.

### Build and Run

#### To run this project, enter the root directory and execute the following commands:

```bash
dotnet build
```

```bash
dotnet run .\MailClient.API.csproj
```

##### To run the consumer:

```bash
dotnet run .\MailClient.Consumer.csproj
```

### Run requirements MongoDB and RabbitMQ on Docker

```bash
docker volume create mongodb
docker run --name mongodb -d -p 27017:27017 -v mongodb:/data/db mongo
docker run --name rabbitmq -d -p 15672:15672 -p 5672:5672 -p 25676:25676 rabbitmq:3-management
```
