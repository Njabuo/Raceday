# RaceDay Testing Notes

## Database Testing

The RaceDay database was created and tested using SQL Server Management Studio.

The following records were inserted successfully:

- 4 Users
- 2 Organisers
- 2 Participants
- 3 Events
- 7 Categories
- 4 Enrolments
- 2 Results

## Relationship Testing

Foreign key relationships were created between the related entities to maintain referential integrity.

## Constraint Testing

The database includes:

- Primary key constraints
- Foreign key constraints
- UNIQUE constraints
- NOT NULL constraints
- DEFAULT values
- CHECK constraints

## GitHub Actions Testing

GitHub Actions was configured to validate the required Part 1 documentation.

The validation workflow successfully completed after correcting the ERD filename and capitalization.

## Future API Testing

The REST API endpoints will be tested during Part 2 using appropriate requests and expected responses.
