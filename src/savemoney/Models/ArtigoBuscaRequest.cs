using Microsoft.AspNetCore.Mvc;

namespace savemoney.Models
{
    public class ArtigoBuscaRequest
    {
        [FromQuery(Name = "searchTerm")]
        public string? SearchTerm { get; set; }

        [FromQuery(Name = "region")]
        public string? Region { get; set; }

        [FromQuery(Name = "sortOrder")]
        public string? SortOrder { get; set; }

        // MUDANÇA: 'Topic' removido
        
        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 6;
    }
}