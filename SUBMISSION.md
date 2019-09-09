# How to run the API

## Preparation

To run this app you'll need to have installed in your machine the following:

  - [.NetCore SDK 2.1] (or higher)
  - [Docker]

## Build and run the container

The API uses a docker container hosted PostgreSql instance as database. 
Let's get it up and running, shall we?

Inside the root folder of the project run the following commands:

**Build the image (this may take a few seconds)**
  ```sh
  $ docker build -t postgresondocker Docker
  ```
Having a similar result to :
```
Status: Downloaded newer image for postgres:latest
 ---> c3fe76fef0a6
Step 2/9 : COPY 1-Airports.sql /docker-entrypoint-initdb.d/
 ---> e275cb5e405c
Step 3/9 : COPY 2-Airlines.sql /docker-entrypoint-initdb.d/
 ---> 0ccadafb3da5
Step 4/9 : COPY 3-Routes.sql /docker-entrypoint-initdb.d/
 ---> 799cf0cf375a
Step 5/9 : COPY 4-Import.sh /docker-entrypoint-initdb.d/
 ---> 3396848ccdc9
Step 6/9 : COPY airports.csv data/
 ---> 2b532bc9406c
Step 7/9 : COPY airlines.csv data/
 ---> 2de7cf976c73
Step 8/9 : COPY routes.csv data/
 ---> b3bfe0ea52bf
Step 9/9 : RUN chmod +x /docker-entrypoint-initdb.d/*
 ---> Running in 37d79d9feb39
Removing intermediate container 37d79d9feb39
 ---> 9636ddaacf43
Successfully built 9636ddaacf43
Successfully tagged postgresondocker:latest
```
  
Obs.: You can skip this step next time you wish to run the container
  
**Run the container (Be sure to have port 5432 available)**
  ```sh
  $ docker run --rm --name postgresondocker -p 5432:5432 -e POSTGRES_PASSWORD=postgresondocker -d postgresondocker
  ```

Having a similar result to:

```
28da70fcde6bed7e304835b53ff4541a84c0e82c6c65d9c49eec85e7e5e4b705
```

You can confirm if your container is running with the command `docker ps`. Look for the row with **postgresondocker** in **NAMES** column.

## Build, Test and Run the Api
Once our database is set we can run run the api (build and tests don't require the database):

Still on the project's root folder, run the following commands:

**Build the api**

```sh
$ dotnet build
```
Having the similar result:
```sh
0 Error(s)

Time Elapsed 00:00:12.57
```

**Run all tests**
```sh
$ dotnet test
```
Having the similar result:
```sh
Starting test execution, please wait...

Total tests: 7. Passed: 7. Failed: 0. Skipped: 0.
Test Run Successful.
Test execution time: 3.2943 Seconds

Total tests: 16. Passed: 16. Failed: 0. Skipped: 0.
Test Run Successful.
Test execution time: 3.4381 Seconds

Total tests: 4. Passed: 4. Failed: 0. Skipped: 0.
Test Run Successful.
Test execution time: 3.3092 Seconds
```

**Run the api**

```sh
$ dotnet run --project TakeHome.Web.Api
```

Having the result:

```sh
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

## Usage

To use the api, simply hit the endpoint 
```
http://localhost:5000/api/takehome/get?origin=&destination=
```

providing an Iata3 code for origin and destination parameters as the following example:

```
http://localhost:5000/api/takehome/get?origin=atl&destination=dub
```

Having the content result:

```
"ATL -> YYZ -> DUB"
```

or 
```
"No Route"
```
when no route could be found.

Also, in case of invalid parameters:
```
"Invalid Origin"
```
or
```
"Invalid Destination"
```

Finally, if an unexpected error happens, you shall receive the following message:

```
Something wrong happened, please try again in a few minutes.
```

# Plan B

If for some reason you run into a snag when following the previous steps, you can still use a production version of the API deployed in Azure.

Just hit the follwing url using the steps described in the **Usage** section

```
https://takehomewebapi20190909121404.azurewebsites.net/api/takehome/get?origin=&destination=
```

**Good luck and have fun!**


   [.NetCore SDK 2.1]: <https://dotnet.microsoft.com/download/dotnet-core/2.1>
   [Docker]: <https://www.docker.com/get-started>
   