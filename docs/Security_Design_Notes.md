# RaceDay Security Design Notes

## Authentication

The RaceDay system will require users to authenticate before accessing protected functionality.

Passwords should be stored securely using password hashing rather than storing plain-text passwords.

## Role-Based Access Control

The system will distinguish between Organisers and Participants.

Organisers will have permission to manage events, categories and race results.

Participants will have permission to manage their own enrolments and profile information.

## Data Protection

User information should only be accessed by authorised users.

The API should validate incoming requests and prevent unauthorised access to protected resources.

## Database Security

The database uses primary keys, foreign keys, UNIQUE constraints and CHECK constraints to maintain data integrity.

Database credentials should not be stored directly in source code or committed to GitHub.

## Future Security Improvements

Part 2 can implement authentication tokens, authorisation middleware, input validation and secure password hashing.
