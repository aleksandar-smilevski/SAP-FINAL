//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaperSurvey
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PaperSurvey()
        {
            this.OfflineAnswer = new HashSet<OfflineAnswer>();
        }
    
        public int id { get; set; }
        public int id_offlinesurvey { get; set; }
        public string id_interviewer { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfflineAnswer> OfflineAnswer { get; set; }
        public virtual OfflineSurvey OfflineSurvey { get; set; }
    }
}
