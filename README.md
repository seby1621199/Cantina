# Canteen

## Description
This project originated from the idea of solving a problem I encountered during my time as a student, specifically the management of orders at the student canteen. This project consists of four main components:

- **The database**
- **The web server for the backend**
- **The client application in React**
- **The control panel for the canteen staff**

## Database
The database is relational, using SQL Server, and I used RDS for hosting in the cloud. For better communication between the server and the database, we used Entity Framework and repositories.

<img src="https://github.com/user-attachments/assets/5b24cb14-119d-4f06-96ff-1858e34b7040" alt="Database description" width="500">

## Web Server Architecture
For the web server, I used ASP.NET and a three-layer architecture:
- **API** | Controllers
- **Business logic** | Services
- **Data access** | Repositories
  
Communication with the server is handled through APIs and SignalR web sockets.

## Client Application
The client application was created using the React library. It includes several web pages accessible to the user based on their roles. Roles are obtained from a JWT stored in local memory.

<img src="https://github.com/user-attachments/assets/1f6db089-ff23-4dd3-bc00-4a55ecb4918a" alt="Login" width="600">
<img src="https://github.com/user-attachments/assets/42dd10ae-cb8c-484c-9ff3-af255ba872e7" alt="Order" width="600">
<img src="https://github.com/user-attachments/assets/6c9eb4eb-5901-4865-825a-040b66ddd209" alt="Menu" width="600">
<img src="https://github.com/user-attachments/assets/aa0b2744-7aa1-4562-94e7-47d3a552fcbe" alt="Orders" width="600">
<img src="https://github.com/user-attachments/assets/dbe1808c-6db1-4278-b2cb-d8769b34546f" alt="Orders Modal" width="600">
<img src="https://github.com/user-attachments/assets/c910506b-d7cc-4f03-a58d-72239060e571" alt="Cart" width="600">
<img src="https://github.com/user-attachments/assets/d5e58bf5-5295-4758-83f0-3f0cf226404c" alt="Delivery" width="600">

## Control Panel
The control panel was created using the WPF framework and custom user controls for managing canteen orders and deliveries. I used the singleton pattern for windows and pages, ensuring that only one instance of each can exist.

<img src="https://github.com/user-attachments/assets/2f15151d-fe94-43b1-a688-9df65e6a44a5" alt="Control Panel Image 1">
<br>
<img src="https://github.com/user-attachments/assets/2cdd8b98-fe23-492d-a7ef-efed045f9ab3" alt="Control Panel Image 2" width="600">
<img src="https://github.com/user-attachments/assets/c5c79f13-f71c-4881-a464-d323326c526f" alt="Control Panel Image 4" width="600">
<img src="https://github.com/user-attachments/assets/197c5fc0-c993-473d-b1fe-d46e33552db7" alt="Control Panel Image 3" width="600">
<br>
<img src="https://github.com/user-attachments/assets/254357a8-0087-42f8-8264-ef32baf945a1" alt="Control Panel Image 5">





