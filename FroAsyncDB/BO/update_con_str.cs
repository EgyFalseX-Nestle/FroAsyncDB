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
    
    public partial class update_con_str
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public update_con_str()
        {
            this.update_op_config = new HashSet<update_op_config>();
            this.update_op_config1 = new HashSet<update_op_config>();
            this.execute_sp = new HashSet<execute_sp>();
        }
    
        public int up_con_id { get; set; }
        public string con_str { get; set; }
        public string con_desc { get; set; }
        public string db_name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<update_op_config> update_op_config { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<update_op_config> update_op_config1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<execute_sp> execute_sp { get; set; }
    }
}
