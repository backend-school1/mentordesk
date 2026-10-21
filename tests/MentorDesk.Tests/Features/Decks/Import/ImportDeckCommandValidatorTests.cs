using FluentAssertions;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Features.Decks.Import;
using MentorDesk.Domain.Entities;
using Moq;

namespace MentorDesk.Tests.Features.Decks.Import;

public class ImportDeckCommandValidatorTests
{
    private readonly ImportDeckCommandValidator _validator;

    public ImportDeckCommandValidatorTests()
    {
        _validator = new ImportDeckCommandValidator();
    }

    [Fact]
    public async Task Validate_WhenDeckIdIsEmpty_ShouldReturnValidationError()
    {
        var command = new ImportDeckCommand(Guid.Empty, new MemoryStream());

        var resultValidate = await _validator.ValidateAsync(command);

        resultValidate.Errors.Should().ContainSingle(
            error => error.ErrorMessage == "ID колоды не может быть пустым");
    }

    [Fact]
    public async Task Validate_WhenCommandIsValid_ShouldReturnTrue()
    {
        var command = new ImportDeckCommand(Guid.NewGuid(), new MemoryStream());

        var resultValidate = await _validator.ValidateAsync(command);

        resultValidate.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WhenCsvIsNull_ShouldReturnValidationError()
    {
        var command = new ImportDeckCommand(Guid.NewGuid(), null);

        var resultValidate = await _validator.ValidateAsync(command);

        resultValidate.Errors.Should().ContainSingle(
            error => error.ErrorMessage == "CSV файл не может быть пустым");
    }
}
