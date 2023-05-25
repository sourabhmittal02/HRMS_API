USE [CALL_CENTER_NEW]
GO
/****** Object:  StoredProcedure [dbo].[SearchComplaintByComplaintNo]    Script Date: 04-04-2023 09:26:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 ALTER PROCEDURE [dbo].[SearchComplaintByComplaintNo]
@Complaint_NO VARCHAR(15)
AS
BEGIN
	SELECT A.OFFICE_CODE,B.COMPLAINT_TYPE,A.COMPLAINT_NO,A.NAME,A.FATHER_NAME,A.KNO,A.LANDLINE_NO,C.SOURCE_NAME,A.MOBILE_NO,A.ALTERNATE_MOBILE_NO,ISNULL(A.ADDRESS1,'')+' '+ISNULL(A.ADDRESS2,'')+' '+ISNULL(A.ADDRESS3,'')+' '+ISNULL(A.LANDMARK,'') ADDRESS,
	D.COMPLAINT_status
	FROM COMPLAINT(NOLOCK) A
	LEFT OUTER JOIN MST_COMPLAINT_TYPE(NOLOCK) B ON A.COMPLAINT_TYPE=B.ID
	LEFT OUTER JOIN MST_COMPLAINT_SOURCE(NOLOCK) C ON A.COMPLAINT_SOURCE_ID=C.ID
	LEFT OUTER JOIN MST_COMPLAINT_STEPS(NOLOCK) D ON A.COMPLAINT_status=D.ID
	WHERE COMPLAINT_NO=@Complaint_NO
END