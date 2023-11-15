/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode.Core;

internal record DboBook(Guid BookUid, string Title);
internal record DboChapter(Guid ChapterUid, Guid BookUid, string Title, string Content);

public record BookProvider
{
    internal List<DboBook> Books = new List<DboBook>();
    internal List<DboChapter> Chapters = new List<DboChapter>();

    public BookProvider()
    {
        this.Populate();
    }


    public async ValueTask<ItemQueryResult<Book>> GetBookAsync(BookUid bookUid)
    {
        await Task.Delay(10);

        var book = this.Books.FirstOrDefault(item => item.BookUid == bookUid.Value);
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

    private void Populate()
    {
        this.Books = new()
        {
            new(Guid.NewGuid(), "Life as a Coder" ),
            new(Guid.NewGuid(), "The Coder's Bible" ),
        };

        foreach (DboBook book in this.Books)
        {
            this.Chapters.Add(new(Guid.NewGuid(), book.BookUid, "Chapter 1", "Blah, blah"));
            this.Chapters.Add(new(Guid.NewGuid(), book.BookUid, "Chapter 2", "Blah, blah"));
        }
    }
}