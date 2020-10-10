# PVBPS

**This repository contains possibly dangerous projects!** 

- Do not use without understanding of innerworkings!
- Do not use on devices without owners/users permissions!
- Usage of virtual enviroment is recommended.
- Use at your own risk!

## Projects:

### Keylogger
 - Simple keylogger application
 - Will hide in background
 - Sends keys and clipboard to *Server*

### Keylogger2
 - Extends *Keylogger*
 - Run on startup using registy
 - Request Administrator privileges on first run or if command checks failed 
 - Can run specified commands
 - Will try to disable firewall
 - Will check if firewall is disabled on startup. If not, it will try to disable it.

### Server
 - Blazor WebAssembly Server powered by ASP.NET Core
 - Contains Endpoints for Keyloggers
 - Can show Keylogger Logs and Machines to logged users