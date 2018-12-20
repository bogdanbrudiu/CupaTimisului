

using System.ComponentModel.DataAnnotations;

namespace Submitter.Models
{
    public class Entry
    {

        [Key]
        public int ID { get; set; }

        [Display(Name = "Indicativ:")]
        [Required(ErrorMessage = "Indicativul este obligatoriu!")]
        public string Calsign { get; set; }

        [Display(Name = "Email:")]
        [RegularExpression(@"\b[A-Za-z0-9\._\+\-]+@(?:[A-Za-z0-9-]+\.)+[A-Za-z]{2,4}\b", ErrorMessage = "Email address is not valid!")]
        [Required(ErrorMessage = "Adresa de email este obligatorie!")]
        public string Email { get; set; }

        [Display(Name = "Telefon:")]
        [Required(ErrorMessage = "Numarul de telefon este obligatoriu!")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Log-ul este obligatoriu!")]
        public string Log { get; set; }

        public string FileName { get; set; }
       
    }

}
