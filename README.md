# README #

Externalize Configurable Project

### The technologie implemented ###

* .Net Core 3.1
* Blazor SPA
* Micro Service
* Docker
* Rabbitmq
* MongoDb
* Microsoft DI IOC
* Im-Memory Cache

### Small Help ###

This project has 2 user interface
### 1) Ora.API: ###
  Web API that is Connected to Ora.Services.Configurable service by Command pattern and Rabbitmq.

### 2) Ora.Web: ###
  Blazor SPA that connected to Ora.Services.Configurable service by HttpClientOra.Common.Commands assembly is base on micro-service architect that you can call in each project, In this project, we have Repository pattern for transaction data in Mongo DB that you could find it in Repository Folder, and Repository is related to Controller by Services that they implemented in Service Folder.
* In Task folder in the following project I implemented a scheduled task that runs in period times that you found define interval time in appseting.json.
* Ora.Common and Ora.Common.Commands: are a common project with classes that help you in each project.
* The project is run by docker and docker-compose file.

### Contribution guidelines ###

* Writing tests
* Code review
* Other guidelines

### Who do I talk to? ###

* Repo owner or admin
* Other community or team contact
