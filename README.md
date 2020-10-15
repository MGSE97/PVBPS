# PVBPS

Computer viruses and security of computer systems. #DangerousThings

### **This repository contains possibly dangerous projects!** 

- Do not use without understanding of innerworkings!
- Do not use on devices without owners/users permissions!
- Usage of virtual enviroment is recommended.
- Use at your own risk!

## Getting Started

1. Download this repository
2. Open [solution](PVBPS.sln) 
3. Select project
4. Build & Run

### Prerequisites

Software:
* [ASP.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core) - Framework

IDE:
* [Visual Studio 2019](https://visualstudio.microsoft.com/cs/vs/)

## Projects:

Under one solution, written in C#.

### [Keylogger](KeyLogger)
 - Simple keylogger application
 - Will hide in background
 - Sends keys and clipboard to *Server*

### [Keylogger2](KeyLogger2)
 - Extends *Keylogger*
 - Run on startup using registy
 - Request Administrator privileges on first run or if command checks failed 
 - Can run specified commands
 - Will try to disable firewall
 - Will check if firewall is disabled on startup. If not, it will try to disable it.

### [Server](Server/Server)
 - Blazor WebAssembly Server powered by ASP.NET Core
 - Contains Endpoints for Keyloggers
 - Can show Keylogger Logs and Machines to logged users

## Author

* [**MGSE97**](https://github.com/MGSE97)

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.