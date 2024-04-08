
# Rihal Cinema 

This is a soluation for Rihal CodeStacker Compeition for the year 2024.
## Technologies Used
- ASP.NET Core 6 For Building Apis
- Postgresql For Storing Data
- MinIO For Storing Photos
- Basic Authentication
- Docker Engine to encapsulate technologies together.

## Installation

Install Rihal Cinema Application with Docker
- First, Make Sure That Docker Engine Is Running
- Then, Inside The Project The Root Path, Run this Command
```bash
docker-compose up --build
```
- The Command Will Install All Technologies

## Access To APIs
- After Running The Project in Docker Successfully, Visit This Url To Test All Apis in Swagger
```bash
http://localhost:8088/swagger/index.html
```
- To test the APIs, initial registration is necessary. An API is available for registering as a new user. Following registration, users can log in by clicking the "Authorize" button located at the top right of the page. Subsequently, all APIs will become accessible.

- All APIs mentioned in the Rihal CodeStacker Competition documentation have been implemented, with the exception of the chat system.
## Support

For support, Email qalnaabi00@gmail.com

