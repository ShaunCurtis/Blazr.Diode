/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Core;

public class BookCommandHandler
{
    BookProvider _bookProvider;

    public BookCommandHandler(BookProvider bookProvider)
    {
        _bookProvider = bookProvider;
    }

    public async ValueTask<CommandResult> ExecuteAsync(CommandRequest<Book> request)
    {
        await Task.Delay(10);

        if (request.type == CommandRequestType.Create)
        {
            if (_bookProvider.Books.Any(item => item.BookUid == request.item.BookId.Value))
                return new CommandResult(false, "A book already exists with that Uid.");

            return this.AddBook(request.item);
        }

        if (request.type == CommandRequestType.Update)
        {
            var book = _bookProvider.Books.FirstOrDefault(item => item.BookUid == request.item.BookId.Value);

            if (book is null)
                return new CommandResult(false, "No book exists with that Uid.");

            _bookProvider.Books.Remove(book);

            return this.AddBook(request.item);
        }

        if (request.type == CommandRequestType.Delete)
        {
            var book = _bookProvider.Books.FirstOrDefault(item => item.BookUid == request.item.BookId.Value);

            if (book is null)
                return new CommandResult(false, "No book exists with that Uid.");

            _bookProvider.Books.Remove(book);

            return new CommandResult(true);
        }

        return new CommandResult(false, "No action requested.");
    }

    private CommandResult AddBook(Book book)
    {
        _bookProvider.Books.Add(new DboBook(
            book.BookId.Value,
            book.Title));

        return new CommandResult(true);
    }
}