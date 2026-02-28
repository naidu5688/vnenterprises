using Microsoft.Data.SqlClient;
using System.Data;
using vnenterprises.Models;

namespace vnenterprises.Support
{
    public class AdminSupport
    {
        private readonly string _connectionString;
        public AdminSupport(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public int UpdateorinsertPlaform(Platforms platobj, int UserId)
        {
            var result = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_InsertPlatform", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@PlatformId", platobj.Id);
                cmd.Parameters.AddWithValue("@PlatformName", platobj.Name);
                cmd.Parameters.AddWithValue("@IsActive", platobj.Status);
                cmd.Parameters.AddWithValue("@CreatedBy", UserId);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result = dr["Result"] != DBNull.Value ?
                                                      Convert.ToInt32(dr["Result"]) : 0;

                        }
                    }

                }
                return result;
            }
        }
        public string ConvertFromBase64(string base64Password)
        {
            var bytes = Convert.FromBase64String(base64Password);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        public EmployeeModel getEditEmployeeDetail(GetEmployeeModel modelobj)
        {
            EmployeeModel model = new EmployeeModel();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetUserDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BranchIds", modelobj.BranchIds);
                cmd.Parameters.AddWithValue("@UserId", modelobj.UserId);
                cmd.Parameters.AddWithValue("@RoleIds", modelobj.RoleIds);
                cmd.Parameters.AddWithValue("@Kyc", modelobj.Kyc);
                cmd.Parameters.AddWithValue("@SearchText", modelobj.SearchText ?? "");
                cmd.Parameters.AddWithValue("@PageNo", modelobj.page);
                cmd.Parameters.AddWithValue("@PageSize", modelobj.pageSize);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    /* ========================
                       RESULT SET 1 → USER
                    ======================== */
                    if (dr.Read())
                    {
                        model.EmployeeId = Convert.ToInt32(dr["EmployeeId"]);
                        model.MobileNumber = Convert.ToString(dr["MobileNumber"]);
                        model.FirstName = Convert.ToString(dr["UserFirstName"]);
                        model.LastName = Convert.ToString(dr["UserLastName"]);

                        model.AadhaarNumber = Convert.ToString(dr["AadharNumber"]);
                        model.PanNumber = Convert.ToString(dr["PanNumber"]);

                        model.aadharfrontpath = Convert.ToString(dr["AadharImgFront"]);
                        model.aadharbackpath = Convert.ToString(dr["AadharImgBack"]);
                        model.panfrontpath = Convert.ToString(dr["PanImgFront"]);
                        model.panbackpath = Convert.ToString(dr["PanImgBack"]);
                        model.MPIN = Convert.ToString(dr["Mpin"]);
                        model.KycStatus = Convert.ToString(dr["KYCStatus"]);

                        // If password stored base64
                        if (dr["Password"] != DBNull.Value)
                        {
                            string encPwd = Convert.ToString(dr["Password"]);
                            model.Password = ConvertFromBase64(encPwd);
                            model.ConfirmPassword = model.Password;
                        }
                    }

                    /* ========================
                       RESULT SET 2 → BRANCHES
                    ======================== */
                    if (dr.NextResult())
                    {
                        model.SelectedBranches = new List<int>();

                        while (dr.Read())
                        {
                            model.SelectedBranches.Add(
                                Convert.ToInt32(dr["BranchId"])
                            );
                        }
                    }

                    /* ========================
                       RESULT SET 3 → GATEWAYS
                    ======================== */
                    if (dr.NextResult())
                    {
                        model.SelectedGateways = new List<int>();

                        while (dr.Read())
                        {
                            model.SelectedGateways.Add(
                                Convert.ToInt32(dr["GatewayID"])
                            );
                        }
                    }
                }
            }

            return model;
        }


        public TransactionSummary GetTransactionSummary()
        {
            TransactionSummary result = new TransactionSummary();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("usp_fn_GetTransactionSummary", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            result.TotalTransaction =
                                dr["TotalTransaction"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalTransaction"]);

                            result.TotalTransactionsAmount =
                                dr["TotalTransactionsAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["TotalTransactionsAmount"]);

                            result.TodayTransaction =
                                dr["TodayTransaction"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TodayTransaction"]);

                            result.TodayTransactionsAmount =
                                dr["TodayTransactionsAmount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TodayTransactionsAmount"]);

                            result.ProfitOverall =
                                dr["ProfitOverall"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ProfitOverall"]);

                            result.ProfitToday =
                                dr["ProfitToday"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ProfitToday"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new TransactionSummary();
            }

            return result;
        }
        private DataTable ToIntListTable(List<int> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var item in list)
                dt.Rows.Add(item);

            return dt;
        }

        public Platforms SaveOrUpdatePlatform(Platforms model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            int status = 0;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("vn_InsertorUpdatePlatform", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PlatformId", model.Id);
                cmd.Parameters.AddWithValue("@PlatformName", model.Name);
                cmd.Parameters.AddWithValue("@IsActive", model.Status == "Active" ? 1 : 0);
                cmd.Parameters.AddWithValue("@PlatformCharge", model.Charge);

                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        status = reader.GetInt32(0); // SP returns Status
                }

                // If inserted, get the new PlatformId
                if (model.Id == 0 && status == 1)
                {
                    // Get the newly inserted Id
                    using (var idCmd = new SqlCommand("SELECT TOP 1 PlatformId FROM tblPlatforms ORDER BY CreatedOn DESC", con))
                    {
                        model.Id = (int)idCmd.ExecuteScalar();
                    }
                }
            }

            if (status == 2)
                throw new Exception(model.Id == 0 ? "Platform already exists" : "PlatformId not found");

            return model;
        }
        public Gateway SaveOrUpdateGateway(Gateway model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(model.Name) || model.PlatformId <= 0)
                throw new ArgumentException("Invalid gateway data");

            int status = 0;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("vn_InsertorUpdateGateway", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@GatewayID", model.Id);
                cmd.Parameters.AddWithValue("@PlatformID", model.PlatformId);
                cmd.Parameters.AddWithValue("@GatewayName", model.Name);
                cmd.Parameters.AddWithValue("@IsActive", model.Status == "Active" ? 1 : 0);
                cmd.Parameters.AddWithValue("@GatewayCharge", model.Charge);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        status = reader.GetInt32(0);
                }

                // If inserted, get the new GatewayID
                if (model.Id == 0 && status == 1)
                {
                    using (var idCmd = new SqlCommand("SELECT TOP 1 GatewayID FROM tblGateways ORDER BY CreatedOn DESC", conn))
                    {
                        model.Id = (int)idCmd.ExecuteScalar();
                    }
                }
            }

            if (status == 2)
                throw new Exception(model.Id == 0 ? "Gateway already exists under this platform" : "GatewayID not found");

            return model;
        }

        public (List<TransactionListModel> data, int totalCount) GetTransactionsForAdmin(TransactionviewModel model)
        {
            List<TransactionListModel> result = new();
            int totalCount = 0;

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("usp_fn_GetTransactionforAdmin", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TransactionId", model.TransactionId);
            cmd.Parameters.AddWithValue("@TransactionType", model.TransactionType ?? "");
            cmd.Parameters.AddWithValue("@BranchId", model.BranchId ?? "");
            cmd.Parameters.AddWithValue("@PlatformId", model.PlatformId ?? "");
            cmd.Parameters.AddWithValue("@GatewayId", model.GatewayId ?? "");
            cmd.Parameters.AddWithValue("@SearchText", model.SearchText ?? "");
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate ?? "");
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate ?? "");
            cmd.Parameters.AddWithValue("@PageNo", model.PageNo);      // ✅ FIX
            cmd.Parameters.AddWithValue("@PageSize", model.PageSize);
            con.Open();

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    result.Add(new TransactionListModel
                    {
                        TransactionId = Convert.ToInt32(dr["TransactionId"]),
                        UserId = Convert.ToInt32(dr["UserId"]),
                        CustomerId = Convert.ToInt32(dr["CustomerId"]),
                        PlatformId = Convert.ToInt32(dr["PlatformId"]),
                        PlatformName = dr["PlatformName"].ToString(),
                        GatewayId = Convert.ToInt32(dr["GatewayId"]),
                        GatewayName = dr["GatewayName"].ToString(),
                        GatewayCharge = Convert.ToDecimal(dr["GatewayCharge"]),
                        IncentiveId = Convert.ToInt32(dr["IncentiveId"]),
                        IncentiveName = dr["IncentiveName"].ToString(),
                        IncentiveAmount = Convert.ToDecimal(dr["IncentiveAmount"]),

                        WithdrawCardId = Convert.ToInt32(dr["WithdrawCardId"]),
                        WithdrawCardNumber = dr["WithdrawCardNumber"].ToString(),
                        WithdrawNameOnCard = dr["WithdrawNameOnCard"].ToString(),
                        WithdrawCardType = dr["WithdrawCardType"].ToString(),
                        WithdrawCardCVV = dr["WithdrawCardCVV"].ToString(),
                        WithdrawCardExpiryDate = dr["WithdrawCardExpiryDate"].ToString(),

                        WithdrawBankId = Convert.ToInt32(dr["WithdrawBankId"]),
                        WithdrawBankNumber = dr["WithdrawBankNumber"].ToString(),
                        WithdrawBankName = dr["WithdrawBankName"].ToString(),
                        WithdrawBankIFSC = dr["WithdrawBankIFSC"].ToString(),

                        TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]),
                        BillAmount = Convert.ToDecimal(dr["BillAmount"]),
                        PlatformChargeAmount = Convert.ToDecimal(dr["PlatformChargeAmount"]),
                        EmployeeChargePercent = Convert.ToDecimal(dr["EmployeeChargePercent"]),
                        ProfitAmount = Convert.ToDecimal(dr["ProfitAmount"]),
                        PayOut = Convert.ToDecimal(dr["PayOut"]),
                        FinalAmount = Convert.ToDecimal(dr["FinalAmount"]),
                        WalletAmount = Convert.ToDecimal(dr["WalletAmount"]),
                        TobePaidByCustomer = Convert.ToDecimal(dr["TobePaidByCustomer"]),

                        SwipedCardId = Convert.ToInt32(dr["SwipedCardId"]),
                        SwipedCardNumber = dr["SwipedCardNumber"].ToString(),
                        SwipedNameOnCard = dr["SwipedNameOnCard"].ToString(),
                        SwipedCardType = dr["SwipedCardType"].ToString(),
                        SwipedCardCVV = dr["SwipedCardCVV"].ToString(),
                        SwipedCardExpiryDate = dr["SwipedCardExpiryDate"].ToString(),

                        SwipedBankId = Convert.ToInt32(dr["SwipedBankId"]),
                        SwipedBankNumber = dr["SwipedBankNumber"].ToString(),
                        SwipedBankName = dr["SwipedBankName"].ToString(),
                        SwipedBankIFSC = dr["SwipedBankIFSC"].ToString(),

                        CardAmountTransfer = Convert.ToDecimal(dr["CardAmountTransfer"]),
                        AccountAmountTransfer = Convert.ToDecimal(dr["AccountAmountTransfer"]),
                        QRPayAmountTransfer = Convert.ToDecimal(dr["QRPayAmountTransfer"]),
                        AccountPayAmountTransfer = Convert.ToDecimal(dr["AccountPayAmountTransfer"]),
                        UPIPayAmountTransfer = Convert.ToDecimal(dr["UPIPayAmountTransfer"]),
                        OthersAmountTransfer = Convert.ToDecimal(dr["OthersAmountTransfer"]),
                        Difference = Convert.ToDecimal(dr["Difference"]),

                        Remarks = dr["Remarks"].ToString(),
                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"]),
                        CustomerName = dr["CustomerName"].ToString(),
                        EmployeeName = dr["EmployeeName"].ToString()

                    });
                }

                // 👉 NEXT RESULT = TOTAL COUNT (you must add this in SP)
                if (dr.NextResult() && dr.Read())
                    totalCount = Convert.ToInt32(dr["TotalCount"]);
            }

            return (result, totalCount);
        }
        public ResultResponse UpdateorInsertManager(ManagerModel model , int UserId)
        {
            var response = new ResultResponse();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_InsertorupdateManager", con)) {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ManagerId", model.ManagerId);
                cmd.Parameters.AddWithValue("@UserFirstName", model.UserFirstName);
                cmd.Parameters.AddWithValue("@UserLastName", model.UserLastName);
                cmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                cmd.Parameters.AddWithValue("@Password", model.Password);
                cmd.Parameters.AddWithValue("@MPIN", model.MPIN);
                cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus ?? "");

                cmd.Parameters.Add(new SqlParameter("@SelectedBranches", SqlDbType.Structured)
                {
                    TypeName = "dbo.IntList",
                    Value = ToIntListTable(model.SelectedBranches)
                });
                cmd.Parameters.AddWithValue("@IsEmployeeAddAccess", model.IsEmployeeAddAccess);
                cmd.Parameters.AddWithValue("@IsEmployeeEditAccess", model.IsEmployeeEditAccess);
                cmd.Parameters.AddWithValue("@IsEmployeeViewAccess", model.IsEmployeeViewAccess);
                cmd.Parameters.AddWithValue("@IsRetailerAddAccess", model.IsRetailerAddAccess);
                cmd.Parameters.AddWithValue("@IsRetailerEditAccess", model.IsRetailerEditAccess);
                cmd.Parameters.AddWithValue("@IsRetailerViewAccess", model.IsRetailerViewAccess);
                cmd.Parameters.AddWithValue("@IsKycViewAccess", model.IsKycViewAccess);
                cmd.Parameters.AddWithValue("@IsKycEditAccess", model.IsKycEditAccess);
                cmd.Parameters.AddWithValue("@IsKycAddAccess", model.IsKycAddAccess);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                con.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    response.result = Convert.ToInt32(dr["Result"]);
                    response.StatusMessage = dr["Status"].ToString();
                }
            }

            return response;
        }
        public KycStatus GetKycCounts(int FlagType)
        {
            KycStatus result = new KycStatus();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetKycStatusCount", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FlagType", FlagType);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.Pending = reader["Pending"] != DBNull.Value ? Convert.ToInt32(reader["Pending"]) : 0;
                        result.Rejected = reader["Rejected"] != DBNull.Value ? Convert.ToInt32(reader["Rejected"]) : 0;
                        result.Approved = reader["Approved"] != DBNull.Value ? Convert.ToInt32(reader["Approved"]) : 0;
                    }

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            result.TotalCount = reader["TotalCount"] != DBNull.Value
                                ? Convert.ToInt32(reader["TotalCount"])
                                : 0;
                        }
                    }
                }
            }

            return result;
        }
        public (List<CustomerDetailsModel> data, int totalCount) GetCustomers(GetEmployeeModel model)
        {
            int totalCount = 0;
            List<CustomerDetailsModel> result = new();

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("usp_fn_GetCustomerDetails", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", model.UserId);
            cmd.Parameters.AddWithValue("@Kyc", model.Kyc);
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
            cmd.Parameters.AddWithValue("@SearchText", model.SearchText ?? "");
            cmd.Parameters.AddWithValue("@PageNo", model.page);
            cmd.Parameters.AddWithValue("@PageSize", model.pageSize);

            con.Open();

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    result.Add(new CustomerDetailsModel
                    {
                        CustomerId = Convert.ToInt32(dr["CustomerId"]),
                        PhoneNumber = dr["PhoneNumber"].ToString(),
                        CustomerFullName = dr["CustomerName"].ToString(),
                        aadharnumber = dr["AadharNumber"].ToString(),
                        pannumber = dr["PanNumber"].ToString(),
                        //BranchId = Convert.ToInt32(dr["BranchId"]),
                        KycStatus = dr["KycStatus"].ToString(),
                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        CreatedBy = dr["CreatedBy"].ToString()
                    });
                }

                if (dr.NextResult() && dr.Read())
                    totalCount = Convert.ToInt32(dr["TotalCount"]);
            }

            return (result, totalCount);
        }
        public (List<GetEmployeeModelList> data, int totalCount) getEmployeeDetail(GetEmployeeModel model)
        {
            int totalCount = 0;
            List<GetEmployeeModelList> result = new();

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("usp_fn_GetUserDetails", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchIds", model.BranchIds);
            cmd.Parameters.AddWithValue("@UserId", model.UserId);
            cmd.Parameters.AddWithValue("@RoleIds", model.RoleIds);
            cmd.Parameters.AddWithValue("@Kyc", model.Kyc);
            cmd.Parameters.AddWithValue("@SearchText", model.SearchText ?? "");
            cmd.Parameters.AddWithValue("@PageNo", model.page);      
            cmd.Parameters.AddWithValue("@PageSize", model.pageSize);

            con.Open();

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    result.Add(new GetEmployeeModelList
                    {
                        UserId = Convert.ToInt32(dr["UserId"]),
                        MobileNumber = dr["MobileNumber"].ToString(),
                        EmployeeName = dr["EmployeeName"].ToString(),
                        AadharNumber = dr["AadharNumber"].ToString(),
                        PanNumber = dr["PanNumber"].ToString(),
                        UserRoleId = Convert.ToInt32(dr["UserRoleId"]),
                        //BranchId = Convert.ToInt32(dr["BranchId"]),
                        AccessName = dr["AccessName"].ToString(),
                        IsKycApproveAccess = Convert.ToBoolean(dr["IsKycApproveAccess"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        KYCStatus = dr["KYCStatus"].ToString(),
                        KYCApprovedOn = Convert.ToDateTime(dr["KYCApprovedOn"]),
                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        CreatedBy = dr["CreatedBy"].ToString()
                    });
                }

                if (dr.NextResult() && dr.Read())
                    totalCount = Convert.ToInt32(dr["TotalCount"]);
            }

            return (result, totalCount);
        }
        public ResultResponse UpdateorInsertEmployee(EmployeeModel model, int UserId)
        {
            var response = new ResultResponse();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("vn_InsertorupdateEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
                    cmd.Parameters.AddWithValue("@UserFirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@UserLastName", model.LastName);
                    cmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@MPIN", model.MPIN);
                    cmd.Parameters.AddWithValue("@AadhaarNumber", model.AadhaarNumber);
                    cmd.Parameters.AddWithValue("@PanNumber", model.PanNumber);
                    cmd.Parameters.AddWithValue("@AadhaarFrontImage", model.aadharfrontpath);
                    cmd.Parameters.AddWithValue("@AadhaarBackImage", model.aadharbackpath);
                    cmd.Parameters.AddWithValue("@PanFrontImage", model.panfrontpath);
                    cmd.Parameters.AddWithValue("@PanBackImage", model.panbackpath ?? "");
                    cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus ?? "");
                    cmd.Parameters.Add(new SqlParameter("@SelectedGateways", SqlDbType.Structured)
                    {
                        TypeName = "dbo.IntList",
                        Value = ToIntListTable(model.SelectedGateways)
                    });
                    cmd.Parameters.Add(new SqlParameter("@SelectedBranches", SqlDbType.Structured)
                    {
                        TypeName = "dbo.IntList",
                        Value = ToIntListTable(model.SelectedBranches)
                    });
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    con.Open();
                    using SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        response.result = Convert.ToInt32(dr["Result"]);
                        response.StatusMessage = dr["Status"].ToString();
                    }
                }

            }
            catch (Exception e)
            {
                response.result = 0; ;
                response.StatusMessage = "Something Wrong. Please Try again.";
            }
            return response;
        }
        public ResultResponse UpdateorInsertRetailer(EmployeeModel model, int UserId)
        {
            var response = new ResultResponse();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("vn_InsertorupdateRetailer", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@EmployeeId", model.EmployeeId);
                    cmd.Parameters.AddWithValue("@UserFirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@UserLastName", model.LastName);
                    cmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@MPIN", model.MPIN);
                    cmd.Parameters.AddWithValue("@AadhaarNumber", model.AadhaarNumber);
                    cmd.Parameters.AddWithValue("@PanNumber", model.PanNumber);
                    cmd.Parameters.AddWithValue("@AadhaarFrontImage", model.aadharfrontpath);
                    cmd.Parameters.AddWithValue("@AadhaarBackImage", model.aadharbackpath);
                    cmd.Parameters.AddWithValue("@PanFrontImage", model.panfrontpath);
                    cmd.Parameters.AddWithValue("@PanBackImage", model.panbackpath ?? "");
                    cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus ?? "");
                    cmd.Parameters.Add(new SqlParameter("@SelectedGateways", SqlDbType.Structured)
                    {
                        TypeName = "dbo.IntList",
                        Value = ToIntListTable(model.SelectedGateways)
                    });
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    con.Open();
                    using SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        response.result = Convert.ToInt32(dr["Result"]);
                        response.StatusMessage = dr["Status"].ToString();
                    }
                }

            }
            catch (Exception e)
            {
                response.result = 0; ;
                response.StatusMessage = "Something Wrong. Please Try again.";
            }
            return response;
        }
        public ManagerModel GetManagerDetails(int ManagerId)
        {
            ManagerModel model = new ManagerModel();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetUserDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BranchIds", "");
                cmd.Parameters.AddWithValue("@UserId", ManagerId);
                cmd.Parameters.AddWithValue("@RoleIds", "");
                cmd.Parameters.AddWithValue("@Kyc", "");
                cmd.Parameters.AddWithValue("@SearchText",  "");
                cmd.Parameters.AddWithValue("@PageNo", 1);
                cmd.Parameters.AddWithValue("@PageSize", 10);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            model.UserId = dr["UserId"] != DBNull.Value ? Convert.ToInt32(dr["UserId"]) : 0;
                            model.ManagerId = dr["EmployeeId"] != DBNull.Value ? Convert.ToInt32(dr["EmployeeId"]) : 0;

                            model.UserFirstName = dr["UserFirstName"] != DBNull.Value ? Convert.ToString(dr["UserFirstName"]) : "";
                            model.UserLastName = dr["UserLastName"] != DBNull.Value ? Convert.ToString(dr["UserLastName"]) : "";

                            model.MobileNumber = dr["MobileNumber"] != DBNull.Value ? Convert.ToString(dr["MobileNumber"]) : "";
                            string encPwd = dr["Password"] != DBNull.Value ? Convert.ToString(dr["Password"]) : "";
                            model.Password = ConvertFromBase64(encPwd);
                            model.ConfirmPassword = model.Password;
                            model.MPIN = dr["MPIN"] != DBNull.Value ? Convert.ToString(dr["MPIN"]) : "";
                            model.KycStatus = dr["KycStatus"] != DBNull.Value ? Convert.ToString(dr["KycStatus"]) : "Pending";

                            model.IsEmployeeViewAccess = dr["IsEmployeeViewAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsEmployeeViewAccess"]) : false;
                            model.IsEmployeeAddAccess = dr["IsEmployeeAddAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsEmployeeAddAccess"]) : false;
                            model.IsEmployeeEditAccess = dr["IsEmployeeEditAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsEmployeeEditAccess"]) : false;

                            model.IsRetailerViewAccess = dr["IsRetailerViewAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsRetailerViewAccess"]) : false;
                            model.IsRetailerAddAccess = dr["IsRetailerAddAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsRetailerAddAccess"]) : false;
                            model.IsRetailerEditAccess = dr["IsRetailerEditAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsRetailerEditAccess"]) : false;

                            model.IsKycViewAccess = dr["IsKycViewAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsKycViewAccess"]) : false;
                            model.IsKycAddAccess = dr["IsKycAddAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsKycAddAccess"]) : false;
                            model.IsKycEditAccess = dr["IsKycEditAccess"] != DBNull.Value ? Convert.ToBoolean(dr["IsKycEditAccess"]) : false;

                            model.IsActive = dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false;

                        }
                        if (dr.NextResult())
                        {
                            model.SelectedBranches = new List<int>();

                            while (dr.Read())
                            {
                                model.SelectedBranches.Add(
                                    Convert.ToInt32(dr["BranchId"])
                                );
                            }
                        }
                    }

                }
                return model;
            }
        }
        public List<Banks> GetBanksList()
        {
            var bankList = new List<Banks>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("vn_GetBanks", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bankList.Add(new Banks
                            {
                                BankId = reader.GetInt32(reader.GetOrdinal("BankId")),
                                BankName = reader.GetString(reader.GetOrdinal("BankName")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                            });
                        }
                    }
                }
            }

            return bankList;
        }
        public List<Branches> GetBranchList()
        {
            var branchList = new List<Branches>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("vn_GetBranch", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            branchList.Add(new Branches
                            {
                                BranchId = reader.GetInt32(reader.GetOrdinal("BranchId")),
                                BranchName = reader.GetString(reader.GetOrdinal("BranchName")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                            });
                        }
                    }
                }
            }

            return branchList;
        }
        public List<UserRoles> GetUserRoles()
        {
            var userroles = new List<UserRoles>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("vn_GetUserRoles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userroles.Add(new UserRoles
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserRoleId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            });
                        }
                    }
                }
            }

            return userroles;
        }
        public void DeleteSettings(int flag, int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_DeleteSettings", con)) // new SP for Delete
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Flag", flag);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void SaveSettings(int flag, Settings model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_SaveSettings", con)) // new SP for Save/Update
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Flag", flag);
                cmd.Parameters.AddWithValue("@Id", model?.Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", model?.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Amount", model?.Amount ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<Settings> GetSettings(int flag)
        {
            var list = new List<Settings>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetSettings", con)) // new SP for Get
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Flag", flag);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new Settings
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString(),
                            Amount = dr["Amount"] == DBNull.Value ? null : Convert.ToDecimal(dr["Amount"])
                        });
                    }
                }
            }
            return list;
        }
        public List<Settings> ManageSettings(int flag, Settings model)
        {
            var list = new List<Settings>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_fn_GetSettings", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", flag);
                    cmd.Parameters.AddWithValue("@Id", model?.Id ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", model?.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Amount", model?.Amount ?? (object)DBNull.Value);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new Settings
                            {
                                Id = Convert.ToInt32(dr["Id"]),
                                Name = dr["Name"].ToString(),
                                Amount = dr["Amount"] == DBNull.Value ? null : Convert.ToDecimal(dr["Amount"])
                            });
                        }
                    }
                }
            }

            return list;
        }
        public PlatformGatewayViewModel GetPlatformGatewayList()
        {
            var viewModel = new PlatformGatewayViewModel
            {
                Platforms = new List<Platforms>(),
                Gateways = new List<Gateway>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("vn_GetPlatfotmGateway", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PlatformId", 0);
                    cmd.Parameters.AddWithValue("@FlagType", 1);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Read Platforms
                        while (reader.Read())
                        {
                            viewModel.Platforms.Add(new Platforms
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Charge = reader.GetDecimal(reader.GetOrdinal("Charge")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            });
                        }

                        // Move to next result set (Gateways)
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                viewModel.Gateways.Add(new Gateway
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    PlatformName = reader.GetString(reader.GetOrdinal("PlatformName")),
                                    Charge = reader.GetDecimal(reader.GetOrdinal("Charge")),
                                    Status = reader.GetString(reader.GetOrdinal("Status"))
                                });
                            }
                        }
                    }
                }
            }

            return viewModel;
        }
    }
}
