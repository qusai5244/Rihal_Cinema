
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
- Installation might take some time to Finish

## Access To APIs
- After Running The Project in Docker Successfully, Visit This Url To Test All Apis in Swagger
```bash
http://localhost:8088/swagger/index.html
```
- To test the APIs, initial registration is necessary. An API is available for registering as a new user. Following registration, users can log in by clicking the "Authorize" button located at the top right of the page. Subsequently, all APIs will become accessible.

- All APIs mentioned in the Rihal CodeStacker Competition documentation have been implemented, with the exception of the chat system.
## API Reference

### Auth
#### Registration
```http
  POST /api/auth/Register
```
| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Email` | `string` | **Required**. Must Be in Email Format And Not User Before  |
| `Password` | `string` | **Required**.   |


### Movie
#### Get all Movies

```http
  GET /api/Movie
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Page` | `int` | **Not Required**. Page Number For Pagination, Defult Value is **1** |
| `PageSize` | `int` | **Not Required**. Page Size For Pagination, Defult Value is **50** |


#### Get Movie

```http
  GET /api/Movie/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `int` | **Required**. Id of movie to fetch |

#### Rate Movie

```http
  Post /api/Movie/rate
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `movieId`      | `int` | **Required**. Id of movie to rate |
| `rate`      | `int` | **Required**. Move Rating |
- Each user is permitted to assign only one rating to each movie.
    - For instance, if a user rates a movie with a 6 and subsequently rates it again, the latest rating will overwrite the previous one.

#### Search in Movies

```http
  GET /api/Movie/search
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `searchInput ` | `string` | **Required**. Search Input |
| `Page` | `int` | **Not Required**. Page Number For Pagination, Defult Value is **1** |
| `PageSize` | `int` | **Not Required**. Page Size For Pagination, Defult Value is **50** |

#### Get My Top 5 Rated Movies

```http
  GET /api/Movie/top5RatedMovies
```
#### Guess Movie

```http
  GET /api/Movie/guessMovie
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `scrambledName`      | `String` | **Required**. Scrambled Name For Search  |

#### Compare Movies Ratings 

```http
  GET /api/Movie/RatingsCompare
```

#### Star System

```http
  GET /api/Movie/StarSystem
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `MoviesIds`      | `List<int>` | **Required**. List Of Movies Ids  |


### Memory

#### Create Memory

```http
  Post /api/memory/create
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `MovieId` | `int` | **Required**.|
| `Title` | `string` | **Required**. |
| `TakenOn` | `Date` | **Not Required**. should Be in this Format **(yy-mm-dd)**|
| `Story` | `string` | **Not Required**. |
| `Image` | `File` | **Not Required**. The uploaded file must be a valid image format, and no other file types will be accepted.|

#### Get all Memories

```http
  GET /api/memory
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `Page` | `int` | **Not Required**. Page Number For Pagination, Defult Value is **1** |
| `PageSize` | `int` | **Not Required**. Page Size For Pagination, Defult Value is **50** |


#### Get Memory

```http
  GET /api/Memory/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `int` | **Required**. Id of Memory to fetch |

#### Delete Memory

```http
  DELETE /api/Memory/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `int` | **Required**. Id of Memory to Delete |

#### Update Memory

```http
  PUT /api/Memory/Update
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `int` | **Required**. Id of Memory to Update |
| `title`      | `string` | **Not Required**. |
| `story`      | `string` | **Not Required**. |

#### Extract Urls Links From Memory Story

```http
  GET /api/Memory/GetUrlLinks/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `int` | **Required**.|

#### Get Top Five Used Words In Memories Stories

```http
  GET /api/Memory/GetUrlLinks
```

#### Get Memory Photo 

```http
  GET /api/Memory/Photo/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `int` | **Required**. Photo Id|

#### Delete Memory Photo 

```http
  Delete /api/Memory/Photo/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `id`      | `int` | **Required**. Photo Id|

#### Add Memory Photo 

```http
  Post /api/Memory/Photo
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `MemoryId`      | `int` | **Required**. Memory Id|
| `Image` | `File` | **Required**. The uploaded file must be a valid image format, and no other file types will be accepted.|



## Support

For support, Email qalnaabi00@gmail.com

