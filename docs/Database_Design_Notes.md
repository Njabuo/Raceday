# RaceDay Database Design Notes

## Purpose

The RaceDay database is designed to store and manage information about race organisers, participants, events, race categories, enrolments and race results.

## Main Relationships

- A User can have one Organiser profile.
- A User can have one Participant profile.
- An Organiser can manage many Events.
- An Event can have many Categories.
- A Participant can have many Enrolments.
- A Category can have many Enrolments.
- An Enrolment can have one Result.

## Database Design Principles

The database uses primary keys to uniquely identify records and foreign keys to maintain relationships between related tables.

NOT NULL constraints are used for required information, while UNIQUE constraints prevent duplicate values where necessary.

The database also uses default values and CHECK constraints to maintain data integrity.

## Technology

The database is implemented using Microsoft SQL Server and was tested using SQL Server Management Studio.
