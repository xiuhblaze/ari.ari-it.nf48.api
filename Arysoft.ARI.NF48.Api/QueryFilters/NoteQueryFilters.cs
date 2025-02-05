using Arysoft.ARI.NF48.Api.Enumerations;
using System;

namespace Arysoft.ARI.NF48.Api.QueryFilters
{
    public class NoteQueryFilters : BaseQueryFilters
    {
        public Guid? OwnerID { get; set; }

        public string Text { get; set; }

        public StatusType? Status { get; set; }

        public NoteOrderType? Order { get; set; }
    }
}