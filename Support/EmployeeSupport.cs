using Azure;
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
                                NameOnCard = dr["NameOnCard"]?.ToString() ?? "",
                                CardNumber = dr["CardNumber"]?.ToString() ?? "",
                                CardTypeId = dr["CardTypeId"] != DBNull.Value ? Convert.ToInt32(dr["CardTypeId"]) : 0,
                                CardCVV = dr["CardCVV"] != DBNull.Value ? Convert.ToInt32(dr["CardCVV"]) : 0,
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
                                BankId = dr["BankId"] != DBNull.Value ? Convert.ToInt32(dr["BankId"]) : 0,
                                BankName = dr["BankName"]?.ToString() ?? "",
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
        private DataTable ToIntListTable(List<CreditCardModel> list)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("CardTypeId", typeof(int));
            dt.Columns.Add("NameOnCard", typeof(string));
            dt.Columns.Add("CardNumber", typeof(string));
            dt.Columns.Add("ExprityDate", typeof(string));
            dt.Columns.Add("CardCVV", typeof(int));
            dt.Columns.Add("IsActive", typeof(bool));
            if(list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    dt.Rows.Add(
                        item.CardTypeId,     // int
                        item.NameOnCard,     // string
                        item.CardNumber,     // string
                        item.ExpiryDate,     // string
                        item.CardCVV,        // int
                        item.IsActive        // bool
                    );
                }
            }
            

            return dt;
        }
        private DataTable ToIntListTable(List<BanksDetails> list)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("BankId", typeof(int));
            dt.Columns.Add("BankName", typeof(string));
            dt.Columns.Add("AccountNumber", typeof(string));
            dt.Columns.Add("IFSCCode", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            if(list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    dt.Rows.Add(
                        item.BankId,     // int
                        item.BankName,     // string
                        item.AccountNumber,     // int
                        item.IFSCCode,        // string
                        item.IsActive        // bool
                    );
                }
            }
            

            return dt;//test
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
                        CustomerName = dr["CustomerName"].ToString(),
                        EmployeeName = dr["EmployeeName"].ToString(),
                        CardNumber = dr["CardNumber"].ToString(),
                        CardTypeName = dr["CardTypeName"].ToString(),
                        CardCVV = dr["CardCVV"].ToString(),
                        CardExpiryDate = dr["CardExpiryDate"].ToString(),
                        BankAccountNumber = dr["BankAccountNumber"].ToString(),
                        BankIFSC = dr["BankIFSC"].ToString(),
                        PlatformName = dr["PlatformName"].ToString(),
                        GatewayName = dr["GatewayName"].ToString(),
                        IncentiveName = dr["IncentiveName"].ToString(),
                        PlatformCharge = Convert.ToDecimal(dr["PlatformCharge"]),
                        GatewayCharge = Convert.ToDecimal(dr["GatewayCharge"]),
                        EmployeeChargePercent = Convert.ToDecimal(dr["EmployeeChargePercent"]),
                        EmployeeChargeAmount = Convert.ToDecimal(dr["EmployeeChargeAmount"]),
                        TransactionAmount = Convert.ToDecimal(dr["TransactionAmount"]),
                        PayOut = Convert.ToDecimal(dr["PayOut"] ?? 0.0) ,
                        FinalAmount = Convert.ToDecimal(dr["FinalAmount"]),
                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"]),
                        Remark = dr["Remark"].ToString()
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
    }
    }
