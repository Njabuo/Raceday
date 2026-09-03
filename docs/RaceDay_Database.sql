CREATE DATABASE RaceDay;
GO

USE RaceDay;
GO

-- 1. USERS TABLE
CREATE TABLE Users
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT CK_Users_Role
        CHECK (Role IN ('Organiser', 'Participant'))
);
GO

-- 2. ORGANISERS TABLE
CREATE TABLE Organisers
(
    OrganiserID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    OrganisationName VARCHAR(150) NOT NULL,
    ContactNumber VARCHAR(20) NOT NULL,

    CONSTRAINT FK_Organisers_Users
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- 3. PARTICIPANTS TABLE
CREATE TABLE Participants
(
    ParticipantID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    DateOfBirth DATE NOT NULL,
    Gender VARCHAR(20) NOT NULL,
    EmergencyContact VARCHAR(100) NOT NULL,
    EmergencyPhone VARCHAR(20) NOT NULL,

    CONSTRAINT FK_Participants_Users
        FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- 4. EVENTS TABLE
CREATE TABLE Events
(
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    EventName VARCHAR(150) NOT NULL,
    EventDate DATE NOT NULL,
    Location VARCHAR(200) NOT NULL,
    Description VARCHAR(500) NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Upcoming',

    CONSTRAINT FK_Events_Organisers
        FOREIGN KEY (OrganiserID) REFERENCES Organisers(OrganiserID),

    CONSTRAINT CK_Events_Status
        CHECK (Status IN ('Upcoming', 'Open', 'Completed', 'Cancelled'))
);
GO

-- 5. CATEGORIES TABLE
CREATE TABLE Categories
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName VARCHAR(100) NOT NULL,
    DistanceKm DECIMAL(5,2) NOT NULL,
    MaximumParticipants INT NOT NULL,

    CONSTRAINT FK_Categories_Events
        FOREIGN KEY (EventID) REFERENCES Events(EventID),

    CONSTRAINT CK_Categories_Distance
        CHECK (DistanceKm > 0),

    CONSTRAINT CK_Categories_MaxParticipants
        CHECK (MaximumParticipants > 0),

    CONSTRAINT UQ_Categories_Event_Category
        UNIQUE (EventID, CategoryName)
);
GO

-- 6. ENROLMENTS TABLE
CREATE TABLE Enrolments
(
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATE NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    RaceNumber VARCHAR(20) NOT NULL UNIQUE,

    CONSTRAINT FK_Enrolments_Participants
        FOREIGN KEY (ParticipantID) REFERENCES Participants(ParticipantID),

    CONSTRAINT FK_Enrolments_Categories
        FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),

    CONSTRAINT CK_Enrolments_Status
        CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),

    CONSTRAINT UQ_Enrolments_Participant_Category
        UNIQUE (ParticipantID, CategoryID)
);
GO

-- 7. RESULTS TABLE
CREATE TABLE Results
(
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    FinishTime TIME NOT NULL,
    Position INT NOT NULL,
    ResultStatus VARCHAR(20) NOT NULL DEFAULT 'Finished',

    CONSTRAINT FK_Results_Enrolments
        FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID),

    CONSTRAINT CK_Results_Position
        CHECK (Position > 0),

    CONSTRAINT CK_Results_Status
        CHECK (ResultStatus IN ('Finished', 'Did Not Finish', 'Disqualified'))
);
GO