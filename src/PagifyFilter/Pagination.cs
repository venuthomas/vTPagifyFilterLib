
namespace PagifyFilter
{
    public class Pagination
    {
        public int? CurrentPageNo { get; set; }
        public int? ItemsPerPage { get; set; }
        public long? TotalItems { get; set; }
        public string? SortColumn { get; set; }
        public bool? IsAscendingSort { get; set; }
    }
}