# LoadBalancer

LoadBalancer is a server load balancing tool that distributes incoming network traffic across multiple backend servers using a round-robin algorithm. It includes features like health checking and connection pooling.

## Table of Contents

- [Introduction](#introduction)
- [Installation](#installation)
- [Usage](#usage)
- [Functionality](#functionality)
- [Learning Outcomes](#learningoutcomes)
- [Contributing](#contributing)


## Introduction

**LoadBalancer** is a lightweight load balancing tool written in C# that distributes incoming client requests across multiple backend servers using a round-robin algorithm. It includes features such as health checks for backend servers and connection pooling to improve performance.

## Installation

To use LoadBalancer, you need to have the .NET runtime installed on your machine. You can download it from the official .NET website.

Once you have .NET installed, you can clone this repository or download the source code as a ZIP file and extract it to your desired location.

## Usage

LoadBalancer is a command-line tool, and it accepts two arguments: the health check period in seconds and the health check URL. Here's how you can run it:

```bash
dotnet run -- <healthCheckPeriodInSeconds> <healthCheckUrl>
```

Replace <healthCheckPeriodInSeconds> with the interval for health checks (in seconds) and <healthCheckUrl> with the URL path for health checks on the backend servers.

## Functionality

LoadBalancer provides the following functionality:

* __Round-Robin Load Balancing:__  Distributes incoming requests evenly across multiple backend servers.

* __Health Checking:__  Periodically checks the health of backend servers and updates the list of available servers accordingly.

* __Connection Pooling:__ Reuses existing TCP connections to backend servers to improve performance.

* __Connection Cleanup:__  Regularly cleans up idle or closed connections to maintain efficient resource usage.

## Learning Outcomes

By working with LoadBalancer's code, users can expect to gain the following knowledge and skills:

- Understanding of load balancing algorithms and their application in distributing network traffic.
- Proficiency in implementing round-robin load balancing and health check mechanisms.
- Familiarity with TCP connection management and pooling in C#.
- Ability to ensure high availability and reliability of backend servers through periodic health checks.
- Experience with developing and maintaining server-side tools for efficient traffic management.

## Contributing

Contributions are welcome! If you find any bugs, have feature requests, or want to contribute code, please open an issue or submit a pull request on the GitHub repository.
Feel free to modify any sections as needed.
