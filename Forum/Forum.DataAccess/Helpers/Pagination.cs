using System.Collections.Generic;

namespace Forum.DataAccess.Helpers
{
    public class Pagination<T> where T : class
    {
        public Pagination(int pageIndex, int pageSize, int count, int last, IEnumerable<int> pages, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            LastPage = last;
            Pages = pages;
            Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public int LastPage { get; set; }
        public IEnumerable<int> Pages { get; internal set; }

        public IReadOnlyList<T> Data { get; set; }
    }
}
