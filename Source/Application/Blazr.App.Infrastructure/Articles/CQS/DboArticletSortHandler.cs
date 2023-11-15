/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public class DboArticleSortHandler : RecordSortHandler<DboArticle>, IRecordSortHandler<DboArticle>
{
    public DboArticleSortHandler()
    {
        DefaultSorter = (item) => item.Title;
        DefaultSortDescending = false;
    }
}
