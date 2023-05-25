﻿using CallCenterCoreAPI.Database.Repository;
using CallCenterCoreAPI.Models;
using CallCenterCoreAPI.Models.QueryModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CallCenterCoreAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ComplaintController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public ComplaintController(ILogger<ComplaintController> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        [HttpPost]
        [Route("SaveComplaint")]
        public async Task<IActionResult> SaveComplaint(COMPLAINT modelComplaint)
        {
            ReturnStatusModel returnStatus = new ReturnStatusModel();
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            Int64 retStatus = 0;
            string msg = string.Empty;
            DataSet dsResponse = modelComplaintRepository.SearchComplaint(modelComplaint.KNO);
            if (dsResponse.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt16(dsResponse.Tables[0].Rows[0]["COMPLAINT_status"].ToString()) != 2)
                {
                    _logger.LogInformation("You Already Have Complaint No." + dsResponse.Tables[0].Rows[0]["COMPLAINT_NO"].ToString() + " with Status " + dsResponse.Tables[0].Rows[0]["cst"]);
                    returnStatus.response = 0;
                    returnStatus.status = "You Already Have Complaint No. " + dsResponse.Tables[0].Rows[0]["COMPLAINT_NO"].ToString() + " with Status " + dsResponse.Tables[0].Rows[0]["cst"];
                    return BadRequest(returnStatus);
                }
            }
            else
            {
                retStatus = await modelComplaintRepository.SaveComplaint(modelComplaint);
            }
            if (retStatus > 0)
            {
                returnStatus.response = 1;
                returnStatus.status = "Complaint Successfully Registered With Complaint No. " + retStatus.ToString();
                return Ok(returnStatus);
            }
                
            else
            {
                returnStatus.response = 0;
                returnStatus.status = "Error in Saving Complaint";
                return BadRequest(returnStatus);
            }
                
           
        }

        [HttpPost]
        [Route("SearchComplaintByKNO")]
        public IActionResult SearchComplaintByKNO(ComplaintSearchQueryModel complaintSearchQueryModel)
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            List<COMPLAINT_SEARCH> lstComplaints = modelComplaintRepository.GetPreviousComplaintByKno(complaintSearchQueryModel.KNO);
            return Ok(lstComplaints);
        }

        [HttpPost]
        [Route("SearchComplaintByComplaintNo")]
        public IActionResult SearchComplaintByComplaintNo(ComplaintSearchQueryModel complaintSearchQueryModel)
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            List<COMPLAINT_SEARCH> lstComplaints = modelComplaintRepository.GetPreviousComplaintNo(complaintSearchQueryModel.ComplaintNo);
            return Ok(lstComplaints);
        }



        [HttpGet]
        [Route("GetOfficeList")]
        public IActionResult GetOfficeList()
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            List<ModelOfficeCode> lst;
            lst = modelComplaintRepository.GetOfficeList();
            return Ok(lst);
        }

        [HttpGet]
        [Route("GetComplaintTypeList")]
        public IActionResult GetComplaintTypeList(string officeid)
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);

            List<ModelComplaintType> obj;
            obj = modelComplaintRepository.GetComplaintTypeList(officeid);
            return Ok(obj);
        }

        [HttpGet]
        [Route("GetSubComplaintTypeList")]
        public IActionResult GetSubComplaintTypeList(int ComplaintTypeId)
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);

            List<ModelComplaintType> obj;
            obj = modelComplaintRepository.GetSubComplaintTypeList(ComplaintTypeId);
            return Ok(obj);
        }

        [HttpPost]
        [Route("Add_KNO")]
        public async Task<IActionResult> Add_KNO(KNOMODEL KnoDetail)
        {
            ReturnStatusModel returnStatus = new ReturnStatusModel();
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            int retStatus = await modelComplaintRepository.AddKNO(KnoDetail);
            if (retStatus ==1)
            {
                returnStatus.response = 1;
                returnStatus.status = "Kno Has been added Successfully";
                return Ok(returnStatus);
            }
            else if (retStatus == 2)
            {
                returnStatus.response = 1;
                returnStatus.status = "Invalid Kno";
                return Ok(returnStatus);
            }

            else
            {
                returnStatus.response = 0;
                returnStatus.status = "Error in Adding Kno or KNo Aready mapped with another User";
                return BadRequest(returnStatus);
            }

        }
        [HttpPost]
        [Route("ListKNO")]
        public IActionResult ListKNO(KNOMODEL KnoDetail)
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);

            List<KNOMODEL> obj;
            obj = modelComplaintRepository.ListKNO(KnoDetail.userid);
            return Ok(obj);
        }
        [HttpPost]
        [Route("UpdateDetail")]
        public async Task<IActionResult> UpdateDetail(ModelUser UserDetail)
        {
            ReturnStatusModel returnStatus = new ReturnStatusModel();
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            int retStatus = await modelComplaintRepository.UpdateDetail(UserDetail);
            if (retStatus > 0)
            {
                returnStatus.response = 1;
                returnStatus.status = "Detail Updated Successfully";
                return Ok(returnStatus);
            }

            else
            {
                returnStatus.response = 0;
                returnStatus.status = "Error in Updating Details";
                return BadRequest(returnStatus);
            }

        }
        [HttpPost]
        [Route("GetDetail")]
        public IActionResult GetDetail(KNOMODEL UserDetail)
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);

            List<ModelUser> obj;
            obj = modelComplaintRepository.GetDetail(UserDetail.userid);
            return Ok(obj);
        }

        [HttpPost]
        [Route("GetKNODetail")]
        public IActionResult GetKNODetail(KNOMODEL UserDetail)
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);

            List<COMPLAINT> obj;
            obj = modelComplaintRepository.GetKNODetailS(UserDetail.kno);
            return Ok(obj);
        }
        [HttpPost]
        [Route("GetFRTWiseComplaint")]
        public IActionResult GetFRTWiseComplaint(FRTWiseComplaintModel frtWiseComplaintModel)
        {
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            List<COMPLAINT_SEARCH> lstComplaints = modelComplaintRepository.GetPendingComplaintFRTWise(frtWiseComplaintModel.OfficeId);
            return Ok(lstComplaints);
        }

        [HttpPost]
        [Route("SaveRemark")]
        public async Task<IActionResult> SaveRemark(RemarkModel modelRemark)
        {
            ReturnStatusModel returnStatus = new ReturnStatusModel();
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            Int64 retStatus = 0;
            string msg = string.Empty;
            retStatus = await modelComplaintRepository.SaveRemark(modelRemark);
            if (retStatus > 0)
            {
                returnStatus.response = 1;
                returnStatus.status = "Remark has been saved";
                return Ok(returnStatus);
            }

            else
            {
                returnStatus.response = 0;
                returnStatus.status = "Error in Saving Remark";
                return BadRequest(returnStatus);
            }


        }

        [HttpPost]
        [Route("CallDetails")]
        public async Task<IActionResult> CallDetails(CallDetailModel modelRemark)
        {
            ReturnStatusModel returnStatus = new ReturnStatusModel();
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            Int64 retStatus = 0;
            string msg = string.Empty;
            retStatus = await modelComplaintRepository.SaveCallDetail(modelRemark);
            if (retStatus > 0)
            {
                returnStatus.response = 1;
                returnStatus.status = "Detail has been saved";
                return Ok(returnStatus);
            }

            else
            {
                returnStatus.response = 0;
                returnStatus.status = "Error in Saving Detail";
                return BadRequest(returnStatus);
            }


        }



        [HttpPost]
        [Route("SendSms")]
        public async Task<IActionResult> SendSms(SMSModel smsmodel)
        {
            ReturnStatusModel returnStatus = new ReturnStatusModel();
            ILogger<ComplaintRepository> modelLogger = _loggerFactory.CreateLogger<ComplaintRepository>();
            ComplaintRepository modelComplaintRepository = new ComplaintRepository(modelLogger);
            string retStatus = "0";
            string msg = string.Empty;
            retStatus = await modelComplaintRepository.SendSmsRep(smsmodel);
            if (retStatus == "0")
            {
                returnStatus.response = 0;
                returnStatus.status = "Error in Sending SMS";
                return BadRequest(returnStatus);
                
            }

            else
            {
                returnStatus.response = 1;
                returnStatus.status = retStatus;
                return Ok(returnStatus);
            }


        }

    }
    
}
