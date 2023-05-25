﻿using Azure;
using CallCenterCoreAPI.ExternalAPI.TextSmsAPI;
using CallCenterCoreAPI.Models;
using CallCenterCoreAPI.Models.QueryModel;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace CallCenterCoreAPI.Database.Repository
{
    public class ComplaintRepository
    {
        private readonly ILogger<ComplaintRepository> _logger;
        private string conn = AppSettingsHelper.Setting(Key: "ConnectionStrings:DevConn");
        public ComplaintRepository(ILogger<ComplaintRepository> logger)
        {
            _logger = logger;
        }

        #region SaveComplaint
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="modelComplaint"></param>
        /// <returns></returns>
        public async Task<Int64> SaveComplaint(COMPLAINT modelComplaint)
        {
            Int64 retStatus = 0;
            string retMsg = String.Empty; ;
            COMPLAINT obj = new COMPLAINT();
            obj = modelComplaint;

            SqlParameter parmretStatus = new SqlParameter();
            parmretStatus.ParameterName = "@retStatus";
            parmretStatus.DbType = DbType.Int32;
            parmretStatus.Size = 8;
            parmretStatus.Direction = ParameterDirection.Output;

            SqlParameter parmretMsg = new SqlParameter();
            parmretMsg.ParameterName = "@retMsg";
            parmretMsg.DbType = DbType.String;
            parmretMsg.Size = 8;
            parmretMsg.Direction = ParameterDirection.Output;


            SqlParameter parmretComplaint_no = new SqlParameter();
            parmretComplaint_no.ParameterName = "@retComplaint_no";
            parmretComplaint_no.DbType = DbType.Int64;
            parmretComplaint_no.Size = 8;
            parmretComplaint_no.Direction = ParameterDirection.Output;
            SqlParameter[] param ={
                    new SqlParameter("@OFFICE_CODE",modelComplaint.OFFICE_CODE),
                    new SqlParameter("@COMPLAINT_TYPE",modelComplaint.ComplaintTypeId),
                    new SqlParameter("@COMPLAINT_SOURCE_ID",modelComplaint.sourceId),//modelComplaint.com),
                    new SqlParameter("@NAME",modelComplaint.NAME),
                    new SqlParameter("@FATHER_NAME",modelComplaint.FATHER_NAME),
                    new SqlParameter("@KNO",modelComplaint.KNO),
                    new SqlParameter("@LANDLINE_NO",modelComplaint.LANDLINE_NO),
                    new SqlParameter("@MOBILE_NO",modelComplaint.MOBILE_NO),
                    new SqlParameter("@ALTERNATE_MOBILE_NO",modelComplaint.ALTERNATE_MOBILE_NO),
                    new SqlParameter("@EMAIL",modelComplaint.EMAIL),
                    new SqlParameter("@ACCOUNT_NO",modelComplaint.ACCOUNT_NO),
                    new SqlParameter("@ADDRESS1",modelComplaint.ADDRESS1),
                    new SqlParameter("@ADDRESS2",modelComplaint.ADDRESS2),
                    new SqlParameter("@ADDRESS3",modelComplaint.ADDRESS3),

                    new SqlParameter("@LANDMARK",modelComplaint.LANDMARK),
                    new SqlParameter("@CONSUMER_STATUS",modelComplaint.CONSUMER_STATUS),
                    new SqlParameter("@FEEDER_NAME",modelComplaint.FEEDER_NAME),
                    new SqlParameter("@AREA_CODE",modelComplaint.AREA_CODE),
                    new SqlParameter("@REMARKS",modelComplaint.REMARKS),
                     new SqlParameter("@USER_ID",modelComplaint.UserId),
                    parmretStatus,parmretMsg,parmretComplaint_no};
            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "COMPLAINTS_REGISTER_API", param);

                if (param[22].Value != DBNull.Value)// status
                    retStatus = Convert.ToInt64(param[22].Value);
                if (retStatus > 0 && modelComplaint.MOBILE_NO.Length == 10)
                {
                    _logger.LogInformation(modelComplaint.MOBILE_NO.ToString());
                    ModelSmsAPI modelSmsAPI = new ModelSmsAPI();
                    modelSmsAPI.To = "91" + modelComplaint.MOBILE_NO.ToString();
                    modelSmsAPI.Smstext = "Dear Consumer,Your Complaint has been registered with complaint No. " + retStatus + " on Date: " + DateTime.Now.ToString("dd-MMM-yyyy") + " AVVNL";

                    TextSmsAPI textSmsAPI = new TextSmsAPI();
                    string response = await textSmsAPI.RegisterComplaintSMS(modelSmsAPI);
                    //modelComplaint.SMS = modelSmsAPI.Smstext;
                    _logger.LogInformation(response.ToString());

                    PUSH_SMS_DETAIL_Consumer(modelComplaint, response, modelSmsAPI.Smstext);

                }
                else
                    retStatus = 0;
            }
                catch (Exception ex)
            {
                retStatus = -1;
            }
            return retStatus;

        }
        #endregion

        #region SendSmsRep
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="modelComplaint"></param>
        /// <returns></returns>
        public async Task<string> SendSmsRep(SMSModel smsmodel)
        {
            string retStatus = "0";
            _logger.LogInformation(smsmodel.to.ToString());
            ModelSmsAPI modelSmsAPI = new ModelSmsAPI();
            modelSmsAPI.To = "91" + smsmodel.to.ToString();
            modelSmsAPI.Smstext = smsmodel.smsText;
            try
            {
                TextSmsAPI textSmsAPI = new TextSmsAPI();
                string response = await textSmsAPI.RegisterComplaintSMS(modelSmsAPI);
                //modelComplaint.SMS = modelSmsAPI.Smstext;
                _logger.LogInformation(response.ToString());

                UPDATE_SMS_DETAIL_Consumer(response, smsmodel.id);
                retStatus = response;
            }
            catch
            {
                retStatus = "0";
            }

            return retStatus;

        }
        #endregion

        #region SaveRemark
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="modelComplaint"></param>
        /// <returns></returns>
        public async Task<Int64> SaveRemark(RemarkModel modelRemark)
        {
            Int64 retStatus = 0;
            string retMsg = String.Empty; ;
            RemarkModel obj = new RemarkModel();
            obj = modelRemark;

            SqlParameter parmretStatus = new SqlParameter();
            parmretStatus.ParameterName = "@retStatus";
            parmretStatus.DbType = DbType.Int32;
            parmretStatus.Size = 8;
            parmretStatus.Direction = ParameterDirection.Output;

            SqlParameter parmretMsg = new SqlParameter();
            parmretMsg.ParameterName = "@retMsg";
            parmretMsg.DbType = DbType.String;
            parmretMsg.Size = 8;
            parmretMsg.Direction = ParameterDirection.Output;

            SqlParameter[] param ={
                new SqlParameter("@COMPLAINT_NO",modelRemark.ComplaintNo),
                    new SqlParameter("@REMARK",modelRemark.Remark),
                    new SqlParameter("@USER_ID",modelRemark.UserID),
                    parmretStatus,parmretMsg};


            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "SAVE_REMARK", param);

                if (param[3].Value != DBNull.Value)// status
                    retStatus = Convert.ToInt32(param[3].Value);
                else
                    retStatus = 0;
            }
            catch (Exception ex)
            {
                retStatus = -1;
            }



            return retStatus;

        }
        #endregion

        #region SaveCallDetail
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="modelComplaint"></param>
        /// <returns></returns>
        public async Task<Int64> SaveCallDetail(CallDetailModel modelRemark)
        {
            Int64 retStatus = 0;
            string retMsg = String.Empty; ;
            CallDetailModel obj = new CallDetailModel();
            obj = modelRemark;

            SqlParameter parmretStatus = new SqlParameter();
            parmretStatus.ParameterName = "@retStatus";
            parmretStatus.DbType = DbType.Int32;
            parmretStatus.Size = 8;
            parmretStatus.Direction = ParameterDirection.Output;

            SqlParameter parmretMsg = new SqlParameter();
            parmretMsg.ParameterName = "@retMsg";
            parmretMsg.DbType = DbType.String;
            parmretMsg.Size = 8;
            parmretMsg.Direction = ParameterDirection.Output;

            SqlParameter[] param ={
                new SqlParameter("@Date",modelRemark.date),
                    new SqlParameter("@Total_Calls_Offered",modelRemark.Total_Calls_Offered),
                    new SqlParameter("@Total_Calls_Answered",modelRemark.Total_Calls_Answered),
                    new SqlParameter("@Calls_Answered_within_60_Sec",modelRemark.Calls_Answered_within_60_Sec),
                    new SqlParameter("@Calls_Answered_After_60_Sec",modelRemark.Calls_Answered_After_60_Sec),
                    new SqlParameter("@Percent_Calls_Attended_within_60_Second",modelRemark.Percent_Calls_Attended_within_60_Sec),
                    new SqlParameter("@Percent_Calls_Attended_After_60_Second",modelRemark.Percent_Calls_Attended_After_60_Sec),
                    new SqlParameter("@Calls_Abandon",modelRemark.Calls_Abandon),
                    new SqlParameter("@Call_Abandon_Percentage",modelRemark.Call_Abandon_Percentage),
                    new SqlParameter("@Calls_Abandon_within_60_Sec",modelRemark.Calls_Abandon_within_60_Sec),
                    new SqlParameter("@Total_Call_Wait_Time",modelRemark.Total_Call_Wait_Time),
                    new SqlParameter("@Call_Wait_Time_more_than_60_Sec",modelRemark.Call_Wait_Time_more_than_60_Sec),
                    parmretStatus,parmretMsg};


            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "SAVE_CALL", param);

                if (param[12].Value != DBNull.Value)// status
                    retStatus = Convert.ToInt32(param[12].Value);
                else
                    retStatus = 0;
            }
            catch (Exception ex)
            {
                retStatus = -1;
            }



            return retStatus;

        }
        #endregion

        #region SearchComplaint
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="kno"></param>
        /// <returns></returns>
        public DataSet SearchComplaint(string kno)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] param = { new SqlParameter("@kno", kno) };
                ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "Search_Complaint", param);
            }
            catch (Exception ex)
            {

            }
            return ds;
        }
        #endregion

        #region GetPreviousComplaintByKno
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="Kno"></param>
        /// <returns></returns>
        public List<COMPLAINT_SEARCH> GetPreviousComplaintByKno(string Kno)
        {
            List<COMPLAINT_SEARCH> obj = new List<COMPLAINT_SEARCH>();
            SqlParameter[] param ={
                    new SqlParameter("@KNO",Kno) };

            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "SearchComplaintByKNo", param);
            //Bind Complaint generic list using dataRow     
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj.Add(

                    new COMPLAINT_SEARCH
                    {
                        //Consumer Info
                        //SDO_CODE = Convert.ToString(dr["SDO_CODE"]),

                        OFFICE_CODE = Convert.ToInt64(dr["OFFICE_CODE"]),
                        ComplaintType = Convert.ToString(dr["COMPLAINT_TYPE"]),
                        ComplaintNo = Convert.ToString(dr["COMPLAINT_NO"]),
                        NAME = Convert.ToString(dr["NAME"]),
                        FATHER_NAME = Convert.ToString(dr["FATHER_NAME"]),
                        KNO = Convert.ToString(dr["KNO"]),
                        LANDLINE_NO = Convert.ToString(dr["LANDLINE_NO"]),
                        MOBILE_NO = Convert.ToString(dr["MOBILE_NO"]),
                        ALTERNATE_MOBILE_NO = Convert.ToString(dr["ALTERNATE_MOBILE_NO"]),
                        source = Convert.ToString(dr["SOURCE_NAME"]),
                        ADDRESS = Convert.ToString(dr["ADDRESS"]),
                        Complaint_Status = Convert.ToString(dr["COMPLAINT_status"]),

                    }
                    );
            }
            return (obj);
        }
        #endregion

        #region GetPendingComplaintFRTWise
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="Kno"></param>
        /// <returns></returns>
        public List<COMPLAINT_SEARCH> GetPendingComplaintFRTWise(string offcieID)
        {
            List<COMPLAINT_SEARCH> obj = new List<COMPLAINT_SEARCH>();
            SqlParameter[] param ={
                    new SqlParameter("@offcieID",offcieID) };

            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "SearchComplaintByFTR", param);
            //Bind Complaint generic list using dataRow     
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj.Add(

                    new COMPLAINT_SEARCH
                    {
                        //Consumer Info
                        //SDO_CODE = Convert.ToString(dr["SDO_CODE"]),

                        OFFICE_CODE = Convert.ToInt64(dr["OFFICE_CODE"]),
                        ComplaintType = Convert.ToString(dr["COMPLAINT_TYPE"]),
                        ComplaintNo = Convert.ToString(dr["COMPLAINT_NO"]),
                        NAME = Convert.ToString(dr["NAME"]),
                        FATHER_NAME = Convert.ToString(dr["FATHER_NAME"]),
                        KNO = Convert.ToString(dr["KNO"]),
                        LANDLINE_NO = Convert.ToString(dr["LANDLINE_NO"]),
                        MOBILE_NO = Convert.ToString(dr["MOBILE_NO"]),
                        ALTERNATE_MOBILE_NO = Convert.ToString(dr["ALTERNATE_MOBILE_NO"]),
                        source = Convert.ToString(dr["SOURCE_NAME"]),
                        ADDRESS = Convert.ToString(dr["ADDRESS"]),
                        Complaint_Status = Convert.ToString(dr["COMPLAINT_status"]),

                    }
                    );
            }
            return (obj);
        }
        #endregion

        #region GetPreviousComplaintNo
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="complaintNo"></param>
        /// <returns></returns>
        public List<COMPLAINT_SEARCH> GetPreviousComplaintNo(string complaintNo)
        {
            List<COMPLAINT_SEARCH> obj = new List<COMPLAINT_SEARCH>();
            SqlParameter[] param ={
                    new SqlParameter("@Complaint_NO",complaintNo) };

            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "SearchComplaintByComplaintNo", param);
            //Bind Complaint generic list using dataRow     
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj.Add(

                    new COMPLAINT_SEARCH
                    {
                        //Consumer Info
                        //SDO_CODE = Convert.ToString(dr["SDO_CODE"]),

                        OFFICE_CODE = Convert.ToInt64(dr["OFFICE_CODE"]),
                        ComplaintType = Convert.ToString(dr["COMPLAINT_TYPE"]),
                        ComplaintNo = Convert.ToString(dr["COMPLAINT_NO"]),
                        NAME = Convert.ToString(dr["NAME"]),
                        FATHER_NAME = Convert.ToString(dr["FATHER_NAME"]),
                        KNO = Convert.ToString(dr["KNO"]),
                        LANDLINE_NO = Convert.ToString(dr["LANDLINE_NO"]),
                        MOBILE_NO = Convert.ToString(dr["MOBILE_NO"]),
                        ALTERNATE_MOBILE_NO = Convert.ToString(dr["ALTERNATE_MOBILE_NO"]),
                        source = Convert.ToString(dr["SOURCE_NAME"]),
                        ADDRESS = Convert.ToString(dr["ADDRESS"]),
                        Complaint_Status = Convert.ToString(dr["COMPLAINT_status"]),
                    }
                    );
            }
            return (obj);
        }
        #endregion

        #region GetOfficeList
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public List<ModelOfficeCode> GetOfficeList()
        {
            List<ModelOfficeCode> lstOfficeCode = new List<ModelOfficeCode>();
            ModelOfficeCode objBlank = new ModelOfficeCode();
            objBlank.OfficeId = "0";
            objBlank.OfficeCode = "Select Office Code";
            lstOfficeCode.Insert(0, objBlank);

            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetOfficeCode");

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                objBlank = new ModelOfficeCode();
                objBlank.OfficeCode = dr.ItemArray[0].ToString();
                objBlank.OfficeId = dr.ItemArray[1].ToString();
                lstOfficeCode.Add(objBlank);
            }
            return lstOfficeCode;
        }
        #endregion

        #region GetComplaintTypeList
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="OFFICE_ID"></param>
        /// <returns></returns>
        public List<ModelComplaintType> GetComplaintTypeList(string OFFICE_ID)
        {
            List<ModelComplaintType> obj = new List<ModelComplaintType>();
            SqlParameter[] param ={
                    new SqlParameter("@OFFICE_ID",OFFICE_ID)};
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetComplaintType", param);
            //Bind Complaint generic list using dataRow     
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj.Add(

                    new ModelComplaintType
                    {
                        ComplaintTypeId = Convert.ToInt32(dr["Id"]),
                        ComplaintType = Convert.ToString(dr["Complaint_Type"]),
                        ComplaintTileColor = Convert.ToString(dr["TileColor"]),
                        Status = Convert.ToBoolean(dr["IS_ACTIVE"]),
                        COMPLAINT_COUNT = Convert.ToString(dr["COMPLAINT_COUNT"]),
                    }
                    );
            }
            return (obj);
        }
        #endregion

        #region GetSubComplaintTypeList
        /// <summary>
        /// Save Complaint
        /// </summary>
        /// <param name="ComplaintTypeId"></param>
        /// <returns></returns>
        public List<ModelComplaintType> GetSubComplaintTypeList(int ComplaintTypeId)
        {
            List<ModelComplaintType> obj = new List<ModelComplaintType>();
            SqlParameter[] param ={
                    new SqlParameter("@ComplaintTypeId",ComplaintTypeId)};
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetSubComplaintByComplaintType", param);
            //Bind Complaint generic list using dataRow     
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj.Add(

                    new ModelComplaintType
                    {
                        SubComplaintTypeId = Convert.ToInt32(dr["Id"]),
                        SubComplaintType = Convert.ToString(dr["SUB_COMPLAINT_TYPE"]),
                        Status = Convert.ToBoolean(dr["IS_ACTIVE"]),
                    }
                    );
            }
            return (obj);
        }
        #endregion


        public int PUSH_SMS_DETAIL_Consumer(COMPLAINT modelRemark, string response,string SMS)
        {
            int retStatus = 0;
            string retMsg = String.Empty; ;
            COMPLAINT obj = new COMPLAINT();
            obj = modelRemark;
            SqlParameter[] param =
                {
                new SqlParameter("@PHONE_NO",modelRemark.MOBILE_NO),
                new SqlParameter("@TEXT_MEESAGE",SMS),
                new SqlParameter("@DELIVERY_RESPONSE",response),
                new SqlParameter("@REMARK","SMS SENT")};
            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "PUSH_SMS_DETAIL", param);
            }
            catch (Exception ex)
            {
                retStatus = -1;
            }

            return retStatus;

        }

        public int UPDATE_SMS_DETAIL_Consumer(string response, string id)
        {
            int retStatus = 0;
            string retMsg = String.Empty; ;
            SqlParameter[] param =
                {
                new SqlParameter("@id",id),
                new SqlParameter("@DELIVERY_RESPONSE",response)};
            try
            {
                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "UPDATE_SMS_DETAIL", param);
            }
            catch (Exception ex)
            {
                retStatus = -1;
            }

            return retStatus;

        }

        public async Task<int> AddKNO(KNOMODEL KnoDetail)
        {
            int retStatus = 0;
            SqlParameter parmretStatus = new SqlParameter();
            parmretStatus.ParameterName = "@retStatus";
            parmretStatus.DbType = DbType.Int32;
            parmretStatus.Size = 8;
            parmretStatus.Direction = ParameterDirection.Output;
            SqlParameter[] param ={
                    new SqlParameter("@USER_ID",KnoDetail.userid),
                    new SqlParameter("@KNO",KnoDetail.kno),
                    parmretStatus
                    };
            SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "Add_KNO", param);
            if (param[2].Value != DBNull.Value)// status
                retStatus = Convert.ToInt32(param[2].Value);
            else
                retStatus = 0;
            return retStatus;
        }

        public List<KNOMODEL> ListKNO(long userid)
        {
            List<KNOMODEL> obj = new List<KNOMODEL>();
            SqlParameter[] param ={
                    new SqlParameter("@USER_ID",userid)};
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "ListKNO", param);
            //Bind Complaint generic list using dataRow     
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj.Add(

                    new KNOMODEL
                    {
                        userid = userid,
                        kno = Convert.ToInt64(dr["KNO"]),
                    }
                    );
            }
            return (obj);
        }

        public async Task<int> UpdateDetail(ModelUser UserDetail)
        {
            int retStatus = 0;
            try
            {
                SqlParameter parmretStatus = new SqlParameter();
                parmretStatus.ParameterName = "@retStatus";
                parmretStatus.DbType = DbType.Int32;
                parmretStatus.Size = 8;
                parmretStatus.Direction = ParameterDirection.Output;
                long uid = Convert.ToInt64(UserDetail.User_id);
                SqlParameter[] param ={
                    new SqlParameter("@User_ID",uid),
                    new SqlParameter("@Name",UserDetail.Name),
                    new SqlParameter("@Address",UserDetail.Address),
                    new SqlParameter("@Email",UserDetail.Email),
                    new SqlParameter("@Phone",Convert.ToInt64(UserDetail.Mobile_NO)),
                    parmretStatus
                    };

                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "UpdateUsers", param);
                if (param[5].Value != DBNull.Value)// status
                    retStatus = Convert.ToInt32(param[5].Value);
                else
                    retStatus = 0;
            }
            catch (Exception ex)
            {
                retStatus = -1;
            }
            return retStatus;
        }

        public List<ModelUser> GetDetail(long userid)
        {
            List<ModelUser> obj = new List<ModelUser>();
            SqlParameter[] param ={
                    new SqlParameter("@USER_ID",userid)};
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetUsers", param);
            //Bind Complaint generic list using dataRow     
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj.Add(

                    new ModelUser
                    {
                        User_id = Convert.ToInt32(dr["USER_ID"]),
                        User_Name = Convert.ToString(dr["USER_NAME"]),
                        Name = Convert.ToString(dr["NAME"]),
                        Role = Convert.ToString(dr["ROLE_NAME"]),
                        Mobile_NO = Convert.ToInt64(dr["MOBILE_NO"]),
                        Email = Convert.ToString(dr["EMAIL_ID"]),
                        Address = Convert.ToString(dr["ADDRESS"]),
                    }
                    );
            }
            return (obj);
        }

        public List<COMPLAINT> GetKNODetailS(long KNO)
        {
            List<COMPLAINT> obj = new List<COMPLAINT>();
            SqlParameter[] param ={
                    new SqlParameter("@KNO",KNO)};
            DataSet ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, "GetKnoDetails", param);
            //Bind Complaint generic list using dataRow     
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                obj.Add(

                    new COMPLAINT
                    {
                        OFFICE_CODE = Convert.ToInt32(dr["OFFICE_CODE"]),
                        NAME = Convert.ToString(dr["Name"]),
                        FATHER_NAME = Convert.ToString(dr["FatherName"]),
                        KNO = Convert.ToString(dr["KNO"]),
                        MOBILE_NO = Convert.ToString(dr["MOBILE_NO"]),
                        LANDLINE_NO = Convert.ToString(dr["LANDLINENO"]),
                        EMAIL = Convert.ToString(dr["EMAIL_ADDRESS"]),
                        ACCOUNT_NO = Convert.ToString(dr["ACCOUNT_NO"]),
                        ADDRESS1 = Convert.ToString(dr["ADDRESS1"]),
                        ADDRESS2 = Convert.ToString(dr["ADDRESS2"]),
                        ADDRESS3 = Convert.ToString(dr["ADDRESS3"]),
                        LANDMARK = Convert.ToString(dr["LANDMARK"]),
                        CONSUMER_STATUS = Convert.ToString(dr["SERVICE_STATUS"]),
                        FEEDER_NAME = Convert.ToString(dr["FEEDER_NAME"]),
                        AREA_CODE = Convert.ToString(dr["AREA_CODE"]),
                    }
                    );
            }
            return (obj);
        }

    }
}
