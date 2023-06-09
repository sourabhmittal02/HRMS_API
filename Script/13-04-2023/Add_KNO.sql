USE [CALL_CENTER_NEW]
GO
/****** Object:  StoredProcedure [dbo].[Add_KNO]    Script Date: 13-04-2023 09:26:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Add_KNO](
	@USER_ID int=0,
	@KNO bigint=0,
	@retStatus   INT OUTPUT
	)
AS
BEGIN
	DECLARE @count BIGINT
	SELECT @count=COUNT(*) FROM MST_KNO WHERE KNO = @KNO

	IF @count = 0
    BEGIN
		IF(SELECT COUNT(*) FROM CONSUMER WHERE KNO=@KNO)>0
		BEGIN
			INSERT INTO MST_KNO(USER_ID,KNO,IS_ACTIVE,IS_DELETED,TIME_STAMP) VALUES (@USER_ID, @KNO,1,0,GETDATE())
			set @retStatus=1
		END
		ELSE
		BEGIN
			set @retStatus=2
		END
    END
	else
		set @retStatus=0
END

