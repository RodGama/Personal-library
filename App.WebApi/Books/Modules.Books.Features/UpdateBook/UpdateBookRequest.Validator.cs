using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Features.UpdateBook
{
    public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
    {
        public UpdateBookRequestValidator() 
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(book=>book.Title).MinimumLength(3).WithMessage("Title must be at least 3 characters long.")
                                     .NotEmpty().WithMessage("Title cannot be empty.")
                                     .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(book => book.Author).MinimumLength(3).WithMessage("Author name must be at least 3 characters long.")
                                     .NotEmpty().WithMessage("Author name cannot be empty.")
                                     .MaximumLength(100).WithMessage("Author name cannot exceed 100 characters.");

            RuleFor(book => book.Publisher).MinimumLength(3).WithMessage("Publisher name must be at least 3 characters long.")
                                     .NotEmpty().WithMessage("Publisher name cannot be empty.")
                                     .MaximumLength(100).WithMessage("Publisher name cannot exceed 100 characters.");

            RuleFor(book => book.Description).MinimumLength(10).WithMessage("Description must be at least 10 characters long.")
                                     .NotEmpty().WithMessage("Description cannot be empty.")
                                     .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters.");

            RuleFor(book => book.ISBN).NotNull().WithMessage("ISBN cannot be empty.")
                                    .GreaterThan(0).WithMessage("ISBN must be a positive number.")
                                    .Must(IsValidISBN).WithMessage("ISBN not valid");

        }

        private bool IsValidISBN(long arg)
        {
            if (arg.ToString().Length == 10)
                return ISBN10(arg.ToString());
            else if (arg.ToString().Length == 13)
                return ISBN13(arg.ToString());
            return false;
        }

        private bool ISBN10(string isbn)
        {
            if (String.IsNullOrEmpty(isbn) ||
                isbn.Contains("-") ||
                isbn.Contains(" ") ||
                isbn.Length < 10)
                return false;

            isbn = isbn.Replace("X", "10");

            int[] sequence = isbn.Select(c => Convert.ToInt32(c.ToString())).ToArray();

            var sum = sequence[0] * 10 + sequence[1] * 9 + sequence[2] * 8 +
                sequence[3] * 7 + sequence[4] * 6 + sequence[5] * 5 +
                sequence[6] * 4 + sequence[7] * 3 + sequence[8] * 2;

            int remainder = sum % 11;

            int checkdigit = 11 - remainder;

            if (sequence.Length == 11)
            {
                if (checkdigit == 10)
                    return true;
            }


            if (sequence[9] == checkdigit)
                return true;


            return false;
        }

        private bool ISBN13(string isbn)
        {
            if (String.IsNullOrEmpty(isbn) ||
                isbn.Contains("-") ||
                isbn.Contains(" ") ||
                isbn.Length < 13)
                return false;

            if (isbn.Substring(0, 3) != "978")
                return false;

            int[] sequence = isbn.Select(c => Convert.ToInt32(c.ToString())).ToArray();

            var sum = sequence[0] * 1 + sequence[1] * 3 + sequence[2] * 1 +
                sequence[3] * 3 + sequence[4] * 1 + sequence[5] * 3 +
                sequence[6] * 1 + sequence[7] * 3 + sequence[8] * 1 +
                sequence[9] * 3 + sequence[10] * 1 + sequence[11] * 3;

            int remainder = sum % 10;

            int checkdigit = 10 - remainder;

            if (sequence[12] == checkdigit)
                return true;


            return false;
        }

    }
}
