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
    
    public partial class Survey
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Survey()
        {
            this.AddToSurvey = new HashSet<AddToSurvey>();
            this.Links = new HashSet<Links>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Category { get; set; }
        public int Survey_type { get; set; }
        public System.DateTime Date { get; set; }
        public bool Is_active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AddToSurvey> AddToSurvey { get; set; }
        public virtual Category Category1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Links> Links { get; set; }
        public virtual OfflineSurvey OfflineSurvey { get; set; }
        public virtual OnlineSurvey OnlineSurvey { get; set; }
        public virtual SurveyType SurveyType { get; set; }
    }
}
