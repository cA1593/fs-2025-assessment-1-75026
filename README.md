**1. Introduction**

This project implements a DublinBikes Web API using .NET 8, following the specifications outlined in the assignment brief. The API provides two full versions:
- API V1 – uses local JSON file as the data source
- API V2 – uses Azure CosmosDB as the data source
Both API versions expose endpoints for retrieving Dublin Bikes station information, including filtering, searching, sorting, and pagination.
The project also includes:
-Caching
-Background data updates
-Swagger documentation
-Unit tests
-Postman test collection
Separation of concerns through models, services, and endpoint groups

**2. Project Structure**

fs-2025-assessment-1-75026
  - Program.cs
  - README.md
  - Data(folder)
      -  dublinbike.json
  - Models(folder)
      -  Station.cs
  - Services(folder)
      -  StationService.cs
      -  CosmosStationService.cs
      -  StationUpdateBackgroundService.cs
  - Endpoints(folder)
      -  StationEndPoints.cs (V1)
      -  StationV2EndPoints.cs (V2)
  - Tests(folder)
      - StationFilterTests.cs
      - StationEndpointTests.cs
  - Postman(folder)
      -  DublinBikesAPI.postman_collection.json


**3. Mapping Assignment Requirements to Project Implementation**
The following section maps each requirement from the assignment document to the exact file and location in the project where it is implemented.

SECTION 1 — Load JSON at Startup (V1)
  
| Requirement                       | Implementation                   | Location                                                |
| --------------------------------- | -------------------------------- | ------------------------------------------------------- |
| Load `dublinbike.json` on startup | File read + JSON deserialization | `Services/StationService.cs` → `LoadStationsFromJson()` |
| Provide all stations              | In-memory station list           | `StationService.GetAllStations()`                       |
| Provide single station by number  | LINQ query                       | `StationService.GetStationByNumber()`                   |


SECTION 2 — API Version V1 (JSON Backed)

| Requirement                        | Implementation                                         | Location                        |
| ---------------------------------- | ------------------------------------------------------ | ------------------------------- |
| Endpoints under `/api/v1/stations` | Endpoint group with filtering/searching/sorting/paging | `Endpoints/StationEndPoints.cs` |
| GET all stations                   | `MapGet("/")`                                          | Same file                       |
| GET by number                      | `MapGet("/{number}")`                                  | Same file                       |
| GET summary                        | Aggregation logic                                      | Same file                       |
| POST / PUT (placeholder)           | Implemented but without actual persistence             | Same file                       |


SECTION 3 — API Version V2 (CosmosDB Backed)

| Requirement        | Implementation                                                         | Location                           |
| ------------------ | ---------------------------------------------------------------------- | ---------------------------------- |
| Read from CosmosDB | Iterator + LINQ                                                        | `Services/CosmosStationService.cs` |
| GET all (CosmosDB) | `MapGet("/")`                                                          | `Endpoints/StationV2EndPoints.cs`  |
| GET by number      | `MapGet("/{number}")`                                                  | Same file                          |
| Summary            | Grouping logic via LINQ                                                | Same file                          |
| POST / PUT         | Implemented but CosmosDB partition key issues prevented full insertion | Same file (noted in README)        |


SECTION 4 — Caching

| Requirement                 | Implementation                                                  | Location                     |
| --------------------------- | --------------------------------------------------------------- | ---------------------------- |
| Cache results for 5 minutes | `GetAllStations()` and `GetStationByNumber()` with memory cache | `Services/StationService.cs` |
| Cache configuration         | `builder.Services.AddMemoryCache()`                             | `Program.cs`                 |


SECTION 5 — Background Service

| Requirement                           | Implementation                                                         | Location                                     |
| ------------------------------------- | ---------------------------------------------------------------------- | -------------------------------------------- |
| Random update every 15 sec            | `StationUpdateBackgroundService`                                       | `Services/StationUpdateBackgroundService.cs` |
| Updates bikes + last update timestamp | Inside `UpdateStations()`                                              | Same file                                    |
| Registered as hosted service          | `builder.Services.AddHostedService<StationUpdateBackgroundService>();` | `Program.cs`                                 |


SECTION 6 — Tests & Documentation

| Requirement                     | Status        | Location                                          |
| ------------------------------- | ------------- | ------------------------------------------------- |
| Unit test (filtering/searching) | ✔ Implemented | `Tests/StationFilterTests.cs`                     |
| Endpoint test                   | ✔ Implemented | `Tests/StationEndpointTests.cs`                   |
| README                          | ✔ Implemented | `README.md` (this document)                       |
| POSTMAN test collection         | ✔ Implemented | `/Postman/DublinBikesAPI.postman_collection.json` |


******************Additional Notes********************:

  -  POST/PUT (CosmosDB) encountered partition key mismatch errors (expected /contract_name) -> documented clearly as required.
  -  This does not block V2 GET endpoints from functioning correctly.


**4. API Endpoints Overview**

V1 — JSON Backed
Base URL: /api/v1/stations

| Method | Endpoint                    | Description                           |
| ------ | --------------------------- | ------------------------------------- |
| GET    | `/api/v1/stations`          | Filtering, searching, sorting, paging |
| GET    | `/api/v1/stations/{number}` | Retrieve station by number            |
| GET    | `/api/v1/stations/summary`  | Aggregated statistics                 |
| POST   | `/api/v1/stations`          | Create (non-persistent placeholder)   |
| PUT    | `/api/v1/stations/{number}` | Update (non-persistent placeholder)   |


V2 — CosmosDB Backed
Base URL: /api/v2/stations

| Method | Endpoint                    | Description                        |
| ------ | --------------------------- | ---------------------------------- |
| GET    | `/api/v2/stations`          | Read all stations from CosmosDB    |
| GET    | `/api/v2/stations/{number}` | Query CosmosDB for a station       |
| GET    | `/api/v2/stations/summary`  | Aggregated statistics from Cosmos  |
| POST   | `/api/v2/stations`          | Insert (attempt; documented issue) |
| PUT    | `/api/v2/stations/{number}` | Update (attempt; documented issue) |


**5. Background Updates**

Every 15 seconds, a hosted background service:
  - Randomizes available bikes
  - Recalculates available stands
  - Updates last_update timestamp
  - Logs changes to console
  - 
File: Services/StationUpdateBackgroundService.cs

This simulates live bike availability changes.

**6. Running the Project**

  1.  Open the solution in Visual Studio 2022
  2.  Set project as Startup Project
  3.  Run using Ctrl+F5
  4.  Swagger UI automatically loads at:
    -  https://localhost:7259/swagger

**7. Running the Tests**
From Test Explorer:
  -  Test > Run All Tests

Tests included:
  -  Filtering/searching unit tests
  -  Endpoint test using WebApplicationFactory

**8. Postman Collection**

Import the file:
  -  Postman/DublinBikesAPI.postman_collection.json

Includes:
  -  All V1 + V2 endpoints
  -  Filtering tests
  -  Summary tests
  -  POST/PUT bodies

**9. Known Limitations**

CosmosDB POST/PUT
Due to partition key mismatch:
  -  PartitionKey extracted from document doesn't match the one specified
Insert/Upsert operations were partially implemented but could not be completed.
GET operations work correctly and satisfy the assignment’s core requirements.


**10. Conclusion**

All primary requirements of the Dublin Bikes API assignment have been implemented, including:
  -  V1 JSON-backed API
  -  V2 CosmosDB-backed API
  -  Filtering, searching, sorting, paging
  -  Background updates
  -  Caching
  -  Unit tests
  -  Endpoint test
  -  Postman test collection
