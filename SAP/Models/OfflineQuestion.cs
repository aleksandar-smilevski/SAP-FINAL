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
    
    public partial class OfflineQuestion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OfflineQuestion()
        {
            this.OfflineAnswer = new HashSet<OfflineAnswer>();
            this.OfflineValues = new HashSet<OfflineValues>();
        }
    
        public int id_question { get; set; }
        public string question_text { get; set; }
        public string question_desc { get; set; }
        public bool is_required { get; set; }
        public string question_type { get; set; }
        public int id_offline_survey { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfflineAnswer> OfflineAnswer { get; set; }
        public virtual OfflineSurvey OfflineSurvey { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfflineValues> OfflineValues { get; set; }
    }
}
