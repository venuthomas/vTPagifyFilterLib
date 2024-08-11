using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
namespace PagifyFilter
{
    public static class EFCoreExtensions
    {

        public static async Task<PagedResult<Tdata>?> SearchAsync<Tdata, Tsearch>(
            this IEnumerable<Tdata> items,
            Tsearch searchDto,
            CancellationToken cancellationToken = default)
        {
            if (items is null)
                return null;

            IQueryable<Tdata> _data = items.AsQueryable();
#pragma warning disable CS8603 // Possible null reference return.
            if (searchDto is null)
            {
                long totalCount_data = await _data.CountAsync(cancellationToken);
                return new PagedResult<Tdata>(
                Items: _data,
                CurrentPageNo: 0,
                ItemsPerPage: 0,
                TotalItems: totalCount_data);
            }
#pragma warning restore CS8603 // Possible null reference return.

            var paginationProperties = typeof(Pagination).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var searchProperties = searchDto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x =>
            !paginationProperties.Select(y => y.Name).Contains(x.Name));

            var propertiesDictionary = typeof(Tdata).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            // Create an expression tree for filtering
            var parameter = Expression.Parameter(typeof(Tdata), "item");
            Expression filterExpression = Expression.Constant(true); // Initial true condition

            foreach (var prop in searchProperties)
            {
                var searchValue = prop.GetValue(searchDto);
                if (searchValue == null) continue;

                try
                {
                    if (propertiesDictionary.TryGetValue(prop.Name, out var itemProperty))
                    {
                        var itemValue = Expression.Property(parameter, itemProperty);
                        var comparison = Expression.Equal(itemValue, Expression.Constant(searchValue));
                        filterExpression = Expression.AndAlso(filterExpression, comparison);
                    }
                }
                catch { continue; }
            }

            var filterLambda = Expression.Lambda<Func<Tdata, bool>>(filterExpression, parameter);
            var compiledFilter = filterLambda.Compile();

              IQueryable<Tdata> filteredSource = _data.Where(filterLambda);
            long totalCount = await filteredSource.LongCountAsync(cancellationToken);
            
           // Apply ordering (similar logic with pre-defined dictionaries)
            var sortColumn = GetValObj(searchDto, nameof(Pagination.SortColumn))?.ToString();
            var AscendingSort = bool.TryParse(GetValObj(searchDto, nameof(Pagination.IsAscendingSort))?.ToString(), out var isAscendingSort) && isAscendingSort;

            if (!string.IsNullOrWhiteSpace(sortColumn) && propertiesDictionary.TryGetValue(sortColumn, out var orderByProperty))
            {
                filteredSource = AscendingSort
                                        ? filteredSource.OrderByDescending(item => orderByProperty.GetValue(item))
                                        : filteredSource.OrderBy(item => orderByProperty.GetValue(item));
            }

             // Pagination (consider database-level pagination or optimized skipping if applicable)
            int? pageSize = (int?)GetValObj(searchDto, nameof(Pagination.ItemsPerPage));
            int? currentPageNo = (int?)GetValObj(searchDto, nameof(Pagination.CurrentPageNo));

            if (pageSize is not null && currentPageNo is not null)
                filteredSource = filteredSource.Skip((int)pageSize * ((int)currentPageNo - 1)).Take((int)pageSize);

            return new PagedResult<Tdata>(
                Items: filteredSource,
                CurrentPageNo: currentPageNo,
                ItemsPerPage: pageSize,
                TotalItems: totalCount);
        }
        private static object? GetValObj(object obj, string key)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
            return obj.GetType().GetProperty(key).GetValue(obj);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
