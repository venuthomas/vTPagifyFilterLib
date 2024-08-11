namespace PagifyFilter
{
    public class PagedResult<TResult> : Pagination
    {
        public IQueryable<TResult>? Items { get; set; }


        public PagedResult(IQueryable<TResult> Items, int? CurrentPageNo, int? ItemsPerPage, long TotalItems)
        {
            this.Items = Items;
            this.CurrentPageNo = CurrentPageNo;
            this.ItemsPerPage = ItemsPerPage;
            this.TotalItems = TotalItems;
        }
    }

}