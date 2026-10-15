
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using RaceDay.API.Controllers;
using RaceDay.API.DTOs;
using Xunit;

namespace RaceDay.API.Tests;

public class UnitTest1
{
    [Fact]
    public void RegisterRequest_WithMissingDetails_IsInvalid()
    {
        // Arrange
        var request = new RegisterRequest();
        var errors = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            errors,
            validateAllProperties: true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void RegisterRequest_WithValidDetails_IsValid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FullName = "Test Participant",
            Email = "participant@example.com",
            Password = "SecurePassword123!",
            DateOfBirth = new DateOnly(2000, 1, 1),
            Gender = "Other",
            EmergencyContact = "Emergency Contact",
            EmergencyPhone = "0712345678"
        };

        var errors = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            errors,
            validateAllProperties: true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(errors);
    }

    [Fact]
    public void CreateEvent_RequiresOrganiserRole()
    {
        // Arrange
        var method = typeof(EventsController).GetMethod(
            nameof(EventsController.CreateEvent));

        // Act
        var authorization = method?.GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        Assert.NotNull(authorization);
        Assert.Equal("Organiser", authorization.Roles);
    }

    [Fact]
    public void CreateEnrolment_RequiresParticipantRole()
    {
        // Arrange
        var method = typeof(EnrolmentsController).GetMethod(
            nameof(EnrolmentsController.CreateEnrolment));

        // Act
        var authorization = method?.GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        Assert.NotNull(authorization);
        Assert.Equal("Participant", authorization.Roles);
    }

    [Fact]
    public void CreateResult_RequiresOrganiserRole()
    {
        // Arrange
        var method = typeof(ResultsController).GetMethod(
            nameof(ResultsController.CreateResult));

        // Act
        var authorization = method?.GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        Assert.NotNull(authorization);
        Assert.Equal("Organiser", authorization.Roles);
    }
}
