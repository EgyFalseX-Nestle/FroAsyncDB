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
    
    public partial class update_op_conv
    {
        public int conv_id { get; set; }
        public int op_id { get; set; }
        public string col_name { get; set; }
        public string conversion { get; set; }
    
        public virtual update_op_config update_op_config { get; set; }
    }
}
