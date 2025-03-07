# MultiPlayerLobbyGame

MultiPlayerLobbyGame is Lobby service for online games.
### Features
  - Register User
  - Join Lobbies
  - Can be Clustered
### Used tools
  - C#/NET
  - ASP.NET
  - Redis
  - Docker

## Prerequisites

Before running the application, ensure you have the following installed:

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## How to Run

Follow these steps to set up and run the application:

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/harpm/MultiPlayerLobbyGame.git
   cd Src
   ```
2.  **Build and Run the Application**:
Use Docker Compose to build and run the application along with the Redis instance:
  ```bash
  docker-compose up --build
  ```
This will:
  - Build the MultiPlayerLobbyGame.API Docker image.
  - Start a Redis container.
  - Start 3 instances of the MultiPlayerLobbyGame.API application, all connected to the same Redis instance.

3. **Scaling the Application**:
  If you want to run more instances of the application, you can scale the service using the --scale flag:
  ```bash
  docker-compose up --build --scale multiplayerlobbygame-api=5
  ```
4. **Access the Application**:
   Once the containers are running, you can access the application at:
   ```bash
   http://localhost:80
   ```
   (Replace 80 with the port number if you've customized it in the docker-compose.yml file.)
5. **Stopping the Application**:
   To stop the application and all running containers, use:
   ```bash
   docker-compose down
   ```
## Configuration
  - **Redis Connection**: The application connects to Redis using the RedisConnection environment variable, which is set to redis:6379 in the docker-compose.yml file.
  - **Environment Variables**: You can modify the environment variables in the docker-compose.yml file to customize the application behavior.
