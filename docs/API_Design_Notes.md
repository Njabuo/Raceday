# RaceDay API Design Notes

## Purpose

The RaceDay REST API will provide access to the system's authentication, user profiles, events, categories, enrolments and results.

## Authentication

Users will be able to register and log in. Authentication will determine whether a user is an Organiser or Participant.

## User Profiles

Authenticated users will be able to view and update their own profile information.

## Events and Categories

Organisers will be responsible for creating, updating and deleting events and race categories.

Users will be able to view available events and categories.

## Enrolments

Participants will be able to enrol in race categories, view their own enrolments and cancel an enrolment where permitted.

## Results

Organisers will record race results. Authenticated users will be able to view results, while event results can be made publicly available.

## Role-Based Access

The API will use role-based access control:

- Public users can view available events and categories.
- Participants can manage their own enrolments.
- Organisers can manage events, categories and results.

The API implementation in Part 2 will follow the endpoint plan provided in `API_Endpoint_Plan.md`.
