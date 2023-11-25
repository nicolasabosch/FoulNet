using System;


using System.Linq;
using System.Net;


using System.Net.Mail;

using System.Reflection;

using FoulNet.Model;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace FoulNet.Library
{

    public class ToolsLib
    {

        private readonly Entities db;
        public ToolsLib(Entities context)
        {
            db = context;
        }

        public  void SendEMail(string receiverMail, string subject, string body)
        {
            SendEMail(new[] { receiverMail }, null , subject, body);
        }
        public  void SendEMail(string[] receiverMailList, string[] bccMailList,  string subject, string body)
        {


            //Visitar: https://www.google.com/settings/security/lesssecureapps
            MailMessage msg = new MailMessage();

            //Entities db = new Entities();



            var emailSenderAddress = db.Parameter.Find("EmailSenderAddress").ParameterValue;
            var emailSenderPassword = db.Parameter.Find("EmailSenderPassword").ParameterValue;
            var emailSenderDisplayName = db.Parameter.Find("EmailSenderDisplayName").ParameterValue;
            var emailHost = "smtp.gmail.com";
            var emailHostAux = db.Parameter.Where(X => X.ParameterID == "EmailHost").FirstOrDefault();
            if (emailHostAux != null)
            {
                emailHost = emailHostAux.ParameterValue;
            }


            msg.From = new MailAddress(emailSenderAddress, emailSenderDisplayName);
            foreach (var receiverMail in receiverMailList)
            {
                msg.To.Add(receiverMail);
            }

            if (bccMailList != null)
            {
                foreach (var bccMail in bccMailList)
                {
                    msg.Bcc.Add(bccMail);
                }
            }

            msg.IsBodyHtml = true;
            msg.Subject = subject;
            msg.Body = body;
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = emailHost;
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(emailSenderAddress, emailSenderPassword);
            client.Timeout = 20000;
            try
            {
                client.Send(msg);

            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                msg.Dispose();
            }
        }


        public System.Data.DataTable ConvertToDataTable(IQueryable queryResult)
        {
            System.Data.DataTable dtReturn = new System.Data.DataTable("Result");


            // column names
            PropertyInfo[] oProps = null;


            if (queryResult == null)
            {
                return dtReturn;
            }


            foreach (var rec in queryResult)
            {
                // Use reflection to get property names, to create table, Only first time, others
                //will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();


                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;


                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }


                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }


                DataRow dr = dtReturn.NewRow();


                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                }


                dtReturn.Rows.Add(dr);
            }


            return dtReturn;
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public bool VerifyPasword(string userID, string hashedPassword, string clearPassword)
        {
            try
            {
                PasswordHasher<string> pw = new PasswordHasher<string>();
                PasswordVerificationResult v2 = pw.VerifyHashedPassword(userID, hashedPassword, clearPassword);
                return v2 == PasswordVerificationResult.Success;
            }
            catch
            {
                return false;
            }
        }

   
        public  string hashPasword(string userID, string clearPassword)
        {

            PasswordHasher<string> pw = new PasswordHasher<string>();
            string hashedPassword = pw.HashPassword(userID, clearPassword);
            return hashedPassword;
        }

    }
}