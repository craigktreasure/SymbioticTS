using SymbioticTS.Abstractions;
using System;

namespace Example.WebApplication.Models
{
    [TsDto]
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public DateTime Time { get; set; } = DateTime.Now;
    }
}