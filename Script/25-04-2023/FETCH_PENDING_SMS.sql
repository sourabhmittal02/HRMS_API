USE [AVVNL_CALL_CENTER]
GO
/****** Object:  StoredProcedure [dbo].[FETCH_PENDING_SMS]    Script Date: 25-04-2023 22:48:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[FETCH_PENDING_SMS]
AS
BEGIN
	SELECT TOP 100* FROM SMS_DETAIL Where REMARK='Pending'
END