﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class SplitDetailsVM
    {
        public string expenseInformation { get; set; } //TODO ersätt med whoboughtwhat..
        public bool objectionExists { get; set; } = false;
        public List<string> objectionMessage { get; set; }
        public WhoBoughtWhatVM expenseInfo { get; set; }
        public int ExpenseId { get; set; }


    }
}
