using Azure;
using Azure.Core;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.PortableExecutable;
using vnenterprises.Models;


namespace vnenterprises.Support
{
    public class EmployeeSupport
    {
        private readonly string _connectionString;
        public EmployeeSupport(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public int GetCustomerIdByMobile(string mobile)
        {
            var result = 0;
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("usp_fn_GetCustomerUserId", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PhoneNumber", mobile);

            con.Open();
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.HasRows)
                {

                    while (dr.Read())
                    {
                        result = Convert.ToInt32(dr["CustomerId"]);
                    }
                }

            }

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public List<Platforms> GetPlatformlist()
        {
            List<Platforms> platlist = new List<Platforms> ();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_GetPlatfotmGateway", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PlatformId", 0);
                cmd.Parameters.AddWithValue("@FlagType", 2);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        platlist.Add(new Platforms
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString()
                        });
                    }
                }
                return platlist;
            }
        }
        public List<Platforms> GetPlatformsByUserId(int UserId)
        {
            List<Platforms> platlist = new List<Platforms>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_GetPlatfotmGatewayByUserId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PlatformId", 0);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@FlagType", 1);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        platlist.Add(new Platforms
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString(),
                            Charge = Convert.ToDecimal(dr["Charge"])
                        });
                    }
                }
                return platlist;
            }
        }
        public List<BankModel> GetBankList()
        {
            List<BankModel> platlist = new List<BankModel>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_GetBanks", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        platlist.Add(new BankModel
                        {
                            Id = Convert.ToInt32(dr["BankId"]),
                            Name = dr["BankName"].ToString(),
                        });
                    }
                }
                return platlist;
            }
        }

        public CustomerPaymentDetailsVM GetCustomerCardsAndBanks(int customerId)
        {
            var result = new CustomerPaymentDetailsVM();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetCustomerCreditCards", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    // --------- CREDIT CARDS ----------
                    while (dr.Read())
                    {
                        result.CreditCards.Add(new CreditCardListModel
                        {
                            Id = Convert.ToInt32(dr["CreditCardsId"]),
                            cardNumber = dr["CardNumber"].ToString(),
                            nameoncard = dr["NameOnCard"].ToString(),
                            bankname = dr["BankName"].ToString(),
                            cardtypename = dr["CardTypeName"].ToString(),
                            expirydate = dr["ExpiryDate"].ToString(),
                            cardcvv = dr["CardCVV"].ToString()

                        });
                    }

                    // --------- BANKS ----------
                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            result.Banks.Add(new BanksDetails
                            {
                                BankId = Convert.ToInt32(dr["BankId"]),
                                BankName = dr["BankName"].ToString(),
                                HolderName = dr["HolderName"].ToString(),
                                AccountNumber = dr["AccountNumber"].ToString(),
                                IFSCCode = dr["IFSCCode"].ToString(),
                            });
                        }
                    }
                }
            }
            return result;
        }


        public List<BankModel> GetTranasactionTypesList()
        {
            List<BankModel> platlist = new List<BankModel>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetIncentiveType", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        platlist.Add(new BankModel
                        {
                            Id = Convert.ToInt32(dr["IncentiveType"]),
                            Name = dr["IncentiveName"].ToString(),
                        });
                    }
                }
                return platlist;
            }
        }
        public List<Gateway> GetGatewaylist(int PlatformId)
        {
            List<Gateway> platlist = new List<Gateway>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_GetPlatfotmGateway", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PlatformId", PlatformId);
                cmd.Parameters.AddWithValue("@FlagType", 3);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        platlist.Add(new Gateway
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString(),
                            Charge = Convert.ToDecimal(dr["Charge"])
                        });
                    }
                }
                return platlist;
            }
        }
        public List<Gateway> GetGatewaysByUserId(int PlatformId , int UserId, int flagtype = 2)
        {
            List<Gateway> platlist = new List<Gateway>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_GetPlatfotmGatewayByUserId", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PlatformId", PlatformId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@FlagType", flagtype);
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        platlist.Add(new Gateway
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString(),
                            PlatformId = Convert.ToInt32(dr["PlatformId"]),
                            Charge = Convert.ToDecimal(dr["Charge"])
                        });
                    }
                }
                return platlist;
            }
        }
        public CustomerModel GetCustomerDetails(int CustomerId)
        {
            var result = new CustomerModel
            {
                CreditCards = new List<CreditCardModel>() ,// ✅ FIX
                BanksDetailsModel = new List<BanksDetails>()
            };

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetCustomerDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        result.CustomerId = dr["CustomerId"] != DBNull.Value ? Convert.ToInt32(dr["CustomerId"]) : 0;
                        result.FirstName = dr["FirstName"]?.ToString() ?? "";
                        result.LastName = dr["LastName"]?.ToString() ?? "";
                        result.PhoneNumber = dr["PhoneNumber"]?.ToString() ?? "";
                        result.aadharnumber = dr["AadharNumber"]?.ToString() ?? "";
                        result.pannumber = dr["PanNumber"]?.ToString() ?? "";

                        result.aadharfrontpath = dr["AadharFrontImg"]?.ToString() ?? "";
                        result.aadharbackpath = dr["AadharBackImg"]?.ToString() ?? "";
                        result.panfrontpath = dr["PanFrontImg"]?.ToString() ?? "";
                        result.panbackpath = dr["PanBackImg"]?.ToString() ?? "";

                        result.KycStatus = dr["KycStatus"]?.ToString() ?? "Pending";
                    }

                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            result.CreditCards.Add(new CreditCardModel
                            {
                                CreditCardsId = dr["CreditCardsId"] != DBNull.Value ? Convert.ToInt32(dr["CreditCardsId"]) : 0,
                                NameOnCard = dr["NameOnCard"]?.ToString() ?? "",
                                CardNumber = dr["CardNumber"]?.ToString() ?? "",
                                CardTypeId = dr["CardTypeId"] != DBNull.Value ? Convert.ToInt32(dr["CardTypeId"]) : 0,
                                BankId = dr["BankId"] != DBNull.Value ? Convert.ToInt32(dr["BankId"]) : 0,
                                CardCVV = dr["CardCVV"] != DBNull.Value ? Convert.ToString(dr["CardCVV"]) : "",
                                BankName = dr["BankName"] != DBNull.Value ? Convert.ToString(dr["BankName"]) : "",
                               
                                ExpiryDate = dr["ExpiryDate"]?.ToString() ?? "",
                                IsActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"])
                            });
                        }
                    }
                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            result.BanksDetailsModel.Add(new BanksDetails
                            {
                                Id = dr["Id"] != DBNull.Value ? Convert.ToInt32(dr["Id"]) : 0,
                                BankId = dr["BankId"] != DBNull.Value ? Convert.ToInt32(dr["BankId"]) : 0,
                                BankName = dr["BankName"]?.ToString() ?? "",
                                HolderName = dr["HolderName"]?.ToString() ?? "",
                                AccountNumber = dr["AccountNumber"]?.ToString() ?? "",
                                IFSCCode = dr["IFSCCode"]?.ToString() ?? "",
                                IsActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"])
                            });
                        }
                    }
                }
            }

            return result;
        }
        public (List<GetEmployeeModelList> data, int totalCount) getEmployeeDetail(GetEmployeeModel model)
        {
            int totalCount = 0;
            List<GetEmployeeModelList> result = new();
            
            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("usp_fn_GetRetailerUserDetails", con);

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
        private DataTable ToIntListTable(List<CreditCardModel> list)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("CreditCardsId", typeof(int));
            dt.Columns.Add("CardTypeId", typeof(int));
            dt.Columns.Add("NameOnCard", typeof(string));
            dt.Columns.Add("CardNumber", typeof(string));
            dt.Columns.Add("ExprityDate", typeof(string));
            dt.Columns.Add("CardCVV", typeof(string));
            dt.Columns.Add("BankId", typeof(int));
            dt.Columns.Add("IsActive", typeof(bool));
            if(list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    dt.Rows.Add(
                        item.CreditCardsId,     // int
                        item.CardTypeId,     // int
                        item.NameOnCard,     // string
                        item.CardNumber,     // string
                        item.ExpiryDate,     // string
                        item.CardCVV,        // int
                        item.BankId,        // int
                        item.IsActive        // bool
                    );
                }
            }
            

            return dt;
        }
        private DataTable ToIntListTable(List<BanksDetails> list)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("BankId", typeof(int));
            dt.Columns.Add("BankName", typeof(string));
            dt.Columns.Add("HolderName", typeof(string));
            dt.Columns.Add("AccountNumber", typeof(string));
            dt.Columns.Add("IFSCCode", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            if(list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    dt.Rows.Add(
                        item.Id,     // int
                        item.BankId,     // int
                        item.BankName,     // string
                        item.HolderName,     // string
                        item.AccountNumber,     // int
                        item.IFSCCode,        // string
                        item.IsActive        // bool
                    );
                }
            }
            

            return dt;//test
        }
        private DataTable ToIntListTable(List<int> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var item in list)
                dt.Rows.Add(item);

            return dt;
        }
        public (List<TransactionListModel> data, int totalCount) GetTransactions(TransactionviewModel model)
        {
            List<TransactionListModel> result = new();
            int totalCount = 0;

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand("usp_fn_GetTransactionDetails", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TransactionId", model.TransactionId);
            cmd.Parameters.AddWithValue("@TransactionType", model.TransactionType ?? "");
            cmd.Parameters.AddWithValue("@SearchText", model.SearchText ?? "");
            cmd.Parameters.AddWithValue("@StartDate", model.StartDate ?? "");
            cmd.Parameters.AddWithValue("@EndDate", model.EndDate ?? "");
            cmd.Parameters.AddWithValue("@PageNo", model.PageNo);      // ✅ FIX
            cmd.Parameters.AddWithValue("@PageSize", model.PageSize);
            cmd.Parameters.AddWithValue("@UserId", model.UserId);

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
                        WithdrawCardBankName = dr["WithdrawCardBankName"].ToString(),
                        WithdrawCardNumber = dr["WithdrawCardNumber"].ToString(),
                        WithdrawNameOnCard = dr["WithdrawNameOnCard"].ToString(),
                        WithdrawCardType = dr["WithdrawCardType"].ToString(),
                        WithdrawCardCVV = dr["WithdrawCardCVV"].ToString(),
                        WithdrawCardExpiryDate = dr["WithdrawCardExpiryDate"].ToString(),

                        WithdrawBankId = Convert.ToInt32(dr["WithdrawBankId"]),
                        WithdrawBankHolderName = dr["WithdrawBankHolderName"].ToString(),
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
                        SwipedCardBankName = dr["SwipedCardBankName"].ToString(),
                        SwipedNameOnCard = dr["SwipedNameOnCard"].ToString(),
                        SwipedCardType = dr["SwipedCardType"].ToString(),
                        SwipedCardCVV = dr["SwipedCardCVV"].ToString(),
                        SwipedCardExpiryDate = dr["SwipedCardExpiryDate"].ToString(),

                        SwipedBankId = Convert.ToInt32(dr["SwipedBankId"]),
                        SwipedBankHolderName = dr["SwipedBankHolderName"].ToString(),
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

        public UserTransactionsummary GetUserTransaction(int userId)
        {
            UserTransactionsummary result = new UserTransactionsummary();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("usp_fn_GetUserTransactionDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            result.TotalTransactions =
                                dr["TotalTransactions"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalTransactions"]);

                            result.TodayTransactions =
                                dr["TodayTransactions"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TodayTransactions"]);

                            result.TotalIncentives =
                                dr["TotalIncentives"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalIncentives"]);

                            result.TodayIncentives =
                                dr["TodayIncentives"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TodayIncentives"]);

                            result.TotalCustomersAdded =
                                dr["TotalCustomersAdded"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalCustomersAdded"]);

                            result.TodayCustomersAdded =
                                dr["TodayCustomersAdded"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TodayCustomersAdded"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 🔴 Log error properly (DB / File / Serilog / NLog)
                // Logger.LogError(ex, "Error while fetching user transaction summary");

                // Return empty object (zeros) instead of crashing UI
                result = new UserTransactionsummary();
            }

            return result;
        }

        public int AddTransaction(TransactionCreateDto model, int UserId)
        {
            var result = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("vn_InsertTransaction", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
                    cmd.Parameters.AddWithValue("@PlatformId", model.PlatformId);
                    cmd.Parameters.AddWithValue("@GatewayId", model.GatewayId);
                    cmd.Parameters.AddWithValue("@TransactionTypeId", model.TransactionTypeId);
                    cmd.Parameters.AddWithValue("@WithdrawCardId", model.WithdrawCardId);
                    cmd.Parameters.AddWithValue("@WithdrawBankId", model.WithdrawBankId);
                    cmd.Parameters.AddWithValue("@TransactionAmount", model.TransactionAmount);
                    cmd.Parameters.AddWithValue("@BillAmount", model.BillAmount);
                    cmd.Parameters.AddWithValue("@PlatformChargeAmount", model.PlatformChargeAmount);
                    cmd.Parameters.AddWithValue("@EmployeeChargePercent", model.EmployeeChargePercent);
                    cmd.Parameters.AddWithValue("@ProfitAmount", model.ProfitAmount);
                    cmd.Parameters.AddWithValue("@PayOut", model.PayOut);
                    cmd.Parameters.AddWithValue("@FinalAmount", model.FinalAmount);
                    cmd.Parameters.AddWithValue("@WalletAmount", model.WalletAmount);
                    cmd.Parameters.AddWithValue("@ToBePaidByCustomer", model.ToBePaidByCustomer);
                    cmd.Parameters.AddWithValue("@CardTransferAmount", model.CardTransferAmount);
                    cmd.Parameters.AddWithValue("@BankTransferAmount", model.BankTransferAmount);
                    cmd.Parameters.AddWithValue("@TransferBankId", model.TransferBankId);
                    cmd.Parameters.AddWithValue("@TransferCardId", model.TransferCardId);
                    cmd.Parameters.AddWithValue("@QrTransferAmt", model.QrTransferAmt);
                    cmd.Parameters.AddWithValue("@AcntTransferAmt", model.AcntTransferAmt);
                    cmd.Parameters.AddWithValue("@UpiTransferAmt", model.UpiTransferAmt);
                    cmd.Parameters.AddWithValue("@OthersTransferAmt", model.OthersTransferAmt);
                    cmd.Parameters.AddWithValue("@DifferneceAmt", model.DifferneceAmt);
                    cmd.Parameters.AddWithValue("@Remarks", model.Remarks);
                    cmd.Parameters.AddWithValue("@UserId", UserId);

                    con.Open();
                    using SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        result = Convert.ToInt32(dr["TransactionId"]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return result;
        }


        public ResultResponse UpdateorInsertCustomer(CustomerModel model, int UserId)
        {
            var response = new ResultResponse();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("vn_InsertorupdateCustomer", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CustomerId", model.CustomerId);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@AadharNumber", model.aadharnumber);
                    cmd.Parameters.AddWithValue("@PanNumber", model.pannumber);
                    cmd.Parameters.AddWithValue("@AadharFrontImg", model.aadharfrontpath);
                    cmd.Parameters.AddWithValue("@AadharBackImg", model.aadharbackpath);
                    cmd.Parameters.AddWithValue("@PanFrontImg", model.panfrontpath);
                    cmd.Parameters.AddWithValue("@PanBackImg", model.panbackpath ?? "");
                    cmd.Parameters.AddWithValue("@OtherImg", "");
                    cmd.Parameters.AddWithValue("@KycStatus", model.KycStatus);
                    cmd.Parameters.Add(new SqlParameter("@CreditCards", SqlDbType.Structured)
                    {
                        TypeName = "dbo.CreditCardInsertType",
                        //Value = ToDataTable(model.CreditCards)
                        Value = ToIntListTable(model.CreditCards)
                    });
                    cmd.Parameters.Add(new SqlParameter("@BankDetails", SqlDbType.Structured)
                    {
                        TypeName = "dbo.CustomerBankInsertType",
                        //Value = ToDataTable(model.BanksDetailsModel)
                        Value = ToIntListTable(model.BanksDetailsModel)
                    });
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@CreatedBy", UserId);
                    cmd.Parameters.AddWithValue("@ChangedBy", UserId);
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
                Console.WriteLine(e.Message);
            }


            return response;
        }
        public userDetailsReposne GetUserDetails(int UserId)
        {
            var result = new userDetailsReposne();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetEmployeeUserDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", UserId);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result.UserId = dr["UserId"] != DBNull.Value ?
                                                      Convert.ToInt32(dr["UserId"]) : 0;
                            result.AccessId = Convert.ToInt32(dr["AccessId"]);
                            result.AccessType = dr["AccessType"].ToString();
                            result.UserName = dr["UserName"].ToString();
                            
                        }
                    }

                }
                return result;
            }
        }

        public List<credircardtypes> GetActiveCardTypes()
        {
            var list = new List<credircardtypes>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("usp_fn_GetCreditCardTypes", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new credircardtypes
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Name = dr["Name"].ToString()
                        });
                    }
                }
                return list;
            }
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

        public void StartWork(int userId)
        {
            TimeZoneInfo ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime loginTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ist);

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
IF NOT EXISTS(
    SELECT 1 FROM tblEmployeeLogHours
    WHERE UserId=@UserId
    AND CAST(LoginTime AS DATE)=CAST(@LoginTime AS DATE)
)
BEGIN
    INSERT INTO tblEmployeeLogHours(UserId,LoginTime,CreatedOn)
    VALUES(@UserId,@LoginTime,@LoginTime)
END";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@LoginTime", loginTime);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void EndWork(int userId)
        {
            TimeZoneInfo ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime logoutTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ist);

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
UPDATE tblEmployeeLogHours
SET 
LogoutTime=@LogoutTime,
TotalHours = DATEDIFF(MINUTE,LoginTime,@LogoutTime)/60.0
WHERE UserId=@UserId
AND CAST(LoginTime AS DATE)=CAST(@LogoutTime AS DATE)
AND LogoutTime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@LogoutTime", logoutTime);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Dictionary<string, string> GetTodayLog(int userId)
        {
            var result = new Dictionary<string, string>();

            TimeZoneInfo ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime todayIST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ist);

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
SELECT TOP 1 LoginTime,LogoutTime
FROM tblEmployeeLogHours
WHERE UserId=@UserId
AND CAST(LoginTime AS DATE)=CAST(@Today AS DATE)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Today", todayIST);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    result["loginTime"] = dr["LoginTime"]?.ToString();
                    result["logoutTime"] = dr["LogoutTime"]?.ToString();
                }
            }

            return result;
        }

        // GET HISTORY
        public List<object> GetLogHours(int userId)
        {
            var list = new List<object>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string query = @"
    SELECT 
    CAST(LoginTime AS DATE) Date,
    LoginTime,
    LogoutTime,
    TotalHours
    FROM tblEmployeeLogHours
    WHERE UserId=@UserId
    ORDER BY LoginTime DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new
                    {
                        date = Convert.ToDateTime(dr["Date"]).ToString("dd MMM yyyy"),
                        loginTime = Convert.ToDateTime(dr["LoginTime"]).ToString("hh:mm tt"),
                        logoutTime = dr["LogoutTime"] == DBNull.Value ? null :
                            Convert.ToDateTime(dr["LogoutTime"]).ToString("hh:mm tt"),
                        totalHours = dr["TotalHours"]?.ToString()
                    });
                }
            }

            return list;
        }
        public ResultResponse UpdateTransaction(UpdateTransactionModel model)
        {
            ResultResponse result = new ResultResponse();
            using SqlConnection con = new SqlConnection(_connectionString);
            con.Open();
            using SqlCommand cmd = new SqlCommand("usp_UpdateTransaction", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TransactionId", model.TransactionId);
            cmd.Parameters.AddWithValue("@TransactionAmount", model.TransactionAmount);
            cmd.Parameters.AddWithValue("@BillAmount", model.BillAmount);
            cmd.Parameters.AddWithValue("@EmployeeChargePercent", model.EmployeeChargePercent);
            cmd.Parameters.AddWithValue("@PlatformChargeAmount", model.PlatformChargeAmount);
            cmd.Parameters.AddWithValue("@ProfitAmount", model.ProfitAmount);
            cmd.Parameters.AddWithValue("@PayOut", model.PayOut);
            cmd.Parameters.AddWithValue("@FinalAmount", model.FinalAmount);
            cmd.Parameters.AddWithValue("@WalletAmount", model.WalletAmount);
            cmd.Parameters.AddWithValue("@ToBePaidByCustomer", model.ToBePaidByCustomer);
            cmd.Parameters.AddWithValue("@AccountAmountTransfer", model.AccountAmountTransfer);
            cmd.Parameters.AddWithValue("@CardAmountTransfer", model.CardAmountTransfer);
            cmd.Parameters.AddWithValue("@QrPayAmountTransfer", model.QrPayAmountTransfer);
            cmd.Parameters.AddWithValue("@AccountPayAmountTransfer", model.AccountPayAmountTransfer);
            cmd.Parameters.AddWithValue("@UpiPayAmountTransfer", model.UpiPayAmountTransfer);
            cmd.Parameters.AddWithValue("@OthersAmountTransfer", model.OthersAmountTransfer);
            cmd.Parameters.AddWithValue("@Difference", model.Difference);
            cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? "");
            cmd.Parameters.AddWithValue("@ChangedBy", model.ChangedBy);

            SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter msgParam = new SqlParameter("@StatusMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(resultParam);
            cmd.Parameters.Add(msgParam);
            cmd.ExecuteNonQuery();
            result.result = Convert.ToInt32(resultParam.Value);
            result.StatusMessage = msgParam.Value?.ToString();
            return result;
        }
    }
    }
