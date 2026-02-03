using Azure;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using vnenterprises.Models;
using Microsoft.AspNetCore.Mvc;


namespace vnenterprises.Support
{
    public class UserSupport
    {
        private readonly string _connectionString;
        public UserSupport(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public int UpdateMpin( AccountModelHandle loginmodel , int UserId)
        {
            var result = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_UserUpdatePassword", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MobileNumber", loginmodel.mpin.PhoneNumber);
                cmd.Parameters.AddWithValue("@UserPassword", loginmodel.mpin.Password);
                cmd.Parameters.AddWithValue("@UserMpin", loginmodel.mpin.Mpin);
                cmd.Parameters.AddWithValue("@FlagType", 2);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result = dr["Status"] != DBNull.Value ?
                                                      Convert.ToInt32(dr["Status"]) : 0;
                        }
                    }

                }
                return result;
            }
        }

        public int UpdatePassword(AccountModelHandle loginmodel , int UserId)
        {
            var result = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_UserUpdatePassword", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@MobileNumber", loginmodel.forgotpassword.PhoneNumber);
                cmd.Parameters.AddWithValue("@UserPassword", loginmodel.forgotpassword.Password);
                cmd.Parameters.AddWithValue("@UserMpin", loginmodel.forgotpassword.MPIN);
                cmd.Parameters.AddWithValue("@FlagType", 1);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result = dr["Status"] != DBNull.Value ?
                                                      Convert.ToInt32(dr["Status"]) : 0;

                        }
                    }

                }
                return result;
            }
        }
        public LogInResponse GetUserAccess(AccountModelHandle loginmodel)
        {
            var result = new LogInResponse();
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("vn_UserSignInAccess", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MobileNumber", loginmodel.login.PhoneNumber);
                cmd.Parameters.AddWithValue("@UserPassword", loginmodel.login.Password);
                cmd.Parameters.AddWithValue("@UserMpin", loginmodel.login.MPIN);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result.UserId = dr["UserId"] != DBNull.Value ?
                                                      Convert.ToInt32(dr["UserId"]) : 0;
                            
                            result.AccessType = Convert.ToInt32(dr["AccessId"]);
                        }
                    }

                }
                return result;
            }
        }
    }
}
