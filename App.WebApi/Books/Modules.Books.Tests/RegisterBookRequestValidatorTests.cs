using Modules.Books.Features.RegisterBook;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace Modules.Books.Tests
{
    public class RegisterBookRequestValidatorTests
    {
        private readonly RegisterBookRequestValidator _validator;

        public RegisterBookRequestValidatorTests()
        {
            _validator = new RegisterBookRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var model = new RegisterBookRequest(string.Empty, string.Empty, string.Empty,0, string.Empty, string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Title)
                .WithErrorMessage("Title cannot be empty.");
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Too_Short()
        {
            var model = new RegisterBookRequest("AB", string.Empty, string.Empty, 0, string.Empty, string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Title)
                .WithErrorMessage("Title must be at least 3 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Too_Long()
        {
            
            var model = new RegisterBookRequest(new string('A', 101), string.Empty, string.Empty, 0, string.Empty, string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Title)
                .WithErrorMessage("Title cannot exceed 100 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Author_Is_Empty()
        {
            var model = new RegisterBookRequest(new string('A', 100), string.Empty, string.Empty, 0, string.Empty, string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Author)
                .WithErrorMessage("Author name cannot be empty.");
        }

        [Fact]
        public void Should_Have_Error_When_Author_Is_Too_Short()
        {
            var model = new RegisterBookRequest(new string('A', 100), "AB", string.Empty, 0, string.Empty, string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Author)
                .WithErrorMessage("Author name must be at least 3 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_Author_Is_Too_Long()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 101), string.Empty, 0, string.Empty, string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Author)
                .WithErrorMessage("Author name cannot exceed 100 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Publisher_Is_Empty()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 100), string.Empty, 0, string.Empty, string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Publisher)
                .WithErrorMessage("Publisher name cannot be empty.");
        }

        [Fact]
        public void Should_Have_Error_When_Publisher_Is_Too_Short()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 100), string.Empty, 0, "AB", string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Publisher)
                .WithErrorMessage("Publisher name must be at least 3 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_Publisher_Is_Too_Long()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 100), string.Empty, 0, new string('A', 101), string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Publisher)
                .WithErrorMessage("Publisher name cannot exceed 100 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 100), string.Empty, 0, new string('A', 101), string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Description)
                .WithErrorMessage("Description cannot be empty.");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Too_Short()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 100), "AB", 0, new string('A', 101), string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Description)
                .WithErrorMessage("Description must be at least 10 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Too_Long()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 100), new string('A', 4001), 0, new string('A', 101), string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.Description)
                .WithErrorMessage("Description cannot exceed 4000 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_ISBN_Is_Less_Than_Zero()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 100), new string('A', 4001), -1, new string('A', 101), string.Empty, string.Empty, new Guid());
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(book => book.ISBN)
                .WithErrorMessage("ISBN must be a positive number.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var model = new RegisterBookRequest(new string('A', 100), new string('A', 100), new string('A', 4000), 9789577473561, new string('A', 100), "AB", string.Empty, new Guid());

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(book => book.Title);
            result.ShouldNotHaveValidationErrorFor(book => book.Author);
            result.ShouldNotHaveValidationErrorFor(book => book.Publisher);
            result.ShouldNotHaveValidationErrorFor(book => book.Description);
            result.ShouldNotHaveValidationErrorFor(book => book.ISBN);
        }
    }
}
