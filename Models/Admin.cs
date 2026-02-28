namespace vnenterprises.Models
{
    public class Admin
    {
    }
    public class Platforms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Charge { get; set; }
        public string Status { get; set; } // Active / Inactive
    }

    public class UserRoles
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Gateway
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlatformId { get; set; }
        public string PlatformName { get; set; }
        public decimal Charge { get; set; }
        public string Status { get; set; } // Active / Inactive
    }
    public class TransactionSummary
    {
        public int TotalTransaction { get; set; }
        public decimal TotalTransactionsAmount { get; set; }
        public int TodayTransaction { get; set; }
        public decimal TodayTransactionsAmount { get; set; }
        public decimal ProfitOverall { get; set; }
        public decimal ProfitToday { get; set; }
    }
    // ViewModel
    public class PlatformGatewayViewModel
    {
        public List<Platforms> Platforms { get; set; }
        public List<Gateway> Gateways { get; set; }
    }

    public class Banks
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class Branches
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ManagerModel
    {
        public int UserId { get; set; }
        public int ManagerId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string MPIN { get; set; }
        public string KycStatus { get; set; }
        public PlatformGatewayViewModel platformgatwaymodel { get; set; }
        public List<Branches> branchmodel { get; set; }
        public bool IsEmployeeViewAccess { get; set; }
        public bool IsEmployeeEditAccess { get; set; }
        public bool IsEmployeeAddAccess { get; set; }
        public bool IsRetailerViewAccess { get; set; }
        public bool IsRetailerEditAccess { get; set; }
        public bool IsRetailerAddAccess { get; set; }
        public bool IsKycViewAccess { get; set; }
        public bool IsKycEditAccess { get; set; }
        public bool IsKycAddAccess { get; set; }
        public List<int> SelectedBranches { get; set; } = new();
        public bool IsActive { get; set; }

    }

    public class EmployeeModel
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }   // 10 digits
        public string MPIN { get; set; }            // 4 digits
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string AadhaarNumber { get; set; } 
        public string PanNumber { get; set; }
        public string KycStatus { get; set; }
        public IFormFile AadhaarFrontImage { get; set; }   // REQUIRED
        public IFormFile AadhaarBackImage { get; set; }    // REQUIRED
        public IFormFile PanFrontImage { get; set; }       // REQUIRED
        public IFormFile? PanBackImage { get; set; }

        /* Allocation */
        public string aadharfrontpath { get; set; }
        public string aadharbackpath { get; set; }
        public string panfrontpath { get; set; }
        public string panbackpath { get; set; }
        public List<Branches> branchmodel { get; set; }
        public PlatformGatewayViewModel PlatformGatewayModel { get; set; }
        public List<int> SelectedGateways { get; set; } = new();
        public List<int> SelectedBranches { get; set; } = new();

    }

    public class ResultResponse 
    {
        public int result { get; set; }
        public string StatusMessage { get; set; }
    }
    public class GetEmployeeModel
    {
        public string BranchIds { get; set; }
        public int UserId { get; set; }
        public string RoleIds { get; set; }
        public string Kyc { get; set; }
        public string SearchText { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
    public class KycStatus
    {
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int TotalCount { get; set; }
    }

    public class GetEmployeeModelList
    {
        public int UserId { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeName { get; set; }
        public string AadharNumber { get; set; }
        public string PanNumber { get; set; }
        public bool IsKycApproveAccess { get; set; }
        public bool IsActive { get; set; }
        public int UserRoleId { get; set; }
        public int BranchId { get; set; }
        public string AccessName { get; set; }
        public string KYCStatus { get; set; }
        public DateTime KYCApprovedOn { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }

    public class Settings
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public bool IsActive { get; set; }
        public int Flag { get; set; }
    }
}
