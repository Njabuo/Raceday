USE RaceDay;
GO

-- Check the number of records in each table
SELECT COUNT(*) AS UserCount FROM Users;
SELECT COUNT(*) AS OrganiserCount FROM Organisers;
SELECT COUNT(*) AS ParticipantCount FROM Participants;
SELECT COUNT(*) AS EventCount FROM Events;
SELECT COUNT(*) AS CategoryCount FROM Categories;
SELECT COUNT(*) AS EnrolmentCount FROM Enrolments;
SELECT COUNT(*) AS ResultCount FROM Results;
GO

-- Display events and their organisers
SELECT
    E.EventID,
    E.EventName,
    O.OrganisationName
FROM Events E
INNER JOIN Organisers O
    ON E.OrganiserID = O.OrganiserID;
GO

-- Display participants and their enrolments
SELECT
    P.ParticipantID,
    U.FullName,
    E.RaceNumber,
    E.Status
FROM Participants P
INNER JOIN Users U
    ON P.UserID = U.UserID
INNER JOIN Enrolments E
    ON P.ParticipantID = E.ParticipantID;
GO

-- Display race results
SELECT
    R.ResultID,
    E.RaceNumber,
    R.FinishTime,
    R.Position,
    R.ResultStatus
FROM Results R
INNER JOIN Enrolments E
    ON R.EnrolmentID = E.EnrolmentID;
GO
