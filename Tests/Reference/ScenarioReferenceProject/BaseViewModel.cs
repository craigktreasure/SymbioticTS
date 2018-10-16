using SymbioticTS.Abstractions;
using System;

namespace ScenarioReferenceProject
{
    [TsClass(name: "ViewModelBase")]
    public abstract class BaseViewModel
    {
        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}