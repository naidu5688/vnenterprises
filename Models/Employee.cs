using System.ComponentModel.DataAnnotations;

namespace vnenterprises.Models
{
    public class Employee
    {

    }
    public class userDetailsReposne()
    {
        public int UserId { get; set; }
        public int AccessId { get; set; }
        public string AccessType { get; set; }
        public string UserName { get; set; }
    }

    public class CustomerModel
    {
        public int CustomerId { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public string aadharnumber { get; set; }
        public string pannumber { get; set; }

        // Mandatory documents
        [Required]
        public IFormFile AadhaarFrontImage { get; set; }

        [Required]
        public IFormFile AadhaarBackImage { get; set; }

        [Required]
        public IFormFile PanFrontImage { get; set; }

        // Optional
        public IFormFile? PanBackImage { get; set; }
        public string aadharfrontpath { get; set; }
        public string aadharbackpath { get; set; }
        public string panfrontpath { get; set; }
        public string panbackpath { get; set; }
        public string KycStatus { get; set; }

        // Multiple Credit Cards
        public List<CreditCardModel> CreditCards { get; set; }
        public List<BanksDetails> BanksDetailsModel { get; set; }
    }

    public class BanksDetails
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public bool IsActive { get; set; }
    }
    public class CreditCardModel
    {
        [Required]
        public string NameOnCard { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public int CardCVV  { get; set; }

        [Required]
        public string ExpiryDate { get; set; }

        [Required]
        public int CardTypeId { get; set; }
        public bool IsActive { get; set; }
    }

    public class credircardtypes
    {
        public int Id { get; set; }
        public string  Name { get; set; }
    }
    public class CustomerDetailsModel
    {
        public int CustomerId { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public string aadharnumber { get; set; }
        public string pannumber { get; set; }

        // Mandatory documents
        [Required]
        public IFormFile AadhaarFrontImage { get; set; }

        [Required]
        public IFormFile AadhaarBackImage { get; set; }

        [Required]
        public IFormFile PanFrontImage { get; set; }

        // Optional
        public IFormFile? PanBackImage { get; set; }

        public string aadharfrontpath { get; set; }
        public string aadharbackpath { get; set; }
        public string panfrontpath { get; set; }
        public string panbackpath { get; set; }
        public string KycStatus { get; set; }
        public int TransactionsCount { get; set; }
        public List<CreditCardModel> CreditCards { get; set; }
    }

    public class BankModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class CreditCardListModel
    {
        public int Id { get; set; }
        public string cardNumber { get; set; }
    }
}
