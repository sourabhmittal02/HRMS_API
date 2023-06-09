USE [AVVNL_CALL_CENTER]
GO
/****** Object:  StoredProcedure [dbo].[SAVE_REMARK]    Script Date: 30-04-2023 00:03:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SAVE_CALL]
@Date datetime,
@Total_Calls_Offered bigint,
@Total_Calls_Answered bigint,
@Calls_Answered_within_60_Sec bigint,
@Calls_Answered_After_60_Sec bigint,
@Percent_Calls_Attended_within_60_Second decimal(18,2),
@Percent_Calls_Attended_After_60_Second decimal(18,2),
@Calls_Abandon bigint,
@Call_Abandon_Percentage decimal(18,2),
@Calls_Abandon_within_60_Sec bigint,
@Total_Call_Wait_Time bigint,
@Call_Wait_Time_more_than_60_Sec bigint,
@retStatus   INT OUTPUT,
@retMsg   VARCHAR (255) OUTPUT
AS
BEGIN
	INSERT INTO CallAbandonAndWaitin
	VALUES(@Date,@Total_Calls_Offered,@Total_Calls_Answered,@Calls_Answered_within_60_Sec,@Calls_Answered_After_60_Sec,@Percent_Calls_Attended_within_60_Second,@Percent_Calls_Attended_After_60_Second,
	@Calls_Abandon,@Call_Abandon_Percentage,@Calls_Abandon_within_60_Sec,@Total_Call_Wait_Time,@Call_Wait_Time_more_than_60_Sec,1,0,GETDATE())
	SELECT @retStatus = 1 , @retMsg='Remark Saved';
END

