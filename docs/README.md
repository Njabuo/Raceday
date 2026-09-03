# RaceDay System – Part 1

## Project Overview

RaceDay is a race event management system designed to manage organisers, participants, events, race categories, enrolments and race results.

Part 1 focuses on system planning and database design. The project includes a relational database, Entity Relationship Diagram (ERD), REST API endpoint plan and GitHub Actions validation.

## Part 1 Documentation

The following documents are included in this repository:

- **RaceDay ERD** – Entity Relationship Diagram showing the system entities, attributes, relationships, primary keys and foreign keys.
- **API Endpoint Plan** – Planned RESTful API endpoints, HTTP methods, descriptions, roles, request bodies and expected responses.
- **RaceDay Database SQL Script** – SQL Server script used to create and populate the RaceDay database.

## Database Entities

The RaceDay database contains the following entities:

1. Users
2. Organisers
3. Participants
4. Events
5. Categories
6. Enrolments
7. Results

## Technologies Used

- Microsoft SQL Server
- SQL Server Management Studio (SSMS)
- GitHub
- GitHub Actions
- REST API planning
- Relational database design

## GitHub Actions

GitHub Actions is used to automatically validate that the required Part 1 documentation files are present in the `/docs` folder.

The validation workflow checks for:

- `RaceDay_ERD.png`
- `API_Endpoint_Plan.md`
- `RaceDay_Database.sql`

The validation workflow completed successfully.

<img width="1215" height="1518" alt="Screenshot_3-9-2026_204030_github com" src="https://github.com/user-attachments/assets/6f6caada-5307-4895-a557-ffa5103fa84b" />


## Project Structure

```text
Raceday/
│
├── .github/
│   └── workflows/
│       └── validate-docs.yml
│
└── docs/
    ├── RaceDay_ERD.png
    ├── API_Endpoint_Plan.md
    ├── RaceDay_Database.sql
    └── README.md
