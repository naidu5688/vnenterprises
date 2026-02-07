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

    public class TransactionviewModel
    {
        public int TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string SearchText { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int UserId { get; set; }
    }

    public class TransactionListModel
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int CardTypeId { get; set; }
        public string CardTypeName { get; set; }
        public  string CardHolder { get; set; }
		public string CardExpiryDate { get; set; }
		public string CardCVV { get; set; }
        public string CardNumber { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankIFSC { get; set; }
        public string BankId { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string IncentiveName { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime CreatedOn { get; set; }  
        public int PlatformId { get; set; }
        public string PlatformName { get; set; }
        public decimal PlatformCharge { get; set; }
        public int GatewayId { get; set; }
        public string GatewayName { get; set; }
        public decimal GatewayCharge { get; set; }
        public decimal EmployeeChargePercent { get; set; }
        public decimal EmployeeChargeAmount { get; set; }
        public string Remark { get; set; }
    }
    public class UserTransactionsummary
    {
        public int TotalTransactions { get; set; }
        public int TodayTransactions { get; set; }
        public int TotalIncentives { get; set; }
        public int TodayIncentives { get; set; }
        public int TotalCustomersAdded { get; set; }
        public int TodayCustomersAdded { get; set; }
    }
    public class TransactionCreateDto
    {
        public int CustomerId { get; set; }
        public int PlatformId { get; set; }
        public int GatewayId { get; set; }
        public int CardId { get; set; }
        public int BankDetailsId { get; set; }
        public string Remarks { get; set; }
        public int TransactionTypeId { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal PlatformChargeAmount { get; set; }
        public decimal EmployeeChargePercent { get; set; }
        public decimal EmployeeChargeAmount { get; set; }
        public decimal FinalAmount { get; set; }
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
    public class CustomerPaymentDetailsVM
    {
        public List<CreditCardListModel> CreditCards { get; set; } = new();
        public List<BanksDetails> Banks { get; set; } = new();
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
        public List<BanksDetails> BanksDetailsModel { get; set; }
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
        public string nameoncard { get; set; }
        public string cardtypename { get; set; }
        public string cardcvv { get; set; }
        public string expirydate { get; set; }
    }
}
