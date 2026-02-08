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

        private DataTable ToIntListTable(List<int> list)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var item in list)
                dt.Rows.Add(item);

            return dt;
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
                //cmd.Parameters.AddWithValue("@SelectedGateways", model.SelectedGateways);
                //cmd.Parameters.AddWithValue("@SelectedBranches", model.SelectedBranches);
                //cmd.Parameters.Add(new SqlParameter("@SelectedGateways", SqlDbType.Structured)
                //{
                //    TypeName = "dbo.IntList",
                //    Value = ToIntListTable(model.SelectedGateways)
                //});

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
        public ManagerModel GetManagerDetails(int ManagerId)
        {
            ManagerModel model = new ManagerModel();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_GetManagerDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ManagerId", ManagerId);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            model.ManagerId = dr["ManagerId"] != DBNull.Value ? Convert.ToInt32(dr["ManagerId"]) : 0;
                            model.UserFirstName = dr["UserFirstName"] != DBNull.Value ? Convert.ToString(dr["UserFirstName"]) : "";
                            model.UserLastName = dr["UserLastName"] != DBNull.Value ? Convert.ToString(dr["UserLastName"]) : "";
                            model.Password = dr["Password"] != DBNull.Value ? Convert.ToString(dr["Password"]) : "";
                            model.ConfirmPassword = dr["Password"] != DBNull.Value ? Convert.ToString(dr["Password"]) : "";
                            model.MPIN = dr["MPIN"] != DBNull.Value ? Convert.ToString(dr["MPIN"]) : "";
                            model.UserFirstName = dr["UserFirstName"] != DBNull.Value ? Convert.ToString(dr["UserFirstName"]) : "";
                            model.UserFirstName = dr["UserFirstName"] != DBNull.Value ? Convert.ToString(dr["UserFirstName"]) : "";
                            model.UserFirstName = dr["UserFirstName"] != DBNull.Value ? Convert.ToString(dr["UserFirstName"]) : "";
                            model.UserFirstName = dr["UserFirstName"] != DBNull.Value ? Convert.ToString(dr["UserFirstName"]) : "";
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
