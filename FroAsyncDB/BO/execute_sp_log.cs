//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FroAsyncDB.BO
{
    using System;
    using System.Collections.Generic;
    
    public partial class execute_sp_log
    {
        public long sp_log { get; set; }
        public Nullable<System.DateTime> log_date { get; set; }
        public Nullable<int> exec_id { get; set; }
        public string log_result { get; set; }
    
        public virtual execute_sp execute_sp { get; set; }
    }
}