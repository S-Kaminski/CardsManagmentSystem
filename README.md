# Card Management System

The **Card Management System** is a C# console application designed to manage card details such as credit cards. It provides functionality to store, retrieve, update, and delete card information in a SQL Server database. The system is built using a modular approach, with a library (`CardManagment.dll`) that encapsulates the core logic for card management and database interactions, and a console application (`SystemZarzadzaniaKartami`) that provides a user interface for interacting with the system.

---

## Features

### Core Library (`CardManagment.dll`)
- **Card Management**:
  - Add new cards with unique IDs.
  - Search for cards by owner ID, card serial number, or card ID.
  - Remove cards from the database.
- **Database Integration**:
  - Automatically initializes the database and table if they don't exist.
  - Uses **Dapper** for efficient database operations.
- **Validation**:
  - Validates card PINs (must be 4 numeric digits).
  - Validates input for numeric patterns.

### Console Application (`SystemZarzadzaniaKartami`)
- **User Interface**:
  - Add new cards with owner ID, PIN, and card serial number.
  - Search for cards using owner ID, card serial number, or card ID.
  - Remove cards by card serial number.
- **Input Validation**:
  - Ensures all inputs are valid and meet the required format.

---

## Dependencies

The project relies on the following libraries and frameworks:

- **.NET 8.0**: The application is built using C# and .NET.
- **Dapper**: A lightweight ORM (Object-Relational Mapper) for database operations.
- **Microsoft.Data.SqlClient**: Provides SQL Server database connectivity.

To install the required dependencies, run the following commands:

```bash
dotnet add package Dapper
dotnet add package Microsoft.Data.SqlClient
```

## Code Structure

### 1. `Card` Struct
Represents a card with the following properties:
- `OwnerId`: The ID of the card owner.
- `Pin`: The 4-digit PIN associated with the card.
- `CardSerialNumber`: The serial number of the card.
- `CardId`: A unique 32-character ID generated for the card.

The `Card` struct includes a method `GenerateUniqueCardId` to generate a unique ID for each card.

---

### 2. `CardConnection` Class
Handles database connections and operations. Key methods include:
- `ExecuteQuery`: Executes a SQL query and returns the number of affected rows.
- `SelectQuery`: Executes a SQL query and returns a `Card` object.
- `DevInitDatabase`: Initializes the database and creates the `Cards` table if they don't exist.

---

### 3. `Extensions` Class
Provides utility methods for:
- Converting card details to SQL queries (`NewCardToSqlQuery`).
- Searching for cards (`SearchTermToQuery`).
- Removing cards (`RemoveCardToQuery`).
- Validating PINs (`PinValidation`).
- Validating input (`IsInputValid`).

---

### 4. `SystemManager` Class
Manages the user interface and interaction with the `CardManagment` library. Key features include:
- **Add Card**: Allows the user to add a new card with owner ID, PIN, and card serial number.
- **Search Card**: Allows the user to search for a card using owner ID, card serial number, or card ID.
- **Remove Card**: Allows the user to remove a card by its serial number.

---

## Usage

### Setting Up the Database
1. Ensure SQL Server is installed and running.
2. Update the `CardConnection` constructor with your server name and database name (defaults to `localhost` and `CardsManagement`).
3. Call (uncomment for single use) the `DevInitDatabase` method to initialize the database and create the `Cards` table.
```csharp
CardConnection devInit = new CardConnection("localhost", "CardsManagement"); 
devInit.DevInitDatabase();
```
