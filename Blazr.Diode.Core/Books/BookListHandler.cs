/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode.Core;

public class BookListHandler
{
    BookProvider _bookProvider;

    public BookListHandler(BookProvider bookProvider)
    {
        _bookProvider = bookProvider;
    }

    public async ValueTask<ItemQueryResult<Book>> ExecuteAsync(ItemRequest request)
    {
        await Task.Delay(10);

        if (!(request.KeyValue is Guid value))
            return new ItemQueryResult<Book>(null, false, "Wrong Key requested.");


        var book = _bookProvider.Books.FirstOrDefault(item => item.BookUid == bookUid.Value);
        if (book == null)
            return new ItemQueryResult<Book>(null, false, "No Book found.");

        var chapters = this.Chapters.Where(item => item.ChapterUid == bookUid.Value);

        var chapterList = new List<Chapter>();
        foreach (var chapter in chapters)
        {
            chapterList.Add(new Chapter()
            {
                ChapterId = new(chapter.ChapterUid),
                BookId = new(book.BookUid),
                Title = chapter.Title,
                Content = chapter.Content
            });
        }

        var newBook = new Book()
        {
            BookId = new(book.BookUid),
            Title = book.Title,
            Chapters = chapterList
        };

        return new ItemQueryResult<Book>(newBook, true);

    }

}
