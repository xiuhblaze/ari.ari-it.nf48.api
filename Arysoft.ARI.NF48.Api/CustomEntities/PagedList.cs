using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arysoft.ARI.NF48.Api.CustomEntities
{
    public class PagedList<T> : List<T>
    {
        //private IEnumerable<MD5> items;
        //private int pageNumber;

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public int? PreviousPageNumber => HasPreviousPage ? CurrentPage - 1 : (int?)null;
        public int? NextPageNumber => HasNextPage ? CurrentPage + 1 : (int?)null;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(count / (double)pageSize);
            CurrentPage = pageNumber > TotalPages ? TotalPages : pageNumber;

            AddRange(items);
        }

        //public PagedList(IEnumerable<MD5> items, int pageNumber, int pageSize)
        //{
        //    this.items = items;
        //    this.pageNumber = pageNumber;
        //    PageSize = pageSize;
        //}

        /// <summary>
        /// Pagina en MEMORIA. Úsalo solo con colecciones ya materializadas.
        /// Si le pasas un IQueryable tipado como IEnumerable, ejecutará el query completo
        /// dos veces (Count y Skip/Take).
        /// </summary>
        public static PagedList<T> Create(IEnumerable<T> source, int? pageNumber = 0, int? pageSize = 0)
        {
            var pNumber = pageNumber == null || pageNumber < 1 ? 1 : pageNumber.Value;
            var pSize = pageSize == null || pageSize < 1 ? source.Count() : pageSize.Value;

            var count = source.Count();
            var items = source.Skip((pNumber - 1) * pSize).Take(pSize).ToList();

            return new PagedList<T>(items, count, pNumber, pSize);
        } // Create (IEnumerable)

        /// <summary>
        /// Pagina en SQL (COUNT + OFFSET/FETCH). El query DEBE venir ordenado (OrderBy),
        /// EF6 no soporta Skip sobre un query sin ordenar.
        /// </summary>
        public static PagedList<T> Create(IQueryable<T> source, int? pageNumber = 0, int? pageSize = 0)
        {
            var pNumber = pageNumber == null || pageNumber < 1 ? 1 : pageNumber.Value;

            var count = source.Count();
            var pSize = pageSize == null || pageSize < 1 ? count : pageSize.Value;

            if (count == 0 || pSize == 0)
                return new PagedList<T>(new List<T>(), count, pNumber, pSize);

            var items = source.Skip((pNumber - 1) * pSize).Take(pSize).ToList();

            return new PagedList<T>(items, count, pNumber, pSize);
        } // Create (IQueryable)

        //public static PagedList<T> Create(IEnumerable<T> source, int? pageNumber = 0, int? pageSize = 0)
        //{
        //    var pNumber = (int)(pageNumber == null || pageNumber == 0 ? 1 : pageNumber);
        //    var pSize = (int)(pageSize == null || pageSize == 0 ? source.Count() : pageSize);

        //    var count = source.Count();
        //    var items = source.Skip((pNumber - 1) * pSize).Take(pSize).ToList();

        //    return new PagedList<T>(items, count, pNumber, pSize);
        //}
    }
}