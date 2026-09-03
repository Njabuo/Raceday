# RaceDay REST API Endpoint Plan

## 1. Authentication

| HTTP Method | Route              | Description         | Role Required | Request Body                    | Expected Response                     |
| ----------- | ------------------ | ------------------- | ------------- | ------------------------------- | ------------------------------------- |
| POST        | /api/auth/register | Register a new user | Public        | FullName, Email, Password, Role | User account created successfully     |
| POST        | /api/auth/login    | Authenticate a user | Public        | Email, Password                 | Authentication token and user details |

## 2. User Profile

| HTTP Method | Route         | Description                     | Role Required          | Request Body                | Expected Response    |
| ----------- | ------------- | ------------------------------- | ---------------------- | --------------------------- | -------------------- |
| GET         | /api/users/me | View logged-in user's profile   | Any authenticated user | None                        | User profile details |
| PUT         | /api/users/me | Update logged-in user's profile | Any authenticated user | Updated profile information | Updated user profile |

## 3. Events

| HTTP Method | Route            | Description              | Role Required | Request Body                                        | Expected Response                   |
| ----------- | ---------------- | ------------------------ | ------------- | --------------------------------------------------- | ----------------------------------- |
| GET         | /api/events      | View all race events     | Public        | None                                                | List of events                      |
| GET         | /api/events/{id} | View a specific event    | Public        | None                                                | Event details                       |
| POST        | /api/events      | Create a new race event  | Organiser     | EventName, EventDate, Location, Description, Status | Event created successfully          |
| PUT         | /api/events/{id} | Update an existing event | Organiser     | Updated event information                           | Updated event                       |
| DELETE      | /api/events/{id} | Delete an event          | Organiser     | None                                                | Confirmation that event was deleted |

## 4. Categories

| HTTP Method | Route                            | Description                  | Role Required | Request Body                                  | Expected Response                      |
| ----------- | -------------------------------- | ---------------------------- | ------------- | --------------------------------------------- | -------------------------------------- |
| GET         | /api/categories                  | View all race categories     | Public        | None                                          | List of categories                     |
| GET         | /api/events/{eventId}/categories | View categories for an event | Public        | None                                          | List of event categories               |
| POST        | /api/events/{eventId}/categories | Add a category to an event   | Organiser     | CategoryName, DistanceKm, MaximumParticipants | Category created successfully          |
| PUT         | /api/categories/{id}             | Update a race category       | Organiser     | Updated category information                  | Updated category                       |
| DELETE      | /api/categories/{id}             | Delete a race category       | Organiser     | None                                          | Confirmation that category was deleted |

## 5. Event Enrolments

| HTTP Method | Route                            | Description                            | Role Required | Request Body | Expected Response                |
| ----------- | -------------------------------- | -------------------------------------- | ------------- | ------------ | -------------------------------- |
| POST        | /api/enrolments                  | Enrol a participant in a race category | Participant   | CategoryID   | Enrolment created successfully   |
| GET         | /api/enrolments/my               | View my enrolments                     | Participant   | None         | List of participant enrolments   |
| GET         | /api/events/{eventId}/enrolments | View enrolments for an event           | Organiser     | None         | List of event enrolments         |
| DELETE      | /api/enrolments/{id}             | Cancel an enrolment                    | Participant   | None         | Enrolment cancelled successfully |

## 6. Results

| HTTP Method | Route                         | Description                        | Role Required      | Request Body                                    | Expected Response           |
| ----------- | ----------------------------- | ---------------------------------- | ------------------ | ----------------------------------------------- | --------------------------- |
| POST        | /api/results                  | Record a participant's race result | Organiser          | EnrolmentID, FinishTime, Position, ResultStatus | Result created successfully |
| GET         | /api/results/{id}             | View a specific race result        | Authenticated user | None                                            | Result details              |
| GET         | /api/events/{eventId}/results | View results for an event          | Public             | None                                            | List of event results       |

## Role Definitions

### Public

Users do not need to log in.

### Participant

A registered participant who can manage their own profile and race enrolments.

### Organiser

A registered organiser who can create and manage events, categories, enrolments and results.

### Authenticated User

Any logged-in RaceDay user, whether an Organiser or Participant.
